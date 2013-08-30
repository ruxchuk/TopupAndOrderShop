namespace TAOS
{
    partial class FRMCustomerTopup
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
            this.txtValueBaht = new DevExpress.XtraEditors.TextEdit();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label22 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // txtValueBaht
            // 
            this.txtValueBaht.CausesValidation = false;
            this.txtValueBaht.EditValue = "";
            this.txtValueBaht.Location = new System.Drawing.Point(128, 18);
            this.txtValueBaht.Name = "txtValueBaht";
            this.txtValueBaht.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtValueBaht.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValueBaht.Properties.Appearance.ForeColor = System.Drawing.Color.Yellow;
            this.txtValueBaht.Properties.Appearance.Options.UseBackColor = true;
            this.txtValueBaht.Properties.Appearance.Options.UseFont = true;
            this.txtValueBaht.Properties.Appearance.Options.UseForeColor = true;
            this.txtValueBaht.Size = new System.Drawing.Size(108, 40);
            this.txtValueBaht.TabIndex = 1;
            this.txtValueBaht.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValueBaht_KeyDown);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.ErrorImage = null;
            this.pictureBox6.Image = global::TAOS.Properties.Resources.money;
            this.pictureBox6.ImageLocation = "";
            this.pictureBox6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox6.InitialImage = null;
            this.pictureBox6.Location = new System.Drawing.Point(25, 13);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(76, 51);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 35;
            this.pictureBox6.TabStop = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(258, 23);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 31);
            this.label22.TabIndex = 37;
            this.label22.Text = "บาท";
            // 
            // FRMCustomerTopup
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(163)))), ((int)(((byte)(53)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 77);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtValueBaht);
            this.Controls.Add(this.pictureBox6);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(355, 116);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(355, 116);
            this.Name = "FRMCustomerTopup";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ระบุจำนวนเงิน";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRMCustomerTopup_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.txtValueBaht.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.TextEdit txtValueBaht;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label22;

    }
}