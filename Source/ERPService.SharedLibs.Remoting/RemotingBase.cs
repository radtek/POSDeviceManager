using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

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
        protected string GetChannelName(string prefix)
        {
            return string.Format("{1}Channel{0}", prefix, Guid.NewGuid());
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
