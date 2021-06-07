using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace MelsecProtocolTest
{
    class MelsecPLC
    {
        // Public Connection parameters

        public string IP;
        private string ip
        {
            get { return IP; }

            set
            {
                IP = value;
            }
        }

        public int ReadPort
        { get; set; }

        public int WritePort
        { get; set; }

        public bool IsConnected = false;

        // Private Connection parameters

        Socket plcSocket;

        public bool IsAvailable
        {
           get
            {
                Ping ping = new Ping();
                PingReply result = ping.Send(IP);
                if (result.Status == IPStatus.Success)
                    return true;
                else
                    return false;   
            }
        }
        
        public MelsecPLC(string ip, int readport, int writeport)
        {
             IP = ip;
             ReadPort = readport;
             WritePort = writeport;
        }

        // Error variables

        public ErrorCode lastErrorCode;
        string lastErrorString;       

      

        #region PLC Connection Open, Close
        public ErrorCode Open()
        {           

            try
            {
                // check if PLC available
                Ping ping = new Ping();
                PingReply reply = ping.Send(IP);
                if (reply.Status != IPStatus.Success)
                    throw new Exception();
            }

            catch
            {
                lastErrorCode = ErrorCode.IPAddressNotAvailable;
                lastErrorString = "Destination IP-Address" + IP + "not available";

            }

            try{

            //Creation Socket, designate address and start listening
            plcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            plcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            plcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);

            // Creation of PLC Endpoin
            IPEndPoint _plcServer = new IPEndPoint(new IPAddress(IPToByteArray(IP)),WritePort);
            
            // Creation of local host Endpoint
            System.Net.IPHostEntry localMachineInfo = System.Net.Dns.GetHostEntry(Dns.GetHostName());
            System.Net.IPEndPoint myEndPoint = new IPEndPoint(localMachineInfo.AddressList[0],102);

            // Binding local host to remote Endpoint, start listening and connect
            //plcSocket.Bind(myEndPoint);
            plcSocket.Connect(_plcServer);
            //plcSocket.Listen((int)SocketOptionName.MaxConnections);


            if (plcSocket.Connected == true)
                IsConnected = true;

             

            }

            catch (Exception ex)
            {
                lastErrorCode = ErrorCode.ConnectionError;
                lastErrorString = ex.Message;
                
            }
            return ErrorCode.ConnectionError;

        }

        public void Close()
        {
            plcSocket.Close();
        }

        #endregion

        public void ReadWordData(string deviceCode, int startAddr, int devicePointsNumber, out string ResponceData)
        {
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
            string DeviceCode = deviceCode;

            // Head Device Address
            string StartAddr = startAddr.ToString();

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
                StartAddr = startAddr.ToString();

            // Number of Device Points
            string DevicePointNumber = devicePointsNumber.ToString();

            if (DevicePointNumber.Length == 1)
                DevicePointNumber = "000" + DevicePointNumber;
            else if (DevicePointNumber.Length == 2)
                DevicePointNumber = "00" + DevicePointNumber;
            else if (DevicePointNumber.Length == 3)
                DevicePointNumber = "0" + DevicePointNumber;
            else if (DevicePointNumber.Length == 4)
                DevicePointNumber = devicePointsNumber.ToString();

           string cmd = CommandHeader + ReadCommand + ReadSubcommand + DeviceCode + StartAddr + DevicePointNumber;
           byte[] byteCmd = Encoding.ASCII.GetBytes(cmd);

           plcSocket.Send(byteCmd);

           byte[] bReceive = new byte[30 + devicePointsNumber*4];

           int numReceive = plcSocket.Receive(bReceive, 30 + devicePointsNumber * 4, SocketFlags.None);
           int lenghtHex = Convert.ToInt16(((bReceive[25].ToString("X"))));
         //  if (lenghtHex != (30 + devicePointsNumber * 4 + 4))
          //      throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());

            string responceData = (Encoding.ASCII.GetString(bReceive)).Substring(30, bReceive.Length - 30);// responce data from ByteArray to string           
            //string responceData = Encoding.ASCII.GetString(bReceive);
            ResponceData = responceData;

        }


        #region Convert string IP to ByteArray
        private byte[] IPToByteArray(string ip)
        {
            byte[] IpByteArray = new byte[4]; // Output value IP as byte[4] array
            string txt = ip; // Input IP-address as string
            string txt2 = null; // in-between IP-address substring
            try
            {
                txt2 = txt.Substring(0, txt.IndexOf("."));
                IpByteArray[0] = byte.Parse(txt2);
                txt = txt.Substring(txt2.Length + 1);

                 txt2 = txt.Substring(0, txt.IndexOf("."));
                IpByteArray[1] = byte.Parse(txt2);
                txt = txt.Substring(txt2.Length + 1);

                txt2 = txt.Substring(0, txt.IndexOf("."));
                IpByteArray[2] = byte.Parse(txt2);
                txt = txt.Substring(txt2.Length + 1);

                IpByteArray[3] = byte.Parse(txt);
                return IpByteArray;
            }
            catch
            {
                IpByteArray[0] = 0;
                IpByteArray[1] = 0;
                IpByteArray[2] = 0;
                IpByteArray[3] = 0;
                return IpByteArray;
            }
        }
        #endregion

    }

    }




