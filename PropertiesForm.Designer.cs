namespace ACSO
{
    partial class PropertiesForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.thresholdBox = new System.Windows.Forms.NumericUpDown();
            this.iterationsBox = new System.Windows.Forms.NumericUpDown();
            this.numberCitiesBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.startBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.knownOptimumBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.thresholdBox);
            this.panel1.Controls.Add(this.iterationsBox);
            this.panel1.Controls.Add(this.numberCitiesBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.startBtn);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.knownOptimumBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 201);
            this.panel1.TabIndex = 15;
            // 
            // thresholdBox
            // 
            this.thresholdBox.DecimalPlaces = 2;
            this.thresholdBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.thresholdBox.Location = new System.Drawing.Point(14, 103);
            this.thresholdBox.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.thresholdBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.thresholdBox.Name = "thresholdBox";
            this.thresholdBox.Size = new System.Drawing.Size(100, 20);
            this.thresholdBox.TabIndex = 12;
            this.thresholdBox.Value = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            // 
            // iterationsBox
            // 
            this.iterationsBox.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.iterationsBox.Location = new System.Drawing.Point(14, 64);
            this.iterationsBox.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.iterationsBox.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(100, 20);
            this.iterationsBox.TabIndex = 11;
            this.iterationsBox.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // numberCitiesBox
            // 
            this.numberCitiesBox.Location = new System.Drawing.Point(15, 25);
            this.numberCitiesBox.Name = "numberCitiesBox";
            this.numberCitiesBox.ReadOnly = true;
            this.numberCitiesBox.Size = new System.Drawing.Size(100, 20);
            this.numberCitiesBox.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Probability Threshold";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of Cities";
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(15, 168);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 8;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Known Optimum Solution (Optional)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Number of Iterations";
            // 
            // knownOptimumBox
            // 
            this.knownOptimumBox.Location = new System.Drawing.Point(14, 142);
            this.knownOptimumBox.Name = "knownOptimumBox";
            this.knownOptimumBox.Size = new System.Drawing.Size(100, 20);
            this.knownOptimumBox.TabIndex = 6;
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 201);
            this.Controls.Add(this.panel1);
            this.Name = "PropertiesForm";
            this.Text = "Properties";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox numberCitiesBox;
        public System.Windows.Forms.TextBox knownOptimumBox;
        public System.Windows.Forms.Button startBtn;
        public System.Windows.Forms.NumericUpDown iterationsBox;
        public System.Windows.Forms.NumericUpDown thresholdBox;
    }
}