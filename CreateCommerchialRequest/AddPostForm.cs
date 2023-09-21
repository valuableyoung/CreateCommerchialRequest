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

namespace CreateCommerchialRequest
{
    public partial class AddPostForm : Form
    {
        
        //public AddPostForm()
        //{
        //    InitializeComponent();
             
        //    Cursor.Current = Cursors.WaitCursor;
        //    this.StartPosition = FormStartPosition.CenterParent;
        //    Cursor.Current = Cursors.WaitCursor;
        //}

        fmMain fmain = new fmMain();
         
        //AddPostForm apf = new AddPostForm();



        private void AddPostForm_Load(object sender, EventArgs e)
        {
            filllistOfPosts(FocusValue.id_tov, FocusValue.id_tm, FocusValue.id_ZNP);
        }

        //private void fillgcAddPost()
        //{
        //    string sql = $@"SELECT distinct  sKontrTitle.idKontrTitle as idKontr,
        //                             sKontrTitle.nkontrTitle as Post
                           
        //                   FROM rKontrTitleTm (nolock) 
        //                   INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle ";
        //    gcAddPost.DataSource = DBExecute.SelectTable(sql);
        //}
        private void filllistOfPosts(int idtov, int idtm, int idZNP)
        {
             
            string sql = $@"SELECT distinct  sKontrTitle.idKontrTitle as idKontr,
                                     sKontrTitle.nkontrTitle as Post                                      
                           FROM rKontrTitleTm (nolock) 
                           INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle 
                           WHERE sKontrTitle.idTypeKontrTitle = 2
                          

                           Except

                           SELECT distinct  sKontrTitle.idKontrTitle as idKontr,
                                     sKontrTitle.nkontrTitle as Post                                                         
                           FROM  SPR_TOV (nolock)
                           INNER JOIN rKontrTitleTm (nolock) on rKontrTitleTm.idTm =spr_tov.id_tm
                           INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle
                           INNER JOIN ZNP (nolock) on ZNP.id_tov = spr_tov.id_tov
                           INNER JOIN ZKP (nolock) on ZKP.id_ZNP = ZNP.id_ZNP
                           WHERE spr_tov.id_tm = {idtm} and
                                 ZNP.id_tov = {idtov} and
                                 ZNP.id_ZNP = {idZNP} and 
                                 ZKP.id_tov = {idtov} and
                                 ZKP.id_kontr =  sKontrTitle.idKontrTitle"; // WHERE sKontrTitle.idTypeKontrTitle = 2 , and sKontrTitle.idTypeKontrTitle = 2

            gcAddPost.DataSource = DBExecute.SelectTable(sql);
        
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            AddKontr();
            this.Close();
        }

        private void AddKontr()
        {
            var selectedrows = gvAddPost.GetSelectedRows();
             
            var idkontr = 0;
            foreach (var row in selectedrows)
            {
                var dr = gvAddPost.GetDataRow(row);
                idkontr = Convert.ToInt32(dr["idKontr"]);
                fmain.CreateZKP(true, idkontr);
            }
            //string concat = "";
            //foreach (DataRow dr in selectedrows.Rows)
            //{
            //    concat += dr.ToString() + ",";

            //}
            //concat.TrimEnd(',');
            /*string sqlAddPost = $@"SELECT distinct  sKontrTitle.idKontrTitle as idKontr,
                                     sKontrTitle.nkontrTitle as Post
                         
                                FROM rKontrTitleTm (nolock) 
                                INNER JOIN sKontrTitle (nolock)  on sKontrTitle.idKontrTitle = rKontrTitleTm.idKontrTitle
                                WHERE rKontrTitleTm.idKontrTitle in ({concat})";*/
            //fmain.CreateZKP(true, selectedrow);
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
