//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	[TestClass]
	public class BrickWallTest
	{
		public BrickWallTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get;
			set;
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void CheckSyntax()
		{
			var wall = new BrickWall<Foo> ();

			wall.AddBrick (x => x.Name)
				.Name ("name")
				.Icon ("icon")
				.Title ("title")
				.Template ()
				.Title ("template title")
				.Text ("template text")
				.End ()
				.DebugDump ();

			wall.AddBrick (x => x.Bar)
				.Name ("name")
				.Icon ("icon")
				.Title ("title")
				.Template ()
				.Title ("template title")
				.Title (x => string.Format ("{0}", x.Value))
				.Title (x => x.Value)
				.Text ("template text")
				.End ()
				.DebugDump ();

			wall.AddBrick (x => x)
				.Title ("Article")
				.Icon ("Data.Article")

				.Input ()
				 .Title ("N° d'article")
				 .Field (x => x.Id).Width (74)
				.End ()

				.Separator ()

				.Input ()
				 .Title ("Nom de l'article")
				 .Field (x => x.Name)
				.End ()

				.DebugDump ();
		}

		class Foo
		{
			public int Id
			{
				get;
				set;
			}
			public string Name
			{
				get;
				set;
			}

			public Bar Bar
			{
				get;
				set;
			}
		}
		class Bar
		{
			public int Value
			{
				get;
				set;
			}
		}
	}
}
