using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using ERPService.SharedLibs.PropertyGrid.Editors;
using ERPService.SharedLibs.PropertyGrid.Converters;

namespace ERPService.SharedLibs.Eventlog
{
    /// <summary>
    /// Параметры фильтра событий
    /// </summary>
    [Serializable]
    public abstract class EventLinkFilterBase : ICloneable
    {
        private Int32 _maxEvents;
        private String[] _eventSources;
        private Int32 _maxEventsPerIteration;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected EventLinkFilterBase()
        {
            ShowInfos = true;
            ShowErrors = true;
            ShowWarnings = true;
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
            _maxEvents = 100;
            _eventSources = new String[0];
            _maxEventsPerIteration = 100;
        }

        /// <summary>
        /// Возвращает список всех доступных источников событий
        /// </summary>
        public abstract String[] GetAvailableEventSources();

        /// <summary>
        /// Показывать информационные события
        /// </summary>
        [Browsable(true)]
        [Category("Типы событий")]
        [DisplayName("Информация")]
        [Description("Показывать информационные события")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public Boolean ShowInfos { get; set; }

        /// <summary>
        /// Показывать события, содержащие сведения об ошибках
        /// </summary>
        [Browsable(true)]
        [Category("Типы событий")]
        [DisplayName("Ошибки")]
        [Description("Показывать события, содержащие сведения об ошибках")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public Boolean ShowErrors { get; set; }

        /// <summary>
        /// Показывать события, содержащие предупреждения
        /// </summary>
        [Browsable(true)]
        [Category("Типы событий")]
        [DisplayName("Предупреждения")]
        [Description("Показывать события, содержащие предупреждения")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public Boolean ShowWarnings { get; set; }

        /// <summary>
        /// Начало интервала
        /// </summary>
        [Browsable(true)]
        [Category("Ограничения")]
        [DisplayName("Начало интервала")]
        [Description("Начало интервала для просмотра событий")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Конец интервала
        /// </summary>
        [Browsable(true)]
        [Category("Ограничения")]
        [DisplayName("Конец интервала")]
        [Description("Конец интервала для просмотра событий")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Максимальное число событий
        /// </summary>
        [Browsable(true)]
        [Category("Ограничения")]
        [DisplayName("Максимальное число событий")]
        [Description("Максимальное число событий, загружаемое с сервера")]
        [DefaultValue(100)]
        public Int32 MaxEvents
        {
            get { return _maxEvents; }
            set 
            {
                if ((value < 1 || value > Int32.MaxValue) && value != -1)
                    throw new ArgumentOutOfRangeException("value");
                _maxEvents = value; 
            }
        }

        /// <summary>
        /// Источники событий
        /// </summary>
        [Browsable(true)]
        [Category("Прочее")]
        [DisplayName("Источники событий")]
        [Description("Источники, события от которых нужно загружать")]
        [Editor(typeof(EventSourcesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public String[] EventSources
        {
            get { return _eventSources; }
            set 
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _eventSources = value; 
            }
        }

        /// <summary>
        /// Максимальный размер блока событий, загружаемый за одно обращение к источнику событий
        /// </summary>
        [Browsable(true)]
        [Category("Ограничения")]
        [DisplayName("Максимальный размер блока")]
        [Description("Максимальный размер блока событий, загружаемый за одно обращение к источнику событий")]
        [DefaultValue(100)]
        public Int32 MaxEventsPerIteration
        {
            get { return _maxEventsPerIteration; }
            set
            {
                if (value < 1 || value > Int32.MaxValue)
                    throw new ArgumentOutOfRangeException("value");
                _maxEventsPerIteration = value;
            }
        }

        /// <summary>
        /// Копирование значений 
        /// </summary>
        /// <param name="source">Фильтр-источник</param>
        public void Assign(EventLinkFilterBase source)
        {
            ShowInfos = source.ShowInfos;
            ShowErrors = source.ShowErrors;
            ShowWarnings = source.ShowWarnings;
            FromDate = source.FromDate;
            ToDate = source.ToDate;
            _maxEvents = source.MaxEvents;
            _eventSources = new String[source.EventSources.Length];
            _maxEventsPerIteration = source.MaxEventsPerIteration;
            Array.Copy(source.EventSources, _eventSources, source.EventSources.Length);
        }

        #region Реализация ICloneable

        /// <summary>
        /// Клонирование объекта
        /// </summary>
        public abstract Object Clone();

        #endregion
    }
}
