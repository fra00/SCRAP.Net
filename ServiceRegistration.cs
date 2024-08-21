using MainRobot.Robot.ActionExec;
using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Comunication.ComunicationTransport.HttpClient;
using MainRobot.Robot.Comunication.WebSocketTransport;
using MainRobot.Robot.Device;
using MainRobot.Robot.LLM.HuggingFace;
using MainRobot.Robot.NaturalLanguage;
using MainRobot.Robot.Navigation;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge;
using MainRobot.Robot.Navigation.Recharge.Interface;
using MainRobot.Robot.Room;
using MainRobot.Robot.WebSocket;
using MainRobot.WebSocketServer;
using Microsoft.Extensions.DependencyInjection;

namespace MainRobot
{
    public static class ServiceRegistration
    {
        public static void Register(IServiceCollection services) {
            services.AddSingleton<IPathFinding, PathFinding>();

            //services.AddSingleton<IComunication, SerialComunication>();
            services.AddSingleton<IComunication, HttpComunication>();

            services.AddSingleton<IRechargeNavigation, RechargeNavigation>();

            //services.AddSingleton<IIntentRecognition, IntentRecognition>();
            services.AddSingleton<IIntentRecognition, LLMHuggingFace>();
            //services.AddSingleton<IIntentRecognition, LLMIntentRecognition>();

            services.AddSingleton<IRoomInfo, RoomInfo>();

            services.AddSingleton<IActionExec, ActionExec>();

            if (Configuration.FAKE_HW)
                services.AddSingleton<IHttpClientComunication, FakeHttpClientComunication>();
            else
            {
                services.AddSingleton<IHttpClientComunication, HttpClientComunication>();
                //services.AddSingleton<IHttpClientComunication, TcpClientComunication>();
            }

            //if (Configuration.FAKE_HW)
            //    services.AddSingleton<ISerial, FakeSerial>();
            //else
            //    services.AddSingleton<ISerial, Serial>();

            //for mobile comunication
            services.AddSingleton<IWebSocketServer, WebSocketServer.WebSocketServer>();
            services.AddSingleton<IWebSocketHandler,RobotWebSocketHandler>();
            services.AddSingleton<IWebSocketCommand, WebSocketCommand>();


            services.AddSingleton<ILLMRestCall, LLMHFCall>();
        }
    }
}
