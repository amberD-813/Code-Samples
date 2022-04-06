/*
Centrifugal Interface class

This class accepts values from main
and writes values to PLC tags through Modbus TCP
DINTs or bools

Amber D. 2021 C# Visual Studio 2019
*/


using System;
using HslCommunication.ModBus;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emerson_Alpha_User_Interface
{
    class writeModbus
    {
        //Class Variables
        //bool gVis_EStop;
        // bool gVis_Run;
        bool action;
        UInt16 writeNum;
        //string gC1_ModelNum;
        string address;
        ModbusTcpNet conn;

        public writeModbus()
        {
            //Empty Class Constructor
            
        }
        public writeModbus(UInt16 num, string add, ModbusTcpNet connection)//Method to accept address and Uint16 for PLC DINTs
        {
            //writeSpeed();
            writeNum = num;
            address = add;
            conn = connection;
        }
        public writeModbus(bool doSomething,  string add, ModbusTcpNet connection)//Method to start or stop
        {           
            action = doSomething;
            address = add;
            conn = connection;
        }
        //public writeModbus(string model, string add, ModbusTcpNet connection)//Method to accept address and Uint16 for PLC DINTs
        //{
        //    //writeSpeed();
        //    gC1_ModelNum = model;
        //    address = add;
        //    conn = connection;
        //}

        //Getters and Setters
        public string num { get; set; }
        public UInt16 add { get; set; }
        public bool doSomething { get; set; }
        public ModbusTcpNet connection { get; set; }
        public string model { get; set; }

        public bool writeData()//Class method to write speed to iPro
        {
            bool success = false;
            try
            {
                HslCommunication.OperateResult write = conn.Write(address, writeNum);

                if (write.IsSuccess)
                {
                    success = true;
                }
                else
                {
                    MessageBox.Show(write.ToMessageShowString(), "Emerson Climate Technologies");
                    success = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }
            return success;
        }
        public bool writeAction()//Class method to write stop or start drive to iPro
        {
            bool completed = false;
            try
            {
                HslCommunication.OperateResult write = conn.Write(address, action);//Write command Modbus

                if (write.IsSuccess)
                {
                    completed = true;
                }
                else
                {
                    MessageBox.Show(write.ToMessageShowString(), "Emerson Climate Technologies");
                    completed = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }
            return completed;          
        }
        //public bool writeString()//Class method to write speed to iPro
        //{
        //    bool success = false;
        //    try
        //    {
        //        HslCommunication.OperateResult write = conn.Write(address, gC1_ModelNum);

        //        if (write.IsSuccess)
        //        {
        //            success = true;
        //        }
        //        else
        //        {
        //            MessageBox.Show(write.ToMessageShowString(), "Emerson Climate Technologies");
        //            success = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
        //    }
            //return success;
       // }
    }
}
