using System;
using System.Collections.Generic;
using CreateCommerchialRequest.Models;
using CreateCommerchialRequest.Database;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Export.Xl;
using DevExpress.XtraReports.ReportGeneration;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Collections;


namespace CreateCommerchialRequest.Excel
{
    public class CreateExcel
    {
        private readonly IEnumerable<DataRow> selectedpost;

        public CreateExcel(IEnumerable<DataRow> Selectedpost)
        {
            selectedpost = Selectedpost;
        }

        public bool StartExport()
        {
            try
            {
                //if (selectedpost.Count<SKU>() < 1)
                //{ 
                //    throw new Exception("Нет SKU для отправки ЗКП");
                //}

                // Create an exporter instance.
                IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

                var path = Config.GetFullPath(Config.path); ;
                  
                // Проверка есть ли файл с таким названием

                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {

                    // Create a new document and begin to write it to the specified stream.
                    using (IXlDocument document = exporter.CreateDocument(stream))
                    {
                        // Add a new worksheet to the document.
                        using (IXlSheet sheet = document.CreateSheet())
                        {
                            // Specify the worksheet name.
                            sheet.Name = "Запрос на КП";

                            // Create the first column and set its width.
                            using (IXlColumn column = sheet.CreateColumn()) // Бренд 
                            {
                                column.WidthInPixels = 100;
                            }

                            // Create the second column and set its width.
                            using (IXlColumn column = sheet.CreateColumn()) // Артикул
                            {
                                column.WidthInPixels = 100;
                            }

                            // Create the third column and set the specific number format for its cells.
                            using (IXlColumn column = sheet.CreateColumn()) // Наименование
                            {
                                column.WidthInPixels = 400;
                                column.Formatting = new XlCellFormatting();
                                //column.Formatting.NumberFormat = @"_([$$-409]* #,##0.00_);_([$$-409]* \(#,##0.00\);_([$$-409]* ""-""??_);_(@_)";
                            }

                            using (IXlColumn column = sheet.CreateColumn())  // Количество 
                            {
                                column.WidthInPixels = 100;
                            }

                            using (IXlColumn column = sheet.CreateColumn())  // Количество поставщика
                            {
                                column.WidthInPixels = 200;
                            }

                            using (IXlColumn column = sheet.CreateColumn())  // Цена поставщика
                            {
                                column.WidthInPixels = 200;
                            }

                            // Specify cell font attributes.
                            XlCellFormatting cellFormatting = new XlCellFormatting();
                            cellFormatting.Font = new XlFont();
                            cellFormatting.Font.Name = "Arial";
                            cellFormatting.Font.Size = 10;
                            cellFormatting.Font.SchemeStyle = XlFontSchemeStyles.None;

                            // Specify formatting settings for the header row.
                            XlCellFormatting headerRowFormatting = new XlCellFormatting();
                            headerRowFormatting.CopyFrom(cellFormatting);
                            headerRowFormatting.Font.Bold = true;
                            headerRowFormatting.Font.Color = XlColor.DefaultForeground; //XlColor.FromTheme(XlThemeColor.Light1, 0.0);
                            headerRowFormatting.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent4, 0.0));

                             
                            // Create the header row.
                            using (IXlRow row = sheet.CreateRow())
                            {
                                XlCellFormatting cellHeaderFormatting = new XlCellFormatting();
                                cellFormatting.Font = new XlFont();
                                cellFormatting.Font.Name = "Arial";
                                cellFormatting.Font.Size = 10;
                                cellFormatting.Font.SchemeStyle = XlFontSchemeStyles.None;

                                row.HeightInPixels = 40;
                                row.ApplyFormatting(cellHeaderFormatting);
                                row.Formatting.Alignment = new XlCellAlignment();
                                row.Formatting.Alignment.VerticalAlignment = XlVerticalAlignment.Center;
                                row.Formatting.Alignment.HorizontalAlignment = XlHorizontalAlignment.Center;


                                foreach (var nameColumn in Config.NameCols)
                                {
                                    using (IXlCell cell = row.CreateCell())
                                    {
                                        cell.Value = nameColumn;
                                        cell.ApplyFormatting(headerRowFormatting);
                                        
                                         


                                    }
                                }
 
                            }

                            foreach (DataRow elem in selectedpost)
                            {
                                using (IXlRow row = sheet.CreateRow())
                                {
                                    foreach (var valueColumn in SKU.SKUval)
                                    {
                                        using (IXlCell cell = row.CreateCell())
                                        {

                                            cell.Value = elem[valueColumn].ToString();//elem.valueColumn.ToString();
                                            cell.ApplyFormatting(cellFormatting);
  
                                        }
                                    }
                                }
                            }

                            // Enable AutoFilter for the created cell range.
                            sheet.AutoFilterRange = sheet.DataRange;

                        }
                    }
                    stream.Dispose();
                }
                 
                GC.Collect();  
                               
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                UniLogger.WriteLog($"Экспорт ЗКП в xlsx. Клиент {selectedpost.First()["id_kontr"]}", 1, ex.Message);
                return false;
            }

        }
    }
}
 

