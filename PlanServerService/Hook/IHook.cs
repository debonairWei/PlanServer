using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using NLog;
using System.Text;
using System.Web;

namespace PlanServerService.Hook
{
    /// <summary>
    /// ��Ҫ����֪ͨ�Ľӿ�
    /// </summary>
    public interface IHook
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="message"></param>
        void Hook(string message);
    }
}
