using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CreateCommerchialRequest
{
    class CommonProperty
    {
        public static string DBServer { get; set; }
        public static string DBBase { get; set; }

        static CommonProperty()
        {

        }

        public static void LoadDataAppConfig()
        {
            DBServer = ConfigurationSettings.AppSettings.Get("DBServer");
            DBBase = ConfigurationSettings.AppSettings.Get("DBBase");
        }

    }
}