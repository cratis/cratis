// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Copyright (c) Cratis. All rights reserved.

using Orleans;

namespace Cratis.Events.Store.Grains.Observation
{
    /// <summary>
    /// Defines a partitioned observer.
    /// </summary>
    public interface IPartitionedObserver : IGrainWithGuidCompoundKey
    {
        /// <summary>
        /// Handle the next event.
        /// </summary>
        /// <param name="observer"><see cref="IObserverHandler"/> that will be handling each event.</param>
        /// <param name="event">The actual event.</param>
        Task<bool> OnNext(IObserverHandler observer, AppendedEvent @event);
        Task ReportStatus();
    }
}
