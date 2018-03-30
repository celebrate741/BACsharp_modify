namespace BACnet_modify
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.btn_read = new System.Windows.Forms.Button();
            this.lb_port_local = new System.Windows.Forms.Label();
            this.btn_port_local = new System.Windows.Forms.Button();
            this.cmbObjectType = new System.Windows.Forms.ComboBox();
            this.numObjectInst = new System.Windows.Forms.NumericUpDown();
            this.btn_write = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_port_dest = new System.Windows.Forms.Label();
            this.btn_port_dest = new System.Windows.Forms.Button();
            this.btn_device = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lb_broadcast_ip = new System.Windows.Forms.Label();
            this.btn_broadcast = new System.Windows.Forms.Button();
            this.num_low_limit = new System.Windows.Forms.NumericUpDown();
            this.num_high_limit = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.num_timeout = new System.Windows.Forms.NumericUpDown();
            this.btn_clear = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.num_tag = new System.Windows.Forms.NumericUpDown();
            this.cmbProperty = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_download = new System.Windows.Forms.Button();
            this.btn_upload = new System.Windows.Forms.Button();
            this.chk_mstp = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numObjectInst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_low_limit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_high_limit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_timeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_tag)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "local UDP port";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(59, 97);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(222, 25);
            this.txtIP.TabIndex = 1;
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(328, 10);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(462, 473);
            this.richTextBox.TabIndex = 4;
            this.richTextBox.Text = "";
            // 
            // btn_read
            // 
            this.btn_read.Location = new System.Drawing.Point(17, 242);
            this.btn_read.Name = "btn_read";
            this.btn_read.Size = new System.Drawing.Size(121, 48);
            this.btn_read.TabIndex = 5;
            this.btn_read.Text = "read";
            this.btn_read.UseVisualStyleBackColor = true;
            this.btn_read.Click += new System.EventHandler(this.btn_read_Click);
            // 
            // lb_port_local
            // 
            this.lb_port_local.AutoSize = true;
            this.lb_port_local.Location = new System.Drawing.Point(137, 13);
            this.lb_port_local.Name = "lb_port_local";
            this.lb_port_local.Size = new System.Drawing.Size(42, 15);
            this.lb_port_local.TabIndex = 0;
            this.lb_port_local.Text = "47812";
            // 
            // btn_port_local
            // 
            this.btn_port_local.Location = new System.Drawing.Point(185, 10);
            this.btn_port_local.Name = "btn_port_local";
            this.btn_port_local.Size = new System.Drawing.Size(75, 23);
            this.btn_port_local.TabIndex = 6;
            this.btn_port_local.Tag = "local";
            this.btn_port_local.Text = "edit";
            this.btn_port_local.UseVisualStyleBackColor = true;
            this.btn_port_local.Click += new System.EventHandler(this.btn_port_Click);
            // 
            // cmbObjectType
            // 
            this.cmbObjectType.FormattingEnabled = true;
            this.cmbObjectType.Location = new System.Drawing.Point(15, 153);
            this.cmbObjectType.Name = "cmbObjectType";
            this.cmbObjectType.Size = new System.Drawing.Size(266, 23);
            this.cmbObjectType.TabIndex = 7;
            // 
            // numObjectInst
            // 
            this.numObjectInst.Location = new System.Drawing.Point(48, 211);
            this.numObjectInst.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numObjectInst.Name = "numObjectInst";
            this.numObjectInst.Size = new System.Drawing.Size(120, 25);
            this.numObjectInst.TabIndex = 8;
            // 
            // btn_write
            // 
            this.btn_write.Location = new System.Drawing.Point(162, 242);
            this.btn_write.Name = "btn_write";
            this.btn_write.Size = new System.Drawing.Size(121, 48);
            this.btn_write.TabIndex = 5;
            this.btn_write.Text = "write";
            this.btn_write.UseVisualStyleBackColor = true;
            this.btn_write.Click += new System.EventHandler(this.btn_write_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "dest  UDP port";
            // 
            // lb_port_dest
            // 
            this.lb_port_dest.AutoSize = true;
            this.lb_port_dest.Location = new System.Drawing.Point(137, 43);
            this.lb_port_dest.Name = "lb_port_dest";
            this.lb_port_dest.Size = new System.Drawing.Size(42, 15);
            this.lb_port_dest.TabIndex = 0;
            this.lb_port_dest.Text = "47812";
            // 
            // btn_port_dest
            // 
            this.btn_port_dest.Location = new System.Drawing.Point(185, 39);
            this.btn_port_dest.Name = "btn_port_dest";
            this.btn_port_dest.Size = new System.Drawing.Size(75, 23);
            this.btn_port_dest.TabIndex = 6;
            this.btn_port_dest.Tag = "dest";
            this.btn_port_dest.Text = "edit";
            this.btn_port_dest.UseVisualStyleBackColor = true;
            this.btn_port_dest.Click += new System.EventHandler(this.btn_port_Click);
            // 
            // btn_device
            // 
            this.btn_device.Location = new System.Drawing.Point(108, 512);
            this.btn_device.Name = "btn_device";
            this.btn_device.Size = new System.Drawing.Size(121, 48);
            this.btn_device.TabIndex = 5;
            this.btn_device.Text = "get device";
            this.btn_device.UseVisualStyleBackColor = true;
            this.btn_device.Click += new System.EventHandler(this.btn_device_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 387);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Broadcast IP";
            // 
            // lb_broadcast_ip
            // 
            this.lb_broadcast_ip.AutoSize = true;
            this.lb_broadcast_ip.Location = new System.Drawing.Point(106, 387);
            this.lb_broadcast_ip.Name = "lb_broadcast_ip";
            this.lb_broadcast_ip.Size = new System.Drawing.Size(103, 15);
            this.lb_broadcast_ip.TabIndex = 0;
            this.lb_broadcast_ip.Text = "255.255.255.255";
            // 
            // btn_broadcast
            // 
            this.btn_broadcast.Location = new System.Drawing.Point(215, 383);
            this.btn_broadcast.Name = "btn_broadcast";
            this.btn_broadcast.Size = new System.Drawing.Size(75, 23);
            this.btn_broadcast.TabIndex = 6;
            this.btn_broadcast.Tag = "dest";
            this.btn_broadcast.Text = "edit";
            this.btn_broadcast.UseVisualStyleBackColor = true;
            this.btn_broadcast.Click += new System.EventHandler(this.btn_edit_broadcast_Click);
            // 
            // num_low_limit
            // 
            this.num_low_limit.Location = new System.Drawing.Point(109, 412);
            this.num_low_limit.Maximum = new decimal(new int[] {
            4194303,
            0,
            0,
            0});
            this.num_low_limit.Name = "num_low_limit";
            this.num_low_limit.Size = new System.Drawing.Size(120, 25);
            this.num_low_limit.TabIndex = 8;
            // 
            // num_high_limit
            // 
            this.num_high_limit.Location = new System.Drawing.Point(109, 443);
            this.num_high_limit.Maximum = new decimal(new int[] {
            4194303,
            0,
            0,
            0});
            this.num_high_limit.Name = "num_high_limit";
            this.num_high_limit.Size = new System.Drawing.Size(120, 25);
            this.num_high_limit.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 414);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "low limit";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 445);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "high limit";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 476);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "timeout";
            // 
            // num_timeout
            // 
            this.num_timeout.Location = new System.Drawing.Point(109, 474);
            this.num_timeout.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_timeout.Name = "num_timeout";
            this.num_timeout.Size = new System.Drawing.Size(120, 25);
            this.num_timeout.TabIndex = 8;
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(328, 489);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(462, 32);
            this.btn_clear.TabIndex = 5;
            this.btn_clear.Text = "clear";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 213);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "Inst";
            // 
            // num_tag
            // 
            this.num_tag.Location = new System.Drawing.Point(256, 296);
            this.num_tag.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_tag.Name = "num_tag";
            this.num_tag.Size = new System.Drawing.Size(66, 25);
            this.num_tag.TabIndex = 10;
            this.num_tag.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // cmbProperty
            // 
            this.cmbProperty.FormattingEnabled = true;
            this.cmbProperty.Location = new System.Drawing.Point(15, 182);
            this.cmbProperty.Name = "cmbProperty";
            this.cmbProperty.Size = new System.Drawing.Size(266, 23);
            this.cmbProperty.TabIndex = 7;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 128);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(147, 19);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "TCP(not implement)";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btn_download
            // 
            this.btn_download.Location = new System.Drawing.Point(17, 296);
            this.btn_download.Name = "btn_download";
            this.btn_download.Size = new System.Drawing.Size(75, 23);
            this.btn_download.TabIndex = 12;
            this.btn_download.Text = "download";
            this.btn_download.UseVisualStyleBackColor = true;
            this.btn_download.Click += new System.EventHandler(this.btn_download_Click);
            // 
            // btn_upload
            // 
            this.btn_upload.Location = new System.Drawing.Point(114, 296);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(75, 23);
            this.btn_upload.TabIndex = 13;
            this.btn_upload.Text = "upload";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // chk_mstp
            // 
            this.chk_mstp.AutoSize = true;
            this.chk_mstp.Location = new System.Drawing.Point(185, 212);
            this.chk_mstp.Name = "chk_mstp";
            this.chk_mstp.Size = new System.Drawing.Size(67, 19);
            this.chk_mstp.TabIndex = 14;
            this.chk_mstp.Text = "MSTP";
            this.chk_mstp.UseVisualStyleBackColor = true;
            this.chk_mstp.CheckedChanged += new System.EventHandler(this.chk_mstp_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 579);
            this.Controls.Add(this.chk_mstp);
            this.Controls.Add(this.btn_upload);
            this.Controls.Add(this.btn_download);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.num_tag);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.num_timeout);
            this.Controls.Add(this.num_high_limit);
            this.Controls.Add(this.num_low_limit);
            this.Controls.Add(this.numObjectInst);
            this.Controls.Add(this.cmbProperty);
            this.Controls.Add(this.cmbObjectType);
            this.Controls.Add(this.btn_broadcast);
            this.Controls.Add(this.btn_port_dest);
            this.Controls.Add(this.btn_port_local);
            this.Controls.Add(this.btn_write);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_device);
            this.Controls.Add(this.btn_read);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.lb_broadcast_ip);
            this.Controls.Add(this.lb_port_dest);
            this.Controls.Add(this.lb_port_local);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numObjectInst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_low_limit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_high_limit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_timeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_tag)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Button btn_read;
        private System.Windows.Forms.Label lb_port_local;
        private System.Windows.Forms.Button btn_port_local;
        private System.Windows.Forms.ComboBox cmbObjectType;
        private System.Windows.Forms.NumericUpDown numObjectInst;
        private System.Windows.Forms.Button btn_write;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_port_dest;
        private System.Windows.Forms.Button btn_port_dest;
        private System.Windows.Forms.Button btn_device;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_broadcast_ip;
        private System.Windows.Forms.Button btn_broadcast;
        private System.Windows.Forms.NumericUpDown num_low_limit;
        private System.Windows.Forms.NumericUpDown num_high_limit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown num_timeout;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown num_tag;
        private System.Windows.Forms.ComboBox cmbProperty;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btn_download;
        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.CheckBox chk_mstp;
    }
}

