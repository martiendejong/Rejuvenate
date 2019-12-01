[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CardGame.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CardGame.NinjectWebCommon), "Stop")]

namespace CardGame
{
    using System;
    using System.Web;
    using AutoMapper;
    using CardGame.Models;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var gameContext = new GameContext();// @"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            //var gameContext = new GameContext();// @"Server=.\SQLEXPRESS64; Database=RejuvenatingTests; Integrated Security=True;");
            //kernel.Bind<IExampleContext>().To<ExampleContext>().InSingletonScope();
            //kernel.Bind<IGameContext>().ToMethod((a) => gameContext).InSingletonScope();
            kernel.Bind<IGameContext>().ToConstant(gameContext);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Game, GameViewModel>()
                    .ForMember(gameViewModel => gameViewModel.Id, opt => opt.MapFrom(game => game.Id))
                    .ForMember(gameViewModel => gameViewModel.Players, opt => opt.MapFrom(game => game.Players));
                cfg.CreateMap<Game, Game>();
            });
            var mapper = config.CreateMapper();
            kernel.Bind<IMapper>().ToConstant(mapper);
        }
    }
}
