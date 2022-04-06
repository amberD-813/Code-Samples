
/*
ATTS - Plt 8 Tool Trackng App
Class for database connection
Accepts SQL command, executes, and gives conformation 
of completion.

Should improve to pass path as argument
instead of hard code.

Amber D. 2019 C#
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Tooling_Tracker_Info
{
    class DatabaseConnection
    {
        //Class Variables
        private string strSql;//SQL Query String Variable                    
        private string strCon = @"Data Source=157.103.1.35;Initial Catalog=dataTools;User ID=HIDElTracker;Password=HIDEpswd;Connect Timeout=20";//DB connection string
        public Boolean confirmation = false;

        //Getters and Setters
        public string Sql
        {
            set { strSql = value; }
        }

        public string Con
        {
            set { strCon = value; }
        }

        public DataSet GetConnection
        {
            get
            { return MyDataSet(); }
        }

        private DataSet MyDataSet()//Method to Get Data
        {
            SqlConnection con = new SqlConnection(strCon);//Connection Path to DB
            SqlCommand cmd = new SqlCommand(strSql, con); //Command made from SQL Query string and Connection Path
            SqlDataAdapter da = new SqlDataAdapter(cmd);//Create and Load Adapter 
            DataSet data = new DataSet(); //Create DataTable to store DataAdapter data         
            confirmation = false;

            try
            {
             da.Fill(data); //Use the DataAdapter to fill the DataTables      
             confirmation = true;
            }
            catch (SqlException ex)
            {
                //Display Error Message
                MessageBox.Show(ex.ToString(), "Tool Tracking Info");
            }

            return data; //Return data table
        }
        public Boolean confirm()
        {
            return confirmation;
        }
    }
}
