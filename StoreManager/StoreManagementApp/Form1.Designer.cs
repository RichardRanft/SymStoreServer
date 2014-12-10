namespace StoreManagementApp
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tPageVersion = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dataGridViewServer = new System.Windows.Forms.DataGridView();
            this.Transaction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pointer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Product = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRefreshServerView = new System.Windows.Forms.Button();
            this.tPageStore = new System.Windows.Forms.TabPage();
            this.btnFetchJobCount = new System.Windows.Forms.Button();
            this.lblPendingJobCount = new System.Windows.Forms.Label();
            this.dataGridViewHistory = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbPostTestResponse = new System.Windows.Forms.TextBox();
            this.btnRefreshHistoryView = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbStoreAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tPageVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewServer)).BeginInit();
            this.tPageStore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tPageVersion);
            this.tabControl1.Controls.Add(this.tPageStore);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(917, 506);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Resize += new System.EventHandler(this.tabControl1_Resize);
            // 
            // tPageVersion
            // 
            this.tPageVersion.Controls.Add(this.label4);
            this.tPageVersion.Controls.Add(this.btnDelete);
            this.tPageVersion.Controls.Add(this.dataGridViewServer);
            this.tPageVersion.Controls.Add(this.btnRefreshServerView);
            this.tPageVersion.Location = new System.Drawing.Point(4, 22);
            this.tPageVersion.Name = "tPageVersion";
            this.tPageVersion.Padding = new System.Windows.Forms.Padding(3);
            this.tPageVersion.Size = new System.Drawing.Size(909, 480);
            this.tPageVersion.TabIndex = 0;
            this.tPageVersion.Text = "Version Management";
            this.tPageVersion.UseVisualStyleBackColor = true;
            this.tPageVersion.Resize += new System.EventHandler(this.tPageVersion_Resize);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Current Symbol Store Status";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(829, 454);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 21);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dataGridViewServer
            // 
            this.dataGridViewServer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Transaction,
            this.Type,
            this.Pointer,
            this.Date,
            this.Time,
            this.Product,
            this.Version,
            this.Comment});
            this.dataGridViewServer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewServer.Location = new System.Drawing.Point(3, 19);
            this.dataGridViewServer.MultiSelect = false;
            this.dataGridViewServer.Name = "dataGridViewServer";
            this.dataGridViewServer.Size = new System.Drawing.Size(901, 429);
            this.dataGridViewServer.TabIndex = 6;
            this.dataGridViewServer.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewServer_CellMouseUp);
            this.dataGridViewServer.SelectionChanged += new System.EventHandler(this.dataGridViewServer_SelectionChanged);
            this.dataGridViewServer.MouseEnter += new System.EventHandler(this.dataGridViewServer_MouseEnter);
            this.dataGridViewServer.MouseLeave += new System.EventHandler(this.dataGridViewServer_MouseLeave);
            this.dataGridViewServer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGridViewServer_MouseUp);
            // 
            // Transaction
            // 
            this.Transaction.HeaderText = "ID";
            this.Transaction.Name = "Transaction";
            this.Transaction.Width = 80;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.Width = 40;
            // 
            // Pointer
            // 
            this.Pointer.HeaderText = "Pointer";
            this.Pointer.Name = "Pointer";
            this.Pointer.Width = 40;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.Width = 80;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.Width = 60;
            // 
            // Product
            // 
            this.Product.HeaderText = "Product";
            this.Product.Name = "Product";
            this.Product.Width = 200;
            // 
            // Version
            // 
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            this.Version.Width = 200;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.Width = 500;
            // 
            // btnRefreshServerView
            // 
            this.btnRefreshServerView.Location = new System.Drawing.Point(0, 454);
            this.btnRefreshServerView.Name = "btnRefreshServerView";
            this.btnRefreshServerView.Size = new System.Drawing.Size(75, 21);
            this.btnRefreshServerView.TabIndex = 5;
            this.btnRefreshServerView.Text = "Refresh";
            this.btnRefreshServerView.UseVisualStyleBackColor = true;
            this.btnRefreshServerView.Click += new System.EventHandler(this.btnRefreshServerView_Click);
            // 
            // tPageStore
            // 
            this.tPageStore.Controls.Add(this.btnFetchJobCount);
            this.tPageStore.Controls.Add(this.lblPendingJobCount);
            this.tPageStore.Controls.Add(this.dataGridViewHistory);
            this.tPageStore.Controls.Add(this.tbPostTestResponse);
            this.tPageStore.Controls.Add(this.btnRefreshHistoryView);
            this.tPageStore.Controls.Add(this.label2);
            this.tPageStore.Controls.Add(this.tbStoreAddress);
            this.tPageStore.Controls.Add(this.label1);
            this.tPageStore.Location = new System.Drawing.Point(4, 22);
            this.tPageStore.Name = "tPageStore";
            this.tPageStore.Padding = new System.Windows.Forms.Padding(3);
            this.tPageStore.Size = new System.Drawing.Size(909, 480);
            this.tPageStore.TabIndex = 1;
            this.tPageStore.Text = "Store Information";
            this.tPageStore.UseVisualStyleBackColor = true;
            // 
            // btnFetchJobCount
            // 
            this.btnFetchJobCount.Location = new System.Drawing.Point(276, 37);
            this.btnFetchJobCount.Name = "btnFetchJobCount";
            this.btnFetchJobCount.Size = new System.Drawing.Size(82, 22);
            this.btnFetchJobCount.TabIndex = 12;
            this.btnFetchJobCount.Text = "Pending Jobs:";
            this.btnFetchJobCount.UseVisualStyleBackColor = true;
            this.btnFetchJobCount.Click += new System.EventHandler(this.btnFetchJobCount_Click);
            // 
            // lblPendingJobCount
            // 
            this.lblPendingJobCount.AutoSize = true;
            this.lblPendingJobCount.Location = new System.Drawing.Point(364, 42);
            this.lblPendingJobCount.Name = "lblPendingJobCount";
            this.lblPendingJobCount.Size = new System.Drawing.Size(13, 13);
            this.lblPendingJobCount.TabIndex = 11;
            this.lblPendingJobCount.Text = "0";
            // 
            // dataGridViewHistory
            // 
            this.dataGridViewHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
            this.dataGridViewHistory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewHistory.Location = new System.Drawing.Point(3, 65);
            this.dataGridViewHistory.MultiSelect = false;
            this.dataGridViewHistory.Name = "dataGridViewHistory";
            this.dataGridViewHistory.Size = new System.Drawing.Size(901, 285);
            this.dataGridViewHistory.TabIndex = 9;
            this.dataGridViewHistory.MouseEnter += new System.EventHandler(this.dataGridViewHistory_MouseEnter);
            this.dataGridViewHistory.MouseLeave += new System.EventHandler(this.dataGridViewHistory_MouseLeave);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Type";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 40;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Pointer";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 40;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Date";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Time";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 60;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Product";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 200;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "Version";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 200;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Comment";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Width = 500;
            // 
            // tbPostTestResponse
            // 
            this.tbPostTestResponse.Location = new System.Drawing.Point(6, 383);
            this.tbPostTestResponse.Multiline = true;
            this.tbPostTestResponse.Name = "tbPostTestResponse";
            this.tbPostTestResponse.ReadOnly = true;
            this.tbPostTestResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPostTestResponse.Size = new System.Drawing.Size(901, 94);
            this.tbPostTestResponse.TabIndex = 6;
            this.tbPostTestResponse.MouseEnter += new System.EventHandler(this.tbPostTestResponse_MouseEnter);
            this.tbPostTestResponse.MouseLeave += new System.EventHandler(this.tbPostTestResponse_MouseLeave);
            // 
            // btnRefreshHistoryView
            // 
            this.btnRefreshHistoryView.Location = new System.Drawing.Point(6, 356);
            this.btnRefreshHistoryView.Name = "btnRefreshHistoryView";
            this.btnRefreshHistoryView.Size = new System.Drawing.Size(75, 21);
            this.btnRefreshHistoryView.TabIndex = 4;
            this.btnRefreshHistoryView.Text = "Refresh";
            this.btnRefreshHistoryView.UseVisualStyleBackColor = true;
            this.btnRefreshHistoryView.Click += new System.EventHandler(this.btnRefreshHistoryView_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Transaction Listing";
            // 
            // tbStoreAddress
            // 
            this.tbStoreAddress.Location = new System.Drawing.Point(3, 23);
            this.tbStoreAddress.Name = "tbStoreAddress";
            this.tbStoreAddress.Size = new System.Drawing.Size(264, 20);
            this.tbStoreAddress.TabIndex = 1;
            this.tbStoreAddress.Text = "10.10.22.127";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Symbol Store Address";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 506);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "Form1";
            this.Text = "Symbol Store Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.tabControl1.ResumeLayout(false);
            this.tPageVersion.ResumeLayout(false);
            this.tPageVersion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewServer)).EndInit();
            this.tPageStore.ResumeLayout(false);
            this.tPageStore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tPageVersion;
        private System.Windows.Forms.TabPage tPageStore;
        private System.Windows.Forms.TextBox tbStoreAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefreshHistoryView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPostTestResponse;
        private System.Windows.Forms.Button btnRefreshServerView;
        private System.Windows.Forms.DataGridView dataGridViewServer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Transaction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pointer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Product;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridView dataGridViewHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnFetchJobCount;
        private System.Windows.Forms.Label lblPendingJobCount;
    }
}

