using MainRobot.Http;
using MainRobot.Robot.ControllerWeb;
using MainRobot.Robot.Device.IpCam;
using MainRobot.Robot.Navigation;
using MainRobot.Robot.Navigation.Astar;
using MainRobot.Robot.Navigation.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MainRobot.TTS;
using Serilog;
using MainRobot.Robot.Battery;
using MainRobot.Robot.Navigation.Recharge;
using MainRobot.Robot.Comunication;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge.Interface;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot;
using MainRobot.Robot.Device.Lidar;
using MainRobot.Robot.Comunication.WebSocketTransport;
using MainRobot.WebSocketServer;

namespace MainRobot
{
    public class Program
    {
        private static async Task Main(string[] args)
        {


            // Crea una classe IHost che configura i servizi
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                    // Registra il servizio come transient
                    services.AddTransient<NavigationWebController>();

                    services.AddSingleton<IRobot, Robot.Robot>();
                    services.AddSingleton<INavigation, Navigation>();
                    services.AddSingleton<INavigationMover, NavigationMover>();
                    services.AddSingleton<ICommandComunication, CommandComunication>();
                    services.AddSingleton<ICommandQueue, CommandQueue>();
                    services.AddSingleton<IMovement, Movement>();
                    services.AddSingleton<ITextToSpeach, TextToSpeach>();
                    services.AddSingleton<IAstar, Astar>();
                    services.AddSingleton<IHelperInvisibleWall, HelperInvisibleWall>();
                    services.AddSingleton<IBatteryManager, BatteryManager>();
                    services.AddSingleton<ILidarManager, LidarManager>();
                    services.AddSingleton<IRechargeManager, RechargeManager>();


                    if (Configuration.FAKE_HW)
                        services.AddSingleton<IIpCam, FakeIpCam>();
                    else
                        services.AddSingleton<IIpCam, IpCam>();



                    ServiceRegistration.Register(services);
                })
                .Build();

            // Configurazione del WebSocket
            var webSocketService = host.Services.GetRequiredService<IWebSocketServer>();
            var webSocketHandler = host.Services.GetRequiredService<IWebSocketHandler>();
            webSocketService.SetHandler(webSocketHandler);

            // Create a LoggerConfiguration instance and use WriteTo.File to add the file sink
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.File("logs.txt");

            // Create a ILogger instance using the CreateLogger method of the configuration
            var logger = loggerConfig.CreateLogger();

            // Assign the logger to the static Log property
            Log.Logger = logger;

            var robot = host.Services.GetRequiredService<IRobot>();
            await robot.InitRobot();

            
            // Create an instance of the http server class with a prefix

            //HttpServer server = new HttpServer("http://localhost:5000/",host.Services);
            HttpServer server = new HttpServer("http://*:5000/", host.Services);

            // Start the server asynchronously without blocking the program execution
            server.StartAsync();

            Log.Logger.Information("Avvio Robot");

            await host.RunAsync();


        }
    }
}