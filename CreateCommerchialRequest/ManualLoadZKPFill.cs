using CreateCommerchialRequest.Database;
using CreateCommerchialRequest.Heplers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateCommerchialRequest.Models;

using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace CreateCommerchialRequest
{
    public partial class ManualLoadZKP 
    {

        public static string fields;
        public static bool checkSKU;
        public static string directory = "c:\\";



        public static int idZKP;
        public static int cellBrand;
        public static int cellArt;
        public static int cellCountSup;
        public static int cellPriceSup;
        public static int StartRow;
        public static int EndRow; 
        

        public bool CheckValue(string idZKP= "" , string cell = "")
        {
            try
            {
                int NumZKP = 0;
                if (idZKP != "")
                {
                    if (!int.TryParse(idZKP, out NumZKP))  
                    {
                        MessageBox.Show("Введеное значение не соответствует числовому", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                         
                        return false;
                    }
                    else
                    {
                        if((Convert.ToInt32(idZKP) < 0) || (Convert.ToInt32(idZKP) == 0))
                        {
                            MessageBox.Show("Введеное числовое значение должно быть больше 0", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }
                else if (cell != "")
                {
                    char c;
                    if ((!char.TryParse(cell, out c)))
                    {
                        MessageBox.Show("Введеное значение не соответствует латинской букве", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                       
                        return false;
                    }
                    if (!char.IsLetter(Convert.ToChar(cell)))
                    {
                        MessageBox.Show("Введеное значение не соответствует латинской букве", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
 

                }
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Что-то пошло не так", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UniLogger.WriteLog("", 3, "ManualLoadZKP change TextBox.Text " + ex.Message);
                return false; 
            }

             

        } 

      




        public static string CheckParse(string ColumnVal, int flag, string ColumnValBrand = "")
        {
            string sql = "";
            if ((ColumnVal != null) && (ColumnVal != ""))
            {
                if (ColumnVal.Contains(',')) { ColumnVal = ColumnVal.Replace(',', '.'); }


                switch (flag)
                {

                    case 1:

                        sql = $@"select TOP(1) id_tov 
                                from spr_tov (nolock) 
                                inner join spr_tm (nolock) on spr_tov.id_tm = spr_tm.tm_id                                
                                where (spr_tov.id_tov_oem_short = dbo.f_replace_for_cross('{ColumnVal}') or spr_tov.id_tov_oem =  dbo.f_replace_for_cross('{ColumnVal}'))
                                    and spr_tm.tm_name = '{ColumnValBrand}'";
                        DataRow dr = DBExecute.SelectRow(sql);
                        if (dr != null)
                        {
                            return ColumnVal;
                        }
                        else
                        {
                            char[] s = new char[] { '/', '\'', '*', '-', '+', '#', '№', '$', '%', '(', ')', '!', '@', '"', '[', ']', '{', '}', '&', '?', '^' };
                            foreach (char c in s) { if (ColumnVal.Length == 0) return ""; else { ColumnVal = ColumnVal.Replace(c.ToString(), ""); ColumnValBrand = ColumnValBrand.Replace(c.ToString(), ""); } }

                            sql = $@"select TOP(1) id_tov 
                                    from spr_tov (nolock) 
                                    inner join spr_tm (nolock) on spr_tov.id_tm = spr_tm.tm_id                                
                                    where (spr_tov.id_tov_oem_short = '{ColumnVal}' or spr_tov.id_tov_oem =  '{ColumnVal}')
                                        and spr_tm.tm_name = '{ColumnValBrand}'";
                            dr = DBExecute.SelectRow(sql);
                            if (dr != null)
                            {
                                return ColumnVal;
                            }
                            else
                            {
                                return "";
                            }


                        }
                    // Артикул

                    case 2:
                        {
                            double count = 0;
                            int countprice = 0;
                            int i = 0;
                            List<char> nondigit = new List<char>();


                            if (ColumnVal.Contains("р")) ColumnVal.Remove(ColumnVal.IndexOf("р"));
                            while (i < ColumnVal.Length)
                            {
                                if ((!char.IsDigit(ColumnVal[i])) && (Convert.ToChar(ColumnVal[i]) != '.')) { nondigit.Add(Convert.ToChar(ColumnVal[i])); }
                                i++;
                            }

                            foreach (var c in nondigit) { if (ColumnVal.Length == 0) return ""; else { ColumnVal = ColumnVal.Replace(c.ToString(), ""); } }

                            if (int.TryParse(ColumnVal, out countprice) || double.TryParse(ColumnVal, out count)) { return ColumnVal; }
                            else
                            {
                                return "";
                            }
                        }
                    default: return "";
                }

            }
            else { return ""; }
        }



        public void GetDataPost(bool firstload = false)
        {
            string sql = $@"Select distinct sKontrTitle.idKontrTitle as idKontr, 
                                sKontrTitle.nKontrTitle  as Post                                                                   
                               from sKontrTitle (nolock) 
                               INNER JOIN rKontrTitleTM (nolock) ON sKontrTitle.idKontrTitle = rKontrTitleTM.idKontrTitle
                               INNER JOIN rKontrTitleKontr (nolock) on rKontrTitleKontr.idKontrTitle = rKontrTitleTM.idKontrTitle
                               where  sKontrTitle.idTypeKontrTitle = 2 and
                                      rKontrTitleKontr.fActual = 1  and      
                                      sKontrTitle.idtypeeconomy <> 1    
                                ";
            DataTable dt = new DataTable();
 


            dt = DBExecute.SelectTable(sql);
            DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "--Выберите значение--";
            dt.Rows.InsertAt(row, 0);
             
            Postcmb.DataSource = dt;
            Postcmb.DisplayMember = "Post";             
            Postcmb.ValueMember = "idKontr";
            

        }






        public void GetZKPKontrSet()
        {
            object idKontr = 0;
            try
            {
                idKontr = (Postcmb.SelectedValue as DataRowView).Row.ItemArray[0];
            }
            catch
            {
                 idKontr =  Postcmb.SelectedValue;
            }
            
            if(Convert.ToInt32(idKontr) == 0) { return; }
            string Post = Postcmb.Text;
            string sql = $@"Select idRow as idKontrSet, 
                                   nKontrSet 
                                   
                            from ZKPKontrSet
                            where idKontr = {idKontr}
                                ";
             
            DataTable dt = DBExecute.SelectTable(sql);
            if((dt == null) || (dt.Rows.Count == 0))  
            {
                sql = $@"insert into ZKPKontrSet (idKontr , nKontrSet) values ({idKontr}, '{Post} - Настройка КП')";
                DBExecute.ExecuteQuery(sql);
                GetZKPKontrSet();
            }
            else
            {
                Setcmb.DataSource = dt;
                Setcmb.DisplayMember = "nKontrSet";
                Setcmb.ValueMember = "idKontrSet";

                int idKontrSet = Convert.ToInt32(Setcmb.SelectedValue);//as DataRowView).Row.ItemArray[0].ToString());


                 sql = $@"Select  
                                   ColumnBrand,
                                   ColumnArt, 
                                   ColumnKolSup,
                                   ColumnPriceSup  
                            from ZKPKontrSet
                            where idKontr = {idKontr} and idRow = {idKontrSet}
                                ";

                DataRow row = DBExecute.SelectRow(sql);

 
                string ColumnBrand = row.ItemArray[0].ToString();
                string ColumnArt = row.ItemArray[1].ToString();
                string ColumnKolSup = row.ItemArray[2].ToString();
                string ColumnPriceSup = row.ItemArray[3].ToString();

                cellBrandtb.Text = ColumnBrand;
                cellArttb.Text = ColumnArt;
                cellCountSuptb.Text = ColumnKolSup;
                cellPriceSuptb.Text = ColumnPriceSup;


            }
        }

        public void UpdateSetKontr(bool b = false)
        {
            string sql;

            if(b)
            {
                sql = $@"Update ZKPKontrSet 
                             set ColumnBrand = '{cellBrandtb.Text}' , ColumnArt = '{cellArttb.Text}' , ColumnKolSup = '{cellCountSuptb.Text}' , ColumnPriceSup = '{cellPriceSuptb.Text}'
                             where idKontr = {Postcmb.SelectedValue.ToString()}";                 
            }
            else
            {
                sql = $@"Update ZKPKontrSet 
                             set ColumnBrand = null , ColumnArt = null , ColumnKolSup = null , ColumnPriceSup = null
                             where idKontr = {Postcmb.SelectedValue.ToString()}";
            }
            DBExecute.ExecuteQuery(sql);
        }



        public string FindKP()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = directory;
                openFileDialog.Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    Reset();
                    string filePath = openFileDialog.FileName;
                    string s = @"\";
                    string filename = openFileDialog.FileName.Split(Convert.ToChar(s)).Last();
                    directory = openFileDialog.FileName.Substring(0,openFileDialog.FileName.Length - filename.Length);


                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();

                    DataTableCollection dt = FileReader.Read(filePath);
                    //using (StreamReader reader = new StreamReader(filePath))
                    //{
                         
                    //}
                    gcKP.DataSource = dt[0];
                    if(gvKP.RowCount > 0)
                    {
                        
                        DataRow row =   gvKP.GetDataRow(1);
                        for (int i = 0; i < gvKP.Columns.Count; i++)
                        {
                            gvKP.Columns[i].Caption = row[i].ToString() + " " + "'" + GetAlphaBetForNum(i).ToString().ToUpper()  + "'";
                            gvKP.Columns[i].AppearanceHeader.Font = new System.Drawing.Font("Arial" , 10 ,  System.Drawing.FontStyle.Bold);
                        }                                                                             
                        gvKP.Appearance.Row.Font = new System.Drawing.Font("Arial", 10);
                        row.Delete();
                            
                            
                    }
                    return filePath;
                }
                return "";
            }
        }

        public void Reset()
        {
            ChooseFileDirtb.Text = "";
            gcKP.DataSource = null;
            int ColumnsCount = gvKP.Columns.Count;
 
            for (int i = 0; i < ColumnsCount - 1; i++)
            {
                     
                gvKP.Columns[i].Caption = "";
            }
 
         

            for(int i = ColumnsCount - 1; i > -1; i--)
            {
                gvKP.Columns.Remove(gvKP.Columns[i]);
            }
                 
            
             

        }

        


        public void LoadKP(string filepath)
        {
            foreach (TextBox c in this.panelControlLoadZKP.Controls.OfType<TextBox>())
            {
                if ((c.Text == "") || (c.Name != StartRowtb.Name))
                {
                    MessageBox.Show("Заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }                
            }
            try
            {
                idZKP = Convert.ToInt32(idZKPtb.Text);
                cellBrand = Convert.ToInt32(GetNumAlphabetforcell(Convert.ToChar(cellBrandtb.Text)));
                cellArt = Convert.ToInt32(GetNumAlphabetforcell(Convert.ToChar(cellArttb.Text)));
                cellCountSup = Convert.ToInt32(GetNumAlphabetforcell(Convert.ToChar(cellCountSuptb.Text)));
                cellPriceSup = Convert.ToInt32(GetNumAlphabetforcell(Convert.ToChar(cellPriceSuptb.Text)));
                StartRow = Convert.ToInt32(StartRowtb.Text);
                //EndRow = Convert.ToInt32(EndRowtb.Text);

                //LoadKP(ChooseFileDirtb.Text);
                //

                DataTable dtParceFile = gcKP.DataSource as DataTable;
                DataTable GetZKP = FillDataTable(dtParceFile, idZKP);
                ExportData(GetZKP, idZKP);
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UniLogger.WriteLog("", 3, "ManualLoadZKP LoadZKPbtn " + ex.Message);
            }
        }

        public static DataTableCollection ReadFileAllFormat(string filePath)   //ParserSettings setting
        {
            //UniLogger.WriteLog("", 0, "Флаг ручной загрузки: " + setting.fHardLoad.ToString());
            try
            {
                var extension = filePath.Split('.').Last();

                var result = FileReader.Read(filePath);
                if (result == null)
                {
                    UniLogger.WriteLog("", 3, "Файл имеет неизвестное расширение " + extension);
                    return null;
                }
                return result;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Необходимо закрыть выбранный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
             
        }



        public int GetNumAlphabetforcell(char cell)
        {
            string str = @"AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            return str.IndexOf(cell) / 2;
        }


        public char GetAlphaBetForNum(int num)
        {
            string str = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return Convert.ToChar(str.Substring(num, 1));  
        }


        public static DataTable FillDataTable(DataTable dt, int idZKP)
        {
            if(dt == null) 
             {
                return null;

            }

            DataTable dtGetZKP = new DataTable();
            dtGetZKP.Columns.Add("idTovOEM");
            //dtGetZKP.Columns.Add("needcount");
            dtGetZKP.Columns.Add("countSup");
            dtGetZKP.Columns.Add("priceSup");
            dtGetZKP.Columns.Add("Brand");



            try
            {
                int checktemp = 0;
                //foreach (DataTable dt in dtcollection)
                //{
                    checkSKU = false;
                    var a = dt.Rows.Count;
                    for (int i = StartRow - 1; i < dt.Rows.Count - 1; i++)
                    {
                        

                        string[] s = new string[4];
                        s[0] = CheckParse(dt.Rows[i][cellArt].ToString(), 1, dt.Rows[i][cellBrand].ToString());
                        //s[1] = CheckParse(dt.Rows[i][3].ToString(), 2);
                        s[1] = CheckParse(dt.Rows[i][cellCountSup].ToString(), 2);
                        s[2] = CheckParse(dt.Rows[i][cellPriceSup].ToString(), 2);
                        s[3] = dt.Rows[i][cellBrand].ToString();




                        checktemp += CheckColumn(s[0], s[0], s[3], "Артикул", idZKP, 2);
                        //checktemp += CheckColumn(s[1], "Количество");
                        checktemp += CheckColumn(s[1], s[0], s[3], "Кол-во поставщика", idZKP, 3);
                        checktemp += CheckColumn(s[2], s[0], s[3], "Цена поставщика", idZKP, 4);
                        checktemp += CheckColumn(s[3], s[0], s[3], "Бренд", idZKP, -1);


                        if (checktemp == s.Length)
                        {

                            DataRow row = dtGetZKP.NewRow();
                            row["idTovOEM"] = s[0];
                            //row["needcount"] = s[1];
                            row["countSup"] = s[1];
                            row["priceSup"] = s[2];
                            row["Brand"] = s[3];
                            dtGetZKP.Rows.Add(row);
                        }
                        else
                        {

                            checkSKU = true;
                            fields = fields.TrimEnd(',');
                            UniLogger.WriteLog("", 0, $"Некорректное заполнение для SKU Бренд - Артикул: {s[0].ToString() + " " + s[3].ToString()}, некорректно заполнены поле(я): {fields}. SKU не была добавлена в обработку КП");
                        }
                        fields = "";
                        checktemp = 0;

                    }
                    //if(allskusuccess)
                    //{
                    //    string sql = $@"Insert into ZKPExchangeLog (idZKP , idStatusCheckLog) values({idZKP}, 0)";
                    //    DBExecutor.ExecuteQuery(sql);
                    //}

                
                //AutoClosingMessageBox.Show("Успешно обработано строк: " + dtGetZKP.Rows.Count.ToString(), "Внимание", 3000);
                //AutoClosingMessageBox.Show("Готово", "", 2000);
                //string sql = $@"Insert into ZKPExchangeLog (idZKP , idStatusCheckLog , idSup , idUser) values({idZKP}, 8, {0}, {User.CurrentUserId} )";
                //DBExecute.ExecuteQuery(sql);
                UniLogger.WriteLog("", 0, "Ручной парсинг, успешно обработано строк: " + dtGetZKP.Rows.Count.ToString());
                return dtGetZKP;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
               // AutoClosingMessageBox.Show("Успешно обработано строк: " + dtGetZKP.Rows.Count.ToString() + ",обнаружены пустые строки", "Внимание", 3000);
                //string sql = $@"Insert into ZKPExchangeLog (idZKP , idStatusCheckLog , idSup , idUser) values({idZKP}, 8, {idSup}, {User.CurrentUserId} )";
                //DBExecute.ExecuteQuery(sql);
                UniLogger.WriteLog("", 3, "Ручной парсинг, успешно обработано строк: " + dtGetZKP.Rows.Count.ToString() + ", но с пустыми строками");
               // AutoClosingMessageBox.Show("Готово", "", 2000);
                return dtGetZKP;
                
            }
            
        }

        public static int CheckColumn(string StringVal, string idtovoem, string brand, string fieldname, int idZKP, int idStatusCheckLog)
        {
            if (StringVal == "")
            {
                if (idStatusCheckLog != -1)
                {
                    //string sql = $@"Insert into ZKPExchangeLog (idZKP , idTovOem , TmName, idStatusCheckLog, idSup) values({idZKP}, '{idtovoem}' , '{brand}', {idStatusCheckLog} , {idSup})";
                    //DBExecute.ExecuteQuery(sql);
                }
                fields += fieldname + ","; // для Unilogger
                return 0;
            }
            else
            {
                return 1;
            }
        }

        

        public static void ExportData(DataTable dt, int idZKP)
        {
            if ((dt == null) || (dt.Rows.Count == 0)) { return; }
            var vdatatable = dt;
            //string sql = $@"EXEC [dbo].[up_InsertReceiveZKP] {vdatatable} , {idZKP}";
            //DBExecutor.ExecuteQuery(sql);
            //using (var command = new SqlCommand("InsertTable") { CommandType = CommandType.StoredProcedure })
            //{
            //    var dt = new DataTable(); //create your own data table
            //    command.Parameters.Add(new SqlParameter("@myTableType", dt));
            //    SqlHelper.Exec(command);
            //}
            SqlParameter par = new SqlParameter("GetZKPIn", SqlDbType.Structured);
            par.Value = dt;
            par.TypeName = "GetZKP";
            SqlParameter par2 = new SqlParameter("idZKP", SqlDbType.Int);
            par2.Value = idZKP;
            int a = dt.Rows.Count;
            DBExecute.ExecuteProcedure("up_InsertReceiveZKP", par, par2);
            //if (!checkSKU) { string sql = $@"Insert into ZKPExchangeLog (idZKP , idStatusCheckLog , idSup) values({idZKP}, 0 , {idSup})"; DBExecutor.ExecuteQuery(sql); }
        }








    }
}

