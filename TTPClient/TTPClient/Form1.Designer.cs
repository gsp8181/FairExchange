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
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regTimer = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.notifyIconMenu.SuspendLayout();
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
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyIconMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "TTP Client";
            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked_1);
            // 
            // notifyIconMenu
            // 
            this.notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.quitToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.notifyIconMenu.Name = "contextMenuStrip1";
            this.notifyIconMenu.Size = new System.Drawing.Size(117, 70);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.startToolStripMenuItem.Text = "&Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // regTimer
            // 
            this.regTimer.Interval = 600000;
            this.regTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(187, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "GETPEM";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PortOpenButton);
            this.Controls.Add(this.regWithTrackerButton);
            this.Controls.Add(this.whatsMyIpButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.notifyIconMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button whatsMyIpButton;
        private System.Windows.Forms.Button regWithTrackerButton;
        private System.Windows.Forms.Button PortOpenButton;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Timer regTimer;
        private System.Windows.Forms.Button button1;
    }
}

