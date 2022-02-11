// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Lattice.Model;
using Vlingo.Xoom.Symbio;
using Xunit;

namespace Vlingo.Xoom.Lattice.Tests.Model;

public class ApplyFailedExceptionTest
{
    [Fact]
    public void TestThatFailedHasApplicable()
    {
        var applicable = new Applicable<object>(null, Enumerable.Empty<ISource>(), Metadata.NullMetadata(), CompletionSupplier<object>.SupplierOrNull<object>(() => null, null));
        var e = new ApplyFailedException<object>(applicable);

        Assert.NotNull(e);
        Assert.NotNull(e.Applicable);
        Assert.Null(e.Applicable.State);
        Assert.NotNull(e.Applicable.Sources);
        Assert.NotNull(e.Applicable.Metadata);
        Assert.NotNull(e.Applicable.CompletionSupplier);
        Assert.Equal("Exception of type 'Vlingo.Xoom.Lattice.Model.ApplyFailedException`1[System.Object]' was thrown.", e.Message);
        Assert.Null(e.InnerException);
    }
        
    [Fact]
    public void TestThatFailedHasExceptionInfo()
    {
        var cause = new Exception("TestInner", new Exception());
        var applicable = new Applicable<object>(null, Enumerable.Empty<ISource>(), Metadata.NullMetadata(), CompletionSupplier<object>.SupplierOrNull<object>(() => null, null));
        var e = new ApplyFailedException<object>(applicable, "TestOuter", cause);

        Assert.NotNull(e);
        Assert.NotNull(e.Applicable);
        Assert.Equal("TestOuter", e.Message);
        Assert.NotNull(e.InnerException);
        Assert.Equal("TestInner", e.InnerException.Message);
        Assert.NotNull(e.Message);
        Assert.NotNull(e.InnerException.InnerException);
        Assert.Equal("Exception of type 'System.Exception' was thrown.", e.InnerException.InnerException.Message);
    }
}