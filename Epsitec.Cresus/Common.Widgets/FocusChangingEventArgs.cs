//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public FocusChangingEventArgs(Widget oldFocus, Widget newFocus)
		{
			this.oldFocus = oldFocus;
			this.newFocus = newFocus;
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

		private readonly Widget oldFocus;
		private readonly Widget newFocus;
	}
}
