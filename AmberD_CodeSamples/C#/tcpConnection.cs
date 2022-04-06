/*This class creates connection to Modbus TCP PLC
 /* Developed in C# by: Amber Davidson 
 /* 
 */

using HslCommunication.ModBus;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emerson_Alpha_User_Interface
{
    class tcpConnection//Connect Modbus TCP
    {        
        public tcpConnection()
        {
            //Empty Class Constructor
        }

        //Connect Modbus TCP
        public ModbusTcpNet connectModbusTCP(string ip, int port, byte station)//Class method for connection
        {                  
            ModbusTcpNet busTcpClient = new ModbusTcpNet(ip, port, station);//Declare client
            PingReply reply;
            string answer = "";
            try
            {                   
               // bool pingable = false;
                Ping pinger = null;

                try
                {
                    pinger = new Ping();//Ping the PLC to confirm physical network connetction
                    reply = pinger.Send(ip);
                    answer = reply.Status.ToString();//Get reply                      
                }
                catch (PingException ex)
                {
                    MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
                    return busTcpClient;//Return client to main
                }
                finally
                {
                    if (answer == "TimedOut")//If no Connection return
                    {
                        pinger.Dispose();
                        busTcpClient = null;
                        
                    }
                    if (answer != "TimedOut")//If ping succeeds, connect to server
                    {
                        busTcpClient.ConnectServer();//Attempt Connection to TCP Server 
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");               
            }

            return busTcpClient;//Return client to main
        }
    }
}
