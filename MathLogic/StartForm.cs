using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MathLogic
{
	public partial class StartForm : Form
	{
		public static StartForm LastInstance { get; private set; }
		private static List<string> alphabet = new List<string>();
		private static List<Triple<string, string, bool>> permuts = new List<Triple<string, string, bool>>();
		private delegate void InvokeWork();

		Thread work;

		public StartForm()
		{
			InitializeComponent();
			LastInstance = this;
		}

		private void addAlphabetButton_Click(object sender, EventArgs e)
		{
			var form = new AddAlphabetForm();
			var result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (form.Result.Contains(".."))
				{
					try
					{
						alphabet.Add(DirtyWork.CharRange(form.Result[0], form.Result[3]));
					}
					catch (ArgumentOutOfRangeException)
					{
						MessageBox.Show("Выбрано неправильное множество символов!");
					}
				}
				else alphabet.Add(form.Result);
				alphabetListBox.Items.Add(form.Result);
			}
		}

		private void alphabetListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			deleteAlphabetButton.Enabled = alphabetListBox.SelectedIndex != -1;
		}

		private void permutsListBox_SelectedIndexChanged(Object sender, EventArgs e)
		{
			deletePermutsButton.Enabled = permutsListBox.SelectedIndex != -1;
		}

		private void addPermutsButton_Click(Object sender, EventArgs e)
		{
			var form = new AddPermutsForm();
			var result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (permuts.Any((x) => x.Key == form.From))
				{
					MessageBox.Show("Замена с такого значения уже имеется!");
					return;
				}
				permuts.Add(new Triple<string, string, bool>(form.From, form.To, form.IsFinal));
				permutsListBox.Items.Add(string.Format("{0} ->{1} {2}", form.From, (form.IsFinal ? "." : string.Empty), form.To));
			}
		}

		private void alphabetListBox_DoubleClick(Object sender, EventArgs e)
		{
			if (alphabetListBox.SelectedIndex != -1)
			{
				var form = new AddAlphabetForm((string)alphabetListBox.Items[alphabetListBox.SelectedIndex]);
				var result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					if (form.Result != (string)alphabetListBox.Items[alphabetListBox.SelectedIndex])
					{
						alphabet.RemoveAt(alphabetListBox.SelectedIndex);
						alphabetListBox.Items.RemoveAt(alphabetListBox.SelectedIndex);
						if (form.Result.Contains(".."))
						{
							try
							{
								alphabet.Add(DirtyWork.CharRange(form.Result[0], form.Result[3]));
							}
							catch (ArgumentOutOfRangeException)
							{
								MessageBox.Show("Выбрано неправильное множество символов!");
							}
						}
						else alphabet.Add(form.Result);
						alphabetListBox.Items.Add(form.Result);
					}
				}
			}
		}

		private void permutsListBox_DoubleClick(Object sender, EventArgs e)
		{
			var index = permutsListBox.SelectedIndex;
			if (index != -1)
			{
				var form = new AddPermutsForm(permuts[index].Key, permuts[index].Value, permuts[index].Final);
				var result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					if (permuts.Any((x) => (permuts[index] != x) && (x.Key == form.From)))
					{
						{
							MessageBox.Show("Замена с такого значения уже имеется!");
							return;
						}
					}
					permuts[index].Key = form.From;
					permuts[index].Value = form.To;
					permuts[index].Final = form.IsFinal;
					permutsListBox.Items.RemoveAt(index);
					permutsListBox.Items.Insert(index, string.Format("{0} ->{1} {2}", form.From, (form.IsFinal ? "." : string.Empty), form.To));
				}
			}
		}

		private void startButton_Click(Object sender, EventArgs e)
		{
			if (!DirtyWork.CheckByAlphabet(alphabet, permuts, inputTextBox.Text))
				return;
			addAlphabetButton.Enabled = addPermutsButton.Enabled = deleteAlphabetButton.Enabled =
				deletePermutsButton.Enabled = alphabetListBox.Enabled = permutsListBox.Enabled =
				inputTextBox.Enabled = false;
			startButton.Text = "Стоп";
			startButton.Click -= startButton_Click;
			startButton.Click += stopButton_Click;
			stepsDataGridView.Rows.Clear();

			work = new Thread(DoWork);
			work.Start();
		}

		private void stopButton_Click(Object sender, EventArgs e)
		{
			DirtyWork.Stop = true;
			startButton.Text = "Старт!";
			startButton.Click -= stopButton_Click;
			startButton.Click += startButton_Click;
		}

		private void DoWork()
		{
			MessageBox.Show("Результат: \"" + DirtyWork.DoWork(this, permuts) + "\"");
			Invoke(new InvokeWork(() =>
			{
				addAlphabetButton.Enabled = addPermutsButton.Enabled = alphabetListBox.Enabled =
					permutsListBox.Enabled = inputTextBox.Enabled = true;
				deleteAlphabetButton.Enabled = alphabetListBox.SelectedIndex != -1;
				deletePermutsButton.Enabled = permutsListBox.SelectedIndex != -1;
				stopButton_Click(this, new EventArgs());
			}));
		}

		private void inputTextBox_TextChanged(Object sender, EventArgs e)
		{
			startButton.Enabled = inputTextBox.Text.Length != 0;
		}

		private void deleteAlphabetButton_Click(Object sender, EventArgs e)
		{
			alphabet.RemoveAt(alphabetListBox.SelectedIndex);
			alphabetListBox.Items.RemoveAt(alphabetListBox.SelectedIndex);
			if (alphabetListBox.Items.Count != 0)
				alphabetListBox.SelectedIndex = 0;
		}

		private void deletePermutsButton_Click(Object sender, EventArgs e)
		{
			permuts.RemoveAt(permutsListBox.SelectedIndex);
			permutsListBox.Items.RemoveAt(permutsListBox.SelectedIndex);
			if (permutsListBox.Items.Count != 0)
				permutsListBox.SelectedIndex = 0;
		}

		private void импортToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog()
			{
				Filter = "MASF File (*.masf)|*.masf"
			};
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(new StreamReader(ofd.FileName).ReadToEnd());
					alphabet = data.alphabet;
                    alphabetListBox.Items.Clear();
					foreach (var a in alphabet)
						alphabetListBox.Items.Add(a);
					permuts = data.permuts;
                    permutsListBox.Items.Clear();
					foreach (var p in permuts)
						permutsListBox.Items.Add(string.Format("{0} ->{1} {2}", p.Key, (p.Final ? "." : string.Empty), p.Value));
                    inputTextBox.Text = string.Empty;
				}
				catch
				{
					MessageBox.Show("Выбран неверный или поврежденный файл импорта!");
				}
			}
		}

		private void экспортToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog()
			{
				Filter = "MASF File (*.masf)|*.masf"
			};
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					Data data = new Data();
					data.alphabet = alphabet;
					data.permuts = permuts;
					var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
					StreamWriter sw = new StreamWriter(sfd.FileName, false);
					sw.Write(serialized);
					sw.Flush(); sw.Close();
				}
				catch
				{
					MessageBox.Show("Возникла ошибка!");
				}
			}
		}

		private void выходToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		internal static void AddPermutsFromScript(List<Triple<string, string, bool>> list)
		{
			foreach (var element in list)
			{
				permuts.Add(new Triple<string, string, bool>(element.Key, element.Value, element.Final));
				LastInstance.permutsListBox.Items.Add(string.Format("{0} ->{1} {2}", element.Key, (element.Final ? "." : string.Empty) ,element.Value));
			}
		}
	}

	public class Triple<T1, T2, T3>
	{
		public T1 Key { get; set; }
		public T2 Value { get; set; }
		public T3 Final { get; set; }

		public Triple(T1 key, T2 value, T3 final)
		{
			Key = key;
			Value = value;
			Final = final;
		}
	}

	class Data
	{
		public List<string> alphabet { get; set; }
		public List<Triple<string, string, bool>> permuts { get; set; }
	}
}
