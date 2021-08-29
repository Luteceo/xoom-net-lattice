// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Lattice.Model
{
    /// <summary>
    /// Abstract base of all entity types.
    /// </summary>
    public abstract class EntityActor : Actor
    {
        /// <summary>
        /// Restore my state.
        /// </summary>
        protected abstract void Restore();
    }
}