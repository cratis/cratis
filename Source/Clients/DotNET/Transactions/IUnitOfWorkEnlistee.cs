// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Transactions;

/// <summary>
/// Defines a unit of work enlistee.
/// </summary>
public interface IUnitOfWorkEnlistee
{
    /// <summary>
    /// Called when the <see cref="IUnitOfWork"/> is completed.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task Completed();

    /// <summary>
    /// Called when the <see cref="IUnitOfWork"/> is rolled back.
    /// </summary>
    /// <returns>Awaitable task.</returns>
    Task RolledBack();
}
