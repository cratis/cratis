// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Events.Store.Observation;
using Cratis.Execution;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;

namespace Cratis.Events.Store.Grains.Observation
{
    /// <summary>
    /// Represents an implementation of <see cref="IPartitionedObserver"/>.
    /// </summary>
    [StorageProvider(ProviderName = FailedObserverState.StorageProvider)]
    public class PartitionedObserver : Grain<FailedObserverState>, IPartitionedObserver, IRemindable
    {
        const string RecoverReminder = "partitioned-observer-failure-recovery";

        ObserverId _observerId = ObserverId.Unspecified;
        IAsyncStream<AppendedEvent>? _stream;
        IGrainReminder? _recoverReminder;
        TenantId _tenantId = TenantId.NotSet;
        EventSourceId _eventSourceId = EventSourceId.Unspecified;
        EventLogId _eventLogId = EventLogId.Unspecified;

        /// <inheritdoc/>
        public override async Task OnActivateAsync()
        {
            _observerId = this.GetPrimaryKey(out var key);
            var (tenantId, eventLogId, eventSourceId) = PartitionedObserverKeyHelper.Parse(key);
            _tenantId = tenantId;
            _eventLogId = eventLogId;
            _eventSourceId = eventSourceId;

            var streamProvider = GetStreamProvider("observer-handlers");
            _stream = streamProvider.GetStream<AppendedEvent>(_observerId, null);

            _recoverReminder = await GetReminder(RecoverReminder);
            if (State.IsFailed)
            {
                if (_recoverReminder == default)
                {
                    await HandleReminderRegistration();
                }
            }
            else if (_recoverReminder != default)
            {
                await UnregisterReminder(_recoverReminder);
            }

            await base.OnActivateAsync();
        }

        /// <inheritdoc/>
        public async Task OnNext(AppendedEvent @event, IEnumerable<EventType> eventTypes)
        {
            if (State.IsFailed)
            {
                return;
            }

            await HandleEvent(@event, eventTypes);
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (reminderName == RecoverReminder)
            {
                var reminder = await GetReminder(RecoverReminder);

                if (reminder == default)
                {
                    return;
                }

                await UnregisterReminder(reminder);

                var streamProvider = GetStreamProvider(EventLog.StreamProvider);

                var eventLogStream = streamProvider.GetStream<AppendedEvent>(_eventLogId, _tenantId.ToString());
                StreamSubscriptionHandle<AppendedEvent>? subscriptionId = null;

                subscriptionId = await eventLogStream.SubscribeAsync(
                    async (@event, _) =>
                    {
                        await HandleEvent(@event, State.EventTypes);

                        if (!State.IsFailed)
                        {
                            State.IsFailed = false;
                            await WriteStateAsync();
                            await subscriptionId!.UnsubscribeAsync();
                        }
                    },
                    new EventLogSequenceNumberTokenWithFilter(State.SequenceNumber, State.EventTypes, _eventSourceId));
            }
        }

        public async Task TryResume()
        {
            // Get the event log stream for this tenant
            // Use a stream sequence token that has partition in it
            // Subscribe to the stream with this token
            // When stream is at the edge - unsubscribe to the stream

            await Task.CompletedTask;
        }

        async Task HandleEvent(AppendedEvent @event, IEnumerable<EventType> eventTypes)
        {
            try
            {
                await _stream!.OnNextAsync(@event);
                State.IsFailed = false;
                await WriteStateAsync();
            }
            catch (Exception ex)
            {
                State.IsFailed = true;
                State.Occurred = DateTimeOffset.UtcNow;
                State.SequenceNumber = @event.Metadata.SequenceNumber;
                State.StackTrace = ex.StackTrace ?? string.Empty;
                State.EventTypes = eventTypes;
                State.Messages = GetMessagesFromException(ex);
                State.Attempts++;
                await WriteStateAsync();

                await HandleReminderRegistration();
            }
        }

        async Task HandleReminderRegistration()
        {
            if (State.Attempts <= 10)
            {
                _recoverReminder = await RegisterOrUpdateReminder(
                    RecoverReminder,
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(60) * State.Attempts);
            }
            else
            {
                var reminder = await GetReminder(RecoverReminder);
                if (reminder != null)
                {
                    await UnregisterReminder(reminder);
                }
            }
        }

        string[] GetMessagesFromException(Exception ex)
        {
            var messages = new List<string>
                {
                    ex.Message
                };

            while (ex.InnerException != null)
            {
                messages.Insert(0, ex.InnerException.Message);
                ex = ex.InnerException;
            }

            return messages.ToArray();
        }
    }
}
