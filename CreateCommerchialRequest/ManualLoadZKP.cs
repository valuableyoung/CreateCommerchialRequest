using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateCommerchialRequest
{
    public partial class ManualLoadZKP : Form
    {
        public ManualLoadZKP()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            GetDataPost(true);
        }

        private void idZKPtb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue(idZKPtb.Text)) { idZKPtb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void cellBrandtb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue( "" , cellBrandtb.Text)) { cellBrandtb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void cellArttb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue("", cellArttb.Text)) { cellArttb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void cellCountSuptb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue("", cellCountSuptb.Text)) { cellCountSuptb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void cellPriceSuptb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue("",cellPriceSuptb.Text)) { cellPriceSuptb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void simpleButtonFindDir_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ChooseFileDirtb.Text = FindKP();
            this.Cursor = Cursors.Default;
        }

        private void LoadZKPbtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LoadKP(ChooseFileDirtb.Text);
            this.Cursor = Cursors.Default;



        }

        private void StartRowtb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue(StartRowtb.Text)) { StartRowtb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void EndRowtb_TextChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (!CheckValue(EndRowtb.Text)) { EndRowtb.Text = ""; };
            this.Cursor = Cursors.Default;
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Postcmb_TextChanged(object sender, EventArgs e)
        {
            GetZKPKontrSet();
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {
            UpdateSetKontr(true);
            GetZKPKontrSet();
        }

        private void Resetbtn_Click(object sender, EventArgs e)
        {
            UpdateSetKontr();
            GetZKPKontrSet();
        }
    }
}
