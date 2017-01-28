using System;
using System.Linq;
using System.Windows.Forms;

namespace MathLogic
{
	public partial class ScriptForm : Form
	{
		public ScriptForm()
		{
			InitializeComponent();
			scriptRichTextBox.SelectionTabs = new int[] { 28, 56, 84, 132 };
		}

		private void executeButton_Click(object sender, EventArgs e)
		{
			Interpreter interpreter = new Interpreter();
			interpreter.DoScript(scriptRichTextBox.Text);
			string permuts = "Следующие замены будут добавлены: " + Environment.NewLine;
			permuts = Interpreter.Output.Aggregate(permuts, (temp, x) => temp + $"{x.Key} ->{(x.Final ? "." : string.Empty)} {x.Value}" + Environment.NewLine);
			MessageBox.Show(permuts);
			StartForm.AddPermutsFromScript(Interpreter.Output);
			DialogResult = DialogResult.OK;
		}

		private void checkButton_Click(object sender, EventArgs e)
		{
			Interpreter interpreter = new Interpreter();
			interpreter.DoScript(scriptRichTextBox.Text);
			string permuts = "Следующие замены будут добавлены: " + Environment.NewLine;
			permuts = Interpreter.Output.Aggregate(permuts, (temp, x) => temp + $"{x.Key} ->{(x.Final ? "." : string.Empty)} {x.Value}" + Environment.NewLine);
			MessageBox.Show(permuts);
		}

		private void ScriptForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.None)
				DialogResult = DialogResult.Cancel;
		}

		private int TabsCount(string line)
		{
			int answer = 0;
			for (int i = 0; i < line?.Length; i++)
				if (line[i] == '\t')
					answer++;
				else break;
			return answer;
		}

		private void scriptRichTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			int currentLine = scriptRichTextBox.GetLineFromCharIndex(scriptRichTextBox.SelectionStart);
			if (e.KeyCode == Keys.Enter && scriptRichTextBox.Lines[currentLine] == string.Empty)
			{
				string prevLine = scriptRichTextBox.Lines[currentLine - 1] ?? string.Empty;
				int prevLineTabs = TabsCount(prevLine);
				Interpreter.RemoveUnnecessarySpaces(ref prevLine);
				int openBrackets = 0, closeBrackets = 0, symbolBracketsCount = 0;
				foreach (char c in prevLine)
				{
					if ((c == '\"') || (c == '\''))
					{
						symbolBracketsCount = ++symbolBracketsCount % 2;
					}
					else if (c == '{')
					{
						openBrackets++;
					}
				}
				int currentLineTabs = prevLineTabs + openBrackets - closeBrackets;
				if (currentLineTabs < 0)
					currentLineTabs = 0;
				ChangeLine(currentLine, string.Empty.PadLeft(currentLineTabs, '\t'));
			}
			else if (e.KeyCode == Keys.OemOpenBrackets && e.Shift)
			{
				int pos = scriptRichTextBox.SelectionStart;
				int count = TabsCount(scriptRichTextBox.Lines[currentLine]);
				scriptRichTextBox.Text = scriptRichTextBox.Text.Insert(scriptRichTextBox.SelectionStart, Environment.NewLine + string.Empty.PadLeft(count, '\t') + '}');
				scriptRichTextBox.SelectionStart = pos;
			}
			else if (e.KeyCode == Keys.OemCloseBrackets && e.Shift)
			{
				string line = scriptRichTextBox.Lines[currentLine];
				Interpreter.RemoveUnnecessarySpaces(ref line);
				if (scriptRichTextBox.Lines[currentLine].StartsWith('\t'.ToString()) && line == "}")
				{
					ChangeLine(currentLine, scriptRichTextBox.Lines[currentLine].Remove(0, 1));
				}
			}
		}

		private void ChangeLine(int line, string text)
		{
			int s1 = scriptRichTextBox.GetFirstCharIndexFromLine(line);
			int s2 = line < scriptRichTextBox.Lines.Count() - 1 ?
					  scriptRichTextBox.GetFirstCharIndexFromLine(line + 1) - 1 :
					  scriptRichTextBox.Text.Length;
			scriptRichTextBox.Select(s1, s2 - s1);
			scriptRichTextBox.SelectedText = text;
		}
	}
}
