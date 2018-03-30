namespace BACnet_modify.Dialog
{
    partial class frm_MSTP
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.num_sourceLength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.num_network = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.num_MACAddr = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_sourceLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_network)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MACAddr)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_Cancel.Location = new System.Drawing.Point(30, 139);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(94, 49);
            this.btn_Cancel.TabIndex = 0;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_OK.Location = new System.Drawing.Point(176, 139);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(94, 49);
            this.btn_OK.TabIndex = 0;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // num_sourceLength
            // 
            this.num_sourceLength.Location = new System.Drawing.Point(164, 20);
            this.num_sourceLength.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_sourceLength.Name = "num_sourceLength";
            this.num_sourceLength.Size = new System.Drawing.Size(120, 25);
            this.num_sourceLength.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source Length";
            // 
            // num_network
            // 
            this.num_network.Location = new System.Drawing.Point(164, 51);
            this.num_network.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_network.Name = "num_network";
            this.num_network.Size = new System.Drawing.Size(120, 25);
            this.num_network.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Network";
            // 
            // num_MACAddr
            // 
            this.num_MACAddr.Location = new System.Drawing.Point(164, 82);
            this.num_MACAddr.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_MACAddr.Name = "num_MACAddr";
            this.num_MACAddr.Size = new System.Drawing.Size(120, 25);
            this.num_MACAddr.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "MAC Address";
            // 
            // frm_MSTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 216);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.num_MACAddr);
            this.Controls.Add(this.num_network);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.num_sourceLength);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Cancel);
            this.Name = "frm_MSTP";
            this.ShowIcon = false;
            this.Text = "MSTP";
            ((System.ComponentModel.ISupportInitialize)(this.num_sourceLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_network)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_MACAddr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.NumericUpDown num_sourceLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown num_network;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_MACAddr;
        private System.Windows.Forms.Label label3;
    }
}