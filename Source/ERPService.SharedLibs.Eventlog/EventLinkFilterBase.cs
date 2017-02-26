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
    /// ��������� ������� �������
    /// </summary>
    [Serializable]
    public abstract class EventLinkFilterBase : ICloneable
    {
        private int _maxEvents;
        private string[] _eventSources;
        private int _maxEventsPerIteration;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected EventLinkFilterBase()
        {
            ShowInfos = true;
            ShowErrors = true;
            ShowWarnings = true;
            FromDate = DateTime.Today.AddDays(-7);
            ToDate = DateTime.Today;
            _maxEvents = 100;
            _eventSources = new string[0];
            _maxEventsPerIteration = 100;
        }

        /// <summary>
        /// ���������� ������ ���� ��������� ���������� �������
        /// </summary>
        public abstract string[] GetAvailableEventSources();

        /// <summary>
        /// ���������� �������������� �������
        /// </summary>
        [Browsable(true)]
        [Category("���� �������")]
        [DisplayName("����������")]
        [Description("���������� �������������� �������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool ShowInfos { get; set; }

        /// <summary>
        /// ���������� �������, ���������� �������� �� �������
        /// </summary>
        [Browsable(true)]
        [Category("���� �������")]
        [DisplayName("������")]
        [Description("���������� �������, ���������� �������� �� �������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool ShowErrors { get; set; }

        /// <summary>
        /// ���������� �������, ���������� ��������������
        /// </summary>
        [Browsable(true)]
        [Category("���� �������")]
        [DisplayName("��������������")]
        [Description("���������� �������, ���������� ��������������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(RussianBooleanConverter))]
        public bool ShowWarnings { get; set; }

        /// <summary>
        /// ������ ���������
        /// </summary>
        [Browsable(true)]
        [Category("�����������")]
        [DisplayName("������ ���������")]
        [Description("������ ��������� ��� ��������� �������")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// ����� ���������
        /// </summary>
        [Browsable(true)]
        [Category("�����������")]
        [DisplayName("����� ���������")]
        [Description("����� ��������� ��� ��������� �������")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// ������������ ����� �������
        /// </summary>
        [Browsable(true)]
        [Category("�����������")]
        [DisplayName("������������ ����� �������")]
        [Description("������������ ����� �������, ����������� � �������")]
        [DefaultValue(100)]
        public int MaxEvents
        {
            get { return _maxEvents; }
            set 
            {
                if ((value < 1 || value > int.MaxValue) && value != -1)
                    throw new ArgumentOutOfRangeException("value");
                _maxEvents = value; 
            }
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        [Browsable(true)]
        [Category("������")]
        [DisplayName("��������� �������")]
        [Description("���������, ������� �� ������� ����� ���������")]
        [Editor(typeof(EventSourcesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HideValueConverter))]
        public string[] EventSources
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
        /// ������������ ������ ����� �������, ����������� �� ���� ��������� � ��������� �������
        /// </summary>
        [Browsable(true)]
        [Category("�����������")]
        [DisplayName("������������ ������ �����")]
        [Description("������������ ������ ����� �������, ����������� �� ���� ��������� � ��������� �������")]
        [DefaultValue(100)]
        public int MaxEventsPerIteration
        {
            get { return _maxEventsPerIteration; }
            set
            {
                if (value < 1 || value > int.MaxValue)
                    throw new ArgumentOutOfRangeException("value");
                _maxEventsPerIteration = value;
            }
        }

        /// <summary>
        /// ����������� �������� 
        /// </summary>
        /// <param name="source">������-��������</param>
        public void Assign(EventLinkFilterBase source)
        {
            ShowInfos = source.ShowInfos;
            ShowErrors = source.ShowErrors;
            ShowWarnings = source.ShowWarnings;
            FromDate = source.FromDate;
            ToDate = source.ToDate;
            _maxEvents = source.MaxEvents;
            _eventSources = new string[source.EventSources.Length];
            _maxEventsPerIteration = source.MaxEventsPerIteration;
            Array.Copy(source.EventSources, _eventSources, source.EventSources.Length);
        }

        #region ���������� ICloneable

        /// <summary>
        /// ������������ �������
        /// </summary>
        public abstract Object Clone();

        #endregion
    }
}
