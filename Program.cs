using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelsecProtocolTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connect to PLC...");
            MelsecPLC plc = new MelsecPLC("10.123.241.26", 2000, 2001);
            string ip = plc.IP;
            int rPort = plc.ReadPort;
            int wPort = plc.WritePort;

            
            plc.Open();


            if(plc.IsAvailable==true)
                Console.WriteLine("PLC is available!");

            if (plc.IsConnected == true)
                Console.WriteLine("PLC is Connected!: IP = {0}, ReadPort = {1}, WritePort = {2}", ip, rPort, wPort);          
            
           
            string respData = "";

            plc.ReadWordData("W*", 430, 1, out respData);

            string wr = respData;
            byte[] respDataByte = Encoding.ASCII.GetBytes(respData);

            int respDataInt = Convert.ToInt32(respData, 16);

            Console.WriteLine(respDataInt);
            Console.ReadLine();

        }


     /*   public static void CreateMessage() // Test!!!!
        {

             // 4E Frame Message Format Read Command

            // Serial to determine external device for PLC
            int SerialNoInt = 1234;
            string SerialNo = SerialNoInt.ToString();

            //Subheader
            string SubHeader = "5400" + SerialNo + "0000";

            // Network Number
            string NetworkNo = "00";

            // PC Number
            string PCNumber = "FF";

            // Request Destination Module I/O Number
            string ReqDistModule = "03FF";

            //Request Destination Module station number
            string ReqDistStation = "00";

            // Request data Lenght
            string ReqDataLenght = "0018";

            // CPU Moitoring Timer
            string monitorTime = "0010";

            string CommandHeader = SubHeader + NetworkNo + PCNumber + ReqDistModule + ReqDistStation + ReqDataLenght + monitorTime;

            // Read Command
            string ReadCommand = "0401";

            // Read SubCommand
            string ReadSubcommand = "0000";

            // Device Code
            string DeviceCode = "W*";

            // Head Device Address
            string StartAddr = 100.ToString();

            if (StartAddr.Length == 1)
                StartAddr = "00000" + StartAddr;
            else if (StartAddr.Length == 2)
                StartAddr = "0000" + StartAddr;
            else if (StartAddr.Length == 3)
                StartAddr = "000" + StartAddr;
            else if (StartAddr.Length == 4)
                StartAddr = "00" + StartAddr;
            else if (StartAddr.Length == 5)
                StartAddr = "0" + StartAddr;
            else if (StartAddr.Length == 6)
                StartAddr = 100.ToString();

            // Number of Device Points
            string DevicePointNumber = 2.ToString();

            if (DevicePointNumber.Length == 1)
                DevicePointNumber = "000" + DevicePointNumber;
            else if (DevicePointNumber.Length == 2)
                DevicePointNumber = "00" + DevicePointNumber;
            else if (DevicePointNumber.Length == 3)
                DevicePointNumber = "0" + DevicePointNumber;
            else if (DevicePointNumber.Length == 4)
                DevicePointNumber = 2.ToString();

            string cmd = CommandHeader + ReadCommand + ReadSubcommand + DeviceCode + StartAddr + DevicePointNumber;
            byte[] byteCmd = Encoding.ASCII.GetBytes(cmd);

            byteCmd = Encoding.ASCII.GetBytes(cmd); */


            

        }
        
    }
