// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Transactions;

/// <summary>
/// Defines a system that can manage <see cref="IUnitOfWork">units of work</see>.
/// </summary>
public interface IUnitOfWorkManager
{
    /// <summary>
    /// Gets the current <see cref="IUnitOfWork"/>, scoped to the current async context.
    /// </summary>
    /// <exception cref="NoUnitOfWorkHasBeenStarted">If no unit of work has been started.</exception>
    IUnitOfWork Current { get; }

    /// <summary>
    /// Gets a value indicating whether or not there is a current <see cref="IUnitOfWork"/>.
    /// </summary>
    bool HasCurrent { get; }

    /// <summary>
    /// Begin a new <see cref="IUnitOfWork"/> with a specific <see cref="CorrelationId"/> for the current async context.
    /// </summary>
    /// <param name="correlationId">The <see cref="CorrelationId"/> to use for the <see cref="IUnitOfWork"/>. </param>
    /// <returns>A new <see cref="IUnitOfWork"/>.</returns>
    IUnitOfWork Begin(CorrelationId correlationId);
}
