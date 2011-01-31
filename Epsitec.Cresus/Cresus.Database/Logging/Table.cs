using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Table
	{


		internal Table(string name, IEnumerable<Column> columns, IEnumerable<Row> rows)
		{
			name.ThrowIfNull ("name");
			columns.ThrowIfNull ("columns");
			rows.ThrowIfNull ("rows");

			this.Name = name;
			this.Columns = columns.ToList ().AsReadOnly ();
			this.Rows = rows.ToList ().AsReadOnly ();
		}


		public string Name
		{
			get;
			private set;
		}
		

		public ReadOnlyCollection<Column> Columns
		{
			get;
			private set;
		}


		public ReadOnlyCollection<Row> Rows
		{
			get;
			private set;
		}


	}


}
