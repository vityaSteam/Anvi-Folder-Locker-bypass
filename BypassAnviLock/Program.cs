using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Security.Principal;

namespace BypassAnviLock
{
    class Program
    {


        public static string StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return "true";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Bypass AnviLocker | vityaSteam";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
             "  ///                                     ///"+
             "\n  ///                                     ///"+
             "\n  ///            BypassAnviLocker         ///"+
             "\n  ///                                     ///"+
             "\n  ///          Created by vityaSteam      ///"+
             "\n  ///                                     ///\n\n"
                );
            bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if(isElevated == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]Запустите программу от имени администратора!");
                Console.ReadLine();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Вы готовы обойти AnviLocker?" +
                "\nЭто может привести к некорректной работе программы, и необходимости в переустановке ПО(Y/N)");
            switch (Console.ReadLine())
            {
                case "N":
                    Environment.Exit(0);
                    break;
                case "Y":
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR]Вариант не найден, использую по умолчанию(Y)!");
                    break;
            }
            try
            {
                ServiceController service = new ServiceController("AnviFPFltd");
                if (service.Status.ToString() == "Running")
                {
                    string status = StopService("AnviFPFltd", 1);
                    if (status != "true")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR]Ошибка: " + status);
                        Console.ReadLine();
                        return;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO]Служба AnviLocker остановлена!");
                    string keyName = @"SYSTEM\CurrentControlSet\Services";
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
                    {
                        if (key == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("[INFO]Служба AnviLocker не установлена!");
                            Console.ReadLine();
                            return;
                        }
                        else
                        {
                            key.DeleteSubKeyTree("AnviFPFltd");
                            Console.WriteLine("[INFO]Служба AnviLocker удалена!");
                        }
                    }
                    Console.WriteLine("[INFO]Все файлы & папки были раблокированы!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR]Служба AnviLocker не запущена!");
                }
            }catch(System.ComponentModel.Win32Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]Программа Anvi Locker не была найденна на вашем ПК!");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]Возможно вы уже использовали наш софт!");
            }
            Console.ReadLine();
        }
    }
}
