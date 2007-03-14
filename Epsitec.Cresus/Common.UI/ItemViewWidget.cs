//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemViewWidget</c> class defines the root widgets which contain
	/// the graphic representation of the <see cref="ItemView"/> instances in
	/// a <see cref="ItemTable"/>.
	/// </summary>
	public class ItemViewWidget : Widgets.Widget
	{
		public ItemViewWidget(ItemView view)
		{
			this.view = view;
		}


		public ItemView ItemView
		{
			get
			{
				return this.view;
			}
		}

		private ItemView view;
	}
}
