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

			setting.DefaultValueInclusionMode = InclusionMode.None;

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsFalse (setting.CheckInclusion ("B"));
			Assert.IsFalse (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			setting.DefaultValueInclusionMode = InclusionMode.Exclude;

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsFalse (setting.CheckInclusion ("B"));
			Assert.IsFalse (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));

			setting.DefaultValueInclusionMode = InclusionMode.Include;

			Assert.IsTrue (setting.CheckInclusion ("A"));
			Assert.IsTrue (setting.CheckInclusion ("B"));
			Assert.IsTrue (setting.CheckInclusion ("C"));
			Assert.IsFalse (setting.CheckInclusion ("X"));
		}
	}
}
