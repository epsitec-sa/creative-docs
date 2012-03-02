//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataRowComparer</c> class is used to compare two <see cref="EntityDataRow"/>
	/// instances, in order to provide efficient sorting.
	/// </summary>
	public sealed class EntityDataRowComparer : IComparer<EntityDataRow>
	{
		#region IComparer<EntityDataRow> Members

		public int Compare(EntityDataRow x, EntityDataRow y)
		{
			//	TODO: compare numbers if they exist... and also compare all other fields
			//	in case of an exact match...

			return string.CompareOrdinal (x.GetTextField (0), y.GetTextField (0));
		}

		#endregion

		public static readonly EntityDataRowComparer Instance = new EntityDataRowComparer ();
	}
}
