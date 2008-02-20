//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>FocusChangingEventArgs</c> class contains the detailed information
	/// about a focus change. This event is cancellable.
	/// </summary>
	public class FocusChangingEventArgs : Support.CancelEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FocusChangingEventArgs"/> class.
		/// </summary>
		/// <param name="oldFocus">The old focus.</param>
		/// <param name="newFocus">The new focus.</param>
		/// <param name="tabNavigationDirection">The tab navigation direction.</param>
		/// <param name="tabNavigationMode">The tab navigation mode.</param>
		public FocusChangingEventArgs(Widget oldFocus, Widget newFocus, TabNavigationDir tabNavigationDirection, TabNavigationMode tabNavigationMode)
		{
			this.oldFocus = oldFocus;
			this.newFocus = newFocus;
			this.dir = tabNavigationDirection;
			this.mode = tabNavigationMode;
		}

		/// <summary>
		/// Gets the old focus.
		/// </summary>
		/// <value>The old focus.</value>
		public Widget OldFocus
		{
			get
			{
				return this.oldFocus;
			}
		}

		/// <summary>
		/// Gets the new focus.
		/// </summary>
		/// <value>The new focus.</value>
		public Widget NewFocus
		{
			get
			{
				return this.newFocus;
			}
		}

		/// <summary>
		/// Gets the tab navigation direction.
		/// </summary>
		/// <value>The tab navigation direction.</value>
		public TabNavigationDir TabNavigationDirection
		{
			get
			{
				return this.dir;
			}
		}

		/// <summary>
		/// Gets the tab navigation mode.
		/// </summary>
		/// <value>The tab navigation mode.</value>
		public TabNavigationMode TabNavigationMode
		{
			get
			{
				return this.mode;
			}
		}

		private readonly Widget oldFocus;
		private readonly Widget newFocus;
		private readonly TabNavigationDir dir;
		private readonly TabNavigationMode mode;
	}
}
