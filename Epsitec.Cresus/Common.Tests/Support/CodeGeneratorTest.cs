//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class CodeGeneratorTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Support.Res.Initialize ();
		}

		[Test]
		public void CheckEmit()
		{
			ResourceManager manager = Epsitec.Common.Support.Res.Manager;
			CodeGenerator generator = new CodeGenerator (manager);
			generator.Emit ();
			generator.Formatter.SaveCodeToTextFile ("Common.Support Entities.cs", System.Text.Encoding.UTF8);
		}

		[Test]
		public void CheckEmitEntity()
		{
			ResourceManager manager = Epsitec.Common.Support.Res.Manager;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);
			formatter.IndentationChars = "  ";
			CodeGenerator generator = new CodeGenerator (formatter, manager);

			Assert.AreEqual ("Epsitec.Common.Support", generator.SourceNamespaceRes);
			
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceDateTimeType);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceBaseType);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceCaption);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceBase);

			buffer.AppendLine ();

			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceCommand);
			generator.Emit (Epsitec.Common.Support.Res.Types.Shortcut);

			buffer.AppendLine ();

			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceString);
			//generator.Emit (Epsitec.Common.Support.Res.Types.TestInterface);
			//generator.Emit (Epsitec.Common.Support.Res.Types.TestInterfaceUser);

			formatter.Flush ();

			System.Console.Out.Write (buffer);
		}
	}
}
