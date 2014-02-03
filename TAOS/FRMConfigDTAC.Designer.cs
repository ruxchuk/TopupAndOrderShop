namespace TAOS
{
    partial class FRMConfigDTAC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMConfigDTAC));
            this.label40 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.topupCode = new System.Windows.Forms.RichTextBox();
            this.returnCode = new System.Windows.Forms.RichTextBox();
            this.tbxPort = new DevExpress.XtraEditors.TextEdit();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxImei = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxImei.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label40
            // 
            this.label40.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label40.ForeColor = System.Drawing.Color.White;
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(12, 221);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(62, 25);
            this.label40.TabIndex = 38;
            this.label40.Text = "Port:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 25);
            this.label1.TabIndex = 38;
            this.label1.Text = "รหัสดึงเงินคืน:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 25);
            this.label2.TabIndex = 38;
            this.label2.Text = "รหัสเติมเงิน:";
            // 
            // topupCode
            // 
            this.topupCode.Location = new System.Drawing.Point(164, 9);
            this.topupCode.Name = "topupCode";
            this.topupCode.Size = new System.Drawing.Size(205, 61);
            this.topupCode.TabIndex = 39;
            this.topupCode.Text = "";
            // 
            // returnCode
            // 
            this.returnCode.Location = new System.Drawing.Point(164, 90);
            this.returnCode.Name = "returnCode";
            this.returnCode.Size = new System.Drawing.Size(205, 61);
            this.returnCode.TabIndex = 39;
            this.returnCode.Text = "";
            // 
            // tbxPort
            // 
            this.tbxPort.CausesValidation = false;
            this.tbxPort.EditValue = "No Port";
            this.tbxPort.Enabled = false;
            this.tbxPort.Location = new System.Drawing.Point(164, 221);
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tbxPort.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxPort.Properties.Appearance.ForeColor = System.Drawing.Color.Yellow;
            this.tbxPort.Properties.Appearance.Options.UseBackColor = true;
            this.tbxPort.Properties.Appearance.Options.UseFont = true;
            this.tbxPort.Properties.Appearance.Options.UseForeColor = true;
            this.tbxPort.Size = new System.Drawing.Size(208, 32);
            this.tbxPort.TabIndex = 40;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Transparent;
            this.btnConnect.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConnect.BackgroundImage")));
            this.btnConnect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConnect.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(164, 266);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(101, 33);
            this.btnConnect.TabIndex = 41;
            this.btnConnect.Text = "   Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(271, 266);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 33);
            this.btnSave.TabIndex = 41;
            this.btnSave.Text = "      Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(12, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 25);
            this.label3.TabIndex = 38;
            this.label3.Text = "IMEI No.:";
            // 
            // tbxImei
            // 
            this.tbxImei.CausesValidation = false;
            this.tbxImei.EditValue = "imei";
            this.tbxImei.Enabled = false;
            this.tbxImei.Location = new System.Drawing.Point(164, 171);
            this.tbxImei.Name = "tbxImei";
            this.tbxImei.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tbxImei.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxImei.Properties.Appearance.ForeColor = System.Drawing.Color.Yellow;
            this.tbxImei.Properties.Appearance.Options.UseBackColor = true;
            this.tbxImei.Properties.Appearance.Options.UseFont = true;
            this.tbxImei.Properties.Appearance.Options.UseForeColor = true;
            this.tbxImei.Size = new System.Drawing.Size(208, 32);
            this.tbxImei.TabIndex = 40;
            // 
            // FRMConfigDTAC
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(141)))), ((int)(((byte)(214)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 311);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tbxImei);
            this.Controls.Add(this.tbxPort);
            this.Controls.Add(this.returnCode);
            this.Controls.Add(this.topupCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label40);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 350);
            this.Name = "FRMConfigDTAC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ตั้งค่า DTAC";
            ((System.ComponentModel.ISupportInitialize)(this.tbxPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxImei.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox topupCode;
        private System.Windows.Forms.RichTextBox returnCode;
        public DevExpress.XtraEditors.TextEdit tbxPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label3;
        public DevExpress.XtraEditors.TextEdit tbxImei;
    }
}