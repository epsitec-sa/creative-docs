//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Reporting;
using Epsitec.Common.Reporting.Settings;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	[TestFixture]
	public class VectorSettingTest
	{
		[Test]
		public void Check01InclusionMode()
		{
			VectorSetting setting = new VectorSetting ();

			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "A" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "X" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.None,    Id = "B" });

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsFalse (setting.CheckInclusion ("B"));
			Assert.IsFalse (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			Assert.AreEqual ("A", setting.CreateList (new string[] { "A", "B", "C", "X", "Y" }).Join (" "));
		}

		[Test]
		public void Check02InclusionMode()
		{
			VectorSetting setting = new VectorSetting ();

			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "A" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "X" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.None,    Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "*" });

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsFalse (setting.CheckInclusion ("B"));
			Assert.IsFalse (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			Assert.AreEqual ("A", setting.CreateList (new string[] { "A", "B", "C", "X", "Y" }).Join (" "));
		}

		[Test]
		public void Check03InclusionMode()
		{
			VectorSetting setting = new VectorSetting ();

			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "A" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "X" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.None,    Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "*" });

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsTrue (setting.CheckInclusion ("B"));
			Assert.IsTrue (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			Assert.AreEqual ("A B C Y", setting.CreateList (new string[] { "A", "B", "C", "X", "Y" }).Join (" "));
			Assert.AreEqual ("A", setting.CreateList (new string[] { "X" }).Join (" "));
			Assert.AreEqual ("A", setting.CreateList (new string[] { }).Join (" "));
		}

		[Test]
		public void Check04InclusionMode()
		{
			VectorSetting setting = new VectorSetting ();

			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "*" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "A" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "X" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.None,    Id = "B" });

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsTrue (setting.CheckInclusion ("B"));
			Assert.IsTrue (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			Assert.AreEqual ("B C Y A", setting.CreateList (new string[] { "A", "B", "C", "X", "Y" }).Join (" "));
			Assert.AreEqual ("A", setting.CreateList (new string[] { "X" }).Join (" "));
			Assert.AreEqual ("A", setting.CreateList (new string[] { }).Join (" "));
		}

		[Test]
		public void Check05InclusionMode()
		{
			VectorSetting setting = new VectorSetting ();

			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "*" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "A" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "B" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Exclude, Id = "X" });
			setting.Values.Add (new VectorValueSetting () { InclusionMode = InclusionMode.Include, Id = "Y" });

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsTrue (setting.CheckInclusion ("B"));
			Assert.IsTrue (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));
			Assert.IsTrue (setting.CheckInclusion ("Y"));

			Assert.AreEqual ("C A B Y", setting.CreateList (new string[] { "A", "B", "C", "X", "Y" }).Join (" "));
			Assert.AreEqual ("C D A B Y", setting.CreateList (new string[] { "Y", "X", "B", "C", "D", "A" }).Join (" "));
			Assert.AreEqual ("A B Y", setting.CreateList (new string[] { "X" }).Join (" "));
			Assert.AreEqual ("A B Y", setting.CreateList (new string[] { }).Join (" "));
		}
	}
}
