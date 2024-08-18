using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Comunication.Model;
using MainRobot.Robot.CustomException;
using Serilog;
using System.Diagnostics;

namespace MainRobot.Robot.Comunication
{
    public class CommandQueue : ICommandQueue
    {
        private IComunication comunication;

        //private SynchronizedCollection<ComunicationCommand> commandsQueue = new SynchronizedCollection<ComunicationCommand>();
        //private ConcurrentQueue<ComunicationCommand> commandsQueue = new ConcurrentQueue<ComunicationCommand>();
        //private ComunicationCommand? currentCommand;
        private Timer timer;

        private int LastId = 0;

        private Action<ComunicationCommandReceived>? onEndCommand { get; set; }

        public CommandQueue(IComunication comunication)
        {
            this.comunication = comunication;
        }



        private async Task<ComunicationCommandReceived> runCommand(ComunicationCommand? currentCommand)
        {
            try
            {
                currentCommand.SendDate = DateTime.Now;
                currentCommand.SendedToArduino = true;
                int maxTentative = 3;
                var commandString = "";
                ComunicationCommandReceived res = null;
                while (maxTentative >= 0)
                {
                    if (maxTentative == 0)
                    {
                        throw new ExceptionRunCommand($"Run Command {currentCommand}");
                    }
                    commandString = await comunication.RunCommand(currentCommand);
                    //null not is a valid value , if is null retry
                    if (commandString == null)
                    {
                        maxTentative -= 1;
                        continue;
                    }
                    res = await parseCommandReceived(commandString);

                    if (res == null || res.ReceivedParam1 == "INVALID-COMMAND")
                    {
                        maxTentative -= 1;
                        continue;
                    }
                    maxTentative = -1;
                    break;
                }
                currentCommand.ReceivedResponseFromArduino = true;

                Log.Logger.Information($"Rpi-serial line received ({commandString})");

                if (currentCommand != null) Log.Logger.Information($"rpi send to arduino {currentCommand.AliasCommand}");
                return res;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"RPI-Error send", ex);
                throw ex;
            }
            
        }

        private async Task<ComunicationCommandReceived> parseCommandReceived(string line)
        {
            var arrayCmd = line.Split(Configuration.SERIAL_SEPARETOR);
            if (arrayCmd.First() != Configuration.SERIAL_START_MESSAGE_ARDU || arrayCmd.Length == 1) return null;
            var commandReceived = new ComunicationCommandReceived(arrayCmd);

            //Log.Logger.Information($"Rpi-serial-Ardu response correct for Id {currentCommand.Id}");

            //ComunicationCommandReceived commandExec = new ComunicationCommandReceived(currentCommand.CommandReceived.Split("|"));
            onEndCommand?.Invoke(commandReceived);
            //var endTask = currentCommand.Task;
            //endTask.SetResult(commandExec);
            //return commandExec;
            return commandReceived;
        }


        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public async Task<ComunicationCommandReceived> Enqueue(string command, string logDescriptionCmd)
        {
            Log.Logger.Information($"RPI-Command-{LastId}-{command}-added to queue ({logDescriptionCmd})");

            var semaphore = new System.Threading.SemaphoreSlim(1);

            int incremented = Interlocked.Increment(ref LastId); // incrementa l'id in modo thread safe
            //#Rpi|{incremented}|{command}
            string commandText = $"{Configuration.SERIAL_START_MESSAGE_RPI}{Configuration.SERIAL_SEPARETOR}{incremented}{Configuration.SERIAL_SEPARETOR}{command}";
            ComunicationCommand cmd = new ComunicationCommand
            {
                Id = incremented,
                Command = commandText,
                AliasCommand = logDescriptionCmd,
            };

            ComunicationCommandReceived r = null;
            //a cosa serve?
            //await Task.Delay(500);
            //40 secondi di timeout
            bool notTimeout = await _semaphore.WaitAsync(1000 * 40);
            if (notTimeout)
            {
                try
                {
                    Console.WriteLine("**************************************************");
                    Console.WriteLine($"send command for {cmd.AliasCommand}");
                    var task = runCommand(cmd);
                    var taskTimeout = Task.Delay(1000 * 40);
                    var taskAny = await Task.WhenAny(task, taskTimeout);
                    if (taskAny == task)
                    {
                        return await task;
                    }
                    else
                    {
                        throw new TimeoutException($"The operation {cmd.Command} has timed out.");
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                throw new TimeoutException("timeout_command");
            }

            return r;
        }
    }
}
