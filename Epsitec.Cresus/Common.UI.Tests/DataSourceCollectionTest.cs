//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture] public class DataSourceCollectionTest
	{
		[Test]
		public void CheckCreation()
		{
			DataSourceCollection collection = new DataSourceCollection ();
			Widgets.Visual visual = new Epsitec.Common.Widgets.Visual ();

			collection.AddDataSource ("A", visual);

			foreach (string name in collection.GetFieldNames ())
			{
				System.Console.Out.WriteLine ("Name: {0}", name);

				foreach (string subPath in collection.GetFieldPaths (name))
				{
					System.Console.Out.WriteLine ("  {0}", subPath);
				}
			}
		}
	}
}
