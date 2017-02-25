using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
	/// <summary>
	/// Базовый класс для фискальных устройств
	/// </summary>
	public abstract class CustomFiscalDevice : CustomPrintableDevice, IFiscalDevice
	{
		// пароль НИ
		private Int32 taxerPassword;

		#region Конструктор

		/// <summary>
		/// Создает фискальное устройство
		/// </summary>
		protected CustomFiscalDevice() : base()
		{
		}

		#endregion

		#region Реализация IFiscalDevice

		/// <summary>
		/// Дата и время фискальной памяти
		/// </summary>
		public abstract DateTime CurrentTimestamp { get; set; }

		/// <summary>
		/// Событие, возникаемое при необходимости реакции пользователя 
		/// на состояние фискального устройства
		/// </summary>
		public abstract event EventHandler<FiscalBreakEventArgs> FiscalBreak;

		/// <summary>
		/// Фискальный отчет
		/// </summary>
		/// <param name="reportType">Тип отчета</param>
		/// <param name="full">Полный или краткий отчет</param>
		/// <param name="reportParams">Параметры отчета (зависят от типа отчета)</param>
		public abstract void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams);

		/// <summary>
		/// Фискализация
		/// </summary>
		/// <param name="newPassword">Новый пароль налогового инспектора</param>
		/// <param name="registrationNumber">Регистрационный номер</param>
		/// <param name="taxPayerNumber">ИНН</param>
		public abstract void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber);

		/// <summary>
		/// Диапазоны дат и смен
		/// </summary>
		/// <param name="firstDate">Дата первой закрытой смены</param>
		/// <param name="lastDate">Дата последней закрытой смены</param>
		/// <param name="firstShift">Номер первой смены</param>
		/// <param name="lastShift">Номер последней смены</param>
		public abstract void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, 
			out int lastShift);

		/// <summary>
		/// Аппаратные характеристики фискального устройства
		/// </summary>
		public abstract FiscalDeviceInfo Info { get; }

		/// <summary>
		/// Флаги состояния
		/// </summary>
		public abstract FiscalStatusFlags Status { get; }

		/// <summary>
		/// Текущий пароль налогового инспектора
		/// Функции режима НИ вызываются с использованием этого пароля.
		/// Свойство должно быть инициализировано до вызова первой функции режима НИ
		/// </summary>
		public int TaxerPassword
		{
			get
			{
				return taxerPassword;
			}
			set
			{
				taxerPassword = value;
			}
		}

		#endregion
	}
}
