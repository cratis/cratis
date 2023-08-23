// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Identities;

namespace Aksio.Cratis.Kernel.MongoDB.Identities.for_MongoDBIdentityStore.when_getting_chain_for_single_identity;

public class and_it_does_not_exist : given.no_identities_registered
{
    Identity identity;
    IEnumerable<IdentityId> identities;

    void Establish() => identity = new Identity("Some subject", "Some name", "Some user name");

    async Task Because() => identities = await store.GetFor(identity);

    [Fact] void should_return_only_one_identity() => identities.Count().ShouldEqual(1);
    [Fact] void should_insert_the_identity() => inserted_identities.Count.ShouldEqual(1);
    [Fact] void should_insert_the_identity_with_the_correct_subject() => inserted_identities.First().Subject.ShouldEqual(identity.Subject);
    [Fact] void should_insert_the_identity_with_the_correct_name() => inserted_identities.First().Name.ShouldEqual(identity.Name);
    [Fact] void should_insert_the_identity_with_the_correct_user_name() => inserted_identities.First().UserName.ShouldEqual(identity.UserName);
}