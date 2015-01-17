using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.WebCore.Server.Results
{
	/// <summary>
	/// This class is used for JSON serialization
	/// </summary>
	public class QueryResult
	{
		public int Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string RawQuery
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}
	}
}
