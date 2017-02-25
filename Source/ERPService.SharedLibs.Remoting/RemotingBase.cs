using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// ������� ����� ��� �������� � �����������
    /// </summary>
    public abstract class RemotingBase : IDisposable
    {
        /// <summary>
        /// ������ ����������� ������
        /// </summary>
        /// <param name="channel">�����</param>
        protected void SafeUnregisterChannel(IChannel channel)
        {
            if (channel == null)
                return;

            try
            {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
            }
            catch (RemotingException)
            {
            }
        }

        /// <summary>
        /// ��������� ��� ������
        /// </summary>
        /// <param name="prefix">�������</param>
        /// <returns>��� ���������� ������</returns>
        protected String GetChannelName(String prefix)
        {
            return String.Format("{1}Channel{0}", prefix, Guid.NewGuid());
        }

        #region ���������� IDisposable

        /// <summary>
        /// ������������ �������� � �.�.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
