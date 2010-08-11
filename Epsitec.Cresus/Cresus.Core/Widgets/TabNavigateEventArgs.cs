//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TabNavigateEventArgs</c> class provides data for widget related
	/// cancelable TAB-key navigation events.
	/// </summary>
	public class TabNavigateEventArgs : CancelEventArgs
	{
		public TabNavigateEventArgs(TabNavigationDir direction)
		{
			this.direction = direction;
		}


		public TabNavigationDir Direction
		{
			get
			{
				return this.direction;
			}
		}

		/// <summary>
		/// Gets or sets the replacement widget. Setting a replacement widget to some
		/// non-<c>null</c> value means that the navigation should be forwarded to the
		/// specified widget.
		/// </summary>
		/// <value>The replacement widget.</value>
		public Widget ReplacementWidget
		{
			get;
			set;
		}

		private readonly TabNavigationDir direction;
	}
}