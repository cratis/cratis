// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using Polly.Retry;
using Polly.Timeout;
namespace Cratis.Chronicle.Setup;

/// <summary>
/// The options for <see cref="ResilientGrainStorage"/>.
/// </summary>
public class ResilientStorageOptions
{
    /// <summary>
    /// The <see cref="RetryStrategyOptions"/>.
    /// </summary>
    [Required]
    public RetryStrategyOptions Retry { get; set; } = new();

    /// <summary>
    /// The <see cref="TimeoutStrategyOptions"/>.
    /// </summary>
    [Required]
    public TimeoutStrategyOptions Timeout { get; set; } = new();
}