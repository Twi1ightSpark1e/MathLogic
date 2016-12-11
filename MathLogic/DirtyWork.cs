using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MathLogic
{
	static class DirtyWork
	{
		private delegate void InvokeWork();
		public static bool Stop { get; set; }

		public static bool CheckByAlphabet(List<string> alphabet, List<Tuple<string, string>> permuts, string text)
		{
			bool found;
			foreach (Tuple<string, string> x in permuts)
			{
				foreach (char c in x.Item1)
				{
					found = false;
					foreach (string a in alphabet)
					{
						if (a.Contains(c))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						MessageBox.Show(string.Format("В списках замен используется символ (\"{0}\"), отсутствующий в алфавите!", c));
						return false;
					}
				}
				foreach (char c in x.Item2)
				{
					found = false;
					foreach (string a in alphabet)
					{
						if (a.Contains(c))
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						MessageBox.Show(string.Format("В списках замен используется символ (\"{0}\"), отсутствующий в алфавите!", c));
						return false;
					}
				}
			}
			foreach (char c in text)
			{
				found = false;
				foreach (string a in alphabet)
				{
					if (a.Contains(c))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					MessageBox.Show(string.Format("В исходном тексте используется символ (\"{0}\"), отсутствующий в алфавите!", c));
					return false;
				}
			}
			return true;
		}

		public static string CharRange(char startSymbol, char endSymbol)
		{
			return new string(Enumerable.Range(startSymbol, endSymbol - startSymbol + 1).Select(c => (char)c).ToArray());
		}

        public static int StringFind(string source, string subject)
        {
            if (subject == string.Empty)
                return 0;
            if (source.Contains(subject))
            {
                for (int i = 0; i <= source.Length - subject.Length; i++)
                    if (source.Substring(i, subject.Length) == subject)
                        return i;
            }
            return -1;
        }

		public static int StringFindAny(string source, string[] subjects, out int index)
		{
			if (subjects.Length == 0)
			{
				index = -1;
				return -1;
			}
			int minpos = source.Length;
			index = -1;
			for (int i = 0; i < subjects.Length; i++)
			{
				int pos = StringFind(source, subjects[i]);
				if (pos != -1)
					if ((pos < minpos) || (minpos == source.Length))
					{
						minpos = pos;
						index = i;
					}
			}
			return (minpos == source.Length ? -1 : minpos);
		}

        public static string DoWork(StartForm form, List<Tuple<string, string>> permuts, List<bool> finals)
        {
            int step = 1;
            Stop = false;
            string text = form.inputTextBox.Text;
            int i = 0;
            int pos = -1;
            bool found = false;
            while (!Stop)
            {
                var x = permuts[i];
                do
                {
                    if (Stop)
                        break;
                    pos = StringFind(text, x.Item1);
                    if (pos != -1)
                    {
                        string before = text;
                        if (x.Item2 == string.Empty)
                            text = text.Remove(pos, x.Item1.Length);
                        else if (x.Item1 == string.Empty)
                            text = text.Insert(pos, x.Item2);
                        else
                            text = text.Remove(pos, x.Item1.Length).Insert(pos, x.Item2);
                        form.Invoke(new InvokeWork(() =>
                        {
                            form.stepsDataGridView.Rows.Add(new object[] { step, before, text });
                        }));
                        step++;
                        if (finals[i])
                            Stop = true;
                        found = true;
                        if (x.Item1 == string.Empty)
                            break;
                    }
                }
                while (pos != -1);
                if (!found)
                    i = (i + 1) % permuts.Count;
                else i = 0;
                if ((i == 0) && (!found))
                    Stop = true;
                else found = false;
            }
            return text;
        }
    }
}
