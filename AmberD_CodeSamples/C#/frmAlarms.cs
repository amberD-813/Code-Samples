/*
Amber D. 2022 C# Visual Studio 2022

Code behind form to display error codes for Centrifugal Interface
Read static double from main form read from PLC, change to binary,
loop for each active bit. Call fileReader class to 
get alarm reason for each, then display for user.

*/


using System;
using ThreadingTimer = System.Threading.Timer;
using System.Timers;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace Emerson_Alpha_User_Interface
{
    public partial class frmAlarms : Form
    {
        public frmAlarms()
        {
            InitializeComponent();

            timer = new System.Timers.Timer(1000);//Start timer thread for reading
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(DisplayTimeEvent);
            timer.Start();
        }
        //Global Variables
        string[] alarms = new string[30];//Holds data from tags read in main form
        string[] alarms2 = new string[1];//Holds data from tags read in main form
        private System.Timers.Timer timer;//Timer for reading values from main form
        string[] errorExplained2 = new string[4];
        string message = "";
        List<string> list = new List<string>();

        private void frmAlarms_Load(object sender, EventArgs e)
        {
            lblValue.Visible = false;
            gvAlarms.Columns.Add("Alarm", "Alarms");
          //  gvAlarms.Columns[0].Width = 200;
          //  gvAlarms.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
        }


        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            alarms = FrmSpeedControl.alarms; //get alarm list from main form

            if (InvokeRequired)//Use this to prevent cross threading             
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    loadAlarms();

                }));
            }
        }
        private void loadAlarms()
        {
            try
            { //Drive Fault
                if (alarms[0] != null || alarms[0] != "0")//If drive alarm active 0-3 Reserved
                {
                    lblValue.Text = alarms[0];
                    lblValue.Visible = true;
                    lblVFDDisplay.Text = alarms[1];
                    lblVFDDisplay.Visible = true;
                    lblVFDStatusMsg.Text = alarms[2];
                    lblVFDStatusMsg.Visible = true;
                    lblVFDMeaning.Text = alarms[3];
                    lblVFDMeaning.Visible = true;
                    lblVFDAlarmPic.Visible = true;
                }
                else
                {
                    lblValue.Text = "Clear";
                    lblVFDStatusMsg.Visible = false;
                    lblVFDMeaning.Visible = false;
                    lblVFDAlarmPic.Visible = false;
                }

                //Compressor Faults
                string[] address = new string[5];
                address[0] = "10050";
                address[1] = "10051";
                address[2] = "10078";
                address[3] = "10079";
                int j = 0;

                try
                {
                    for (int x = 0; x < address.Length; x++)
                    {
                        int value = Convert.ToInt16(alarms[x + 6]);//Get error value from array
                        BitArray b = new BitArray(new int[] { value });//Change to bits
                        b.Length = 16;

                        for (int i = 0; i < 16; i++)//For each bit
                        {
                            if (b[i] == true)//If bit is high
                            {
                                int bit = i;
                                FileReader getCompInfo = new FileReader();//Invoke instance of FileReader class
                                message = getCompInfo.readErrorNum2(address[x], bit);//Get returned file data
                                if (message != null && message != "" && !list.Contains(message))
                                {
                                    list.Add(message);
                                    //alarms2[alarms2.Length] = message;
                                    gvAlarms.Rows.Add(message);
                                    picCompALarms.Visible = true;
                                    gvAlarms.ClearSelection();
                                    gvAlarms.EnableHeadersVisualStyles = false;
                                    gvAlarms.Columns[0].HeaderCell.Style.BackColor = Color.Red;
                                    gvAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                                }              
                            }

                        }

                    }                 

                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch (IndexOutOfRangeException ex2)
                {
                    MessageBox.Show(ex2.ToString(), "Error");
                }

            }
            catch (NullReferenceException ex)
            {
                timer.Stop();
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
                this.Close();
            }
            catch (Exception ex2)
            {
                timer.Stop();
                MessageBox.Show(ex2.ToString(), "Emerson Climate Technologies");
                this.Close();
            }
        }
        protected override bool ProcessDialogKey(Keys keyData) //Escape key causes form to close. Cancel Option. 
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();          
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string address = "10103";
            bool success = false;

            writeModbus reset = new writeModbus(1, address, frmLogIn.busTcpClient);
            success = reset.writeData();
            if (success == true)
            {
                gvAlarms.Columns.Clear();//Clear gridview
                list.Clear();//Clear List
                gvAlarms.Columns.Add("Alarm", "Alarms");
                //this.Close();//Close form
            }
        }
    }
}
