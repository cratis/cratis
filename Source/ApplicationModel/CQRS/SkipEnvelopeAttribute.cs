// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Applications;

/// <summary>
/// Attribute for indicating that we want to skip the envelope and return raw from a controller.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SkipEnvelopeAttribute : Attribute
{
}
