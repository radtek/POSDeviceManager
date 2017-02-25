namespace DevmanConfig
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ôàéëToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSvcStart = new System.Windows.Forms.ToolStripMenuItem();
            this.miSvcStop = new System.Windows.Forms.ToolStripMenuItem();
            this.miSvcRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.miClose = new System.Windows.Forms.ToolStripMenuItem();
            this.âèäToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.miFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.êîíôèãóðàöèÿToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.miAddDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.ñïðàâêàToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.îÏðîãðàììåToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbSvcVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbConfigVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbSvcStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslEventsReloadProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSvcStart = new System.Windows.Forms.ToolStripButton();
            this.btnSvcStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnAddDevice = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.tvDevices = new System.Windows.Forms.TreeView();
            this.cmConfig = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiTest = new System.Windows.Forms.ToolStripMenuItem();
            this.ilNodes = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.pnlConfig = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.cmLog = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.ilMessageTypes = new System.Windows.Forms.ImageList(this.components);
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvLog = new System.Windows.Forms.ListView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.cmConfig.SuspendLayout();
            this.pnlConfig.SuspendLayout();
            this.cmLog.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ôàéëToolStripMenuItem,
            this.âèäToolStripMenuItem,
            this.êîíôèãóðàöèÿToolStripMenuItem,
            this.ñïðàâêàToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(760, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ôàéëToolStripMenuItem
            // 
            this.ôàéëToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSvcStart,
            this.miSvcStop,
            this.miSvcRestart,
            this.toolStripSeparator1,
            this.miTest,
            this.toolStripSeparator6,
            this.miClose});
            this.ôàéëToolStripMenuItem.Name = "ôàéëToolStripMenuItem";
            this.ôàéëToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.ôàéëToolStripMenuItem.Text = "Ñåðâèñ";
            // 
            // miSvcStart
            // 
            this.miSvcStart.Image = global::DevmanConfig.Properties.Resources.start;
            this.miSvcStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miSvcStart.Name = "miSvcStart";
            this.miSvcStart.ShortcutKeyDisplayString = "F9";
            this.miSvcStart.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.miSvcStart.Size = new System.Drawing.Size(170, 22);
            this.miSvcStart.Text = "Ïóñê";
            // 
            // miSvcStop
            // 
            this.miSvcStop.Image = global::DevmanConfig.Properties.Resources.stop;
            this.miSvcStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miSvcStop.Name = "miSvcStop";
            this.miSvcStop.Size = new System.Drawing.Size(170, 22);
            this.miSvcStop.Text = "Ñòîï";
            // 
            // miSvcRestart
            // 
            this.miSvcRestart.Name = "miSvcRestart";
            this.miSvcRestart.Size = new System.Drawing.Size(170, 22);
            this.miSvcRestart.Text = "Ïåðåçàïóñê";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // miTest
            // 
            this.miTest.Image = global::DevmanConfig.Properties.Resources.test;
            this.miTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miTest.Name = "miTest";
            this.miTest.Size = new System.Drawing.Size(170, 22);
            this.miTest.Text = "Òåñò óñòðîéñòâà...";
            this.miTest.Click += new System.EventHandler(this.OnTest);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(167, 6);
            // 
            // miClose
            // 
            this.miClose.Name = "miClose";
            this.miClose.Size = new System.Drawing.Size(170, 22);
            this.miClose.Text = "Âûõîä";
            this.miClose.Click += new System.EventHandler(this.miClose_Click);
            // 
            // âèäToolStripMenuItem
            // 
            this.âèäToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRefresh,
            this.miFilter,
            this.toolStripMenuItem1,
            this.miDetails});
            this.âèäToolStripMenuItem.Name = "âèäToolStripMenuItem";
            this.âèäToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.âèäToolStripMenuItem.Text = "Âèä";
            // 
            // miRefresh
            // 
            this.miRefresh.Image = global::DevmanConfig.Properties.Resources.refresh;
            this.miRefresh.Name = "miRefresh";
            this.miRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miRefresh.Size = new System.Drawing.Size(145, 22);
            this.miRefresh.Text = "Îáíîâèòü";
            // 
            // miFilter
            // 
            this.miFilter.Image = global::DevmanConfig.Properties.Resources.filter;
            this.miFilter.Name = "miFilter";
            this.miFilter.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.miFilter.Size = new System.Drawing.Size(145, 22);
            this.miFilter.Text = "Ôèëüòð";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(142, 6);
            // 
            // miDetails
            // 
            this.miDetails.Name = "miDetails";
            this.miDetails.Size = new System.Drawing.Size(145, 22);
            this.miDetails.Text = "Ïîäðîáíî";
            // 
            // êîíôèãóðàöèÿToolStripMenuItem
            // 
            this.êîíôèãóðàöèÿToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSaveConfig,
            this.toolStripSeparator8,
            this.miAddDevice,
            this.miRemove});
            this.êîíôèãóðàöèÿToolStripMenuItem.Name = "êîíôèãóðàöèÿToolStripMenuItem";
            this.êîíôèãóðàöèÿToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.êîíôèãóðàöèÿToolStripMenuItem.Text = "Êîíôèãóðàöèÿ";
            // 
            // miSaveConfig
            // 
            this.miSaveConfig.Image = global::DevmanConfig.Properties.Resources.save;
            this.miSaveConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miSaveConfig.Name = "miSaveConfig";
            this.miSaveConfig.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSaveConfig.Size = new System.Drawing.Size(187, 22);
            this.miSaveConfig.Text = "Ñîõðàíèòü";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(184, 6);
            // 
            // miAddDevice
            // 
            this.miAddDevice.Image = global::DevmanConfig.Properties.Resources.add;
            this.miAddDevice.Name = "miAddDevice";
            this.miAddDevice.Size = new System.Drawing.Size(187, 22);
            this.miAddDevice.Text = "Äîáàâèòü óñòðîéñòâî";
            this.miAddDevice.Click += new System.EventHandler(this.OnAddDevice);
            // 
            // miRemove
            // 
            this.miRemove.Image = global::DevmanConfig.Properties.Resources.delete;
            this.miRemove.Name = "miRemove";
            this.miRemove.Size = new System.Drawing.Size(187, 22);
            this.miRemove.Text = "Óäàëèòü óñòðîéñòâî";
            this.miRemove.Click += new System.EventHandler(this.OnRemove);
            // 
            // ñïðàâêàToolStripMenuItem
            // 
            this.ñïðàâêàToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.îÏðîãðàììåToolStripMenuItem});
            this.ñïðàâêàToolStripMenuItem.Name = "ñïðàâêàToolStripMenuItem";
            this.ñïðàâêàToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.ñïðàâêàToolStripMenuItem.Text = "Ñïðàâêà";
            // 
            // îÏðîãðàììåToolStripMenuItem
            // 
            this.îÏðîãðàììåToolStripMenuItem.Name = "îÏðîãðàììåToolStripMenuItem";
            this.îÏðîãðàììåToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.îÏðîãðàììåToolStripMenuItem.Text = "Î ïðîãðàììå...";
            this.îÏðîãðàììåToolStripMenuItem.Click += new System.EventHandler(this.OnAbout);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbSvcVersion,
            this.lbConfigVersion,
            this.lbSvcStatus,
            this.tsslEventsReloadProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 591);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(760, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbSvcVersion
            // 
            this.lbSvcVersion.Name = "lbSvcVersion";
            this.lbSvcVersion.Size = new System.Drawing.Size(86, 17);
            this.lbSvcVersion.Text = "Âåðñèÿ ñëóæáû";
            // 
            // lbConfigVersion
            // 
            this.lbConfigVersion.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbConfigVersion.Name = "lbConfigVersion";
            this.lbConfigVersion.Size = new System.Drawing.Size(146, 17);
            this.lbConfigVersion.Text = "Âåðñèÿ êîíôèãóðàöèè: íåò";
            // 
            // lbSvcStatus
            // 
            this.lbSvcStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbSvcStatus.Name = "lbSvcStatus";
            this.lbSvcStatus.Size = new System.Drawing.Size(131, 17);
            this.lbSvcStatus.Text = "Ñëóæáà íå óñòàíîâëåíà";
            this.lbSvcStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tsslEventsReloadProgress
            // 
            this.tsslEventsReloadProgress.Name = "tsslEventsReloadProgress";
            this.tsslEventsReloadProgress.Size = new System.Drawing.Size(351, 17);
            this.tsslEventsReloadProgress.Spring = true;
            this.tsslEventsReloadProgress.Text = "tsslEventsReloadProgress";
            this.tsslEventsReloadProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSvcStart,
            this.btnSvcStop,
            this.toolStripSeparator2,
            this.btnTest,
            this.toolStripSeparator3,
            this.btnRefresh,
            this.btnFilter,
            this.toolStripSeparator4,
            this.btnSave,
            this.btnAddDevice,
            this.btnRemove});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(760, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSvcStart
            // 
            this.btnSvcStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSvcStart.Image = global::DevmanConfig.Properties.Resources.start;
            this.btnSvcStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSvcStart.Name = "btnSvcStart";
            this.btnSvcStart.Size = new System.Drawing.Size(23, 22);
            this.btnSvcStart.Text = "Çàïóñòèòü";
            // 
            // btnSvcStop
            // 
            this.btnSvcStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSvcStop.Image = global::DevmanConfig.Properties.Resources.stop;
            this.btnSvcStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSvcStop.Name = "btnSvcStop";
            this.btnSvcStop.Size = new System.Drawing.Size(23, 22);
            this.btnSvcStop.Text = "Îñòàíîâèòü";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnTest
            // 
            this.btnTest.Image = global::DevmanConfig.Properties.Resources.test;
            this.btnTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(120, 22);
            this.btnTest.Text = "Òåñò óñòðîéñòâà...";
            this.btnTest.ToolTipText = "Òåñò óñòðîéñòâà...";
            this.btnTest.Click += new System.EventHandler(this.OnTest);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::DevmanConfig.Properties.Resources.refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Îáíîâèòü";
            // 
            // btnFilter
            // 
            this.btnFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFilter.Image = global::DevmanConfig.Properties.Resources.filter;
            this.btnFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(23, 22);
            this.btnFilter.Text = "Ôèëüòð";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::DevmanConfig.Properties.Resources.save;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Ñîõðàíèòü êîíôèãóðàöèþ";
            this.btnSave.Click += new System.EventHandler(this.OnSaveConfig);
            // 
            // btnAddDevice
            // 
            this.btnAddDevice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddDevice.Image = global::DevmanConfig.Properties.Resources.add;
            this.btnAddDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddDevice.Name = "btnAddDevice";
            this.btnAddDevice.Size = new System.Drawing.Size(23, 22);
            this.btnAddDevice.Text = "Äîáàâèòü óñòðîéñòâî";
            this.btnAddDevice.Click += new System.EventHandler(this.OnAddDevice);
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Image = global::DevmanConfig.Properties.Resources.delete;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(23, 22);
            this.btnRemove.Text = "Óäàëèòü óñòðîéñòâî";
            this.btnRemove.Click += new System.EventHandler(this.OnRemove);
            // 
            // tvDevices
            // 
            this.tvDevices.ContextMenuStrip = this.cmConfig;
            this.tvDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvDevices.HideSelection = false;
            this.tvDevices.ImageIndex = 0;
            this.tvDevices.ImageList = this.ilNodes;
            this.tvDevices.Location = new System.Drawing.Point(0, 0);
            this.tvDevices.Name = "tvDevices";
            this.tvDevices.SelectedImageIndex = 0;
            this.tvDevices.Size = new System.Drawing.Size(286, 332);
            this.tvDevices.TabIndex = 3;
            this.tvDevices.DoubleClick += new System.EventHandler(this.OnTest);
            this.tvDevices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDevices_AfterSelect);
            // 
            // cmConfig
            // 
            this.cmConfig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiAddDevice,
            this.cmiRemove,
            this.toolStripSeparator9,
            this.cmiTest});
            this.cmConfig.Name = "contextMenu";
            this.cmConfig.Size = new System.Drawing.Size(197, 76);
            // 
            // cmiAddDevice
            // 
            this.cmiAddDevice.Image = global::DevmanConfig.Properties.Resources.add;
            this.cmiAddDevice.Name = "cmiAddDevice";
            this.cmiAddDevice.Size = new System.Drawing.Size(196, 22);
            this.cmiAddDevice.Text = "Äîáàâèòü óñòðîéñòâî";
            this.cmiAddDevice.Click += new System.EventHandler(this.OnAddDevice);
            // 
            // cmiRemove
            // 
            this.cmiRemove.Image = global::DevmanConfig.Properties.Resources.delete;
            this.cmiRemove.Name = "cmiRemove";
            this.cmiRemove.Size = new System.Drawing.Size(196, 22);
            this.cmiRemove.Text = "Óäàëèòü óñòðîéñòâî";
            this.cmiRemove.Click += new System.EventHandler(this.OnRemove);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(193, 6);
            // 
            // cmiTest
            // 
            this.cmiTest.Image = global::DevmanConfig.Properties.Resources.test;
            this.cmiTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmiTest.Name = "cmiTest";
            this.cmiTest.Size = new System.Drawing.Size(196, 22);
            this.cmiTest.Text = "Òåñò óñòðîéñòâà...";
            this.cmiTest.Click += new System.EventHandler(this.OnTest);
            // 
            // ilNodes
            // 
            this.ilNodes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNodes.ImageStream")));
            this.ilNodes.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNodes.Images.SetKeyName(0, "root.ico");
            this.ilNodes.Images.SetKeyName(1, "category.ico");
            this.ilNodes.Images.SetKeyName(2, "item.ico");
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.propertyGrid.HelpBackColor = System.Drawing.SystemColors.Info;
            this.propertyGrid.Location = new System.Drawing.Point(0, 332);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(286, 210);
            this.propertyGrid.TabIndex = 5;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // pnlConfig
            // 
            this.pnlConfig.Controls.Add(this.splitter1);
            this.pnlConfig.Controls.Add(this.tvDevices);
            this.pnlConfig.Controls.Add(this.propertyGrid);
            this.pnlConfig.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlConfig.Location = new System.Drawing.Point(0, 49);
            this.pnlConfig.Name = "pnlConfig";
            this.pnlConfig.Size = new System.Drawing.Size(286, 542);
            this.pnlConfig.TabIndex = 6;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 329);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(286, 3);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // cmLog
            // 
            this.cmLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiRefresh,
            this.cmiFilter});
            this.cmLog.Name = "cmLog";
            this.cmLog.Size = new System.Drawing.Size(136, 48);
            // 
            // cmiRefresh
            // 
            this.cmiRefresh.Image = global::DevmanConfig.Properties.Resources.refresh;
            this.cmiRefresh.Name = "cmiRefresh";
            this.cmiRefresh.Size = new System.Drawing.Size(135, 22);
            this.cmiRefresh.Text = "Îáíîâèòü";
            // 
            // cmiFilter
            // 
            this.cmiFilter.Image = global::DevmanConfig.Properties.Resources.filter;
            this.cmiFilter.Name = "cmiFilter";
            this.cmiFilter.Size = new System.Drawing.Size(135, 22);
            this.cmiFilter.Text = "Ôèëüòð";
            // 
            // ilMessageTypes
            // 
            this.ilMessageTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMessageTypes.ImageStream")));
            this.ilMessageTypes.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMessageTypes.Images.SetKeyName(0, "info.ico");
            this.ilMessageTypes.Images.SetKeyName(1, "info.ico");
            this.ilMessageTypes.Images.SetKeyName(2, "error.ico");
            this.ilMessageTypes.Images.SetKeyName(3, "warning.ico");
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(286, 49);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 542);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(289, 49);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvLog);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Size = new System.Drawing.Size(471, 542);
            this.splitContainer1.SplitterDistance = 385;
            this.splitContainer1.TabIndex = 9;
            // 
            // lvLog
            // 
            this.lvLog.ContextMenuStrip = this.cmLog;
            this.lvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLog.FullRowSelect = true;
            this.lvLog.GridLines = true;
            this.lvLog.HideSelection = false;
            this.lvLog.Location = new System.Drawing.Point(0, 0);
            this.lvLog.MultiSelect = false;
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(471, 385);
            this.lvLog.SmallImageList = this.ilMessageTypes;
            this.lvLog.TabIndex = 8;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(471, 153);
            this.textBox1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 613);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.pnlConfig);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(570, 370);
            this.Name = "MainForm";
            this.Text = "Ôîðèíò-Ñ: Äèñïåò÷åð óñòðîéñòâ. Êîíôèãóðàòîð";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cmConfig.ResumeLayout(false);
            this.pnlConfig.ResumeLayout(false);
            this.cmLog.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TreeView tvDevices;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ContextMenuStrip cmConfig;
        private System.Windows.Forms.ToolStripMenuItem cmiAddDevice;
        private System.Windows.Forms.ToolStripMenuItem cmiRemove;
        private System.Windows.Forms.ToolStripMenuItem ôàéëToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miClose;
        private System.Windows.Forms.ToolStripMenuItem êîíôèãóðàöèÿToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miAddDevice;
        private System.Windows.Forms.ToolStripMenuItem miRemove;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnAddDevice;
        private System.Windows.Forms.ImageList ilNodes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ToolStripButton btnSvcStart;
        private System.Windows.Forms.ToolStripButton btnSvcStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnTest;
        private System.Windows.Forms.ToolStripMenuItem miSvcStart;
        private System.Windows.Forms.ToolStripMenuItem miSvcStop;
        private System.Windows.Forms.ToolStripMenuItem miSvcRestart;
        private System.Windows.Forms.ToolStripMenuItem miSaveConfig;
        private System.Windows.Forms.ToolStripStatusLabel lbSvcStatus;
        private System.Windows.Forms.ToolStripMenuItem ñïðàâêàToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem îÏðîãðàììåToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miTest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem cmiTest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripStatusLabel lbConfigVersion;
        private System.Windows.Forms.ToolStripStatusLabel lbSvcVersion;
        private System.Windows.Forms.Panel pnlConfig;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList ilMessageTypes;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnFilter;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripMenuItem âèäToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miRefresh;
        private System.Windows.Forms.ToolStripMenuItem miFilter;
        private System.Windows.Forms.ContextMenuStrip cmLog;
        private System.Windows.Forms.ToolStripMenuItem cmiRefresh;
        private System.Windows.Forms.ToolStripMenuItem cmiFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miDetails;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripStatusLabel tsslEventsReloadProgress;
    }
}

