//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class CodeCompilerTest
	{
		[Test]
		public void CheckBuildDriverIsValidInstallation()
		{
			BuildDriver driver = new BuildDriver ();

			Assert.IsTrue (driver.IsValidInstallation);
		}

		[Test]
		public void CheckCodeProjectReference()
		{
			CodeProjectReference r1 = CodeProjectReference.FromAssembly (typeof (int).Assembly);
			CodeProjectReference r2 = CodeProjectReference.FromAssembly (typeof (CodeProject).Assembly);

			Assert.AreEqual (@"<Reference Include=""System"" />", r1.ToSimpleString ());
			Assert.AreEqual (@"<Reference Include=""Common.Support"" />", r2.ToSimpleString ());

			Assert.AreEqual (@"<Reference Include=""System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL"" />", r1.ToString ());
			Assert.AreEqual (@"<Reference Include=""Common.Support, Version=2.0.0.0, Culture=neutral, PublicKeyToken=7344997cc606b490, processorArchitecture=MSIL"" />", r2.ToString ());

			Assert.IsTrue (r1.IsFrameworkAssembly ());
			Assert.IsFalse (r2.IsFrameworkAssembly ());
		}

		[Test]
		public void CheckCodeProjectReplace()
		{
			CodeProject project = new CodeProject ();

			project.Add (TemplateItem.ProjectGuid, "{1234}");
			project.Add (TemplateItem.CompileInsertionPoint, @"<Compile Include=""abc""/>");
			project.Add (TemplateItem.CompileInsertionPoint, @"<Compile Include=""xyz""/>");

			System.Console.Out.WriteLine (project.CreateProjectFile ());
		}
	}
}
