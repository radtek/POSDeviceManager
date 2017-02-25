using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace DevicesCommon.Helpers
{
	/// <summary>
	/// ����� ��������� ��������
	/// </summary>
	[Serializable]
	public class PrinterStatusFlags : ISerializable
	{
		#region ��������� ���� ����������� ����������

		/// <summary>
		/// ���� ������ ���������
		/// </summary>
		public readonly bool Printing;

		/// <summary>
		/// ��� ������
		/// </summary>
		public readonly PaperOutStatus PaperOut;

		/// <summary>
		/// �������� ��������
		/// </summary>
		public readonly bool OpenedDocument;

		/// <summary>
		/// �������� �������� ����
		/// </summary>
		public readonly bool OpenedDrawer;

		#endregion

		#region  �����������

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="printing">���� ������ ���������</param>
		/// <param name="paperOut">��� ������</param>
		/// <param name="openedDocument">�������� ��������</param>
		/// <param name="openedDrawer">�������� �������� ����</param>
		public PrinterStatusFlags(bool printing, PaperOutStatus paperOut, bool openedDocument,
			bool openedDrawer)
		{
			Printing = printing;
			PaperOut = paperOut;
			OpenedDocument = openedDocument;
			OpenedDrawer = openedDrawer;
		}

		/// <summary>
		/// ����������� ��� ��������������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ��� ������������</param>
		protected PrinterStatusFlags(SerializationInfo info, StreamingContext context)
		{
            Printing = info.GetBoolean("Printing");
            PaperOut = (PaperOutStatus)info.GetValue("PaperOut", typeof(PaperOutStatus));
            OpenedDocument = info.GetBoolean("OpenedDocument");
            OpenedDrawer = info.GetBoolean("OpenedDrawer");
		}

		#endregion

		#region ������������

		/// <summary>
		/// ������������ �������� ������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ��� ������������</param>
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
	/// ����� ��������� ����������� ����������
	/// </summary>
	[Serializable]
	public class FiscalStatusFlags : ISerializable
	{
		#region ��������� ���� ����������� ����������

		/// <summary>
		/// �������� �����
		/// </summary>
		public readonly bool OpenedShift;

		/// <summary>
		/// ����� ��������� 24 ���
		/// </summary>
		public readonly bool OverShift;

		/// <summary>
		/// ���������� (����., �� ������������� ������ ��)
		/// </summary>
		public readonly bool Locked;

		/// <summary>
		/// ��������������
		/// </summary>
		public readonly bool Fiscalized;

		/// <summary>
		/// ����� ��������� ���������
		/// ����� ����, ���� �������� ������ ��� �������� ������������
		/// </summary>
		public readonly UInt64 DocumentAmount;

		/// <summary>
		/// ����� �������� ����� � �������� ����� � ���
		/// </summary>
		public readonly UInt64 CashInDrawer;

		#endregion

		#region �����������

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="openedShift">�������� �����</param>
		/// <param name="overShift">����� ��������� 24 ����</param>
		/// <param name="locked">�������������</param>
		/// <param name="fiscalized">���������������</param>
		/// <param name="documentAmount">����� ��������� ���������</param>
		/// <param name="cashInDrawer">����� ���������� � �������� �����</param>
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
		/// ����������� ��� ��������������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ��� ������������</param>
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

		#region ������������

		/// <summary>
		/// ������������ �������� ������
		/// </summary>
		/// <param name="info">����� <see cref="SerializationInfo"/> ��� ���������� � ������������</param>
		/// <param name="context">�������� ��� ������������</param>
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
