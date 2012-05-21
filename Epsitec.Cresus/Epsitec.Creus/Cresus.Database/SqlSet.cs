using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{

	public sealed class SqlSet
	{


		public SqlSet(DbRawType type, IEnumerable<object> values)
		{
			this.Type = type;
			this.Values = values.ToList ();
		}


		public DbRawType Type
		{
			get;
			private set;
		}


		public IEnumerable<object> Values
		{
			get;
			private set;
		}
		

	}


}
