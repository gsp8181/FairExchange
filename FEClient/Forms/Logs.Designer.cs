namespace FEClient.Forms
{
    partial class Logs
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.sentTabPage = new System.Windows.Forms.TabPage();
            this.receivedTabPage = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.sentTabPage);
            this.tabControl.Controls.Add(this.receivedTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(284, 262);
            this.tabControl.TabIndex = 0;
            // 
            // sentTabPage
            // 
            this.sentTabPage.Location = new System.Drawing.Point(4, 22);
            this.sentTabPage.Name = "sentTabPage";
            this.sentTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.sentTabPage.Size = new System.Drawing.Size(276, 236);
            this.sentTabPage.TabIndex = 1;
            this.sentTabPage.Text = "Sent";
            this.sentTabPage.UseVisualStyleBackColor = true;
            // 
            // receivedTabPage
            // 
            this.receivedTabPage.Location = new System.Drawing.Point(4, 22);
            this.receivedTabPage.Name = "receivedTabPage";
            this.receivedTabPage.Size = new System.Drawing.Size(276, 236);
            this.receivedTabPage.TabIndex = 2;
            this.receivedTabPage.Text = "Received";
            this.receivedTabPage.UseVisualStyleBackColor = true;
            // 
            // Logs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tabControl);
            this.Icon = global::FEClient.Properties.Resources.Icojam_Blue_Bits_Document_arrow_down;
            this.Name = "Logs";
            this.Text = "Logs";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage sentTabPage;
        private System.Windows.Forms.TabPage receivedTabPage;
    }
}