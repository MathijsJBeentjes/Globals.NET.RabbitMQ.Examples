namespace VoteFormServer
{
    partial class VoteFormServer
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
            if (disposing)
            {
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
            this.lbYesCnt = new System.Windows.Forms.Label();
            this.lbNoCnt = new System.Windows.Forms.Label();
            this.lbNo = new System.Windows.Forms.Label();
            this.lbYes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbYesCnt
            // 
            this.lbYesCnt.AutoSize = true;
            this.lbYesCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbYesCnt.Location = new System.Drawing.Point(136, 9);
            this.lbYesCnt.Name = "lbYesCnt";
            this.lbYesCnt.Size = new System.Drawing.Size(25, 25);
            this.lbYesCnt.TabIndex = 9;
            this.lbYesCnt.Text = "0";
            // 
            // lbNoCnt
            // 
            this.lbNoCnt.AutoSize = true;
            this.lbNoCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNoCnt.Location = new System.Drawing.Point(136, 34);
            this.lbNoCnt.Name = "lbNoCnt";
            this.lbNoCnt.Size = new System.Drawing.Size(25, 25);
            this.lbNoCnt.TabIndex = 8;
            this.lbNoCnt.Text = "0";
            // 
            // lbNo
            // 
            this.lbNo.AutoSize = true;
            this.lbNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNo.Location = new System.Drawing.Point(64, 34);
            this.lbNo.Name = "lbNo";
            this.lbNo.Size = new System.Drawing.Size(41, 25);
            this.lbNo.TabIndex = 7;
            this.lbNo.Text = "No";
            // 
            // lbYes
            // 
            this.lbYes.AutoSize = true;
            this.lbYes.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbYes.Location = new System.Drawing.Point(64, 9);
            this.lbYes.Name = "lbYes";
            this.lbYes.Size = new System.Drawing.Size(53, 25);
            this.lbYes.TabIndex = 6;
            this.lbYes.Text = "Yes";
            // 
            // VoteFormServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 73);
            this.Controls.Add(this.lbYesCnt);
            this.Controls.Add(this.lbNoCnt);
            this.Controls.Add(this.lbNo);
            this.Controls.Add(this.lbYes);
            this.Name = "VoteFormServer";
            this.Text = "Vote Form Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VoteFormServer_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbYesCnt;
        private System.Windows.Forms.Label lbNoCnt;
        private System.Windows.Forms.Label lbNo;
        private System.Windows.Forms.Label lbYes;
    }
}

