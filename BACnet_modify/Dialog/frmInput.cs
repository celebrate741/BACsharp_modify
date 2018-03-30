using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BACnet_modify.Dialog
{
    public partial class frmInput : Form
    {
        public string txt;
        public frmInput()
        {
            InitializeComponent();
        }

        private void frmInput_Load(object sender, EventArgs e)
        {
            
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            txt = textBox1.Text;
        }

        private void frmInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btn_OK.PerformClick();
        }
    }
}
