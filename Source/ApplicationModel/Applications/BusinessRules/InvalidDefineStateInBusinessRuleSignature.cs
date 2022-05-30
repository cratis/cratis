// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Applications.BusinessRules;

/// <summary>
/// Exception that gets thrown when the signature of `DefineState` on a business rule is wrong.
/// </summary>
public class InvalidDefineStateInBusinessRuleSignature : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDefineStateInBusinessRuleSignature"/> class.
    /// </summary>
    /// <param name="type">Type that has wrong method signature.</param>
    public InvalidDefineStateInBusinessRuleSignature(Type type) : base($"Invalid `DefineState` signature for '{type.FullName}'. Expected a public or non public instance method signature 'void DefineState(IProjectionBuilderFor<{type.Name}>)'")
    {
    }
}
