//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	/// The <c>DataQueryResult</c> class extends <see cref="DataQuery"/> with
	/// information gathered by <see cref="DataBrowser"/> when it generates
	/// the result.
	/// </summary>
	public sealed class DataQueryResult : DataQuery
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataQueryResult"/> class.
		/// </summary>
		public DataQueryResult(DataQuery originalQuery)
			: base (originalQuery)
		{
			this.entityIds = new List<Druid> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataQueryResult"/> class.
		/// </summary>
		/// <param name="originalQuery">The original query.</param>
		/// <param name="entityIds">The associated entity ids.</param>
		public DataQueryResult(DataQuery originalQuery, IEnumerable<Druid> entityIds)
			: this (originalQuery)
		{
			this.entityIds.AddRange (entityIds);
		}


		/// <summary>
		/// Gets the entity ids associated with the indexes produced by the
		/// <see cref="DbReader"/>.
		/// </summary>
		/// <value>The entity ids.</value>
		public IList<Druid> EntityIds
		{
			get
			{
				return this.entityIds;
			}
		}


		private readonly List<Druid> entityIds;
	}
}
