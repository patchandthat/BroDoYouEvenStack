using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Autofac;
using Bootstrapper.Interface.UI;
using Bootstrapper.Interface.Util;
using Caliburn.Micro;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using StartupEventArgs = System.Windows.StartupEventArgs;

namespace Bootstrapper.Interface
{
    public class CaliburnMicroBootstrapper : BootstrapperBase
    {
        private IContainer _container;
        private readonly BootstrapperApplication _app;

        public CaliburnMicroBootstrapper(BootstrapperApplication app)
        {
            LogManager.GetLog = type => new WixLog(app.Engine, type);

            this._app = app;

            Initialize();
            SetupContainer();
        }

        private void SetupContainer()
        {
            var builder = new ContainerBuilder();

            RegisterCaliburnMicroFramework(builder);

            builder.RegisterInstance<BootstrapperApplication>(_app).SingleInstance();

            _container = builder.Build();
        }

        private void RegisterCaliburnMicroFramework(ContainerBuilder builder)
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.Name.EndsWith("ViewModel") || t.Name.EndsWith("View"))
                   .AsSelf()
                   .InstancePerDependency();

            builder.RegisterType<DotaDirectoryLocator>().AsSelf().InstancePerLifetimeScope();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var settings = new Dictionary<string, object>
            {
                { "SizeToContent", SizeToContent.Manual },
                { "Height" , 350  },
                { "Width"  , 600 },
                //{ "ResizeMode", ResizeMode.NoResize }
            };
            DisplayRootViewFor<ShellViewModel>(settings);
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (_container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (_container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }

        protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }
    }
}
