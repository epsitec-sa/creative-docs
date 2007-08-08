
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
			EntityCodeGenerator generator = new EntityCodeGenerator (formatter, manager);
			
			Assert.AreEqual ("Epsitec.Common.Support", generator.SourceNamespace);

			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceDateTimeType);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceBaseType);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceCaption);
			generator.Emit (Epsitec.Common.Support.Res.Types.ResourceBase);

			StructuredType i1 = new StructuredType (StructuredTypeClass.Interface);
			i1.Caption.Name = "IFoo";

			generator.Emit (i1);

			System.Console.Out.Write (buffer);
		}
	}
}
