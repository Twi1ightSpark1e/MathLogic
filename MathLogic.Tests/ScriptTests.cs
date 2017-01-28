using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MathLogic.Tests
{
	[TestClass]
	public class ScriptTests
	{
		[TestMethod]
		public void VariableDefining()
		{
			string script = @"int i; 
uint ui; 
bool b; 
char c; 
string s; 
float f; 
double d;";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(7, vars.Keys.Count);
				Assert.AreEqual("Int64", vars["i"].GetType().Name);
				Assert.AreEqual("UInt64", vars["ui"].GetType().Name);
				Assert.AreEqual("Boolean", vars["b"].GetType().Name);
				Assert.AreEqual("Char", vars["c"].GetType().Name);
				Assert.AreEqual("String", vars["s"].GetType().Name);
				Assert.AreEqual("Single", vars["f"].GetType().Name);
				Assert.AreEqual("Double", vars["d"].GetType().Name);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void VariableInitializing()
		{
			string script = @"int i = 1;
uint ui = 1; 
bool b = true; 
char c = 'a'; 
string s = ""a""; 
float f = 1.0; 
double d = 1.0;";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(7, vars.Keys.Count);
				Assert.AreEqual(Convert.ToInt64(1), vars["i"]);
				Assert.AreEqual(Convert.ToUInt64(1), vars["ui"]);
				Assert.AreEqual(true, vars["b"]);
				Assert.AreEqual('a', vars["c"]);
				Assert.AreEqual("a", vars["s"]);
				Assert.AreEqual(1.0, vars["f"]);
				Assert.AreEqual(1.0, vars["d"]);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void VariableAssigment()
		{
			string script = @"int i;
uint ui; 
bool b; 
char c; 
string s; 
float f; 
double d;

i = 1;
ui = 1;
b = true;
c = 'a';
s = ""a"";
f = 1.0;
d = 1.0;";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(7, vars.Keys.Count);
				Assert.AreEqual(Convert.ToInt64(1), vars["i"]);
				Assert.AreEqual(Convert.ToUInt64(1), vars["ui"]);
				Assert.AreEqual(true, vars["b"]);
				Assert.AreEqual('a', vars["c"]);
				Assert.AreEqual("a", vars["s"]);
				Assert.AreEqual(1.0, vars["f"]);
				Assert.AreEqual(1.0, vars["d"]);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void CastingToString()
		{
			string script = @"int i = 1;
uint ui = 1;
bool b = true;
char c = 'a';
string s = ""a"";
float f = 1.5;
double d = 1.5;

string r1 = i + ""b"";
string r2 = ui + ""b"";
string r3 = b + ""b"";
string r4 = c + ""b"";
string r5 = s + ""b"";
string r6 = f + ""b"";
string r7 = d + ""b"";";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(14, vars.Keys.Count);
				Assert.AreEqual("1b", vars["r1"]);
				Assert.AreEqual("1b", vars["r2"]);
				Assert.AreEqual("Trueb", vars["r3"]);
				Assert.AreEqual("ab", vars["r4"]);
				Assert.AreEqual("ab", vars["r5"]);
				Assert.AreEqual("1,5b", vars["r6"]);
				Assert.AreEqual("1,5b", vars["r7"]);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void PushMethod()
		{
			string script = @"int i = 1;
uint ui = 1; 
bool b = true; 
char c = 'a'; 
string s = ""a""; 
float f = 1.0; 
double d = 1.0;

push(i, i, b);
push(ui, ui, b);
push(b, b, b);
push(c, c, b);
push(s, s, b);
push(f, f, b);
push(d, d, b);";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(7, output.Count);
				Assert.AreEqual(7, vars.Keys.Count);
				Assert.IsTrue(output.All((x) => x.Final));
				Assert.AreEqual(output[0].Key, output[0].Value); Assert.AreEqual("1", output[0].Key);
				Assert.AreEqual(output[1].Key, output[1].Value); Assert.AreEqual("1", output[1].Key);
				Assert.AreEqual(output[2].Key, output[2].Value); Assert.AreEqual("True", output[2].Key);
				Assert.AreEqual(output[3].Key, output[3].Value); Assert.AreEqual("a", output[3].Key);
				Assert.AreEqual(output[4].Key, output[4].Value); Assert.AreEqual("a", output[4].Key);
				Assert.AreEqual(output[5].Key, output[5].Value); Assert.AreEqual("1", output[5].Key);
				Assert.AreEqual(output[6].Key, output[6].Value); Assert.AreEqual("1", output[6].Key);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void SimpleCondition()
		{
			string script = @"bool r1 = true;
bool r2 = false;
bool r3 = true;
bool r4 = false;

if (false)
	r1 = false;
if (true)
	r2 = true;
if (0)
	r3 = false;
if (1)
	r4 = true;";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(4, vars.Keys.Count);
				Assert.IsTrue(vars.All((x) => x.Value));
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void ComplexCondition()
		{
			string script = @"bool r1 = false;
bool r2 = false;
bool r3 = false;
bool r4 = false;

if (true || false)
	r1 = true;
if (1 != 2)
	r2 = true;
if ('a' == 'a')
	r3 = true;
if (""abc"" == ""abc"")
	r4 = true;";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(4, vars.Keys.Count);
				Assert.IsTrue(vars.All((x) => x.Value));
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void WhileLoop()
		{
			string script = @"int i = 1;
string s;

while (i < 10)
{
	s = s + i;
	i = i + 1;
}";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				Assert.AreEqual(0, output.Count);
				Assert.AreEqual(2, vars.Keys.Count);
				Assert.AreEqual("123456789", vars["s"]);
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void BreakInLoop()
		{
			string script = @"int i = 0;
int j = 0;

while (i < 4)
{
	j = 0;
	while (j < 4)
	{
		push(i, j, false);
		j = j + 1;
		if (j == 2)
			break;
	}
	i = i + 1;
	if (i == 2)
		break;
}";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				var testout = new List<Triple<string, string, bool>>();

				for (int i = 0; i < 2; i++)
					for (int j = 0; j < 2; j++)
						testout.Add(new Triple<string, string, bool>(i.ToString(), j.ToString(), false));

				Assert.AreEqual(testout.Count, output.Count);
				Assert.AreEqual(2, vars.Keys.Count);
				
				for (int i = 0; i < testout.Count; i++)
				{
					Assert.AreEqual(testout[i].Key, output[i].Key);
					Assert.AreEqual(testout[i].Value, output[i].Value);
					Assert.AreEqual(testout[i].Final, output[i].Final);
				}
			}
			else Assert.Fail();
		}

		[TestMethod]
		public void ContinueInLoop()
		{
			string script = @"int i = -1;
int j;

while (i < 4)
{
	i = i + 1;
	if (i == 2)
		continue;
	j = -1;
	while (j < 4)
	{
		j = j + 1;
		if (j == 2)
			continue;
		push(i, j, false);
	}
}";
			Interpreter interpreter = new Interpreter();
			var state = interpreter.DoScript(script);
			if (state == Interpreter.WorkState.CommandsDone)
			{
				var vars = Interpreter.Variables;
				var output = Interpreter.Output;
				var testout = new List<Triple<string, string, bool>>();

				for (int i = 0; i < 5; i++)
				{
					if (i == 2)
						continue;
					for (int j = 0; j < 5; j++)
					{
						if (j == 2)
							continue;
						testout.Add(new Triple<string, string, bool>(i.ToString(), j.ToString(), false));
					}
				}

				Assert.AreEqual(testout.Count, output.Count);
				Assert.AreEqual(2, vars.Keys.Count);

				for (int i = 0; i < testout.Count; i++)
				{
					Assert.AreEqual(testout[i].Key, output[i].Key);
					Assert.AreEqual(testout[i].Value, output[i].Value);
					Assert.AreEqual(testout[i].Final, output[i].Final);
				}
			}
			else Assert.Fail();
		}
	}
}
