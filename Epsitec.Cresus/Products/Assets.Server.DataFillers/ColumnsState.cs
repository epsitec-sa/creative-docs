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
		public ColumnsState(int[] mapper, ColumnState[] columns, SortedColumn[] sorted, int dockToLeftCount)
		{
			System.Diagnostics.Debug.Assert (mapper.Length == columns.Length);

			this.Mapper          = mapper;
			this.Columns         = columns;
			this.Sorted          = sorted;
			this.DockToLeftCount = dockToLeftCount;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Mapper == null || this.Mapper.Length == 0;
			}
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


		public static ColumnsState Empty = new ColumnsState (new int[0], new ColumnState[0], new SortedColumn[0], 0);


		public readonly int[]					Mapper;
		public readonly ColumnState[]			Columns;
		public readonly SortedColumn[]			Sorted;
		public readonly int						DockToLeftCount;
	}
}
