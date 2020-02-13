namespace AutoChemist
{
    partial class Chemist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chemist));
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtMaterials = new System.Windows.Forms.TextBox();
            this.btnCalc = new System.Windows.Forms.Button();
            this.btnLogChemical = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtResult
            // 
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtResult.Enabled = false;
            this.txtResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(0, 535);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(784, 26);
            this.txtResult.TabIndex = 0;
            this.txtResult.Text = "Result is: ";
            // 
            // txtMaterials
            // 
            this.txtMaterials.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaterials.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMaterials.Location = new System.Drawing.Point(0, 515);
            this.txtMaterials.Name = "txtMaterials";
            this.txtMaterials.Size = new System.Drawing.Size(784, 20);
            this.txtMaterials.TabIndex = 1;
            this.txtMaterials.Text = "H2O2,KMnO4,O2,MnO2,KOH,H2O";
            // 
            // btnCalc
            // 
            this.btnCalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalc.Location = new System.Drawing.Point(638, 470);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(134, 45);
            this.btnCalc.TabIndex = 2;
            this.btnCalc.Text = "Auto Generate Balanced Equation";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // btnLogChemical
            // 
            this.btnLogChemical.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogChemical.Location = new System.Drawing.Point(12, 470);
            this.btnLogChemical.Name = "btnLogChemical";
            this.btnLogChemical.Size = new System.Drawing.Size(134, 45);
            this.btnLogChemical.TabIndex = 3;
            this.btnLogChemical.Text = "Log Chemical From Selection (1 at a time)";
            this.btnLogChemical.UseVisualStyleBackColor = true;
            this.btnLogChemical.Click += new System.EventHandler(this.btnLogChemical_Click);
            // 
            // Chemist
            // 
            this.AcceptButton = this.btnCalc;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnLogChemical);
            this.Controls.Add(this.btnCalc);
            this.Controls.Add(this.txtMaterials);
            this.Controls.Add(this.txtResult);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Chemist";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Chemist";
            this.Load += new System.EventHandler(this.Chemist_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Chemist_KeyUp);
            this.Resize += new System.EventHandler(this.Chemist_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtMaterials;
        private System.Windows.Forms.Button btnCalc;
        private System.Windows.Forms.Button btnLogChemical;
    }
}

