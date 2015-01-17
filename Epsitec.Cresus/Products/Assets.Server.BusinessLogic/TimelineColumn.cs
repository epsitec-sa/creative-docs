//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Une colonne d'une timeline pour un Timestamp donné, avec une ligne par objet.
	/// </summary>
	public class TimelineColumn
	{
		public TimelineColumn(int rowsCount, Timestamp timestamp, TimelineGroupedMode groupedMode)
		{
			this.Timestamp   = timestamp;
			this.GroupedMode = groupedMode;

			this.cells = new TimelineCell[rowsCount];

			for (int i=0; i<rowsCount; i++)
			{
				this.cells[i] = TimelineCell.Empty;
			}
		}

		public TimelineCell this[int index]
		{
			get
			{
				return this.cells[index];
			}
			set
			{
				this.cells[index] = value;
			}
		}


		public readonly Timestamp				Timestamp;
		public readonly TimelineGroupedMode		GroupedMode;

		private readonly TimelineCell[]			cells;
	}
}