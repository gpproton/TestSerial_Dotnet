using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Threading.Tasks;
using NLog;
using TestSerial_Dotnet.model;

namespace TestSerial_Dotnet
{

    class Program
    {
        static int _Fixed_Bytes = 20;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            string dbName = "TestDatabase.db";
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }

            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("file") { FileName = "testinfo.log" };
            var logconsole = new NLog.Targets.ConsoleTarget("console");
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // Apply config           
            NLog.LogManager.Configuration = config;


            //Check if Internet is ON or OF
            if(HasConnection())
            {
                Console.WriteLine("Internet ON");
            }
            else
            {
                Console.WriteLine("Internet OFF");
            }

            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("Serial ports available:");
            Console.WriteLine("-----------------------");

            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
            Console.WriteLine("-----------------------");

            // SerialPort srp = new SerialPort("/dev/tty.usbserial-1410", 9600);
            String serialName = "/dev/tty.usbserial-1410";
            SerialPort srp = new SerialPort {BaudRate = 9600, PortName = serialName, DataBits = 8};


            int CountBits = 0;
            byte[] tempByte = new byte[13];
            Random rnd = new Random();
            var dbContext = new AppsContext();
            dbContext.Database.EnsureCreated();

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
                        var finalHex = BitConverter.ToString(tempByte).Replace("-", string.Empty);
                        var cutHex = finalHex.Replace("E20000", string.Empty);

                        //var cardNumber = Convert.ToUInt64("16370402410910C2E9", 16);
                        var cardNumber = BigInteger.Parse(cutHex, System.Globalization.NumberStyles.HexNumber);

                        if (CountBits == _Fixed_Bytes -1) {

                            //Ensure database is created
                            //if (!dbContext.Cards.Any())
                            //{
                            
                            //}

                            try
                            {
                                dbContext.Cards.AddRange(new Card[]
                                {
                                            new Card{ Id=rnd.Next(999999999), CardNo=(decimal)cardNumber, Location=1, SubLocation=23, CaptureType=1 }
                                });

                                if (dbContext.SaveChanges() > 0)
                                {
                                    try
                                    {
                                        Logger.Info("UHF RFID NO:- " + cardNumber);

                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex, "Goodbye cruel errors...");
                                    }
                                }

                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
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

        //Check if Internet is ON or OF
        public static bool HasConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}