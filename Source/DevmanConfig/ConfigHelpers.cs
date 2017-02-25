using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevicesCommon.Helpers;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;
using DevicesCommon;
using ERPService.SharedLibs.PropertyGrid;
using ERPService.SharedLibs.PropertyGrid.Converters;
using ERPService.SharedLibs.PropertyGrid.Editors;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevmanConfig
{
    #region Редакторы свойств

    /// <summary>
    /// Редактор строки подключения к весам
    /// </summary>
    internal class ScalesConnectionEditor : UITypeEditor
    {
        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    using (ScalesConnectionEditorForm editorForm = new ScalesConnectionEditorForm())
                    {
                        editorForm.ConnectionString = (string)value;
                        if (svc.ShowDialog(editorForm) == DialogResult.OK)
                            value = editorForm.ConnectionString;
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Возвращаем стиль редактора - модальное окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
                return UITypeEditorEditStyle.Modal;
            else
                return base.GetEditStyle(context);
        }
    }

    /// <summary>
    /// Редактор графического клише (подвала)
    /// </summary>
    internal class GraphicHeaderEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value != null)
            {
                e.Graphics.DrawImage((System.Drawing.Bitmap)e.Value, e.Bounds);
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
                return UITypeEditorEditStyle.Modal;
            else
                return base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    using (OpenFileDialog fileDlg = new OpenFileDialog())
                    {
                        fileDlg.Filter = "Файлы рисунков (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
                        if (fileDlg.ShowDialog() == DialogResult.OK)
                        {
                            value = new System.Drawing.Bitmap(fileDlg.FileName);
                        }
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }
    }

    /// <summary>
    /// Редактор свойств вида "список ключ-значение"
    /// </summary>
    internal class KeyValueEditor : UITypeEditor
    {
        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    using (KeyValueEditorForm editorForm = new KeyValueEditorForm())
                    {
                        editorForm.Collection = (IDictionary<string, string>)value;
                        if (svc.ShowDialog(editorForm) == DialogResult.OK)
                            value = editorForm.Collection;
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Возвращаем стиль редактора - модальное окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
                return UITypeEditorEditStyle.Modal;
            else
                return base.GetEditStyle(context);
        }
    }

    /// <summary>
    /// Редактор направления прохода турникета
    /// </summary>
    internal class EntryTypeEditor : CustomDropdownEditor
    {
        public override string[] Values
        {
            get { return new string[] { "Вход", "Выход" }; }
        }

        protected override Int32 ObjectToIndex(Object value)
        {
            return (Int32)value;
        }

        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return (TurnstileDirection)selectedIndex;
        }
    }

    /// <summary>
    /// Редактор направления прохода турникета
    /// </summary>
    internal class PrinterKindEditor : CustomDropdownEditor
    {
        public override string[] Values
        {
            get { return new string[] { "на чековой ленте", "на подкладном документе", "комбинированный" }; }
        }

        protected override Int32 ObjectToIndex(Object value)
        {
            return (Int32)value;
        }

        protected override Object IndexToObject(Int32 selectedIndex)
        {
            return (PrinterKind)selectedIndex;
        }
    }

    #endregion

    #region Конвертеры

    /// <summary>
    /// Конвертер направления прохода турникета
    /// </summary>
    internal class EntryTypeConverter : TypeConverter
    {
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value,
                  Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                return new EntryTypeEditor().Values[(Int32)value];
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Конвертер вида принтера (рулонный, подкладной, комбинированный)
    /// </summary>
    internal class PrinterKindConverter : TypeConverter
    {
        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value,
                  Type destinationType)
        {
            if (destinationType == typeof(String))
                return new PrinterKindEditor().Values[(Int32)value];
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Конвертер для графического клише (подвала)
    /// </summary>
    internal class GraphicHeaderTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return "(нажмите на кнопку справа)";
        }
    }

    /// <summary>
    /// Конвертер портов
    /// </summary>
    internal class PortTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<string> ports = new List<string>();
            ports.AddRange(SerialPortsEnumerator.Enumerate());
            ports.AddRange(SerialPortsEnumerator.EnumerateLPT());

            return new StandardValuesCollection(ports);
        }
    }

    /// <summary>
    /// Базовый класс конвертеров типов устройств. Выбирает все типы устройств в
    /// сборках каталога
    /// </summary>
    internal class DeviceTypesConverterBase : StringConverter 
    {
        private static List<DeviceAttribute> _deviceTypes;

        protected static List<DeviceAttribute> DeviceTypes
        {
            get
            {
                if (_deviceTypes == null)
                {
                    _deviceTypes = new List<DeviceAttribute>();
                    string baseFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    foreach (string fileName in System.IO.Directory.GetFiles(baseFolder, "*.dll", System.IO.SearchOption.AllDirectories))
                    {
                        try
                        {
                            Assembly currAsm = System.Reflection.Assembly.LoadFile(fileName);
                            foreach (Type currType in currAsm.GetTypes().Where(t =>
                                t.IsPublic && !t.IsAbstract))
                            {
                                // выбираем с атрибутом DeviceAttribute
                                if (currType.IsDefined(typeof(DeviceAttribute), true))
                                {
                                    _deviceTypes.Add((DeviceAttribute)Attribute.GetCustomAttribute(currType, typeof(DeviceAttribute)));
                                    //break;
                                }
                            }
                        }
                        catch (BadImageFormatException)
                        {
                            // неуправляемая сборка
                        }
                    }
                }
                return _deviceTypes;
            }
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    /// <summary>
    /// Конвертер типов устройств. Выбирает типы устройств для указанного атрибута
    /// </summary>
    /// <typeparam name="T">Атрибут типа устройства</typeparam>
    internal class DeviceTypesConverter<T> : DeviceTypesConverterBase 
        where T: DeviceAttribute
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList deviceNames = new ArrayList();
            foreach (DeviceAttribute attr in DeviceTypes)
            {
                if (attr is T)
                    deviceNames.Add(attr.DeviceType);
            }
            return new StandardValuesCollection(deviceNames);
        }
    }

    /// <summary>
    /// Конвертер типов устройств. Выбирает типы устройств принтеров и фискальнков
    /// </summary>
    internal class PrintableTypesConverter : DeviceTypesConverterBase
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList deviceNames = new ArrayList();
            foreach (DeviceAttribute attr in DeviceTypes)
            {
                if (attr is PrintableDeviceAttribute || attr is FiscalDeviceAttribute)
                    deviceNames.Add(attr.DeviceType);
            }
            return new StandardValuesCollection(deviceNames);
        }
    }

    /// <summary>
    /// Конвертер для сортировки свойств по номеру
    /// </summary>
    internal class PropertySorter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Возвращает упорядоченный список свойств
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(
          ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            ArrayList orderedProperties = new ArrayList();

            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                if (attribute != null)
                {
                    // атрибут есть - используем номер п/п из него
                    PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    // атрибута нет – считаем, что 0
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }

            // сортируем по Order-у
            orderedProperties.Sort();

            // формируем список имен свойств
            ArrayList propertyNames = new ArrayList();

            foreach (PropertyOrderPair pop in orderedProperties)
                propertyNames.Add(pop.Name);

            // возвращаем
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }
    }


    #endregion

    #region Атрибуты

    /// <summary>
    /// Атрибут для задания порядка сортировки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal class PropertyOrderAttribute : Attribute
    {
        private int _order;

        public PropertyOrderAttribute(int order)
        {
            _order = order;
        }

        public int Order
        {
            get { return _order; }
        }
    }

    #endregion

    #region Вспомогательные классы

    /// <summary>
    /// Вспомогательный класс для сортировки свойств по номеру
    /// </summary>
    internal class PropertyOrderPair : IComparable
    {
        private int _order;
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public PropertyOrderPair(string name, int order)
        {
            _order = order;
            _name = name;
        }

        /// <summary>
        /// Собственно метод сравнения
        /// </summary>
        public int CompareTo(object obj)
        {
            int otherOrder = ((PropertyOrderPair)obj)._order;

            if (otherOrder == _order)
            {
                // если Order одинаковый - сортируем по именам
                string otherName = ((PropertyOrderPair)obj)._name;
                return string.Compare(_name, otherName);
            }
            else if (otherOrder > _order)
                return -1;

            return 1;
        }
    }

    /// <summary>
    /// Вспомогательный класс, реализующий сериализуемый в xml словарь
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerizable methods

        void IXmlSerializable.ReadXml(XmlReader r)
        {
            XmlSerializer keySer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer = new XmlSerializer(typeof(TValue));
            r.Read();
            r.ReadStartElement("Dictionary");
            while (r.NodeType != XmlNodeType.EndElement)
            {
                r.ReadStartElement("KeyValuePair");
                r.ReadStartElement("Key");
                TKey key = (TKey)keySer.Deserialize(r);
                r.ReadEndElement();
                r.ReadStartElement("Value");
                TValue value = (TValue)valueSer.Deserialize(r);
                r.ReadEndElement();
                this.Add(key, value);
                r.ReadEndElement();
                r.MoveToContent();
            }
            r.ReadEndElement();
            r.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter w)
        {
            XmlSerializer keySer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSer = new XmlSerializer(typeof(TValue));
            w.WriteStartElement("Dictionary");
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                w.WriteStartElement("KeyValuePair");
                w.WriteStartElement("Key");
                keySer.Serialize(w, pair.Key);
                w.WriteEndElement();
                w.WriteStartElement("Value");
                valueSer.Serialize(w, pair.Value);
                w.WriteEndElement();
                w.WriteEndElement();
            }
            w.WriteEndElement();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        #endregion
    }

    #endregion
}
