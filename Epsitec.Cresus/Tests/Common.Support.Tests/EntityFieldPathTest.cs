//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class EntityFieldPathTest
	{
		[Test]
		public void Check01StartsWith()
		{
			EntityFieldPath p0 = new EntityFieldPath ();
			EntityFieldPath p1 = EntityFieldPath.CreateRelativePath ("a");
			EntityFieldPath p2 = EntityFieldPath.CreateRelativePath ("a", "b");
			EntityFieldPath p3 = EntityFieldPath.CreateRelativePath ("a", "b", "c");
			EntityFieldPath p3a = EntityFieldPath.CreateRelativePath ("a", "b", "c");
			EntityFieldPath p4 = EntityFieldPath.CreateRelativePath ("x");

			Assert.IsTrue (p3.StartsWith (p0));
			Assert.IsTrue (p3.StartsWith (p1));
			Assert.IsTrue (p3.StartsWith (p2));
			Assert.IsTrue (p3.StartsWith (p3));
			Assert.IsTrue (p3.StartsWith (p3a));
			Assert.IsFalse (p3.StartsWith (p4));

			Assert.IsTrue (p2.StartsWith (p2));
			Assert.IsFalse (p2.StartsWith (p3));

			Assert.IsTrue (p1.StartsWith (p1));
			Assert.IsFalse (p1.StartsWith (p2));

			Assert.IsTrue (p0.StartsWith (p0));
			Assert.IsFalse (p0.StartsWith (p1));
		}

		[Test]
		public void Check02CreateRelativePath()
		{
			EntityFieldPath p0 = new EntityFieldPath ();
			EntityFieldPath p1 = EntityFieldPath.CreateRelativePath ("a");
			EntityFieldPath p2 = EntityFieldPath.CreateRelativePath ("a", "b");
			EntityFieldPath p3 = EntityFieldPath.CreateRelativePath (p1, "b", "c");
			EntityFieldPath p4 = EntityFieldPath.CreateRelativePath (p2, "c");
			EntityFieldPath p5 = EntityFieldPath.CreateRelativePath (p3);
			EntityFieldPath p6 = EntityFieldPath.CreateRelativePath (p0, "a", "b", "c");

			Assert.AreEqual ("a.b.c", p3.ToString ());
			Assert.AreEqual ("a.b.c", p4.ToString ());
			Assert.AreEqual ("a.b.c", p5.ToString ());
			Assert.AreEqual ("a.b.c", p6.ToString ());
		}
		
		[Test]
		public void Check03Count()
		{
			EntityFieldPath p0 = new EntityFieldPath ();
			EntityFieldPath p1 = EntityFieldPath.CreateRelativePath ("a");
			EntityFieldPath p2 = EntityFieldPath.CreateRelativePath ("a", "b");
			EntityFieldPath p3 = EntityFieldPath.CreateRelativePath ("a", "b", "c");

			Assert.AreEqual (0, p0.Count);
			Assert.AreEqual (1, p1.Count);
			Assert.AreEqual (2, p2.Count);
			Assert.AreEqual (3, p3.Count);
		}
	}
}
