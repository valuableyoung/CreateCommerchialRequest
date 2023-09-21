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
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Text = Text + $@", Сервер - {Program.srvname}, База - {Program.mainDBName}, Пользователь - {User.Current.NKontrFull}";
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            fillgcUpRequest(FocusValue.rowHandleUpRequest);
            gcPostLocation();
            ConvertFormat(gcPost.DataSource as DataTable);
            gvPost.OptionsBehavior.EditorShowMode = EditorShowMode.MouseUp;

            //fillgcSKU(FocusValue.rowHandleSKU);
            //checkCreateZKPbtn();
            //checkCreateApprovebtn();
            //checkSendZKP(); 
            //TestSpliiterForm tsf = new TestSpliiterForm();
            //tsf.Show();

            //пример вызова метода ЕАС для создания заказа
            //var res = Program.ws.rc_createorderbuyZNP(Connection.ConnectionUserID, idznp);
        }


        private void gvUpRequest_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!checkSearchVal()) { return; }

            //fillZNPlbl();
             
            fillgcSKU(FocusValue.rowHandleSKU);
            if (!checkFillgvSKU()) { return; }
            fillgcDeliver();
            CheckPriceZKP();
            checkEnabledZKPbtn();
            ConvertFormat(gcPost.DataSource as DataTable);
            //checkSendZKP();

        }

        private void gvSKU_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
           if (!checkSearchVal()) { return; }

            //fillgcSKU(FocusValue.rowHandleSKU);
            if (!checkFillgvSKU()) { return; }
            fillgcDeliver();
            CheckPriceZKP();
            checkEnabledZKPbtn();
            ConvertFormat(gcPost.DataSource as DataTable);

        }

        private void CreateRKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!checkSearchVal()) { return; }
            CreateZKP();
            checkEnabledZKPbtn();

            fillgcSKU(FocusValue.rowHandleSKU);
            if (!checkFillgvSKU()) { return; }
            fillgcDeliver(FocusValue.rowHandleDeliver);
            ConvertFormat(gcPost.DataSource as DataTable);
            this.Cursor = Cursors.Default;
            //checkCreateApprovebtn();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (!checkSearchVal()) { return; }
            OpenlistPostForm();
            //fillgcSKU(FocusValue.rowHandleSKU);
            //if (!checkFillgvSKU()) { return; }
            //fillgcDeliver();
            //checkEnabledZKPbtn();
            //checkCreateApprovebtn();
            //checkSendZKP();
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            if (!checkSearchVal()) { return; }
            checkEnabledZKPbtn();
            //RemovePost();
            //fillgcSKU(FocusValue.rowHandleSKU);
            //if (!checkFillgvSKU()) { return; }
            //fillgcDeliver();
            //checkCreateApprovebtn();
        }

        private void gvPost_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
             

            if (gvPost.DataSource == null) { return; }
            if (e.FocusedRowHandle < 0) { return; }
            DataRow dr = gvPost.GetDataRow(e.FocusedRowHandle);
            if ((dr["fCheckRTKOrder"].ToString() == "1")) // проверка при заполнении для заказа || (Convert.ToInt32(dr["kolSup"]) == 0 && Convert.ToInt32(dr["priceSup"]) == 0)
            {
                gvPost.Columns["Kol"].OptionsColumn.AllowEdit = false;
                gvPost.Columns["Price"].OptionsColumn.AllowEdit = false;
                
            }
            else if (dr["fCheckRTKOrder"].ToString() == "0") // проверка при заполнении для заказа Convert.ToInt32(dr["kolSup"]) > 0 && Convert.ToInt32(dr["priceSup"]) > 0 &&  попросили оставить полностью доступным   на редактирование поля Цена и Кол-во Заказ        
            {
                gvPost.Columns["Kol"].OptionsColumn.AllowEdit = true;
                gvPost.Columns["Price"].OptionsColumn.AllowEdit = true;
            }
            else  // проверка при заполнении для заказа для остальных вариантов 
            {
                //gvPost.Columns["Kol"].OptionsColumn.AllowEdit = false;
                //gvPost.Columns["Price"].OptionsColumn.AllowEdit = false;
                gvPost.Columns["Kol"].OptionsColumn.AllowEdit = true; // оставить доступными поля для редактирования даже если у поставщика нет КП
                gvPost.Columns["Price"].OptionsColumn.AllowEdit = true;
            }

              //(gcPost.DataSource as DataTable).Rows)
            
            if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 0) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() != "")  && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder))  { ApproveRKPbtn.Enabled = true; }
            else if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 0) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() == "") && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder)) { ApproveRKPbtn.Enabled = false; }
            else if ((Convert.ToInt32(gvPost.GetFocusedDataRow()["fCheckRTKOrder"]) == 1) && ((gvPost.GetFocusedDataRow() as DataRow)["NomOrder"].ToString() != "") && (ApproveRKPbtn.Text == ApproveRKPBtnTextOrder)) { ApproveRKPbtn.Enabled = false; }

            FocusValue.OldPrice = gvPost.GetFocusedDataRow()["Price"].ToString();
            FocusValue.OldKol = gvPost.GetFocusedDataRow()["Kol"].ToString();


        }

        private void ApproveRKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;            
            if (!checkSearchVal()) { return; }
            ApproveSKU();
            fillgcUpRequest(FocusValue.rowHandleUpRequest);
            fillgcSKU(FocusValue.rowHandleSKU);
            if (!checkFillgvSKU()) { return; }
            fillgcDeliver(FocusValue.rowHandleDeliver);
            checkEnabledZKPbtn();
            ConvertFormat(gcPost.DataSource as DataTable);
            this.Cursor = Cursors.Default;
            //checkCreateApprovebtn();
            //checkSendZKP();
        }

        private void gvSKU_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (gcSKU.DataSource == null)
                    return;
                DataRow dr = gvSKU.GetDataRow(e.RowHandle);
                 
                if (dr == null)
                    return;

                if (gvSKU.RowCount == 1)
                {
                    e.HighPriority = true;
                }


                if (((Convert.ToInt32(dr["idSKUStatus"]) == 20)  ) || (Convert.ToInt32(dr["idSKUStatus"]) == 40) || (Convert.ToInt32(dr["idSKUStatus"]) == 60))  //(Convert.ToInt32(dr["isApproved"]) == 0) && (!CreateRKPbtn.Enabled)
                {
                        

                    e.Appearance.ForeColor= DXColor.FromArgb(255, 36, 0); //255, 226, 183


                }

                else if(Convert.ToInt32(dr["isApproved"]) == 1)
                { 

                    e.Appearance.BackColor = DXColor.FromArgb(189, 236, 182);
                         
                }                                    
                    



            }
            catch (Exception ex)
            {
                MessageBox.Show("error. " + ex.Message);
            }
        }

 

        private void SendZKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            SendZKP();
            fillgcUpRequest(FocusValue.rowHandleUpRequest);
            fillgcSKU(FocusValue.rowHandleSKU);
            ConvertFormat(gcPost.DataSource as DataTable);
            this.Cursor = Cursors.Default;

        }

        private void labelReset_Click(object sender, EventArgs e)
        {
            //Reset();
            //fillgcUpRequest(FocusValue.rowHandleUpRequest);
            //fillgcSKU(FocusValue.rowHandleSKU);
            //checkEnabledZKPbtn();
            //checkCreateApprovebtn();
            //checkSendZKP();
        }

        private void simpleButton1_Click(object sender, EventArgs e) // Refresh
        {
            this.Cursor = Cursors.WaitCursor;
            fillgcUpRequest(FocusValue.rowHandleUpRequest = gvUpRequest.FocusedRowHandle);
            fillgcSKU(FocusValue.rowHandleSKU);
            fillgcDeliver(FocusValue.rowHandleDeliver);
            checkEnabledZKPbtn();
            ConvertFormat(gcPost.DataSource as DataTable);
            this.Cursor = Cursors.Default;
        }

        private void gvPost_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (gcPost.DataSource == null)
                    return;

                DataRow dr = gvPost.GetDataRow(e.RowHandle);

                if (dr == null)
                    return;

                

                if (Convert.ToInt32(dr["PriceSupCheck"]) == 1)  //(Convert.ToInt32(dr["isApproved"]) == 0) && (!CreateRKPbtn.Enabled)
                {
                    if(gvPost.RowCount == 1)
                    {
                        e.HighPriority = true;
                    }                                         
                    e.Appearance.ForeColor = DXColor.FromArgb(255, 36, 0); //255, 226, 183
                     

                }
                if (Convert.ToInt32(dr["fCheckRTKOrder"]) == 1)  //(Convert.ToInt32(dr["isApproved"]) == 0) && (!CreateRKPbtn.Enabled)
                {
                    if (gvPost.RowCount == 1)
                    {
                        e.HighPriority = true;
                    }
                    e.Appearance.BackColor = DXColor.FromArgb(189, 236, 182); //255, 226, 183


                }







                //gvPost.Columns["Kol"].OptionsColumn.AllowEdit = true;
                //gvPost.Columns["Price"].OptionsColumn.AllowEdit = true;
                //if ((dr["fCheckRTKOrder"].ToString() == "1") && (gvPost.RowCount == 1))
                //{
                //    gvPost.Columns["Kol"].OptionsColumn.AllowEdit = false;
                //    gvPost.Columns["Price"].OptionsColumn.AllowEdit = false;
                //}









                //else if (Convert.ToInt32(dr["isApproved"]) == 1)
                //{

                //    e.Appearance.BackColor = DXColor.FromArgb(189, 236, 182);

                //}




            }
            catch (Exception ex)
            {
                MessageBox.Show("error. " + ex.Message);
            }
        }

 

        private void splitterControl1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            gcPostLocation();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void ReportGetKP_Click(object sender, EventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;
            ReportGetKP rgkp = new ReportGetKP();            
            rgkp.Show();
            this.Cursor = Cursors.Default;
        }
 

        private void gvPost_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;
            if (UpdateValueOrderZKP(e)) { this.Cursor = Cursors.Default; return; };
            fillgcUpRequest(FocusValue.rowHandleUpRequest);             
            if (!checkFillgvSKU() ) { this.Cursor = Cursors.Default; return; }
            fillgcSKU(FocusValue.rowHandleSKU);
            fillgcDeliver(FocusValue.rowHandleDeliver);
            checkEnabledZKPbtn();
            ConvertFormat(gcPost.DataSource as DataTable);
            this.Cursor = Cursors.Default;

        }

        private void ReportLogOrderBuyTovbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ReportLogOrderBuyTov rl = new ReportLogOrderBuyTov();
            rl.Show();
            this.Cursor = Cursors.Default;
        }

        private void ManualLoadZKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ManualLoadZKP mnl = new ManualLoadZKP();
            mnl.Show();
            this.Cursor = Cursors.Default;
        }

        private void gvPost_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            gvPost.OptionsBehavior.AutoSelectAllInEditor = true;
        }
    } 
}
