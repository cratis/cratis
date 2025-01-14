// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Integration.Base;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Concepts;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Domain.Interfaces;
using Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Events;
using context = Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Scenarios.when_there_are_events_to_rehydrate.and_performing_action_that_appends_event.context;

namespace Cratis.Chronicle.Integration.Orleans.InProcess.AggregateRoots.Scenarios.when_there_are_events_to_rehydrate;

[Collection(GlobalCollection.Name)]
public class and_performing_action_that_appends_event(context context) : Given<context>(context)
{
    public class context(GlobalFixture globalFixture) : given.context_for_aggregates_with_single_unit_of_work(globalFixture)
    {
        UserId _userId;
        public UserName UserName;
        public bool UserExists;
        public bool IsNew;
        public IUser User;
        public UserInternalState ResultState;

        async Task Establish()
        {
            UserName = "some-name";
            _userId = Guid.NewGuid();
            EventsWithEventSourceIdToAppend.Add(new EventAndEventSourceId(_userId.Value, new UserCreated()));
            User = await AggregateRootFactory.Get<IUser>(_userId.Value);
        }

        async Task Because()
        {
            await PerformInUnitOfWork(async () => await User.Onboard(UserName));
            UserExists = await User.Exists();
            ResultState = await User.GetState();
            IsNew = await User.GetIsNew();
            await UnitOfWork.Commit();
        }
    }

    [Fact]
    void should_not_be_new_aggregate() => Context.IsNew.ShouldBeFalse();

    [Fact]
    void should_return_that_user_exists() => Context.UserExists.ShouldBeTrue();

    [Fact]
    void should_commit_unit_of_work_successfully() => Context.UnitOfWork.IsSuccess.ShouldBeTrue();

    [Fact]
    void should_have_completed_unit_of_work() => Context.UnitOfWork.IsCompleted.ShouldBeTrue();

    [Fact]
    void should_commit_a_single_event() => Context.UnitOfWork.GetEvents().ShouldContainSingleItem();

    [Fact]
    void should_not_be_deleted() => Context.ResultState.Deleted.ShouldEqual(new(false, 0));

    [Fact]
    void should_have_assigned_username_to_internal_state() => Context.ResultState.Name.ShouldEqual(new(Context.UserName, 1));
}
