using System;
using System.Drawing;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// Аргументы обработчика события печатающего устройства
    /// </summary>
    public class PrinterBreakEventArgs : EventArgs
	{
		private PrinterBreak breakReason;
		private bool canContinue;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="breakReason">Причина вызова</param>
		public PrinterBreakEventArgs(PrinterBreak breakReason) : base()
		{
			this.breakReason = breakReason;
			canContinue = true;
		}

		/// <summary>
		/// Причина вызова
		/// </summary>
		public PrinterBreak BreakReason
		{
			get
			{
				return breakReason;
			}
		}

		/// <summary>
		/// Флаг продолжения работы
		/// </summary>
		public bool CanContinue
		{
			get
			{
				return canContinue;
			}
			set
			{
				canContinue = value;
			}
		}
	}

	/// <summary>
	/// Интерфейс драйвера печатающего устройства
	/// </summary>
	public interface IPrintableDevice : ISerialDevice
	{
		#region Свойства печатающего устройства

		/// <summary>
		/// Флаги состояния принтера
		/// </summary>
		PrinterStatusFlags PrinterStatus { get; }

		/// <summary>
		/// Аппаратные характеристики печатающего устройства
		/// </summary>
		PrintableDeviceInfo PrinterInfo { get; }

        /// <summary>
        /// Открытие денежного ящика
        /// </summary>
        void OpenDrawer();


        #region Шапка и подвал документа

        /// <summary>
		/// Заголовок документа
		/// </summary>
        string[] DocumentHeader { get; set; }

        /// <summary>
        /// Подвал документа
        /// </summary>
        string[] DocumentFooter { get; set; }

        /// <summary>
        /// Графический заголовок документа
        /// </summary>
        Bitmap GraphicHeader { get; set; }

        /// <summary>
        /// Графический подвал документа
        /// </summary>
        Bitmap GraphicFooter { get; set; }

        /// <summary>
        /// Печатать графический заголовок чека
        /// </summary>
        bool PrintGraphicHeader { get; set; }

        /// <summary>
        /// Печатать графический подвал документа
        /// </summary>
        bool PrintGraphicFooter { get; set; }

        /// <summary>
        /// Печатать заголовок документа
        /// </summary>
        bool PrintHeader { get; set; }

        /// <summary>
        /// Печатать подвал документа
        /// </summary>
        bool PrintFooter { get; set; }

        #endregion

        /// <summary>
		/// Символ-разделитель логических секций документа
		/// </summary>
		Char Separator { get; set; }

		#endregion

		#region Работа с документом

		/// <summary>
		/// Печать документа
		/// </summary>
		/// <param name="xmlData">Данные XML-документа</param>
		void Print(string xmlData);

		#endregion

		#region События

		/// <summary>
		/// Событие, возникаемое при необходимости реакции пользователя
		/// на состояние печатающего устройства
		/// </summary>
		event EventHandler<PrinterBreakEventArgs> PrinterBreak;

		#endregion
	}
}
