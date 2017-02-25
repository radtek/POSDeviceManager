using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using ERPService.SharedLibs.PropertyGrid;
using ERPService.SharedLibs.PropertyGrid.Converters;
using ERPService.SharedLibs.PropertyGrid.Editors;

namespace TsBiometricsLogic
{
    [Serializable]
    public class CMSBiometricsLogicSettings
    {
        public CMSBiometricsLogicSettings()
        {
            HostOrIp = "localhost";
            Port = 34601;
        }

        [DisplayName("Имя хоста")]
        [Description("Имя хоста или его IP-адрес для подключения к \"Форинт-CMS\"")]
        [Category("Прочее")]
        [DefaultValue("localhost")]
        public string HostOrIp { get; set; }

        [DisplayName("Порт")]
        [Description("TCP-порт для подключения к \"Форинт-CMS\"")]
        [Category("Прочее")]
        [DefaultValue(34601)]
        public Int32 Port { get; set; }

        [DisplayName("Минимальный баланс")]
        [Description("Минимальный баланс на счете клиента")]
        [Category("Прочее")]
        [DefaultValue(0)]
        public decimal MinBalance { get; set; }

        [DisplayName("Регистрировать посещение")]
        [Description("Выполнять регистрацию посещения клиента в сервере CMS при успешной авторизации")]
        [Category("Регистрация посещения")]
        [DefaultValue(false)]
        [TypeConverter(typeof(RusBooleanConverter))]
        [Editor(typeof(BooleanEditor), typeof(UITypeEditor))]
        public bool RegisterVisit { get; set; }

        [DisplayName("Точка обслуживания")]
        [Description("Идентификатор точки приема карт в сервере CMS")]
        [Category("Регистрация посещения")]
        public string PointId { get; set; }


        [DisplayName("Терминал")]
        [Description("Номер терминала в сервере CMS")]
        [Category("Регистрация посещения")]
        [DefaultValue(1)]
        public int TerminalNo { get; set; }
    }
}
