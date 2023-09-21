using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateCommerchialRequest.Database;
using CreateCommerchialRequest.Models;

namespace CreateCommerchialRequest.Models
{
    public class SKU
    {
        public string id_tov { get; set; }
        public string id_tm { get; set; }
        public string tm_name { get; set; }
        public string art { get; set; }
        public string ntov { get; set; }
        public string cnt { get; set; }
        public string price { get;set; }
        public string id_kontr { get; set; }
        public string n_kontr { get; set; }
        public string id_ZNP { get; set; }
        public string id_ZKP { get; set; }

        public string kontrCnt { get; set; }
        public string kontrPrice { get; set; }

        public SKU(string Id_tov, string Id_tm , string Tm_name , string Art,  string NTov , string Cnt , string Price , string Id_kontr , string N_kontr, string Id_ZNP , string Id_ZKP)
        {
            id_tov = Id_tov;
            id_tm = Id_tm;
            tm_name = Tm_name;
            art = Art;
            ntov = NTov;
            cnt = Cnt;
            price = Price;
            id_kontr = Id_kontr;
            n_kontr = N_kontr;
            id_ZNP = Id_ZNP;
            id_ZKP = Id_ZKP;
        }

        public static List<string> SKUval = new List<string>()
        {

            "Brand",
            "art"  ,
            "ntov" ,
            "cnt"
        };

 

    }
}
