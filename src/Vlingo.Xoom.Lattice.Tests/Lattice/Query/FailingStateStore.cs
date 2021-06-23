// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Vlingo.Xoom.Common;
using Vlingo.Xoom.Symbio;
using Vlingo.Xoom.Symbio.Store;
using Vlingo.Xoom.Symbio.Store.State;

namespace Vlingo.Tests.Lattice.Query
{
    public class FailingStateStore : IStateStore
    {
        private readonly IStateStore _delegate;
        private readonly AtomicInteger _readCount = new AtomicInteger(0);
        private readonly AtomicInteger _expectedReadFailures = new AtomicInteger(0);

        public FailingStateStore(IStateStore @delegate) => _delegate = @delegate;

        public void Read<TState>(string id, IReadResultInterest interest)
        {
            throw new System.NotImplementedException();
        }

        public void Read<TState>(string id, IReadResultInterest interest, object? @object)
        {
            if (_readCount.IncrementAndGet() > _expectedReadFailures.Get())
            {
                _delegate.Read<TState>(id, interest, @object);
            }
            else
            {
                interest.ReadResultedIn<IOutcome<StorageException, Result>>(Failure.Of<StorageException, Result>(new StorageException(Result.NotFound, "Not found.")), id, null, -1, null, @object);
            }
        }

        public void ReadAll<TState>(IEnumerable<TypedStateBundle> bundles, IReadResultInterest interest, object? @object)
        {
            _readCount.IncrementAndGet();
            _delegate.ReadAll<TState>(bundles, interest, @object);
        }

        public void Write<TState>(string id, TState state, int stateVersion, IWriteResultInterest interest)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState, TSource>(string id, TState state, int stateVersion, IEnumerable<TSource> sources, IWriteResultInterest interest)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState>(string id, TState state, int stateVersion, Metadata metadata, IWriteResultInterest interest)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState, TSource>(string id, TState state, int stateVersion, IEnumerable<TSource> sources, Metadata metadata, IWriteResultInterest interest)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState>(string id, TState state, int stateVersion, IWriteResultInterest interest, object @object)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState, TSource>(string id, TState state, int stateVersion, IEnumerable<TSource> sources, IWriteResultInterest interest, object @object)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState>(string id, TState state, int stateVersion, Metadata metadata, IWriteResultInterest interest, object? @object)
        {
            throw new System.NotImplementedException();
        }

        public void Write<TState, TSource>(string id, TState state, int stateVersion, IEnumerable<TSource> sources, Metadata metadata, IWriteResultInterest interest, object? @object) => 
            _delegate.Write(id, state, stateVersion, sources, metadata, interest, @object);

        public ICompletes<IStateStoreEntryReader> EntryReader<TEntry>(string name) where TEntry : IEntry => 
            _delegate.EntryReader<TEntry>(name);
    }
}