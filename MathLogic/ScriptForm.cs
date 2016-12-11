using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathLogic
{
    public partial class ScriptForm : Form
    {
        private Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
		public Hashtable types = new Hashtable()
		{
			{ "int", "int64" },
			{ "uint", "uint64" },
			{ "bool", "bool" },
			{ "char", "char" },
			{ "string", "string" },
			{ "float", "single" },
			{ "double", "double" }
		};
		private List<string> commands = new List<string>()
		{
			"push",
			"show"
		};
		private List<(string name, int priority, Func<string, dynamic, dynamic, dynamic> function)> operators = new List<(string name, int priority, Func<string, object, object, dynamic> function)>()
		{
			{ ("+",  1,  (string type, dynamic left, dynamic right) => left + right) },
			{ ("-",  1,  (string type, dynamic left, dynamic right) => left - right) },
			{ ("*",  0,  (string type, dynamic left, dynamic right) => left * right) },
			{ ("/",  0,  (string type, dynamic left, dynamic right) => left / right) },
			{ ("%",  0,  (string type, dynamic left, dynamic right) => left % right) },
			{ ("&",  2,  (string type, dynamic left, dynamic right) => left & right) },
			{ ("|",  2,  (string type, dynamic left, dynamic right) => left | right) },
			{ ("&&", -1, (string type, dynamic left, dynamic right) => left && right) },
			{ ("||", -1, (string type, dynamic left, dynamic right) => left || right) },
			{ ("=", -2, null) },
			{ ("==", -1, (string type, dynamic left, dynamic right) => left == right) }
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
			var temp = Activator.CreateInstance(Type.GetType($"System.{type}", false, true));
			temp = Convert.ChangeType(value, Type.GetType($"System.{type}", false, true));
			return temp;
		}
		
		private string GetTypeByValue(string value)
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

		private string ReadOperator(string source, ref int start, bool isReverse = false)
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

		private string ReadOperand(string source, ref int start, bool isReverse = false)
		{
			string temp = string.Empty;
			int i, bracketsCount = 0;
			for (i = start; (i < source.Length) && (i >= 0); i += (isReverse ? -1 : 1))
			{
				if (source[i] == '(')
				{
					bracketsCount++;
					temp += source[i];
					continue;
				}
				else if (source[i] == ')')
				{
					bracketsCount--;
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
					if (!found)
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

		private void ResolveExpression(ref string expression)
		{
			//expression = expression.Replace(' ', new char()); //Not so fast, John! Expression may contain strings!
			foreach (var variable in variables)
			{
				if (expression == variable.Key)
					expression = variables[expression].ToString();
			}
			for (int priority = 0; priority < 3; priority++)
			{
				var ops = from x in operators
						  where x.priority == priority
						  select x;
				for (int i = 0; i < ops.Count(); i++)
				{
					if (expression.Contains(ops.ElementAt(i).name))
					{
						int position = DirtyWork.StringFind(expression, ops.ElementAt(i).name);
						int temp = position - 1;
						string left = new string(ReadOperand(expression, ref temp, true).Reverse().ToArray());
						int insert = position - left.Length;
						temp = position + 1;
						string right = ReadOperand(expression, ref temp, false);
						expression = expression.Remove(insert, left.Length + right.Length + 1).Replace("()", "");
						ResolveExpression(ref left);
						ResolveExpression(ref right);
						dynamic l = Convert.ChangeType(left, Type.GetType($"System.{types[GetTypeByValue(left)].ToString()}", false, true));
						dynamic r = Convert.ChangeType(right, Type.GetType($"System.{types[GetTypeByValue(right)].ToString()}", false, true));
						expression = expression.Insert(insert, ops.ElementAt(i).function(GetTypeByValue(left), l, r).ToString());
						i = -1;
					}
				}
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
							string value = ReadOperand(command, ref temp);
							if ((typename.Key.ToString() == "float") || (typename.Key.ToString() == "double"))
								value = value.Replace('.', ',');
							variables.Add(varname, GetInstance(typename.Value.ToString(), value));
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
				foreach (var varname in variables)
				{
					if (command.StartsWith(varname.Key))
					{
						int temp = 0;
						command = command.Remove(0, varname.Key.Length);
						string op = ReadOperator(command, ref temp);
						if (op == "=")
						{
							//string value =
							assigmentFound = true; 
						}
						else continue; //wutifak do u want from me? o_O
					}
				}
				if (assigmentFound)
					continue;
			}
		}

        private void executeButton_Click(object sender, EventArgs e)
        {
			DoScript();
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            DoScript();
        }
    }
}
