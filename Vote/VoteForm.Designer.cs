namespace Vote
{
    partial class FormVote
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
            if (disposing)
            {
                if ((components != null))
                {
                    components.Dispose();
                }
                Yes.Dispose();
                TotalYes.Dispose();
                TotalNo.Dispose();
                ServerOnline.Dispose();
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
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.lbYes = new System.Windows.Forms.Label();
            this.lbNo = new System.Windows.Forms.Label();
            this.lbNoCnt = new System.Windows.Forms.Label();
            this.lbYesCnt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            this.btnYes.Enabled = false;
            this.btnYes.Location = new System.Drawing.Point(19, 65);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(112, 33);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.BtnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Enabled = false;
            this.btnNo.Location = new System.Drawing.Point(137, 65);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(112, 33);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.BtnNo_Click);
            // 
            // lbYes
            // 
            this.lbYes.AutoSize = true;
            this.lbYes.Location = new System.Drawing.Point(25, 12);
            this.lbYes.Name = "lbYes";
            this.lbYes.Size = new System.Drawing.Size(25, 13);
            this.lbYes.TabIndex = 2;
            this.lbYes.Text = "Yes";
            // 
            // lbNo
            // 
            this.lbNo.AutoSize = true;
            this.lbNo.Location = new System.Drawing.Point(25, 37);
            this.lbNo.Name = "lbNo";
            this.lbNo.Size = new System.Drawing.Size(21, 13);
            this.lbNo.TabIndex = 3;
            this.lbNo.Text = "No";
            // 
            // lbNoCnt
            // 
            this.lbNoCnt.AutoSize = true;
            this.lbNoCnt.Location = new System.Drawing.Point(56, 37);
            this.lbNoCnt.Name = "lbNoCnt";
            this.lbNoCnt.Size = new System.Drawing.Size(13, 13);
            this.lbNoCnt.TabIndex = 4;
            this.lbNoCnt.Text = "0";
            // 
            // lbYesCnt
            // 
            this.lbYesCnt.AutoSize = true;
            this.lbYesCnt.Location = new System.Drawing.Point(56, 12);
            this.lbYesCnt.Name = "lbYesCnt";
            this.lbYesCnt.Size = new System.Drawing.Size(13, 13);
            this.lbYesCnt.TabIndex = 5;
            this.lbYesCnt.Text = "0";
            // 
            // FormVote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 111);
            this.Controls.Add(this.lbYesCnt);
            this.Controls.Add(this.lbNoCnt);
            this.Controls.Add(this.lbNo);
            this.Controls.Add(this.lbYes);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormVote";
            this.Text = "Vote";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Label lbYes;
        private System.Windows.Forms.Label lbNo;
        private System.Windows.Forms.Label lbNoCnt;
        private System.Windows.Forms.Label lbYesCnt;
    }
}

