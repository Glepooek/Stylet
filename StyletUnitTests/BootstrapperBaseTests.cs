﻿#if NETFRAMEWORK

using Moq;
using NUnit.Framework;
using Stylet;
using System;
using System.Threading;
using System.Windows;

namespace StyletUnitTests;

[TestFixture, Apartment(ApartmentState.STA)]
public class BootstrapperBaseTests
{
    private class RootViewModel : IDisposable
    {
        public bool DisposeCalled;

        public void Dispose()
        {
            this.DisposeCalled = true;
        }
    }

    private class MyBootstrapperBase : BootstrapperBase
    {
        private readonly IViewManager viewManager;
        private readonly IWindowManager windowManager;

        public MyBootstrapperBase(IViewManager viewManager, IWindowManager windowManager)
        {
            this.viewManager = viewManager;
            this.windowManager = windowManager;

            this.Start(new string[0]);
        }

        public new Application Application => base.Application;

        public readonly RootViewModel MyRootViewModel = new();

        public bool GetInstanceCalled;
        public override object GetInstance(Type service)
        {
            this.GetInstanceCalled = true;
            if (service == typeof(IViewManager))
                return this.viewManager;
            if (service == typeof(IWindowManager))
                return this.windowManager;
            return null;
        }

        public bool LaunchCalled;
        protected override void Launch()
        {
            this.LaunchCalled = true;
        }

        public bool OnLaunchCalled;
        protected override void OnLaunch()
        {
            this.OnLaunchCalled = true;
        }

        public bool OnStartCalled;
        protected override void OnStart()
        {
            this.OnStartCalled = true;
        }

        public bool OnExitCalled;
        protected override void OnExit(ExitEventArgs e)
        {
            this.OnExitCalled = true;
        }

        public bool ConfigureBootstrapperCalled;
        protected override void ConfigureBootstrapper()
        {
            this.ConfigureBootstrapperCalled = true;
            base.ConfigureBootstrapper();
        }

        public new void Start(string[] args)
        {
            base.Start(args);
        }

        public new void DisplayRootView(object rootViewModel)
        {
            base.DisplayRootView(rootViewModel);
        }
    }

    private class FakeDispatcher : IDispatcher
    {
        public void Post(Action action)
        {
            throw new NotImplementedException();
        }

        public void Send(Action action)
        {
            throw new NotImplementedException();
        }

        public bool IsCurrent => throw new NotImplementedException();
    }

    
    private MyBootstrapperBase bootstrapper;
    private Mock<IViewManager> viewManager;
    private Mock<IWindowManager> windowManager;

    private IDispatcher dispatcher;

    [SetUp]
    public void SetUp()
    {
        this.dispatcher = Execute.Dispatcher;
        this.viewManager = new Mock<IViewManager>();
        this.windowManager = new Mock<IWindowManager>();
        this.bootstrapper = new MyBootstrapperBase(this.viewManager.Object, this.windowManager.Object);
    }

    [TearDown]
    public void TearDown()
    {
        Execute.Dispatcher = this.dispatcher;
    }

    [Test]
    public void SetupThrowsIfApplicationIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => this.bootstrapper.Setup(null));
    }

    [Test]
    public void StartCallsConfigureBootstrapper()
    {
        this.bootstrapper.Start(new string[0]);
        Assert.True(this.bootstrapper.ConfigureBootstrapperCalled);
    }

    [Test]
    public void StartAssignsArgs()
    {
        this.bootstrapper.Start(new[] { "one", "two" });
        Assert.That(this.bootstrapper.Args, Is.EquivalentTo(new[] { "one", "two" }));
    }

    [Test]
    public void StartCallsLaunch()
    {
        this.bootstrapper.Start(new string[0]);
        Assert.True(this.bootstrapper.LaunchCalled);
    }

    [Test]
    public void StartCallsOnLaunch()
    {
        this.bootstrapper.Start(new string[0]);
        Assert.True(this.bootstrapper.OnLaunchCalled);
    }

    [Test]
    public void DisplayRootViewDisplaysTheRootView()
    {
        object viewModel = new();
        this.bootstrapper.DisplayRootView(viewModel);

        this.windowManager.Verify(x => x.ShowWindow(viewModel));
    }
}

#endif