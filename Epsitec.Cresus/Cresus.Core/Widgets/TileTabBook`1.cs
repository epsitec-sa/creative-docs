//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TileTabBook{T}</c> implements a <see cref="TileTabBook"/> where the tab
	/// pages are identified by ids of type <c>T</c>.
	/// </summary>
	/// <typeparam name="T">The type used for the tab page ids.</typeparam>
	public class TileTabBook<T> : TileTabBook
	{
		public TileTabBook(IEnumerable<TabPageDef<T>> items)
			: base (items)
		{
		}

		
		public new IEnumerable<TabPageDef<T>> Items
		{
			get
			{
				return base.Items.Cast<TabPageDef<T>> ();
			}
		}

		
		public void SelectTabPage(T id)
		{
			this.SelectTabPage (this.Items.Where (x => x.Id.Equals (id)).FirstOrDefault ());
		}
	}
}
