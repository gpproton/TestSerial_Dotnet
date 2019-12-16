using System;
using System.IO;
using System.IO.Ports;
namespace TestSerial_Dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] ports = SerialPort.GetPortNames();
            
            Console.WriteLine("Serial ports available:");
            Console.WriteLine("-----------------------");
            foreach(string port in ports)
            {
                Console.WriteLine(port);
            }

            // SerialPort srp = new SerialPort("/dev/tty.usbserial-1410", 9600);
            SerialPort srp = new SerialPort();

            srp.BaudRate = 9600;
            srp.PortName = "/dev/tty.usbserial-1410";
            srp.DataBits = 8;

            try
            {
                srp.Open();
                while(true) {
                    if(srp.BytesToRead > 0) {
                        byte b = (byte)srp.ReadByte();
                        // String all = srp.ReadExisting();
                        // String line = srp.ReadLine();
                        // Char c = (Char)srp.ReadChar();
                        Console.WriteLine(b);
                    }
                }
            } catch(IOException e) {
                Console.WriteLine(e);
            }

            // Console.ReadLine();
        }
    }
}