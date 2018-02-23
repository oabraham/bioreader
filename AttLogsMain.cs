
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace AttLogs
{
    public partial class AttLogsMain : Form
    {
        public AttLogsMain()
        {
            InitializeComponent();
            GetSavedIPs();
            LoadConfiguration();
        }

        //Create Standalone SDK class dynamicly.
        public hfemkeeper.CHFEMClass axCHFEM1 = new hfemkeeper.CHFEMClass();

        #region Communication
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (txtIP.Text.Trim() == "" || txtPort.Text.Trim() == "")
            {
                MessageBox.Show("IP and Port cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "DisConnect")
            {
                axCHFEM1.Disconnect();
                bIsConnected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                return;
            }

            bIsConnected = axCHFEM1.Connect_Net(txtIP.Text, Convert.ToInt32(txtPort.Text));
            if (bIsConnected == true)
            {
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCHFEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                SaveIPs();
            }
            else
            {
                axCHFEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        //If your device supports the SerialPort communications, you can refer to this.
        //private void btnRsConnect_Click(object sender, EventArgs e)
        //{
        //    if (cbPort.Text.Trim() == "" || cbBaudRate.Text.Trim() == "" || txtMachineSN.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Port,BaudRate and MachineSN cannot be null", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;
        //    //accept serialport number from string like "COMi"
        //    int iPort;
        //    string sPort = cbPort.Text.Trim();
        //    for (iPort = 1; iPort < 10; iPort++)
        //    {
        //        if (sPort.IndexOf(iPort.ToString()) > -1)
        //        {
        //            break;
        //        }
        //    }

        //    Cursor = Cursors.WaitCursor;
        //    if (btnRsConnect.Text == "Disconnect")
        //    {
        //        axCHFEM1.Disconnect();
        //        bIsConnected = false;
        //        btnRsConnect.Text = "Connect";
        //        btnRsConnect.Refresh();
        //        lblState.Text = "Current State:Disconnected";
        //        Cursor = Cursors.Default;
        //        return;
        //    }

        //    iMachineNumber = Convert.ToInt32(txtMachineSN.Text.Trim());//when you are using the serial port communication,you can distinguish different devices by their serial port number.
        //    bIsConnected = axCHFEM1.Connect_Com(iPort, iMachineNumber, Convert.ToInt32(cbBaudRate.Text.Trim()));

        //    if (bIsConnected == true)
        //    {
        //        btnRsConnect.Text = "Disconnect";
        //        btnRsConnect.Refresh();
        //        lblState.Text = "Current State:Connected";

        //        axCHFEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
        //    }
        //    else
        //    {
        //        axCHFEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }

        //    Cursor = Cursors.Default;
        //}

        //If your device supports the USBCLient, you can refer to this.
        //Not all series devices can support this kind of connection.Please make sure your device supports USBClient.
        //Connect the device via the virtual serial port created by USBClient
        //private void btnUSBConnect_Click(object sender, EventArgs e)
        //{
        //    int idwErrorCode = 0;

        //    Cursor = Cursors.WaitCursor;

        //    if (btnUSBConnect.Text == "Disconnect")
        //    {
        //        axCHFEM1.Disconnect();
        //        bIsConnected = false;
        //        btnUSBConnect.Text = "Connect";
        //        btnUSBConnect.Refresh();
        //        lblState.Text = "Current State:Disconnected";
        //        Cursor = Cursors.Default;
        //        return;
        //    }

        //    SearchforUSBCom usbcom = new SearchforUSBCom();
        //    string sCom = "";
        //    bool bSearch = usbcom.SearchforCom(ref sCom);//modify by Darcy on Nov.26 2009
        //    if (bSearch == false)//modify by Darcy on Nov.26 2009
        //    {
        //        MessageBox.Show("Can not find the virtual serial port that can be used", "Error");
        //        Cursor = Cursors.Default;
        //        return;
        //    }

        //    int iPort;
        //    for (iPort = 1; iPort < 10; iPort++)
        //    {
        //        if (sCom.IndexOf(iPort.ToString()) > -1)
        //        {
        //            break;
        //        }
        //    }

        //    iMachineNumber = Convert.ToInt32(txtMachineSN2.Text.Trim());
        //    if (iMachineNumber == 0 || iMachineNumber > 255)
        //    {
        //        MessageBox.Show("The Machine Number is invalid!", "Error");
        //        Cursor = Cursors.Default;
        //        return;
        //    }

        //    int iBaudRate = 115200;//115200 is one possible baudrate value(its value cannot be 0)
        //    bIsConnected = axCHFEM1.Connect_Com(iPort, iMachineNumber, iBaudRate);

        //    if (bIsConnected == true)
        //    {
        //        btnUSBConnect.Text = "Disconnect";
        //        btnUSBConnect.Refresh();
        //        lblState.Text = "Current State:Connected";
        //        axCHFEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
        //    }
        //    else
        //    {
        //        axCHFEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }

        //    Cursor = Cursors.Default;
        //}

        #endregion

        #region AttLogs

        //Download the attendance records from the device(For both Black&White and TFT screen devices).
        private void btnGetGeneralLogData_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }

            string sdwEnrollNumber = "";
            int idwTMachineNumber = 0;
            int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;
            int iGLCount = 0;
            int iIndex = 0;

            Cursor = Cursors.WaitCursor;
            lvLogs.Items.Clear();
            axCHFEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCHFEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCHFEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                           out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    iGLCount++;
                    lvLogs.Items.Add(iGLCount.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());
                    iIndex++;
                }
            }
            else
            {
                Cursor = Cursors.Default;
                axCHFEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!", "Error");
                }
            }
            axCHFEM1.EnableDevice(iMachineNumber, true);//enable the device
            Cursor = Cursors.Default;
        }

        //Clear all attendance records from terminal (For both Black&White and TFT screen devices).
        private void btnClearGLog_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }

            if (!CheckConfig())
            {
                return;
            }

            int idwErrorCode = 0;
            bool isSuccessful = false;
            DataTable TableToUpload = new DataTable("UploadTable");
           
            String UserGUID = txtUserGuid.Text.Trim();
            String CompID = txtCompanyID.Text.Trim();

            //TableToUpload = CreateTestTable();

            if (lvLogs.Items.Count > 0) //if(TableToUpload.Rows.Count > 0)
            {

                //5F5C3B0E-0435-4D89-B8DE-789DEA1ED4B7
                //OSI

                //GetData
                FromListView(TableToUpload, lvLogs);

                //Send Data to Web Service
                ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls; 
                //localhost.AttendanceService ServiceRequest = new localhost.AttendanceService();
                AttServProd.AttendanceServiceSoapClient  RequestProduction = new AttServProd.AttendanceServiceSoapClient();
                //localhost.ImportInfo  resultado = ServiceRequest.ImportBioPunchs(CompID, UserGUID , TableToUpload );
                //AttServProd.ImportInfo resultado = RequestProduction.ImportBioPunchs("XDM", "5F5C3B0E-0435-4D89-B8DE-789DEA1ED4B7", TableToUpload);
                AttServProd.ImportInfo resultado = RequestProduction.ImportBioPunchs(CompID, UserGUID, TableToUpload);
                //If operation is successful clear logs
                if (isSuccessful) {

                    lvLogs.Items.Clear();
                    axCHFEM1.EnableDevice(iMachineNumber, false);//disable the device
                    if (axCHFEM1.ClearGLog(iMachineNumber))
                    {
                        axCHFEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                        MessageBox.Show("All att Logs have been cleared from the terminal!", "Success");
                    }
                    else
                    {
                        axCHFEM1.GetLastError(ref idwErrorCode);
                        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                    }
                    axCHFEM1.EnableDevice(iMachineNumber, true);//enable the device

                    SaveConfiguration();

                } else
                {
                    //Display a message
                    MessageBox.Show("Operation failed,ErrorCode=", "Error");
                }

            }
        }

        //Get the count of attendance records in from ternimal(For both Black&White and TFT screen devices).
        private void btnGetDeviceStatus_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;
            int iValue = 0;

            axCHFEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCHFEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                MessageBox.Show("The count of the AttLogs in the device is " + iValue.ToString(), "Success");
            }
            else
            {
                axCHFEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCHFEM1.EnableDevice(iMachineNumber, true);//enable the device
        }
        #endregion

        private void label10_Click(object sender, EventArgs e)
        {

        }

        public static void FromListView(DataTable table, ListView lvw)
        {
            table.Clear();
            var columns = lvw.Columns.Count;

            foreach (ColumnHeader column in lvw.Columns)
                table.Columns.Add(column.Text);

            foreach (ListViewItem item in lvw.Items)
            {
                var cells = new object[columns];
                for (var i = 0; i < columns; i++)
                    if (i == 6)
                    {
                        cells[i] = "";
                    }
                    else
                    {
                        cells[i] = item.SubItems[i].Text;
                    }
                table.Rows.Add(cells);
            }
        }

        public bool AcceptAllCertifications(object sender , System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors ) 
        { 
            return true;
        }

        public void GetSavedIPs()
        {
            try
            {
                txtIP.Text = Properties.Settings.Default["IP"].ToString();
                txtPort.Text = Properties.Settings.Default["Port"].ToString();
            }
            catch
            {

            }
         
        }

        public void LoadConfiguration()
        {
            try
            {
                txtCompanyID.Text = Properties.Settings.Default["CompID"].ToString();
                txtUserGuid.Text = Properties.Settings.Default["UserID"].ToString();
                txtSchedule.Text = Properties.Settings.Default["Schedule"].ToString();
            }
            catch
            {

            }
        }

        public void SaveIPs()
        {
            Properties.Settings.Default["IP"] = txtIP.Text;
            Properties.Settings.Default["Port"] = txtPort.Text;
            Properties.Settings.Default.Save(); 
        }

        public void SaveConfiguration()
        {
                Properties.Settings.Default["CompID"] = txtCompanyID.Text;
                Properties.Settings.Default["UserID"] = txtUserGuid.Text;
                Properties.Settings.Default["Schedule"] = txtSchedule.Text;
                Properties.Settings.Default.Save();

        }

        public bool CheckConfig()
        {
            if (txtCompanyID.Text.Trim() == "" || txtUserGuid.Text.Trim() == "")
            {
                MessageBox.Show("CompID or UserId is not set", "Error");
                MissingLabel.Visible = true;
                return false;
            }
            else
            {
                MissingLabel.Visible = false;
                return true;
            }
        }

        public  DataTable CreateTestTable()
        {
            DataTable aDatatable = new DataTable("UploadTable");

            aDatatable.Columns.Add("ID" ,Type.GetType("System.Int64"));
            aDatatable.Columns.Add("EnrollNumber" ,Type.GetType("System.String"));
            aDatatable.Columns.Add("VerifyMode", Type.GetType("System.Int64"));
            aDatatable.Columns.Add("InOutMode", Type.GetType("System.Int64"));
            aDatatable.Columns.Add("PunchTime", Type.GetType("System.DateTime"));
            aDatatable.Columns.Add("WorkCode", Type.GetType("System.String"));
            aDatatable.Columns.Add("Reserv", Type.GetType("System.String"));

            DataColumn[] prmk = new DataColumn[1];
            prmk[0] = aDatatable.Columns["ID"];
            aDatatable.PrimaryKey = prmk;
            aDatatable.Columns["ID"].AutoIncrement = true;
            aDatatable.Columns["ID"].AutoIncrementSeed = 1;
            aDatatable.Columns["ID"].ReadOnly = true;

            //0—Check-In (default value) 1—Check-Out 2—Break-Out
            //3—Break - In 4—OT - In 5—OT - Out 
            //Check Ins 0 , 2, 4
            //Check Outs 1, 3, 5
            DataRow myNewRow = aDatatable.NewRow();
            myNewRow["EnrollNumber"] = "0504";
            myNewRow["VerifyMode"] = 1; //1 es fingerprint , 0 es pin ,  2 es card
            myNewRow["InOutMode"] = 0;
            myNewRow["PunchTime"] = DateTime.Now;
            myNewRow["WorkCode"] = "99"; // 1 es default department
            myNewRow["Reserv"] = "";
            aDatatable.Rows.Add(myNewRow);

            myNewRow = aDatatable.NewRow();
            myNewRow["EnrollNumber"] = "050412";
            myNewRow["VerifyMode"] = 1;
            myNewRow["InOutMode"] = 1;
            myNewRow["PunchTime"] = DateTime.Now;
            myNewRow["WorkCode"] = "00";
            myNewRow["Reserv"] = "";
            aDatatable.Rows.Add(myNewRow);

            return aDatatable;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }


} 