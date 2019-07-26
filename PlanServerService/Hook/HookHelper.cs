using System;
using System.Reflection;
using System.Threading;

namespace PlanServerService.Hook
{
    /// <summary>
    /// ����֪ͨ�İ�����
    /// </summary>
    public static class HookHelper
    {
        /// <summary>
        /// ���ݲ������ͣ�����֪ͨ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void DoHook(Enum type, string msg)
        {
            var hook = GetAttribute<BaseHook>(type);
            if (hook != null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => hook.Hook(msg), null);
            }
        }

        /// <summary>
        /// ��ö�ٶ����ϻ�ȡָ����Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        static T GetAttribute<T>(Enum enumObj) where T : Attribute, IHook
        {
            Type type = enumObj.GetType();
            Attribute attr = null;
            try
            {
                // ��ȡ��Ӧ��ö����
                FieldInfo field = type.GetField(Enum.GetName(type, enumObj));
                var arr = field.GetCustomAttributes(typeof(T), false);
                if (arr.Length > 0)
                    attr = (Attribute) arr[0];
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return (T) attr;
        }
    }
}
