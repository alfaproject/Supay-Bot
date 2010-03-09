namespace Supay.Bot
{
  sealed partial class Main
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ToolStripMenuItem mnuFile;
      System.Windows.Forms.ToolStripSeparator mnuSeperator;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
      System.Windows.Forms.SplitContainer splitContainer;
      this.menu = new System.Windows.Forms.MenuStrip();
      this.btnConnect = new System.Windows.Forms.ToolStripMenuItem();
      this.btnReconnect = new System.Windows.Forms.ToolStripMenuItem();
      this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
      this.statusBar = new System.Windows.Forms.StatusStrip();
      this.lblUtcTimer = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblUpdateTimer = new System.Windows.Forms.ToolStripStatusLabel();
      this.treeView = new System.Windows.Forms.TreeView();
      this.textBox = new System.Windows.Forms.RichTextBox();
      mnuFile = new System.Windows.Forms.ToolStripMenuItem();
      mnuSeperator = new System.Windows.Forms.ToolStripSeparator();
      splitContainer = new System.Windows.Forms.SplitContainer();
      this.menu.SuspendLayout();
      this.statusBar.SuspendLayout();
      splitContainer.Panel1.SuspendLayout();
      splitContainer.Panel2.SuspendLayout();
      splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // menu
      // 
      this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuFile});
      this.menu.Location = new System.Drawing.Point(0, 0);
      this.menu.Name = "menu";
      this.menu.Size = new System.Drawing.Size(423, 24);
      this.menu.TabIndex = 0;
      this.menu.Text = "menuStrip1";
      // 
      // mnuFile
      // 
      mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConnect,
            this.btnReconnect,
            mnuSeperator,
            this.btnExit});
      mnuFile.Name = "mnuFile";
      mnuFile.Size = new System.Drawing.Size(37, 20);
      mnuFile.Text = "&File";
      // 
      // btnConnect
      // 
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(152, 22);
      this.btnConnect.Text = "&Connect";
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // btnReconnect
      // 
      this.btnReconnect.Name = "btnReconnect";
      this.btnReconnect.Size = new System.Drawing.Size(152, 22);
      this.btnReconnect.Text = "&Reconnect";
      this.btnReconnect.Click += new System.EventHandler(this.btnReconnect_Click);
      // 
      // mnuSeperator
      // 
      mnuSeperator.Name = "mnuSeperator";
      mnuSeperator.Size = new System.Drawing.Size(149, 6);
      // 
      // btnExit
      // 
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(152, 22);
      this.btnExit.Text = "E&xit";
      this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
      // 
      // notifyIcon
      // 
      this.notifyIcon.Icon = ((System.Drawing.Icon) (resources.GetObject("notifyIcon.Icon")));
      this.notifyIcon.Text = "Supay Bot";
      this.notifyIcon.Visible = true;
      this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
      // 
      // statusBar
      // 
      this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUtcTimer,
            this.lblUpdateTimer});
      this.statusBar.Location = new System.Drawing.Point(0, 316);
      this.statusBar.Name = "statusBar";
      this.statusBar.Size = new System.Drawing.Size(423, 24);
      this.statusBar.TabIndex = 1;
      this.statusBar.Text = "statusStrip1";
      // 
      // lblUtcTimer
      // 
      this.lblUtcTimer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.lblUtcTimer.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
      this.lblUtcTimer.Name = "lblUtcTimer";
      this.lblUtcTimer.Size = new System.Drawing.Size(108, 19);
      this.lblUtcTimer.Text = "UTC: <hh:mm:ss>";
      // 
      // lblUpdateTimer
      // 
      this.lblUpdateTimer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides) ((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.lblUpdateTimer.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
      this.lblUpdateTimer.Name = "lblUpdateTimer";
      this.lblUpdateTimer.Size = new System.Drawing.Size(149, 19);
      this.lblUpdateTimer.Text = "Next update: <hh:mm:ss>";
      // 
      // splitContainer
      // 
      splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer.Location = new System.Drawing.Point(0, 24);
      splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      splitContainer.Panel1.Controls.Add(this.treeView);
      // 
      // splitContainer.Panel2
      // 
      splitContainer.Panel2.Controls.Add(this.textBox);
      splitContainer.Size = new System.Drawing.Size(423, 292);
      splitContainer.SplitterDistance = 121;
      splitContainer.TabIndex = 2;
      // 
      // treeView
      // 
      this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView.Location = new System.Drawing.Point(0, 0);
      this.treeView.Name = "treeView";
      this.treeView.Size = new System.Drawing.Size(121, 292);
      this.treeView.TabIndex = 1;
      // 
      // textBox
      // 
      this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBox.Location = new System.Drawing.Point(0, 0);
      this.textBox.Name = "textBox";
      this.textBox.Size = new System.Drawing.Size(298, 292);
      this.textBox.TabIndex = 0;
      this.textBox.Text = "";
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(423, 340);
      this.Controls.Add(splitContainer);
      this.Controls.Add(this.statusBar);
      this.Controls.Add(this.menu);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
      this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menu;
      this.Name = "Main";
      this.Text = "Supay Bot";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
      this.Shown += new System.EventHandler(this.Main_Shown);
      this.Resize += new System.EventHandler(this.Main_Resize);
      this.menu.ResumeLayout(false);
      this.menu.PerformLayout();
      this.statusBar.ResumeLayout(false);
      this.statusBar.PerformLayout();
      splitContainer.Panel1.ResumeLayout(false);
      splitContainer.Panel2.ResumeLayout(false);
      splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menu;
    private System.Windows.Forms.ToolStripMenuItem btnConnect;
    private System.Windows.Forms.ToolStripMenuItem btnExit;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.StatusStrip statusBar;
    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.RichTextBox textBox;
    private System.Windows.Forms.ToolStripStatusLabel lblUtcTimer;
    private System.Windows.Forms.ToolStripStatusLabel lblUpdateTimer;
    private System.Windows.Forms.ToolStripMenuItem btnReconnect;
  }
}

