//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	[TestClass]
	public class BrickWallTest
	{
		[TestMethod]
		public void CheckSyntax()
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			for (int i = -1; i < 1000; i++)
			{
				var wall = new BrickWall<Foo> ();

				wall.AddBrick (x => x.Bar)
					.Name ("name")
					.Icon ("icon")
					.Title ("title")
					.Template ()
					.Title ("template title")
					.Text ("template text")
					.End ()
					;

				wall.AddBrick (x => x.Bars)
					.Name ("name")
					.Template ()
						.Icon ("icon")
						.Title ("template title")
						.Title (x => string.Format ("{0}", x.Value))
						.Title (x => x.Value)
						.Text ("template text")
					.End ()
					;

				wall.AddBrick ()
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
					;
				
				if (i < 0)
				{
					watch.Stop ();
					System.Diagnostics.Debug.WriteLine (string.Format ("First iteration took {0} ms", watch.ElapsedMilliseconds));
					watch.Reset ();
					watch.Start ();
				}
			}

			watch.Stop ();
			System.Diagnostics.Debug.WriteLine (string.Format ("Executed loop in {0} ms", watch.ElapsedMilliseconds));
		}

		class DummyEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				throw new System.NotImplementedException ();
			}

			public override string GetEntityStructuredTypeKey()
			{
				throw new System.NotImplementedException ();
			}
		}

		class Foo : DummyEntity
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
			public IList<Bar> Bars
			{
				get;
				set;
			}
		}

		class Bar : DummyEntity
		{
			public int Value
			{
				get;
				set;
			}
		}
	}
}
