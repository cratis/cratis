// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ProtoBuf;

namespace Cratis.Kernel.Contracts.Projections;

/// <summary>
/// Represents the definition for a collection of property actions to perform for all events in the projection.
/// </summary>
[ProtoContract]
public class AllDefinition
{
    /// <summary>
    /// Gets or sets the properties and expressions for each property.
    /// </summary>
    [ProtoMember(1)]
    public IDictionary<string, string> Properties { get; set; }

    /// <summary>
    /// Gets or sets whether or not to include event types from child projections.
    /// </summary>
    [ProtoMember(2)]
    public bool IncludeChildren { get; set; }
}
