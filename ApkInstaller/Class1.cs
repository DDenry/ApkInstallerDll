using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DDenry
{
    public class ApkInstaller
    {
        public const int NO_JAVA_ENVIRONMENT = -1;
        public const int NO_ADB_PATH = -2;
        public const int NO_APK_FILE = -3;
        public const int NO_DEVICE = 0;
        public const int INSTALL_START = 1;

        private List<String> devices_list = new List<string>();

        protected Boolean toAll = false;

        public Boolean TO_ALL {
            get {
                return toAll;
            }
        }


        protected int devicesCount = 0;

        public int DEVICES_COUNT
        {
            get
            {
                return this.devicesCount;
            }
        }
        //
        public DataReceivedEventHandler HandleOutput;

        public int Install2Phone(String adbPath, String apkPath, Boolean toAll = false)
        {
            //判断是否有Java环境
            if (JavaEnvironment())
            {
                //判断adb路径是否正确
                if (!String.IsNullOrEmpty(adbPath))
                {
                    FileInfo fileInfo = new FileInfo(adbPath);
                    if (fileInfo.Name.Equals("adb.exe"))
                    {
                        if (!File.Exists(apkPath)) return NO_APK_FILE;
                        else
                        {
                            FileInfo _fileInfo = new FileInfo(apkPath);
                            if (_fileInfo.Extension.Equals(".apk"))
                            {
                                adbPath = fileInfo.DirectoryName;
                                //
                                CheckAdvices(adbPath);
                                //
                                if (DEVICES_COUNT > 0)
                                {
                                    //
                                    this.toAll = toAll;
                                    //
                                    Process p = new Process();
                                    p.StartInfo.FileName = "cmd.exe";
                                    p.StartInfo.UseShellExecute = false;
                                    p.StartInfo.RedirectStandardInput = true;
                                    p.StartInfo.RedirectStandardOutput = true;
                                    p.StartInfo.RedirectStandardError = true;
                                    p.StartInfo.CreateNoWindow = true;
                                    //
                                    p.OutputDataReceived += new DataReceivedEventHandler(HandleOutput);
                                    p.ErrorDataReceived += new DataReceivedEventHandler(HandleOutput);
                                    //
                                    p.Start();
                                    //
                                    p.BeginOutputReadLine();
                                    p.BeginErrorReadLine();

                                    adbPath = fileInfo.DirectoryName;
                                    String cmd_InstallApkToPhone = "adb install -d " + apkPath;

                                    //向cmd窗口发送输入信息
                                    p.StandardInput.WriteLine("cd /");
                                    p.StandardInput.WriteLine("cd " + adbPath);
                                    //p.StandardInput.WriteLine("adb devices");

                                    //如果选择所有设备
                                    if (toAll)
                                    {
                                        for (int i = 0; i < DEVICES_COUNT; i++)
                                        {
                                            cmd_InstallApkToPhone = "adb -s " + devices_list[i] + " install " + apkPath; ;
                                            p.StandardInput.WriteLine(cmd_InstallApkToPhone);
                                        }
                                    }//默认只安装至第一个设备
                                    else {
                                        cmd_InstallApkToPhone = "adb -s " + devices_list[0] + " install " + apkPath; ;
                                        p.StandardInput.WriteLine(cmd_InstallApkToPhone);
                                    }
                                    p.StandardInput.WriteLine("&exit");

                                    return INSTALL_START;
                                }
                                else return NO_DEVICE;
                            }
                            else return NO_APK_FILE;
                        }
                    }
                    else return NO_ADB_PATH;
                }
                else return NO_ADB_PATH;
            }
            else return NO_JAVA_ENVIRONMENT;
        }
        //
        private void CheckAdvices(String adbPath)
        {
            //
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            //
            p.Start();

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine("cd /");
            p.StandardInput.WriteLine("cd " + adbPath);
            p.StandardInput.WriteLine("adb devices&exit");
            //
            String outLine = "";
            while (!p.StandardOutput.EndOfStream)
            {
                Console.WriteLine(outLine = p.StandardOutput.ReadLine());
                if (!String.IsNullOrEmpty(outLine))
                {
                    if (outLine.Contains("	device"))
                    {
                        devices_list.Add(outLine.Replace("device", "").Trim());
                        Console.WriteLine("检测到USB连接的android设备," + devices_list[devicesCount]);
                        devicesCount++;
                    }
                }
            }

            p.WaitForExit();
            //
            p.Close();
        }
        //
        private Boolean JavaEnvironment()
        {
            Boolean haveJava = false;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine("java -version&exit");
            p.StandardInput.AutoFlush = true;

            String str = "";
            while (!p.StandardError.EndOfStream)
            {
                Console.WriteLine(str = p.StandardError.ReadLine());
                //
                if (str.Contains("java version"))
                    haveJava = true;
            }

            p.WaitForExit();
            p.Close();
            return haveJava;
        }
    }
}