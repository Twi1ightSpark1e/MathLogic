using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathLogic
{
	public partial class AddAlphabetForm : Form
	{
		public string Result { get; private set; }

		public AddAlphabetForm()
		{
			InitializeComponent();
			new ToolTip().SetToolTip(this.inputTextBox, "Символ без кавычек и \\, перечисление через ..");
		}

		public AddAlphabetForm(string text)
		{
			InitializeComponent();
			inputTextBox.Text = text;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Result = inputTextBox.Text;
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void inputTextBox_TextChanged(object sender, EventArgs e)
		{
			if (inputTextBox.Text.Length == 0)
				okButton.Enabled = false;
			else okButton.Enabled = true;
		}
	}
}
