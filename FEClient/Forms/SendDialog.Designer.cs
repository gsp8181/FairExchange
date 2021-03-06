﻿using System.ComponentModel;
using System.Windows.Forms;

namespace FEClient.Forms
{
    partial class SendDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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

            _logWriter.Close();
            _log.Close();

            _logWriter.Dispose();
            _log.Dispose();

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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.abortButton = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.timeoutTimer = new System.Windows.Forms.Timer(this.components);
            this.sendKeysBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.generateKeysBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(239, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(258, 13);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(14, 23);
            this.abortButton.TabIndex = 1;
            this.abortButton.Text = "X";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(13, 60);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(84, 13);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.Text = "Generating keys";
            // 
            // timeoutTimer
            // 
            this.timeoutTimer.Interval = 60000;
            this.timeoutTimer.Tick += new System.EventHandler(this.timeoutTimer_Tick);
            // 
            // sendKeysBackgroundWorker
            // 
            this.sendKeysBackgroundWorker.WorkerReportsProgress = true;
            this.sendKeysBackgroundWorker.WorkerSupportsCancellation = true;
            this.sendKeysBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.sendKeysBackgroundWorker_DoWork);
            this.sendKeysBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.sendKeysBackgroundWorker_ProgressChanged);
            this.sendKeysBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.sendKeysBackgroundWorker_RunWorkerCompleted);
            // 
            // generateKeysBackgroundWorker
            // 
            this.generateKeysBackgroundWorker.WorkerSupportsCancellation = true;
            this.generateKeysBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.generateKeysBackgroundWorker_DoWork);
            this.generateKeysBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.generateKeysBackgroundWorker_RunWorkerCompleted);
            // 
            // SendDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 85);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.abortButton);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::FEClient.Properties.Resources.Icojam_Blue_Bits_Document_arrow_down;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SendDialog";
            this.Text = "Sending File";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SendDialog_FormClosed);
            this.Load += new System.EventHandler(this.SendDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgressBar progressBar;
        private Button abortButton;
        private Label progressLabel;
        private Timer timeoutTimer;
        private BackgroundWorker sendKeysBackgroundWorker;
        private BackgroundWorker generateKeysBackgroundWorker;
    }
}