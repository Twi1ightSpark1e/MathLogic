using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
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

        private static Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
		public static Hashtable types = new Hashtable()
		{
			{ "int", "int64" },
			{ "uint", "uint64" },
			{ "bool", "boolean" },
			{ "char", "char" },
			{ "string", "string" },
			{ "float", "single" },
			{ "double", "double" }
		};
		private static List<(string name, int args, Func<dynamic, dynamic> func)> commands = new List<(string name, int args, Func<dynamic, dynamic> func)>()
		{
			{ ("push", 3, new Func<dynamic, dynamic>((dynamic i) =>
				{
					string arg0 = i.Arg0;
					string arg1 = i.Arg1;
					string arg2 = i.Arg2;
					ResolveExpression(ref arg0);
					ResolveExpression(ref arg1);
					ResolveExpression(ref arg2);
					output.Add(new Triple<string, string, bool>(TrimPairs(arg0, TrimSymbols.Apostrophes | TrimSymbols.Quotes), TrimPairs(arg1, TrimSymbols.Apostrophes | TrimSymbols.Quotes), 
						bool.Parse(arg2)));
					return new object();
				})) },
			{ ("show", 1, new Func<dynamic, dynamic>((dynamic i) =>
				{
					string arg0 = i.Arg0;
					ResolveExpression(ref arg0);
					MessageBox.Show(arg0, "Сообщение скрипта");
					return new object();
				})) }
		};
		private static List<(string name, int priority, Func<string, dynamic, dynamic, dynamic> function)> operators = new List<(string name, int priority, Func<string, object, object, dynamic> function)>()
		{
			{ ("+",  1, (string type, dynamic left, dynamic right) => left +  right) },
			{ ("-",  1, (string type, dynamic left, dynamic right) => left -  right) },
			{ ("*",  0, (string type, dynamic left, dynamic right) => left *  right) },
			{ ("/",  0, (string type, dynamic left, dynamic right) => left /  right) },
			{ ("%",  0, (string type, dynamic left, dynamic right) => left %  right) },
			//{ ("&",  2, (string type, dynamic left, dynamic right) => left &  right) },
			//{ ("|",  2, (string type, dynamic left, dynamic right) => left |  right) },
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
        }

		private dynamic GetInstance(string type)
		{
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

		private string ReadUntilSemicolon(string source, ref int start)
        {
			string temp = string.Empty;
			int i;
            for (i = start; i < source.Length; i++)
                if (source[i] != ';')
                    temp += source[i];
                else break;
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
				if ((symbolBracketsCount == 0) && (!skipNext) && (value[i] == ' '))
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
				if (expression == variable.Key)
					expression = variables[expression].ToString();
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
					pos = DirtyWork.StringFindAny(expression, opsNames.ToArray(), out int index);
					if (pos > 0)
					{
						int position = DirtyWork.StringFind(expression, expression.Substring(pos, opsNames[index].Length));
						//Check for bitwise operator found instead of logical
						//int t = position;
						//string tString = ReadOperator(expression, ref t);
						//if (tString != opsNames[index])
						//{
						//	opsNames.RemoveAt(index);
						//	continue;
						//}

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
						dynamic l = Convert.ChangeType(TrimPairs(left, TrimSymbols.Apostrophes | TrimSymbols.Quotes), Type.GetType($"System.{types[GetTypeByValue(left)].ToString()}", false, true));
						dynamic r = Convert.ChangeType(TrimPairs(right, TrimSymbols.Apostrophes | TrimSymbols.Quotes), Type.GetType($"System.{types[GetTypeByValue(right)].ToString()}", false, true));
						expression = expression.Insert(insert, func(SelectDestinationType(left, right), l, r).ToString());
					}
				}
				while (pos > 0);
			}
		}

        private void DoScript()
        {
            int position = 0;
            while (position < scriptRichTextBox.Text.Length-1)
            {
                string command = ReadUntilSemicolon(scriptRichTextBox.Text, ref position).
					Replace('\r', new char()).Replace('\n', new char()).Replace('\t', new char()).Trim(new char[] { '\0' });
				position++; //To skip semicolon
				//Search for defining variables
				bool varFound = false;
				foreach (DictionaryEntry typename in types)
				{
					if (command.StartsWith(typename.Key.ToString()))
					{
						int temp = 0;
						command = command.Remove(0, typename.Key.ToString().Length);
						string varname = ReadOperand(command, ref temp);
						string op = ReadOperator(command, ref temp);
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
						varFound = true;
						break;
					}
				}
				if (varFound)
					continue;
				//Search for assigments
				bool assigmentFound = false;
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
							variables[varname] = Convert.ChangeType(value, Type.GetType($"System.{GetTypeByValue(varname)}", false, true));
							assigmentFound = true; 
						}
						else continue; //wutifak do u want from me? o_O
					}
				}
				if (assigmentFound)
					continue;
				//Search for methods
				bool methodFound = false;
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
						methodFound = true;
					}
				}
				if (methodFound)
					continue;
			}
		}

        private void executeButton_Click(object sender, EventArgs e)
        {
			variables.Clear();
			output.Clear();
			DoScript();
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
			variables.Clear();
			output.Clear();
			DoScript();
        }
    }
}
