﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using PlanServerService;

namespace PlanServer
{
    class Program
    {
        static void Main()
        {
            // 刚刚测试发现，如果error.exe直接抛出异常，比如 int.Parse("abc");
            // 能在任务管理器里看到2个进程，持续几秒后2个都退出了
            // Process.Start(@"F:\error.exe", "");
            // return;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Process process = Process.GetCurrentProcess();
            string msg = string.Format("启动目录:{0}\r\n启动文件:{1}\r\n\r\n 程序启动……",
                AppDomain.CurrentDomain.BaseDirectory,
                process.MainModule.FileName);

            if (IsRunning(process))
            {
                msg += "\r\n应用程序已经在运行中。";
                LogHelper.WriteCustom(msg, @"start\", false);
                Thread.Sleep(1000);
                Environment.Exit(1);
            }

            Console.WriteLine(msg);
            LogHelper.WriteCustom(msg, @"start\", false);

            // 轮询数据库，处理任务的线程
            new Thread(TaskAutoRunService.Run) { IsBackground = true }.Start();

            if (Common.GetBoolean("enableSocketListen"))
            {
                // 端口监听，处理管理程序的进程
                var method = new SocketServer.OperationDelegate(TaskService.ServerOperation);
                new Thread(SocketServer.ListeningBySocket).Start(method);

                msg = " 开始监听端口：" + TaskService.ListenPort;
                Console.WriteLine(msg);
                LogHelper.WriteCustom(msg, @"start\", false);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Utils.Output("未知错误", ex);
        }

        /// <summary>
        /// 根据指定进程名和文件路径，判断程序是否已经在运行中。
        /// 通常用于程序单例运行，避免启动多个实例
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static bool IsRunning(Process current)
        {
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id &&
                    process.MainModule.FileName.Equals(current.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
