using MainRobot.Robot.Comunication.WebSocketTransport;

public interface IWebSocketHandler {
    WebSocketOutputData? Exec(WebSocketOutputData? dataReceived);
}

