﻿using NUnit.Framework;
using StyletIoC;
using System;

namespace StyletUnitTests.StyletIoC;

[TestFixture]
public class StyletIoCBindingChecksTests
{
    private interface I1 { }

    private class C1 : I1 { }

    private class C2 { }

    private interface I3 : I1 { }

    private abstract class C4 : I1 { }

    private class C5<T> : I1 { }

    private interface I6<T> { }

    private class C6<T> : I6<T> { }

    private interface I7<T1, T2> { }

    private class C7<T1, T2> { }

    private class C8 : I6<int> { }

    [Test]
    public void ThrowsIfTypeDoesNotImplementService()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind<I1>().To<C2>());
    }

    [Test]
    public void ThrowsIfImplementationIsNotConcrete()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind<I1>().To<I3>());
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind<I1>().To<C4>());
    }

    [Test]
    public void ThrowsIfUnboundGenericServiceBoundToNormalImplementation()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind(typeof(I6<>)).To<C6<int>>());
    }

    [Test]
    public void ThrowsINonGenericServiceBoundToNormalImplementation()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind(typeof(I6<>)).To<C8>());
    }

    [Test]
    public void ThrowsIfNormalServiceBoundToUnboundGenericService()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind<I6<int>>().To(typeof(C6<>)));
    }

    [Test]
    public void ThrowsIfUnboundTypesHaveDifferentNumbersOfTypeParameters()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind(typeof(I6<>)).To(typeof(C7<,>)));
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind(typeof(I7<,>)).To(typeof(C6<>)));
    }

    [Test]
    public void ThrowsIfFactoryBoundToUnboundGeneric()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.Bind(typeof(I6<>)).ToFactory(x => new C6<int>()));
    }

    [Test]
    public void ThrowsIfRegistrationFactoryIsNUll()
    {
        var builder = new StyletIoCBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.Bind<C1>().ToSelf().WithRegistrationFactory(null));
    }

    [Test]
    public void AllowsFactoryToProvideInterfaceType()
    {
        var builder = new StyletIoCBuilder();
        Assert.DoesNotThrow(() => builder.Bind<I1>().ToFactory<I1>(c =>new C1()));
    }

    [Test]
    public void AllowsInstanceToBeInterfaceType()
    {
        var builder = new StyletIoCBuilder();
        I1 i1 = new C1();
        Assert.DoesNotThrow(() => builder.Bind<I1>().ToInstance(i1));
    }

    [Test]
    public void ThrowsIfMissingBuilderBinding()
    {
        var builder = new StyletIoCBuilder();
        builder.Bind<C1>();
        Assert.Throws<StyletIoCRegistrationException>(() => builder.BuildContainer());
    }
}
