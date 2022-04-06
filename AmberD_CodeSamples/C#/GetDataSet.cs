/*
Amber D. 2019 C# Visual studio 2019

Get SQL data class for Tool tracking App
*/


using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Tooling_Tracker_Info
{
    class GetDataSet
    {
        //Class Variables
        DataSet dsData = new DataSet();
        DataSet dsData2 = new DataSet();
        DataSet dsDataBuffer;
        private string strSql;
        private string toolID;
        private string id;
        private string toolEnum;
        private string machine;
        private DateTime date1;
        private DateTime date2;

        public GetDataSet()
        {
            //Empty Class Constructor
        }

        public GetDataSet(string ID)
        {
            id = ID;
        }

        public GetDataSet(string machineNum, DateTime start, DateTime end)
        {
            machine = machineNum;
            date1 = start;
            date2 = end;
        }

        //Getters and Setters
        public string ID { get; set; }
        public string Machine { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public DataSet getToolListLoad()
        {
            strSql = "SELECT DISTINCT toolID FROM tblToolID WHERE activeBool = 1" + //SQL for loading Tool List 
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Tool' AND activeBool = 1" + //SQL for Tool List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Holder' AND activeBool = 1" + //SQL for holder List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Ret. Knob' AND activeBool = 1" + //SQL for knob List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Collet 1' AND activeBool = 1" + //SQL for collet1 List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Collet 2' AND activeBool = 1" + //SQL for collet2 List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Collet Nut' AND activeBool = 1" + //SQL for collet nut List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Extension' AND activeBool = 1" + //SQL for extension List on frmAdd
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Ext. Nut' AND activeBool = 1" + //SQL for ext. nut List on frmAdd  
                " SELECT componentEnum FROM tblToolComponents WHERE componentType = 'Bridge' AND activeBool = 1" + //SQL for bridge List on frmAdd 
                " SELECT cell FROM tblCell" +//SQL for Cell List
                " SELECT toolFamily FROM tblToolFamily" +//SQL for Tool Family
                " SELECT radiusType FROm tblToolRadiusType";

            connectDB();//DB Method
            return dsData;//Return 
        }

        //Method for tool serial number zoller record
        public DataSet sqlScanRecord()
        {
            if (frmToolInfo.SetValueForScanOrSearch == "scan")//If Scan is used
            {
                strSql = "SELECT DISTINCT serialNum, toolID, setDateTime, toolLength, toolCrossWays, count, cell FROM tblToolSetRecord " +
                "WHERE serialNum = '" + id + "'";
            }
            if (frmToolInfo.SetValueForScanOrSearch == "type")//If Type Selectedc from list
            {
                strSql = "SELECT serialNum, toolID, setDateTime, toolLength, toolCrossWays, count, cell FROM tblToolSetRecord " +
                "WHERE toolID = '" + id + "'";
            }
                connectDB();//DB Method         
            try
            {
                //Set Header Text for record
                dsData.Tables[0].Columns["serialNum"].ColumnName = "Serial Number";
                dsData.Tables[0].Columns["toolID"].ColumnName = "Tool ID";
                dsData.Tables[0].Columns["setDateTime"].ColumnName = "Last Set  ";
                dsData.Tables[0].Columns["toolLength"].ColumnName = "Length";
                dsData.Tables[0].Columns["toolCrossWays"].ColumnName = "Radius";
                dsData.Tables[0].Columns["count"].ColumnName = "# of times set";
                dsData.Tables[0].Columns["cell"].ColumnName = "Cell";
            }
            catch(DataException ex)
            {
                MessageBox.Show(ex.ToString(), "Plt8 Tool Info");
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.ToString(), "Plt8 Tool Info");
            }
 
            return dsData;//Return 
        }

        //Method for tool information from serial number
        //This could be more efficient if using SQL JOIN instead of multiple queries and table restructuring
        public DataSet sqlToolInfo()
        {
            if (frmToolInfo.SetValueForScanOrSearch != "type")//If Scan is used
            {
                strSql = "SELECT toolID FROM tblToolSetRecord WHERE serialNum = '" + id + "'";//SQL Query to find ID from s#

                connectDB();//DB Method

                if (dsData.Tables[0] != null && dsData.Tables[0].Rows.Count > 0)//If Data returned is present 
                {
                    toolID = dsData.Tables[0].Rows[0]["toolID"].ToString();//Get zollerID from Data Set
                                                                           // dsData.Clear();//Clear Dataset for next Query
                }
            }
            else if (frmToolInfo.SetValueForScanOrSearch != "scan")//If Scan is used
            {
                toolID = frmToolInfo.SetValueForSearch;
                //   strSql = "SELECT * FROM tblToolInfo WHERE toolID = '" + frmToolInfo.SetValueForSearch + "' "; //Set SQL to retrieve tool info 
            }
            if (toolID != null)
            {
                //Get tool info 
                strSql = "SELECT tblToolInfo.toolID, tblToolInfo.toolNum, tblToolComponents.componentDesc FROM tblToolInfo LEFT JOIN tblToolComponents " +
                    "ON tblToolInfo.tool = tblToolComponents.componentEnum WHERE tblToolInfo.toolID = '" + toolID + "' " +
                    //Get tool offsets
                    "SELECT length, lengthTolerance, radius, radiusToleranceLow, radiusToleranceHigh, chamferOffset FROM tblToolInfo  " +
                    "WHERE toolID = '" + toolID + "' " +
                    //Get tool info 2
                    "SELECT toolLife, lifeBumpAllowed, lifeBumpMax, radiusType, toolFamily FROM tblToolInfo  " +
                    "WHERE toolID = '" + toolID + "' " +
                   //Get Components
                   "SELECT tool, holder, retKnob, collet1, collet2, colletNut, extension, extNut, bridge FROM tblToolInfo  " +
                    "WHERE toolID = '" + toolID + "'";

                connectDB();//DB Method             
            }
          
                return dsData;
        }

        //Method to return Zoller ID for list
        public DataSet getToolList()
        {
            strSql = "SELECT DISTINCT toolID FROM tblToolID WHERE activeBool = 1";//SQL Query to find ID from s#
            connectDB();//DB Method 
            return dsData;
        }
        //Method to return Cell list
        public DataSet getCellList()
        {
            strSql = "SELECT DISTINCT cell FROM tblCell"; //SQL for loading Cell List
            connectDB();//DB Method 
            return dsData;
        }
        public DataSet getTotalList()
        {
            strSql = "SELECT * FROM tblToolInfo JOIN tblToolID ON tblToolInfo.toolID = tblToolID.toolID" + 
                "";//SQL Query to find ID from s#

            connectDB();//DB Method 
            if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Columns.Contains("toolID1"))
            {
                dsData.Tables[0].Columns.Remove("toolID1");
            }

            return dsData;
        }

        //To fill add page existing info and confirm record exists to prevent duplicate errors
        public DataSet loadExisting()
        {
            strSql = "SELECT * FROM tblToolInfo WHERE toolID = '" + id + "'"; //Set SQL to retrieve tool info
            connectDB();
            return dsData;
        }
        public DataSet getComponents()
        {
            strSql = "SELECT componentDesc, ComponentEnum, ComponentType FROM tblToolComponents " +
                "WHERE activeBool = 1"; //SQL for loading Components List
            connectDB();//DB Method 
            return dsData;
        }
        public DataSet checkComponentStatus()
        {
            strSql = "SELECT activeBool FROM tblToolComponents " +
               "WHERE componentEnum = '" + id + "'"; //SQL for loading Component List
            connectDB();//DB Method 
            return dsData;
        }
        public DataSet getComponentType()
        {
            strSql = "SELECT DISTINCT componentType FROM tblToolComponentType"; //SQL for loading Cell List
            connectDB();//DB Method 
            return dsData;
        }
        public DataSet getMachineTools()//Method to get active tools from cell and machine selection
        {
            strSql = "SELECT toolNum, serialNum, toolLife, length, crossways, timeIn From tblToolMachine WHERE machineNum = '" + id +
                "' AND statusInt = 1 ORDER BY toolNum";
            connectDB();//DB Method

            if (dsData.Tables.Count > 0)//If Data returned is present 
            {
                getToolDesc();
            }
                 
            return dsData2;
        }

        public DataSet getMachines()
        {
            strSql = "SELECT DISTINCT tblToolMachine.machineNum FROM tblToolMachine " +
                "JOIN tblToolSetRecord ON tblToolSetRecord.serialNum = tblToolMachine.serialNum AND tblToolSetRecord.cell = '" + id + "'";

            connectDB();//DB Method 
            return dsData;
        }
        public DataSet getCellListZoller()
        {
            strSql = "SELECT DISTINCT cell FROM tblToolSetRecord";
            connectDB();//DB Method 
            return dsData;
        }
        public DataSet getMachineHistory()
        {
            strSql = "SELECT toolNum, serialNum, reasonChanged, timeIn, timeOut From tblToolMachine WHERE machineNum = '" + machine + "' AND timeIn >= '" + date1.Date +
                "' AND timeIn <= '" + date2.Date + "' OR timeOut = null ORDER BY toolNum";
            connectDB();//DB Method

            if (dsData.Tables.Count > 0)//If Data returned is present 
            {
                getToolDesc();
                //dsData2 = dsData.Copy();//Copy Data
                //dsData2.Tables[0].Columns.Add("Tool ID", typeof(string)); //Add Columns to dataset
                //dsData2.Tables[0].Columns.Add("Tool Desc", typeof(string)); //Add Columns to dataset
                //dsData.Clear();//Clear Dataset for next Query

                //for (int i = 0; i < dsData2.Tables[0].Rows.Count; i++)//Loop through entire list
                //{
                //    int toolNum = Convert.ToInt32(dsData2.Tables[0].Rows[i]["toolNum"]);//Get tool number
                //    strSql = "SELECT toolID, tool FROM tblToolInfo WHERE toolNum = '" + toolNum + "'";
                //    connectDB();//DB Method

                //    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)//If Data returned is present 
                //    {
                //        dsData2.Tables[0].Rows[i]["Tool ID"] = dsData.Tables[0].Rows[0]["toolID"];//Copy found tool ID to row of other table
                //        strSql = "SELECT componentDesc FROM tblToolComponents WHERE componentEnum = '" + dsData.Tables[0].Rows[0]["tool"] + "'";
                //        dsData.Clear();//Clear Dataset for next Query
                //        connectDB();//DB Method

                //        if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)//If Data returned is present 
                //        {
                //            dsData2.Tables[0].Rows[i]["Tool Desc"] = dsData.Tables[0].Rows[0]["componentDesc"];//Copy found description to row of other table
                //        }
                //    }
                //}
            }

            //connectDB();//DB Method 
            return dsData2;
        }

        private void getToolDesc()
        {
            dsData2 = dsData.Copy();//Copy Data
            dsData2.Tables[0].Columns.Add("Tool ID", typeof(string)); //Add Columns to dataset
            dsData2.Tables[0].Columns.Add("Tool Desc", typeof(string)); //Add Columns to dataset
            dsData.Clear();//Clear Dataset for next Query

            for (int i = 0; i < dsData2.Tables[0].Rows.Count; i++)//Loop through entire list
            {
                int toolNum = Convert.ToInt32(dsData2.Tables[0].Rows[i]["toolNum"]);//Get tool number
                strSql = "SELECT toolID, tool FROM tblToolInfo WHERE toolNum = '" + toolNum + "'";
                connectDB();//DB Method

                if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)//If Data returned is present 
                {
                    dsData2.Tables[0].Rows[i]["Tool ID"] = dsData.Tables[0].Rows[0]["toolID"];//Copy found tool ID to row of other table
                    strSql = "SELECT componentDesc FROM tblToolComponents WHERE componentEnum = '" + dsData.Tables[0].Rows[0]["tool"] + "'";
                    dsData.Clear();//Clear Dataset for next Query
                    connectDB();//DB Method

                    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)//If Data returned is present 
                    {
                        dsData2.Tables[0].Rows[i]["Tool Desc"] = dsData.Tables[0].Rows[0]["componentDesc"];//Copy found description to row of other table
                    }
                }
            }
        }

        //Method to Instantiate object of DatabaseConnection class
        private void connectDB()
        {
            try
            {
                DatabaseConnection objConnect = new DatabaseConnection();//Create class Object DataBaseConnection
                objConnect.Sql = strSql;//Set SQL statement for class variable
                dsData = objConnect.GetConnection;//Get Data from DB Clas
            }
            catch (SqlException ex)
            {
                //Display Error Message
                // MessageBox.Show("Cannot Retrieve Record!", "Plt8 Tool Info");
            }
        }
    }
}
