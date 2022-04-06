/*Class for reading Modbus values from iPro PLC tag addresses
 * Emerson Centrifugal User Interface v1
  Developed in C# Visual Studion v2019
  by Amber Davidson 9/1/2021

*/

using HslCommunication.ModBus;
using System;
using System.Net;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;

namespace Emerson_Alpha_User_Interface
{
    class readModbus
    {
        //Class Variables
        ushort[] lenArray;//Length to read
        string[] addArray;
        ModbusTcpNet conn;
        string[] dataArray = new string[50];

        public readModbus()
        {
            //Empty Class Constructor
        }    
            
        public readModbus(string[] add, ushort[] ln, ModbusTcpNet tcpNet)//Method to accept address, length, and TCP connection
        {
            addArray = add;
            lenArray = ln;          
            conn = tcpNet;
        }

        //Getters and Setters
        public string add { get; set; }
        public double ln1 { get; set; }
        public double ln2 { get; set; }

        public string[] readExecute(bool stop)//Method to read PLC
        {          
            if (stop)
            {
                //timer.Dispose();
                return dataArray;
            }
            else if (!stop)
            {
                if (conn.ConnectionId != null)
                { //Declare Read operation results
                    HslCommunication.OperateResult<UInt16[]> readStatus = null;//Read - Status, 0, Comp Alarm1, Comp Alarm2,Max Power,Surge,Choke,Act Demand, Req Power,Act,0,Req Speed,Act Speed (12)
                    HslCommunication.OperateResult<UInt16[]> readVFD1 = null;//Read - VFD DCV,0,0,0,Current, Temp (6)
                    HslCommunication.OperateResult<UInt16[]> readFaults = null;//Read Compressor Faults (2)
                    HslCommunication.OperateResult<UInt16[]> readStuff = null;//Fault Reset, Evap exit temp, Cond exit temp, Op.Modes (4)
                    HslCommunication.OperateResult<UInt16[]> readTEMP_PSI = null;//Read SuctP, IntP, DischP, PR, ViP, 0,0,SuctT, IntT, DischT, CoolingT, MotorT, RB1T, RB2T, ThrustT,ViT, VIGV1, VIGV2(20)
                    HslCommunication.OperateResult<bool[]> read_AlarmStats = null;//Read VIGV Alarms,VFD Online (3)
                    HslCommunication.OperateResult<UInt16[]> readVIGVAngles = null;//Read Angle feedback of VIGVs and their set points (4)
                    HslCommunication.OperateResult<UInt16[]> readManualSpeed = null;//Read manual speed control for Interface (1)
                    // HslCommunication.OperateResult<string> readModel = null;Read Model Serial (Future) //(read/write) gASCII_Array_Model

                    try
                    {// Read Tags                                            
                        readStatus = conn.ReadUInt16(addArray[0], lenArray[0]);//Status, 0, Comp Alarm1, Comp Alarm2,Max Power,Surge,Choke,Act Demand, Req Power,Act,0,Req Speed,Act Speed(13)
                        readVFD1 = conn.ReadUInt16(addArray[1], lenArray[1]);//Read - VFD DC, VCurrent, Temp(3)
                        readFaults = conn.ReadUInt16(addArray[2], lenArray[2]);//Read Compressor Faults (2)
                        readStuff = conn.ReadUInt16(addArray[3], lenArray[3]);//Fault Reset, Evap exit temp, Cond exit temp, Op.Modes (4)
                        readTEMP_PSI = conn.ReadUInt16(addArray[4], lenArray[4]);//Read SuctP, IntP, DischP, PR, ViP, 0,0,SuctT, IntT, DischT, CoolingT, MotorT, RB1T, RB2T, ThrustT,ViT,(16)
                        read_AlarmStats = conn.ReadBool(addArray[5], lenArray[5]);//Read VIGV Alarms,VFD Online (3)
                        readVIGVAngles = conn.ReadUInt16(addArray[6], lenArray[3]);//Read Angle feedback of VIGVs (2)
                      //  readManualSpeed = conn.ReadUInt16(addArray[11], lenArray[3]);//Read manual speed control for Interface

                        if (readStatus.IsSuccess)//Status, 0, Comp Alarm1, Comp Alarm2,Max Power,Surge,Choke,Act Demand, Req Power,Act Power,0,Req Speed,Act Speed
                        {
                            //0 = Stopped, 1 = Compressor Ready, 2 = Compressor Starting, 3 = Active Control
                            //6 = Compressor Stopping, 7 = VFD Ready, 8 = VFD Running , 9 = VLV Cool , 10 = VLV motor, 11 = VLV LBW, 12 = , 13 = , 14 = , 15 =
                            dataArray[0] = Convert.ToString(readStatus.Content[0]);//gC1_Status_1
                            dataArray[1] = Convert.ToString(readStatus.Content[2]);//gC1_Alarms_1
                            dataArray[2] = Convert.ToString(readStatus.Content[3]);//gC1_Alarms_2
                            dataArray[3] = Convert.ToString(readStatus.Content[4]);//g_Reg10052 ---Not Used
                            dataArray[4] = Convert.ToString(readStatus.Content[5]);//gC1_Speed_Surge
                            dataArray[5] = Convert.ToString(readStatus.Content[6]);//gC1_Speed_Choke
                            dataArray[6] = Convert.ToString(readStatus.Content[7]);//gC1_Demand_ACT
                            dataArray[7] = Convert.ToString(readStatus.Content[8]);//gC1_Power_CMD  ---Not Used
                            dataArray[8] = Convert.ToString(readStatus.Content[9]);//gC1_Power_ACT
                            dataArray[9] = Convert.ToString(readStatus.Content[11]);//gC1_Speed_CMD ---Mathew's tag for auto control
                            dataArray[10] = Convert.ToString(readStatus.Content[12]);//gC1_Speed_ACT
                        }
                        if(readVFD1.IsSuccess)
                        {
                            dataArray[11] = Convert.ToString(readVFD1.Content[0]);//gC1_VFD_DC
                            dataArray[12] = Convert.ToString(readVFD1.Content[1]);//gC1_VFD_Current
                            dataArray[13] = Convert.ToString(readVFD1.Content[2]);//gC1_Temp_VFD_Display
                        }
                        if (readFaults.IsSuccess)
                        {
                            dataArray[14] = Convert.ToString(readFaults.Content[0]);//gC1_Faults_1
                            dataArray[15] = Convert.ToString(readFaults.Content[1]);//gC1_Faults_2
                        }
                        if (readStuff.IsSuccess)
                        {
                            dataArray[16] = Convert.ToString(readStuff.Content[0]);//g_Reset_Faults
                            dataArray[17] = Convert.ToString(readStuff.Content[1]);//gC1_Temp_Evap_Display
                            dataArray[18] = Convert.ToString(readStuff.Content[2]);//gC1_Cond_Evap_Display
                            dataArray[19] = Convert.ToString(readStuff.Content[3]);//gC1_Modes
                        }
                        if (readTEMP_PSI.IsSuccess)
                        {
                            dataArray[20] = Convert.ToString(readTEMP_PSI.Content[0]);//gC1_Press_Suct_Display
                            dataArray[21] = Convert.ToString(readTEMP_PSI.Content[1]);//gC1_Press_Int_Display
                            dataArray[22] = Convert.ToString(readTEMP_PSI.Content[2]);//gC1_Press_Disc_Display
                            dataArray[23] = Convert.ToString(readTEMP_PSI.Content[3]);//gC1_PR_Display
                            dataArray[24] = Convert.ToString(readTEMP_PSI.Content[4]);//g_Reg10154 --VI Not used
                            dataArray[25] = Convert.ToString(readTEMP_PSI.Content[7]);//gC1_Temp_Suct_Display
                            dataArray[26] = Convert.ToString(readTEMP_PSI.Content[8]);//gC1_Temp_Int_Display
                            dataArray[27] = Convert.ToString(readTEMP_PSI.Content[9]);//gC1_Temp_Disc_Display
                            dataArray[28] = Convert.ToString(readTEMP_PSI.Content[10]);//gC1_Temp_Cool_Display
                            dataArray[29] = Convert.ToString(readTEMP_PSI.Content[11]);//gC1_Temp_Motor_Display
                            dataArray[30] = Convert.ToString(readTEMP_PSI.Content[12]);//gC1_Temp_RB1_Display
                            dataArray[31] = Convert.ToString(readTEMP_PSI.Content[13]);//gC1_Temp_RB2_Display
                            dataArray[32] = Convert.ToString(readTEMP_PSI.Content[14]);//gC1_Temp_THB_Display
                            dataArray[33] = Convert.ToString(readTEMP_PSI.Content[15]);//g_Reg10165 -- VI Not used                    
                        }
                        if (read_AlarmStats.IsSuccess)
                        {                                                                                         
                            dataArray[34] = Convert.ToString(read_AlarmStats.Content[0]);//gC1_VIGV1_Alarm
                            dataArray[35] = Convert.ToString(read_AlarmStats.Content[1]);//gC1_VIGV2_Alarm
                            dataArray[36] = Convert.ToString(read_AlarmStats.Content[2]);//gC1_VFD_Online
                        }
                        if (readVIGVAngles.IsSuccess)
                        {
                            int angleOne = readVIGVAngles.Content[0] - ((readVIGVAngles.Content[0] > 32767) ? 65536 : 0);//Conversion for possible negative number
                            double angleOneB = Convert.ToDouble(angleOne) / 10;
                            angleOneB = Math.Round(angleOneB, 1);//Round to 1 decimal place                          
                            dataArray[37] = Convert.ToString(angleOneB);//gC1_VIGV_1_Display2

                            int angleTwo = readVIGVAngles.Content[1] - ((readVIGVAngles.Content[1] > 32767) ? 65536 : 0);//Conversion for possible negative number
                            double angleTwoB = Convert.ToDouble(angleTwo) / 10;
                            angleTwoB = Math.Round(angleTwoB, 1);//Round to 1 decimal place   
                            dataArray[38] = Convert.ToString(angleTwo);//gC1_VIGV_2_Display2

                            angleOne = readVIGVAngles.Content[2] - ((readVIGVAngles.Content[2] > 32767) ? 65536 : 0);//Conversion for possible negative number
                            dataArray[39] = Convert.ToString(angleOne);//gC1_VIGV_1_Set

                            angleTwo = readVIGVAngles.Content[3] - ((readVIGVAngles.Content[3] > 32767) ? 65536 : 0);//Conversion for possible negative number
                            dataArray[40] = Convert.ToString(angleTwo);//gC1_VIGV_2_Set
                        }
                        //if (readManualSpeed.IsSuccess)
                        //{
                        //    dataArray[41] = Convert.ToString(readManualSpeed.Content[0]);//gC1_VIGV_2_Set
                        //}


                        //if (readModel.IsSuccess)
                        //{
                        // string input = readModel.Content.ToString();
                        // string output = "";
                        // foreach (char c in input)
                        //     if (c != '\0')//Skip unwanted
                        //     {
                        //         char letter = c;//Get character
                        //         output = output + c;//Append to string
                        //     }

                        // dataArray[24] = output;//gASCII_Array 50

                        //}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
                        return dataArray;
                    }
                }
            }            
            return dataArray;//Return data
        }
    }
}
