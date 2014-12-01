//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{
	public sealed class Result
	{
		internal Result(IEnumerable<Table> tables)
		{
			tables.ThrowIfNull ("tables");

			this.Tables = tables.ToList ().AsReadOnly ();
		}


		public ReadOnlyCollection<Table>		Tables
		{
			get;
			private set;
		}

		public override string ToString()
		{
			int tableCount = this.Tables.Count;
			int rowCount = this.Tables.Sum (x => x.Rows.Count);
			
			return string.Format ("{0} tables, total {1} rows", tableCount, rowCount);
		}
	}
}
