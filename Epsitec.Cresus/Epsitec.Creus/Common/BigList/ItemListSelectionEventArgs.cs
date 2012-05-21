//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListSelectionEventArgs : EventArgs
	{
		public ItemListSelectionEventArgs()
		{
			this.infos = new List<ItemListSelectionInfo> ();
		}


		public int Count
		{
			get
			{
				return this.infos.Count;
			}
		}

		public IList<ItemListSelectionInfo> Items
		{
			get
			{
				return this.infos.AsReadOnly ();
			}
		}


		public void Add(int index, bool isSelected)
		{
			this.infos.Add (new ItemListSelectionInfo (index, isSelected));
		}


		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			foreach (var item in this.infos)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (" ");
				}

				buffer.Append (item.Index);
				buffer.Append (":");
				buffer.Append (item.IsSelected ? "sel" : "---");
			}

			return buffer.ToString ();
		}


		private readonly List<ItemListSelectionInfo> infos;
	}
}
