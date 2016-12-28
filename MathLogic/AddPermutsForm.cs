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
	public partial class AddPermutsForm : Form
	{
		public string From { get; private set; }
		public string To { get; private set; }
		public bool IsFinal { get; private set; }


		public AddPermutsForm()
		{
			InitializeComponent();
			new ToolTip().SetToolTip(this.fromTextBox, "Символ/строка");
			new ToolTip().SetToolTip(this.toTextBox, "Символ/строка");
		}

		public AddPermutsForm(string from, string to, bool final)
		{
			InitializeComponent();
			fromTextBox.Text = from;
			toTextBox.Text = to;
			finalCheckBox.Checked = final;
		}

		private void fromTextBox_TextChanged(Object sender, EventArgs e)
		{
			if ((fromTextBox.Text.Length != 0) || (toTextBox.Text.Length != 0))
				okButton.Enabled = true;
			else okButton.Enabled = false;
		}

		private void okButton_Click(Object sender, EventArgs e)
		{
			From = fromTextBox.Text;
			To = toTextBox.Text;
			IsFinal = finalCheckBox.Checked;
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(Object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

        private void scriptButton_Click(object sender, EventArgs e)
        {
            ScriptForm sf = new ScriptForm();
            if (sf.ShowDialog() == DialogResult.OK)
			{
				DialogResult = DialogResult.Cancel;
			}
        }
    }
}
