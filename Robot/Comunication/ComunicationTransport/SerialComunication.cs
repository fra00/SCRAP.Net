using MainRobot.Robot.Comunication.Model;
using MainRobot.Robot.Device;
using Serilog;
using System.Collections.Concurrent;

namespace MainRobot.Robot.Comunication.ComunicationTransport
{
    public class SerialComunication : IComunication
    {
        private ISerial serial;
        private ComunicationCommand? commandRunned;
        private TaskCompletionSource<string> tcs;

        private int LastId = 0;
        private int countTentative = 0;
        private void handlerMessage(string message)
        {
            string[] lineData = message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            if (!lineData.Any()) return;
            foreach (string line in lineData)
            {
                ParseCommandReceived(line);
                Log.Logger.Information($"Rpi-serial line received ({line})");
            }
        }

        private Action<ComunicationCommandReceived>? onEndCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        public SerialComunication(ISerial serial)
        {
            this.serial = serial;
            serial.OpenSerial(handlerMessage);
        }


        public void ParseCommandReceived(string line)
        {
            var arrayCmd = line.Split(Configuration.SERIAL_SEPARETOR );
            if (arrayCmd.First() != Configuration.SERIAL_START_MESSAGE_ARDU || arrayCmd.Length == 1) return;
            var commandReceived = new ComunicationCommandReceived(arrayCmd);

            //se currentCummand è null, vuol dire che non c'è nessun comando richiesto e quindi non sto 
            //aspettando nessuna risposta ma può essere arduino che invia dati ,valutare se serve , dovrebbe essere 
            //sempre raspberry che chiede i dati e non viceversa
            if (commandRunned != null && tcs!=null)
            {
                if (commandReceived.ReceivedCmd != null)
                {
                    //lettura della notifica di ricezione da parte di raspberry 
                    //il comando è 00 solo quando riceve una notifica di risposta
                    if (commandReceived.ReceivedCmd == MainRobot.Configuration.SERIAL_END_MESSAGE)
                    {
                        //in questo ho ottenuto una risposta di conferma ma gli id non corrispondono con il comando 
                        //corrente non sono nel comando giusto, c'è stato qualche problema
                        //non so se si verifica
                        if (commandReceived.ReceivedId != commandRunned.Id.ToString())
                        {
                            Log.Logger.Error($"Rpi-serial-Ardu response correct but wrong id current:{commandRunned.Id} received:{commandReceived.ReceivedId}");
                        }
                        commandRunned.ReceivedResponseFromArduino = true;
                        Log.Logger.Information($"Rpi-serial-Ardu response correct for Id {commandRunned.Id}");
                    }
                    else if (!commandRunned.ReceivedResponseFromArduino || commandReceived.ReceivedCmd == "000")
                    {
                        if (countTentative > 2)
                        {
                            //ho effettuato troppi tentativi arduino continua a richiedere la ricezione di notifica , c'è qualcosa
                            //di sbagliato nella comunicazione , dopo la terza volta smetto di provare
                            tcs.SetException(new Exception("Too many request notification"));
                            return;
                            //throw new Exception("Too many request notification");
                        }
                        //in questo caso non ho ottenuto ancora la notifica per la ricezione oppure se arriva "000" è una richiesta di 
                        //un nuovo invio di una notifica che non è stata ricevuta da arduino provo quindi ad effettuare un nuovo tentativo
                        countTentative += 1;
                        Log.Logger.Information($"Rpi-Notify tentative num {countTentative} form command {line}");
                        if (commandReceived.ReceivedCmd != "000")
                        {
                            commandRunned.CommandReceived = line;
                        }
                        notifyCommandReceived(commandReceived.ReceivedId);
                        //inviata la notifica di ricezione esco, devo aspettare la risposta
                        return;
                    }
                    if (commandRunned != null)
                        //ho ricevuto il messaggio e la notifica di ricezione 
                        tcs.SetResult(commandRunned.CommandReceived);
                }
                else
                {
                    //non so se si verifica mai questo caso ma è comunque gestito
                    commandRunned.Task.SetException(new Exception("Too many request notification"));
                    Log.Logger.Information("Command received but ReceivedCmd is null");
                }
            }
            else { notifyCommandReceived(commandReceived.ReceivedId); }
        }

        private void notifyCommandReceived(string receivedId)
        {
            try
            {
                serial.WriteLine($"{Configuration.SERIAL_START_MESSAGE_RPI}{Configuration.SERIAL_SEPARETOR}{receivedId}{Configuration.SERIAL_SEPARETOR}{Configuration.SERIAL_END_MESSAGE}");
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Rpi-notify error", ex);
                throw ex;
            }
        }


        public async Task<string> RunCommand(ComunicationCommand? currentCommand)
        {
            try
            {
                countTentative = 0;
                //set current command runned
                commandRunned = currentCommand;
                tcs = new TaskCompletionSource<string>();

                //send command to serial
                serial.WriteLine(currentCommand.Command);
                
                //await response from handler
                string commandString = await tcs.Task;
                
                //command is done clear current command
                commandRunned = null;
                tcs = null;
                return commandString;
            }
            catch (Exception ex)
            {
                commandRunned = null;
                Log.Logger.Error($"RPI-Error send", ex);
                throw ex;
            }
        }
    }
}