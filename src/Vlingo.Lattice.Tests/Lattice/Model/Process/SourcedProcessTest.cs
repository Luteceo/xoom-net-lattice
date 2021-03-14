// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Actors;
using Vlingo.Common.Message;
using Vlingo.Lattice.Exchange;
using Vlingo.Lattice.Exchange.Local;
using Vlingo.Lattice.Model.Process;
using Vlingo.Lattice.Model.Sourcing;
using Vlingo.Symbio;
using Vlingo.Symbio.Store.Journal;
using Vlingo.Symbio.Store.Journal.InMemory;
using Vlingo.Tests.Lattice.Model.Sourcing;
using Xunit;
using Xunit.Abstractions;

namespace Vlingo.Tests.Lattice.Model.Process
{
    public class SourcedProcessTest : IDisposable
    {
        private readonly IExchange _exchange;
        private readonly IJournal<string> _journal;
        private readonly ExchangeReceivers _exchangeReceivers;
        private readonly LocalExchangeSender _exchangeSender;
        private readonly SourcedTypeRegistry _sourcedTypeRegistry;
        private readonly MockJournalDispatcher _dispatcher;
        private readonly World _world;

        [Fact]
        public void TestFiveStepSendingProcess()
        {
            var process = _world.ActorFor<IFiveStepProcess>(() => new FiveStepSendingSourcedProcess());
            _exchangeReceivers.SetProcess(process);
            
            _exchange.Send(new DoStepOne());

            Assert.Equal(5, _exchangeReceivers.Access.ReadFrom<int>("stepCount"));

            Assert.Equal(5, process.QueryStepCount().Await());
        }
        
        [Fact]
        public void TestFiveStepEmittingProcess()
        {
            var process = _world.ActorFor<IFiveStepProcess>(() => new FiveStepEmittingSourcedProcess());
            _exchangeReceivers.SetProcess(process);
            var listenerAccess = _dispatcher.AfterCompleting(4);
            
            _exchange.Send(new DoStepOne());

            Assert.Equal(5, _exchangeReceivers.Access.ReadFrom<int>("stepCount"));

            Assert.Equal(5, process.QueryStepCount().Await());
            
            Assert.Equal(4, listenerAccess.ReadFrom<int>("entriesCount"));
        }

        public SourcedProcessTest(ITestOutputHelper output)
        {
            var converter = new Converter(output);
            Console.SetOut(converter);
            
            _world = World.StartWithDefaults("five-step-process-test");

            var queue = new AsyncMessageQueue(null);
            _exchange = new LocalExchange(queue);
            _dispatcher = new MockJournalDispatcher();
            _journal = new InMemoryJournal<string>(_dispatcher, _world);
            
            _sourcedTypeRegistry = new SourcedTypeRegistry(_world);
            
            RegisterSourcedTypes<FiveStepSendingSourcedProcess>();
            RegisterSourcedTypes<FiveStepEmittingSourcedProcess>();

            _exchangeReceivers = new ExchangeReceivers();
            _exchangeSender = new LocalExchangeSender(queue);

            var processTypeRegistry = new ProcessTypeRegistry(_world);
            processTypeRegistry.Register(new SourcedProcessInfo<FiveStepSendingSourcedProcess>(nameof(FiveStepSendingSourcedProcess), _exchange, _sourcedTypeRegistry));
            processTypeRegistry.Register(new SourcedProcessInfo<FiveStepEmittingSourcedProcess>(nameof(FiveStepEmittingSourcedProcess), _exchange, _sourcedTypeRegistry));
            
            RegisterExchangeCoveys();
        }
        
        private void RegisterExchangeCoveys()
        {
            _exchange
                .Register(Covey<DoStepOne, DoStepOne, LocalExchangeMessage>.Of(
                    _exchangeSender,
                    _exchangeReceivers.DoStepOneReceiver,
                    new LocalExchangeAdapter<DoStepOne, DoStepOne>()))
            .Register(Covey<DoStepTwo, DoStepTwo, LocalExchangeMessage>.Of(
                _exchangeSender,
                _exchangeReceivers.DoStepTwoReceiver,
                new LocalExchangeAdapter<DoStepTwo, DoStepTwo>()))
            .Register(Covey<DoStepThree, DoStepThree, LocalExchangeMessage>.Of(
                _exchangeSender,
                _exchangeReceivers.DoStepThreeReceiver,
                new LocalExchangeAdapter<DoStepThree, DoStepThree>()))
            .Register(Covey<DoStepFour, DoStepFour, LocalExchangeMessage>.Of(
                _exchangeSender,
                _exchangeReceivers.DoStepFourReceiver,
                new LocalExchangeAdapter<DoStepFour, DoStepFour>()))
            .Register(Covey<DoStepFive, DoStepFive, LocalExchangeMessage>.Of(
                _exchangeSender,
                _exchangeReceivers.DoStepFiveReceiver,
                new LocalExchangeAdapter<DoStepFive, DoStepFive>()));
        }
        
        private void RegisterSourcedTypes<TSourced>()
        {
            var entryAdapterProvider = EntryAdapterProvider.Instance(_world);

            _sourcedTypeRegistry.Register(Vlingo.Lattice.Model.Sourcing.Info.RegisterSourced<TSourced>(_journal));

            _sourcedTypeRegistry.Info<TSourced>()?
                .RegisterEntryAdapter(new ProcessMessageTextAdapter(), 
                    adapter => entryAdapterProvider.RegisterAdapter(adapter))
                .RegisterEntryAdapter(new DoStepOneAdapter(),
                    adapter => entryAdapterProvider.RegisterAdapter(adapter))
                .RegisterEntryAdapter(new DoStepTwoAdapter(),
                    adapter => entryAdapterProvider.RegisterAdapter(adapter))
                .RegisterEntryAdapter(new DoStepThreeAdapter(),
                    adapter => entryAdapterProvider.RegisterAdapter(adapter))
                .RegisterEntryAdapter(new DoStepFourAdapter(),
                    adapter => entryAdapterProvider.RegisterAdapter(adapter))
                .RegisterEntryAdapter(new DoStepFiveAdapter(),
                    adapter => entryAdapterProvider.RegisterAdapter(adapter));
        }

        public void Dispose() => _world.Terminate();
    }
}