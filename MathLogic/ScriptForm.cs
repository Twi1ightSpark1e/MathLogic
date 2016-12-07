using System;
using System.Collections;
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
    public partial class ScriptForm : Form
    {
        //private List<Tuple<string, dynamic>> variables = new List<Tuple<string, dynamic>>();
        private Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
		private List<string> types = new List<string>()
		{
			"int32",
			"bool",
			"char",
			"string"
		};

		private List<string> commands = new List<string>()
		{
			"push",
			"show"
		};

        public ScriptForm()
        {
            InitializeComponent();
        }

		private dynamic GetInstance(string type)
		{
			return Activator.CreateInstance(Type.GetType(type, false, true));
		}

		private dynamic GetInstance(string type, dynamic value)
		{
			var temp = Activator.CreateInstance(Type.GetType(type, false, true));
			temp = Convert.ChangeType(value, Type.GetType(type, false, true));
			return temp;
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

		private dynamic ResolveExpression(string expression)
		{
			return null;
		}

        private void DoScript()
        {
            int position = 0;
            while (position < scriptRichTextBox.Text.Length-1)
            {
                string command = ReadUntilSemicolon(scriptRichTextBox.Text, ref position).
					Replace('\r', new char()).Replace('\n', new char());
				string[] words = command.Split(new char[] { ' ' });
                //Search for defining variables
				if (types.Contains(words[0]))
				{
					if (words.Length == 2)
						variables.Add(words[1], GetInstance($"System.{words[0]}"));
					else variables.Add(words[1], GetInstance($"System.{words[0]}", words[3]));
				}
				//Search for commands

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
