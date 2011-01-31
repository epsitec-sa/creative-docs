using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Row
	{

		internal Row(IEnumerable<object> values)
		{
			values.ThrowIfNull ("values");

			this.Values = values.ToList ().AsReadOnly ();
		}
		

		public ReadOnlyCollection<object> Values
		{
			get;
			private set;
		}


	}


}
