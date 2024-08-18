using System.IO.Ports; // Importa il namespace
using Serilog;

namespace MainRobot.Robot.Device
{
    public class Serial : ISerial
    {
        private SerialPort serialPort { get; set; }


        public void OpenSerial(Action<string> dataReceivedHandler)
        {
            // Crea un oggetto SerialPort
            serialPort = new SerialPort(Configuration.SERIAL_NAME, Configuration.SERIAL_SPEED);

            serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); // Aggiunge l'handler per l'evento DataReceived
            serialPort.Open(); // Apre la porta

            // Definisce l'handler per l'evento DataReceived
            void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                string message = serialPort.ReadLine(); // Legge una linea di testo dal buffer
                dataReceivedHandler(message);
                Log.Logger.Information("Messaggio ricevuto: " + message);
            }
        }

        public void OpenSerialByte(string serialName,int speed, Action<byte[]> dataReceivedHandler)
        {
            // Crea un oggetto SerialPort
            serialPort = new SerialPort(serialName, speed);

            serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); // Aggiunge l'handler per l'evento DataReceived
            serialPort.Open(); // Apre la porta

            // Definisce l'handler per l'evento DataReceived
            void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                //int message = serialPort.ReadByte(); 
                //dataReceivedHandler(message);

                byte[] buffer = new byte[serialPort.BytesToRead];
                serialPort.Read(buffer, 0, buffer.Length);

                dataReceivedHandler(buffer);
            }
        }

        public void WriteLine(string message)
        {
            serialPort.WriteLine(message); // Scrive il messaggio seguito da un carattere di nuova linea
            Log.Logger.Information("Messaggio inviato: " + message);
        }

        public void CloseSerial()
        {
            // Chiude la porta quando si esce dal programma
            serialPort.Close();
        }
    }
}
