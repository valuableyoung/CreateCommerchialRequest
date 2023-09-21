using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateCommerchialRequest.Models
{
    public static class FocusValue
    {
        public static int id_tov { get; set; }
        public static int id_tm { get; set; }
        public static int id_ZNP { get; set; }
        public static int rowHandleUpRequest { get; set; }
        public static int rowHandleSKU { get; set; }
        public static int rowHandleDeliver{ get; set; }

        public static string strkontrmailnotdelivery { get; set; }
        public static string OldPrice { get; set; }
        public static string OldKol { get; set; }

    }
}
