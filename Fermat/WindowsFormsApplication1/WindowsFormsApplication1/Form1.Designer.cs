namespace WindowsFormsApplication1
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
            this.input = new System.Windows.Forms.TextBox();
            this.ktests = new System.Windows.Forms.TextBox();
            this.output = new System.Windows.Forms.TextBox();
            this.solve = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // input
            // 
            this.input.Location = new System.Drawing.Point(125, 24);
            this.input.Name = "input";
            this.input.Size = new System.Drawing.Size(198, 20);
            this.input.TabIndex = 0;
            // 
            // ktests
            // 
            this.ktests.Location = new System.Drawing.Point(125, 63);
            this.ktests.Name = "ktests";
            this.ktests.Size = new System.Drawing.Size(198, 20);
            this.ktests.TabIndex = 1;
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(125, 98);
            this.output.Multiline = true;
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(198, 88);
            this.output.TabIndex = 2;
            // 
            // solve
            // 
            this.solve.Location = new System.Drawing.Point(248, 208);
            this.solve.Name = "solve";
            this.solve.Size = new System.Drawing.Size(75, 23);
            this.solve.TabIndex = 3;
            this.solve.Text = "Solve";
            this.solve.UseVisualStyleBackColor = true;
            this.solve.Click += new System.EventHandler(this.solve_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Input";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Number of Tests";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 245);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.solve);
            this.Controls.Add(this.output);
            this.Controls.Add(this.ktests);
            this.Controls.Add(this.input);
            this.Name = "Form1";
            this.Text = "Primality Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input;
        private System.Windows.Forms.TextBox ktests;
        private System.Windows.Forms.TextBox output;
        private System.Windows.Forms.Button solve;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

