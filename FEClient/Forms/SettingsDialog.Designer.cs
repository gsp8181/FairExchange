﻿using System.ComponentModel;
using System.Windows.Forms;

namespace FEClient.Forms
{
    partial class SettingsDialog
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
            this.emailBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.regenKeysButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.publicKeyButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // emailBox
            // 
            this.emailBox.Location = new System.Drawing.Point(86, 12);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(186, 20);
            this.emailBox.TabIndex = 11;
            this.emailBox.Validated += new System.EventHandler(this.emailBox_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Email:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(197, 38);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 12;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // regenKeysButton
            // 
            this.regenKeysButton.Location = new System.Drawing.Point(13, 38);
            this.regenKeysButton.Name = "regenKeysButton";
            this.regenKeysButton.Size = new System.Drawing.Size(75, 23);
            this.regenKeysButton.TabIndex = 13;
            this.regenKeysButton.Text = "Regen Keys";
            this.regenKeysButton.UseVisualStyleBackColor = true;
            this.regenKeysButton.Click += new System.EventHandler(this.GenDSAKeysButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // publicKeyButton
            // 
            this.publicKeyButton.Location = new System.Drawing.Point(105, 38);
            this.publicKeyButton.Name = "publicKeyButton";
            this.publicKeyButton.Size = new System.Drawing.Size(75, 23);
            this.publicKeyButton.TabIndex = 14;
            this.publicKeyButton.Text = "Get PubKey";
            this.publicKeyButton.UseVisualStyleBackColor = true;
            this.publicKeyButton.Click += new System.EventHandler(this.publicKeyButton_Click);
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 72);
            this.Controls.Add(this.publicKeyButton);
            this.Controls.Add(this.regenKeysButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.emailBox);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::FEClient.Properties.Resources.Icojam_Blue_Bits_Document_arrow_down;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox emailBox;
        private Label label2;
        private Button okButton;
        private Button regenKeysButton;
        private ErrorProvider errorProvider;
        private Button publicKeyButton;
    }
}