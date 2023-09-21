using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateCommerchialRequest.Database;
using CreateCommerchialRequest.Models;
using CreateCommerchialRequest.Excel;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using DevExpress.Utils;
using CreateCommerchialRequest.Holidays; 

namespace CreateCommerchialRequest
{
     
    public partial class fmMain : Form
    {
        //public Encoding Encod = Encoding.UTF8;
        ExportToExcel exc = new ExportToExcel();
        string sql;


        //public void checkSendZKP()
        //{
        //    sql = $@"Select id_ZNP 
        //            from ZNP
        //            where id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} and
        //                  ZNPstatus = 2";

        //    System.Data.DataTable dt = DBExecute.SelectTable(sql);
        //    if(dt.Rows.Count > 0)
        //    {
        //        SendZKPbtn.Enabled = true;                
        //    }
        //    else
        //    {
        //        SendZKPbtn.Enabled = false;
        //    }
        //}
        public bool  checkDaysWait()
        {
            sql = $@"select DaysWait
                            from ZKPmailparam (nolock)";
            DataRow bodysubject = DBExecute.SelectRow(sql);
            int countdays = 0;

            if ((bodysubject == null) || (bodysubject["DaysWait"].ToString() == "") || (!int.TryParse(bodysubject["DaysWait"].ToString(), out countdays)))
            {
                MessageBox.Show(@"Внимание! Не задан срок ожидания КП в окне Настройка ЗКП. Письма не отправлены поставщикам.", "Внимание", MessageBoxButtons.OK,
                               MessageBoxIcon.Exclamation);
                UniLogger.WriteLog("", 3, "Попытка отправки ЗКП: Не задан срок ожидания КП в окне Настройка ЗКП. ЗКП не отправлены поставщикам.");
                return true;
            }
            return false;
        }



        public void SendZKP()
        {
            try
            {
                FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;
                if (checkDaysWait()) { return; }

                int id_znp;                
                id_znp = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);
                sql = $@"select distinct  ZKP.idZKP
                     from ZKP (nolock)
                     inner join ZKPTov on ZKP.idZKP = ZKPTov.idZKP
                     where idZNP =  {id_znp} and SendDate is null and  ZKPTov.isApproved = 1";
                DataTable dt = DBExecute.SelectTable(sql);
                if(dt.Rows.Count == 0)
                {
                     
                    DialogResult dr = MessageBox.Show(@"Все SKU, включенные в КП, по данной заявке на пополнение уже были отправлены поставщикам!" + "\r\n" +"Вы уверены, что хотите отправить ЗКП снова?", "Внимание", MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Question);
                    if (dr == DialogResult.Cancel) {  return; }
                    else
                    {
                        sql = $@"select distinct  idSup
                             from ZKP (nolock)
                             where idZNP =  {id_znp}";
                       dt = DBExecute.SelectTable(sql);
                    }
                }
                sql = $@"select distinct  idSup
                     from ZKP (nolock)
                     inner join ZKPTov (nolock) on ZKP.idZKP = ZKPTov.idZKP
                     where idZNP =  {id_znp} and SendDate is null and  ZKPTov.isApproved = 1";
                dt = DBExecute.SelectTable(sql);


                string id_kontrs = "";
                foreach (DataRow dr in dt.Rows)
                {
                    id_kontrs += dr["idSup"].ToString() + ",";
                }
                id_kontrs = id_kontrs.TrimEnd(',');


                //string str_emails = "";
                sql = $@"select distinct idKontrTitle as id_kontr ,
                                     email as el_mail
                     from  sKontrTitle (nolock)
                     where idKontrTitle in({id_kontrs}) ";
                DataTable emails = DBExecute.SelectTable(sql);

                //foreach (DataRow email in emails.Rows)
                //{
                //    if ((email["el_mail"].ToString() == "") || (email["el_mail"].ToString() == null))
                //    {
                //        str_emails += "notfound" + ",";
                //        email["el_mail"]  = "notfound";
                //    }
                //    str_emails += email["el_mail"].ToString() + ",";
                //}
                //str_emails = str_emails.TrimEnd(',');

                 
                sql = $@"  SELECT distinct  ZNPTov.idtov as id_tov,
                            spr_tm.tm_id as id_tm,
                            spr_tm.tm_name as Brand ,
                            spr_tov.id_tov_oem as Art,  
                            spr_tov.n_tov as  Ntov, 
                            ZNPTov.Kol as Cnt,                            
                            ZKP.idSup as id_kontr ,
                            sKontrTitle.nKontrTitle as n_kontr ,
                            ZNPTov.idZNP as id_ZNP  ,
                            ZKP.idZKP  as idZKP                                                     
                           FROM ZNPTov (nolock)
                           INNER JOIN spr_tov (nolock) on ZNPTov.idtov = spr_tov.id_tov
                           INNER JOIN spr_tm (nolock) on spr_tm.tm_id = spr_tov.id_tm 
                           INNER JOIN ZKP (nolock) on ZKP.idZNP = ZNPTov.idZNP                                                         
                           INNER join ZKPTov on ZKPTov.idZKP = ZKP.idZKP  and ZNPTov.idtov = ZKPTov.idtov
                           INNER JOIN sKontrTitle on ZKP.idSup = sKontrTitle.idKontrTitle                        
                           WHERE ZNPTov.idZNP = {id_znp} and
                           ZKP.idSup  in ({id_kontrs})
                           order by ZKP.idSup";

                DataTable sku = DBExecute.SelectTable(sql);
                //List<SKU> SKUParams = new List<SKU>();
                //foreach (DataRow skurow in sku.Rows)
                //{

                //    SKUParams.Add(
                //            new SKU(skurow["id_tov"].ToString(),skurow["id_tm"].ToString(), skurow["Brand"].ToString(), skurow["Art"].ToString(), skurow["Ntov"].ToString(),
                //                    skurow["Cnt"].ToString(), skurow["price"].ToString(), skurow["id_kontr"].ToString(), skurow["n_kontr"].ToString(), skurow["id_ZNP"].ToString())
                //                 );

                //}
                 
                foreach (DataRow emailkontr in emails.Rows)
                {
                   

                    //var selectedpost = from s in SKUParams
                    //                  where s.id_kontr == emailkontr["id_kontr"].ToString()
                    //                  select s;

                    var selectedkontr = from rows in sku.AsEnumerable()
                                           where rows["id_kontr"].ToString() == emailkontr["id_kontr"].ToString()
                                           select rows;
                    if(selectedkontr == null) { return; }
                    //ce.CreateExcelZKP(selectedpost); 
                    exc.ExportExcel(selectedkontr);
                    Config.AttachPath = Config.AttachPath.TrimEnd(';');
                    //string mail = emailkontr["el_mail"].ToString();
                    Stopwatch startTime = Stopwatch.StartNew();
                    if (SendMail(emailkontr["el_mail"].ToString(), Config.AttachPath, selectedkontr))
                    {
                        UniLogger.WriteLog("", 0, $"Письмо с ЗКП по поставщику  {selectedkontr.First()["id_kontr"]} успешно отправлено за " + ExportToExcel.elapsedTime(startTime));
                        
                    }
                    else
                    {                         
                        FocusValue.strkontrmailnotdelivery += "\r\n" + selectedkontr.First()["n_kontr"];
                        UniLogger.WriteLog("", 3, $"Ошибка при отправке почты поставщику {selectedkontr.First()["n_kontr"]}");
                    } 
                    if (FocusValue.strkontrmailnotdelivery == "")
                    {
                        FocusValue.strkontrmailnotdelivery = "1";
                    }
                    checkStatus(selectedkontr, emailkontr["el_mail"].ToString());
                    Config.AttachPath = "";
                }


                 
                //checkStatus(selectedkontr, str_emails);
                // MessageBox.Show("Письма поставщикам с ЗКП отправлены");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + ex.StackTrace);                 
                UniLogger.WriteLog("", 3, $"Ошибка в SendMessage() " + ex.Message);                 
            }
            

             
        }

        public bool SendMail(string email, string Attachpaths, IEnumerable<DataRow> SelectedKontr)
        {
            sql = $@"select Subject,
                            Body , 
                            DaysWait
                            from ZKPmailparam (nolock)";
            DataRow bodysubject = DBExecute.SelectRow(sql);
            


            try
            {
                
                    //int days = 0;

                    var emailtemp = email;
                //if((email != "")) { email = "dorogovtsevvv@arkona36.ru"; } //"dorogovtsevvv@arkona36.ru"
                if (Program.srvname == @"DBSRV\DBSRV") { if (email != "") { } }// email = MailSettings.email; } }

                    
                    string elmailRTK = DBExecute.SelectScalar($@"select el_mail from spr_kontr where spr_kontr.id_kontr = {User.CurrentUserId}").ToString(); //добавляем почту закупщика в копию
                    MailAddress from = new MailAddress(MailSettings.email);
                    MailAddress to = new MailAddress(email);
                    MailAddress cc = new MailAddress ("goncharovma@arkona36.ru");// elmailRTK email    DmitrievaUV@arkona36.ru      "dorogovtsevvv@arkona36.ru"    
                MailMessage m = new MailMessage(from, to);
                    m.CC.Add(cc);//добавляем почту закупщика 
                    
                    string partsubject = bodysubject["Subject"].ToString().Contains("№") ? bodysubject["Subject"].ToString() : bodysubject["Subject"].ToString() + " №";
                    m.Subject = partsubject + SelectedKontr.First()["idZKP"].ToString() + " " + DateTime.Now.ToShortDateString() ;
                    
                    m.Body = bodysubject["Body"].ToString() + "\r\n" + "Ждем вашего ответа до " + Convert.ToDateTime(HolidaysRussia.getHolidaysForCurrentDatefromCalendar(Convert.ToInt32(SelectedKontr.First()["idZKP"]))).ToShortDateString() + "\r\n";// + "Поставщик адрес: " + emailtemp;
                    SmtpClient smtp = new SmtpClient("mail.arkona36.ru", 25);
                    smtp.Credentials = new NetworkCredential(MailSettings.email, MailSettings.password); //"SZmsoRE6"
                    smtp.EnableSsl = false;
                    var attachment = new Attachment(Attachpaths);
                    m.Attachments.Add(attachment);
                    smtp.Send(m);

                    string id_tov = "";
                    foreach (var sku in SelectedKontr)
                    {
                        id_tov += sku["id_tov"] + ",";
                    }
                    id_tov = id_tov.TrimEnd(',');
                    sql = $@"Update ZNPTov
                    set idTovStatus = 50
                    where idTov in ({id_tov}) and
                          idZNP = {SelectedKontr.First()["id_ZNP"]} and idTovStatus < 50";

                    DBExecute.ExecuteQuery(sql);


                    sql = $@"Update ZKP
                        set SendDate  = @datenow
                        where   idZKP = {SelectedKontr.First()["idZKP"]}";
                    SqlParameter spdate = new SqlParameter("@datenow", SqlDbType.DateTime);
                    spdate.Value = DateTime.Now.ToUniversalTime().ToString();

                    DBExecute.ExecuteQuery(sql, spdate);


                smtp.Dispose();
                    return true;
               
            }

            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message + ex.StackTrace);
                UniLogger.WriteLog("", 3, $"Ошибка при отправке почты " +  ex.Message);
                GC.Collect();
                return false;
            }
 
        }


        public void checkStatus(IEnumerable<DataRow> SelectedKontr, string emails = "")
        {
            string id_tov = "";
            foreach (var sku in SelectedKontr)
            {
                id_tov += sku["id_tov"] + ",";
            }
            id_tov = id_tov.TrimEnd(',');


            if ((emails == "notfound") || (emails ==null) || (emails == ""))
            {
                string s = SelectedKontr.First()["id_kontr"].ToString();


                sql = $@"Update ZNPTov
                    set idTovStatus = 40
                    from ZNPTov
                    inner join ZKP on ZKP.idZNP = ZNPTov.idZNP
                    where ZNPTov.idTov in ({id_tov}) and
                          ZKP.idZNP = {SelectedKontr.First()["id_ZNP"].ToString()} and
                          ZKP.idSup = {SelectedKontr.First()["id_kontr"].ToString()} and not exists 
                                                                                                   (select distinct idTov from ZNPTov
                                                                                                    inner join ZKP on ZKP.idZNP = ZNPTov.idZNP
                                                                                                    where ZNPTov.idTov in ({id_tov}) and
                                                                                                          ZKP.idZNP = {SelectedKontr.First()["id_ZNP"].ToString()} and                                                                                                          
                                                                                                          ZNPTov.idTovStatus > 40)";

                DBExecute.ExecuteQuery(sql);

                sql = $@"Update ZNPTov
                    set idTovStatus = 40
                    where idTov in ({id_tov}) and
                          idZNP = {SelectedKontr.First()["id_ZNP"].ToString()} and
                          idTov not in (
                                 select ZKPTov.idTov
                                 from ZKPTov    
                                 inner join ZKP on ZKPTov.idZKP = ZKP.idZKP 
                                 inner join ZNPTov on ZNPTov.idZNP = ZKP.idZNP                                 
                                 where ZKPTov.idTov in ({id_tov}) and
                                       ZKP.idZNP = {SelectedKontr.First()["id_ZNP"].ToString()} and
                                       ZNPTov.idTovStatus > 40
                                group by ZKPTov.idtov
                                having count(ZKPTov.idtov) > 0
                               )";
                DBExecute.ExecuteQuery(sql);



            }
            else
            {

                sql = $@"Select distinct ZNPTov.idtov 
                        from ZKPTov (nolock)
                        inner join ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP
                        inner join ZNPTov (nolock) on ZNPTov.idZNP = ZKP.idZNP
                         where  
                          ZKP.idZNP = {SelectedKontr.First()["id_ZNP"].ToString()} and
                          ZNPTov.idTovStatus  > 40 ";
                DataTable check = DBExecute.SelectTable(sql);


                if (check.Rows.Count > 0)
                {
                    sql = $@"Update ZNP
                         set idZNPStatus = 30
                         where idZNP = {SelectedKontr.First()["id_ZNP"]}";

                    DBExecute.ExecuteQuery(sql);

                }
                else
                {
                    sql = $@"Update ZNP
                         set idZNPStatus = 20
                         where idZNP = {SelectedKontr.First()["id_ZNP"]}";

                    DBExecute.ExecuteQuery(sql);
                }
            } 
 

           
            
        }


    }
}
