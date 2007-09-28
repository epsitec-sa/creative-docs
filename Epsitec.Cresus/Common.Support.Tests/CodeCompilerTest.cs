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
		public void CheckBuildDriverCompile()
		{
			using (BuildDriver driver = new BuildDriver ())
			{
				driver.CreateBuildDirectory ();

				System.IO.File.WriteAllText (System.IO.Path.Combine (driver.BuildDirectory, "x.cs"),
					"namespace Foo.Bar\r\n" +
					"{\r\n" +
					"	public static class Demo\r\n" +
					"  {\r\n" +
					"    public static void DoSomething() { élémentaire (); }\r\n" + 
					"  }\r\n" +
					"}\r\n");

				CodeProjectSettings settings = driver.CreateSettings ("Foo.Bar");

				settings.References.Add (new CodeProjectReference ("System.Core"));
				settings.Sources.Add (new CodeProjectSource ("x.cs"));

				List<string> messages;

				Assert.IsFalse (driver.Compile (new CodeProject (settings)));

				messages = Types.Collection.ToList (driver.GetBuildMessages ());

				Assert.AreEqual (1, messages.Count);
				Assert.AreEqual ("x.cs(5,40): error CS0103: The name 'élémentaire' does not exist in the current context", messages[0]);
				Assert.IsNull (driver.GetCompiledAssemblyPath ());
				Assert.IsNull (driver.GetCompiledAssemblyDebugInfoPath ());

				System.IO.File.WriteAllText (System.IO.Path.Combine (driver.BuildDirectory, "x.cs"),
					"namespace Foo.Bar\r\n" +
					"{\r\n" +
					"	public static class Demo\r\n" +
					"  {\r\n" +
					"    public static void DoSomething() { }\r\n" + 
					"  }\r\n" +
					"}\r\n");

				Assert.IsTrue (driver.Compile (new CodeProject (settings)));

				messages = Types.Collection.ToList (driver.GetBuildMessages ());

				Assert.AreEqual (0, messages.Count);
				Assert.IsNotNull (driver.GetCompiledAssemblyPath ());
				Assert.IsNotNull (driver.GetCompiledAssemblyDebugInfoPath ());
			}
		}

		[Test]
		public void CheckBuildDriverHasValidFrameworkVersions()
		{
			BuildDriver driver = new BuildDriver ();

			Assert.IsTrue (driver.HasValidFrameworkVersions);
		}

		[Test]
		public void CheckCodeProjectReference()
		{
			CodeProjectReference r1 = CodeProjectReference.FromAssembly (typeof (int).Assembly);
			CodeProjectReference r2 = CodeProjectReference.FromAssembly (typeof (CodeProject).Assembly);

			Assert.AreEqual (@"<Reference Include=""System"" />", r1.ToSimpleString ());
			Assert.AreEqual (@"<Reference Include=""Common.Support"" />", r2.ToSimpleString ());

			Assert.AreEqual (@"<Reference Include=""System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL"" />", r1.ToString ());
			Assert.AreEqual ("<Reference Include=\"Common.Support, Version=2.0.0.0, Culture=neutral, PublicKeyToken=7344997cc606b490, processorArchitecture=MSIL\">\r\n  <HintPath>", r2.ToString ().Remove (145));

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

			System.Console.Out.WriteLine (project.CreateProjectSource ());
		}

		[Test]
		public void CheckCodeProjectSetProjectSettings()
		{
			CodeProject project = new CodeProject ();
			CodeProjectSettings settings = new CodeProjectSettings ();

			settings.References.Add (CodeProjectReference.FromAssembly (typeof (int).Assembly));
			settings.References.Add (CodeProjectReference.FromAssembly (typeof (long).Assembly));
			settings.References.Add (CodeProjectReference.FromAssembly (typeof (Druid).Assembly));
			settings.References.Add (CodeProjectReference.FromAssembly (typeof (TestAttribute).Assembly));
			settings.References.Add (CodeProjectReference.FromAssembly (typeof (System.Xml.XmlDocument).Assembly));

			settings.Sources.Add (new CodeProjectSource ("Abc.cs"));
			settings.Sources.Add (new CodeProjectSource ("Xyz.cs"));
			settings.Sources.Add (new CodeProjectSource ("abc.cs"));

			project.SetProjectSettings (settings);

			System.Console.Out.WriteLine (project.CreateProjectSource ());
		}
	}
}
