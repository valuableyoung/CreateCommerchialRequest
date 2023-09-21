using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateCommerchialRequest.Models;
using System.IO;

namespace CreateCommerchialRequest.Excel
{
    public static class Config
    {
         

        public static string FileName { get; set; }

        public static string AttachPath { get; set; }

        public static string path = @"U:\Дмитриева\";

        private static readonly string extFormat = ".xlsx";
         

        public static string GetFullPath(string path)
        {
            path = path + @"\ZKP";
            Directory.CreateDirectory(path);
            return path + @"\" + $@"{Config.FileName}" + extFormat;
        }


        public static readonly List<string> NameCols = new List<string>()
        {
            "Бренд",
            "Артикул",
            "Наименование",
            "Кол-во",
            "Кол-во поставщика",
            "Цена поставщика" 
        };

        public static readonly List<string> NameColsDataTable = new List<string>()
        {
            "tm_name",
            "art",
            "n_tov",
            "count"
        };

    }
}
