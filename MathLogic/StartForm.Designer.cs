namespace MathLogic
{
    partial class StartForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.alphabetLabel = new System.Windows.Forms.Label();
			this.permutsLabel = new System.Windows.Forms.Label();
			this.alphabetListBox = new System.Windows.Forms.ListBox();
			this.permutsListBox = new System.Windows.Forms.ListBox();
			this.addAlphabetButton = new System.Windows.Forms.Button();
			this.deleteAlphabetButton = new System.Windows.Forms.Button();
			this.addPermutsButton = new System.Windows.Forms.Button();
			this.deletePermutsButton = new System.Windows.Forms.Button();
			this.inputLabel = new System.Windows.Forms.Label();
			this.inputTextBox = new System.Windows.Forms.TextBox();
			this.startButton = new System.Windows.Forms.Button();
			this.stepsDataGridView = new System.Windows.Forms.DataGridView();
			this.idColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fromColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.главноеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.импортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.экспортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.stepsDataGridView)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// alphabetLabel
			// 
			this.alphabetLabel.AutoSize = true;
			this.alphabetLabel.Location = new System.Drawing.Point(54, 34);
			this.alphabetLabel.Name = "alphabetLabel";
			this.alphabetLabel.Size = new System.Drawing.Size(51, 13);
			this.alphabetLabel.TabIndex = 0;
			this.alphabetLabel.Text = "Алфавит";
			// 
			// permutsLabel
			// 
			this.permutsLabel.AutoSize = true;
			this.permutsLabel.Location = new System.Drawing.Point(307, 34);
			this.permutsLabel.Name = "permutsLabel";
			this.permutsLabel.Size = new System.Drawing.Size(48, 13);
			this.permutsLabel.TabIndex = 1;
			this.permutsLabel.Text = "Замены";
			// 
			// alphabetListBox
			// 
			this.alphabetListBox.FormattingEnabled = true;
			this.alphabetListBox.Location = new System.Drawing.Point(46, 51);
			this.alphabetListBox.Name = "alphabetListBox";
			this.alphabetListBox.Size = new System.Drawing.Size(120, 121);
			this.alphabetListBox.TabIndex = 2;
			this.alphabetListBox.SelectedIndexChanged += new System.EventHandler(this.alphabetListBox_SelectedIndexChanged);
			this.alphabetListBox.DoubleClick += new System.EventHandler(this.alphabetListBox_DoubleClick);
			// 
			// permutsListBox
			// 
			this.permutsListBox.FormattingEnabled = true;
			this.permutsListBox.Location = new System.Drawing.Point(297, 51);
			this.permutsListBox.Name = "permutsListBox";
			this.permutsListBox.Size = new System.Drawing.Size(120, 121);
			this.permutsListBox.TabIndex = 3;
			this.permutsListBox.SelectedIndexChanged += new System.EventHandler(this.permutsListBox_SelectedIndexChanged);
			this.permutsListBox.DoubleClick += new System.EventHandler(this.permutsListBox_DoubleClick);
			// 
			// addAlphabetButton
			// 
			this.addAlphabetButton.Location = new System.Drawing.Point(66, 178);
			this.addAlphabetButton.Name = "addAlphabetButton";
			this.addAlphabetButton.Size = new System.Drawing.Size(75, 23);
			this.addAlphabetButton.TabIndex = 4;
			this.addAlphabetButton.Text = "Добавить";
			this.addAlphabetButton.UseVisualStyleBackColor = true;
			this.addAlphabetButton.Click += new System.EventHandler(this.addAlphabetButton_Click);
			// 
			// deleteAlphabetButton
			// 
			this.deleteAlphabetButton.Enabled = false;
			this.deleteAlphabetButton.Location = new System.Drawing.Point(66, 207);
			this.deleteAlphabetButton.Name = "deleteAlphabetButton";
			this.deleteAlphabetButton.Size = new System.Drawing.Size(75, 23);
			this.deleteAlphabetButton.TabIndex = 5;
			this.deleteAlphabetButton.Text = "Удалить";
			this.deleteAlphabetButton.UseVisualStyleBackColor = true;
			this.deleteAlphabetButton.Click += new System.EventHandler(this.deleteAlphabetButton_Click);
			// 
			// addPermutsButton
			// 
			this.addPermutsButton.Location = new System.Drawing.Point(319, 178);
			this.addPermutsButton.Name = "addPermutsButton";
			this.addPermutsButton.Size = new System.Drawing.Size(75, 23);
			this.addPermutsButton.TabIndex = 6;
			this.addPermutsButton.Text = "Добавить";
			this.addPermutsButton.UseVisualStyleBackColor = true;
			this.addPermutsButton.Click += new System.EventHandler(this.addPermutsButton_Click);
			// 
			// deletePermutsButton
			// 
			this.deletePermutsButton.Enabled = false;
			this.deletePermutsButton.Location = new System.Drawing.Point(319, 207);
			this.deletePermutsButton.Name = "deletePermutsButton";
			this.deletePermutsButton.Size = new System.Drawing.Size(75, 23);
			this.deletePermutsButton.TabIndex = 7;
			this.deletePermutsButton.Text = "Удалить";
			this.deletePermutsButton.UseVisualStyleBackColor = true;
			this.deletePermutsButton.Click += new System.EventHandler(this.deletePermutsButton_Click);
			// 
			// inputLabel
			// 
			this.inputLabel.AutoSize = true;
			this.inputLabel.Location = new System.Drawing.Point(11, 245);
			this.inputLabel.Name = "inputLabel";
			this.inputLabel.Size = new System.Drawing.Size(37, 13);
			this.inputLabel.TabIndex = 8;
			this.inputLabel.Text = "Текст";
			// 
			// inputTextBox
			// 
			this.inputTextBox.AcceptsReturn = true;
			this.inputTextBox.Location = new System.Drawing.Point(54, 242);
			this.inputTextBox.Multiline = true;
			this.inputTextBox.Name = "inputTextBox";
			this.inputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.inputTextBox.Size = new System.Drawing.Size(392, 57);
			this.inputTextBox.TabIndex = 9;
			this.inputTextBox.TextChanged += new System.EventHandler(this.inputTextBox_TextChanged);
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(159, 305);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(154, 23);
			this.startButton.TabIndex = 10;
			this.startButton.Text = "Начать!";
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stepsDataGridView
			// 
			this.stepsDataGridView.AllowUserToAddRows = false;
			this.stepsDataGridView.AllowUserToDeleteRows = false;
			this.stepsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
			this.stepsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.stepsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idColumn,
            this.fromColumn,
            this.toColumn});
			this.stepsDataGridView.Location = new System.Drawing.Point(11, 334);
			this.stepsDataGridView.Name = "stepsDataGridView";
			this.stepsDataGridView.ReadOnly = true;
			this.stepsDataGridView.RowHeadersVisible = false;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.stepsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
			this.stepsDataGridView.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.stepsDataGridView.RowTemplate.ReadOnly = true;
			this.stepsDataGridView.ShowEditingIcon = false;
			this.stepsDataGridView.Size = new System.Drawing.Size(435, 150);
			this.stepsDataGridView.TabIndex = 11;
			// 
			// idColumn
			// 
			this.idColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.idColumn.Frozen = true;
			this.idColumn.HeaderText = "Шаг";
			this.idColumn.Name = "idColumn";
			this.idColumn.ReadOnly = true;
			this.idColumn.Width = 52;
			// 
			// fromColumn
			// 
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.fromColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.fromColumn.Frozen = true;
			this.fromColumn.HeaderText = "Было";
			this.fromColumn.Name = "fromColumn";
			this.fromColumn.ReadOnly = true;
			this.fromColumn.Width = 182;
			// 
			// toColumn
			// 
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.toColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.toColumn.Frozen = true;
			this.toColumn.HeaderText = "Стало";
			this.toColumn.Name = "toColumn";
			this.toColumn.ReadOnly = true;
			this.toColumn.Width = 182;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.главноеToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip.Size = new System.Drawing.Size(459, 24);
			this.menuStrip.TabIndex = 12;
			this.menuStrip.Text = "menuStrip1";
			// 
			// главноеToolStripMenuItem
			// 
			this.главноеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.импортToolStripMenuItem,
            this.экспортToolStripMenuItem,
            this.выходToolStripMenuItem});
			this.главноеToolStripMenuItem.Name = "главноеToolStripMenuItem";
			this.главноеToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.главноеToolStripMenuItem.Text = "Главное";
			// 
			// импортToolStripMenuItem
			// 
			this.импортToolStripMenuItem.Name = "импортToolStripMenuItem";
			this.импортToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.импортToolStripMenuItem.Text = "Импорт";
			this.импортToolStripMenuItem.Click += new System.EventHandler(this.импортToolStripMenuItem_Click);
			// 
			// экспортToolStripMenuItem
			// 
			this.экспортToolStripMenuItem.Name = "экспортToolStripMenuItem";
			this.экспортToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.экспортToolStripMenuItem.Text = "Экспорт";
			this.экспортToolStripMenuItem.Click += new System.EventHandler(this.экспортToolStripMenuItem_Click);
			// 
			// выходToolStripMenuItem
			// 
			this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
			this.выходToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.выходToolStripMenuItem.Text = "Выход";
			this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
			// 
			// StartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(459, 496);
			this.Controls.Add(this.stepsDataGridView);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.inputTextBox);
			this.Controls.Add(this.inputLabel);
			this.Controls.Add(this.deletePermutsButton);
			this.Controls.Add(this.addPermutsButton);
			this.Controls.Add(this.deleteAlphabetButton);
			this.Controls.Add(this.addAlphabetButton);
			this.Controls.Add(this.permutsListBox);
			this.Controls.Add(this.alphabetListBox);
			this.Controls.Add(this.permutsLabel);
			this.Controls.Add(this.alphabetLabel);
			this.Controls.Add(this.menuStrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainMenuStrip = this.menuStrip;
			this.MaximizeBox = false;
			this.Name = "StartForm";
			this.Text = "Алгоритмы Маркова";
			((System.ComponentModel.ISupportInitialize)(this.stepsDataGridView)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label alphabetLabel;
        private System.Windows.Forms.Label permutsLabel;
        private System.Windows.Forms.Label inputLabel;
		private System.Windows.Forms.DataGridViewTextBoxColumn idColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn fromColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn toColumn;
		public System.Windows.Forms.TextBox inputTextBox;
		public System.Windows.Forms.ListBox alphabetListBox;
		public System.Windows.Forms.ListBox permutsListBox;
		public System.Windows.Forms.Button addAlphabetButton;
		public System.Windows.Forms.Button deleteAlphabetButton;
		public System.Windows.Forms.Button addPermutsButton;
		public System.Windows.Forms.Button deletePermutsButton;
		public System.Windows.Forms.Button startButton;
		public System.Windows.Forms.DataGridView stepsDataGridView;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem главноеToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem импортToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem экспортToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
	}
}

