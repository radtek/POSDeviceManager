using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ERPService.SharedLibs.Eventlog.Properties;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Тип команды для работы с представлением списка событий
    /// </summary>
    public enum EventsViewCommand
    {
        /// <summary>
        /// Подробный просмотр события
        /// </summary>
        Details,

        /// <summary>
        /// Обновление списка событий
        /// </summary>
        Update,

        /// <summary>
        /// Очистка логов на источнике событий
        /// </summary>
        Cleanup,

        /// <summary>
        /// Вызов диалога для установки фильтра событий
        /// </summary>
        Filter
    }

    /// <summary>
    /// Просмотр журнала событий в ListView
    /// </summary>
    public class ListedEventsViewLink : IEventsViewLink
    {
        #region Поля

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

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="lv">Представление для размещения событий</param>
        /// <param name="filterParams">Исходная настройка фильтра для просмотра событий</param>
        /// <param name="viewSettings">Ширина колонок</param>
        /// <param name="notifyOnReloadProgress">Сообщать о прогрессе загрузки событий</param>
        /// <param name="replaceControlItemIcons">Заменять иконки у элементов управления</param>
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

            // список значков для событий
            _lvImages = new ImageList();
            _lvImages.ColorDepth = ColorDepth.Depth32Bit;
            _lvImages.ImageSize = new System.Drawing.Size(16, 16);
            _lvImages.Images.Add(Resources.Info);
            _lvImages.Images.Add(Resources.Error);
            _lvImages.Images.Add(Resources.Warning);

            // меняем вид представления под отображение лога
            _lv = lv;
            _lv.BeginUpdate();
            try
            {
                _lv.SmallImageList = _lvImages;
                _lv.LargeImageList = _lvImages;
                _lv.Items.Clear();
                _lv.Columns.Clear();

                // внешний вид и поведение
                _lv.GridLines = true;
                _lv.View = View.Details;
                _lv.FullRowSelect = true;
                _lv.MultiSelect = false;
                _lv.HideSelection = false;

                // добавляем колонки
                _viewSettings = viewSettings;
                if (_viewSettings == null)
                    _viewSettings = new ListedEventsViewSettings();

                _lv.Columns.Add("Дата и время", _viewSettings[0]);
                _lv.Columns.Add("Приложение", _viewSettings[1]);
                _lv.Columns.Add("Тип", _viewSettings[2]);
                _lv.Columns.Add("Текст", _viewSettings[3]);

                // вешаем обработчики событий
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

            // списки контролов для управления процессом просмотра логов
            _filterItems = new List<ToolStripItem>();
            _detailsItems = new List<ToolStripItem>();
            _updateItems = new List<ToolStripItem>();
            _cleanupItems = new List<ToolStripItem>();
            _detailedViewItems = new List<Control>();
            UpdateControlsEnabled();
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="lv">Представление для размещения событий</param>
        /// <param name="filterParams">Исходная настройка фильтра для просмотра событий</param>
        /// <param name="viewSettings">Ширина колонок</param>
        /// <param name="notifyOnReloadProgress">Сообщать о прогрессе загрузки событий</param>
        public ListedEventsViewLink(ListView lv, EventLinkFilterBase filterParams,
            ListedEventsViewSettings viewSettings, bool notifyOnReloadProgress)
            : this(lv, filterParams, viewSettings, notifyOnReloadProgress, true)
        {
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="lv">Представление для размещения событий</param>
        /// <param name="filterParams">Исходная настройка фильтра для просмотра событий</param>
        /// <param name="viewSettings">Ширина колонок</param>
        public ListedEventsViewLink(ListView lv, EventLinkFilterBase filterParams,
            ListedEventsViewSettings viewSettings)
            : this(lv, filterParams, viewSettings, false, true)
        {
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Добавляет управляющий элемент команды в список
        /// </summary>
        /// <param name="item">Управляющий элемент</param>
        /// <param name="commandType">Тип команды для работы с представлением</param>
        public void AddCommandItem(ToolStripItem item, EventsViewCommand commandType)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            switch (commandType)
            {
                case EventsViewCommand.Cleanup:
                    item.Text = "Удалить события";
                    item.Click += new EventHandler(OnCleanupClick);
                    _cleanupItems.Add(item);
                    break;
                case EventsViewCommand.Details:
                    item.Text = "Подробно";
                    item.Click += new EventHandler(OnDetailsClick);
                    _detailsItems.Add(item);
                    break;
                case EventsViewCommand.Filter:
                    item.Text = "Фильтр";
                    item.Click += new EventHandler(OnFilterClick);
                    if (_replaceControlItemIcons)
                        item.Image = Resources.Filter;
                    _filterItems.Add(item);
                    break;
                case EventsViewCommand.Update:
                    item.Text = "Обновить";
                    item.Click += new EventHandler(OnUpdateClick);
                    if (_replaceControlItemIcons)
                        item.Image = Resources.Refresh.ToBitmap();
                    _updateItems.Add(item);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Тип команды {0} не поддерживается", 
                        commandType));
            }
            UpdateControlsEnabled();
        }

        /// <summary>
        /// Добавляет элемент для детального просмотра текста события
        /// </summary>
        /// <param name="control">Элемент управления</param>
        public void AddDetailedViewControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            _detailedViewItems.Add(control);
        }

        /// <summary>
        /// Перезагрузка списка событий
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
        /// Коннектор к источнику событий
        /// </summary>
        public IEventSourceConnector SourceConnector
        {
            get { return _sourceConnector; }
            set 
            {
                if (_updating && value == null)
                    throw new InvalidOperationException("Дождитесь завершения обновления списка событий");

                _sourceConnector = value;
                Update();
            }
        }

        /// <summary>
        /// Ширина колонок
        /// </summary>
        public ListedEventsViewSettings ViewSettings
        {
            get { return _viewSettings; }
        }

        /// <summary>
        /// Признак выполняющегося обновления представления событий
        /// </summary>
        public bool Updating
        {
            get { return _updating; }
        }

        #endregion

        #region Закрытые свойства и методы

        #region Обработчики событий от контролов

        /// <summary>
        /// Очистить статистику
        /// </summary>
        private void OnCleanupClick(Object sender, EventArgs args)
        {
            DialogResult dr = MessageBox.Show(
                "Будут удалены события, попадающие под текущий фильтр по дате. Продолжить?",
                "Просмотр событий", MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Cancel)
                return;

            _lv.Parent.Cursor = Cursors.WaitCursor;
            try
            {
                // выполняем очистку логов
                _sourceConnector.OpenConnector();
                try
                {
                    _sourceConnector.Source.TruncLog(_filterParams.FromDate, _filterParams.ToDate);
                }
                finally
                {
                    _sourceConnector.CloseConnector();
                }

                // перезагружаем события
                ReloadEvents();
            }
            finally
            {
                _lv.Parent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Подробно
        /// </summary>
        private void OnDetailsClick(Object sender, EventArgs args)
        {
            EventsView.Show(this);
        }

        /// <summary>
        /// Фильтр
        /// </summary>
        private void OnFilterClick(Object sender, EventArgs args)
        {
            FormEventsFilterEdit filterEdit = new FormEventsFilterEdit();
            if (filterEdit.Execute(_filterParams))
            {
                // запоминаем параметры фильтрации
                _filterParams.Assign(filterEdit.NewValue);
                // перезагружаем список событий
                if (_sourceConnector != null && !_updating)
                    ReloadEvents();
            }
        }

        /// <summary>
        /// Обновить
        /// </summary>
        private void OnUpdateClick(Object sender, EventArgs args)
        {
            ReloadEvents();
        }

        #endregion

        #region Обработчики событий представления

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
        /// Обновляет доступность контролов
        /// </summary>
        private void UpdateControlsEnabled()
        {
            // удаление событий
            UpdateControlsEnabled(_cleanupItems, _sourceConnector != null);
            // подробно
            UpdateControlsEnabled(_detailsItems, _lv.SelectedIndices.Count > 0);
            UpdateDetailedControls(_lv.SelectedIndices.Count > 0);
            // фильтр
            UpdateControlsEnabled(_filterItems, true);
            // обновить
            UpdateControlsEnabled(_updateItems, _sourceConnector != null && !_updating);
        }

        private int InsertEvents(EventRecord[] eventRecords, int prevEventsCount)
        {
            // конвертируем события в элементы списка
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
        /// Перезагрузка событий в соответствии с текущим фильтром
        /// </summary>
        private void ReloadEvents()
        {
            if (_updating)
                return;

            _lv.BeginUpdate();
            try
            {
                // очищаем представление
                _lv.Items.Clear();
                UpdateControlsEnabled();

                if (_sourceConnector == null)
                    return;

                if (_notifyOnReloadProgress)
                    _sourceConnector.ReloadProgress(0);

                // формируем фильтр по типам событий
                List<EventType> eventTypeFilter = new List<EventType>();
                if (_filterParams.ShowInfos)
                    eventTypeFilter.Add(EventType.Information);
                if (_filterParams.ShowErrors)
                    eventTypeFilter.Add(EventType.Error);
                if (_filterParams.ShowWarnings)
                    eventTypeFilter.Add(EventType.Warning);

                _updating = true;

                // запускаем поток, загружающий события
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
                            // запрашиваем события у источника
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
                        // не даем упасть приложению, если GUI-поток уже завершился
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
        /// Индекс последней записи
        /// </summary>
        private int LastIndex
        {
            get { return _lv.Items.Count - 1; }
        }

        /// <summary>
        /// Возвращает ссылку на интерфейс записи по индексу и выбирает запись
        /// в списке
        /// </summary>
        /// <param name="index">Индекс записи</param>
        private EventRecord GetEventRecordByIndex(int index)
        {
            _lv.Items[index].Selected = true;
            return (EventRecord)_lv.Items[index].Tag;
        }

        #endregion

        #region Реализация IEventsViewLink

        /// <summary>
        /// Следующее событие
        /// </summary>
        public EventRecord NextEvent()
        {
            // если в списке есть события
            // и хотя бы одно выбрано
            if (_lv.Items.Count > 0 && _lv.SelectedItems.Count > 0)
            {
                // определяем индекс записи, на которую нужно переключиться
                int preferredIndex =
                    _lv.Items[LastIndex].Selected ? 0 : _lv.SelectedItems[0].Index + 1;
                // возвращаем запись
                return GetEventRecordByIndex(preferredIndex);
            }
            else
                return null;
        }

        /// <summary>
        /// Предыдущее событие
        /// </summary>
        public EventRecord PreviousEvent()
        {
            // если в списке есть события
            // и хотя бы одно выбрано
            if (_lv.Items.Count > 0 && _lv.SelectedItems.Count > 0)
            {
                // определяем индекс записи, на которую нужно переключиться
                int preferredIndex =
                    _lv.Items[0].Selected ? LastIndex : _lv.SelectedItems[0].Index - 1;
                // возвращаем запись
                return GetEventRecordByIndex(preferredIndex);
            }
            else
                return null;
        }

        /// <summary>
        /// Текущее событие
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
