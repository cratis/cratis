// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Concepts;

namespace Basic;

public record PersonId(Guid Value) : ConceptAs<Guid>(Value);
