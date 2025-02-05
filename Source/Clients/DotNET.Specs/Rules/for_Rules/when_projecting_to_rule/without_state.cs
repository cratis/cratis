// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Models;

namespace Cratis.Chronicle.Rules.for_Rules.when_projecting_to_rule;

public class without_state : given.no_rules
{
    const string model_identifier = "282c491b-10a9-4ec0-ae23-659c4e6aaf16";
    RuleWithoutState rule;

    void Establish()
    {
        rule = new();
    }

    void Because() => rules.ProjectTo(rule, model_identifier);

    [Fact] void should_not_get_instance_from_immediate_projection() => _projections.DidNotReceive().GetInstanceById(rule.GetRuleId().Value, Arg.Any<ModelKey>());
}
