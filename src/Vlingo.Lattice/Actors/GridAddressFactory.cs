// Copyright © 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Actors;
using Vlingo.Common.Identity;
using Vlingo.UUID;

namespace Vlingo.Lattice.Actors
{
    public class GridAddressFactory : IAddressFactory
    {
        private static IAddress Empty = new GridAddress(Guid.Empty, "(Empty)");
        
        private readonly IIdentityGenerator _generator;
        private IdentityGeneratorType _type;
        
        public GridAddressFactory(IdentityGeneratorType type)
        {
            _type = type;
            _generator = type.Generator();
        }

        public IAddress FindableBy<T>(T id) => 
            Guid.TryParse(id!.ToString(), out var parsed) ? 
                new GridAddress(parsed) :
                new GridAddress(long.Parse(id!.ToString()).ToGuid());

        public IAddress From(long reservedId, string name) =>
            Guid.TryParse(reservedId!.ToString(), out var parsed) ? 
                new GridAddress(parsed, name) :
                new GridAddress(long.Parse(reservedId!.ToString()).ToGuid(), name);

        public IAddress From(string idString) =>
            Guid.TryParse(idString, out var parsed) ? 
                new GridAddress(parsed) :
                new GridAddress(long.Parse(idString).ToGuid());

        public IAddress From(string idString, string name) =>
            Guid.TryParse(idString, out var parsed) ? 
                new GridAddress(parsed, name) :
                new GridAddress(long.Parse(idString).ToGuid(), name);

        public IAddress None() => Empty;

        public IAddress Unique() => new GridAddress(_generator.Generate());

        public IAddress UniquePrefixedWith(string prefixedWith) => new GridAddress(_generator.Generate(), prefixedWith, true);

        public IAddress UniqueWith(string? name) => new GridAddress(_generator.Generate(name!), name);

        public IAddress WithHighId() => throw new NotImplementedException("Unsupported for GridAddress.");

        public IAddress WithHighId(string name) => throw new NotImplementedException("Unsupported for GridAddress.");

        public long TestNextIdValue() => throw new NotImplementedException("Unsupported for GridAddress.");
    }
}