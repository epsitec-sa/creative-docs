//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Metadata.Tests.Vs
{
	[TestClass]
	public class UnitTestColumnFilterConstant
	{
		[TestMethod]
		public void CheckFromAndParse()
		{
			var entityKey = new EntityKey (Druid.Parse ("[J1AB1]"), new DbKey (new DbId (2)));

			var col1 = ColumnFilterConstant.From (entityKey);
			var col2 = ColumnFilterConstant.FromEnum (BindingMode.OneTime);
			var col3 = ColumnFilterConstant.From (10);
			var col4 = ColumnFilterConstant.From (100.123M);
			var col5 = ColumnFilterConstant.From ("Hello");
			var col6 = ColumnFilterConstant.From (new System.DateTime (2012, 9, 3));
			var col7 = ColumnFilterConstant.From (new Date (2012, 9, 3));
			var col8 = ColumnFilterConstant.From (new Time (11, 00, 59));
			var col9 = ColumnFilterConstant.From (false);
			var col10 = ColumnFilterConstant.From (42L);
			var col11 = ColumnFilterConstant.From ((bool?) null);

			Assert.AreEqual (ColumnFilterConstantType.EntityKey, col1.Type);
			Assert.AreEqual (ColumnFilterConstantType.Enumeration, col2.Type);
			Assert.AreEqual (ColumnFilterConstantType.Integer, col3.Type);
			Assert.AreEqual (ColumnFilterConstantType.Decimal, col4.Type);
			Assert.AreEqual (ColumnFilterConstantType.String, col5.Type);
			Assert.AreEqual (ColumnFilterConstantType.DateTime, col6.Type);
			Assert.AreEqual (ColumnFilterConstantType.Date, col7.Type);
			Assert.AreEqual (ColumnFilterConstantType.Time, col8.Type);
			Assert.AreEqual (ColumnFilterConstantType.Boolean, col9.Type);
			Assert.AreEqual (ColumnFilterConstantType.Long, col10.Type);
			Assert.AreEqual (ColumnFilterConstantType.Boolean, col11.Type);

			var ser1 = col1.ToString ();
			var ser2 = col2.ToString ();
			var ser3 = col3.ToString ();
			var ser4 = col4.ToString ();
			var ser5 = col5.ToString ();
			var ser6 = col6.ToString ();
			var ser7 = col7.ToString ();
			var ser8 = col8.ToString ();
			var ser9 = col9.ToString ();
			var ser10 = col10.ToString ();
			var ser11 = col11.ToString ();

			Assert.AreEqual (col1, ColumnFilterConstant.Parse (ser1));
			Assert.AreEqual (col2, ColumnFilterConstant.Parse (ser2));
			Assert.AreEqual (col3, ColumnFilterConstant.Parse (ser3));
			Assert.AreEqual (col4, ColumnFilterConstant.Parse (ser4));
			Assert.AreEqual (col5, ColumnFilterConstant.Parse (ser5));
			Assert.AreEqual (col6, ColumnFilterConstant.Parse (ser6));
			Assert.AreEqual (col7, ColumnFilterConstant.Parse (ser7));
			Assert.AreEqual (col8, ColumnFilterConstant.Parse (ser8));
			Assert.AreEqual (col9, ColumnFilterConstant.Parse (ser9));
			Assert.AreEqual (col10, ColumnFilterConstant.Parse (ser10));
			Assert.AreEqual (col11, ColumnFilterConstant.Parse (ser11));
		}
	}
}
