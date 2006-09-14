using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class SourceTest
	{
		[SetUp] public void Initialize()
		{
			Common.Widgets.Widget.Initialize ();
			Common.Document.Engine.Initialize ();
			Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		
		[Test] public void CheckSourceGeneration()
		{
			Common.UI.Data.ObsoleteRecord record  = new Epsitec.Common.UI.Data.ObsoleteRecord ();
			Types.IDataValue[]    values  = SourceTest.CreateValues (out record);
			
			Source source = SourceTest.CreateSource (values);
			
			Assert.AreEqual ("Hello", source.Name);
			Assert.AreEqual (2, source.Methods.Length);
			Assert.AreEqual ("Main", source.Methods[0].Name);
			Assert.AreEqual ("Mysterious", source.Methods[1].Name);
			Assert.AreEqual ("Main:Void()", source.Methods[0].Signature);
			Assert.AreEqual ("Mysterious:Void(Integer,String,Integer)", source.Methods[1].Signature);
			
			string script_source = source.GenerateAssemblySource ();
			
			System.Console.Out.WriteLine (script_source);
			
			Engine engine = new Engine ();
			Script script = engine.Compile (source.Name, script_source);
			
			if (script.HasErrors)
			{
				foreach (string error in script.Errors)
				{
					System.Console.Out.WriteLine (error);
				}
			}
			
			object[] a_in  = { 12, "Hello" };
			object[] a_out;
			
			script.Attach (record);
			
			Assert.IsTrue (script.Execute ("Main"));
			Assert.IsTrue (script.Execute ("Mysterious", a_in, out a_out));
			Assert.IsFalse (script.Execute ("MissingMethod"));
			
			Assert.IsNotNull (a_out);
			Assert.AreEqual (2, a_out.Length);
			Assert.AreEqual ("HELLO", a_out[0]);
			Assert.AreEqual (24, a_out[1]);
			Assert.AreEqual ("hello", record["UserName"].Value);
			
			a_in = new object[] { "12", "Hello" };
			Assert.IsTrue (script.Execute ("Mysterious", a_in, out a_out));
			
			a_in = new object[] { 12, 34 };
			Assert.IsTrue (script.Execute ("Mysterious", a_in, out a_out));
			
			a_in = new object[] { "x", "y" };
			Assert.IsFalse (script.Execute ("Mysterious", a_in, out a_out));
			
			script.Dispose ();
		}
		
		[Test] public void CheckScriptWrapper()
		{
			Common.UI.Data.ObsoleteRecord record = new Epsitec.Common.UI.Data.ObsoleteRecord ();
			Types.IDataValue[]    values = SourceTest.CreateValues (out record);
			Source                source = SourceTest.CreateSource (values);
			
			ScriptWrapper wrapper = new ScriptWrapper ();
			
			wrapper.Source = source;
			wrapper.CreateXmlDocument (false).Save (System.Console.Out);
			
			string xml = wrapper.CreateXmlDocument (false).DocumentElement.OuterXml;
			
			wrapper.LoadFromXml (xml);
			
			string s1 = source.GenerateAssemblySource ();
			string s2 = wrapper.Source.GenerateAssemblySource ();
			
			Assert.AreEqual (s1, s2);
		}
		
		public static Source CreateSource(Types.IDataValue[] values)
		{
			Source.Method[]      methods = new Source.Method[2];
			Source.CodeSection[] code_1  = new Source.CodeSection[1];
			Source.CodeSection[] code_2  = new Source.CodeSection[1];
			Source.ParameterInfo[] par_2 = new Source.ParameterInfo[3];
			
			string code_1_source = "System.Diagnostics.Debug.WriteLine (&quot;Executing the &apos;Main&apos; script. UserName set to &apos;&quot; + this.UserName + &quot;&apos;.&quot;);<br/>";
			string code_2_source = "System.Diagnostics.Debug.WriteLine (&quot;Executing the &apos;Mysterious&apos; script. arg1=&quot; + arg1 + &quot;, arg2=&quot; + arg2);<br/>arg2 = arg2.ToUpper ();<br/>arg3 = arg1 * 2;<br/>this.UserName = arg2.ToLower ();<br/>";
			
			code_1[0]  = new Source.CodeSection (Source.CodeType.Local, code_1_source);
			code_2[0]  = new Source.CodeSection (Source.CodeType.Local, code_2_source);
			
			par_2[0] = new Source.ParameterInfo (Source.ParameterDirection.In, new Types.IntegerType (), "arg1");
			par_2[1] = new Source.ParameterInfo (Source.ParameterDirection.InOut, new Types.StringType (), "arg2");
			par_2[2] = new Source.ParameterInfo (Source.ParameterDirection.Out, new Types.IntegerType (), "arg3");
			
			methods[0] = new Source.Method ("Main", Types.VoidType.Default, null, code_1);
			methods[1] = new Source.Method ("Mysterious", Types.VoidType.Default, par_2, code_2);
			
			return new Source ("Hello", methods, values, "");
		}
		
		public static Types.IDataValue[] CreateValues(out Common.UI.Data.ObsoleteRecord record)
		{
			record  = new Epsitec.Common.UI.Data.ObsoleteRecord ();
			
			Types.IDataValue[]    values  = new Types.IDataValue[1];
			Common.UI.Data.ObsoleteField  field_1 = new Epsitec.Common.UI.Data.ObsoleteField ("UserName", "anonymous", new Types.StringType ());
			
			record.Add (field_1);
			
			values[0] = field_1;
			
			return values;
		}
	}
}
