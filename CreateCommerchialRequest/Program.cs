using AddEASSpace;
using CreateCommerchialRequest.Helpers;
using CreateCommerchialRequest.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateCommerchialRequest
{
    static class Program
    {
        public static string srvname = "";
        public static string conn_string = "";
        public static string mainConnection = "";
        public static string mainDBName = "";

        //public static WStoEAS ws;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //закомментить при сборке
            #region ParametersForDebug
            args = new string[4];

            //args[0] = "EasZakTov";
            //args[1] = "ddfi3)es";
            //args[2] = "real";

            args[0] = "DmitrievaUV";
            args[1] = "DmitrievaUV8798";
            args[2] = "clon";


            //args[0] = "muhinan";
            //args[1] = "muhinan1017";
            //args[2] = "test";   

            //args[0] = "ermilovaiv";
            //args[1] = "ermilova120";
            //args[2] = "test";

            //args[0] = "PodvorotnikovMS";
            //args[1] = "PodvorotnikovMS3453";
            //args[2] = "real";

            //args[0] = "ZolotuhinAS";
            //args[1] = "ZolotuhinAS490";
            //args[2] = "real";

            //args[0] = "natali";
            //args[1] = "natali123";
            //args[2] = "test";

            //args[0] = "selyutinvd";
            //args[1] = "vd325";
            //args[2] = "test";
            #endregion
            //
            //CommonProperty.LoadDataAppConfig();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CultureInfoDefault.CheckAndSetCultureInfo();

            if (args.Length > 0)
            {
                try
                {

                    var arg1 = args[0] == null ? "" : args[0];
                    var arg2 = args[1] == null ? "" : args[1];
                    var arg3 = args[2] == null ? "" : args[2];
                    //WStoEAS wse = new WStoEAS();
                     
                    
                    //MessageBox.Show( (wse.rc_checkwork()).ToString()); //WStoEAS.GetData() + " " +

                    //UniLogger.WriteLog("", 1, $"{arg1} {arg2} {arg3}");

                    if (arg3.ToLower() == "clon" || arg3.ToLower() == "test")
                    {
                        srvname = @"DBSRV\DBSRV";
                        conn_string = $@"Server={srvname};Database={arg3};Integrated Security=SSPI;Connect Timeout=600";
                        //MailSettings.email = "dorogovtsevvv@arkona36.ru";
                        //MailSettings.password = "SZmsoRE6";
                        //MailSettings.email = "DmitrievaUV@arkona36.ru";
                        //MailSettings.password = "DmI2020";
                        MailSettings.email = "commerceoffer@arkona36.ru";
                        MailSettings.password = "Ghtlkju01)";



                    }

                    if (arg3 == "real")
                    {
                        srvname = @"DBSRV2";
                        conn_string = $@"Server={srvname};Database={arg3};Integrated Security=SSPI;Connect Timeout=600";
                        MailSettings.email = "commerceoffer@arkona36.ru";
                        MailSettings.password = "Ghtlkju01)";
                    }

                    

                    mainConnection = conn_string;
                    mainDBName = arg3;

                    if (User.LoginUser(arg1.ToLower(), arg2.ToLower()))
                    {
                        var idpost = User.GetPostByUserId(User.Current.IdUser);
                        var isdeveloper = User.InRole(User.Current.IdUser, "Developers");
                        var isRTK = User.InRole(User.Current.IdUser, "SelectSupplier");
                        var isChief = User.InRole(User.Current.IdUser, "OptChiefBuyDepartment");

                        try
                        {
                            WStoEAS wse = new WStoEAS();
                                                        

                            string response = wse.rc_createorderbuyZNP(User.CurrentUserId, "854", "100", "100");

                            /* 
                             <idrowzkp/> - код позиции ЗКП
                             <kol/> - кол-во
                             <price/> - цена
                             */
                            //MessageBox.Show(response.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        if (isdeveloper || isRTK || isChief) //девелопер или РТК или НОП
                        {
                            Application.Run(new fmMain());
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно прав доступа для запуска!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким логином или паролем не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    UniLogger.WriteLog("", 3, ex.Message);
                    
                }
                
            }
            else
            {
                MessageBox.Show("Неверные параметры запуска приложения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
             
            GC.Collect();
            Process.GetCurrentProcess().Kill();

        }
    }

    internal static class UniLogger
    {
        private static BlockingCollection<string> _blockingCollection;
        private static string _filename = Directory.GetCurrentDirectory() + @"\log\log" + DateTime.Now.ToString("dd MM yy HH mm ss") + ".txt";
        //private static string _filename = @"c:\inetpub\wwwroot\log\log" + DateTime.Now.ToString("dd MM yy HH mm ss") + ".txt";
        private static Task _task;

        static UniLogger()
        {
            _blockingCollection = new BlockingCollection<string>();

            _task = Task.Factory.StartNew(() =>
            {
                using (var streamWriter = new StreamWriter(_filename, true, Encoding.UTF8))
                {
                    streamWriter.AutoFlush = true;

                    foreach (var s in _blockingCollection.GetConsumingEnumerable())
                        streamWriter.WriteLine(s);
                }
            },
            TaskCreationOptions.LongRunning);
        }

        public static void WriteLog(string action, int errorCode, string errorDescription)
        {
            //_blockingCollection.Add($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")} действие: {action}, код: {errorCode.ToString()}, описание: { errorDiscription} ");
            _blockingCollection.Add(@"["  + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "]" + "действие: " + action  + "тип: " + errorCode.ToString() + " сообщение: " + errorDescription);
             
        }

        public static void Flush()
        {
            _blockingCollection.CompleteAdding();
            _task.Wait();
        }
    }
}
