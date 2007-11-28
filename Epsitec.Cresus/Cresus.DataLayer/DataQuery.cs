//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataQuery</c> class defines a high level query used by the
	/// <see cref="DataBrowser"/> to define a SQL SELECT statement.
	/// </summary>
	public class DataQuery
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataQuery"/> class.
		/// </summary>
		public DataQuery()
		{
			this.outputFields = new List<EntityFieldPath> ();
		}


		/// <summary>
		/// The collection of output fields.
		/// </summary>
		/// <value>The collection of output fields.</value>
		public IList<EntityFieldPath> OutputFields
		{
			get
			{
				return this.outputFields;
			}
		}


		private readonly List<EntityFieldPath> outputFields;
	}
}
