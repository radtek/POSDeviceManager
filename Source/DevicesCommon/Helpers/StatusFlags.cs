using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace DevicesCommon.Helpers
{
	/// <summary>
	/// Флаги состояния принтера
	/// </summary>
	[Serializable]
	public class PrinterStatusFlags : ISerializable
	{
		#region Статусные поля печатающего устройства

		/// <summary>
		/// Идет печать документа
		/// </summary>
		public readonly bool Printing;

		/// <summary>
		/// Нет бумаги
		/// </summary>
		public readonly PaperOutStatus PaperOut;

		/// <summary>
		/// Открытый документ
		/// </summary>
		public readonly bool OpenedDocument;

		/// <summary>
		/// Открытый денежный ящик
		/// </summary>
		public readonly bool OpenedDrawer;

		#endregion

		#region  Конструктор

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="printing">Идет печать документа</param>
		/// <param name="paperOut">Нет бумаги</param>
		/// <param name="openedDocument">Открытый документ</param>
		/// <param name="openedDrawer">Открытый денежный ящик</param>
		public PrinterStatusFlags(bool printing, PaperOutStatus paperOut, bool openedDocument,
			bool openedDrawer)
		{
			Printing = printing;
			PaperOut = paperOut;
			OpenedDocument = openedDocument;
			OpenedDrawer = openedDrawer;
		}

		/// <summary>
		/// Конструктор для десериализации
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Источник для сериализации</param>
		protected PrinterStatusFlags(SerializationInfo info, StreamingContext context)
		{
            Printing = info.GetBoolean("Printing");
            PaperOut = (PaperOutStatus)info.GetValue("PaperOut", typeof(PaperOutStatus));
            OpenedDocument = info.GetBoolean("OpenedDocument");
            OpenedDrawer = info.GetBoolean("OpenedDrawer");
		}

		#endregion

		#region Сериализация

		/// <summary>
		/// Сериализация объектов класса
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Источник для сериализации</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Printing", Printing);
			info.AddValue("PaperOut", PaperOut);
			info.AddValue("OpenedDocument", OpenedDocument);
			info.AddValue("OpenedDrawer", OpenedDrawer);
		}

		#endregion
	}

	/// <summary>
	/// Флаги состояния фискального устройства
	/// </summary>
	[Serializable]
	public class FiscalStatusFlags : ISerializable
	{
		#region Статусные поля фискального устройства

		/// <summary>
		/// Открытая смена
		/// </summary>
		public readonly bool OpenedShift;

		/// <summary>
		/// Смена превысила 24 час
		/// </summary>
		public readonly bool OverShift;

		/// <summary>
		/// Блокирован (напр., по неправильному паролю НИ)
		/// </summary>
		public readonly bool Locked;

		/// <summary>
		/// Фискализирован
		/// </summary>
		public readonly bool Fiscalized;

		/// <summary>
		/// Сумма открытого документа
		/// Равна нулю, если документ закрыт или документ нефискальный
		/// </summary>
		public readonly UInt64 DocumentAmount;

		/// <summary>
		/// Сумма наличных денег в денежном ящике в МДЕ
		/// </summary>
		public readonly UInt64 CashInDrawer;

		#endregion

		#region Конструктор

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="openedShift">Открытая смена</param>
		/// <param name="overShift">Смена превысила 24 часа</param>
		/// <param name="locked">Заблокировано</param>
		/// <param name="fiscalized">Фискализировано</param>
		/// <param name="documentAmount">Сумма открытого документа</param>
		/// <param name="cashInDrawer">Сумма наличности в денежном ящике</param>
		public FiscalStatusFlags(bool openedShift, bool overShift, bool locked, bool fiscalized,
			UInt64 documentAmount, UInt64 cashInDrawer)
		{
			OpenedShift = openedShift;
			OverShift = overShift;
			Locked = locked;
			Fiscalized = fiscalized;
			DocumentAmount = documentAmount;
			CashInDrawer = cashInDrawer;
		}

		/// <summary>
		/// Конструктор для десериализации
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Источник для сериализации</param>
		protected FiscalStatusFlags(SerializationInfo info, StreamingContext context)
		{
            OpenedShift = info.GetBoolean("OpenedShift");
            OverShift = info.GetBoolean("OverShift");
            Locked = info.GetBoolean("Locked");
            Fiscalized = info.GetBoolean("Fiscalized");
            DocumentAmount = info.GetUInt64("DocumentAmount");
            CashInDrawer = info.GetUInt64("CashInDrawer");
		}

		#endregion

		#region Сериализация

		/// <summary>
		/// Сериализация объектов класса
		/// </summary>
		/// <param name="info">Класс <see cref="SerializationInfo"/> для информации о сериализации</param>
		/// <param name="context">Источник для сериализации</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("OpenedShift", OpenedShift);
			info.AddValue("OverShift", OverShift);
			info.AddValue("Locked", Locked);
			info.AddValue("Fiscalized", Fiscalized);
            info.AddValue("DocumentAmount", DocumentAmount);
            info.AddValue("CashInDrawer", CashInDrawer);
		}

		#endregion
	}
}
