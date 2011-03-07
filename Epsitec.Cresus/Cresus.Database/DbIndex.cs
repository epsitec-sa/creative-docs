//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;

namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbIndex</c> class represents an index definition for a <see cref="DbTable"/>.
	/// </summary>
	public sealed class DbIndex : IName
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="DbIndex"/> class.
		/// </summary>
		/// <param name="name">The name of the index.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public DbIndex(string name, IEnumerable<DbColumn> columns, SqlSortOrder sortOrder)
		{
			this.Name = name;
			this.Columns = columns.ToList ().AsReadOnly ();
			this.SortOrder = sortOrder;
		}


		/// <summary>
		/// Gets the name of the index.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the sort order.
		/// </summary>
		/// <value>The sort order.</value>
		public SqlSortOrder SortOrder
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public ReadOnlyCollection<DbColumn> Columns
		{
			get;
			private set;
		}


	}


}
