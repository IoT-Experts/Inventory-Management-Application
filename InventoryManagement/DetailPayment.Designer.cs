namespace InventoryManagement
{
    partial class DetailPayment
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
            this.label1 = new System.Windows.Forms.Label();
            this.numThouSend = new System.Windows.Forms.NumericUpDown();
            this.numFiveHundred = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numHundred = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numTen = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numTwenty = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numFifty = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numOne = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numTwo = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numFive = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.numTotal = new System.Windows.Forms.NumericUpDown();
            this.btnDoneDetailPayment = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numThouSend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFiveHundred)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHundred)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTwenty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFifty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTwo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotal)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "1000 X";
            // 
            // numThouSend
            // 
            this.numThouSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numThouSend.Location = new System.Drawing.Point(78, 17);
            this.numThouSend.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numThouSend.Name = "numThouSend";
            this.numThouSend.Size = new System.Drawing.Size(55, 26);
            this.numThouSend.TabIndex = 1;
            this.numThouSend.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // numFiveHundred
            // 
            this.numFiveHundred.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numFiveHundred.Location = new System.Drawing.Point(208, 17);
            this.numFiveHundred.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFiveHundred.Name = "numFiveHundred";
            this.numFiveHundred.Size = new System.Drawing.Size(55, 26);
            this.numFiveHundred.TabIndex = 3;
            this.numFiveHundred.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(151, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "500 X";
            // 
            // numHundred
            // 
            this.numHundred.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numHundred.Location = new System.Drawing.Point(345, 17);
            this.numHundred.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numHundred.Name = "numHundred";
            this.numHundred.Size = new System.Drawing.Size(55, 26);
            this.numHundred.TabIndex = 5;
            this.numHundred.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(288, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "100 X";
            // 
            // numTen
            // 
            this.numTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTen.Location = new System.Drawing.Point(345, 49);
            this.numTen.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numTen.Name = "numTen";
            this.numTen.Size = new System.Drawing.Size(55, 26);
            this.numTen.TabIndex = 11;
            this.numTen.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(297, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "10 X";
            // 
            // numTwenty
            // 
            this.numTwenty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTwenty.Location = new System.Drawing.Point(208, 49);
            this.numTwenty.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numTwenty.Name = "numTwenty";
            this.numTwenty.Size = new System.Drawing.Size(55, 26);
            this.numTwenty.TabIndex = 9;
            this.numTwenty.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(160, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "20 X";
            // 
            // numFifty
            // 
            this.numFifty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numFifty.Location = new System.Drawing.Point(78, 49);
            this.numFifty.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFifty.Name = "numFifty";
            this.numFifty.Size = new System.Drawing.Size(55, 26);
            this.numFifty.TabIndex = 7;
            this.numFifty.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(30, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "50 X";
            // 
            // numOne
            // 
            this.numOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numOne.Location = new System.Drawing.Point(345, 81);
            this.numOne.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numOne.Name = "numOne";
            this.numOne.Size = new System.Drawing.Size(55, 26);
            this.numOne.TabIndex = 17;
            this.numOne.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(306, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 20);
            this.label7.TabIndex = 16;
            this.label7.Text = "1 X";
            // 
            // numTwo
            // 
            this.numTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTwo.Location = new System.Drawing.Point(208, 81);
            this.numTwo.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numTwo.Name = "numTwo";
            this.numTwo.Size = new System.Drawing.Size(55, 26);
            this.numTwo.TabIndex = 15;
            this.numTwo.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(169, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 20);
            this.label8.TabIndex = 14;
            this.label8.Text = "2 X";
            // 
            // numFive
            // 
            this.numFive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numFive.Location = new System.Drawing.Point(78, 81);
            this.numFive.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numFive.Name = "numFive";
            this.numFive.Size = new System.Drawing.Size(55, 26);
            this.numFive.TabIndex = 13;
            this.numFive.ValueChanged += new System.EventHandler(this.amount_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(39, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 20);
            this.label9.TabIndex = 12;
            this.label9.Text = "5 X";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(30, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 20);
            this.label10.TabIndex = 18;
            this.label10.Text = "Total Payment :";
            // 
            // numTotal
            // 
            this.numTotal.DecimalPlaces = 2;
            this.numTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTotal.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numTotal.Location = new System.Drawing.Point(155, 136);
            this.numTotal.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numTotal.Name = "numTotal";
            this.numTotal.Size = new System.Drawing.Size(108, 26);
            this.numTotal.TabIndex = 19;
            // 
            // btnDoneDetailPayment
            // 
            this.btnDoneDetailPayment.BackColor = System.Drawing.Color.Tan;
            this.btnDoneDetailPayment.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoneDetailPayment.Location = new System.Drawing.Point(292, 113);
            this.btnDoneDetailPayment.Name = "btnDoneDetailPayment";
            this.btnDoneDetailPayment.Size = new System.Drawing.Size(108, 53);
            this.btnDoneDetailPayment.TabIndex = 20;
            this.btnDoneDetailPayment.Text = "Done";
            this.btnDoneDetailPayment.UseVisualStyleBackColor = false;
            this.btnDoneDetailPayment.Click += new System.EventHandler(this.btnDoneDetailPayment_Click);
            // 
            // DetailPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Beige;
            this.ClientSize = new System.Drawing.Size(419, 184);
            this.Controls.Add(this.btnDoneDetailPayment);
            this.Controls.Add(this.numTotal);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numOne);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numTwo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numFive);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.numTen);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numTwenty);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numFifty);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numHundred);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numFiveHundred);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numThouSend);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "DetailPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Detail Payment Information";
            ((System.ComponentModel.ISupportInitialize)(this.numThouSend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFiveHundred)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHundred)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTwenty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFifty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTwo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numThouSend;
        private System.Windows.Forms.NumericUpDown numFiveHundred;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numHundred;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numTen;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numTwenty;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numFifty;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numOne;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numTwo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numFive;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numTotal;
        private System.Windows.Forms.Button btnDoneDetailPayment;
    }
}