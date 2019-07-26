using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PlanServerService.Hook
{
    /// <summary>
    /// ��Ҫ����֪ͨ��������
    /// </summary>
    public class DingHookAttribute : BaseHook
    {
        /// <summary>
        /// Ҫ����Hook֪ͨ��url
        /// </summary>
        public static List<string> Url { get; } = new List<string>();

        public string Title { get; private set; }
        public DingHookAttribute() : this("Job����")
        {
        }

        public DingHookAttribute(string title)
        {
            Title = title;
        }

        /// <summary>
        /// ���Ͷ�����Ϣ
        /// </summary>
        /// <param name="message"></param>
        public override void Hook(string message)
        {
            // curl "https://oapi.dingtalk.com/robot/send?access_token=aaa" -d '{"msgtype":"link","link":{"text":"test","title":"testtitle","picUrl":"","messageUrl":"http://baidu.com"}}' -H 'content-type: application/json'
            // curl "https://oapi.dingtalk.com/robot/send?access_token=aaa" -d '{"msgtype":"markdown","markdown":{"text":"# test","title":"testtitle"}}' -H 'content-type: application/json'

            //            var msg = "{\"msgtype\":\"link\",\"link\":{\"text\":\"" + ProcessChar(message) +
            //                      "\",\"title\":\"" + ProcessChar(Title) +
            //                      "\",\"picUrl\":\"\",\"messageUrl\":\"http://www.baidu.com\"}}";
            var msg = "{\"msgtype\":\"markdown\",\"markdown\":{\"text\":\"" + ProcessChar(message) +
                      "\",\"title\":\"" + ProcessChar(Title) +
                      "\"}}";
            foreach (var url in Url)
            {
                try
                {
                    var ret = GetPage(url, msg);
                    LogHelper.WriteInfo("Hook: " + ret);
                }
                catch (Exception exp)
                {
                    LogHelper.WriteException("Hook error", exp);
                }
            }
        }

        static string ProcessChar(string message)
        {
            return message.Replace("\"", "\\\"").Replace("\\", "\\\\");
        }
        
        static string GetPage(string url, string jsonMsg)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Add("Accept-Charset", "utf-8");
            request.UserAgent = "beinet1.0";
            request.AllowAutoRedirect = true; //����301��302֮���ת��ʱ���Ƿ�Ҫת��
            request.Method = "POST";
            request.ContentType = "application/json";

            // �����ύ������
            if (!string.IsNullOrEmpty(jsonMsg))
            {
                // ������ת��Ϊ�ֽ�����
                byte[] l_data = Encoding.UTF8.GetBytes(jsonMsg);
                request.ContentLength = l_data.Length;
                // ����������ContentLength�����ܴ�GetRequestStream
                // ContentLength���ú�reqStream.Closeǰ����д����ͬ�ֽڵ����ݣ�����Request�ᱻȡ��
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(l_data, 0, l_data.Length);
                    newStream.Close();
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                    return null;
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
