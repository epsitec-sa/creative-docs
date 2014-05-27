//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Etats de toutes les colonnes. On conserve ici quelles sont les colonnes visibles,
	/// dans quel ordre, et comment elles sont tri�es.
	/// </summary>
	public struct ColumnsState
	{
		public ColumnsState(string name, int[] mapper, ColumnState[] columns, SortedColumn[] sorted, int dockToLeftCount)
		{
			System.Diagnostics.Debug.Assert (mapper.Length == columns.Length);

			this.Name            = name;
			this.Mapper          = mapper;
			this.Columns         = columns;
			this.Sorted          = sorted;
			this.DockToLeftCount = dockToLeftCount;
		}

		public IEnumerable<ColumnState> MappedColumns
		{
			get
			{
				for (int i=0; i<this.Mapper.Length; i++)
				{
					int j = this.Mapper[i];
					yield return this.Columns[j];
				}
			}
		}

		public int AbsoluteToMapped(int absRank)
		{
			return this.Mapper.ToList ().IndexOf (absRank);
		}

		public int MappedToAbsolute(int mappedRank)
		{
			return this.Mapper[mappedRank];
		}

		public readonly string					Name;
		public readonly int[]					Mapper;
		public readonly ColumnState[]			Columns;
		public readonly SortedColumn[]			Sorted;
		public readonly int						DockToLeftCount;
	}
}
