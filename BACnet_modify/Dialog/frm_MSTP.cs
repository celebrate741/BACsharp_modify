using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BACnet_modify.Dialog
{
    public partial class frm_MSTP : Form
    {
        public UInt16 SourceLength
        {
            get { return (UInt16)num_sourceLength.Value; }
            set { num_sourceLength.Value = value; }
        }
        public UInt16 Network
        {
            get { return (UInt16)num_network.Value; }
            set { num_network.Value = value; }
        }
        public UInt16 MACAddr
        {
            get { return (UInt16)num_MACAddr.Value; }
            set { num_MACAddr.Value = value; }
        }

        public frm_MSTP()
        {
            InitializeComponent();
        }
    }
}
