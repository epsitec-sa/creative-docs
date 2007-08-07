
using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGenerators;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
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
		public void CheckEmitEntity()
		{
			ResourceManager manager = Epsitec.Common.Support.Res.Manager;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);
			EntityCodeGenerator generator = new EntityCodeGenerator (manager);
			
			Assert.AreEqual ("Epsitec.Common.Support", generator.SourceNamespace);

			generator.EmitEntity (formatter, Epsitec.Common.Support.Res.Types.ResourceDateTimeType);
			generator.EmitEntity (formatter, Epsitec.Common.Support.Res.Types.ResourceBaseType);
			generator.EmitEntity (formatter, Epsitec.Common.Support.Res.Types.ResourceCaption);
			generator.EmitEntity (formatter, Epsitec.Common.Support.Res.Types.ResourceBase);

			System.Console.Out.Write (buffer);
		}
	}
}
