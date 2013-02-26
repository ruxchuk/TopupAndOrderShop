namespace TAOS
{
    partial class AddBehindhand
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddBehindhand));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerName = new DevExpress.XtraEditors.TextEdit();
            this.txtValueBaht = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnAddBehindhand = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Location = new System.Drawing.Point(12, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(85, 29);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "ชื่อลูกค้า";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(131, 10);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            this.txtCustomerName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerName.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtCustomerName.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerName.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerName.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerName.Size = new System.Drawing.Size(160, 40);
            this.txtCustomerName.TabIndex = 1;
            // 
            // txtValueBaht
            // 
            this.txtValueBaht.CausesValidation = false;
            this.txtValueBaht.EditValue = "";
            this.txtValueBaht.Location = new System.Drawing.Point(131, 56);
            this.txtValueBaht.Name = "txtValueBaht";
            this.txtValueBaht.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            this.txtValueBaht.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValueBaht.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtValueBaht.Properties.Appearance.Options.UseBackColor = true;
            this.txtValueBaht.Properties.Appearance.Options.UseFont = true;
            this.txtValueBaht.Properties.Appearance.Options.UseForeColor = true;
            this.txtValueBaht.Size = new System.Drawing.Size(108, 40);
            this.txtValueBaht.TabIndex = 2;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Location = new System.Drawing.Point(12, 62);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(102, 29);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "จำนวนเงิน";
            // 
            // btnAddBehindhand
            // 
            this.btnAddBehindhand.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddBehindhand.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAddBehindhand.BackgroundImage")));
            this.btnAddBehindhand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAddBehindhand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddBehindhand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddBehindhand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAddBehindhand.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddBehindhand.Location = new System.Drawing.Point(332, 49);
            this.btnAddBehindhand.Name = "btnAddBehindhand";
            this.btnAddBehindhand.Size = new System.Drawing.Size(51, 47);
            this.btnAddBehindhand.TabIndex = 3;
            this.btnAddBehindhand.UseVisualStyleBackColor = true;
            this.btnAddBehindhand.Click += new System.EventHandler(this.btnAddBehindhand_Click);
            // 
            // AddBehindhand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 107);
            this.Controls.Add(this.btnAddBehindhand);
            this.Controls.Add(this.txtValueBaht);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddBehindhand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "เพิ่มค้้างชำระ";
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtCustomerName;
        public DevExpress.XtraEditors.TextEdit txtValueBaht;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.Button btnAddBehindhand;
    }
}