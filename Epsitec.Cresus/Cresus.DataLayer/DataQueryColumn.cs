//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	/// The <c>DataQueryColumn</c> class defines how a <see cref="DataBrowserRow"/>
	/// should be populated by <see cref="DataBrowser"/>.
	/// </summary>
	public sealed class DataQueryColumn : System.IEquatable<DataQueryColumn>, System.IComparable<DataQueryColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataQueryColumn"/> class.
		/// The <see cref="SortOrder"/> defaults to <c>None</c>.
		/// </summary>
		/// <param name="fieldPath">The field path.</param>
		public DataQueryColumn(EntityFieldPath fieldPath)
		{
			this.fieldPath = fieldPath;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataQueryColumn"/> class.
		/// </summary>
		/// <param name="entityFieldPath">The entity field path.</param>
		/// <param name="sortOrder">The sort order.</param>
		public DataQueryColumn(EntityFieldPath entityFieldPath, DataQuerySortOrder sortOrder)
		{
			this.fieldPath = entityFieldPath;
			this.sortOrder = sortOrder;
		}


		/// <summary>
		/// Gets the field path for the column.
		/// </summary>
		/// <value>The field path.</value>
		public EntityFieldPath FieldPath
		{
			get
			{
				return this.fieldPath;
			}
		}

		/// <summary>
		/// Gets the sort order for the column.
		/// </summary>
		/// <value>The sort order.</value>
		public DataQuerySortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		#region IEquatable<DataQueryColumn> Members

		public bool Equals(DataQueryColumn other)
		{
			if (other == null)
			{
				return false;
			}

			if (other.fieldPath.Equals (this.fieldPath))
			{
				return other.sortOrder == this.sortOrder;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IComparable<DataQueryColumn> Members

		public int CompareTo(DataQueryColumn other)
		{
			if (other == null)
			{
				return 1;
			}

			int result = this.fieldPath.CompareTo (other.fieldPath);

			if (result == 0)
			{
				result = System.Math.Sign (this.sortOrder - other.sortOrder);
			}
			
			return result;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as DataQueryColumn);
		}

		public override int GetHashCode()
		{
			return this.fieldPath.GetHashCode ();
		}

		
		private readonly EntityFieldPath		fieldPath;
		private readonly DataQuerySortOrder		sortOrder;
	}
}
