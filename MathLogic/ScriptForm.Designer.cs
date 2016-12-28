namespace MathLogic
{
    partial class ScriptForm
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
			this.scriptRichTextBox = new System.Windows.Forms.RichTextBox();
			this.executeButton = new System.Windows.Forms.Button();
			this.checkButton = new System.Windows.Forms.Button();
			this.helpButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// scriptRichTextBox
			// 
			this.scriptRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scriptRichTextBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.scriptRichTextBox.Location = new System.Drawing.Point(0, 0);
			this.scriptRichTextBox.Name = "scriptRichTextBox";
			this.scriptRichTextBox.Size = new System.Drawing.Size(516, 290);
			this.scriptRichTextBox.TabIndex = 0;
			this.scriptRichTextBox.Text = "";
			// 
			// executeButton
			// 
			this.executeButton.Location = new System.Drawing.Point(116, 296);
			this.executeButton.Name = "executeButton";
			this.executeButton.Size = new System.Drawing.Size(75, 23);
			this.executeButton.TabIndex = 1;
			this.executeButton.Text = "Выполнить";
			this.executeButton.UseVisualStyleBackColor = true;
			this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
			// 
			// checkButton
			// 
			this.checkButton.Location = new System.Drawing.Point(213, 296);
			this.checkButton.Name = "checkButton";
			this.checkButton.Size = new System.Drawing.Size(75, 23);
			this.checkButton.TabIndex = 2;
			this.checkButton.Text = "Проверить";
			this.checkButton.UseVisualStyleBackColor = true;
			this.checkButton.Click += new System.EventHandler(this.checkButton_Click);
			// 
			// helpButton
			// 
			this.helpButton.Location = new System.Drawing.Point(307, 296);
			this.helpButton.Name = "helpButton";
			this.helpButton.Size = new System.Drawing.Size(75, 23);
			this.helpButton.TabIndex = 3;
			this.helpButton.Text = "Помощь";
			this.helpButton.UseVisualStyleBackColor = true;
			// 
			// ScriptForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(516, 340);
			this.Controls.Add(this.helpButton);
			this.Controls.Add(this.checkButton);
			this.Controls.Add(this.executeButton);
			this.Controls.Add(this.scriptRichTextBox);
			this.Name = "ScriptForm";
			this.Text = "ScriptForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptForm_FormClosing);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox scriptRichTextBox;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Button helpButton;
    }
}