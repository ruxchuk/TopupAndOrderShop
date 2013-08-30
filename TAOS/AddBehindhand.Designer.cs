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
            this.txtCustomerName = new DevExpress.XtraEditors.TextEdit();
            this.txtValueBaht = new DevExpress.XtraEditors.TextEdit();
            this.btnAddBehindhand = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(131, 9);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            this.txtCustomerName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomerName.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtCustomerName.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerName.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerName.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerName.Size = new System.Drawing.Size(180, 40);
            this.txtCustomerName.TabIndex = 1;
            this.txtCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerName_KeyDown);
            // 
            // txtValueBaht
            // 
            this.txtValueBaht.CausesValidation = false;
            this.txtValueBaht.EditValue = "";
            this.txtValueBaht.Location = new System.Drawing.Point(131, 57);
            this.txtValueBaht.Name = "txtValueBaht";
            this.txtValueBaht.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            this.txtValueBaht.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValueBaht.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtValueBaht.Properties.Appearance.Options.UseBackColor = true;
            this.txtValueBaht.Properties.Appearance.Options.UseFont = true;
            this.txtValueBaht.Properties.Appearance.Options.UseForeColor = true;
            this.txtValueBaht.Size = new System.Drawing.Size(108, 40);
            this.txtValueBaht.TabIndex = 2;
            this.txtValueBaht.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValueBaht_KeyDown);
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
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label24.AutoSize = true;
            this.label24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(12, 64);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(105, 27);
            this.label24.TabIndex = 45;
            this.label24.Text = "จำนวนเงิน";
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label25.AutoSize = true;
            this.label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(21, 16);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(85, 27);
            this.label25.TabIndex = 44;
            this.label25.Text = "ชื่อลูกค้า";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(261, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 27);
            this.label1.TabIndex = 45;
            this.label1.Text = "บาท";
            // 
            // AddBehindhand
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 106);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.btnAddBehindhand);
            this.Controls.Add(this.txtValueBaht);
            this.Controls.Add(this.txtCustomerName);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(410, 145);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(410, 145);
            this.Name = "AddBehindhand";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "เพิ่มค้้างชำระ";
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtCustomerName;
        public DevExpress.XtraEditors.TextEdit txtValueBaht;
        private System.Windows.Forms.Button btnAddBehindhand;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label1;
    }
}