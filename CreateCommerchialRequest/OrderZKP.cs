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
using AddEASSpace;


namespace CreateCommerchialRequest
{
    public partial class fmMain : Form
    {
        public bool UpdateValueOrderZKP(DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                string sql;
                int idOrderBuy;
                int idRowBuy;
                decimal val;
                var Kol = gvPost.GetFocusedDataRow()["Kol"].ToString();
                var PriceOrder = gvPost.GetFocusedDataRow()["Price"].ToString();
                var Art = gvSKU.GetFocusedDataRow()["Art"];
                var NumOrder = gvPost.GetFocusedDataRow()["NomOrder"].ToString();
                var idPost = gvPost.GetFocusedDataRow()["idKontr"].ToString();
                var nPost = gvPost.GetFocusedDataRow()["Post"].ToString();
                var idRowZKP = gvPost.GetFocusedDataRow()["idRowZKP"];
                //var idRowBuy = gvPost.GetFocusedDataRow()["idRowBuy"];
                var idZKP = gvPost.GetFocusedDataRow()["idZKP"];
                var idZNP = gvUpRequest.GetFocusedDataRow()["id_ZNP"];
                var idTov = gvPost.GetFocusedDataRow()["idTov"].ToString();



                FocusValue.rowHandleDeliver = e.RowHandle;
                FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle;
                FocusValue.rowHandleSKU = gvSKU.FocusedRowHandle;




                //if ((Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"])  < 75 ) || (Convert.ToInt32(gvSKU.GetFocusedDataRow()["idSKUStatus"]) > 80) && (e.Value.ToString() != "")) // проверка что при вводе значения SKU имеет нужный статус для заказа
                //{
                //    MessageBox.Show("Данная SKU еще не на шаге заказа или уже согласована в заказе", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    gvPost.SetFocusedRowCellValue(e.Column.FieldName, "");
                //    return false;
                //}


                if ((e.Value.ToString() == "" && Kol != "") || (e.Value.ToString() == "" && PriceOrder != "")) { return true; }

                else if (e.Value.ToString() != "" && !decimal.TryParse(e.Value.ToString(), out val)) // вместо данной проверки работает встроенная 
                {
                    MessageBox.Show("Введено нечисловое значение", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    gvPost.SetFocusedRowCellValue(e.Column.FieldName, "");

                    return true;
                }



                if (!int.TryParse(gvPost.GetFocusedDataRow()["idOrderBuy"].ToString(), out idOrderBuy)) { idOrderBuy = 0; } else { idOrderBuy = Convert.ToInt32(gvPost.GetFocusedDataRow()["idOrderBuy"]); } // если по данному поставщику не сформирован заказ то idOrderBuy  = 0 вместо null или ""
                if (!int.TryParse(gvPost.GetFocusedDataRow()["idRowBuy"].ToString(), out idRowBuy)) { idRowBuy = 0; } else { idRowBuy = Convert.ToInt32(gvPost.GetFocusedDataRow()["idRowBuy"]); } // если по данному поставщику не сформирован заказ то idRowBuy  = 0 вместо null или ""

                if ((idOrderBuy == 0 && Kol != "" && PriceOrder == "") || (idOrderBuy == 0 && Kol == "" && PriceOrder != "")) { return true; }  //Если пользователь заполнил только первое из двух значений Кол-во и Цена в заказе а заказа как такового еще нет, то не обновляем интерфейс Поставщиков чтобы введенные значения сохранились на интерфейсе
                //if ((idOrderBuy != 0 && Kol != "" && PriceOrder == "") || (idOrderBuy == 0 && Kol == "" && PriceOrder != "")) { return true; }  //Если пользователь заполнил только первое из двух значений Кол-во и Цена в заказе а заказа как такового еще нет, то не обновляем интерфейс Поставщиков чтобы введенные значения сохранились на интерфейсе


                if (e.Value.ToString() != "")
                {
                    string value = (e.Value).ToString().Replace(" ", "");
                    sql = $@"Update OrderBuyTov  set {e.Column.FieldName.ToString()} = {value} where idOrderBuy =  {idOrderBuy} and idTov = {gvPost.GetFocusedDataRow()["idTov"].ToString()} and idRowBuy ={idRowBuy}  ";
                    DBExecute.ExecuteQuery(sql);
                    if (idOrderBuy != 0)
                    {
                        if (e.Column.FieldName.ToString() == "Price")
                        {
                            UniLogger.WriteLog("", 1, $"Изменение {e.Column.FieldName.ToString()} , новое значение {value}, старое значение {FocusValue.OldPrice} для SKU {idTov} в заказе " + NumOrder.ToString() + ", UserId " + User.CurrentUserId.ToString());
                            sql = $@"INSERT INTO  LogOrderBuyTov (dateChanged, idOrderBuy,  idTov, KolPlan, PricePlan, KolDelivery, PriceDelivery, Action, idAction, idRowBuy, idUser , nameuser) values(GETDATE(), {idOrderBuy}, {idTov} , 0,0,0,0,'Поле <Цена, заказ> , старое значение: {FocusValue.OldPrice} , новое значение: {string.Format("{0:n0}", Convert.ToDecimal(e.Value)).ToString()}', 50 , {idRowBuy} , {User.CurrentUserId.ToString()} , '{User.GetUserDomainName().ToString()}'  )";

                        }
                        else
                        {
                            UniLogger.WriteLog("", 1, $"Изменение {e.Column.FieldName.ToString()} , новое значение {value }, старое значение {FocusValue.OldKol} для SKU {idTov} в заказе " + NumOrder.ToString() + ", UserId " + User.CurrentUserId.ToString());
                            sql = $@"INSERT INTO  LogOrderBuyTov (dateChanged, idOrderBuy, idTov, KolPlan, PricePlan, KolDelivery, PriceDelivery, Action, idAction, idRowBuy, idUser, nameuser) values(GETDATE(), {idOrderBuy}, {idTov} , 0,0,0,0, 'Поле <Кол-во, заказ> , старое значение: {FocusValue.OldKol} , новое значение: {string.Format("{0:n2}", Convert.ToDecimal(e.Value)).ToString()}', 50 , {idRowBuy} , {User.CurrentUserId.ToString()} , '{User.GetUserDomainName().ToString()}'  )";
                        }
                        DBExecute.ExecuteQuery(sql);
                    }

                }





                if ((Kol == "") && (PriceOrder == "")) // если пользователь стирает оба значения подряд то попытка найти и удалить строку заказа, а также если строка заказа была одна то и шапку заказа
                {
                    sql = $@"INSERT INTO  LogOrderBuyTov (dateChanged, idOrderBuy,  idTov, KolPlan, PricePlan, KolDelivery, PriceDelivery, Action, idAction, idRowBuy, idUser , nameuser) values(GETDATE(), {idOrderBuy}, {idTov} , 0,0,0,0,'Удаление SKU из заказа №{NumOrder}', 55 , {idRowBuy} , {User.CurrentUserId.ToString()} , '{User.GetUserDomainName().ToString()}'  )";
                    DBExecute.ExecuteQuery(sql);

                    sql = $@"delete from OrderBuyTov where idOrderBuy = {idOrderBuy} and idTov = {idTov} and idRowBuy = {idRowBuy}";
                    DBExecute.ExecuteQuery(sql);
                    sql = $@"update ZKPTov  set idOrderBuy = Null , idRowBuy = null where idOrderBuy = {idOrderBuy} and idTov = {idTov} and idZKP = {idZKP} and idRowBuy = {idRowBuy} "; //idRowBuy = Null
                    DBExecute.ExecuteQuery(sql);
                    sql = $@"select 1 from OrderbuyTov (nolock) where idOrderBuy = {idOrderBuy} ";
                    DataRow checkrow = DBExecute.SelectRow(sql);
                    if (checkrow == null) // если строка заказа была одна то удаляем шапку заказа то есть заказ 
                    {
                        sql = $@"delete from OrderBuy where idOrderBuy  = {idOrderBuy}";
                        //sql = $@"Update OrderBuy set idStatusOrder = -1 where idOrderBuy  = {idOrderBuy}"; // физичесески не удаляем шпаку заказа а ставим статус -1 
                        DBExecute.ExecuteQuery(sql);
                    }




                    string s = "";
                    foreach (DataRow dr in (gcPost.DataSource as DataTable).Rows) // проверяем есть ли данная SKU в принципе в других заказах по поставщику для данной ЗНП
                    {
                        s += dr["idZKP"].ToString() + ",";

                    }
                    s = s.TrimEnd(',');

                    sql = $@"select 1 from OrderbuyTov (nolock)
                             inner join ZKPTov (nolock) on OrderbuyTov.idOrderBuy = ZKPTov.idOrderBuy 
                             where ZKPTov.idTov = {idTov}  and ZKPTov.idZKP in ({s}) ";
                    checkrow = DBExecute.SelectRow(sql);
                    if (checkrow == null)
                    {
                        sql = $@"Update ZNPTov  set idTovStatus = 75 where idTov = {idTov} and idZNP = {idZNP}"; // SKU не в заказе

                        DBExecute.ExecuteQuery(sql);
                    }


                    sql = $@"                   update OrderBuy 
                                                set idStatusOrder = 305 
                                                where idOrderBuy = {idOrderBuy} and  not exists
                                                                                               ( select 1 
                                                                                                            from OrderBuyTov (nolock)
                                                                                                           where idOrderBuy = {idOrderBuy} and
                                                                                                            fCheckRTK = 0)";
                    DBExecute.ExecuteQuery(sql);




                    sql = $@"                   update ZNP
                                                set idZNPStatus = 70 
                                                where idZNP = {idZNP} and idZNPStatus > 50 and  not exists
                                                                                               (
							                                                                      select 1
                                                                                                            from OrderBuyTov (nolock)
																											inner join ZKPTov (nolock) on ZKPTov.idOrderBuy = OrderBuyTov.idOrderBuy and ZKPTov.idRowBuy = OrderBuyTov.idRowBuy
																											inner join ZKP (nolock) on ZKP.idZKP = ZKPTov.idZKP
                                                                                                            where ZKP.idZNP = {idZNP} and                                                                                                           
                                                                                                            OrderBuyTov.fCheckRTK = 0)";

                    DBExecute.ExecuteQuery(sql);


                    UniLogger.WriteLog("", 1, $"Удаление SKU {idTov} из заказа " + NumOrder.ToString() + ", UserId " + User.CurrentUserId.ToString());

                }


                else if ((Kol != "") && (PriceOrder != "") && (NumOrder == "")) // если пользователь ввел оба значения Кол-во и Цена, но номера заказа нет, то заказ нужно сформировать
                {
                    //rc_call Таня
                    WStoEAS wse = new WStoEAS();
                    //MessageBox.Show(WStoEAS.GetData() + " ");
                    try
                    {
                        //sql = $@"select 1 from OrderBuy where idSupplier = {idPost} and fCheckRTK = 0"; // проверка есть ли уже существующий заказ для этого поставщика, который еще не согласован
                        //DataRow checkSupplierOrder = DBExecute.SelectRow(sql);
                        //if(checkSupplierOrder == null)
                        //{
                        if (wse.rc_checkwork() == 1)
                        {

                            string response = wse.rc_createorderbuyZNP(User.CurrentUserId, idRowZKP.ToString(), Kol.ToString(), PriceOrder.ToString()); // варианты для создания нового заказа и для существующего, метод Тани делает оба варианта
                                                                                                                                                        //AutoClosingMessageBox.Show(response, "Подтверждение", 1000);
                            sql = $@"select distinct ZKPTov.idOrderBuy, 
                                        ZKPTov.idRowBuy,
                                        OrderBuy.Nomorder
                                        from ZKPTov
                                        inner join OrderBuy on ZKPTov.idOrderBuy = OrderBuy.idOrderBuy
                                        where ZKPTov.idRowZKP = {idRowZKP}";
                            DataRow row = DBExecute.SelectRow(sql);
                            NumOrder = row["Nomorder"].ToString();
                            idOrderBuy = Convert.ToInt32(row["idOrderBuy"]);
                            idRowBuy = Convert.ToInt32(row["idRowBuy"]);

                            UniLogger.WriteLog("", 0, $"Создание нового заказа {NumOrder} для ЗНП {idZNP.ToString()} для idTov {idTov.ToString()} + , UserId " + User.CurrentUserId.ToString());
                        }
                        //}
                        //else
                        //{
                        //    sql = $@"Insert into OrderBuyTov(idOrderBuy , idTov , Kol , Price )
                        //      values( 
                        //            {idOrderBuy},
                        //            {idTov},
                        //            {Kol},
                        //            {PriceOrder})";
                        //    DBExecute.ExecuteQuery(sql); // Метод Тани делает инсерт и в табличную часть заказа
                        //}

                    }
                    catch (Exception ex) { }

                    //MessageBox.Show( $"SKU {Art.ToString()} добавлена в заказ {NumOrder} поставщику " + nPost, "Подтверждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UniLogger.WriteLog("", 1, $"Добавление SKU {idTov} в заказа " + NumOrder.ToString() + ", UserId " + User.CurrentUserId.ToString());
                    sql = $@"INSERT INTO  LogOrderBuyTov (dateChanged, idOrderBuy,  idTov, KolPlan, PricePlan, KolDelivery, PriceDelivery, Action, idAction, idRowBuy, idUser , nameuser) values(GETDATE(), {idOrderBuy}, {idTov} , 0,0,0,0,'Добавление SKU в заказ №{NumOrder}', 53 , {idRowBuy} , {User.CurrentUserId.ToString()} , '{User.GetUserDomainName().ToString()}'  )";
                    DBExecute.ExecuteQuery(sql);
                }



                if ((Kol != "") && (PriceOrder != "") && (NumOrder != ""))
                {
                    sql = $@"select 1 from OrderbuyTov where idOrderBuy = {idOrderBuy} and idTov = {idTov} and idRowBuy = {idRowBuy}  ";
                    DataRow checkrow = DBExecute.SelectRow(sql);
                    if (checkrow == null)
                    {
                        //sql = $@"Insert into OrderBuyTov(idOrderBuy , idTov , Kol , Price )
                        //      values( 
                        //            {idOrderBuy},
                        //            {idTov},
                        //            {Kol},
                        //            {PriceOrder})";
                        //DBExecute.ExecuteQuery(sql); // Метод Тани делает инсерт и в табличную часть заказа

                        //    sql = $@"Update ZKPTov set idOrderBuy = {idOrderBuy} ";//, idRowBuy = (select idRowBuy from OrderBuyTov where idOrderBuy = {idOrderBuy} and idTov = {idTov}) ";
                        //    DBExecute.ExecuteQuery(sql);

                        //    sql = $@"Update ZNPTov set idTovStatus = 80  where idTov = {idTov} and idZNP = {idZNP}) ";
                        //    DBExecute.ExecuteQuery(sql);
                        //    UniLogger.WriteLog("", 1, $"Обновление статус 80 SKU {idTov} в заказе " + NumOrder.ToString());
                    }

                    //sql = $@"Update ZKPTov set idOrderBuy = {idOrderBuy} , idRowBuy = {}" //(select idRowBuy from OrderBuyTov where idOrderBuy = {idOrderBuy} and idTov = {idTov}) ";
                    //DBExecute.ExecuteQuery(sql);

                    sql = $@"Update ZNPTov set idTovStatus = 80  where idTov = {idTov} and idZNP = {idZNP} ";
                    DBExecute.ExecuteQuery(sql);
                    UniLogger.WriteLog("", 1, $"Обновление статус 80 SKU {idTov} в заказе " + NumOrder.ToString() + ", UserId " + User.CurrentUserId.ToString());

                }

 
                Timer t = new Timer();
                t.Interval = 5000;
                t.Start();
                SuccessSavebtn.Visible = true;
                SuccessSavelbl.Visible = true;
                t.Tick += delegate
               {
                   t.Stop();
                   SuccessSavebtn.Visible = false;
                   SuccessSavelbl.Visible = false;
               };

                 

                //AutoClosingMessageBox.Show("Изменения внесены", "Подтверждение", 1000);
                //MessageBox.Show("Изменения внесены", "Подтверждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
                MessageBox.Show("Ошибка при изменении значения SKU в заказе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UniLogger.WriteLog("", 3, "Ошибка при изменении значения заказа: " + gvPost.GetFocusedDataRow()["NomOrder"].ToString() + " " + ex.Message);
                return false;

            }
        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }




















        public bool checkKolOrder(int idTov , int idZNP)
        {
            string sql = $@"select  iif(Sum(SKUKol.Kol) > Min(SKUKol.KolZNP), 0, 1) as CheckKol
                            from  
                            (
                                    select distinct  OrderBuyTov.Kol , ZNPTov.Kol as KolZNP 
                                    from OrderBuyTov
                                    inner join ZKPTov on ZKPTov.idOrderBuy = OrderBuyTov.idOrderBuy and ZKPTov.idRowBuy = OrderBuyTov.idRowBuy
                                    inner join ZKP on ZKP.idZKP = ZKPTov.idZKP
									inner join ZNPTov on ZNPTov.idZNP = ZKP.idZNP and ZNPTov.idTov = ZKPTov.idTov
                                    where ZKP.idZNP = {idZNP} and OrderBuyTov.idTov = {idTov} --2759738 -- 2716984
                            ) as SKUKol";
            DataRow dr = DBExecute.SelectRow(sql);
            if (dr["CheckKol"].ToString() == "0") { return true; } else { return false; }
 
        }
    }
}
