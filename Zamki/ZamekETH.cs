//***************************************
// Andrzej Kupiec
//  ver 1.0     07-01-2016


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zamki
{
    class ZamekETH
    {
        static int sleepTime = 1000; //min 700
        static int readTimeOut = 3000; //min 1000

        private byte[] adres = new byte[8];
        private string ip;
        private bool tamper;
        private bool hall;
        private int openTime;
        private int sustainTime;
        private int current;
        private int tensity;
        private int temerature;

        public ZamekETH(string hex, string adresIp)
        {

            if (hex.Length == 16)
                for (int i = 0; i < hex.Length; i += 2)
                    adres[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            ip = adresIp;

        }

        public byte[] Adres
        {
            get
            {
                return adres;
            }

            set
            {
                adres = value;
            }
        }

        public bool Tamper
        {
            get
            {
                return tamper;
            }

            set
            {
                tamper = value;
            }
        }

        public bool Hall
        {
            get
            {
                return hall;
            }

            set
            {
                hall = value;
            }
        }

        public int OpenTime
        {
            get
            {
                return openTime;
            }

            set
            {
                openTime = value;
            }
        }

        public int SustainTime
        {
            get
            {
                return sustainTime;
            }

            set
            {
                sustainTime = value;
            }
        }

        public int Current
        {
            get
            {
                return current;
            }

            set
            {
                current = value;
            }
        }

        public int Tensity
        {
            get
            {
                return tensity;
            }

            set
            {
                tensity = value;
            }
        }

        public int Temerature
        {
            get
            {
                return temerature;
            }

            set
            {
                temerature = value;
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public int GetAdress()
        {

            int wynik=1;
            byte[] response = new byte[13];

            try
            {

                              
                byte[] outbuf = new byte[13];
            
                byte[] CRC = new byte[2];
                ushort licznik = 0;
                ushort j;

                //Add bytecount to message:
                outbuf[licznik++] = 0x13;
                outbuf[licznik++] = 0x64;
                outbuf[licznik++] = 0x01;
                for (j = 0; j < 8; j++)
                {
                    if (j < 4)
                        outbuf[licznik++] = 0xAA;
                    else
                        outbuf[licznik++] = 0x33;
                }

                outbuf[0] = (byte)outbuf.Length;

                GetCRC(outbuf, ref CRC);
                outbuf[11] = CRC[1];
                outbuf[12] = CRC[0];


                for (int i = 0; i < outbuf.Length; i++)
                {
                    System.Console.WriteLine("Ask[" + i + "]:" + outbuf[i]);
                }


                TcpClient client = new TcpClient(ip, 1001);
                NetworkStream stream = client.GetStream();

                stream.Write(outbuf, 0, outbuf.Length);
                Thread.Sleep(sleepTime);

                stream.ReadTimeout = readTimeOut;
                try
                {
                    Int32 bytes = stream.Read(response, 0, response.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadTimeOut: {0}", e);
                }

                for (int i = 0; i < response.Length; i++)  System.Console.WriteLine("Rep[" + i + "]:" + response[i]);
                for (int i = 0; i < 8; i++) adres[i] = response[i+3];

                GetCRC(response, ref CRC);
                stream.Close();
                client.Close();

                if ((response[11] == CRC[1]) && (response[12] == CRC[0])) { wynik = 0; } else { wynik = 2; }
              

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                wynik = 3;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                wynik = 4;
            }

            if (response[0] == 0x00) wynik = 1;
            
            return wynik;


        }



        public int Open()
        {
            
           
                int wynik=4;

                byte[] outbuf = new byte[13];
                byte[] response = new byte[15];
                byte[] CRC = new byte[2];
                byte[] CRC1 = new byte[2];
                ushort licznik = 0;
                ushort j;

            try
            {

                //Add bytecount to message:
                outbuf[licznik++] = 0x13;
                outbuf[licznik++] = 0x65;
                outbuf[licznik++] = 0x03;
                for (j = 0; j < 8; j++) outbuf[licznik++] = adres[j];
                outbuf[0] = (byte)outbuf.Length;
                GetCRC(outbuf, ref CRC);
                outbuf[11] = CRC[1];
                outbuf[12] = CRC[0];


                for (int i = 0; i < outbuf.Length; i++)
                {
                    System.Console.WriteLine("Ask[" + i + "]:" + outbuf[i]);
                }


                TcpClient client = new TcpClient(ip, 1001);
                NetworkStream stream = client.GetStream();
                
                stream.Write(outbuf, 0, outbuf.Length);
                Thread.Sleep(sleepTime);


                stream.ReadTimeout = readTimeOut;

                try
                {
                    Int32 bytes = stream.Read(response, 0, response.Length);
                    if (bytes> response.Length) Console.WriteLine("ReadTimeOut");


                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadTimeOut: {0}", e);
                }

                if (response[0] == 0x00)
                {
                    for (int i = 1; i < response.Length; i++) response[i - 1] = response[i];
                }

                for (int i = 0; i < response.Length; i++) System.Console.WriteLine("Rep[" + i + "]:" + response[i]);

                GetCRC1(response, 14, ref CRC1);

                stream.Close();
                client.Close();

                wynik = response[11];
                if ((response[12] != CRC1[1]) || (response[13] != CRC1[0])) { wynik=3; }

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                wynik= 5;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                wynik=6;
            }

            if (response[0] == 0x00) wynik = 4;

            return wynik;

        }
        

        public int Status()
        {

            int wynik = 2;
            byte[] response = new byte[20];

            try
            {


                byte[] outbuf = new byte[13];

                byte[] CRC = new byte[2];
                ushort licznik = 0;
                ushort j;

                //Add bytecount to message:
                outbuf[licznik++] = 0x13;
                outbuf[licznik++] = 0x67;
                outbuf[licznik++] = 0x03;
                for (j = 0; j < 8; j++) outbuf[licznik++] = adres[j];
                outbuf[0] = (byte)outbuf.Length;
                GetCRC(outbuf, ref CRC);
                outbuf[11] = CRC[1];
                outbuf[12] = CRC[0];


                for (int i = 0; i < outbuf.Length; i++)
                {
                    System.Console.WriteLine("Ask[" + i + "]:" + outbuf[i]);
                }

                TcpClient client = new TcpClient(ip, 1001);
                NetworkStream stream = client.GetStream();


                stream.Write(outbuf, 0, outbuf.Length);
                Thread.Sleep(sleepTime);

                stream.ReadTimeout = readTimeOut;
            


                try
                {

                    Int32 bytes = stream.Read(response, 0, response.Length);
                
                    /*
                    Int32 bytes = stream.Read(response, 0, 1);

                    if (response[0]==0) {
                        Console.WriteLine("Zero");
                       bytes = stream.Read(response, 0, response.Length); }
                    else {
                        Console.WriteLine("Cos");
                        bytes = stream.Read(response, 1, response.Length-1); }
                    */
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadTimeOut: {0}", e);
                }


                if (response[0] == 0x00)
                {
                    for (int i = 1; i < response.Length; i++) response[i - 1] = response[i];
                }


                for (int i = 0; i < response.Length; i++) System.Console.WriteLine("Rep[" + i + "]:" + response[i]);
                if (response[11] == 1) tamper = true; else tamper = false;
                if (response[12] == 1) hall = true; else hall = false;
                openTime = (response[13] * 256) + response[14];
                sustainTime = (response[15] * 256) + response[16];

                GetCRC1(response,19, ref CRC);
                stream.Close();
                client.Close();

                if ((response[17] == CRC[1]) && (response[18] == CRC[0])) { wynik=0; } else { wynik=1; }

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                wynik=3;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                wynik=4;
            }

            if (response[0] == 0x00) wynik = 2;

            return wynik;

        }


        
        public int Condition()
        {

            int wynik = 2;
            byte[] response = new byte[20];

            try
            {


                byte[] outbuf = new byte[13];


                byte[] CRC = new byte[2];
                ushort licznik = 0;
                ushort j;

                //Add bytecount to message:
                outbuf[licznik++] = 0x13;
                outbuf[licznik++] = 0x68;
                outbuf[licznik++] = 0x03;
                for (j = 0; j < 8; j++) outbuf[licznik++] = Adres[j];
                outbuf[0] = (byte)outbuf.Length;
                GetCRC(outbuf, ref CRC);
                outbuf[11] = CRC[1];
                outbuf[12] = CRC[0];


                for (int i = 0; i < outbuf.Length; i++)
                {
                    System.Console.WriteLine("Ask[" + i + "]:" + outbuf[i]);
                }


                TcpClient client = new TcpClient(ip, 1001);
                NetworkStream stream = client.GetStream();

                stream.Write(outbuf, 0, outbuf.Length);
                Thread.Sleep(sleepTime);

                stream.ReadTimeout = readTimeOut;
                try
                {
                    Int32 bytes = stream.Read(response, 0, response.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadTimeOut: {0}", e);
                }


                if (response[0] == 0x00)
                {
                    for (int i = 1; i < response.Length; i++) response[i - 1] = response[i];
                }

                for (int i = 0; i < response.Length; i++) System.Console.WriteLine("Rep[" + i + "]:" + response[i]);

                current = (response[11] * 256) + response[12];
                temerature = (response[13] * 256) + response[14];
                tensity = (response[15] * 256) + response[16];

                GetCRC1(response, 19, ref CRC); 
                stream.Close();
                client.Close();

                if ((response[17] == CRC[1]) && (response[18] == CRC[0])) { wynik=0; } else { wynik=1; }


            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                wynik=3;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                wynik = 4;
            }

            if (response[0] == 0x00) wynik = 2;
            return wynik;

        }

        
        public int Setings(ushort czas1, ushort czas2)
        {


            int wynik = 2;
            byte[] response = new byte[14];

            try
            {


                byte[] outbuf = new byte[17];
                                
                byte[] CRC = new byte[2];
                ushort licznik = 0;
                ushort j;

                //Add bytecount to message:
                outbuf[licznik++] = 0x11;
                outbuf[licznik++] = 0x66;
                outbuf[licznik++] = 0x01;
                for (j = 0; j < 8; j++) outbuf[licznik++] = adres[j];
                outbuf[0] = (byte)outbuf.Length;

                outbuf[11] = (byte)(czas1 >> 8);
                outbuf[12] = (byte)(czas1 & 0xFF);

                outbuf[13] = (byte)(czas2 >> 8);
                outbuf[14] = (byte)(czas2 & 0xFF);

                GetCRC(outbuf, ref CRC);

                outbuf[15] = CRC[1];
                outbuf[16] = CRC[0];


                for (int i = 0; i < outbuf.Length; i++)
                {
                    System.Console.WriteLine("Ask[" + i + "]:" + outbuf[i]);
                }


                TcpClient client = new TcpClient(ip, 1001);
                NetworkStream stream = client.GetStream();

                stream.Write(outbuf, 0, outbuf.Length);
                Thread.Sleep(sleepTime);

                stream.ReadTimeout = readTimeOut;
                try
                {
                    Int32 bytes = stream.Read(response, 0, response.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadTimeOut: {0}", e);
                }


                if (response[0] == 0x00)
                {
                    for (int i = 1; i < response.Length; i++) response[i - 1] = response[i];

                }

                for (int i = 0; i < response.Length; i++) System.Console.WriteLine("Rep[" + i + "]:" + response[i]);
                for (int i = 0; i < 8; i++) adres[i] = response[i + 3];


                GetCRC1(response, 13, ref CRC);
                stream.Close();
                client.Close();

                if ((response[11] == CRC[1]) && (response[12] == CRC[0])) { wynik=0; } else { wynik=1; }


            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                wynik=2;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                wynik=3;
            }

            if (response[0] == 0x00) wynik = 1;

            return wynik;

        }
        

        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }


        private void GetCRC1(byte[] message,int how, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (how) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }



    }
}
