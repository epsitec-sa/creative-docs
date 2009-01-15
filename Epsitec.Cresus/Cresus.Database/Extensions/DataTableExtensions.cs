//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Database.Extensions
{
	public static class DataTableExtensions
	{
		public static System.Data.DataRow[] ToArray(this System.Data.DataRowCollection collection)
		{
			System.Data.DataRow[] rows = new System.Data.DataRow[collection.Count];
			collection.CopyTo (rows, 0);
			return rows;
		}
	}
}
