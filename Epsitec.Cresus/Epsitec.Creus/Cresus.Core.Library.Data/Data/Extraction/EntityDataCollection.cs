//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataCollection</c> class stores a sorted collection of <see cref="EntityDataRow"/>
	/// and is managed by <see cref="EntityDataExtractor"/>.
	/// </summary>
	public sealed class EntityDataCollection
	{
		internal EntityDataCollection(IComparer<EntityDataRow> comparer, IEnumerable<EntityDataRow> rows)
		{
			this.comparer     = comparer;
			this.rows         = new List<EntityDataRow> (rows);
			this.isSortNeeded = true;
		}


		public FormattedText					Label
		{
			get;
			set;
		}

		public IList<EntityDataRow>				Rows
		{
			get
			{
				this.EnsureSorted ();
				return this.rows.AsReadOnly ();
			}
		}

		public long								Id
		{
			get;
			set;
		}


		internal void Fill(IEnumerable<EntityDataRow> rows)
		{
			this.rows.Clear ();
			this.rows.AddRange (rows);
			this.isSortNeeded = true;
		}

		internal void Insert(EntityDataRow row)
		{
			if (this.isSortNeeded)
			{
				this.rows.Add (row);
			}
			else
			{
				int index = this.FindByBisection (row);

				this.rows.Insert (index, row);
			}
		}

		internal void Remove(EntityDataRow row)
		{
			if (this.isSortNeeded)
			{
				if (this.rows.Remove (row) == false)
				{
					throw new System.ArgumentException ("The specified row cannot be found in the collection");
				}
			}
			else
			{
				int index = this.FindExisting (row);

				if (index < 0)
				{
					throw new System.ArgumentException ("The specified row cannot be found in the collection");
				}

				this.rows.RemoveAt (index);
			}
		}

		
		private void EnsureSorted()
		{
			if (this.isSortNeeded)
			{
				this.rows.Sort (this.comparer);
				this.isSortNeeded = false;
			}
		}

		
		private int FindByBisection(EntityDataRow row)
		{
			System.Diagnostics.Debug.Assert (this.isSortNeeded == false);

			int minIndex = 0;
			int maxIndex = this.rows.Count - 1;
			
			while (maxIndex >= minIndex)
			{
				int probe  = (maxIndex - minIndex) / 2 + minIndex;
				int result = this.comparer.Compare (row, this.rows[probe]);

				if (result < 0)
				{
					maxIndex = probe-1;
				}
				else if (result > 0)
				{
					minIndex = probe+1;
				}
				else
				{
					return probe;
				}
			}

			return minIndex;
		}

		private int FindExisting(EntityDataRow row)
		{
			int pos   = this.FindByBisection (row);
			int count = this.rows.Count;

			int posBefore = pos;
			int posAfter  = pos+1;

			//	Locate an exact match, starting at a position where the bisection told us
			//	that we had a comparison match. Explore before/after the position, step by
			//	step, until we find the row or we no longer find equal rows.

			while ((posBefore >= 0) || (posAfter < count))
			{
				if ((posBefore >= 0) &&
					(this.comparer.Compare (row, this.rows[posBefore]) == 0))
				{
					if (this.rows[posBefore] == row)
					{
						return posBefore;
					}

					posBefore--;
				}
				else
				{
					posBefore = -1;
				}

				if ((posAfter < count) &&
					(this.comparer.Compare (row, this.rows[posAfter]) == 0))
				{
					if (this.rows[posAfter] == row)
					{
						return posAfter;
					}

					posAfter++;
				}
				else
				{
					posAfter = count;
				}
			}

			return -1;
		}

		
		private readonly IComparer<EntityDataRow>	comparer;
		private readonly List<EntityDataRow>		rows;
		private bool								isSortNeeded;
	}
}
