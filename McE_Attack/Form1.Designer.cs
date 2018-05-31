namespace McE_Attack
{
    partial class MainForm
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
            this.txtbox_keys = new System.Windows.Forms.TextBox();
            this.txtbox_unlock = new System.Windows.Forms.TextBox();
            this.btn_keygen = new System.Windows.Forms.Button();
            this.btn_attack = new System.Windows.Forms.Button();
            this.lbl_params = new System.Windows.Forms.Label();
            this.txtbox_cparams = new System.Windows.Forms.TextBox();
            this.cbox_cslist = new System.Windows.Forms.ComboBox();
            this.lbl_time = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtbox_keys
            // 
            this.txtbox_keys.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtbox_keys.Location = new System.Drawing.Point(328, 12);
            this.txtbox_keys.Multiline = true;
            this.txtbox_keys.Name = "txtbox_keys";
            this.txtbox_keys.ReadOnly = true;
            this.txtbox_keys.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbox_keys.Size = new System.Drawing.Size(1165, 391);
            this.txtbox_keys.TabIndex = 0;
            this.txtbox_keys.WordWrap = false;
            // 
            // txtbox_unlock
            // 
            this.txtbox_unlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtbox_unlock.Location = new System.Drawing.Point(328, 409);
            this.txtbox_unlock.Multiline = true;
            this.txtbox_unlock.Name = "txtbox_unlock";
            this.txtbox_unlock.ReadOnly = true;
            this.txtbox_unlock.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbox_unlock.Size = new System.Drawing.Size(1165, 408);
            this.txtbox_unlock.TabIndex = 1;
            this.txtbox_unlock.WordWrap = false;
            // 
            // btn_keygen
            // 
            this.btn_keygen.Location = new System.Drawing.Point(12, 112);
            this.btn_keygen.Name = "btn_keygen";
            this.btn_keygen.Size = new System.Drawing.Size(301, 49);
            this.btn_keygen.TabIndex = 2;
            this.btn_keygen.Text = "Генерировать ключи";
            this.btn_keygen.UseVisualStyleBackColor = true;
            this.btn_keygen.Click += new System.EventHandler(this.btn_keygen_Click);
            // 
            // btn_attack
            // 
            this.btn_attack.Location = new System.Drawing.Point(12, 167);
            this.btn_attack.Name = "btn_attack";
            this.btn_attack.Size = new System.Drawing.Size(301, 49);
            this.btn_attack.TabIndex = 4;
            this.btn_attack.Text = "Получить отмычку";
            this.btn_attack.UseVisualStyleBackColor = true;
            this.btn_attack.Click += new System.EventHandler(this.btn_attack_Click);
            // 
            // lbl_params
            // 
            this.lbl_params.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_params.Location = new System.Drawing.Point(8, 61);
            this.lbl_params.Name = "lbl_params";
            this.lbl_params.Size = new System.Drawing.Size(310, 20);
            this.lbl_params.TabIndex = 5;
            this.lbl_params.Text = "Параметры криптосистемы";
            this.lbl_params.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtbox_cparams
            // 
            this.txtbox_cparams.Location = new System.Drawing.Point(12, 84);
            this.txtbox_cparams.Name = "txtbox_cparams";
            this.txtbox_cparams.Size = new System.Drawing.Size(301, 22);
            this.txtbox_cparams.TabIndex = 6;
            // 
            // cbox_cslist
            // 
            this.cbox_cslist.FormattingEnabled = true;
            this.cbox_cslist.Items.AddRange(new object[] {
            "Hamming-McElice",
            "Hamming-McElice-Sid",
            "RM-McElice",
            "RM-McElice-Sid",
            "RM-McElice-Wish"});
            this.cbox_cslist.Location = new System.Drawing.Point(16, 13);
            this.cbox_cslist.Name = "cbox_cslist";
            this.cbox_cslist.Size = new System.Drawing.Size(297, 24);
            this.cbox_cslist.TabIndex = 7;
            this.cbox_cslist.SelectedIndexChanged += new System.EventHandler(this.cbox_cslist_SelectedIndexChanged);
            // 
            // lbl_time
            // 
            this.lbl_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_time.Location = new System.Drawing.Point(8, 245);
            this.lbl_time.Name = "lbl_time";
            this.lbl_time.Size = new System.Drawing.Size(310, 80);
            this.lbl_time.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1505, 829);
            this.Controls.Add(this.lbl_time);
            this.Controls.Add(this.cbox_cslist);
            this.Controls.Add(this.txtbox_cparams);
            this.Controls.Add(this.lbl_params);
            this.Controls.Add(this.btn_attack);
            this.Controls.Add(this.btn_keygen);
            this.Controls.Add(this.txtbox_unlock);
            this.Controls.Add(this.txtbox_keys);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Атаки на криптосистемы Мак-Элиса";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtbox_keys;
        public System.Windows.Forms.TextBox txtbox_unlock;
        public System.Windows.Forms.Button btn_keygen;
        public System.Windows.Forms.Button btn_attack;
        private System.Windows.Forms.Label lbl_params;
        private System.Windows.Forms.TextBox txtbox_cparams;
        private System.Windows.Forms.ComboBox cbox_cslist;
        private System.Windows.Forms.Label lbl_time;
    }
}

