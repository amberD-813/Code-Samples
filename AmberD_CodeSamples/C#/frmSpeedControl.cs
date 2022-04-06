/*Emerson Centrifugal User Interface v1
  Developed in C# Visual Studio v2019
  by Amber Davidson last updated 2/3/2022

*/
using System;
using System.Collections.Generic;
using System.Drawing;
using HslCommunication.ModBus;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using ThreadingTimer = System.Threading.Timer;
using System.Windows.Forms;
using System.Timers;
using System.Drawing.Drawing2D;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Microsoft.VisualBasic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.IO;

namespace Emerson_Alpha_User_Interface
{
    public partial class FrmSpeedControl : Form
    {
        public FrmSpeedControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Application.EnableVisualStyles();

            //Get current time 
            System.Windows.Forms.Timer t = null;
            t = new System.Windows.Forms.Timer();
            t.Enabled = true;
            t.Interval = 1000;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;

            //Set display boxes to show currnet time
            void t_Tick(object sender, EventArgs e)
            {
                dt5 = DateTime.Now;
                txtBxDate.Text = dt5.ToString("MM/dd/yyyy");
                txtBxTime.Text = dt5.ToString("hh:mm:ss tt");
            }

            //Chart setup. One time only
            // ------CHART STUFF ------
            //Plot elapsed milliseconds and value
            var mapper = Mappers.Xy<driveChart>()
                .X(x => x.ElapsedMilliseconds)
                .Y(x => x.Value);

            //save the mapper globally         
            Charting.For<driveChart>(mapper);

            chrtDriveData.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Seconds",
                Foreground = System.Windows.Media.Brushes.Black,
                //MaxRange = 40
            });
            chrtDriveData.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                MinValue = 0,
                Foreground = System.Windows.Media.Brushes.Black,
            });

            setUpChart();//Method for loading chart series

            //Setup gauge
            //spdGage.SectionsInnerRadius = 500;
            // spdGauge.ToColor = System.Windows.Media.Colors.Yellow;
            // spdGauge.ToColor = System.Windows.Media.Colors.Yellow;
            spdGauge.Stroke = System.Windows.Media.Brushes.Gray;
            spdGauge.StrokeThickness = 1;
            spdGauge.InnerRadius = 38;
           // spdGauge.Width = 400;
           // spdGauge.Height = 90;

            spdGauge.GaugeActiveFill = new System.Windows.Media.LinearGradientBrush
            {
                GradientStops = new System.Windows.Media.GradientStopCollection
                {
                     new System.Windows.Media.GradientStop(System.Windows.Media.Colors.Green,0),
                    // new System.Windows.Media.GradientStop(System.Windows.Media.Colors.Red,20)
                }
            };

        }

        //Global Variables      
        DateTime dt5;
        public static System.Timers.Timer timer;//Timer for PLC read loop
        string[] updateData;
        public static string[] addressArray = new string[20];//IP is [19]
        ushort[] lengthArray = new ushort[10];
        string[] driveInfoArray = new string[10];
        public static bool stop;//Stops threading timers when true;
        bool validConn = false;
        bool VIGVclearing = false;
        int angPic = 0;

        public static bool loggedIn;
        public static bool logInSkipped;
        bool stopping = false;
        bool VFD_Online = false;
        bool VFD_Running = false;
        bool startTrend = false;
        double time;
        public static string[] alarms = new string[30];//Holds data from tags read in main form read by alarm form
        string[] errorExplained = new string[4]; //Holds returned values from drive error codes from XML file. Other alarms analyzed in alarm form
        private static string lastError = "";

        private void frmSpeedControl_Load(object sender, EventArgs e)//Form load event
        {
            assignAddresses();//Call method to load address array           
            
            timer = new System.Timers.Timer(1000);//Start timer thread for reading
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(DisplayTimeEvent);
            timer.Stop();

            //Display Date and Time
            DateTime dateTime = DateTime.Now; //Set Job Date Calender Control to Todays Date
            txtBxDate.Text = dateTime.ToString("MM/dd/yyyy");
            txtBxTime.Text = dateTime.ToString("hh:mm:ss");

            validConn = checkConnection();//Double Check Connection to PLC  

            btnUnitsPsi.Text = Properties.Settings.Default.unitsPsi;//Show PSIA/PSIG selection
            btnUnitsTemp.Text = Properties.Settings.Default.unitsTemp;//Show PSIA/PSIG selection
            updateUnitLabels();//Method to update unit labels for reading         

            btnStart.BackColor = Color.Green;

            //Check Boxes (Set for LTR#1 Temps)
            chkBxSpdSP.Checked = false;
            chkSpeedACT.Checked = false;
            chkBxSP.Checked = false;
            chkBxDP.Checked = false;
            chkBxIP.Checked = false;
            chkBxIT.Checked = false;
            chkBxSt.Checked = false;
            chkBxDT.Checked = false;
            chkBxMT.Checked = false;
            chkBxMcT.Checked = false;
            chkBxVDC.Checked = false;
            chkBxAmps.Checked = false;
            chkBxVDC.Checked = false;
            chkBxKw.Checked = false;
            chkBxTemp.Checked = false;
            chkBxCond.Checked = false;
            chkBxET.Checked = false;
            chkBxPR.Checked = false;
            chkBxStart.Checked = false;//Chart Start unchecked
            chkBxGridview.Checked = false;
            chkBxClrChart.Checked = false;
            chkBxRecord.Checked = false;

            //Add Columns to gridview for export / save
            driveGridView.Columns.Add("Time", "Time");//Add Columns to DataTable
            driveGridView.Columns.Add("KW", "KW");
            driveGridView.Columns.Add("AMPs", "AMPs");
            driveGridView.Columns.Add("DC Volts", "DC Volts");
            driveGridView.Columns.Add("Temp", "Temp");
            driveGridView.Columns.Add("Speed SetP", "Speed SetP");
            driveGridView.Columns.Add("Speed ACT", "Speed ACT");
            driveGridView.Columns.Add("PR", "PR");
            driveGridView.Columns.Add("SuctP", "SuctP");
            driveGridView.Columns.Add("IntP", "Speed ACT");
            driveGridView.Columns.Add("DischP", "DischP");
            driveGridView.Columns.Add("ST", "ST");
            driveGridView.Columns.Add("IT", "IT");
            driveGridView.Columns.Add("DT", "DT");
            driveGridView.Columns.Add("MT", "MT");
            driveGridView.Columns.Add("McT", "McT");
            driveGridView.Columns.Add("EV", "EV");
            driveGridView.Columns.Add("CN", "CN");
            driveGridView.Columns.Add("RB1", "RB1");
            driveGridView.Columns.Add("RB2", "RB2");
            driveGridView.Columns.Add("THB", "THB");
            driveGridView.Columns.Add("UnitsT", "UnitsT");
            driveGridView.Columns.Add("UnitsP", "UnitsP");

            driveGridView.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(driveGridView, true, null);


        }
        private void setUpChart()
        {        
            //Add series for each data point needed
            chrtDriveData.Series = new LiveCharts.SeriesCollection
              {
                  //new LineSeries
                  new LineSeries
                  {
                      Title = "KW",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.DarkRed,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "Amp",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Green,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "DC Volts",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Orange,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                    new LineSeries
                  {
                      Title = "Temp",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Blue,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "DP",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Red,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "SP",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.LightSkyBlue,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "IP",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Yellow,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "DT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Indigo,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "ST",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.LightPink,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                 new LineSeries
                  {
                      Title = "IT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.LawnGreen,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                 new LineSeries
                  {
                      Title = "MT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.DarkGray,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                 new LineSeries
                  {
                      Title = "McT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Beige,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "ET",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.OliveDrab,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "CT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Gold,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "RB1",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.SteelBlue,
                      Fill = System.Windows.Media.Brushes.Transparent,

                  },
                   new LineSeries
                  {
                      Title = "RB2",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Brown,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "THB",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Crimson,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "SPD_ACT",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Azure,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                  new LineSeries
                  {
                      Title = "SPD_SP",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Peru,
                      Fill = System.Windows.Media.Brushes.Transparent
                  },
                   new LineSeries
                  {
                      Title = "PR",
                      Values =  new ChartValues<driveChart>(),
                      PointGeometrySize = 0,
                      Stroke = System.Windows.Media.Brushes.Silver,
                      Fill = System.Windows.Media.Brushes.Transparent
                  }
               };

           

            //Initialize Chart Properties 
            //chrtDriveData.ScrollBarFill = System.Windows.Media.Brushes.Gray;
            // chrtDriveData.ScrollMode = ScrollMode.Y;
            // chrtDriveData.ScrollHorizontalFrom = 0;
            // chrtDriveData.Zoom = 20;
            //Set up Legend for Chart
            //DefaultLegend lg = new DefaultLegend();
            //lg.Foreground = System.Windows.Media.Brushes.Gray;
            //lg.Background = System.Windows.Media.Brushes.Gray;
            //chrtDriveData.DefaultLegend = lg;
            //chrtDriveData.LegendLocation = LegendLocation.Top;

        }
        private void assignAddresses()
        {
            addressArray[0] = "10048";//Read - Status, 0, Comp Alarm1, Comp Alarm2,Max Power,Surge,Choke,Act Demand, Req Power,Act,0,Req Speed,Act Speed (13)
            addressArray[1] = "10064";//Read - VFD DCV,Current, Temp (3)
            addressArray[2] = "10078";//Read Compressor Faults (2)
            addressArray[3] = "10103";//Fault Reset, Evap exit temp, Cond exit temp, Op.Modes (4)
            addressArray[4] = "10150";//Read SuctP, IntP, DischP, PR, ViP, 0,0,SuctT, IntT, DischT, CoolingT, MotorT, RB1T, RB2T, ThrustT,ViT, VIGV1, VIGV2(20)
            addressArray[5] = "10304";//Read VIGV Alarms,VFD Online (3)
            addressArray[6] = "10309";//Read VIGV Angles and setpoints (4)
            //addressArray[6] = "10307";//Read VFD Fault Code (1)
                                      //addressArray[7] = "10000";Read Model Serial (Future) //(read/write) gASCII_Array_Model
            addressArray[7] = "10302";//Write VIGV1 Alarm Reset(1)
            addressArray[8] = "10303";//Write VIGV2 Alarm Reset(1)
            addressArray[9] = "10311";//Write VIGV1 Set(1)
            addressArray[10] = "10312";//Write VIGV2 Set(1)
            addressArray[11] = "10314";//Interface spped command
            addressArray[12] = "10316";//VFD Fault Code
            addressArray[13] = "10315";//VFD Run Command

            lengthArray[0] = 13;//Lengths to read
            lengthArray[1] = 3;
            lengthArray[2] = 2;
            lengthArray[3] = 4;
            lengthArray[4] = 16;
            lengthArray[5] = 3;
            lengthArray[6] = 1;
            //lengthArray[7] = 12;
        }

        private void showDisconnected()
        {
            lblPLCStatus.ForeColor = Color.Red;
            lblPLCStatus.Text = "Disconnected";
            lblDriveStatus.ForeColor = Color.Red;
            lblDriveStatus.Text = "Disconnected";
            //lblDischTempRead.Visible = false;//Hide suction and discharge
            //lblDischPSIRead.Visible = false;
            //lblSuctPSIRead.Visible = false;
           // lblSuctTempRead.Visible = false;
            //lblPRValue.Visible = false;
            btnStart.BackColor = Color.Gray;
            btnStop.BackColor = Color.Gray;
           // lblVIGV1act.Visible = false;
           // lblsymbol.Visible = false;
            //lblSymbol2.Visible = false;
           // lblVIGV2tar.Visible = false;
        }
        public bool checkConnection()
        {
            bool connect = false;
            try
            {
                if (frmLogIn.busTcpClient != null)//Check connection valid
                {
                    stop = false;
                    timer.Start();//Start Timer thread 
                    connect = true;

                    //Show Active. This is from when connect was done from this page. 
                    lblPLCStatus.ForeColor = Color.Green;
                    lblPLCStatus.Text = "Connected";
                    lblDischTempRead.Visible = true;//Show suction and discharge
                    lblDischPSIRead.Visible = true;
                    lblSuctPSIRead.Visible = true;
                    lblSuctTempRead.Visible = true;
                    lblPRValue.Visible = true;
                    btnStart.Enabled = true;
                    btnStop.Enabled = true;
                   // btnLogin.Text = "Log Out";
                }
                else if (frmLogIn.busTcpClient == null)//Check connection valid
                {
                    stop = true;
                    //    timer.Stop(); //Stop Timer thread   
                    showDisconnected();//Update user visuals
                    connect = false;
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }

            return connect;
        }

        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            string[] updateData2 = null; //Final array to hold converted data retrieved from iPro

            updateData2 = pollData();//Call method to monitor data

            if (InvokeRequired && !stop)//Use this to prevent cross threading             
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        //ipAddress = Properties.Settings.Default.iProIPAddress;//Update changes made on settings form

                        if (loggedIn)//Advanced user option
                        {
                            //Add changes here later. What?
                            btnHelp.Text = "Log Out";
                        }
                        else
                        {
                            //Add changes here later. What?
                        }

                        if (updateData2[0] != null)
                        {
                            for (int i = 0; i < updateData2.Length; i++)
                            {                              
                                if (updateData2[i] == null || updateData2[i] == " ")
                                {
                                    updateData2[i] = "0";//Set empty values to 0 to avoid null value runtime errors
                                }
                            }

                            //Status, Comp. Alarms, Speeds
                            int driveStat = Convert.ToInt32(updateData2[0]);//Status //0 = Stopped, 1 = Compressor Ready, 2 = Compressor Starting, 3 = Active Control
                                                                            //6 = Compressor Stopping, 7 = VFD Ready, 8 = VFD Running , 9 = VLV Cool , 10 = VLV motor,
                                                                            //11 = VLV LBW, 12 = , 13 = , 14 = , 15 =                           

                            decimal numS = decimal.Parse(updateData2[4].ToString(), System.Globalization.NumberStyles.Currency);//Add comma
                            lblSurgeVal.Text = numS.ToString("#,#");//Display Surge speed                            
                            decimal numC = decimal.Parse(updateData2[5].ToString(), System.Globalization.NumberStyles.Currency); //Add comma
                            lblChokeVal.Text = numC.ToString("#,#");//Display Choke speed
                            decimal numSP = decimal.Parse(updateData2[9].ToString(), System.Globalization.NumberStyles.Currency); //Add comma                                      //
                            lblSetPoint.Text = numSP.ToString("#,#");//Display speed setpoint
                            if (lblSetPoint.Text == "")
                            {
                                lblSetPoint.Text = "0";
                            }
                            decimal numSV = decimal.Parse(updateData2[10].ToString(), System.Globalization.NumberStyles.Currency); //Add comma 
                            lblSpeedVal.Text = numSV.ToString("#,#");//Display actual speed

                            //Angular Gage                          
                            spdGauge.From = Convert.ToDouble(numS);
                            spdGauge.To = Convert.ToDouble(numC);
                            if (numSV > numS && numSV < numC)
                            {
                                spdGauge.Value = Convert.ToDouble(numSV);
                            }


                            //spdGauge.ClipToBounds = true;

                            //spdGauge.SetCurrentValue.Add(new AngularSection()
                            //{
                            //   ToValue = Convert.ToDouble(numS + 1000),

                            //});



                            if (Convert.ToDouble(updateData2[10]) > 0)//If speed > 0, spin impeller
                            {
                                Image img = picImpeller.Image;
                                picImpeller.Image = img;
                                if (angPic == 0)
                                {
                                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);//CCW 90
                                    angPic += 1;
                                }
                                else if (angPic == 1)
                                {
                                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);//CCW 90
                                    angPic += 1;
                                }
                                //else if (angPic == 2)
                                //{
                                //    img.RotateFlip(RotateFlipType.Rotate90FlipNone);//CCW 270                                  
                                //}
                                else if (angPic == 2)
                                {
                                    img.RotateFlip(RotateFlipType.RotateNoneFlipNone);//CCW 270
                                    angPic = 0;//Reset count
                                }

                            }

                            //Comp. Status and Faults
                            double compStatus = Convert.ToDouble(updateData2[0]);
                            double compAlarm1 = Convert.ToDouble(updateData2[1]);
                            double compAlarm2 = Convert.ToDouble(updateData2[2]);
                            double compFault1 = Convert.ToDouble(updateData2[14]);                              
                            double compFault2 = Convert.ToDouble(updateData2[15]);
                            if (compAlarm1 > 0 ||compAlarm2 > 0|| compFault1 > 0 || compFault2 > 0)//SHOW ALARM INDICATIN ON MAIN PAGE
                            {
                                compressorAlarmPic.Visible = true;
                            }
                            else
                            {
                                compressorAlarmPic.Visible = false; 
                            }
                            //Static Array to be analyzed by alarm page                  
                            alarms[5] = updateData[0];//Comp Status
                            alarms[6] = updateData[1];//Alarms 1
                            alarms[7] = updateData[2];//Alarms 2
                            alarms[8] = updateData[14];//Faults 1
                            alarms[9] = updateData[15];//Faults 2

                            //Reset Faults(Read), Evap & Cond temp, Mode
                                string resetCompFaults = updateData2[16];
                                string evapExitT = updateData2[17];
                                string condExitT = updateData2[18];
                                string compMode = updateData2[19];

                                //Temps & PSI
                                double ev = (Convert.ToDouble(updateData2[17].ToString()) / 10);//Evap Exit Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    ev = (ev - 32) * 5 / 9;
                                    ev = Math.Round(ev, 1);//Round to 1 decimal place
                                }
                                lblEvapT.Text = Convert.ToString(ev);
                                double cn = (Convert.ToDouble(updateData2[18].ToString()) / 10);//Cond Exit Temp                          
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    cn = (cn - 32) * 5 / 9;
                                    cn = Math.Round(cn, 1);//Round to 1 decimal place
                                }
                                lblCondT.Text = Convert.ToString(cn);
                                double sp = (Convert.ToDouble(updateData2[20].ToString()) / 10);//Suct Pressure
                                if (btnUnitsPsi.Text == "psiG")
                                {
                                    sp = (sp - 14.7);//Convert to gage
                                    sp = Math.Round(sp, 1);//Round to 1 decimal place
                                }
                                lblSuctPSIRead.Text = Convert.ToString(sp);//Suct Pressure
                                double ip = (Convert.ToDouble(updateData2[21].ToString()) / 10);//Int Pressure
                                if (btnUnitsPsi.Text == "psiG")
                                {
                                    ip = (ip - 14.7);//Convert to gage
                                    ip = Math.Round(ip, 1);//Round to 1 decimal place
                                }
                                lblIntPsi.Text = Convert.ToString(ip);
                                double dp = (Convert.ToDouble(updateData2[22].ToString()) / 10);//Discharge Pressure
                                if (btnUnitsPsi.Text == "psiG")
                                {
                                    dp = (dp - 14.7);//Convert to gage
                                    dp = Math.Round(dp, 1);//Round to 1 decimal place
                                }
                                lblDischPSIRead.Text = Convert.ToString(dp);
                                double pr = (Convert.ToDouble(updateData2[23].ToString()) / 100);//PR 
                                pr = Math.Round(pr, 2);//Round PR to 1 decimal place
                                lblPRValue.Text = Convert.ToString(pr);//Pressure Ratio
                                //double vi = (Convert.ToDouble(updateData2[24].ToString()) / 10);// Vapor Inj.Pressure
                                //if (btnUnitsPsi.Text == "psiG")
                                //{
                                //    vi = (vi - 14.7);//Convert to gage
                                //    vi = Math.Round(vi, 1);//Round to 1 decimal place
                                //}
                                //lblViPsi.Text = Convert.ToString(vi);//Vapor Inj. Pressure   -- Future Use
                                double st = (Convert.ToDouble(updateData2[25].ToString()) / 10);//Suction Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    st = (st - 32) * 5 / 9;
                                    st = Math.Round(st, 1);//Round to 1 decimal place
                                }
                                lblSuctTempRead.Text = Convert.ToString(st);
                                double it = (Convert.ToDouble(updateData2[26].ToString()) / 10);//Intermediate Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    it = (it - 32) * 5 / 9;
                                    it = Math.Round(it, 1);//Round to 1 decimal place
                                }
                                lblIntTemp.Text = Convert.ToString(it);
                                double dt = (Convert.ToDouble(updateData2[27].ToString()) / 10);//Discharge Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    dt = (dt - 32) * 5 / 9;
                                    dt = Math.Round(dt, 1);//Round to 1 decimal place
                                }
                                lblDischTempRead.Text = Convert.ToString(dt);
                                double mc = (Convert.ToDouble(updateData2[28].ToString()) / 10);//Motor Cooling Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    mc = (mc - 32) * 5 / 9;
                                    mc = Math.Round(mc, 1);//Round to 1 decimal place
                                }
                                lblMcT.Text = Convert.ToString(mc);//Set Text
                                double mt = (Convert.ToDouble(updateData2[29].ToString()) / 10);//Motor Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    mt = (mt - 32) * 5 / 9;
                                    mt = Math.Round(mt, 1);//Round to 1 decimal place
                                }
                                lblMotorT.Text = Convert.ToString(mt);
                                double rb1 = (Convert.ToDouble(updateData2[30].ToString()) / 10);//RB1 Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    rb1 = (rb1 - 32) * 5 / 9;
                                    rb1 = Math.Round(rb1, 1);//Round to 1 decimal place
                                }
                                lblRb1T.Text = Convert.ToString(rb1);
                                double rb2 = (Convert.ToDouble(updateData2[31].ToString()) / 10);//RB2 Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    rb2 = (rb2 - 32) * 5 / 9;
                                    rb2 = Math.Round(rb2, 1);//Round to 1 decimal place
                                }
                                lblRb2T.Text = Convert.ToString(rb2);
                                double thb = (Convert.ToDouble(updateData2[32].ToString()) / 10);//THB Temp
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    thb = (thb - 32) * 5 / 9;
                                    thb = Math.Round(thb, 1);//Round to 1 decimal place
                                }
                                LblThb.Text = Convert.ToString(thb);
                                //double vt = (Convert.ToDouble(updateData2[24].ToString()) / 10);//Vapor Inj. Temp -- Future Use
                                //if (btnUnitsTemp.Text == "C")//Celcius Convert
                                //{
                                //    vt = (vt - 32) * 5 / 9;
                                //    vt = Math.Round(vt, 1);//Round to 1 decimal place
                                //}
                                //lblVITemp.Text = Convert.ToString(vt); // -- Future Use

                                //VIGV Alarm status ***Add VIGV2
                                if (updateData2[34] == "True" && VIGVclearing == false)//If gC1_VIGV1_Alarm = 1 
                                {
                                    picVIGV1Fault.Visible = true;

                                }
                                else if (updateData[34] == "False")
                                {
                                    picVIGV1Fault.Visible = false;

                                    if (VIGVclearing)
                                    {
                                        bool clearAlarm = false;
                                        bool successClear = false;
                                        writeModbus clearReset = new writeModbus(clearAlarm, addressArray[7], frmLogIn.busTcpClient);//Create instance of class
                                        successClear = clearReset.writeAction();//Class method to complete action

                                        if (successClear)
                                        {
                                            VIGVclearing = false;//Fault Reset Done
                                            picVIGV1Fault.Visible = false;//Hide alarm
                                            picVIGV1_Resetting.Visible = false;
                                        }
                                    }
                                }

                                //VIGV Angles 
                                lblVIGV1act.Text = updateData2[37].ToString();
                                lblVIGV2act.Text = updateData2[38].ToString();
                                lblVIGV1tar.Text = updateData2[39].ToString();
                                lblVIGV2tar.Text = updateData2[40].ToString();

                                //VFD INFO
                                double kw = (Convert.ToDouble(updateData2[8].ToString()) / 10);
                                lblKW.Text = Convert.ToString(kw); //Power


                                double dcv = (Convert.ToInt32(updateData2[11].ToString()));
                                lblDCvolts.Text = Convert.ToString(dcv); //DC Volts


                                double amp = (Convert.ToDouble(updateData2[12].ToString()) / 10);
                                lblAmps.Text = Convert.ToString(amp);//Amps
                                double vfdT = (Convert.ToDouble(updateData2[13].ToString()) / 10);
                                if (btnUnitsTemp.Text == "C")//Celcius Convert
                                {
                                    vfdT = (vfdT - 32) * 5 / 9;
                                    vfdT = Math.Round(ev, 1);//Round to 1 decimal place
                                }

                                lblVFDT.Text = Convert.ToString(vfdT);//Show Temp

                            if (Convert.ToDouble(updateData2[13].ToString()) / 10 > 185)//Check for drive temp too high
                                {
                                    picVFDTemp.Visible = true;
                                }
                                else
                                {
                                    picVFDTemp.Visible = false;
                                }

                            lblVFDFaultCode.Text = updateData2[42];//Display Fault Code for FVD
                            
                            if (lblVFDFaultCode.Text != "0")
                                {
                                    lblVFDAlarmPic.Visible = true;
                                    findVFDCode();
                                }

                                if (chkBxRecord.Checked && !chkBxClrChart.Checked)
                                {
                                //Collect rows in Gridview
                                    string unitP = Properties.Settings.Default.unitsPsi;
                                    string unitT = Properties.Settings.Default.unitsTemp;
                                    string exportTime = dt5.ToString();
                                    driveGridView.Rows.Add(exportTime, kw, amp, dcv, vfdT, numSP, numSV, pr, sp, ip, dp, st, it, dt, mt, mc, ev, cn, rb1, rb2, thb, unitP, unitT);
                                    driveGridView.ClearSelection();//Remove Blue from Gridview
                              
                                int intDisplayRows = driveGridView.RowCount -1 ;
                                if(driveGridView.FirstDisplayedScrollingRowIndex >= driveGridView.RowCount - 35)
                                {
                                    driveGridView.FirstDisplayedScrollingRowIndex = intDisplayRows;
                                }                                  
                        }                           

                            if (startTrend)
                            {
                                //Chart Actions
                                time += 1;//X Axis Interval
                                if (time > 60)
                                {
                                    chrtDriveData.AxisX[0].MinValue = time - 60;//Change Axis
                                    chrtDriveData.AxisX[0].MaxValue = time;

                                   // chrtDriveData.Series.Clear();
                                   // setUpChart();//Method for loading chart series
                                }
                                else
                                {
                                    chrtDriveData.AxisX[0].MinValue = 0;
                                    chrtDriveData.AxisX[0].MaxValue = time;
                                }                               

                                if (chkBxKw.Checked)
                                {
                                    chrtDriveData.Series[0].Values.Add(new driveChart//KW
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = kw
                                    });
                                    if (chrtDriveData.Series[16].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[0].Values.RemoveAt(0);
                                    }

                                }
                                if (chkBxAmps.Checked)
                                {
                                    chrtDriveData.Series[1].Values.Add(new driveChart//Amp
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = amp
                                    });
                                    if (chrtDriveData.Series[1].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[1].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxVDC.Checked)
                                {


                                    chrtDriveData.Series[2].Values.Add(new driveChart//DC Volts
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = dcv,
                                    });
                                    if (chrtDriveData.Series[2].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[2].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxTemp.Checked)
                                {
                                    chrtDriveData.Series[3].Values.Add(new driveChart//Temp
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = vfdT
                                    });
                                    if (chrtDriveData.Series[3].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[3].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxDP.Checked)
                                {
                                    chrtDriveData.Series[4].Values.Add(new driveChart//Disch P
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = dp
                                    });
                                    if (chrtDriveData.Series[4].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[4].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxSP.Checked)
                                {
                                    chrtDriveData.Series[5].Values.Add(new driveChart//Suct P
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = sp
                                    });
                                    if (chrtDriveData.Series[5].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[5].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxIP.Checked)
                                {
                                    chrtDriveData.Series[6].Values.Add(new driveChart//Int P
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = ip
                                    });
                                    if (chrtDriveData.Series[6].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[6].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxDT.Checked)
                                {
                                    chrtDriveData.Series[7].Values.Add(new driveChart//Disch T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = dt
                                    });
                                    if (chrtDriveData.Series[7].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[7].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxSt.Checked)
                                {
                                    chrtDriveData.Series[8].Values.Add(new driveChart//Int T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = it
                                    });
                                    if (chrtDriveData.Series[8].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[8].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxIT.Checked)
                                {
                                    chrtDriveData.Series[9].Values.Add(new driveChart//Suct T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = st
                                    });
                                    if (chrtDriveData.Series[9].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[9].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxMT.Checked)
                                {
                                    chrtDriveData.Series[10].Values.Add(new driveChart//Motor T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = mt
                                    });
                                    if (chrtDriveData.Series[10].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[10].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxMcT.Checked)
                                {
                                    chrtDriveData.Series[11].Values.Add(new driveChart//MC T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = mc
                                    });
                                    if (chrtDriveData.Series[11].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[11].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxET.Checked)
                                {
                                    chrtDriveData.Series[12].Values.Add(new driveChart//E T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = ev
                                    });
                                    if (chrtDriveData.Series[12].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[12].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxCond.Checked)
                                {
                                    chrtDriveData.Series[13].Values.Add(new driveChart//C T
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = cn
                                    });
                                    if (chrtDriveData.Series[13].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[13].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxRB1.Checked)
                                {
                                    chrtDriveData.Series[14].Values.Add(new driveChart//RB1
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = rb1
                                    });
                                    if (chrtDriveData.Series[14].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[14].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxRB2.Checked)
                                {
                                    chrtDriveData.Series[15].Values.Add(new driveChart//RB1
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = rb2
                                    });
                                    if (chrtDriveData.Series[15].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[15].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxTHB.Checked)
                                {
                                    chrtDriveData.Series[16].Values.Add(new driveChart//RB1
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = thb
                                    });
                                    if (chrtDriveData.Series[16].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[16].Values.RemoveAt(0);
                                    }
                                }
                                if (chkSpeedACT.Checked)
                                {
                                    chrtDriveData.Series[17].Values.Add(new driveChart//Speed ACT
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = Convert.ToDouble(numSV)
                                    });
                                    if (chrtDriveData.Series[17].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[17].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxSpdSP.Checked)
                                {
                                    chrtDriveData.Series[18].Values.Add(new driveChart//Speed SP
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = Convert.ToDouble(numSP)
                                    });
                                    if (chrtDriveData.Series[18].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[18].Values.RemoveAt(0);
                                    }
                                }
                                if (chkBxPR.Checked)
                                {
                                    chrtDriveData.Series[19].Values.Add(new driveChart//PR
                                    {
                                        ElapsedMilliseconds = time,
                                        Value = Convert.ToDouble(pr)
                                    });
                                    if (chrtDriveData.Series[19].Values.Count > 60)
                                    {
                                        chrtDriveData.Series[19].Values.RemoveAt(0);
                                    }
                                }
                            }

                            //VFD_Online
                            if (updateData2[36] == "True")
                            {
                                VFD_Online = true;
                            }
                            else
                            {
                                VFD_Online = false;
                                lblDriveStatus.Text = "Disconnected";
                                lblDriveStatus.ForeColor = Color.Red;
                                if (VFD_Online)
                                {
                                    MessageBox.Show("VFD Disconnected!");                                  
                                    lblDCV.Text = "0";
                                    lblAmps.Text = "0";
                                    lblKW.Text = "0";
                                    lblVFDT.Text = "0";
                                }
                            }                               

                            if (updateData2[36] == "True" && updateData2[14] == "0" && updateData2[10] == "0")//VFD Online, no faults, and speed ACT = 0
                            {
                                lblDriveStatus.Text = "Ready";
                                lblDriveStatus.ForeColor = Color.Green;
                                btnStart.Enabled = true;
                                btnStart.BackColor = Color.Green;
                                //btnStop.Enabled = false;
                                btnStop.BackColor = Color.Gray;
                            }
                            if ((updateData2[36] == "True" && updateData2[14] == "0" && updateData2[10] != "0") || updateData2[43] == "True")//VFD Online, no faults, and speed ACT > 0 or Running ON
                            {
                                lblDriveStatus.Text = "Active";
                                lblDriveStatus.ForeColor = Color.Green;
                                lblCompStat.Text = "Running";
                                lblCompStat.ForeColor = Color.Green;
                                btnStart.Enabled = false;
                                btnStart.BackColor = Color.Gray;
                                btnStop.Enabled = true;
                                btnStop.BackColor = Color.Red;
                                VFD_Running = true;//Track compressor is running
                            }
                            if (updateData2[10] == "0" && updateData[9] == "0")
                            {
                                btnStart.Enabled = true;
                                btnStart.BackColor = Color.Green;
                                btnStop.Enabled= false;
                                btnStop.BackColor= Color.Gray;
                                VFD_Running = false;//Track compressor is running
                            }

                            //if (driveInfoArray[4] == "True")//Check for drive fault active to indicate to user
                            //{
                            //    lblDriveFault.Visible = true;
                            //    picDriveFault.Visible = true;
                            //    Stop();//Ensure run signal is not active

                            //    lblDriveStatus.ForeColor = Color.Red;
                            //    lblDriveStatus.Text = "Not Ready";
                            //    btnStart.BackColor = Color.Gray;
                            //    btnStop.BackColor = Color.Gray;
                            //    btnStart.Enabled = false;
                            //    btnStop.Enabled = false;
                            //    stopping = false;
                            //}


                            //if (!driveStat || lblPLCStatus.ForeColor == Color.Red)//If drive not ready OR PLC not connected
                            //{
                            //    lblDriveStatus.ForeColor = Color.Red;
                            //    lblDriveStatus.Text = "Not Ready";
                            //    btnStart.BackColor = Color.Gray;
                            //    btnStop.BackColor = Color.Gray;
                            //    btnStart.Enabled = false;
                            //    btnStop.Enabled = false;
                            //    stopping = false;
                            //}
                            //if (driveStat && (!driveRunning && !stopping) && !runStatus)//If drive ready but not running, not stopping and no run command
                            //{                             
                            //    lblDriveStatus.Text = "Ready";
                            //    lblDriveStatus.ForeColor = Color.Green;
                            //    btnSetSpeed.Enabled = true;//Allow speed to be set
                            //    btnStart.BackColor = Color.Green;
                            //    btnStop.BackColor = Color.Gray;
                            //    btnStart.Enabled = true;
                            //    btnStop.Enabled = false;
                            //    stopping = false;
                            //}

                            //if ((!driveRunning || !stopping) && runStatus && driveStat)//If drive is not running and but has run command           
                            //{
                            //    lblDriveStatus.ForeColor = Color.Green;
                            //    lblDriveStatus.Text = "Running";
                            //    btnStart.BackColor = Color.Gray;
                            //    btnStop.BackColor = Color.Red;
                            //    btnStart.Enabled = false;
                            //    btnStop.Enabled = true;

                            //    Image img = picImpeller.Image;
                            //    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            //    picImpeller.Image = img;
                            //}
                            //if (stopping)//If drive is running and has  no run command but has stop command
                            //{
                            //    lblDriveStatus.ForeColor = Color.Red;
                            //    lblDriveStatus.Text = "Stopping";
                            //    btnStart.BackColor = Color.Gray;
                            //    btnStop.BackColor = Color.Gray;
                            //    btnStart.Enabled = false;
                            //    btnStop.Enabled = false;

                            //    if (!driveRunning && !runStatus)
                            //    {
                            //        stopping = false;
                            //    }
                            //}

                        }
                        }
                    
                    catch (NullReferenceException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();//Stop thread
                        showDisconnected();
                        MessageBox.Show(ex.ToString());
                    }
                }));
            }
        }
        private string[] pollData()//Meythod to read data every second
        {
            readModbus data = new readModbus(addressArray, lengthArray, frmLogIn.busTcpClient);//Declare Instance of class
            updateData = data.readExecute(stop);//call class method for data return

            return updateData;
        }
        //private void mnuDisconnect_Click(object sender, EventArgs e)//Disconnect Modbus TCP
        private void picDisconnect_Click(object sender, EventArgs e)
        {
            frmLogIn.busTcpClient.Dispose();

            showDisconnected();//Update user visuals

            stop = true;//Signal to stop
            timer.Stop();//Stop polling for PLC data         
        }

        private void lblVIGV1tar_Click(object sender, EventArgs e)//Stage 1 Only
        {
            frmVIGV1 A1 = new frmVIGV1();
            frmVIGV1.stage = 1;//Set Stage for write
            A1.ShowDialog();
        }
        private void lblVIGV2tar_Click(object sender, EventArgs e)
        {
            frmVIGV1 A2 = new frmVIGV1();
            frmVIGV1.stage = 2;//Set Stage for write
            A2.ShowDialog();
        }
        private void btnStart_Click(object sender, EventArgs e) //
        {
            bool gVis_Run = true;//Variable true = enable start
           // bool gVis_Estop = false;//Variable false = disable stop
            bool successStart = false;
           // bool successStop = false;

            writeModbus start = new writeModbus(gVis_Run, addressArray[13], frmLogIn.busTcpClient);//Create instance of class
            successStart = start.writeAction();//Call class method to enable gVis_Run
            //writeModbus disableStop = new writeModbus(gVis_Estop, addressArray[4], frmLogIn.busTcpClient);//Create instance of class //Delta has 2 seperate registers. Handle this in PLC
            //successStop = disableStop.writeAction();//Call class method to disable gVis_Estop

            if (successStart == true)//If write successful
            {
                btnStart.Enabled = false;//Disable Stop
                btnStart.BackColor = Color.Gray;
                btnStop.Enabled = true;//Enable Start
                btnStop.BackColor = Color.Red;
            }
            else
            {
                //MessageBox.Show("COULD NOT WRITE STOP", "Emerson Climate Technologies");
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            stopping = true;
            bool gVis_Run = false;//Variable true = disable start

            bool successStop = false;
            stopping = true;

            writeModbus stop = new writeModbus(gVis_Run, addressArray[13], frmLogIn.busTcpClient);//Create instance of class
            successStop = stop.writeAction();//Class method to complete action

            if (successStop == true)//If write successful
            {
                btnStop.Enabled = false;//Disable Stop
                btnStop.BackColor = Color.Gray;
                btnStart.Enabled = true;//Enable Start
                btnStart.BackColor = Color.Green;
            }
            else
            {
                //MessageBox.Show("COULD NOT WRITE STOP", "Emerson Climate Technologies");
            }
        }

        private void lblSetPoint_Click(object sender, EventArgs e)
        {
            frmSetSpd speed = new frmSetSpd();
            speed.ShowDialog();
        }
        // Red X Close Event
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }
   
        private void btnComms_Click(object sender, EventArgs e)
        {
            frmConnection settings = new frmConnection();
            settings.ShowDialog();
        }

        // private void btnVIGV_Alarm_CheckedChanged(object sender, EventArgs e)
        private void picVIGV1Fault_Click_1(object sender, EventArgs e)
        {

            //frmAlarms alarm = new frmAlarms();
            //alarm.ShowDialog();

            bool clearAlarm1 = true;//True state to set reset bit
            bool successClear = false;
            picVIGV1_Resetting.Visible = true;

            try
            {
                VIGVclearing = true;
                writeModbus clearVIGV1 = new writeModbus(clearAlarm1, addressArray[7], frmLogIn.busTcpClient);//Create instance of class
                successClear = clearVIGV1.writeAction();//Class method to complete action
                clearAlarm1 = false;//Change state to clear reset bit
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }
        }
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            string status = btnHelp.Text;

            if (lblPLCStatus.Text == "Connected" && frmLogIn.validConn && status == "Log In")//Disable Condition for Testing Graph
            {
                frmLogIn frmLogin = new frmLogIn();
                frmLogin.ShowDialog();
            }
            if (lblPLCStatus.Text == "Connected" && frmLogIn.validConn && status == "Log Out")//Disable Condition for Testing Graph
            {
                loggedIn = false;//Log out advanced user
            }
            else if (lblPLCStatus.Text != "Connected" && !frmLogIn.validConn)
            {
                MessageBox.Show("        Connect to PLC to login", "Emerson Climate Technologies");
            }
        }


        private void lblHelp_Click(object sender, EventArgs e)
        {
            try
            {
                //Open pdf at this path.
                System.Diagnostics.Process.Start(@"C:\Emerson Centrifugal\Copeland AeroLift Manual.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show("File or location does not exist.", "Emerson Climate Technologies");
            }
        }

        private void picVIGV2_Fault_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            export driveList = new export();//Instantiate export class
            driveList.buildRecordSheet(driveGridView);//Call class method and pass gridview
            driveGridView.ClearSelection();
        }

        private void chkBxStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBxStart.Checked)//If starting chart
            {
                startTrend = true;
                chkBxStart.Text = "Stop Chart";;
            }
            else
            {
                startTrend = false;
                chkBxStart.Text = "Start Chart";
            }
        }

        private void btnUnitsTemp_Click(object sender, EventArgs e)//Change Temp Units
        {
            if (btnUnitsTemp.Text == "F")
            {
                Properties.Settings.Default.unitsTemp = "C";//Set system property
            }
            if (btnUnitsTemp.Text == "C")
            {
                Properties.Settings.Default.unitsTemp = "F";//Set  system property
            }

            Properties.Settings.Default.Save();//Save system property
            btnUnitsTemp.Text = Properties.Settings.Default.unitsTemp;//Show F/C selection
            updateUnitLabels();//Update other labels
        }

        private void btnUnitsPsi_Click(object sender, EventArgs e)//Change PSI Units
        {
            string unitP = Properties.Settings.Default.unitsPsi;

            if (btnUnitsPsi.Text == "psiA")
            {
                Properties.Settings.Default.unitsPsi = "psiG";//Set system property             
            }
            if (btnUnitsPsi.Text == "psiG")
            {
                Properties.Settings.Default.unitsPsi = "psiA";//Set system property               
            }
            Properties.Settings.Default.Save();//Save system properties
            btnUnitsPsi.Text = Properties.Settings.Default.unitsPsi;//Show PSIA/PSIG selection
            updateUnitLabels();//Update other labels
           // this.ActiveControl = txtSetSpeed;//Remove focus from button
        }
        private void updateUnitLabels()//Method to update unit labels for readings
        {
            var labels = this.Controls.OfType<Label>();
            if (labels != null)
            {
                foreach (var label in labels)
                {
                    if (label.Name.Contains("lblTemp"))
                    {
                        label.Text = Properties.Settings.Default.unitsTemp;
                    }
                    if (label.Name.Contains("lblPsi"))
                    {
                        label.Text = Properties.Settings.Default.unitsPsi;
                    }
                }
            }
        }
        //Error Box Messages
        private void picVFDTemp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("VFD temp is high. Limit 194°F (90°C)", "Emerson Climate Technologies");
        }
        //Hide chart series with check box
        private void chkBxKw_CheckedChanged(object sender, EventArgs e)//KW
        {
            if (!chkBxKw.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[0] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[0] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxVDC_CheckedChanged(object sender, EventArgs e)//DC Volts
        {
            if (!chkBxVDC.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[2] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[2] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxAmps_CheckedChanged(object sender, EventArgs e)//Amps
        {
            if (!chkBxAmps.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[1] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[1] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxTemp_CheckedChanged(object sender, EventArgs e)//Temp
        {
            if (!chkBxTemp.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[3] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[3] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxDP_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxDP.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[4] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[4] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxSP_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxSP.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[5] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[5] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxIP_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxIP.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[6] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[6] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxDT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxDT.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[7] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[7] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxSt_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxSt.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[8] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[8] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxIT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxIT.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[9] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[9] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxMT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxMT.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[10] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[10] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxMcT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxMcT.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[11] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[11] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxET_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxET.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[12] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[12] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxCond_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxCond.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[13] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[13] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxRB1_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxRB1.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[14] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[14] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxRB2_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxRB2.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[15] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[15] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void chkBxTHB_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxTHB.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[16] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[16] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void chkSpeedACT_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkSpeedACT.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[17] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[17] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxSpdSP_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxSpdSP.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[18] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[18] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void chkBxPR_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxPR.Checked)
            {
                LineSeries seriesToHide = chrtDriveData.Series[19] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LineSeries seriesToHide = chrtDriveData.Series[19] as LineSeries;
                seriesToHide.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void driveBoxClick_Click(object sender, EventArgs e)
        {
            checkBoxes(grpDriveChecks);
        }
        private void SysBoxClick_Click(object sender, EventArgs e)
        {
            checkBoxes(grpSysChecks);
        }
        private void compBxClick_Click(object sender, EventArgs e)
        {
            checkBoxes(grpCompCheck);
        }
        private void spdBxClick_Click(object sender, EventArgs e)
        {
            checkBoxes(grpSpeedChecks);
        }
        private void checkBoxes(GroupBox bx)
        {
            try
            {
                foreach (CheckBox c in bx.Controls.OfType<CheckBox>()) //Get all of checkboxes which are in a groupbox.
                    if (c.Checked == true)
                    {
                        c.Checked = false;
                    }
                    else
                    {
                        c.Checked = true;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Emerson Climate Technologies");
            }        
        }

        private void picAlarms_Click(object sender, EventArgs e)
        {
            frmAlarms Alarm = new frmAlarms();//Instance of form
            Alarm.ShowDialog();
        }
        private void chrtDriveData_OnDataClick(ChartPoint point)
        {
            MessageBox.Show("you clicked (" + point.X + "," + point.Y + ")");

            // point.Instance contains the value as object, in case you passed a class, or any other type
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                //Open pdf at this path.
              System.Diagnostics.Process.Start(@"C:\Emerson Centrifugal\Copeland AeroLift Manual.pdf");//Change file path to folder name for deplayment
            }
            catch (Exception ex)
            {            
                MessageBox.Show("File or location does not exist.", "Emerson Climate Technologies");
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (chrtDriveData != null)
                {
                    chrtDriveData.Series.Clear();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString(), "Emerson Climate Technologies");
            }
        }

        private void chkBxGridview_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBxGridview.Checked)//Hide Grid, Show chart
            {              
                chrtDriveData.Visible = true;
                driveGridView.Visible = false;
                chkBxStart.Visible = true;

            }
            else if (chkBxGridview.Checked)//Hide chart, show grid
            {
                chkBxStart.Visible = false;
                driveGridView.Visible = true;
                chrtDriveData.Visible = false;
            }

            if (driveGridView.Visible)
            {
                chkBxStart.Visible = false;
            }
            else
            {
                chkBxStart.Visible = true;
            }
        }

        private void chkBxClrChart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBxClrChart.Checked)
            {
                chkBxRecord.Visible = false;
                driveGridView.Rows.Clear();///Clear Data

                //for (int i = 0; i < chrtDriveData.Series.Count; i++)
                //{
                //    for (int j = 0; j < chrtDriveData.Series[0].Values.Count; j++)
                //    {
                        chrtDriveData.Series.Clear();
                        setUpChart();
                
                  //  }
              //  }
            }
            if (!chkBxClrChart.Checked)
            {
                chkBxRecord.Visible = true;
            }
        }

        private void findVFDCode()//Call class to search file 
        {
            string errorCode = lblVFDFaultCode.Text;

            if (errorCode != "0")
            {
                lblVFDAlarmPic.Visible = true;
                if (errorCode != lastError)
                {
                    try
                    {
                        FileReader getVFDInfo = new FileReader();//Invoke instance of FileReader class
                        errorExplained = getVFDInfo.readErrorNum(errorCode);//Get returned file data
                        lastError = errorCode;//Track code changing
                        for (int i = 0; i < 4; i++)
                        {
                            alarms[i] = errorExplained[i];//Load first 4 elements of alarm array with drive error info
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
            }
            else
            {
                Array.Clear(alarms, 0, 4);//Clear past data
                alarms = errorExplained;//Load first 4 elements of alarm array with drive error info
            }
            
        }

        private void lblPLCStatus_Click(object sender, EventArgs e)
        {
            if (lblPLCStatus.Text == "Disconnected")//Allow reconnection if not connected initially
            {
                frmLogIn reConn = new frmLogIn();
                reConn.Show();
                this.Hide();
            }
        }

        private void picSettings_Click(object sender, EventArgs e)
        {
            frmAdvanced frmAdvanced = new frmAdvanced();
            frmAdvanced.ShowDialog();
        }
    }    
}

