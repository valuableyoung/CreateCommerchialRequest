using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateCommerchialRequest.Database;
using CreateCommerchialRequest.Models;
using DevExpress.Utils;

namespace CreateCommerchialRequest
{
    public partial class ReportGetKP : Form
    {
        string caption = "Скрыть КП без ошибок"; //отчет по полученным КП
        string captionall = "Показать все КП";

         


        public ReportGetKP()
        {
            InitializeComponent(); 
            this.Text = Text + $@", Сервер - {Program.srvname}, База - {Program.mainDBName}, Пользователь - {User.Current.NKontrFull}";
        }

        private void ReportGetKP_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;                          
            FillgvGetKP(captionall, "where ZKPExchangeLog.idStatusCheckLog > 0");
        }

        

        private void ShowAllKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if(ShowAllKPbtn.Text == caption) { FillgvGetKP(captionall, "where ZKPExchangeLog.idStatusCheckLog > 0"); } else { FillgvGetKP(caption); }                          
            this.Cursor = Cursors.Default;
        }

        public void FillgvGetKP( string ButtonText, string filter = ""  )
        {
            try
            {
                string sql = $@"
                                    select  idZKP, 
                                         idTovOem ,
	                                     TmName ,
	                                     nStatus ,
	                                     DateGet,
                                         idZNP,
                                         nSup,
                                         idStatus,
                                         nUser
                                    
                                    from  (
                                            select distinct ZKPExchangeLog.idZKP, 
                                                    ZKPExchangeLog.idTovOem ,
	                                                ZKPExchangeLog.TmName ,
	                                                ZKPExchangeLogStatuses.nStatus ,
	                                                ZKPExchangeLog.DateGet,
                                                    ZKP.idZNP,
                                                    isNull(sKontrTitle.nKontrTitle, 'Email поставщика-отправителя не найден в базе') as nSup,
                                                    ZKPExchangeLog.idStatusCheckLog as idStatus ,
                                                     isNull(spr_kontr.n_Kontr, '-') as nUser
                                            from ZKPExchangeLog (nolock)    
                                            inner join ZKPExchangeLogStatuses (nolock) on ZKPExchangeLog.idStatusCheckLog = ZKPExchangeLogStatuses.idStatus                                            
                                            left join ZKP (nolock)  on ZKP.idZKP = ZKPExchangeLog.idZKP --and ZKP.idSup = ZKPExchangeLog.idSup 	
                                            left join 	skontrtitle (nolock) on skontrtitle.idKontrTitle = ZKP.idSup
                                            left join spr_kontr on ZKPExchangeLog.idUser = spr_kontr.id_kontr                                             
                                            {filter}

                                    ) as res
                                    order by DateGet desc, idStatus asc ";
                ShowAllKPbtn.Text = ButtonText;
                gcGetKP.DataSource = DBExecute.SelectTable(sql);
            }
 
            catch(Exception ex)
            {
                UniLogger.WriteLog("", 3, "Ошибка в отчете полученных КП: " + ex.Message);
            }
        }

        private void gvGetKP_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

            if (gcGetKP.DataSource == null) { return; }
            if (e.RowHandle < 0) { return; }
                         
            DataRow row = gvGetKP.GetDataRow(e.RowHandle) as DataRow;

             
            if ((Convert.ToInt32(row["idStatus"]) == 0)  || (Convert.ToInt32(row["idStatus"]) == 8)) { e.Appearance.BackColor = DXColor.FromArgb(189, 236, 182); }
            else if((Convert.ToInt32(row["idStatus"]) == 1) || (Convert.ToInt32(row["idStatus"]) == 5) || (Convert.ToInt32(row["idStatus"]) == 6)) { e.Appearance.BackColor = DXColor.FromArgb(255, 36, 0); }
            else { e.Appearance.BackColor = DXColor.FromArgb(251, 236, 93); }
            
        }
    }
}
