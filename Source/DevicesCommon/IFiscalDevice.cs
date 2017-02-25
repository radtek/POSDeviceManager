using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
	/// <summary>
	/// Аргументы делегата для фискального регистратора
	/// </summary>
	public class FiscalBreakEventArgs : EventArgs
	{
		private FiscalBreak breakReason;
		private bool canContinue;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="breakReason">Причина вызова</param>
		public FiscalBreakEventArgs(FiscalBreak breakReason) : base()
		{
			this.breakReason = breakReason;
			canContinue = true;
		}

		/// <summary>
		/// Причина вызова
		/// </summary>
		public FiscalBreak BreakReason
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
	/// Интерфейс драйвера фискального регистратора
	/// </summary>
	public interface IFiscalDevice : IPrintableDevice
	{
		#region Свойства фискального регистратора

		/// <summary>
		/// Флаги состояния
		/// </summary>
		FiscalStatusFlags Status { get; }

		/// <summary>
		/// Аппаратные характеристики фискального устройства
		/// </summary>
		FiscalDeviceInfo Info { get; }

		/// <summary>
		/// Дата и время фискальной памяти
		/// </summary>
		DateTime CurrentTimestamp { get; set; }

		#endregion

		#region События

		/// <summary>
		/// Событие, возникаемое при необходимости реакции пользователя 
		/// на состояние фискального устройства
		/// </summary>
		event EventHandler<FiscalBreakEventArgs> FiscalBreak;

		#endregion

		#region Функции и свойства режима налогового инспектора

		/// <summary>
		/// Фискализация
		/// </summary>
		/// <param name="newPassword">Новый пароль налогового инспектора</param>
		/// <param name="registrationNumber">Регистрационный номер</param>
		/// <param name="taxpayerNumber">ИНН</param>
		void Fiscalization(Int32 newPassword, Int64 registrationNumber, Int64 taxpayerNumber);

		/// <summary>
		/// Фискальный отчет
		/// </summary>
		/// <param name="reportType">Тип отчета</param>
		/// <param name="full">Полный или краткий отчет</param>
		/// <param name="reportOptions">Параметры отчета (зависят от типа отчета)</param>
		void FiscalReport(FiscalReportType reportType, bool full, params Object[] reportOptions);

		/// <summary>
		/// Диапазоны дат и смен
		/// </summary>
		/// <param name="firstDate">Дата первой закрытой смены</param>
		/// <param name="lastDate">Дата последней закрытой смены</param>
		/// <param name="firstShift">Номер первой смены</param>
		/// <param name="lastShift">Номер последней смены</param>
		void GetLifetime(out DateTime firstDate, out DateTime lastDate,
			out Int32 firstShift, out Int32 lastShift);

		/// <summary>
		/// Текущий пароль налогового инспектора
		/// Функции режима НИ вызываются с использованием этого пароля.
		/// Свойство должно быть инициализировано до вызова первой функции режима НИ
		/// </summary>
		Int32 TaxerPassword { get; set;	}

		#endregion
	}
}
