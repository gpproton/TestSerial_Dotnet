using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace TestSerial_Dotnet
{

    class Program
    {
        static int _Fixed_Bytes = 20;
        static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("Serial ports available:");
            Console.WriteLine("-----------------------");

            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            // SerialPort srp = new SerialPort("/dev/tty.usbserial-1410", 9600);
            SerialPort srp = new SerialPort {BaudRate = 9600, PortName = "/dev/tty.usbserial-1410", DataBits = 8};


            int CountBits = 0;
            byte[] tempByte = new byte[13];
            
            try
            {
                srp.Open();

                while (true)
                {
                    if (CountBits >= _Fixed_Bytes) {
                        // Console.WriteLine(tempByte.ToString());
                        CountBits = 0;
                    }
                    
                    if (srp.BytesToRead > 0)
                    {
                        
                        byte b = (byte)srp.ReadByte();
                        if (Enumerable.Range(7,18).Contains(CountBits)) {
                            tempByte[CountBits - 7] = b;
                        }
                        if (CountBits == _Fixed_Bytes -1) {
                            Console.WriteLine(BitConverter.ToString(tempByte));
                        // foreach(byte xx in tempByte) {
                        //     Console.WriteLine(xx);
                        // }
                        }
                        // Console.WriteLine(b + "----" + CountBits.ToString());

                        CountBits++;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }

            // Console.ReadLine();
        }
    }
}