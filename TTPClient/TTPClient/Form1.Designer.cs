﻿namespace TTPClient
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.whatsMyIpButton = new System.Windows.Forms.Button();
            this.regWithTrackerButton = new System.Windows.Forms.Button();
            this.PortOpenButton = new System.Windows.Forms.Button();
            this.getRemoteStatus = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.emailBox = new System.Windows.Forms.TextBox();
            this.GenDSAKeysButton = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // whatsMyIpButton
            // 
            this.whatsMyIpButton.Location = new System.Drawing.Point(12, 85);
            this.whatsMyIpButton.Name = "whatsMyIpButton";
            this.whatsMyIpButton.Size = new System.Drawing.Size(106, 23);
            this.whatsMyIpButton.TabIndex = 0;
            this.whatsMyIpButton.Text = "Whats My IP";
            this.whatsMyIpButton.UseVisualStyleBackColor = true;
            this.whatsMyIpButton.Click += new System.EventHandler(this.whatMyIp_Click);
            // 
            // regWithTrackerButton
            // 
            this.regWithTrackerButton.Location = new System.Drawing.Point(12, 114);
            this.regWithTrackerButton.Name = "regWithTrackerButton";
            this.regWithTrackerButton.Size = new System.Drawing.Size(106, 23);
            this.regWithTrackerButton.TabIndex = 1;
            this.regWithTrackerButton.Text = "Reg With Tracker";
            this.regWithTrackerButton.UseVisualStyleBackColor = true;
            this.regWithTrackerButton.Click += new System.EventHandler(this.regWithTrackerButton_Click);
            // 
            // PortOpenButton
            // 
            this.PortOpenButton.Location = new System.Drawing.Point(12, 143);
            this.PortOpenButton.Name = "PortOpenButton";
            this.PortOpenButton.Size = new System.Drawing.Size(106, 23);
            this.PortOpenButton.TabIndex = 2;
            this.PortOpenButton.Text = "6555 Open?";
            this.PortOpenButton.UseVisualStyleBackColor = true;
            this.PortOpenButton.Click += new System.EventHandler(this.PortOpenButton_Click);
            // 
            // getRemoteStatus
            // 
            this.getRemoteStatus.Location = new System.Drawing.Point(12, 173);
            this.getRemoteStatus.Name = "getRemoteStatus";
            this.getRemoteStatus.Size = new System.Drawing.Size(106, 23);
            this.getRemoteStatus.TabIndex = 3;
            this.getRemoteStatus.Text = "Get IP";
            this.getRemoteStatus.UseVisualStyleBackColor = true;
            this.getRemoteStatus.Click += new System.EventHandler(this.getRemoteStatus_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(86, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(186, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "http://osttp.gsp8181.co.uk";
            this.textBox1.Validated += new System.EventHandler(this.textBox1_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "TTP Server: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Email:";
            // 
            // emailBox
            // 
            this.emailBox.Location = new System.Drawing.Point(86, 38);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(186, 20);
            this.emailBox.TabIndex = 7;
            this.emailBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.emailBox_KeyPress);
            this.emailBox.Validated += new System.EventHandler(this.emailBox_Validated);
            // 
            // GenDSAKeysButton
            // 
            this.GenDSAKeysButton.Location = new System.Drawing.Point(12, 202);
            this.GenDSAKeysButton.Name = "GenDSAKeysButton";
            this.GenDSAKeysButton.Size = new System.Drawing.Size(106, 23);
            this.GenDSAKeysButton.TabIndex = 8;
            this.GenDSAKeysButton.Text = "Gen DSA Keys";
            this.GenDSAKeysButton.UseVisualStyleBackColor = true;
            this.GenDSAKeysButton.Click += new System.EventHandler(this.GenDSAKeysButton_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(197, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.GenDSAKeysButton);
            this.Controls.Add(this.emailBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.getRemoteStatus);
            this.Controls.Add(this.PortOpenButton);
            this.Controls.Add(this.regWithTrackerButton);
            this.Controls.Add(this.whatsMyIpButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button whatsMyIpButton;
        private System.Windows.Forms.Button regWithTrackerButton;
        private System.Windows.Forms.Button PortOpenButton;
        private System.Windows.Forms.Button getRemoteStatus;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox emailBox;
        private System.Windows.Forms.Button GenDSAKeysButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button2;
    }
}

