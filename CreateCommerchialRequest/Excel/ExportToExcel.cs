using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClosedXML.Excel;
using CreateCommerchialRequest.Database;
using CreateCommerchialRequest.Models;
using CreateCommerchialRequest.Excel;
using System.Windows.Forms; 
using System.Data;


namespace CreateCommerchialRequest
{
    public class ExportToExcel
    {
        //public static string attachpath = "";

        //public void CreateExcelZKP(IEnumerable<SKU> SKUParams)
        //{
        //    try
        //    {
                 
        //        var workbook = new XLWorkbook();
        //        workbook.AddWorksheet("Запрос на КП");
        //        var ws = workbook.Worksheet("Запрос на КП");
                 
        //        int row = 1;
        //        ws.Column(1).Width = 30;
        //        ws.Column(2).Width = 30;
        //        ws.Column(3).Width = 70;
        //        ws.Column(4).Width = 19;

        //        ws.Range("A1", "D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
 
        //        ws.Cell("A" + row.ToString()).Style.Font.Bold = true;
        //        ws.Cell("B" + row.ToString()).Style.Font.Bold = true;
        //        ws.Cell("C" + row.ToString()).Style.Font.Bold = true;
        //        ws.Cell("D" + row.ToString()).Style.Font.Bold = true;     
                           
        //        ws.Cell("A" + row.ToString()).Style.Alignment.SetWrapText();
        //        ws.Cell("B" + row.ToString()).Style.Alignment.SetWrapText();
        //        ws.Cell("C" + row.ToString()).Style.Alignment.SetWrapText();
        //        ws.Cell("D" + row.ToString()).Style.Alignment.SetWrapText();

        //        ws.Cell("A" + row.ToString()).Value = "Бренд";
        //        ws.Cell("B" + row.ToString()).Value = "Артикул";
        //        ws.Cell("C" + row.ToString()).Value = "Наименование";
        //        ws.Cell("D" + row.ToString()).Value = "Количество";
        //        row++;
        //        foreach (var elem in SKUParams)
        //        {

        //            ws.Cell("A" + row.ToString()).Value = elem.tm_name;
        //            ws.Cell("B" + row.ToString()).Value = elem.art;
        //            ws.Cell("C" + row.ToString()).Value = elem.ntov;
        //            ws.Cell("D" + row.ToString()).Value = elem.cnt;

        //            row++;

        //        }
                
        //        ws.Range("A2"  , "D" + row.ToString()).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //        if (File.Exists(Config.path + $"ЗКП № {SKUParams.First<SKU>().id_ZNP} - {SKUParams.First<SKU>().n_kontr} - {DateTime.Now.ToShortDateString()}.xlsx"))
        //        {
                    
        //            File.Delete(Config.path + $"ЗКП № {SKUParams.First<SKU>().id_ZNP} - {SKUParams.First<SKU>().n_kontr} - {DateTime.Now.ToShortDateString()}.xlsx");
                   
        //        }

        //        workbook.SaveAs(Config.path + $"ЗКП № {SKUParams.First<SKU>().id_ZNP} - {SKUParams.First<SKU>().n_kontr} - {DateTime.Now.ToShortDateString()}.xlsx");
        //        Config.AttachPath += Config.GetFullPath(Config.path) + $"ЗКП № {SKUParams.First<SKU>().id_ZNP} - {SKUParams.First<SKU>().n_kontr} - {DateTime.Now.ToShortDateString()}.xlsx" + ";";
        //        //Config.AttachPath = Config.AttachPath.TrimEnd(';');
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.Message);
        //    }
            
        //}




        public void ExportExcel(IEnumerable<DataRow> selectedpost)
        {
            Stopwatch startTime = Stopwatch.StartNew();
             
            Config.FileName = $"ЗКП №{selectedpost.First()["idZKP"].ToString()} - {selectedpost.First()["n_kontr"]} - {DateTime.Now.ToShortDateString()}";
            var a = selectedpost.First()["idZKP"];
            a = selectedpost.First()["n_kontr"];
           
            var exp = new Excel.CreateExcel(selectedpost);
            if (File.Exists(Config.GetFullPath(Config.path))) { File.Delete(Config.GetFullPath(Config.path)); }
 
            bool isExported = exp.StartExport();                                    
                       
            if (isExported)
            {
                UniLogger.WriteLog($@"Экспорт ЗКП по поставщику  {selectedpost.First()["id_kontr"]} успешно завершен   ", 0, elapsedTime(startTime));
                Config.AttachPath += Config.GetFullPath(Config.path)  + ";";
            }            
            else
            {
                UniLogger.WriteLog($@"Ошибка при экспорте ЗКП. Поставщик {selectedpost.First()["id_kontr"]}. ЗНП № {selectedpost.First()["id_ZNP"]}", 1, elapsedTime(startTime));
                throw new Exception($@"Ошибка при экспорте ЗКП. Поставщик {selectedpost.First()["id_kontr"]}. ЗНП № {selectedpost.First()["id_ZNP"]}");                
            }
                 
            if (Config.AttachPath != null)
            {
                Config.AttachPath = Config.AttachPath.TrimEnd(';');
            }
        }





        public static string elapsedTime(Stopwatch time)
        {
            return String.Format("{0} мин | {1} сек",
                            time.Elapsed.Minutes,
                            time.Elapsed.Seconds);
        }
    }
}
