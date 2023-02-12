namespace Fireasy.Windows.Forms.SampleNet472
{
    partial class frmTreeList
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
        private void InitializeComponent()
        {
            this.treeList1 = new Fireasy.Windows.Forms.TreeList();
            this.treeListColumn1 = new Fireasy.Windows.Forms.TreeListColumn();
            this.treeListColumn2 = new Fireasy.Windows.Forms.TreeListColumn();
            this.treeListColumn3 = new Fireasy.Windows.Forms.TreeListColumn();
            this.treeListColumn4 = new Fireasy.Windows.Forms.TreeListColumn();
            this.SuspendLayout();
            // 
            // treeList1
            // 
            this.treeList1.AlternateBackColor = System.Drawing.Color.Empty;
            this.treeList1.CheckAllChecked = false;
            this.treeList1.Columns.AddRange(new Fireasy.Windows.Forms.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4});
            this.treeList1.DataSource = null;
            this.treeList1.Footer = null;
            this.treeList1.GroupFont = new System.Drawing.Font("Consolas", 12F);
            this.treeList1.HandCursor = false;
            this.treeList1.Location = new System.Drawing.Point(12, 12);
            this.treeList1.Name = "treeList1";
            this.treeList1.NoneItemText = "没有可显示的数据";
            this.treeList1.RowNumberIndex = 0;
            this.treeList1.Size = new System.Drawing.Size(776, 426);
            this.treeList1.SortKey = null;
            this.treeList1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeList1.TabIndex = 0;
            this.treeList1.Text = "treeList1";
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.CellForeColor = System.Drawing.Color.Empty;
            this.treeListColumn1.ForeColor = System.Drawing.Color.Empty;
            this.treeListColumn1.Formatter = null;
            this.treeListColumn1.Image = null;
            this.treeListColumn1.Text = "姓名";
            this.treeListColumn1.Validator = null;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.CellForeColor = System.Drawing.Color.Empty;
            this.treeListColumn2.ForeColor = System.Drawing.Color.Empty;
            this.treeListColumn2.Formatter = null;
            this.treeListColumn2.Image = null;
            this.treeListColumn2.Text = "出生年月";
            this.treeListColumn2.Validator = null;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.CellForeColor = System.Drawing.Color.Empty;
            this.treeListColumn3.ForeColor = System.Drawing.Color.Empty;
            this.treeListColumn3.Formatter = null;
            this.treeListColumn3.Image = null;
            this.treeListColumn3.Text = "籍贯";
            this.treeListColumn3.Validator = null;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.CellForeColor = System.Drawing.Color.Empty;
            this.treeListColumn4.ForeColor = System.Drawing.Color.Empty;
            this.treeListColumn4.Formatter = null;
            this.treeListColumn4.Image = null;
            this.treeListColumn4.Text = "出生地";
            this.treeListColumn4.Validator = null;
            // 
            // frmTreeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeList1);
            this.Name = "frmTreeList";
            this.Text = "TreeList Sample";
            this.ResumeLayout(false);

        }

        #endregion

        private TreeList treeList1;
        private TreeListColumn treeListColumn1;
        private TreeListColumn treeListColumn2;
        private TreeListColumn treeListColumn3;
        private TreeListColumn treeListColumn4;
    }
}