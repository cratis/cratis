// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Cratis.Chronicle.Concepts.Events;
using Cratis.Chronicle.Concepts.Identities;
using Cratis.Chronicle.Concepts.Keys;
using Cratis.Chronicle.Dynamic;
using Cratis.Chronicle.Properties;
using Cratis.Chronicle.Storage.EventSequences;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cratis.Chronicle.Projections.for_KeyResolvers;

public class when_identifying_model_key_from_parent_hierarchy_with_one_level : Specification
{
    AppendedEvent _rootEvent;
    AppendedEvent _event;
    Key _result;
    IProjection _rootProjection;
    IProjection _childProjection;
    IEventSequenceStorage _storage;
    KeyResolvers _keyResolvers;

    static EventType root_event_type = new("5f4f4368-6989-4d9d-a84e-7393e0b41cfd", 1);
    const string parent_key = "61fcc353-3478-4cf9-a783-da508013b36f";

    void Establish()
    {
        _keyResolvers = new KeyResolvers(NullLogger<KeyResolvers>.Instance);
        _rootEvent = new(
            new(1, root_event_type),
            new(
                EventSourceType.Default,
                "2f005aaf-2f4e-4a47-92ea-63687ef74bd4",
                EventStreamType.All,
                EventStreamId.Default,
                0,
                DateTimeOffset.UtcNow,
                "123b8935-a1a4-410d-aace-e340d48f0aa0",
                "41f18595-4748-4b01-88f7-4c0d0907aa90",
                CorrelationId.New(),
                [],
                Identity.System),
            new ExpandoObject());

        _event = new(
            new(0,
            new("02405794-91e7-4e4f-8ad1-f043070ca297", 1)),
            new(
                EventSourceType.Default,
                "2f005aaf-2f4e-4a47-92ea-63687ef74bd4",
                EventStreamType.All,
                EventStreamId.Default,
                0,
                DateTimeOffset.UtcNow,
                "123b8935-a1a4-410d-aace-e340d48f0aa0",
                "41f18595-4748-4b01-88f7-4c0d0907aa90",
                CorrelationId.New(),
                [],
                Identity.System),
            new
            {
                parentId = parent_key
            }.AsExpandoObject());

        _rootProjection = Substitute.For<IProjection>();
        _rootProjection.EventTypes.Returns(
        [
            root_event_type
        ]);

        _childProjection = Substitute.For<IProjection>();
        _childProjection.HasParent.Returns(true);
        _childProjection.Parent.Returns(_rootProjection);
        _childProjection.ChildrenPropertyPath.Returns((PropertyPath)"children");
        _storage = Substitute.For<IEventSequenceStorage>();

        _storage.TryGetLastInstanceOfAny(parent_key, [root_event_type.Id]).Returns(_rootEvent);
        _rootProjection.GetKeyResolverFor(root_event_type).Returns(_ => (_, __) => Task.FromResult(new Key(parent_key, ArrayIndexers.NoIndexers)));
    }

    async Task Because() => _result = await _keyResolvers.FromParentHierarchy(
        _childProjection,
        _keyResolvers.FromEventSourceId,
        _keyResolvers.FromEventValueProvider(EventValueProviders.EventContent("parentId")),
        "childId")(_storage, _event);

    [Fact] void should_return_expected_key() => _result.Value.ShouldEqual(parent_key);
}
