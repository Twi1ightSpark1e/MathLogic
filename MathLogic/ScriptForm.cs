using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Windows.Forms;

namespace MathLogic
{
	public partial class ScriptForm : Form
	{
		internal static List<Triple<string, string, bool>> output = new List<Triple<string, string, bool>>();

		[Flags]
		enum TrimSymbols
		{
			Brackets = 0x1,
			Apostrophes = 0x2,
			Quotes = 0x4
		}

		enum WorkState
		{
			CannotBeDone,
			CommandsDone,
			Break,
			Continue
		}
		private bool isInLoop;
		private bool exitLoop;

		private static Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();

		private static Hashtable types = new Hashtable()
		{
			{ "int", "int64" },
			{ "uint", "uint64" },
			{ "bool", "boolean" },
			{ "char", "char" },
			{ "string", "string" },
			{ "float", "single" },
			{ "double", "double" }
		};
		private static List<(string name, int args, Action<dynamic> func)> commands = new List<(string name, int args, Action<dynamic> func)>()
		{
			{ ("push", 3, new Action<dynamic>((dynamic i) =>
				{
					string arg0 = i.Arg0;
					string arg1 = i.Arg1;
					string arg2 = i.Arg2;
					ResolveExpression(ref arg0);
					ResolveExpression(ref arg1);
					ResolveExpression(ref arg2);
					output.Add(new Triple<string, string, bool>(TrimPairs(arg0, TrimSymbols.Apostrophes | TrimSymbols.Quotes), TrimPairs(arg1, TrimSymbols.Apostrophes | TrimSymbols.Quotes),
						bool.Parse(arg2)));
				})) },
			{ ("show", 1, new Action<dynamic>((dynamic i) =>
				{
					string arg0 = i.Arg0;
					ResolveExpression(ref arg0);
					MessageBox.Show(TrimPairs(arg0, TrimSymbols.Apostrophes | TrimSymbols.Quotes), "Сообщение скрипта");
				})) }
		};
		private static List<(string name, int priority, Func<string, dynamic, dynamic, dynamic> function)> operators = new List<(string name, int priority, Func<string, object, object, dynamic> function)>()
		{
			{ ("+",  1, (string type, dynamic left, dynamic right) => left +  right) },
			{ ("-",  1, (string type, dynamic left, dynamic right) => left -  right) },
			{ ("*",  0, (string type, dynamic left, dynamic right) => left *  right) },
			{ ("/",  0, (string type, dynamic left, dynamic right) => left /  right) },
			{ ("%",  0, (string type, dynamic left, dynamic right) => left %  right) },
			{ ("&",  2, (string type, dynamic left, dynamic right) => left &  right) },
			{ ("|",  2, (string type, dynamic left, dynamic right) => left |  right) },
			{ ("^",  2, (string type, dynamic left, dynamic right) => left ^  right) },
			{ ("&&", 3, (string type, dynamic left, dynamic right) => left && right) },
			{ ("||", 3, (string type, dynamic left, dynamic right) => left || right) },
			{ ("=", -1, null) },
			{ (",", -1, null) },
			{ ("==", 3, (string type, dynamic left, dynamic right) => left == right) },
			{ (">",  3, (string type, dynamic left, dynamic right) => left >  right) },
			{ ("<",  3, (string type, dynamic left, dynamic right) => left <  right) },
			{ (">=", 3, (string type, dynamic left, dynamic right) => left >= right) },
			{ ("<=", 3, (string type, dynamic left, dynamic right) => left <= right) },
			{ ("!=", 3, (string type, dynamic left, dynamic right) => left != right) }
		};

		public ScriptForm()
		{
			InitializeComponent();
			scriptRichTextBox.SelectionTabs = new int[] { 28, 56, 84, 132 };
		}

		private dynamic GetInstance(string type)
		{
			if (type.ToLower() == "string")
				return string.Empty;
			return Activator.CreateInstance(Type.GetType($"System.{type}", false, true));
		}

		private dynamic GetInstance(string type, dynamic value)
		{
			if (type.ToLower() == "string")
				return value as string;
			var temp = Activator.CreateInstance(Type.GetType($"System.{type}", false, true));
			temp = Convert.ChangeType(TrimPairs(value, TrimSymbols.Apostrophes | TrimSymbols.Quotes), Type.GetType($"System.{type}", false, true));
			return temp;
		}

		private static string GetTypeByValue(string value)
		{
			if (value == string.Empty)
				return "string";
			if (value[0] == '\'')
				return "char";
			if (value[0] == '\"')
				return "string";
			if ((value.ToString().ToLower() == "true") || (value.ToString().ToLower() == "false"))
				return "bool";
			if (Int64.TryParse(value, out long xlong))
				return "int";
			if (UInt64.TryParse(value, out ulong xulong))
				return "uint";
			if (Single.TryParse(value, out float xfloat))
				return "float";
			if (Double.TryParse(value, out double xdouble))
				return "double";
			if (variables.Keys.Contains(value))
				return variables[value].GetType().Name;
			return "error";
		}

		private static string SelectDestinationType(string left, string right)
		{
			string type1 = GetTypeByValue(left);
			string type2 = GetTypeByValue(right);
			if ((type1 == "string") || (type1 == "char") || (type2 == "string") || (type2 == "char"))
				return "string";
			else return type1;
		}

		private string ReadCommand(string source, ref int start)
		{
			string temp = string.Empty;
			int i, symbolBracketsCount = 0, bracketsCount = 0;
			for (i = start; i < source.Length; i++)
			{
				if ((source[i] == '\"') || (source[i] == '\''))
				{
					symbolBracketsCount = ++symbolBracketsCount % 2;
					temp += source[i];
					continue;
				}
				else if (symbolBracketsCount != 0)
				{
					temp += source[i];
					continue;
				}
				else if ((source[i] != ';') && (source[i] != '{') && (source[i] != '}'))
					temp += source[i];
				else break;
				if (source[i] == '(')
					bracketsCount++;
				if (source[i] == ')')
					if (--bracketsCount <= 0)
						break;
			}
			start = i;
			return temp;
		}

		private string ReadCommandsInBrackets(string source, ref int start)
		{
			string temp = string.Empty;
			int i, symbolBracketsCount = 0, opBracketsCount = 0;

			while (source[start] == '\n' || source[start] == '\t' || source[start] == ' ')
				start++;
			if (source[start] != '{')
				return ReadCommand(source, ref start);

			for (i = start; i < source.Length; i++)
			{
				if ((source[i] == '\"') || (source[i] == '\''))
				{
					symbolBracketsCount = ++symbolBracketsCount % 2;
					temp += source[i];
					continue;
				}
				else if (symbolBracketsCount != 0)
				{
					temp += source[i];
					continue;
				}
				else if (source[i] == '{')
				{
					opBracketsCount++;
					temp += source[i];
				}
				else if (source[i] != '}')
					temp += source[i];
				else if (source[i] == '}')
				{
					opBracketsCount--;
					if (opBracketsCount <= 0)
						break;
					else temp += source[i];
				}
				else break;
			}
			start = i;
			return temp;
		}

		private static string ReadOperator(string source, ref int start, bool isReverse = false)
		{
			string temp = string.Empty;
			int i;
			for (i = start; (i < source.Length) && (i >= 0); i += (isReverse ? -1 : 1))
			{
				bool found = false;
				foreach (var op in operators)
				{
					if (op.name.Contains(source[i].ToString()))
					{
						temp += source[i];
						found = true;
						break;
					}
				}
				if ((source[i] == ' ') || (found))
					continue;
				else break;
			}
			start = i;
			return temp;
		}

		private static string ReadOperand(string source, ref int start, bool isReverse = false)
		{
			string temp = string.Empty;
			int i, bracketsCount = 0, symbolBracketsCount = 0;
			for (i = start; (i < source.Length) && (i >= 0); i += (isReverse ? -1 : 1))
			{
				if ((source[i] == '\"') || (source[i] == '\''))
				{
					symbolBracketsCount = ++symbolBracketsCount % 2;
					temp += source[i];
					continue;
				}
				else if (symbolBracketsCount != 0)
				{
					temp += source[i];
					continue;
				}
				else if (source[i] == '(')
				{
					bracketsCount += (isReverse ? -1 : 1);
					if (bracketsCount < 0)
						break;
					temp += source[i];
					continue;
				}
				else if (source[i] == ')')
				{
					bracketsCount -= (isReverse ? -1 : 1);
					if (bracketsCount < 0)
						break;
					temp += source[i];
					continue;
				}
				else if (source[i] == ' ')
					continue;
				else
				{
					bool found = false;
					foreach (var op in operators)
						if (op.name.Contains(source[i].ToString()))
						{
							found = true;
							break;
						}
					if ((!found) || (bracketsCount != 0))
					{
						temp += source[i];
						continue;
					}
				}
				if (bracketsCount == 0)
					break;
			}
			start = i;
			return temp;
		}

		private static string ReadArgument(string source, ref int start)
		{
			string temp = string.Empty;
			while ((start < source.Length) && (source[start] != ','))
				temp += source[start++];
			return temp;
		}

		private static int FindOperator(string source, string[] operators, out int index)
		{
			int symbolBracketsCount = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if ((source[i] == '\"') || (source[i] == '\''))
				{
					symbolBracketsCount = ++symbolBracketsCount % 2;
					continue;
				}
				if (symbolBracketsCount != 0)
					continue;
				for (int j = 0; j < operators.Count(); j++)
					if ((i + operators[j].Length <= source.Length) && (source.Substring(i, operators[j].Length) == operators[j]))
					{
						index = j;
						return i;
					}
			}
			index = -1;
			return -1;
		}

		private static void RemoveUnnecessarySpaces(ref string value)
		{
			int bracketsCount = 0;
			int symbolBracketsCount = 0;
			bool skipNext = false;
			for (int i = 0; i < value.Length; i++)
			{
				if (skipNext)
				{
					skipNext = false;
					continue;
				}
				if ((symbolBracketsCount == 0) && (!skipNext) && ((value[i] == ' ') || (value[i] == '\n') || (value[i] == '\t')))
				{
					value = value.Remove(i, 1);
					i--;
				}
				else if (value[i] == '\'')
				{
					skipNext = true;
				}
				else if (value[i] == '(')
				{
					bracketsCount++;
				}
				else if (value[i] == ')')
				{
					bracketsCount--;
				}
				else if ((value[i] == '\"') || (value[i] == '\''))
				{
					symbolBracketsCount = ++symbolBracketsCount % 2;
				}
			}
		}

		private static string TrimPairs(string source, TrimSymbols trim)
		{
			bool bracketsExists = true;
			while ((bracketsExists) && (source.Length > 0))
			{
				bracketsExists = false;
				if (trim.HasFlag(TrimSymbols.Apostrophes))
					bracketsExists = bracketsExists || (source[0] == '\'') && (source[source.Length - 1] == '\'');
				if (trim.HasFlag(TrimSymbols.Quotes))
					bracketsExists = bracketsExists || (source[0] == '\"') && (source[source.Length - 1] == '\"');
				if (trim.HasFlag(TrimSymbols.Brackets))
					bracketsExists = bracketsExists || (source[0] == '(') && (source[source.Length - 1] == ')');
				if (bracketsExists)
					source = source.Substring(1, source.Length - 2);
			}
			return source;
		}

		private static void ResolveExpression(ref string expression)
		{
			RemoveUnnecessarySpaces(ref expression);
			expression = TrimPairs(expression, TrimSymbols.Brackets);
			foreach (var variable in variables)
			{
				char add = Char.MinValue;
				if (expression == variable.Key)
				{
					if (variable.Value.GetType().Name == "String")
						add = '\"';
					else if (variable.Value.GetType().Name == "Char")
						add = '\'';
					expression = string.Format("{0}{1}{0}", add, variables[expression].ToString()).Trim('\0');
				}
			}
			int minPriority = (from x in operators
							   select x.priority).Max();
			for (int priority = 0; priority <= minPriority; priority++)
			{
				var ops = from x in operators
						  where x.priority == priority
						  select x;
				var opsNames = (from x in operators
								where x.priority == priority
								select x.name).ToList();
				int pos = -1;
				do
				{
					pos = FindOperator(expression, opsNames.ToArray(), out int index);
					if (pos > 0)
					{
						int position = DirtyWork.StringFind(expression, expression.Substring(pos, opsNames[index].Length));
						//Check for bitwise operator found instead of logical
						int t = position;
						string tString = ReadOperator(expression, ref t);
						if (tString != opsNames[index])
						{
							opsNames.RemoveAt(index);
							continue;
						}

						string exp = expression;
						var func = (from x in ops
									where x.name == exp.Substring(pos, opsNames[index].Length)
									select x.function).First();
						int temp = position - 1;
						string left = new string(ReadOperand(expression, ref temp, true).Reverse().ToArray());
						int insert = position - left.Length;
						temp = position + (opsNames[index].Length - 1) + 1;
						string right = ReadOperand(expression, ref temp, false);
						expression = expression.Remove(insert, left.Length + right.Length + opsNames[index].Length);
						left = TrimPairs(left, TrimSymbols.Brackets);
						right = TrimPairs(right, TrimSymbols.Brackets);
						int p = -1;
						do
						{
							p = DirtyWork.StringFind(expression, "()");
							if (p != -1)
							{
								expression = expression.Remove(p, 2);
								if (p + 1 == insert)
									insert--;
							}
						}
						while (p != -1);
						ResolveExpression(ref left);
						ResolveExpression(ref right);
						dynamic l = GetTypeByValue(left).ToLower() == "string" 
							? TrimPairs(left, TrimSymbols.Apostrophes | TrimSymbols.Quotes) 
							: Convert.ChangeType(left, Type.GetType($"System.{types[GetTypeByValue(left)].ToString()}", false, true));
						dynamic r = GetTypeByValue(right).ToLower() == "string" 
							? TrimPairs(right, TrimSymbols.Apostrophes | TrimSymbols.Quotes) 
							: Convert.ChangeType(right, Type.GetType($"System.{types[GetTypeByValue(right)].ToString()}", false, true));
						expression = expression.Insert(insert, func(SelectDestinationType(left, right), l, r).ToString());
					}
				}
				while (pos > 0);
			}
		}

		private bool IsVariableDefining(string command)
		{
			foreach (DictionaryEntry typename in types)
			{
				if (command.StartsWith(typename.Key.ToString()))
				{
					int temp = 0;
					command = command.Remove(0, typename.Key.ToString().Length);
					string varname = ReadOperand(command, ref temp);
					//string op = ReadOperator(command, ref temp);
					if (command.Length > temp + 1)
					{
						string op = command[temp++].ToString();
						if (op == "=") //Variable defined and initialized
						{
							command = command.Remove(0, command.IndexOf('=') + 1);
							string value = command;
							if ((typename.Key.ToString() == "float") || (typename.Key.ToString() == "double"))
								value = value.Replace('.', ',');
							ResolveExpression(ref value);
							variables.Add(varname, GetInstance(typename.Value.ToString(), TrimPairs(value, TrimSymbols.Apostrophes | TrimSymbols.Quotes)));
						}
						else variables.Add(varname, GetInstance(typename.Value.ToString())); //Variable not initialized
					}
					return true;
				}
			}
			return false;
		}

		private bool IsAssigment(string command)
		{
			for (int i = 0; i < variables.Count; i++)
			{
				var varname = variables.ElementAt(i).Key;
				if (command.StartsWith(varname))
				{
					int temp = 0;
					command = command.Remove(0, varname.Length);
					string op = ReadOperator(command, ref temp);
					if (op == "=")
					{
						command = command.Remove(0, command.IndexOf('=') + 1);
						string value = command;
						if ((GetTypeByValue(varname) == "float") || (GetTypeByValue(varname) == "double"))
							value = value.Replace('.', ',');
						ResolveExpression(ref value);
						variables[varname] = Convert.ChangeType(TrimPairs(value, TrimSymbols.Apostrophes | TrimSymbols.Quotes), Type.GetType($"System.{GetTypeByValue(varname)}", false, true));
						return true;
					}
				}
			}
			return false;
		}

		private bool IsMethod(string command)
		{
			foreach (var method in commands)
			{
				if (command.StartsWith(method.name))
				{
					command = command.Remove(0, method.name.Length);
					command = command.Substring(1, command.Length - 2);
					int temp = 0;
					dynamic data = new ExpandoObject();
					var dataDict = (IDictionary<string, object>)data;
					for (int i = 0; i < method.args; i++)
					{
						string arg = ReadArgument(command, ref temp);
						temp++; //To skip comma
						dataDict.Add($"Arg{i}", arg);
					}
					method.func(data);
					return true;
				}
			}
			return false;
		}

		private WorkState IsCondition(string text, string command, ref int position)
		{
			if (command.StartsWith("if"))
			{
				command = command.Remove(0, 2).Trim();
				string cmds = ReadCommandsInBrackets(text, ref position);
				ResolveExpression(ref command);
				bool.TryParse(command, out bool result);
				if (result)
				{
					var state = DoScript(cmds);
					if (state != WorkState.CommandsDone)
						return state;
				}
				return WorkState.CommandsDone;
			}
			return WorkState.CannotBeDone;
		}

		private bool IsLoop(string text, string command, ref int position)
		{
			if (command.StartsWith("while"))
			{
				command = command.Remove(0, 5);
				string expression = command;
				ResolveExpression(ref expression);
				string cmds = ReadCommandsInBrackets(text, ref position);
				bool boolResult;
				long longResult;
				bool.TryParse(expression, out boolResult); long.TryParse(expression, out longResult);
				while (boolResult || (longResult != 0))
				{
					isInLoop = true;
					if (exitLoop)
						break;
					expression = command;
					ResolveExpression(ref expression);
					boolResult = false; longResult = 0;
					bool.TryParse(expression, out boolResult); long.TryParse(expression, out longResult);
					if (boolResult || (longResult != 0))
					{
						var state = DoScript(cmds);
						if (state == WorkState.Break)
							break;
					}
				}
				isInLoop = false;
				return true;
			}
			return false;
		}

		private WorkState DoScript(string text)
		{
			int position = 0;
			while (position < text.Length-1)
			{
				string command = ReadCommand(text, ref position).
					Replace('\r', new char()).Replace('\n', new char()).Replace('\t', new char()).Trim('\0', ' ');
				position++; //To skip semicolon
							//Search for defining variables
				if (command.ToLower() == "continue")
					return WorkState.Continue;
				if (command.ToLower() == "break")
					return WorkState.Break;
				if (IsVariableDefining(command))
					continue;
				//Search for assigments
				if (IsAssigment(command))
					continue;
				//Search for methods
				if (IsMethod(command))
					continue;
				//Search for conditions
				var state = IsCondition(text, command, ref position);
				if (state == WorkState.CommandsDone)
					continue;
				if (state != WorkState.CannotBeDone)
					return state;
				//Search for loops
				if (IsLoop(text, command, ref position))
					continue;
			}
			return WorkState.CommandsDone;
		}

		private void executeButton_Click(object sender, EventArgs e)
		{
			variables.Clear();
			output.Clear();
			DoScript(scriptRichTextBox.Text);
			string permuts = "Следующие замены будут добавлены: " + Environment.NewLine;
			permuts = output.Aggregate(permuts, (temp, x) => temp + $"{x.Key} ->{(x.Final ? "." : string.Empty)} {x.Value}" + Environment.NewLine);
			MessageBox.Show(permuts);
			StartForm.AddPermutsFromScript(output);
			DialogResult = DialogResult.OK;
		}

		private void checkButton_Click(object sender, EventArgs e)
		{
			variables.Clear();
			output.Clear();
			DoScript(scriptRichTextBox.Text);
			string permuts = "Следующие замены будут добавлены: " + Environment.NewLine;
			permuts = output.Aggregate(permuts, (temp, x) => temp + $"{x.Key} ->{(x.Final ? "." : string.Empty)} {x.Value}" + Environment.NewLine);
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
				RemoveUnnecessarySpaces(ref prevLine);
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
			else if (e.KeyValue == 219)
			{
				int pos = scriptRichTextBox.SelectionStart;
				int count = TabsCount(scriptRichTextBox.Lines[currentLine]);
				scriptRichTextBox.Text = scriptRichTextBox.Text.Insert(scriptRichTextBox.SelectionStart, Environment.NewLine + string.Empty.PadLeft(count, '\t') + '}');
				scriptRichTextBox.SelectionStart = pos;
			}
			else if (e.KeyValue == 221)
			{
				string line = scriptRichTextBox.Lines[currentLine];
				RemoveUnnecessarySpaces(ref line);
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
