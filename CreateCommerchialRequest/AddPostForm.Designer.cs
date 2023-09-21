namespace CreateCommerchialRequest
{
    partial class AddPostForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.gcAddPost = new DevExpress.XtraGrid.GridControl();
            this.gvAddPost = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnDeliverSKUListRKP = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDeliverSKUListOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.AddBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.searchControl1 = new DevExpress.XtraEditors.SearchControl();
            ((System.ComponentModel.ISupportInitialize)(this.gcAddPost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAddPost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gcAddPost
            // 
            this.gcAddPost.Location = new System.Drawing.Point(12, 32);
            this.gcAddPost.MainView = this.gvAddPost;
            this.gcAddPost.Name = "gcAddPost";
            this.gcAddPost.Size = new System.Drawing.Size(699, 306);
            this.gcAddPost.TabIndex = 3;
            this.gcAddPost.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAddPost});
            // 
            // gvAddPost
            // 
            this.gvAddPost.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.gvAddPost.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gvAddPost.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.gvAddPost.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gvAddPost.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.gvAddPost.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gvAddPost.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDeliverSKUListRKP,
            this.gridColumnDeliverSKUListOrder,
            this.gridColumn3});
            this.gvAddPost.GridControl = this.gcAddPost;
            this.gvAddPost.Name = "gvAddPost";
            this.gvAddPost.OptionsSelection.MultiSelect = true;
            this.gvAddPost.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumnDeliverSKUListRKP
            // 
            this.gridColumnDeliverSKUListRKP.AppearanceCell.Font = new System.Drawing.Font("Arial", 9.75F);
            this.gridColumnDeliverSKUListRKP.AppearanceCell.Options.UseFont = true;
            this.gridColumnDeliverSKUListRKP.AppearanceHeader.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gridColumnDeliverSKUListRKP.AppearanceHeader.Options.UseFont = true;
            this.gridColumnDeliverSKUListRKP.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnDeliverSKUListRKP.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnDeliverSKUListRKP.Caption = "Cписок поставщиков,  ЗКП";
            this.gridColumnDeliverSKUListRKP.FieldName = "Post";
            this.gridColumnDeliverSKUListRKP.Name = "gridColumnDeliverSKUListRKP";
            this.gridColumnDeliverSKUListRKP.Visible = true;
            this.gridColumnDeliverSKUListRKP.VisibleIndex = 0;
            // 
            // gridColumnDeliverSKUListOrder
            // 
            this.gridColumnDeliverSKUListOrder.AppearanceCell.Font = new System.Drawing.Font("Arial", 9.75F);
            this.gridColumnDeliverSKUListOrder.AppearanceCell.Options.UseFont = true;
            this.gridColumnDeliverSKUListOrder.AppearanceHeader.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.gridColumnDeliverSKUListOrder.AppearanceHeader.Options.UseFont = true;
            this.gridColumnDeliverSKUListOrder.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnDeliverSKUListOrder.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnDeliverSKUListOrder.Caption = "Cписок поставщиков,  Заказ";
            this.gridColumnDeliverSKUListOrder.FieldName = "DeliverListOrder";
            this.gridColumnDeliverSKUListOrder.Name = "gridColumnDeliverSKUListOrder";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "id_kontr";
            this.gridColumn3.FieldName = "idKontr";
            this.gridColumn3.Name = "gridColumn3";
            // 
            // AddBtn
            // 
            this.AddBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddBtn.Location = new System.Drawing.Point(12, 355);
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(129, 34);
            this.AddBtn.TabIndex = 4;
            this.AddBtn.Text = "Добавить";
            this.AddBtn.UseVisualStyleBackColor = true;
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelBtn.Location = new System.Drawing.Point(577, 355);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(134, 34);
            this.CancelBtn.TabIndex = 5;
            this.CancelBtn.Text = "Отмена";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // searchControl1
            // 
            this.searchControl1.Client = this.gcAddPost;
            this.searchControl1.Location = new System.Drawing.Point(12, 6);
            this.searchControl1.Name = "searchControl1";
            this.searchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this.searchControl1.Properties.Client = this.gcAddPost;
            this.searchControl1.Size = new System.Drawing.Size(273, 20);
            this.searchControl1.TabIndex = 6;
            // 
            // AddPostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 401);
            this.Controls.Add(this.searchControl1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.AddBtn);
            this.Controls.Add(this.gcAddPost);
            this.Name = "AddPostForm";
            this.Text = "AddPostForm";
            this.Load += new System.EventHandler(this.AddPostForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcAddPost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAddPost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcAddPost;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAddPost;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDeliverSKUListRKP;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDeliverSKUListOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private System.Windows.Forms.Button AddBtn;
        private System.Windows.Forms.Button CancelBtn;
        private DevExpress.XtraEditors.SearchControl searchControl1;
    }
}