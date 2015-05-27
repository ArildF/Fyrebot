using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Facilities.Startable;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ReactiveUI;
using Rogue.Fyrebot.Properties;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	class Bootstrapper
	{
		private readonly WindsorContainer _container;

		public Bootstrapper()
		{
			_container = new WindsorContainer();
		}

		public LoginProcessor Bootstrap()
		{
			_container.Kernel.ComponentModelBuilder.RemoveContributor(
				_container.Kernel.ComponentModelBuilder.Contributors.OfType<PropertiesDependenciesModelInspector>().Single());

			_container.AddFacility<StartableFacility>(f => f.DeferredTryStart());
			_container.AddFacility<TypedFactoryFacility>();

			_container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));


			_container.Register(Component.For<LoginProcessor>());

			_container.Register(Component.For<IMessageBus>().ImplementedBy<MessageBus>().LifestyleSingleton());
			_container.Register(Component.For<IConsole>().ImplementedBy<FyrebotConsole>().LifestyleSingleton());
			_container.Register(Component.For<ISettings, Rogue.MetroFire.CampfireClient.ISettings>()
				.Instance(Settings.Default));

			_container.Register(AllTypes.FromAssemblyInDirectory(
				new AssemblyFilter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)))
				.BasedOn<IAmNotifiedOnLogin>()
				.WithServiceAllInterfaces()
				.LifestyleSingleton());

			_container.Install(FromAssembly.This());
			_container.Install(FromAssembly.Containing<RequestLoginMessage>());

			return _container.Resolve<LoginProcessor>();
		}
	}
}
