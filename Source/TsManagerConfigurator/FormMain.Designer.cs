namespace TsManagerConfigurator
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ñåðâèñToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.êîíôèãóðàöèÿToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiNewACMS = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNewTurnstile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReloadEvents = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiViewFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.ñïðàâêàToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHome = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbStart = new System.Windows.Forms.ToolStripButton();
            this.tsbStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslServiceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUpdateProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxNewACMS = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxNewTurnstile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.lvLog = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ñåðâèñToolStripMenuItem,
            this.êîíôèãóðàöèÿToolStripMenuItem,
            this.toolStripMenuItem7,
            this.ñïðàâêàToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(725, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ñåðâèñToolStripMenuItem
            // 
            this.ñåðâèñToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiStart,
            this.tsmiStop,
            this.toolStripMenuItem1,
            this.tsmiRestart,
            this.toolStripSeparator3,
            this.tsmiExit});
            this.ñåðâèñToolStripMenuItem.Name = "ñåðâèñToolStripMenuItem";
            this.ñåðâèñToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.ñåðâèñToolStripMenuItem.Text = "Ñåðâèñ";
            // 
            // tsmiStart
            // 
            this.tsmiStart.Image = ((System.Drawing.Image)(resources.GetObject("tsmiStart.Image")));
            this.tsmiStart.Name = "tsmiStart";
            this.tsmiStart.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.tsmiStart.Size = new System.Drawing.Size(138, 22);
            this.tsmiStart.Text = "Ïóñê";
            // 
            // tsmiStop
            // 
            this.tsmiStop.Image = ((System.Drawing.Image)(resources.GetObject("tsmiStop.Image")));
            this.tsmiStop.Name = "tsmiStop";
            this.tsmiStop.Size = new System.Drawing.Size(138, 22);
            this.tsmiStop.Text = "Ñòîï";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 6);
            // 
            // tsmiRestart
            // 
            this.tsmiRestart.Name = "tsmiRestart";
            this.tsmiRestart.Size = new System.Drawing.Size(138, 22);
            this.tsmiRestart.Text = "Ïåðåçàïóñê";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(135, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(138, 22);
            this.tsmiExit.Text = "Âûõîä";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // êîíôèãóðàöèÿToolStripMenuItem
            // 
            this.êîíôèãóðàöèÿToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSave,
            this.toolStripMenuItem3,
            this.tsmiNewACMS,
            this.tsmiNewTurnstile,
            this.toolStripMenuItem4,
            this.tsmiDelete});
            this.êîíôèãóðàöèÿToolStripMenuItem.Name = "êîíôèãóðàöèÿToolStripMenuItem";
            this.êîíôèãóðàöèÿToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.êîíôèãóðàöèÿToolStripMenuItem.Text = "Êîíôèãóðàöèÿ";
            // 
            // tsmiSave
            // 
            this.tsmiSave.Image = ((System.Drawing.Image)(resources.GetObject("tsmiSave.Image")));
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.tsmiSave.Size = new System.Drawing.Size(159, 22);
            this.tsmiSave.Text = "Ñîõðàíèòü";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(156, 6);
            // 
            // tsmiNewACMS
            // 
            this.tsmiNewACMS.Name = "tsmiNewACMS";
            this.tsmiNewACMS.Size = new System.Drawing.Size(159, 22);
            this.tsmiNewACMS.Text = "Íîâàÿ ÑÊÓÄ";
            this.tsmiNewACMS.Click += new System.EventHandler(this.tsmiNewACMS_Click);
            // 
            // tsmiNewTurnstile
            // 
            this.tsmiNewTurnstile.Name = "tsmiNewTurnstile";
            this.tsmiNewTurnstile.Size = new System.Drawing.Size(159, 22);
            this.tsmiNewTurnstile.Text = "Íîâûé òóðíèêåò";
            this.tsmiNewTurnstile.Click += new System.EventHandler(this.tsmiNewTurnstile_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(156, 6);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(159, 22);
            this.tsmiDelete.Text = "Óäàëèòü";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReloadEvents,
            this.tsmiDetails,
            this.toolStripSeparator4,
            this.tsmiViewFilter});
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(38, 20);
            this.toolStripMenuItem7.Text = "Âèä";
            // 
            // tsmiReloadEvents
            // 
            this.tsmiReloadEvents.Image = ((System.Drawing.Image)(resources.GetObject("tsmiReloadEvents.Image")));
            this.tsmiReloadEvents.Name = "tsmiReloadEvents";
            this.tsmiReloadEvents.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.tsmiReloadEvents.Size = new System.Drawing.Size(145, 22);
            this.tsmiReloadEvents.Text = "Îáíîâèòü";
            // 
            // tsmiDetails
            // 
            this.tsmiDetails.Name = "tsmiDetails";
            this.tsmiDetails.Size = new System.Drawing.Size(145, 22);
            this.tsmiDetails.Text = "Ïîäðîáíî...";
            this.tsmiDetails.Click += new System.EventHandler(this.tsmiDetails_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(142, 6);
            // 
            // tsmiViewFilter
            // 
            this.tsmiViewFilter.Name = "tsmiViewFilter";
            this.tsmiViewFilter.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.tsmiViewFilter.Size = new System.Drawing.Size(145, 22);
            this.tsmiViewFilter.Text = "Ôèëüòð...";
            // 
            // ñïðàâêàToolStripMenuItem
            // 
            this.ñïðàâêàToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiHome,
            this.tsmiUpdates,
            this.toolStripMenuItem5,
            this.tsmiAbout});
            this.ñïðàâêàToolStripMenuItem.Name = "ñïðàâêàToolStripMenuItem";
            this.ñïðàâêàToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.ñïðàâêàToolStripMenuItem.Text = "Ñïðàâêà";
            // 
            // tsmiHome
            // 
            this.tsmiHome.Image = ((System.Drawing.Image)(resources.GetObject("tsmiHome.Image")));
            this.tsmiHome.Name = "tsmiHome";
            this.tsmiHome.Size = new System.Drawing.Size(182, 22);
            this.tsmiHome.Text = "Äîìàøíÿÿ ñòðàíèöà";
            this.tsmiHome.Click += new System.EventHandler(this.tsmiHome_Click);
            // 
            // tsmiUpdates
            // 
            this.tsmiUpdates.Name = "tsmiUpdates";
            this.tsmiUpdates.Size = new System.Drawing.Size(182, 22);
            this.tsmiUpdates.Text = "Îáíîâëåíèÿ";
            this.tsmiUpdates.Click += new System.EventHandler(this.tsmiUpdates_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(179, 6);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Image = ((System.Drawing.Image)(resources.GetObject("tsmiAbout.Image")));
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(182, 22);
            this.tsmiAbout.Text = "Î ïðîãðàììå...";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStart,
            this.tsbStop,
            this.toolStripSeparator1,
            this.tsbSave,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(725, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbStart
            // 
            this.tsbStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStart.Image = ((System.Drawing.Image)(resources.GetObject("tsbStart.Image")));
            this.tsbStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStart.Name = "tsbStart";
            this.tsbStart.Size = new System.Drawing.Size(23, 22);
            this.tsbStart.Text = "toolStripButton1";
            // 
            // tsbStop
            // 
            this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbStop.Image")));
            this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStop.Name = "tsbStop";
            this.tsbStop.Size = new System.Drawing.Size(23, 22);
            this.tsbStop.Text = "toolStripButton2";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "toolStripButton3";
            this.tsbSave.ToolTipText = "Ñîõðàíèòü êîíôèãóðàöèþ";
            this.tsbSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Îáíîâèòü ñïèñîê ñîáûòèé";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslServiceStatus,
            this.tsslUpdateProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 472);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(725, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslServiceStatus
            // 
            this.tsslServiceStatus.Name = "tsslServiceStatus";
            this.tsslServiceStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tsslUpdateProgress
            // 
            this.tsslUpdateProgress.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslUpdateProgress.Name = "tsslUpdateProgress";
            this.tsslUpdateProgress.Size = new System.Drawing.Size(102, 17);
            this.tsslUpdateProgress.Text = "tsslUpdateProgress";
            this.tsslUpdateProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxSave,
            this.toolStripSeparator2,
            this.ctxNewACMS,
            this.ctxNewTurnstile,
            this.toolStripMenuItem6,
            this.ctxDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(160, 104);
            // 
            // ctxSave
            // 
            this.ctxSave.Image = ((System.Drawing.Image)(resources.GetObject("ctxSave.Image")));
            this.ctxSave.Name = "ctxSave";
            this.ctxSave.Size = new System.Drawing.Size(159, 22);
            this.ctxSave.Text = "Ñîõðàíèòü";
            this.ctxSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // ctxNewACMS
            // 
            this.ctxNewACMS.Name = "ctxNewACMS";
            this.ctxNewACMS.Size = new System.Drawing.Size(159, 22);
            this.ctxNewACMS.Text = "Íîâàÿ ÑÊÓÄ";
            this.ctxNewACMS.Click += new System.EventHandler(this.tsmiNewACMS_Click);
            // 
            // ctxNewTurnstile
            // 
            this.ctxNewTurnstile.Name = "ctxNewTurnstile";
            this.ctxNewTurnstile.Size = new System.Drawing.Size(159, 22);
            this.ctxNewTurnstile.Text = "Íîâûé òóðíèêåò";
            this.ctxNewTurnstile.Click += new System.EventHandler(this.tsmiNewTurnstile_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(156, 6);
            // 
            // ctxDelete
            // 
            this.ctxDelete.Name = "ctxDelete";
            this.ctxDelete.Size = new System.Drawing.Size(159, 22);
            this.ctxDelete.Text = "Óäàëèòü";
            this.ctxDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "lock.ico");
            this.imageList1.Images.SetKeyName(1, "ball_yellow.ico");
            this.imageList1.Images.SetKeyName(2, "ball_white.ico");
            this.imageList1.Images.SetKeyName(3, "about.ico");
            this.imageList1.Images.SetKeyName(4, "info2.ico");
            this.imageList1.Images.SetKeyName(5, "delete.ico");
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 49);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvLog);
            this.splitContainer2.Size = new System.Drawing.Size(725, 423);
            this.splitContainer2.SplitterDistance = 241;
            this.splitContainer2.TabIndex = 4;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tvSettings);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer3.Size = new System.Drawing.Size(241, 423);
            this.splitContainer3.SplitterDistance = 235;
            this.splitContainer3.TabIndex = 0;
            // 
            // tvSettings
            // 
            this.tvSettings.ContextMenuStrip = this.contextMenuStrip1;
            this.tvSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSettings.ImageIndex = 0;
            this.tvSettings.ImageList = this.imageList1;
            this.tvSettings.Location = new System.Drawing.Point(0, 0);
            this.tvSettings.Name = "tvSettings";
            this.tvSettings.SelectedImageIndex = 0;
            this.tvSettings.Size = new System.Drawing.Size(241, 235);
            this.tvSettings.TabIndex = 1;
            this.tvSettings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSettings_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpBackColor = System.Drawing.SystemColors.Info;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(241, 184);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // lvLog
            // 
            this.lvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLog.FullRowSelect = true;
            this.lvLog.GridLines = true;
            this.lvLog.Location = new System.Drawing.Point(0, 0);
            this.lvLog.MultiSelect = false;
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(480, 423);
            this.lvLog.SmallImageList = this.imageList1;
            this.lvLog.TabIndex = 0;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.SelectedIndexChanged += new System.EventHandler(this.lvLog_SelectedIndexChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 494);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ìåíåäæåð òóðíèêåòîâ - êîíôèãóðàöèÿ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem ñåðâèñToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiStop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestart;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem êîíôèãóðàöèÿToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewACMS;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewTurnstile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem ñïðàâêàToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiHome;
        private System.Windows.Forms.ToolStripMenuItem tsmiUpdates;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripButton tsbStart;
        private System.Windows.Forms.ToolStripButton tsbStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ctxNewACMS;
        private System.Windows.Forms.ToolStripMenuItem ctxNewTurnstile;
        private System.Windows.Forms.ToolStripMenuItem ctxSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem ctxDelete;
        private System.Windows.Forms.ToolStripStatusLabel tsslServiceStatus;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView tvSettings;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewFilter;
        private System.Windows.Forms.ToolStripMenuItem tsmiReloadEvents;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem tsmiDetails;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripStatusLabel tsslUpdateProgress;
    }
}

