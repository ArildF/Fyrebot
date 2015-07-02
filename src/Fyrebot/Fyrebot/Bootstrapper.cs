using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Facilities.Startable;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Raven.Client;
using Raven.Client.Embedded;
using ReactiveUI;
using Rogue.Fyrebot.Properties;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	class Bootstrapper
	{
		private readonly WindsorContainer _container;

		public static IMessageBus Bus;

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
			_container.Register(Component.For<CommandMap>());

			_container.Register(Component.For<IMessageBus>().ImplementedBy<MessageBus>().LifestyleSingleton());
			_container.Register(Component.For<IConsole>().ImplementedBy<FyrebotConsole>().LifestyleSingleton());
			_container.Register(Component.For<ICampfireRoom>().ImplementedBy<CampfireRoom>().LifestyleTransient());
			_container.Register(Component.For<ISettings, Rogue.MetroFire.CampfireClient.ISettings>()
				.Instance(Settings.Default));

			_container.Register(AllTypes.FromAssemblyInDirectory(
				new AssemblyFilter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)))
				.BasedOn<IAmNotifiedOnLogin>()
				.WithServiceAllInterfaces()
				.LifestyleSingleton());

			_container.Register(AllTypes.FromAssemblyInDirectory(
				new AssemblyFilter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)))
				.BasedOn<IFyreModule>()
				.WithService.AllInterfaces()
				.LifestyleSingleton());

			_container.Register(AllTypes.FromAssemblyInDirectory(
				new AssemblyFilter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)))
				.BasedOn<IStartable>()
				.WithServiceSelf()
				.LifestyleSingleton());

			

			_container.Register(Component.For<IDocumentStore>().Instance(InitializeDocumentStore()));

			_container.Install(FromAssembly.This());
			_container.Install(FromAssembly.Containing<RequestLoginMessage>());

			Bus = _container.Resolve<IMessageBus>();


			return _container.Resolve<LoginProcessor>();
		}

		private IDocumentStore InitializeDocumentStore()
		{
			var store = new EmbeddableDocumentStore
			{
				DataDirectory = "data",
				UseEmbeddedHttpServer = true
			};
			store.Initialize();
			return store;
		}
	}
}
