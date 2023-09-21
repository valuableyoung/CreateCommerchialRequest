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
    public partial class ReportLogOrderBuyTov : Form
    {
        string caption = "Скрыть согласования"; //отчет по полученным КП
        string captionall = "Показать весь лог";

         


        public ReportLogOrderBuyTov()
        {
            InitializeComponent(); 
            this.Text = Text + $@", Сервер - {Program.srvname}, База - {Program.mainDBName}, Пользователь - {User.Current.NKontrFull}";
        }

        private void ReportLogOrderBuyTov_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            FillgvLogOrder(captionall);
        }

        

        private void ShowAllKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if(ShowAllKPbtn.Text == caption) { FillgvLogOrder(captionall); } else { FillgvLogOrder(caption , "or LogOrderBuyTov.idAction = 60"); }                          
            this.Cursor = Cursors.Default;
        }

        public void FillgvLogOrder( string ButtonText, string filter = ""  )
        {
            try
            {
                string sql = $@"
                                    select  NomOrder,
                                         idOrderBuy,
                                         idTov,                                                                                          
                                         idTovOem ,
	                                     TmName ,
	                                     Action ,
                                         idAction,
                                         idRowBuy
                                         idUser,
	                                     datechanged,
                                         nUser,
                                         idSup,
                                         idZNP,
                                         idZKP,
                                         nSup
                                          
                                    from  (
                                            select   distinct
		                                        OrderBuy.NomOrder as NomOrder,
		                                        LogOrderBuyTov.idOrderBuy as idOrderBuy,		                                         
		                                        LogOrderBuyTov.idTov as idTov, 
		                                        spr_tov.id_tov_oem as IdTovOem,
		                                        spr_tm.tm_name as TmName,
		                                        LogOrderBuyTov.Action as Action,
		                                        LogOrderBuyTov.idAction as idAction,
		                                        LogOrderBuyTov.idRowBuy as idRowBuy,
		                                        LogOrderBuyTov.idUser as idUser ,
		                                        LogOrderBuyTov.dateChanged as datechanged,
		                                        spr_kontr.n_kontr as nUser,
                                                isNull(ZKP.idSup, 0) as idSup  ,
		                                        isNull(ZKP.idZKP , 0) as idZKP , --iif(isNull(ZKP.idZKP , 0) = 0, '-', cast(ZKP.idZKP as Varchar(100)))
		                                        isNull(ZKP.idZNP, 0) as idZNP  ,
		                                        isNull(sKontrTitle.nkontrTitle,'-') as nSup
                                            from LogOrderBuyTov
                                            inner join OrderBuy on LogOrderBuyTov.idOrderBuy  = OrderBuy.idOrderBuy 
                                            inner join spr_kontr on LogOrderBuyTov.idUser = spr_kontr.id_kontr 
                                            inner join spr_tov on spr_tov.id_tov = LogOrderBuyTov.idTov
                                            inner join spr_tm on spr_tm.tm_id = spr_tov.id_tm 
                                            left join ZKPTov on ZKPTov.idOrderBuy = LogOrderBuyTov.idOrderBuy and LogOrderBuyTov.idRowBuy = ZKPTov.idRowBuy 
                                            left join ZKP  on ZKPTov.idZKP = ZKP.idZKP 
                                            left join skontrtitle on skontrtitle.idKontrTitle  = ZKP.idSup
                                            where LogOrderBuyTov.idAction in(50, 53,55)
                                                {filter}
                                    ) as res
                                    order by datechanged desc, idAction asc "; //50 - изменение полей SKU
                                                                               //53 - добавление SKU в заказ
                                                                               //55 - извлечение SKU из заказа
                ShowAllKPbtn.Text = ButtonText;
                gcLogOrder.DataSource = DBExecute.SelectTable(sql);
            }
 
            catch(Exception ex)
            {
                UniLogger.WriteLog("", 3, "Ошибка в отчете заказов: " + ex.Message);
            }
        }

        private void gvLogOrder_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

            if (gcLogOrder.DataSource == null) { return; }
            if (e.RowHandle < 0) { return; }
                         
            DataRow row = gvLogOrder.GetDataRow(e.RowHandle) as DataRow;

             
            if (Convert.ToInt32(row["idAction"]) == 60) { e.Appearance.BackColor = DXColor.FromArgb(189, 236, 182); }
            //else if((Convert.ToInt32(row["idStatus"]) == 1) || (Convert.ToInt32(row["idStatus"]) == 5) || (Convert.ToInt32(row["idStatus"]) == 6)) { e.Appearance.BackColor = DXColor.FromArgb(255, 36, 0); }
            //else { e.Appearance.BackColor = DXColor.FromArgb(251, 236, 93); }
            
        }
    }
}
