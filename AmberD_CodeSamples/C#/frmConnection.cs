/*
Amber D. 2021 C# Visual Studio 2022

Code behind form that allows user
to change IP and port number for PLC connection.

*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emerson_Alpha_User_Interface
{
    public partial class frmConnection : Form
    {
        public frmConnection()
        {
            InitializeComponent();
            lblIPValue.Text = Properties.Settings.Default.iProIPAddress;//Show IP to user
            lblPort2.Text = Properties.Settings.Default.iProPort.ToString();//Show IP to user
        }

        private void frmSettings_Load(object sender, EventArgs e)//Form load event
        {
            //lblIPValue.Text = Properties.Settings.Default.iProIPAddress;//Show IP to user
            //lblPort2.Text = Properties.Settings.Default.iProPort.ToString();//Show IP to user
            cmboSelect.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (cmboSelect.SelectedIndex == 0)//If setting IP Address
            {
                bool ipFormat = false;

                if (txtEnter.Text.Trim().Length >= 8)//Minimum IP address
                {
                    ipFormat = IsTextAValidIPAddress(txtEnter.Text.Trim());//Check valid IP

                    if (ipFormat)
                    {
                        Properties.Settings.Default.iProIPAddress = txtEnter.Text.Trim();//Set & save user setting for IP
                        Properties.Settings.Default.Save();
                        lblIPValue.Text = Properties.Settings.Default.iProIPAddress;//Show changes to user
                        txtEnter.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Enter IP address in correct format", "Emerson Climate Technologies - Centrifugal");
                    }
                }
            }

            if (cmboSelect.SelectedIndex == 1)//If setting Port Address
            {
                if (txtEnter.Text.Trim().Length == 3)//Minimum IP address
                {
                    foreach (char c in txtEnter.Text.Trim())
                    {
                        if (char.IsLetter(c))
                        {
                            MessageBox.Show("Enter 3 digits for port #", "Emerson Climate Technologies - Centrifugal");
                            return;
                        }
                    }
                    //If successful number
                    int port = Convert.ToInt32(txtEnter.Text.Trim());
                    Properties.Settings.Default.iProPort = port;//Set & save user setting for IP
                    Properties.Settings.Default.Save();
                    lblPort2.Text = Properties.Settings.Default.iProPort.ToString();//Show changes to user
                    txtEnter.Clear();
                }
                else
                {
                    MessageBox.Show("Enter 3 digits for port #", "Emerson Climate Technologies - Centrifugal");
                }
            }
        }
        bool IsTextAValidIPAddress(string text)//
        {
            System.Net.IPAddress test;
            return System.Net.IPAddress.TryParse(text, out test);//Test for IP format
        }

        protected override bool ProcessDialogKey(Keys keyData) //Escape key causes form to close. Cancel Option. 
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                frmLogIn.ipAddress = lblIPValue.Text;
                frmLogIn.portAddress = Convert.ToInt32(lblPort2.Text);
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
