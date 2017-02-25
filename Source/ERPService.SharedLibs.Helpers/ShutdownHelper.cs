using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// 64-bit value guaranteed to be unique only on the system on which it was generated
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID
    {
        /// <summary>
        /// Low order bits
        /// </summary>
        public UInt32 LowPart;

        /// <summary>
        /// High order bits
        /// </summary>
        public Int32 HighPart;
    }

    /// <summary>
    /// Represents a locally unique identifier (LUID) and its attributes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID_AND_ATTRIBUTES
    {
        /// <summary>
        /// Specifies an LUID value
        /// </summary>
        /// <seealso cref="LUID"/>
        public LUID Luid;

        /// <summary>
        /// Specifies attributes of the LUID
        /// </summary>
        public UInt32 Attributes;
    }

    /// <summary>
    /// Contains information about a set of privileges for an access token
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_PRIVILEGES
    {
        /// <summary>
        /// Specifies the number of entries in the Privileges array
        /// </summary>
        public UInt32 PrivilegeCount;

        /// <summary>
        /// Specifies an array of LUID_AND_ATTRIBUTES structures.
        /// Each structure contains the LUID and attributes of a privilege
        /// </summary>
        /// <seealso cref="LUID_AND_ATTRIBUTES"/>
        /// <seealso cref="LUID"/>
        public LUID_AND_ATTRIBUTES Privileges;
    }

    /// <summary>
    /// ������ ������� � �������� Windows API
    /// </summary>
    internal static class WinApi
    {
        #region ������������ ���������

        private const String kernel32 = "kernel32.dll";
        private const String advapi32 = "advapi32.dll";

        #endregion

        #region ��������� 

        /// <summary>
        /// Required to enable or disable the privileges in an access token
        /// </summary>
        public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;

        /// <summary>
        /// Required to query an access token
        /// </summary>
        public const UInt32 TOKEN_QUERY = 0x0008;

        /// <summary>
        /// The function enables the privilege
        /// </summary>
        public const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;

        /// <summary>
        /// Shutdown privilege
        /// </summary>
        public const String SeShutdownPrivilege = "SeShutdownPrivilege";

        #endregion

        #region ������������� �������

        /// <summary>
        /// Retrieves a pseudo handle for the current process
        /// </summary>
        /// <returns></returns>
        [DllImport(kernel32, SetLastError=false)]
        public static extern Int32 GetCurrentProcess();

        /// <summary>
        /// Retrieves the calling thread's last-error code value
        /// </summary>
        /// <returns></returns>
        [DllImport(kernel32, SetLastError=false)]
        public static extern UInt32 GetLastError();

        /// <summary>
        /// Closes an open object handle
        /// </summary>
        /// <param name="hObject">Handle to an open object</param>
        [DllImport(kernel32, SetLastError = true)]
        public static extern Boolean CloseHandle(Int32 hObject);

        /// <summary>
        /// Opens the access token associated with a process
        /// </summary>
        /// <param name="ProcessHandle">Handle to the process whose access token is opened</param>
        /// <param name="DesiredAccess">Specifies an access mask that specifies the requested types of access to the access token</param>
        /// <param name="TokenHandle">Pointer to a handle that identifies the newly opened access token when the function returns</param>
        [DllImport(advapi32, SetLastError=true)]
        public static extern Boolean OpenProcessToken(
            Int32 ProcessHandle,
            UInt32 DesiredAccess,
            out Int32 TokenHandle);

        /// <summary>
        /// retrieves the locally unique identifier (LUID) used on a specified system 
        /// to locally represent the specified privilege name
        /// </summary>
        /// <param name="lpSystemName">Specifies the name of the system on which 
        /// the privilege name is retrieved</param>
        /// <param name="lpName">Specifies the name of the privilege</param>
        /// <param name="lpLuid">Receives the LUID by which the privilege is known 
        /// on the system specified by the lpSystemName parameter</param>
        [DllImport(advapi32, SetLastError=true)]
        public static extern Boolean LookupPrivilegeValue(
          String lpSystemName,
          String lpName,
          out LUID lpLuid);

        /// <summary>
        /// Enables or disables privileges in the specified access token
        /// </summary>
        /// <param name="TokenHandle">Handle to the access token that contains the privileges 
        /// to be modified</param>
        /// <param name="DisableAllPrivileges">Specifies whether the function disables all of the 
        /// token's privileges</param>
        /// <param name="NewState">Specifies an array of privileges and their attributes</param>
        /// <param name="BufferLength">Specifies the size, in bytes, of the buffer pointed to by the 
        /// PreviousState parameter</param>
        /// <param name="PreviousState">Contains the previous state of any privileges that 
        /// the function modifies</param>
        /// <param name="ReturnLength">Receives the required size, in bytes, of the buffer pointed 
        /// to by the PreviousState parameter</param>
        [DllImport(advapi32, SetLastError=true)]
        public static extern Boolean AdjustTokenPrivileges(
          Int32 TokenHandle,
          Boolean DisableAllPrivileges,
          ref TOKEN_PRIVILEGES NewState,
          UInt32 BufferLength,
          IntPtr PreviousState,
          IntPtr ReturnLength);

        /// <summary>
        /// Initiates a shutdown and optional restart of the specified computer
        /// </summary>
        /// <param name="lpMachineName">Specifies the network name of the computer to shut down</param>
        /// <param name="lpMessage">Specifies a message to display in the shutdown dialog box</param>
        /// <param name="dwTimeout">Time that the shutdown dialog box should be displayed, 
        /// in seconds</param>
        /// <param name="bForceAppsClosed">If this parameter is TRUE, applications with unsaved 
        /// changes are to be forcibly closed</param>
        /// <param name="bRebootAfterShutdown">If this parameter is TRUE, the computer is to 
        /// restart immediately after shutting down</param>
        [DllImport(advapi32, SetLastError = true)]
        public static extern Boolean InitiateSystemShutdown(
          String lpMachineName,
          String lpMessage,
          UInt32 dwTimeout,
          Boolean bForceAppsClosed,
          Boolean bRebootAfterShutdown);

        #endregion
    }

	/// <summary>
	/// ��������������� ����� ��� ���������� ������ �������
	/// </summary>
	public static class ShutdownHelper
    {
        #region �������� ������

        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="reboot">������������ ����� ���������� ������</param>
        private static void InternalShutdown(Boolean reboot)
        {
            // �������� ������ �������� ��������
            Int32 tokenHandle;
            Boolean apiCr = WinApi.OpenProcessToken(
                WinApi.GetCurrentProcess(),
                WinApi.TOKEN_ADJUST_PRIVILEGES | WinApi.TOKEN_QUERY,
                out tokenHandle);
            Win32Check(apiCr);

            // �������� ������������� ����������� ����������
            TOKEN_PRIVILEGES tokenPrivileges;
            apiCr = WinApi.LookupPrivilegeValue(
                null, 
                WinApi.SeShutdownPrivilege,
                out tokenPrivileges.Privileges.Luid);
            Win32Check(apiCr);

            // ����������� ����������� ����������
            tokenPrivileges.PrivilegeCount = 1;
            tokenPrivileges.Privileges.Attributes = WinApi.SE_PRIVILEGE_ENABLED;
            apiCr = WinApi.AdjustTokenPrivileges(
                tokenHandle,
                false,
                ref tokenPrivileges,
                0,
                IntPtr.Zero,
                IntPtr.Zero);
            Win32Check(apiCr);

            // ��������� ������ �������
            apiCr = WinApi.InitiateSystemShutdown(
                null,
                null,
                0,
                true,
                reboot);
            Win32Check(apiCr);
        }

        /// <summary>
        /// ������� ���������� ������ API-�������
        /// </summary>
        private static void Win32Check(Boolean result)
        {
            if (!result)
            {
                UInt32 error = WinApi.GetLastError();
                if (error != 0)
                    throw new Win32Exception((Int32)error);
            }
        }

        #endregion

        #region �������� ������

        /// <summary>
        /// ���������� �������
        /// </summary>
        public static void Shutdown()
        {
            InternalShutdown(false);
		}

        /// <summary>
        /// ������������
        /// </summary>
        public static void Reboot()
        {
            InternalShutdown(true);
        }

        #endregion
    }
}
