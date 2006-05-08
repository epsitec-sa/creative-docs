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
			
			Widgets.Visual source1 = new Epsitec.Common.Widgets.Visual ();
			MySimpleDataSource source2 = new MySimpleDataSource ();

			collection.AddDataSource ("A", source1);
			collection.AddDataSource ("B", source2);

			collection.SetValue ("A.Name", "Source1");
			collection.SetValue ("B.x", 1);
			collection.SetValue ("B.y", "foo");

			Assert.AreEqual ("Source1", collection.GetValue ("A.Name"));
			Assert.AreEqual (-1, collection.GetValue ("A.Index"));
			Assert.AreEqual (1, collection.GetValue ("B.x"));
			Assert.AreEqual ("foo", collection.GetValue ("B.y"));

			foreach (string name in collection.GetFieldNames ())
			{
				System.Console.Out.WriteLine ("Name: {0}", name);

				IStructuredData data = collection.GetDataSource (name);
				IStructuredTree tree = data as IStructuredTree;

				Assert.IsNotNull (data);

				if (tree != null)
				{
					Assert.AreNotEqual ("B", name);
					
					System.Text.StringBuilder buffer1 = new System.Text.StringBuilder ();
					System.Text.StringBuilder buffer2 = new System.Text.StringBuilder ();

					foreach (string subPath in collection.GetFieldPaths (name))
					{
						buffer1.Append (subPath);
						buffer1.Append (" ");

						System.Console.Out.WriteLine ("  {0}", subPath);
					}

					foreach (string subPath in tree.GetFieldNames ())
					{
						buffer2.Append (name);
						buffer2.Append (".");
						buffer2.Append (subPath);
						buffer2.Append (" ");
					}

					Assert.AreEqual (buffer1.ToString (), buffer2.ToString ());
				}
				else
				{
					Assert.AreNotEqual ("A", name);
				}
			}
		}

		private class MySimpleDataSource : IStructuredData
		{
			public MySimpleDataSource()
			{
			}
			
			#region IStructuredData Members

			public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			public object GetValue(string path)
			{
				return this.data[path];
			}

			public void SetValue(string path, object value)
			{
				this.data[path] = value;
			}

			#endregion

			Dictionary<string, object> data = new Dictionary<string, object> ();
		}
	}
}
