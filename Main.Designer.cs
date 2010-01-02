namespace Supay.Bot
{
  partial class Main
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
      this.mnu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.btnConnect = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
      this.stbMain = new System.Windows.Forms.StatusStrip();
      this.lblUtcTimer = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblUpdateTimer = new System.Windows.Forms.ToolStripStatusLabel();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.tv = new System.Windows.Forms.TreeView();
      this.txt = new System.Windows.Forms.RichTextBox();
      this.mnu.SuspendLayout();
      this.stbMain.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.SuspendLayout();
      // 
      // mnu
      // 
      this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.mnu.Location = new System.Drawing.Point(0, 0);
      this.mnu.Name = "mnu";
      this.mnu.Size = new System.Drawing.Size(416, 24);
      this.mnu.TabIndex = 0;
      this.mnu.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConnect,
            this.toolStripSeparator1,
            this.btnExit});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // btnConnect
      // 
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(119, 22);
      this.btnConnect.Text = "&Connect";
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(116, 6);
      // 
      // btnExit
      // 
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(119, 22);
      this.btnExit.Text = "E&xit";
      this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
      // 
      // notifyIcon1
      // 
      this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
      this.notifyIcon1.Text = "Supay Bot";
      this.notifyIcon1.Visible = true;
      this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
      // 
      // stbMain
      // 
      this.stbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUtcTimer,
            this.lblUpdateTimer});
      this.stbMain.Location = new System.Drawing.Point(0, 314);
      this.stbMain.Name = "stbMain";
      this.stbMain.Size = new System.Drawing.Size(416, 24);
      this.stbMain.TabIndex = 1;
      this.stbMain.Text = "statusStrip1";
      // 
      // lblUtcTimer
      // 
      this.lblUtcTimer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.lblUtcTimer.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
      this.lblUtcTimer.Name = "lblUtcTimer";
      this.lblUtcTimer.Size = new System.Drawing.Size(108, 19);
      this.lblUtcTimer.Text = "UTC: <hh:mm:ss>";
      // 
      // lblUpdateTimer
      // 
      this.lblUpdateTimer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                  | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
      this.lblUpdateTimer.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
      this.lblUpdateTimer.Name = "lblUpdateTimer";
      this.lblUpdateTimer.Size = new System.Drawing.Size(149, 19);
      this.lblUpdateTimer.Text = "Next update: <hh:mm:ss>";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 24);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.tv);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.txt);
      this.splitContainer1.Size = new System.Drawing.Size(416, 290);
      this.splitContainer1.SplitterDistance = 138;
      this.splitContainer1.TabIndex = 2;
      // 
      // tv
      // 
      this.tv.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tv.Location = new System.Drawing.Point(0, 0);
      this.tv.Name = "tv";
      this.tv.Size = new System.Drawing.Size(138, 290);
      this.tv.TabIndex = 1;
      // 
      // txt
      // 
      this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txt.Location = new System.Drawing.Point(0, 0);
      this.txt.Name = "txt";
      this.txt.Size = new System.Drawing.Size(274, 290);
      this.txt.TabIndex = 0;
      this.txt.Text = "";
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(416, 338);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.stbMain);
      this.Controls.Add(this.mnu);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.mnu;
      this.Name = "Main";
      this.Text = "Supay Bot (c) _aLfa_ and P_Gertrude 2006 - 2009";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
      this.Shown += new System.EventHandler(this.Main_Shown);
      this.Resize += new System.EventHandler(this.Main_Resize);
      this.mnu.ResumeLayout(false);
      this.mnu.PerformLayout();
      this.stbMain.ResumeLayout(false);
      this.stbMain.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip mnu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem btnConnect;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem btnExit;
    private System.Windows.Forms.NotifyIcon notifyIcon1;
    private System.Windows.Forms.StatusStrip stbMain;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView tv;
    private System.Windows.Forms.RichTextBox txt;
    private System.Windows.Forms.ToolStripStatusLabel lblUtcTimer;
    private System.Windows.Forms.ToolStripStatusLabel lblUpdateTimer;
  }
}

