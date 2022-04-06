//Tool Info

/*
Amber D. C# 2019

Code behind Tool Info form 
Allows user to view tool measurement and tool make up info
Page for Plt 8 ATTS Tool tracking app
*/
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tooling_Tracker_Info
{
    public partial class frmToolInfo : Form
    {
        public frmToolInfo()
        {              
            InitializeComponent();
        }

        //internal static cLogging _cLog = new cLogging();//Initialize logging class

        //Global Form Variables
        public static string SetValueForSearch = ""; //Static variable to pass serial number to frmPopup
        public static string SetValueForScanOrSearch = ""; //Static variable to pass serial number to frmPopup  



        private void frmToolInfo_Load(object sender, EventArgs e)
        {
            //try//Logging doesn't work right yet
            //{     // initialize logging
            //    _cLog.ApplicationName = "AMESoftwareToolboxMenu";
            //    _cLog.ApplicationOwner = "emrsn\\andavids";
            //    _cLog.ApplicationVersion = Application.ProductVersion;
            //    // _cLog.user = _user;
            //    _cLog.SqlDatabaseName = "Plt8_FixtureTracking";
            //  //  _cLog.SqlServerName = _cDB.fnGetSqlServer();
            //    _cLog.SqlPassword = "mesapp";
            //    _cLog.SqlUserID = "mespswd";

            //  //  _cDB.cLog = _cLog; // add logging to database class

            //    _cLog.fnWriteLog(System.Diagnostics.EventLogEntryType.Error, 0, "frmMain.Load", "AME Software Toolbox Menu - Good Start - "); //+ _user);

            //}
            //catch (Exception ex)
            //{
            //    int code = _cLog.fnGetErrorCode(ex); // get error code   
            //    _cLog.fnWriteLog(System.Diagnostics.EventLogEntryType.Error, code, "frmMain.Load", "ERROR! - " + ex.Message);
            //}


            loadToolList();//Option for selecting by tool
            lblScanTool.Visible = true;//Show Scan Tool Prompts
            txtScannedTool.Visible = true;
            txtScannedTool.Focus();
            lstToolSelect.SelectedIndex = -1; //Set list index
            lstCellSelect.SelectedIndex = -1; //Set list index    
        }

        private void loadToolList()
        {
            try
            {
                GetDataSet loadTools = new GetDataSet();//Create instance of GetDataSet class
                DataSet dsTools = loadTools.getToolList();//Get dataset from class

                if (dsTools != null && dsTools.Tables[0].Rows.Count > 0)//If Data returned is present
                {
                    lstToolSelect.DataSource = dsTools.Tables[0];//Bind Data to ListBox
                    lstToolSelect.DisplayMember = "toolID";
                    lstToolSelect.ValueMember = "toolID";
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Error Retrieving Data");
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Error Loading Form", "Error");
            }
        }

        private void mnuScan_Click(object sender, EventArgs e)
        {
            lblSelectTool.Visible = false;//Hide Select Tooling list
            lstToolSelect.Visible = false;
            lblSelectCell.Visible = false;//Hide Select Cell list
            lstCellSelect.Visible = false;
            lblScanTool.Visible = true;//Show Scan Tool Prompts
            txtScannedTool.Visible = true;
            txtScannedTool.Focus();
        }

        private void mnuToolingInfo_Click(object sender, EventArgs e)
        {
            lblScanTool.Visible = false;//Hide Scan Tool Prompts
            txtScannedTool.Visible = false;
            lblSelectTool.Visible = true;//Show Select Tooling list
            lstToolSelect.Visible = true;
            btnSelect.Visible = true;
            txtScannedTool.Clear();
        }

        public void btnSelect_Click(object sender, EventArgs e)
        {
            DataSet dsResultsReturned = null;
            resultsGridView.DataSource = null;//Clear Past Results
            resultsGridView2.DataSource = null;//Clear Past Results
            resultsGridView3.DataSource = null;//Clear Past Results
            resultsGridView.Visible = false;//Hide gridview
            resultsGridView2.Visible = false;//Hide gridview
            resultsGridView3.Visible = false;//Hide gridview
            resultsGridView4.Visible = false;//Hide gridview
            resultsGridView5.Visible = false;//Hide gridview
            btnExportResults.Visible = false;
            lblHeader.Visible = false;
            lblOffsets.Visible = false;
            lblComponents.Visible = false;
            lblRecords.Visible = false;
            lblDetails.Visible = false;

            if (txtScannedTool.Text.Trim().Length == 9)//Use tool serial number
            {             
                SetValueForSearch = txtScannedTool.Text;//Set static reference for frmPopup
                SetValueForScanOrSearch = "scan";

                frmPopup frmPopup = new frmPopup();//Instantiate Form for user choice
                frmPopup.ShowDialog();//Show choice Form
                dsResultsReturned = frmPopup.dsResults;//Get Dataset from Popup
            }
            
            if (lstToolSelect.SelectedIndex > -1)//Use Tool type
            {           
                SetValueForSearch = lstToolSelect.SelectedValue.ToString();//Set static reference for frmPopup
                SetValueForScanOrSearch = "type";

                frmPopup frmPopup = new frmPopup();//Instantiate Form for user choice
                frmPopup.ShowDialog();//Show choice Form
                dsResultsReturned = frmPopup.dsResults;//Get Dataset from Popup
            }

            if (dsResultsReturned != null)
            {
                try
                {
                    btnExportResults.Visible = true;//Show export button

                    if (dsResultsReturned.Tables.Count > 1 && dsResultsReturned.Tables[0].Rows.Count != 0)
                    {
                        //Populate gridviews
                        resultsGridView.DataSource = dsResultsReturned.Tables[0];
                        resultsGridView2.DataSource = dsResultsReturned.Tables[1];
                        resultsGridView3.DataSource = dsResultsReturned.Tables[2];
                        resultsGridView4.DataSource = dsResultsReturned.Tables[3];

                        resultsGridView.Visible = true;//Show gridview
                        resultsGridView2.Visible = true;//Show gridview
                        resultsGridView3.Visible = true;//Show gridview
                        resultsGridView4.Visible = true;//Show gridview
                        resultsGridView5.Visible = false
                            ;//Show gridview
                        //Remove blue highlight on 1st cell
                        resultsGridView.ClearSelection();
                        resultsGridView2.ClearSelection();
                        resultsGridView3.ClearSelection();
                        resultsGridView4.ClearSelection();

                        lblHeader.Visible = true;
                        lblOffsets.Visible = true;
                        lblComponents.Visible = true;
                        lblDetails.Visible = true;

                        btnExportResults.Visible = false;
                    }

                    if (dsResultsReturned.Tables.Count == 1 && dsResultsReturned.Tables[0].Columns.Count != 1 && dsResultsReturned.Tables[0].Rows.Count != 0)
                    {
                        {
                            lblRecords.Visible = true;
                            resultsGridView5.Visible = true;//Show gridview
                            resultsGridView5.DataSource = dsResultsReturned.Tables[0];
                            resultsGridView.Visible = false;//Hide gridview
                            resultsGridView2.Visible = false;//Hide gridview
                            resultsGridView3.Visible = false;//Hide gridview
                            resultsGridView4.Visible = false;//Hide gridview
                        }
                    }
                    txtScannedTool.Clear();//Clear data entered
                    SetValueForScanOrSearch = "";

                    if (dsResultsReturned.Tables[0].Rows.Count == 0)//Tool info returned empty
                    {
                        MessageBox.Show("No record found", "ATTS - Plt8 Tool Info");
                        txtScannedTool.Clear();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show("Error retrieving record", "Plt8 Tool Info");
                    txtScannedTool.Clear();
                }
            }          
           
            else  //(txtScannedTool.Text.Trim().Length > 9 || txtScannedTool.Text.Trim().Length < 9)
            {
                MessageBox.Show("Enter valid number", "Plt8 Tool Info");
            }
        }

        private void btnExportInfo_Click(object sender, EventArgs e)
        {
            GetDataSet toolExport = new GetDataSet();//Instantiate Class
            DataSet dsTools = toolExport.getTotalList();//Get Dataset Returned                      

            if (dsTools != null && dsTools.Tables.Count > 0 && dsTools.Tables[0].Rows.Count > 0)
            {
                try
                {   //Populate gridview
                    dgToolInfo.DataSource = dsTools.Tables[0];//Set Data into grid    
                    dgToolInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                    export tList = new export();//Instantiate export class
                    tList.buildToolSheet(dgToolInfo);//Call class method and pass gridview
                }
                catch (DataException ex)
                {
                    MessageBox.Show("Error retrieving record", "Plt8 Tool Info");
                    txtScannedTool.Clear();
                }
            }
        }
 
        private void mnuAdd_Click(object sender, EventArgs e)
        {
            frmAddTool frmAdd = new frmAddTool();//Instantiate Form for adding tool
            frmAdd.Show();//Show choice Form
            this.Hide();
        }

        private void componentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmComponents2 components = new frmComponents2();//Instantiate Form for adding tool
            components.Show();//Show choice Form
            this.Hide();
        }
        // Red X Close Event
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //frmToolInfo frmInfo = new frmToolInfo();//Instantiate Form for adding tool
            //frmInfo.Show();//Show choice Form
            System.Environment.Exit(0);
        }

        private void btnExportResults_Click(object sender, EventArgs e)
        {
            export rList = new export();//Instantiate export class
            rList.buildRecordSheet(resultsGridView5);//Call class method and pass gridview
        }

        private void machineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMachines frmMachines = new frmMachines();//Instantiate Form for adding tool
            frmMachines.Show();
            this.Hide();
        }

        private void lblDetails_Click(object sender, EventArgs e)
        {

        }
    }
}
