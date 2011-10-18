namespace Kontakt.Statistics
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
            this.cbMonth = new System.Windows.Forms.ComboBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.dgvResult2 = new System.Windows.Forms.DataGridView();
            this.tbFilter1 = new System.Windows.Forms.RichTextBox();
            this.cbFilters2 = new System.Windows.Forms.ComboBox();
            this.btnGenerateExcel = new System.Windows.Forms.Button();
            this.cbStatistics = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMonth
            // 
            this.cbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonth.FormattingEnabled = true;
            this.cbMonth.Location = new System.Drawing.Point(406, 12);
            this.cbMonth.Name = "cbMonth";
            this.cbMonth.Size = new System.Drawing.Size(121, 21);
            this.cbMonth.TabIndex = 3;
            this.cbMonth.Visible = false;
            // 
            // dgvDetails
            // 
            this.dgvDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Location = new System.Drawing.Point(12, 459);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.ReadOnly = true;
            this.dgvDetails.Size = new System.Drawing.Size(867, 109);
            this.dgvDetails.TabIndex = 4;
            // 
            // dgvResult2
            // 
            this.dgvResult2.AllowUserToAddRows = false;
            this.dgvResult2.AllowUserToDeleteRows = false;
            this.dgvResult2.AllowUserToOrderColumns = true;
            this.dgvResult2.AllowUserToResizeRows = false;
            this.dgvResult2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResult2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult2.Location = new System.Drawing.Point(12, 104);
            this.dgvResult2.Name = "dgvResult2";
            this.dgvResult2.ReadOnly = true;
            this.dgvResult2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResult2.Size = new System.Drawing.Size(867, 349);
            this.dgvResult2.TabIndex = 8;
            // 
            // tbFilter1
            // 
            this.tbFilter1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilter1.Location = new System.Drawing.Point(12, 39);
            this.tbFilter1.Name = "tbFilter1";
            this.tbFilter1.ReadOnly = true;
            this.tbFilter1.Size = new System.Drawing.Size(867, 59);
            this.tbFilter1.TabIndex = 7;
            this.tbFilter1.Text = "";
            // 
            // cbFilters2
            // 
            this.cbFilters2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilters2.FormattingEnabled = true;
            this.cbFilters2.Location = new System.Drawing.Point(12, 12);
            this.cbFilters2.Name = "cbFilters2";
            this.cbFilters2.Size = new System.Drawing.Size(388, 21);
            this.cbFilters2.TabIndex = 6;
            // 
            // btnGenerateExcel
            // 
            this.btnGenerateExcel.Location = new System.Drawing.Point(804, 10);
            this.btnGenerateExcel.Name = "btnGenerateExcel";
            this.btnGenerateExcel.Size = new System.Drawing.Size(75, 23);
            this.btnGenerateExcel.TabIndex = 9;
            this.btnGenerateExcel.Text = "Generate";
            this.btnGenerateExcel.UseVisualStyleBackColor = true;
            this.btnGenerateExcel.Click += new System.EventHandler(this.btnGenerateExcel_Click);
            // 
            // cbStatistics
            // 
            this.cbStatistics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatistics.FormattingEnabled = true;
            this.cbStatistics.Location = new System.Drawing.Point(533, 12);
            this.cbStatistics.Name = "cbStatistics";
            this.cbStatistics.Size = new System.Drawing.Size(265, 21);
            this.cbStatistics.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 580);
            this.Controls.Add(this.cbStatistics);
            this.Controls.Add(this.btnGenerateExcel);
            this.Controls.Add(this.dgvResult2);
            this.Controls.Add(this.tbFilter1);
            this.Controls.Add(this.cbFilters2);
            this.Controls.Add(this.dgvDetails);
            this.Controls.Add(this.cbMonth);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMonth;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.DataGridView dgvResult2;
        private System.Windows.Forms.RichTextBox tbFilter1;
        private System.Windows.Forms.ComboBox cbFilters2;
        private System.Windows.Forms.Button btnGenerateExcel;
        private System.Windows.Forms.ComboBox cbStatistics;
    }
}

