// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Kernel.Orleans.Configuration;

/// <summary>
/// Represents the different telemetry collector types.
/// </summary>
public static class TelemetryTypes
{
    /// <summary>
    /// Gets the value representing none.
    /// </summary>
    public const string None = "none";

    /// <summary>
    /// Gets the value representing Azure Application Insights.
    /// </summary>
    public const string AppInsights = "app-insights";

    /// <summary>
    /// Gets the value representing Open Telemetry.
    /// </summary>
    public const string OpenTelemetry = "open-telemetry";
}
