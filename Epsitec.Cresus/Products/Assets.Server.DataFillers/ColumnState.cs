//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Etat d'une colonne, qui peut �tre visible ou cach�e. Lorsqu'une colonne est cach�e,
	/// on conserve sa largeur originale, utilis�e lorsqu'elle est rendue visible � nouveau.
	/// </summary>
	public struct ColumnState
	{
		public ColumnState(ObjectField field, int originalWidth, bool hide)
		{
			this.Field         = field;
			this.OriginalWidth = originalWidth;
			this.Hide          = hide;
		}

		public int								FinalWidth
		{
			get
			{
				return this.Hide ? 0 : this.OriginalWidth;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Field == ObjectField.Unknown;
			}
		}

		public static ColumnState Empty = new ColumnState (ObjectField.Unknown, 0, false);

		public readonly ObjectField				Field;
		public readonly int						OriginalWidth;
		public readonly bool					Hide;
	}
}