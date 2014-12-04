//	Copyright © 2010-2014, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{
	public sealed class SqlSet
	{
		public SqlSet(IEnumerable<SqlField> values)
		{
			this.Values = values.ToList ();
		}


		public IEnumerable<SqlField>			Values
		{
			get;
			private set;
		}
	}
}
