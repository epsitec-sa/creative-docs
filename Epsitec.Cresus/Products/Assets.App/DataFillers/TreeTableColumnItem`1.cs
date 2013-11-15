//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class TreeTableColumnItem<T> : TreeTableColumnItem
		where T : ITreeTableCell
	{
		public TreeTableColumnItem()
		{
			this.rows = new List<T> ();
		}


		public IEnumerable<T> Rows
		{
			get
			{
				return this.rows;
			}
		}

		public void AddRow(T value)
		{
			this.rows.Add (value);
		}

		public override TItem[] GetArray<TItem>()
		{
			if (typeof (T) == typeof (TItem))
			{
				var rows = this.Rows as IEnumerable<TItem>;
				return rows.ToArray ();
			}

			throw new System.InvalidOperationException (string.Format ("Cannot cast from {0} to {1}", typeof (T).FullName, typeof (TItem).FullName));
		}


		private readonly List<T> rows;
	}
}
