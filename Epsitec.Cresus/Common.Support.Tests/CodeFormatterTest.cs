
using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGenerators;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class CodeFormatterTest
	{
		[Test]
		public void CheckWriteClass()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");
			
			formatter.WriteBeginClass (new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Sealed), "Class1");
			
			formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.Public), "Class1(int value)");
			formatter.WriteLine ("this.value = value;");
			formatter.WriteEndMethod ();
			
			formatter.WriteBeginProperty (new CodeAttributes (CodeVisibility.Public), "int Value");
			formatter.WriteBeginSetter (new CodeAttributes (CodeVisibility.Internal));
			formatter.WriteEndSetter ();
			formatter.WriteBeginGetter (CodeAttributes.Default);
			formatter.WriteLine ("return this.value;");
			formatter.WriteEndGetter ();
			formatter.WriteEndProperty ();
			
			formatter.WriteInstanceVariable (new CodeAttributes (CodeVisibility.Private, CodeAccessibility.Final, CodeAttributes.ReadOnlyAttribute), "int value");
			
			formatter.WriteEndClass ();
			
			formatter.WriteEndNamespace ();

			System.Console.Out.WriteLine (buffer);
		}
	}
}
