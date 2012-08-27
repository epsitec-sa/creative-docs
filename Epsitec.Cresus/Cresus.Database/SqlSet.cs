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


		public IEnumerable<SqlField> Values
		{
			get;
			private set;
		}
		

	}


}
