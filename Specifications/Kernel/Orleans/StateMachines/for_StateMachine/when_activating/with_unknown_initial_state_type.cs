// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Orleans.StateMachines.when_activating;

public class with_unknown_initial_state_type : given.a_state_machine
{
    Exception exception;


    protected override Type initial_state => typeof(StateThatDoesNotSupportTransitioningFrom);

    protected override IEnumerable<IState<StateMachineState>> CreateStates() => new IState<StateMachineState>[]
        {
            new StateThatSupportsTransitioningFrom()
        };

    async Task Because() => exception = await Catch.Exception(async () => await state_machine.OnActivateAsync(CancellationToken.None));

    [Fact] void should_throw_invalid_type_for_state_exception() => exception.ShouldBeOfExactType<UnknownStateTypeInStateMachine>();
    [Fact] async Task should_set_state_machine_on_all_states() => (await state_machine.GetStates()).All(_ => _.StateMachine.Equals(state_machine)).ShouldBeTrue();
}
