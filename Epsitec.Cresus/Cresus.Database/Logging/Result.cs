using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Result
	{


		internal Result(IEnumerable<Table> tables)
		{
			tables.ThrowIfNull ("tables");

			this.Tables = tables.ToList ().AsReadOnly ();
		}


		public ReadOnlyCollection<Table> Tables
		{
			get;
			private set;
		}


	}


}
