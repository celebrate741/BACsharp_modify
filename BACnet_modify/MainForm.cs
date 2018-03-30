using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using BACsharp.BACnet_Def;
using BACnet_modify.Dialog;

namespace BACnet_modify
{
    public partial class MainForm : Form
    {
        private SimpleReadWrite simpleRW = null;
        private bool isMSTP = false;
        private frm_MSTP frmMSTP = null;
        public MainForm()
        {
            InitializeComponent();
            log_Invoke = new log_delegate(log_Func);
            simpleRW = new SimpleReadWrite(log_Func);
            lb_broadcast_ip.Text = simpleRW.BroadcastEP.Address.ToString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (FileStream fs = new FileStream(".\\log.txt", FileMode.Create))
            using(StreamWriter sw=new StreamWriter(fs))
            {
                sw.Write(richTextBox.Text);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            if (simpleRW != null)
                simpleRW.Dispose();
        }

        private delegate void log_delegate(string txt);
        private log_delegate log_Invoke = null;
        private void log_Func(string txt)
        {
            Console.Write(txt);
            richTextBox.AppendText(txt);
            richTextBox.ScrollToCaret();
        }
        private void Log(string txt)
        {
            richTextBox.BeginInvoke(log_Invoke, new object[] { txt });
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            richTextBox.Clear();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log("MainForm start\n");
            //example set------------------------------
            txtIP.Text = "192.168.88.212";
            simpleRW.UDPPort_local = 0;
            simpleRW.UDPPort_dest = 47808;
            lb_port_local.Text = "0";
            lb_port_dest.Text = "47808";
            foreach(BACnetEnums.BACNET_OBJECT_TYPE type in Enum.GetValues(typeof(BACnetEnums.BACNET_OBJECT_TYPE)))
                cmbObjectType.Items.Add(type);
            foreach (BACnetEnums.BACNET_PROPERTY_ID id in Enum.GetValues(typeof(BACnetEnums.BACNET_PROPERTY_ID)))
                cmbProperty.Items.Add(id);

            try
            {
                cmbObjectType.SelectedIndex = 8;
                cmbProperty.SelectedIndex = 76;
            }
            catch
            {
                cmbObjectType.SelectedIndex = 
                cmbProperty.SelectedIndex = 0;
            }
            numObjectInst.Value = 2811;
            num_low_limit.Value = 0;
            num_high_limit.Value = 9000;
            num_timeout.Value = 2000;
        }

        private void btn_port_Click(object sender, EventArgs e)
        {
            frmInput f = new frmInput();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    int port = int.Parse(f.txt);
                    string tag = (string)(sender as Button).Tag;
                    if (tag == "local")
                    {
                        lb_port_local.Text = f.txt;
                        simpleRW.UDPPort_local = port;
                    }else if(tag == "dest")
                    {
                        lb_port_dest.Text = f.txt;
                        simpleRW.UDPPort_dest = port;
                    }
                }
                catch (Exception ex)
                {
                    Log("Fail : " + ex.ToString());
                }
            }
            f.Dispose();
        }

        private void btn_read_Click(object sender, EventArgs e)
        {
            BACnetEnums.BACNET_OBJECT_TYPE obj_type = 
                (BACnetEnums.BACNET_OBJECT_TYPE)cmbObjectType.SelectedItem;
            BACnetEnums.BACNET_PROPERTY_ID prop_id =
                (BACnetEnums.BACNET_PROPERTY_ID)cmbProperty.SelectedItem;
            uint obj_inst = (uint)numObjectInst.Value;
            Log(string.Format("({0}) {1} {2}\n", (int)obj_type, obj_type.ToString(), obj_inst));

            List<Property> properties = null;
            string addr = txtIP.Text;
            bool ret = false;
            if (!isMSTP)
                ret = simpleRW.SendReadProperty(addr, obj_type, obj_inst, prop_id, out properties);
            else ret = simpleRW.SendReadProperty(addr, obj_type, obj_inst, prop_id, out properties,
                frmMSTP.SourceLength, frmMSTP.Network, frmMSTP.MACAddr);
            /*ret = simpleRW.SendReadProperty_TCP(addr,
                    0, 0, 0, -1, obj_type, obj_inst, prop_id, property);*/
            if (!ret) Log("Read Err(1)\n");
            else if (properties == null) Log("Read Err(2)\n");
            else
            {
                Log("Value:\n");
                foreach (Property property in properties)
                    Log(property + "\n");
            }
        }

        private void btn_write_Click(object sender, EventArgs e)
        {
            Property property = null;
            frmInput f = new frmInput();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    uint value = uint.Parse(f.txt);
                    property = new Property();
                    property.Tag = BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED;
                    property.ValueEnum = value;
                    Log("Value : " + value + "\n");
                }
                catch(Exception ex)
                {
                    Log("Fail : " + ex.ToString() + "\n");
                }
            }
            f.Dispose();
            if (property == null) return;

            BACnetEnums.BACNET_OBJECT_TYPE obj_type =
                (BACnetEnums.BACNET_OBJECT_TYPE)cmbObjectType.SelectedItem;
            BACnetEnums.BACNET_PROPERTY_ID prop_id =
                (BACnetEnums.BACNET_PROPERTY_ID)cmbProperty.SelectedItem;
            uint obj_inst = (uint)numObjectInst.Value;
            Log(string.Format("({0}) {1} {2}\n", (int)obj_type, obj_type.ToString(), obj_inst));

            string addr = txtIP.Text;
            bool ret;
            if (isMSTP) ret = simpleRW.SendWriteProperty(addr, obj_type, obj_inst, prop_id, property, 8);
            else ret = simpleRW.SendWriteProperty(addr, obj_type, obj_inst, prop_id, property, 8,
                frmMSTP.SourceLength, frmMSTP.Network, frmMSTP.MACAddr);
            if (!ret)
            {
                Log("Write Err(1)\n");
            }
            else if (property.Tag != BACnetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED)
            {
                Log("Write Err(2)\n" + property.Tag.ToString());
            }
            else Log("Success\n");
        }

        private void chk_mstp_CheckedChanged(object sender, EventArgs e)
        {
            if (frmMSTP == null) frmMSTP = new frm_MSTP();
            isMSTP = false;
            if (chk_mstp.Checked)
            {
                if (frmMSTP.ShowDialog(this) == DialogResult.OK)
                    isMSTP = true;
                else chk_mstp.Checked = false;
            }
        }

        private void btn_edit_broadcast_Click(object sender, EventArgs e)
        {
            frmInput f = new frmInput();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    lb_broadcast_ip.Text = f.txt;
                    simpleRW.BroadcastEP.Address = System.Net.IPAddress.Parse(f.txt);
                }
                catch (Exception ex)
                {
                    Log("Fail : " + ex.ToString());
                }
            }
            f.Dispose();
        }

        private void btn_device_Click(object sender, EventArgs e)
        {
            int low_limit = (int)num_low_limit.Value;
            int high_limit = (int)num_high_limit.Value;
            int timeout = (int)num_timeout.Value;
            List<Device> devices = simpleRW.GetDevice(timeout, low_limit, high_limit);
            foreach (Device dev in devices)
                Log(dev.Instance + ":" + "[" + dev.ServerEP.ToString() + "](" +
                    "(S:" + dev.SourceLength + ",N:" + dev.Network + ",M:" + dev.MACAddress + ")\n");
            devices = null;
        }

        private void btn_download_Click(object sender, EventArgs e)
        {
            string addr = txtIP.Text;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "BIN|*.bin";
                dialog.FileName = "DDC.bin";
                dialog.InitialDirectory = ".\\";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] file_data;
                    bool ret;
                    if (isMSTP) ret = simpleRW.SendDownloadDDC(addr, out file_data);
                    else ret = simpleRW.SendDownloadDDC(addr, out file_data,
                        frmMSTP.SourceLength, frmMSTP.Network, frmMSTP.MACAddr);
                    if (!ret) Log("Read Err(1)\n");
                    else
                    {
                        File.WriteAllBytes(dialog.FileName, file_data);
                        Log("Read File Success :" + dialog.FileName + " \n");
                    }
                }
            }
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            string addr = txtIP.Text;
            using(OpenFileDialog dialog=new OpenFileDialog())
            {
                dialog.Filter = "BIN File|*.bin";
                dialog.InitialDirectory = Application.StartupPath;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] file_data = File.ReadAllBytes(dialog.FileName);
                    bool ret;
                    if (isMSTP) ret = simpleRW.SendUploadDDC(addr, file_data);
                    else ret = simpleRW.SendUploadDDC(addr, file_data,
                        frmMSTP.SourceLength, frmMSTP.Network, frmMSTP.MACAddr);
                    if (!ret) Log("Read Err(1)\n");
                    else Log("Write File Success \n");
                }
            }
        }
    }
}
