//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Exceptions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Tests.Vs.Types
{
	[TestClass]
	public sealed class UnitTestEnumType
	{
		[TestMethod]
		public void ToCompactString()
		{
			var mode1 = BindingMode.OneTime;
			var mode2 = System.StringSplitOptions.RemoveEmptyEntries;
			var mode3 = Epsitec.Common.Support.EntityEngine.EntityDataState.Unchanged;

			var value1 = EnumType.ToCompactString (mode1);
			var value2 = EnumType.ToCompactString (mode2);
			var value3 = EnumType.ToCompactString (mode3);

			Assert.AreEqual ("[100G]/1", value1);
			Assert.AreEqual ("System.StringSplitOptions/1", value2);
			Assert.AreEqual ("@ComSup.EntityEngine.EntityDataState/0", value3);
		}


		[TestMethod]
		public void FromCompactString()
		{
			var mode1 = BindingMode.OneTime;
			var mode2 = System.StringSplitOptions.RemoveEmptyEntries;
			var mode3 = Epsitec.Common.Support.EntityEngine.EntityDataState.Unchanged;

			var value1 = EnumType.FromCompactString ("[100G]/1");
			var value2 = EnumType.FromCompactString ("System.StringSplitOptions/1");
			var value3 = EnumType.FromCompactString ("@ComSup.EntityEngine.EntityDataState/0");

			Assert.AreEqual (mode1, value1);
			Assert.AreEqual (mode2, value2);
			Assert.AreEqual (mode3, value3);
		}
	}
}
