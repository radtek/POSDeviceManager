using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ERPService.SharedLibs.Eventlog.Properties;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// ��� ������� ��� ������ � �������������� ������ �������
    /// </summary>
    public enum EventsViewCommand
    {
        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        Details,

        /// <summary>
        /// ���������� ������ �������
        /// </summary>
        Update,

        /// <summary>
        /// ������� ����� �� ��������� �������
        /// </summary>
        Cleanup,

        /// <summary>
        /// ����� ������� ��� ��������� ������� �������
        /// </summary>
        Filter
    }

    /// <summary>
    /// �������� ������� ������� � ListView
    /// </summary>
    public class ListedEventsViewLink : IEventsViewLink
    {
        #region ����

        private ListView _lv;
        private EventLinkFilterBase _filterParams;
        private IEventSourceConnector _sourceConnector;
        private List<ToolStripItem> _filterItems;
        private List<ToolStripItem> _detailsItems;
        private List<ToolStripItem> _updateItems;
        private List<ToolStripItem> _cleanupItems;
        private List<Control> _detailedViewItems;
        private ImageList _lvImages;
        private ListedEventsViewSettings _viewSettings;
        private SynchronizationContext _guiSyncContext;
        private bool _notifyOnReloadProgress;
        private bool _replaceControlItemIcons;
        private volatile bool _updating;

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="lv">������������� ��� ���������� �������</param>
        /// <param name="filterParams">�������� ��������� ������� ��� ��������� �������</param>
        /// <param name="viewSettings">������ �������</param>
        /// <param name="notifyOnReloadProgress">�������� � ��������� �������� �������</param>
        /// <param name="replaceControlItemIcons">�������� ������ � ��������� ����������</param>
        public ListedEventsViewLink(ListView lv, EventLinkFilterBase filterParams,
            ListedEventsViewSettings viewSettings, bool notifyOnReloadProgress,
            bool replaceControlItemIcons)
        {
            if (lv == null)
                throw new ArgumentNullException("lv");
            if (filterParams == null)
                throw new ArgumentNullException("filterParams");

            _guiSyncContext = SynchronizationContext.Current;
            _notifyOnReloadProgress = notifyOnReloadProgress;
            _replaceControlItemIcons = replaceControlItemIcons;

            // ������ ������� ��� �������
            _lvImages = new ImageList();
            _lvImages.ColorDepth = ColorDepth.Depth32Bit;
            _lvImages.ImageSize = new System.Drawing.Size(16, 16);
            _lvImages.Images.Add(Resources.Info);
            _lvImages.Images.Add(Resources.Error);
            _lvImages.Images.Add(Resources.Warning);

            // ������ ��� ������������� ��� ����������� ����
            _lv = lv;
            _lv.BeginUpdate();
            try
            {
                _lv.SmallImageList = _lvImages;
                _lv.LargeImageList = _lvImages;
                _lv.Items.Clear();
                _lv.Columns.Clear();

                // ������� ��� � ���������
                _lv.GridLines = true;
                _lv.View = View.Details;
                _lv.FullRowSelect = true;
                _lv.MultiSelect = false;
                _lv.HideSelection = false;

                // ��������� �������
                _viewSettings = viewSettings;
                if (_viewSettings == null)
                    _viewSettings = new ListedEventsViewSettings();

                _lv.Columns.Add("���� � �����", _viewSettings[0]);
                _lv.Columns.Add("����������", _viewSettings[1]);
                _lv.Columns.Add("���", _viewSettings[2]);
                _lv.Columns.Add("�����", _viewSettings[3]);

                // ������ ����������� �������
                _lv.DoubleClick += new EventHandler(OnListViewDoubleClick);
                _lv.ColumnClick += new ColumnClickEventHandler(OnListViewColumnClick);
                _lv.SelectedIndexChanged += new EventHandler(OnListViewSelectedIndexChanged);
                _lv.ColumnWidthChanged += new ColumnWidthChangedEventHandler(OnListViewColumnWidthChanged);
            }
            finally
            {
                _lv.EndUpdate();
            }

            _filterParams = filterParams;

            // ������ ��������� ��� ���������� ��������� ��������� �����
            _filterItems = new List<ToolStripItem>();
            _detailsItems = new List<ToolStripItem>();
            _updateItems = new List<ToolStripItem>();
            _cleanupItems = new List<ToolStripItem>();
            _detailedViewItems = new List<Control>();
            UpdateControlsEnabled();
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="lv">������������� ��� ���������� �������</param>
        /// <param name="filterParams">�������� ��������� ������� ��� ��������� �������</param>
        /// <param name="viewSettings">������ �������</param>
        /// <param name="notifyOnReloadProgress">�������� � ��������� �������� �������</param>
        public ListedEventsViewLink(ListView lv, EventLinkFilterBase filterParams,
            ListedEventsViewSettings viewSettings, bool notifyOnReloadProgress)
            : this(lv, filterParams, viewSettings, notifyOnReloadProgress, true)
        {
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="lv">������������� ��� ���������� �������</param>
        /// <param name="filterParams">�������� ��������� ������� ��� ��������� �������</param>
        /// <param name="viewSettings">������ �������</param>
        public ListedEventsViewLink(ListView lv, EventLinkFilterBase filterParams,
            ListedEventsViewSettings viewSettings)
            : this(lv, filterParams, viewSettings, false, true)
        {
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ��������� ����������� ������� ������� � ������
        /// </summary>
        /// <param name="item">����������� �������</param>
        /// <param name="commandType">��� ������� ��� ������ � ��������������</param>
        public void AddCommandItem(ToolStripItem item, EventsViewCommand commandType)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            switch (commandType)
            {
                case EventsViewCommand.Cleanup:
                    item.Text = "������� �������";
                    item.Click += new EventHandler(OnCleanupClick);
                    _cleanupItems.Add(item);
                    break;
                case EventsViewCommand.Details:
                    item.Text = "��������";
                    item.Click += new EventHandler(OnDetailsClick);
                    _detailsItems.Add(item);
                    break;
                case EventsViewCommand.Filter:
                    item.Text = "������";
                    item.Click += new EventHandler(OnFilterClick);
                    if (_replaceControlItemIcons)
                        item.Image = Resources.Filter;
                    _filterItems.Add(item);
                    break;
                case EventsViewCommand.Update:
                    item.Text = "��������";
                    item.Click += new EventHandler(OnUpdateClick);
                    if (_replaceControlItemIcons)
                        item.Image = Resources.Refresh.ToBitmap();
                    _updateItems.Add(item);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("��� ������� {0} �� ��������������", 
                        commandType));
            }
            UpdateControlsEnabled();
        }

        /// <summary>
        /// ��������� ������� ��� ���������� ��������� ������ �������
        /// </summary>
        /// <param name="control">������� ����������</param>
        public void AddDetailedViewControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            _detailedViewItems.Add(control);
        }

        /// <summary>
        /// ������������ ������ �������
        /// </summary>
        public void Update()
        {
            _lv.Parent.Cursor = Cursors.WaitCursor;
            try
            {
                ReloadEvents();
            }
            finally
            {
                _lv.Parent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// ��������� � ��������� �������
        /// </summary>
        public IEventSourceConnector SourceConnector
        {
            get { return _sourceConnector; }
            set 
            {
                if (_updating && value == null)
                    throw new InvalidOperationException("��������� ���������� ���������� ������ �������");

                _sourceConnector = value;
                Update();
            }
        }

        /// <summary>
        /// ������ �������
        /// </summary>
        public ListedEventsViewSettings ViewSettings
        {
            get { return _viewSettings; }
        }

        /// <summary>
        /// ������� �������������� ���������� ������������� �������
        /// </summary>
        public bool Updating
        {
            get { return _updating; }
        }

        #endregion

        #region �������� �������� � ������

        #region ����������� ������� �� ���������

        /// <summary>
        /// �������� ����������
        /// </summary>
        private void OnCleanupClick(Object sender, EventArgs args)
        {
            DialogResult dr = MessageBox.Show(
                "����� ������� �������, ���������� ��� ������� ������ �� ����. ����������?",
                "�������� �������", MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Cancel)
                return;

            _lv.Parent.Cursor = Cursors.WaitCursor;
            try
            {
                // ��������� ������� �����
                _sourceConnector.OpenConnector();
                try
                {
                    _sourceConnector.Source.TruncLog(_filterParams.FromDate, _filterParams.ToDate);
                }
                finally
                {
                    _sourceConnector.CloseConnector();
                }

                // ������������� �������
                ReloadEvents();
            }
            finally
            {
                _lv.Parent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void OnDetailsClick(Object sender, EventArgs args)
        {
            EventsView.Show(this);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void OnFilterClick(Object sender, EventArgs args)
        {
            FormEventsFilterEdit filterEdit = new FormEventsFilterEdit();
            if (filterEdit.Execute(_filterParams))
            {
                // ���������� ��������� ����������
                _filterParams.Assign(filterEdit.NewValue);
                // ������������� ������ �������
                if (_sourceConnector != null && !_updating)
                    ReloadEvents();
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void OnUpdateClick(Object sender, EventArgs args)
        {
            ReloadEvents();
        }

        #endregion

        #region ����������� ������� �������������

        private void OnListViewColumnWidthChanged(Object sender, ColumnWidthChangedEventArgs args)
        {
            _viewSettings[args.ColumnIndex] = _lv.Columns[args.ColumnIndex].Width;
        }

        private void OnListViewDoubleClick(Object sender, EventArgs args)
        {
            if (_lv.SelectedIndices.Count > 0)
                OnDetailsClick(sender, args);
        }

        private void OnListViewColumnClick(Object sender, ColumnClickEventArgs args)
        {
            OnFilterClick(sender, args);
        }

        private void OnListViewSelectedIndexChanged(Object sender, EventArgs args)
        {
            UpdateControlsEnabled();
        }

        #endregion

        private void UpdateControlsEnabled(List<ToolStripItem> controls, bool value)
        {
            foreach (ToolStripItem item in controls)
            {
                item.Enabled = value;
            }
        }

        private void UpdateDetailedControls(bool value)
        {
            string text = string.Empty;
            if (value)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string line in Current.Text)
                {
                    sb.AppendLine(line);
                }
                text = sb.ToString();
            }

            foreach (Control ctrl in _detailedViewItems)
            {
                ctrl.Text = text;
            }
        }

        /// <summary>
        /// ��������� ����������� ���������
        /// </summary>
        private void UpdateControlsEnabled()
        {
            // �������� �������
            UpdateControlsEnabled(_cleanupItems, _sourceConnector != null);
            // ��������
            UpdateControlsEnabled(_detailsItems, _lv.SelectedIndices.Count > 0);
            UpdateDetailedControls(_lv.SelectedIndices.Count > 0);
            // ������
            UpdateControlsEnabled(_filterItems, true);
            // ��������
            UpdateControlsEnabled(_updateItems, _sourceConnector != null && !_updating);
        }

        private int InsertEvents(EventRecord[] eventRecords, int prevEventsCount)
        {
            // ������������ ������� � �������� ������
            var lviBuf = new ListViewItem[eventRecords.Length];

            for (int i = 0; i < eventRecords.Length; i++)
            {
                lviBuf[i] = new ListViewItem();
                lviBuf[i].Text = eventRecords[i].Timestamp.ToString("dd MMM yyyy  HH:mm:ss");
                lviBuf[i].ImageIndex = (int)eventRecords[i].EventType - 1;
                lviBuf[i].SubItems.Add(eventRecords[i].Source);
                lviBuf[i].SubItems.Add(EventTypeConvertor.ConvertFrom(eventRecords[i].EventType));
                lviBuf[i].SubItems.Add(eventRecords[i].Text[0]);
                lviBuf[i].Tag = eventRecords[i];
            }

            prevEventsCount += eventRecords.Length;

            _guiSyncContext.Send((state) =>
                {
                    
                    _lv.Items.AddRange(lviBuf);

                    if (_notifyOnReloadProgress)
                        _sourceConnector.ReloadProgress(prevEventsCount);

                }, null);

            return prevEventsCount;
        }

        /// <summary>
        /// ������������ ������� � ������������ � ������� ��������
        /// </summary>
        private void ReloadEvents()
        {
            if (_updating)
                return;

            _lv.BeginUpdate();
            try
            {
                // ������� �������������
                _lv.Items.Clear();
                UpdateControlsEnabled();

                if (_sourceConnector == null)
                    return;

                if (_notifyOnReloadProgress)
                    _sourceConnector.ReloadProgress(0);

                // ��������� ������ �� ����� �������
                List<EventType> eventTypeFilter = new List<EventType>();
                if (_filterParams.ShowInfos)
                    eventTypeFilter.Add(EventType.Information);
                if (_filterParams.ShowErrors)
                    eventTypeFilter.Add(EventType.Error);
                if (_filterParams.ShowWarnings)
                    eventTypeFilter.Add(EventType.Warning);

                _updating = true;

                // ��������� �����, ����������� �������
                var reloadThread = new Thread(() =>
                {
                    try
                    {
                        _guiSyncContext.Send((state) =>
                        {
                            _lv.BeginUpdate();
                        }, null);

                        try
                        {
                            // ����������� ������� � ���������
                            _sourceConnector.OpenConnector();
                            try
                            {
                                var source = _sourceConnector.Source;
                                if (source == null)
                                    return;

                                var iteratorId = source.BeginGetLog(
                                    _filterParams.FromDate,
                                    _filterParams.ToDate,
                                    _filterParams.EventSources,
                                    eventTypeFilter.ToArray(),
                                    _filterParams.MaxEvents,
                                    _filterParams.MaxEventsPerIteration);

                                try
                                {
                                    var prevEventsCount = 0;

                                    EventRecord[] eventRecords = null;
                                    do
                                    {
                                        eventRecords = source.GetLog(iteratorId);
                                        if (eventRecords != null)
                                        {
                                            prevEventsCount = InsertEvents(eventRecords,
                                                prevEventsCount);
                                        }
                                    }
                                    while (eventRecords != null);
                                }
                                finally
                                {
                                    source.EndGetLog(iteratorId);
                                }
                            }
                            finally
                            {
                                _sourceConnector.CloseConnector();
                            }
                        }
                        finally
                        {
                            _guiSyncContext.Send((state) =>
                            {
                                _lv.EndUpdate();
                                _updating = false;
                                UpdateControlsEnabled();
                            }, null);
                        }
                    }
                    catch (System.ComponentModel.InvalidAsynchronousStateException)
                    {
                        // �� ���� ������ ����������, ���� GUI-����� ��� ����������
                    }
                });

                reloadThread.Priority = ThreadPriority.Lowest;
                reloadThread.Start();
            }
            finally
            {
                _lv.EndUpdate();
                UpdateControlsEnabled();
            }
        }

        /// <summary>
        /// ������ ��������� ������
        /// </summary>
        private int LastIndex
        {
            get { return _lv.Items.Count - 1; }
        }

        /// <summary>
        /// ���������� ������ �� ��������� ������ �� ������� � �������� ������
        /// � ������
        /// </summary>
        /// <param name="index">������ ������</param>
        private EventRecord GetEventRecordByIndex(int index)
        {
            _lv.Items[index].Selected = true;
            return (EventRecord)_lv.Items[index].Tag;
        }

        #endregion

        #region ���������� IEventsViewLink

        /// <summary>
        /// ��������� �������
        /// </summary>
        public EventRecord NextEvent()
        {
            // ���� � ������ ���� �������
            // � ���� �� ���� �������
            if (_lv.Items.Count > 0 && _lv.SelectedItems.Count > 0)
            {
                // ���������� ������ ������, �� ������� ����� �������������
                int preferredIndex =
                    _lv.Items[LastIndex].Selected ? 0 : _lv.SelectedItems[0].Index + 1;
                // ���������� ������
                return GetEventRecordByIndex(preferredIndex);
            }
            else
                return null;
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        public EventRecord PreviousEvent()
        {
            // ���� � ������ ���� �������
            // � ���� �� ���� �������
            if (_lv.Items.Count > 0 && _lv.SelectedItems.Count > 0)
            {
                // ���������� ������ ������, �� ������� ����� �������������
                int preferredIndex =
                    _lv.Items[0].Selected ? LastIndex : _lv.SelectedItems[0].Index - 1;
                // ���������� ������
                return GetEventRecordByIndex(preferredIndex);
            }
            else
                return null;
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        public EventRecord Current
        {
            get
            {
                if (_lv.Items.Count > 0 && _lv.SelectedItems.Count > 0)
                {
                    return (EventRecord)_lv.SelectedItems[0].Tag;
                }
                else
                    return null;
            }
        }

        #endregion
    }
}
