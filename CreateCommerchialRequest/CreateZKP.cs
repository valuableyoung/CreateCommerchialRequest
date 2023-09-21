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
using CreateCommerchialRequest.Helpers;
using System.Globalization;
using DevExpress.Utils;

namespace CreateCommerchialRequest
{
    public partial class fmMain : Form
    {
        string ApproveRKPBtnTextZKP = "Согласовать ЗКП";
        string ApproveRKPBtnTextOrder = "Согласовать Заказ";

        private void fillgcUpRequest(int rowHandle)
        {
            try
            {
                string sql = @"SELECT distinct idZNP as id_ZNP,
                            DateZNP as CreateDate, 
                           ZNPStatuses.NStatus as  Status,
                           ZNPStatuses.idStatus as idStatus
                           FROM ZNP (nolock)
                           INNER JOIN ZNPStatuses (nolock)
                           on ZNP.idZNPStatus = ZNPStatuses.idStatus
                           WHere  ZNP.idZNPStatus > 5";
                gcUpRequest.DataSource = DBExecute.SelectTable(sql);
                gvUpRequest.FocusedRowHandle = rowHandle;
                
            }
            catch
            {
                return;
            }
            
        }
        public void gcPostLocation ()
        {
            Point psc = new Point(splitterControl1.Location.X, searchControl2.Location.Y);
            searchControl2.Location = psc;
            Point plbl = new Point(splitterControl1.Location.X + searchControl2.Width + 5, label3.Location.Y);
            label3.Location = plbl;
        }
        private void fillgcSKU(int rowHandle)
        {
            if (gvUpRequest.GetFocusedDataRow() == null) { return; };

            int idznp = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_znp"]);
            
            string sql = $@"SELECT distinct  ZNPTov.idTov as id_tov,
                        spr_tm.tm_id as id_tm,
                        spr_tm.tm_name as Brand ,
                        spr_tov.id_tov_oem as Art,  
                        spr_tov.n_tov as  Ntov, 
                        cast(ZNPTov.Kol as Varchar(500)) as Cnt,
                        isNull(spr_tov.[price] , 0) as price, 
                        ZNPTovStatuses.NStatus as  SKUStatus ,  
                        ZNPTovStatuses.idStatus as  idSKUStatus,
                        isNull((select distinct ZKPTov.isApproved
									from ZKPTov (nolock)
									INNER JOIN ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP
									where ZKP.idZNP = {idznp} and ZKPTov.idTov = ZNPTov.idTov),0 ) as isApproved  ,
                        spr_kontr.n_kontr_short as RTK                            
                        FROM ZNP (nolock)
						INNER JOIN ZNPTov (nolock) on ZNP.idZNP = ZNPTov.idZNP
                        INNER JOIN spr_tov (nolock) on ZNPTov.idTov = spr_tov.id_tov
                        INNER JOIN spr_tm (nolock) on spr_tm.tm_id = spr_tov.id_tm
                        INNER JOIN ZNPTovStatuses (nolock) on ZNPTov.idTovstatus = ZNPTovStatuses.idStatus
                        INNER JOIN spr_kontr (nolock) on ZNP.idUser = spr_kontr.id_kontr
                        left join ZKP  (nolock) on ZNP.idZNP = ZKP.idZNP
						left join ZKPTov (nolock) on ZKP.idZKP = ZKPTov.idZKP
                        WHERE ZNP.idZNP = {idznp}
                                ";
            //var a = gvUpRequest.GetFocusedDataRow()["id_znp"];
            gcSKU.DataSource = DBExecute.SelectTable(sql);


            //var b = gcSKU.DataSource;
            if (gvSKU.RowCount != 0)
            {
                gvSKU.FocusedRowHandle = FocusValue.rowHandleSKU;
                if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"].ToString()) < 80) { ApproveRKPbtn.Text = ApproveRKPBtnTextZKP; }
                else { ApproveRKPbtn.Text = ApproveRKPBtnTextOrder; }

                if (gcSKU.DataSource as DataTable != null)
                {
                    foreach (DataRow row in (gcSKU.DataSource as DataTable).Rows)
                    {
                        if (row["Cnt"].ToString() != "")
                        {
                            row["Cnt"] = string.Format("{0:n0}", Convert.ToInt32(row["Cnt"].ToString()));
                        }
                    }
                }
            }
            else if (gvSKU.RowCount == 0)
            {
                MessageBox.Show("Данная заявка на пополнение пуста!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }


            //if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["isApproved"]) == 1)
            //    gvSKU.Appearance.FocusedRow.BackColor = DXColor.FromArgb(189, 236, 182);
            //if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) == 20 || Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) == 40)
            //{
            //    gvSKU.Appearance.FocusedRow.ForeColor = DXColor.FromArgb(255, 36, 0); //255, 226, 183
            //}

            if ((FocusValue.strkontrmailnotdelivery != "") && (FocusValue.strkontrmailnotdelivery != null) && (FocusValue.strkontrmailnotdelivery != "1"))
            {
                MessageBox.Show($"Ошибка при отправке почты поставщику(ам): {FocusValue.strkontrmailnotdelivery}" + "\r\n" + "Проверьте адрес электронной почты в Справочнике поставщиков.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                FocusValue.strkontrmailnotdelivery = "";
            }
            else if (FocusValue.strkontrmailnotdelivery == "1")
            {
                MessageBox.Show($"Все ЗКП были успешно отправлены", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FocusValue.strkontrmailnotdelivery = "";
            }
        }

        private void fillgcDeliver(int rowhandle = 0)
        {
            if (gvSKU.GetFocusedDataRow() == null) { return; }
            
                int id_tm = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tm"]);



            //string sql = $@"SELECT distinct  sKontrTitle.idKontrTitle as idKontr,
            //                         sKontrTitle.nkontrTitle as Post,
            //                         iif(
            //                         isNull((select distinct ZKP.IncludeZKP from ZKP where ZKP.id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]} and ZKP.id_post = sKontrTitle.idKontrTitle  ), 0) = 0,
            //                         'Отсутствует', 'Добавлен') as IncludeZKP                         
            //               FROM  SPR_TOV (nolock)
            //               INNER JOIN rKontrTitleTm (nolock) on rKontrTitleTm.idTm =spr_tov.id_tm
            //               INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle
            //               Left JOIN ZKP (nolock) on ZKP.id_post = rKontrTitleTm.idKontrTitle
            //               WHERE spr_tov.id_tm = {gvSKU.GetFocusedDataRow()["id_tm"]} "; //iif(isNull(ZKP.isApprove, 0)  = 0, 'Отсутствует', 'Добавлен') as IsApprove  , and
            // and sKontrTitle.idTypeKontrTitle = 2

            string sql = $@"Select distinct ZKP.idSup as idKontr, 
                                sKontrTitle.nKontrTitle  as Post   ,
                                cast(ZKPTov.KolSup as Varchar(500)) as kolSup,
                                ZKPTov.PriceSup as PriceSupDecimal,
                                cast(ZKPTov.PriceSup as Varchar(500)) as priceSup,
                                spr_tov.id_tov as idtov, 
                                0.00 as PriceSupcheck ,
                                cast(0.00 as Varchar(500)) as MCMarket,
                                cast(0.00 as Varchar(500)) as RCPriceSup,
                                OrderBuy.NomOrder  as NomOrder,
							    OrderBuy.DateOrder  as DateOrder,
                                cast(OrderBuyTov.Kol as Varchar(500)) as Kol,
                                cast(OrderBuyTov.Price as Varchar (500)) as Price,
                                OrderBuyTov.idOrderBuy as idOrderBuy,
                                ZKPTov.idRowZKP,
                                ZKPTov.idRowBuy,
                                iif(OrderBuyTov.fCheckRTK = 1, 1, 0) as fCheckRTKOrder,
                                ZKP.idZKP
                               FROM  SPR_TOV (nolock)
                               INNER JOIN rKontrTitleTM (nolock) ON rKontrTitleTM.idTM = SPR_TOV.id_tm
                               INNER JOIN sKontrTitle (nolock) on sKontrTitle.idKontrTitle = rKontrTitleTM.idKontrTitle
                               INNER JOIN rKontrTitleKontr (nolock) on rKontrTitleKontr.idKontrTitle = rKontrTitleTM.idKontrTitle
                                                              
                               INNER JOIN ZKP (nolock) on ZKP.idSup =sKontrTitle.idKontrTitle
							   INNER JOIN ZKPTov (nolock) on ZKPTov.idZKP =  ZKP.idZKP and ZKPTov.idTov = spr_tov.id_tov
							   INNER JOIN ZNP  (nolock) on   ZKP.idZNP = ZNP.idZNP
                               LEFT JOIN  OrderBuy (nolock) on OrderBuy.idOrderBuy  = ZKPTov.idOrderBuy
                               LEFT JOIN  OrderBuyTov (nolock) on  OrderBuyTov.idOrderBuy = OrderBuy.idOrderBuy and OrderBuyTov.idTov = ZKPTov.idTov and ZKPTov.idRowBuy = OrderBuyTov.idRowBuy
                               WHERE spr_tov.id_tm = {id_tm}  and                                       
                                     ZKPTov.idTov = {gvSKU.GetFocusedDataRow()["id_tov"]} and
                                     ZKP.idZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} 
                                    
                                     Order By ZKPTov.PriceSup"; //ZKPTov.PriceSup  INNER JOIN spr_kontr (nolock) on spr_kontr.id_kontr = rKontrTitleKontr.idkontr
 

            gcPost.DataSource = DBExecute.SelectTable(sql);           
            gvPost.FocusedRowHandle = FocusValue.rowHandleDeliver;
            
            
            
             
 
            



                CheckPriceZKP();

            //{gvSKU.GetFocusedDataRow()["id_tov"]}
        }

 
        public void CheckPriceZKP()
        {
            if(gvPost.RowCount != 0)
            {
                try
                {
                    DataTable dt = null;
                    string sql;
                    foreach (DataRow row in (gcPost.DataSource as DataTable).Rows)
                    {
                         
                        sql = $@"select * from [dbo].[uf_ComparePriceZKP] ({Convert.ToInt32(row["idtov"])}, {Convert.ToDouble(row["priceSup"])})";
                        dt = DBExecute.SelectTable(sql);
                        var a = Convert.ToInt32(row["idtov"]);
                        var b = Convert.ToDouble(row["priceSup"]);
                        if(b > 0)
                        {
                            a = 1;
                        }
                        if (dt.Rows.Count > 0)
                        { 
                            row["RCPriceSup"] = string.Format("{0:f2}", Convert.ToDouble(dt.Rows[0][0]));
                            row["MCMarket"] = string.Format("{0:f2}", Convert.ToDouble(dt.Rows[0][1]));
                            row["PriceSupCheck"] = string.Format("{0:f2}", Convert.ToDouble(dt.Rows[0][2]));                            
                        }
                        row["priceSup"] = string.Format("{0:f2}", Convert.ToDouble(row["priceSup"]));



                         
 


                    }
                    //gvSKU.GetFocusedDataRow()["Cnt"] = string.Format("{0:n}", Convert.ToInt32(gvSKU.GetFocusedDataRow()["Cnt"]));
                }                
                catch (Exception ex)
                {
                    var a = gvSKU.GetFocusedDataRow()["Cnt"];
                    MessageBox.Show(ex.Message);
                }
               
            }
        }

        public  bool checkFillgvSKU()
        {

            if(gvSKU.RowCount == 0)
            {
                CreateRKPbtn.Enabled = false;
                ApproveRKPbtn.Enabled = false;
                SendZKPbtn.Enabled = false;
                //MessageBox.Show("Данная заявка на пополнение пуста!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private void OpenlistPostForm()
        {
            FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;
                
            FocusValue.id_tov = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tov"]);
            FocusValue.id_tm = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tm"]);
            FocusValue.id_ZNP = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);
            AddPostForm apf = new AddPostForm();
            apf.ShowDialog();
  
                             
            //lookUpEditSelectPost.Properties.DataSource = DBExecute.SelectTable(sql);
            //lookUpEditSelectPost.Properties.ValueMember.Insert(0, "нет");
        }

        public void CreateZKP(bool flag = false, int idKontr = 0)
        {
            try
            {
                if(gvUpRequest.RowCount == 0) { return; }
                if(gvSKU.RowCount == 0) { return; }
                FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;
                string dt = DateTime.Now.ToShortDateString();
                string sql;


                //ddPostForm apf = new AddPostForm();
                //if ((gvPost.DataRowCount == 0) && (!flag))
                //{
                //    MessageBox.Show("Список поставщиков пуст. Добавьте поставщика.");
                //    return;
                //}
                  
                //sql = $@"delete from ZKP where idZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} ";
                //DBExecute.ExecuteQuery(sql);
                //sql  = $@"delete from ZKPTov where idZNP = { gvUpRequest.GetFocusedDataRow()["id_ZNP"]}";
                DialogResult dr = MessageBox.Show("Товарная часть заявки будет обновлена. Подтвердить?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    if (!flag)
                    {
                        //post = "rKontrTitleTm.idKontrTitle";
                        Reset(true);
                        int id_tov = 0, id_znp = 0;

                        sql = $@"select idTov, idZNP
                               from ZNPTov (nolock)
                               where idZNP = {Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"])} and
                               idTovStatus in (10, 20)"; //Создаем ЗКП только для SKU статусов "Пополнить", "Не включено в ЗКП"                                 
                        DataTable ActualSKUForCreateZKP = DBExecute.SelectTable(sql);
                        if (ActualSKUForCreateZKP.Rows.Count ==0) {  }
                        foreach (DataRow drSKU in ActualSKUForCreateZKP.Rows) //(gcSKU.DataSource as DataTable)
                        {
                             
                            id_tov = Convert.ToInt32(drSKU["idTov"]);
                            id_znp = Convert.ToInt32(drSKU["idZNP"]);
                            sql = $@"insert into ZKP (idZNP,   idSup ,   idUser) 
                                select  distinct ZNP.idZNP,
		                            sKontrTitle.idKontrTitle as idKontr  ,
		                        {User.CurrentUserId}
		                        FROM ZNP (nolock)
		                        INNER JOIN ZNPTov (nolock) on ZNP.idZNP = ZNPTOV.idZNP
		                        INNER JOIN  SPR_TOV (nolock) ON SPR_TOV.ID_TOV = ZNPTov.IDTOV
		                        INNER JOIN rKontrTitleTM (nolock) ON rKontrTitleTM.idTM = SPR_TOV.id_tm
		                        INNER JOIN sKontrTitle (nolock) on sKontrTitle.idKontrTitle = rKontrTitleTM.idKontrTitle
                                INNER JOIN rKontrTitleKontr (nolock) on rKontrTitleKontr.idKontrTitle = rKontrTitleTM.idKontrTitle
                                 
		                        Left JOIN ZKP (nolock) on ZKP.idZNP = ZNP.idZNP
		                        Left JOIN ZKPTov (nolock) on ZKP.idZNP = ZNPTov.idZNP
                                WHERE ZNPTov.idtov  =    {id_tov} and  
                                ZNP.idZNP = {id_znp}    and                     	 
                                sKontrTitle.idTypeKontrTitle = 2 and
                                rKontrTitleKontr.fActual = 1  and      
                                sKontrTitle.idtypeeconomy <> 1 and   
                                                                            
                                ZNPTov.idTov not in ( 					 
									            select idTov
									            from ZKPTov (nolock)
									            INNER Join ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP
									            INNER Join ZNP (nolock) on ZNP.idZNP  = ZKP.idZNP									              
									            where  ZKPTov.idTov  =  {id_tov} and
                                                ZNP.idZNP = {id_znp}   and
									            ZKPTov.isApproved = 1
								            ) and
                                not exists (						 
									            select ZKP.idSup
									            from ZKP (nolock)	 
                                                where  ZKP.idSup = sKontrTitle.idKontrTitle 	and ZKP.idZNP = {id_znp}							              								              									                                                               									             
								            )  ";
                            DBExecute.ExecuteQuery(sql); //INNER JOIN spr_kontr (nolock) on spr_kontr.id_kontr = rKontrTitleKontr.idkontr  --ZKPTov.isApproved <> 0 and  isnull(ZKPTov.isApproved,1) <> 0 and 


                            sql = $@"insert into ZKPTov ( idZKP, idtov , KOl,     isApproved)
                                 select distinct    
                                     ZKP.idZKP ,                                                                     
                                     ZNPTov.idtov,                                      
                                     ZNPTov.Kol,                                                                                                            
                                     0
                                 FROM ZNPTov    (nolock)                                             
                                 INNER JOIN  SPR_TOV (nolock) ON SPR_TOV.ID_TOV = ZNPTov.IDTOV                        
                                 INNER JOIN rKontrTitleTM (nolock) ON rKontrTitleTM.idTM = SPR_TOV.id_tm
                                 INNER JOIN sKontrTitle (nolock) on sKontrTitle.idKontrTitle = rKontrTitleTM.idKontrTitle
                                 INNER JOIN rKontrTitleKontr (nolock) on rKontrTitleKontr.idKontrTitle = rKontrTitleTM.idKontrTitle
                                    
                                 INNER JOIN ZKP (nolock) on ZKP.idZNP = ZNPTov.idZNP and ZKP.idSup = sKontrTitle.idKontrTitle
                                 WHERE ZNPTov.idtov  =   {id_tov} and
                                 ZNPTov.idZNP = {id_znp}    and        
                                 sKontrTitle.idTypeKontrTitle = 2 and
                                 rKontrTitleKontr.fActual = 1 and
                                 sKontrTitle.idtypeeconomy <> 1 and
                                 not exists (						 
									            select idTov
									            from ZKPTov (nolock)
									            INNER Join ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP
									            INNER Join ZNP (nolock) on ZNP.idZNP  = ZKP.idZNP
									            where   ZKPTov.idTov  =  {id_tov} and
                                                ZNP.idZNP = {id_znp} and 
									            ZKPTov.isApproved = 1
									            
								            )  ";
                            DBExecute.ExecuteQuery(sql); //LEFT JOIN ZKP on ZKP.id_post = rKontrTitleTM.idKontrTitle    cast(rKontrTitleTm.idKontrTitle + ' ' + sKontrTitle.nkontrTitle as varchar(500))  // INNER JOIN spr_kontr (nolock) on spr_kontr.id_kontr = rKontrTitleKontr.idkontr 



                            //sql = $@"Update ZNP 
                            //            set isCreatedZKP = 1 
                            //            where ZNP.id_tov = {id_tov} and
                            //            ZNP.id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]}";
                            ////and
                            ////exists (select *
                            ////        from ZKP
                            ////        where id_tov = {id_tov} and                             
                            ////              id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]}
                            ////        )";

                            //DBExecute.ExecuteQuery(sql);



                            sql = $@"Update ZNPTov  
                            set idTovStatus = 30 
                            where idtov = {id_tov} and                             
                            idZNP = {id_znp}
                            and exists (
                                        select *
                                        from ZKPTov (nolock)
                                        inner join ZKP (nolock) on  ZKP.idZKP = ZKPtov.idZKP 
                                        where ZKPTov.idtov = {id_tov} and                             
                                              ZKP.idZNP = {id_znp}
                                        )";

                            DBExecute.ExecuteQuery(sql);

                            sql = $@"Update ZNPTov  
                            set idTovStatus = 20 
                            where idtov = {id_tov} and                             
                            idZNP = {id_znp}
                            and not exists (
                                        select *
                                        from ZKPTov (nolock)
                                        inner join ZKP (nolock) on  ZKP.idZKP = ZKPtov.idZKP 
                                        where ZKPTov.idtov = {id_tov} and                             
                                              ZKP.idZNP = {id_znp}
                                        )";
                            DBExecute.ExecuteQuery(sql); // Не включено В ЗКП

                            //  and sKontrTitle.idTypeKontrTitle = 2
                        }

                    }
                }
                if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) == 20 || Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) == 40 )
                {
                    gvSKU.Appearance.FocusedRow.ForeColor = DXColor.FromArgb(255, 36, 0); //255, 226, 183
                }
                UniLogger.WriteLog("", 1, $"По ЗНП { Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"])} созданы ЗКП");
            }
            //else
            //{
            //    if (idKontr == 0)
            //    {
            //        MessageBox.Show("Выберите поставщика для добавления в ЗКП");
            //        return;
            //    }
            //    try
            //    {
            //        sql = $@"insert into rKontrTitleTm (idKontrTitle, idTm, MinSum) values ({idKontr}, {FocusValue.id_tm}, 0)";
            //        DBExecute.ExecuteQuery(sql);
            //    }
            //    catch (Exception ex) { }


            //    sql = $@"insert into ZKP (id_ZKP, id_ZNP, idStatusZNP, id_tov , [count], price, id_skustatus , id_kontr , type_contract,  id_RTK, isApproved)
            //        select cast({idKontr} as varchar(100)) + ' ' + sKontrTitle.nkontrTitle    as id_ZKP,
            //            ZNP.id_ZNP,
            //            ZNP.ZNPStatus,
            //            ZNP.id_tov,
            //            ZNP.count,
            //            ZNP.price,  
            //            3,
            //            {idKontr},
            //            (select distinct sKontrTitle.idtypeKontrTitle from sKontrTitle  where idKontrTitle = {idKontr}) ,                                                                 
            //            { User.CurrentUserId},
            //            0
            //        FROM ZNP 
            //        INNER JOIN  SPR_TOV (nolock) ON SPR_TOV.ID_TOV = ZNP.ID_TOV
            //        INNER JOIN rKontrTitleTM (nolock) ON rKontrTitleTM.idTM = SPR_TOV.id_tm
            //        INNER JOIN sKontrTitle (nolock) on sKontrTitle.idKontrTitle = rKontrTitleTM.idKontrTitle 

            //        WHERE ZNP.id_tov =  {FocusValue.id_tov}
            //              and sKontrTitle.idKontrTitle = {idKontr}  and
            //              sKontrTitle.idTypeKontrTitle = 2"; //and  sKontrTitle.idTypeKontrTitle = 2  LEFT JOIN ZKP on ZKP.id_post = rKontrTitleTM.idKontrTitle  
            //    DBExecute.ExecuteQuery(sql);

            //    if(gvPost.RowCount == 0)
            //    {
            //        sql = $@"Update ZNP  
            //            set skustatus = 3 
            //            where id_tov = {FocusValue.id_tov} and                             
            //            id_ZNP = {FocusValue.id_ZNP}
            //            and exists (
            //                        select *
            //                        from ZKP
            //                        where id_tov = {FocusValue.id_tov} and                             
            //                              id_ZNP = {FocusValue.id_ZNP}
            //                        )";
            //        DBExecute.ExecuteQuery(sql);
            //    }



            // and ZKP.IncludeZKP is null

            //' ' + '{dt}'

            //int idpost;
            //if (gvPost.RowCount == 0)
            //{
            //    idpost = Convert.ToInt32(lookUpEditSelectPost.EditValue);
            //}
            //else
            //{
            //    idpost = Convert.ToInt32(gvPost.GetFocusedDataRow()["idKontr"]);
            //}

            //sql = $@"select id_tov from ZKP (nolock) 
            //        where id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]} and
            //        ZKP.id_post = {idpost} and
            //        ZKP.id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]}";
            //DataTable checkinsert = DBExecute.SelectTable(sql);
            //if (checkinsert.Rows.Count > 0)
            //{
            //    sql = $@"Update ZNP  
            //            set skustatus = 2 
            //            where id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]} and                             
            //            id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} ";

            //    DBExecute.ExecuteQuery(sql);
            //}

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Что-то пошло не так");
                UniLogger.WriteLog("", 3, $"По ЗНП {Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"])} ошибка: не удалось создать ЗКП" + ex.Message);

            }

        } 

        private void ApproveSKU( )
        {
            if(gvPost.RowCount == 0)
            {
                FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;
                gvUpRequest.FocusedRowHandle = FocusValue.rowHandleUpRequest; // FocusValue.rowHandleSKU
                MessageBox.Show("Список поставщиков для данной SKU пуст. Добавьте поставщика.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                return;
            }
            FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;
            // if(Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["Status"])  <


            string text = "";
            /*if (ApproveRKPbtn.Text == ApproveRKPBtnTextZKP)
            {
                text = "Вы уверены, что хотите согласовать все SKU по данной заявке на пополнение?";
                else { text = $"Вы уверены, что хотите согласовать данную SKU в заказ поставщику {gvPost.GetFocusedDataRow()["Post"].ToString()}?"; }


                DialogResult dr = MessageBox.Show(text, "Внимание", MessageBoxButtons.OKCancel,
                                       MessageBoxIcon.Question);
            
            if(dr  == DialogResult.OK)
            {*/
                try
                {
                    if (((gvPost.GetFocusedDataRow()["idOrderBuy"].ToString() == "") || (gvPost.GetFocusedDataRow()["idOrderBuy"] == null)) && Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) > 75) //Проверка SKU для заказа 
                    {
                        FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;
                        FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;

                        MessageBox.Show("Данная SKU не внесена в заказ по выбранному поставщику", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    string sql;
                    int id_tov, id_znp, idOrderBuy, idRowBuy , idSup;
                    string NumOrder, idTovOEM, TMName;
                    id_znp = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);

                    FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;
                    FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;

                    try
                    {
                         
                        if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) > 70) //Если cогласуем SKU когда она уже в заказе, было в условии == 80
                        {

                            idOrderBuy = Convert.ToInt32(gvPost.GetFocusedDataRow()["idOrderBuy"]);
                            idRowBuy = Convert.ToInt32(gvPost.GetFocusedDataRow()["idRowBuy"]);
                            id_tov = Convert.ToInt32(gvPost.GetFocusedDataRow()["idTov"]);
                            NumOrder = gvPost.GetFocusedDataRow()["NomOrder"].ToString();
                            idSup = Convert.ToInt32(gvPost.GetFocusedDataRow()["idKontr"]);
                            idTovOEM = gvSKU.GetFocusedDataRow()["Art"].ToString();
                            TMName = gvSKU.GetFocusedDataRow()["Brand"].ToString();



                            if (checkKolOrder(id_tov, id_znp))
                            {
                                DialogResult checkdr = MessageBox.Show("Общее заказываемое количество для данной SKU  не равно заявленному количеству в заявке на пополнение. Выполнить согласование?", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                                if(checkdr == DialogResult.Cancel) { return; }
                                //return;
                            }  //idOrderBuy = {idOrderBuy} and относится к первому update ниже
                            
                            sql = $@"

                                        begin try 


                                                Begin Transaction



                                                        update OrderBuyTov set fcheckRTK = 1 where  idTov = {id_tov} and idRowBuy = {idRowBuy}  
                             

                                                        update OrderBuy 
                                                                set idStatusOrder = 305 
                                                                where idOrderBuy = {idOrderBuy} and  not exists
                                                                                                               ( select 1 
                                                                                                                            from OrderBuyTov (nolock)
                                                                                                                           where idOrderBuy = {idOrderBuy} and
                                                                                                                            fCheckRTK = 0)
                            

                                                        update ZNPTov
                                                                set idTovStatus = 90 
                                                                where idZNP = {id_znp} and idTov = {id_tov} and  exists
                                                                                                               ( select 1 
                                                                                                                            from OrderBuyTov (nolock)
                                                                                                                            where idOrderBuy = {idOrderBuy} and                             
                                                                                                                            idTov = {id_tov})
                             

                                                        update ZNP
                                                                set idZNPStatus = 70 
                                                                where idZNP = {id_znp} and idZNPStatus > 50 and  not exists
                                                                                                               (
							                                                                                      select 1
                                                                                                                            from OrderBuyTov (nolock)
																											                inner join ZKPTov (nolock) on ZKPTov.idOrderBuy = OrderBuyTov.idOrderBuy and ZKPTov.idRowBuy = OrderBuyTov.idRowBuy
																											                inner join ZKP (nolock) on ZKP.idZKP = ZKPTov.idZKP
                                                                                                                            where ZKP.idZNP = {id_znp} and                                                                                                           
                                                                                                                            OrderBuyTov.fCheckRTK = 0)

                          


                                                Commit Transaction


                                        end try


                                        begin catch

                                                    Rollback Transaction

                                        end catch
                            ";

                            DBExecute.ExecuteQuery(sql);
                            UniLogger.WriteLog("", 1, $"Транзакция согласования SKU {id_tov.ToString()} в заказ idOrderBuy {idOrderBuy.ToString()} , idSup {idSup.ToString()} , поставщик {gvPost.GetFocusedDataRow()["idKontr"].ToString()} ,номер заказа {NumOrder.ToString()} , дата согласования SKU {DateTime.Now.ToString()}  , пользователь {User.CurrentUserId.ToString()}");
                            sql = $@"INSERT INTO  LogOrderBuyTov (dateChanged, idOrderBuy , idTov, KolPlan, PricePlan, KolDelivery, PriceDelivery, Action, idAction, idRowBuy , idUser, nameuser) values(GETDATE(), {idOrderBuy}, {id_tov} , 0,0,0,0, 'Согласование SKU артикул:{idTovOEM} бренд:{TMName}' , 60 , {idRowBuy} , {User.CurrentUserId.ToString()} , '{User.GetUserDomainName().ToString()}' )";
                            DBExecute.ExecuteQuery(sql);
                            return;


                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Здесь " + ex.Message);
                        return;
                    }
                     
                     



                     
                    foreach (DataRow row in (gcSKU.DataSource as DataTable).Rows)
                    {
                        id_tov = Convert.ToInt32(row["id_tov"]);
                         
                        if(Convert.ToInt32(row["idSKUStatus"]) > 50) { continue; }

                        sql = $@"Update ZKPTov
                        set  ZKPTov.isApproved  = 1  
                        FROM  ZKPTov
                        INNER JOIN ZKP on ZKP.idZKP = ZKPTov.idZKP
                        INNER JOIN ZNP on ZNP.idZNP = ZKP.idZNP 
                        INNER JOIN ZNPTov on ZNPTov.idtov = ZKPTov.idtov 
                        where ZKPTov.idTov = {id_tov} and
                            ZKP.idZNP = {id_znp}";

                        DBExecute.ExecuteQuery(sql);


                        sql = $@"Update ZNPTov  
                            set idTovStatus = 30 
                            where idtov = {id_tov} and                             
                            idZNP = {id_znp}
                            and exists (
                                        select *
                                        from ZKPTov       
                                        INNER  JOIN ZKP on ZKPTov.idZKP = ZKP.idZKP
                                        where ZKPTov.idtov = {id_tov} and                             
                                              ZKP.idZNP = {id_znp}
                                        )";

                        DBExecute.ExecuteQuery(sql);

                        

                        sql = $@"Update ZNPTov  
                            set idTovStatus = 20 
                            where idtov = {id_tov} and                             
                            idZNP = {id_znp}
                            and not exists (
                                        select *
                                        from ZKPTov       
                                        INNER  JOIN ZKP on ZKPTov.idZKP = ZKP.idZKP
                                        where ZKPTov.idtov = {id_tov} and                             
                                              ZKP.idZNP = {id_znp}
                                        )";
                        DBExecute.ExecuteQuery(sql);



                        sql = $@"Update ZNP  
                            set idZNPStatus = 20                                                         
                            where idZNP = {id_znp}
                            and not exists (
                                        select idtov
                                        from ZKPTov
                                        INNER  JOIN ZKP on ZKPTov.idZKP = ZKP.idZKP
                                        where ZKP.idZNP = {id_znp} and 
                                        ZKPTov. isApproved = 0 
                                        )";
                        DBExecute.ExecuteQuery(sql);
                        UniLogger.WriteLog("", 1, $"По ЗНП {id_znp} согласована SKU артикул: {row["Art"]}");
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                    UniLogger.WriteLog("", 3, $"По ЗНП { Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"])} ошибка: не удалось согласовать SKU артикул: {gvSKU.GetFocusedDataRow()["Art"]}" + ex.Message);
                    return;
                }

            /*}
            else
            {
                return;
            }*/
        }

 




        //private void RemovePost()
        //{
        //    try
        //    {
        //        FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;

        //        var selectedrows = gvPost.GetSelectedRows();
             
        //        var idkontr = 0;
        //        string sql;
        //        foreach (var row in selectedrows)
        //        {
        //            var dr = gvPost.GetDataRow(row);
        //            idkontr = Convert.ToInt32(dr["idKontr"]);
        //            sql = $@"delete from ZKP
        //                    where ZKP.id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]} and
        //                            ZKP.id_kontr = {dr["idKontr"]} and
        //                            ZKP.id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]}  ";
        //            DBExecute.ExecuteQuery(sql);
        //            sql = $@"Update ZNP
        //                    set znp.skustatus = 2  
        //                    FROM ZNP
        //                    left JOIN ZKP on ZNP.id_tov = ZKP.id_tov and ZNP.id_ZNP = ZKP.id_ZNP
        //                    where ZNP.id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]} and
        //                        ZNP.id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} and
        //                        ZKP.id_ZNP is null";
        //            DBExecute.ExecuteQuery(sql); //, znp.iscreatedzkp = 0


        //        }
                 
        //    }
        //    catch (Exception ex) { }

        //}


        private void checkEnabledZKPbtn()
        {
            if (gvUpRequest.GetFocusedDataRow() == null) { return; }
 
            

            int id_tov, id_znp;
            id_tov = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tov"]);
            id_znp = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);
            sql = $@"(select ZKPTov.idtov from ZKPTov (nolock)
                      INNER  JOIN ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP                           
                      where ZKP.idZNP =  {id_znp} and
                            ZKPTov.isApproved = 0)
                      UNION  
                      (select  ZNPTov.idTov
                       from ZNPTov (nolock)
                       where ZNPTov.idTovStatus < 30 and
                       ZNPTov.idZNP =  {id_znp}) "; ////isCreatedZKP = 1";
            DataTable dt = DBExecute.SelectTable(sql);

            CreateRKPbtn.Visible = false;
            ApproveRKPbtn.Visible = false;
            SendZKPbtn.Visible = false;

            if (dt.Rows.Count == 0)
            {
                sql = $@"select idtov
                     from ZKPTov (nolock)
                     INNER  JOIN ZKP (nolock) on ZKPTov.idZKP = ZKP.idZKP                                                             
                     where ZKP.idZNP = {id_znp}";
                dt = DBExecute.SelectTable(sql);
                if (dt.Rows.Count == 0)
                {
                    CreateRKPbtn.Enabled = true;
                    ApproveRKPbtn.Enabled = true;
                    SendZKPbtn.Enabled = false;
 
                     
                }
                else
                {
                    CreateRKPbtn.Enabled = false;
                    ApproveRKPbtn.Enabled = false;
                    SendZKPbtn.Enabled = true;
                }
            }
            else
            {
                CreateRKPbtn.Enabled = true;
                ApproveRKPbtn.Enabled = true;
                SendZKPbtn.Enabled = false;
            }
            //
            sql = $@"select ZNPTov.idtov
                     from ZNPTov (nolock)
                     INNER  JOIN ZKP (nolock) on ZNPTov.idZNP = ZKP.idZNP
                     Left  JOIN ZKPTov on ZKP.idZKP = ZKPTov.idZKP                                                                
                     where ZKP.idZNP = {id_znp} and ZNPTov.idTovStatus < 50 and ZNPTov.idTovStatus > 20 and isNull(ZKPTov.isApproved, 1) > 0 ";
            dt = DBExecute.SelectTable(sql);
            if(dt.Rows.Count > 0) { SendZKPbtn.Enabled = true; }

            //
            sql = $@"select idtov
                     from ZNPTov (nolock)
                                                                                  
                     where ZNPTov.idZNP = {id_znp} and ZNPTov.idTovStatus < 50  ";
            dt = DBExecute.SelectTable(sql);
            if (dt.Rows.Count == 0)
            {
                CreateRKPbtn.Enabled = false;
                ApproveRKPbtn.Enabled = false;
                SendZKPbtn.Enabled = false;
            }

            //
            sql = $@"select idtov
                     from ZKPTov (nolock)
                     inner join ZKP (nolock) on ZKPTOV.idZKP = ZKP.idZKP                                                             
                     where ZKP.idZNP = {id_znp}  and ZKPTov.isApproved = 0 ";
            dt = DBExecute.SelectTable(sql);
            if (dt.Rows.Count > 0)
            { 
                SendZKPbtn.Enabled = false;
            }
            //
            sql = $@"select distinct ZKP.idZKP
                     from ZNPTov (nolock)					 
                     inner join ZKP (nolock) on ZNPTov.idZNP = ZKP.idZNP    
					 inner join ZKPTov (nolock) on ZKPTov.idZKP = ZKP.idZKP
                     where ZKP.idZNP = {id_znp}  and ZKP.SendDate is null and ZKPTov.isApproved = 1
                     and not exists 
                     (select 1
                     from ZKPTov (nolock)
                     inner join ZKP (nolock) on ZKPTOV.idZKP = ZKP.idZKP                                                             
                     where ZKP.idZNP = {id_znp}  and ZKPTov.isApproved = 0 )";
            dt = DBExecute.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                SendZKPbtn.Enabled = true;                
            }
            CreateRKPbtn.Visible = true;
            ApproveRKPbtn.Visible = true;
            SendZKPbtn.Visible = true;
            //
            if ((Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) == 20) || (Convert.ToInt32(gvSKU.GetFocusedDataRow()["isApproved"]) == 1))
            {
                ApproveRKPbtn.Enabled = false;
            }
            if ((Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) > 70)  && (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) < 90))
            {
                 
                ApproveRKPbtn.Enabled = true; 
            }

            if (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"].ToString()) < 60) { ApproveRKPbtn.Text = ApproveRKPBtnTextZKP; }
            else { ApproveRKPbtn.Text = ApproveRKPBtnTextOrder; }

            if(gvPost.RowCount != 0)
            {
                try
                {
                    if ((gvPost.GetFocusedDataRow() as DataRow)["fCheckRTKOrder"].ToString() != "")
                    {
                        //if (Convert.ToInt32((gvPost.GetFocusedDataRow() as DataRow)["fCheckRTKOrder"]) == 1) && { ApproveRKPbtn.Enabled = false; }
                        if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 0) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() != "") && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder)) { ApproveRKPbtn.Enabled = true; } //проверка кнопки Согласовать заказ если SKU согласована, не согласована , вообще по ней нет заказа
                        else if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 0) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() == "") && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder)) { ApproveRKPbtn.Enabled = false; }
                        else if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 1) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() != "") && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder)) { ApproveRKPbtn.Enabled = false; }

                    }
                }
                catch { ApproveRKPbtn.Enabled = false; } 
            }
            else
            {
                ApproveRKPbtn.Enabled = false;
            }
             
 





            //if (dt.Rows.Count == 0)
            //{
            //    CreateRKPbtn.Enabled = false;
            //    ApproveRKPbtn.Enabled = false;
            //    SendZKPbtn.Enabled = true;
            //    //AddBtn.Enabled = false;
            //    //RemoveBtn.Enabled = true;
            //}
            //else
            //{
            //    CreateRKPbtn.Enabled = true;
            //    ApproveRKPbtn.Enabled = true;
            //    SendZKPbtn.Enabled = false;
            //    //AddBtn.Enabled = false;
            //    //RemoveBtn.Enabled = false;
            //}

        }

 


        private bool checkSearchVal()
        {
            try
            {
                if((searchControl1.Text != "") || (searchControl2.Text != "") || (searchControl3.Text != ""))
                {
                    int id_tov = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tov"]);
                    int id_tm = Convert.ToInt32(gvSKU.GetFocusedDataRow()["id_tm"]);
                    int id_ZNP = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);
                     
                }
                return true;
            }
            catch(Exception ex)
            {
                //gcSKU.DataSource = null;               
                //gcPost.DataSource = null;
                CreateRKPbtn.Enabled = false;
                ApproveRKPbtn.Enabled = false;
                return false;
            }
        }
        //private void checkCreateApprovebtn()
        //{


        //    string sql = $@"select id_ZKP from ZKP 
        //                    where id_ZNP  = {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} and                                   
        //                          id_tov =  {gvSKU.GetFocusedDataRow()["id_tov"]} and
        //                          isApproved = 1";

        //    DataTable dt = DBExecute.SelectTable(sql);
        //    if ((dt.Rows.Count > 0)  || (CreateRKPbtn.Enabled))
        //    {
        //        ApproveRKPbtn.Enabled = false; 
        //        AddBtn.Enabled = false; 
        //        RemoveBtn.Enabled = false;
        //    }
        //    else
        //    {
        //        ApproveRKPbtn.Enabled = true;
        //        AddBtn.Enabled = true;
        //        RemoveBtn.Enabled = true;
        //    }

        //}

        public void Reset(bool flag = false)
        {
            if(gvSKU.RowCount == 0) { return; }
            if(!flag)
            {
                //if(Program.srvname == @"DBSRV\DBSRV")
                //{
                //    sql = @"delete from dbo.ZKP"; // where idZKP not in (241,242,243)";
                //    DBExecute.ExecuteQuery(sql);

                //    sql = @"delete from dbo.ZKPTov "; // where idZKP not in (241,242,243)";
                //    DBExecute.ExecuteQuery(sql);

                //    sql = @"Update ZNP
                //    set    idznpstatus = 10 "; // where idZNP <> 35 ";                 
                //    DBExecute.ExecuteQuery(sql);
                //    sql = @"Update ZNPTov
                //    set idTovStatus = 10"; // where  idZNP <> 35";
                //    DBExecute.ExecuteQuery(sql);
                //}
                
            }
            else
            {
                int  id_znp;                
                id_znp = Convert.ToInt32(gvUpRequest.GetFocusedDataRow()["id_ZNP"]);                          
                sql = $@"delete ZKPTov from ZKPTov
                               inner join ZKP on ZKPTov.idZKP = ZKP.idZKP
                               where ZKP.idZNP = { id_znp} 
                                     and ZKPTov.isApproved = 0";
                DBExecute.ExecuteQuery(sql);
                sql = $@"delete ZKP from ZKP
                                inner join ZKPTov on ZKPTov.idZKP = ZKP.idZKP
                                where ZKP.idZNP = { id_znp}
                                       and ZKPTov.isApproved = 0";
                DBExecute.ExecuteQuery(sql);
                sql = $@"delete ZKP from ZKP
                                left join ZKPTov on ZKPTov.idZKP = ZKP.idZKP
                                where ZKPTov.idZKP is null  and ZKP.idZNP = { id_znp}";
                DBExecute.ExecuteQuery(sql);                
                //sql = $@"Update ZNP
                //            inner join ZKPTov on ZKPTov.idZKP = ZKP.idZKP
                //            set idznpstatus = 10                     
                //            where idZNP =  {id_znp}";
                //DBExecute.ExecuteQuery(sql);
                sql = $@"Update ZNPTov                           
                         set ZNPTov.idTovStatus = 10
                         from ZNPTov 
                         inner join ZKP on ZKP.idZNP = ZNPTov.idZNP
                         inner join ZKPTov on ZKPTov.idZKP =  ZKP.idZNP
                         where ZKP.idZNP =  {id_znp} and
                            ZKPTOV.isApproved = 0 ";
                DBExecute.ExecuteQuery(sql);

                sql = $@"Update ZNPTov                           
                         set ZNPTov.idTovStatus = 10
                         from ZNPTov 
                         left join ZKP on ZKP.idZNP = ZNPTov.idZNP
                         
                         where ZKP.idZNP is null and
                         ZNPTov.idZNP = {id_znp}";
                            
                DBExecute.ExecuteQuery(sql);
            }
             

        }





        //private bool checkAddedCurrentPost()
        //{

        //    string sql = $@"SELECT distinct  sKontrTitle.idKontrTitle as idKontr                                     
        //                    FROM  SPR_TOV (nolock)
        //                    INNER JOIN rKontrTitleTm (nolock) on rKontrTitleTm.idTm =spr_tov.id_tm
        //                    INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle
        //                    Left JOIN ZKP (nolock) on ZKP.id_post = rKontrTitleTm.idKontrTitle
        //                    WHERE spr_tov.id_tm = {gvSKU.GetFocusedDataRow()["id_tm"]} 

        //                Except
        //                    SELECT distinct  ZKP.id_post as idKontr
        //                    From ZKP 
        //                    WHERE  ZKP.id_tov = {gvSKU.GetFocusedDataRow()["id_tov"]}  and ZKP.id_ZNP = {gvUpRequest.GetFocusedDataRow()["id_znp"]} "; //and   sKontrTitle.idTypeKontrTitle = 2

        //    DataTable dt = DBExecute.SelectTable(sql);
        //    if (dt.Rows.Count > 0)
        //    {

        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}


        private void fillZNPlbl()
        {
            //ZNPlbl.Text = $"Заявка на пополнение № {gvUpRequest.GetFocusedDataRow()["id_ZNP"]} от {Convert.ToDateTime(gvUpRequest.GetFocusedDataRow()["CreateDate"]).ToShortDateString()}";
        }

        public void ConvertFormat(DataTable dtPost) //format string игнорирует, так как при передаче из запроса изначально поля были decimal , gv decimal игнорирует format string
        {
            if (dtPost.Rows != null)
            {
                
                foreach (DataRow row in dtPost.Rows)
                {
                    if (row["Price"].ToString() != "")
                    {
                        row["price"] = string.Format("{0:n2}", Convert.ToDecimal(row["price"])).ToString();
                    }

                    if (row["Kol"].ToString() != "")
                    {
                        var bb = row["Kol"];
                        row["Kol"] = string.Format("{0:n0}", Convert.ToInt32(row["Kol"].ToString()));
                    }

                    if (row["MCMarket"].ToString() != "")
                    {
                        row["MCMarket"] = string.Format("{0:n2}", Convert.ToDecimal(row["MCMarket"])).ToString();
                    }

                    if (row["RCPriceSup"].ToString() != "")
                    {
                        row["RCPriceSup"] = string.Format("{0:n2}", Convert.ToDecimal(row["RCPriceSup"])).ToString();
                    }

                    if (row["priceSup"].ToString() != "")
                    {
                        row["priceSup"] = string.Format("{0:n2}", Convert.ToDecimal(row["priceSup"])).ToString();
                    }
                    if (row["kolSup"].ToString() != "")
                    {
                        row["kolSup"] = string.Format("{0:n0}", Convert.ToInt32(row["kolSup"].ToString()));
                    }

                }


            }

            
            

        }

    }
}
