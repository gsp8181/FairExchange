using System.ComponentModel;
using System.Windows.Forms;

namespace FEClient.Forms
{
    partial class Logs
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.sentTabPage = new System.Windows.Forms.TabPage();
            this.sentListView = new System.Windows.Forms.ListView();
            this.receivedTabPage = new System.Windows.Forms.TabPage();
            this.receivedListView = new System.Windows.Forms.ListView();
            this.iconImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl.SuspendLayout();
            this.sentTabPage.SuspendLayout();
            this.receivedTabPage.SuspendLayout();
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
            this.sentTabPage.Controls.Add(this.sentListView);
            this.sentTabPage.Location = new System.Drawing.Point(4, 22);
            this.sentTabPage.Name = "sentTabPage";
            this.sentTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.sentTabPage.Size = new System.Drawing.Size(276, 236);
            this.sentTabPage.TabIndex = 1;
            this.sentTabPage.Text = "Sent";
            this.sentTabPage.UseVisualStyleBackColor = true;
            // 
            // sentListView
            // 
            this.sentListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sentListView.Location = new System.Drawing.Point(3, 3);
            this.sentListView.MultiSelect = false;
            this.sentListView.Name = "sentListView";
            this.sentListView.Size = new System.Drawing.Size(270, 230);
            this.sentListView.TabIndex = 0;
            this.sentListView.UseCompatibleStateImageBehavior = false;
            this.sentListView.View = System.Windows.Forms.View.Tile;
            this.sentListView.DoubleClick += new System.EventHandler(this.sentListView_DoubleClick);
            // 
            // receivedTabPage
            // 
            this.receivedTabPage.Controls.Add(this.receivedListView);
            this.receivedTabPage.Location = new System.Drawing.Point(4, 22);
            this.receivedTabPage.Name = "receivedTabPage";
            this.receivedTabPage.Size = new System.Drawing.Size(276, 236);
            this.receivedTabPage.TabIndex = 2;
            this.receivedTabPage.Text = "Received";
            this.receivedTabPage.UseVisualStyleBackColor = true;
            // 
            // receivedListView
            // 
            this.receivedListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.receivedListView.Location = new System.Drawing.Point(0, 0);
            this.receivedListView.MultiSelect = false;
            this.receivedListView.Name = "receivedListView";
            this.receivedListView.Size = new System.Drawing.Size(276, 236);
            this.receivedListView.TabIndex = 1;
            this.receivedListView.UseCompatibleStateImageBehavior = false;
            this.receivedListView.View = System.Windows.Forms.View.Tile;
            this.receivedListView.DoubleClick += new System.EventHandler(this.receivedListView_DoubleClick);
            // 
            // iconImageList
            // 
            this.iconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.iconImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.iconImageList.TransparentColor = System.Drawing.Color.Transparent;
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
            this.Load += new System.EventHandler(this.Logs_Load);
            this.tabControl.ResumeLayout(false);
            this.sentTabPage.ResumeLayout(false);
            this.receivedTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabControl;
        private TabPage sentTabPage;
        private TabPage receivedTabPage;
        private ListView sentListView;
        private ListView receivedListView;
        private ImageList iconImageList;
    }
}