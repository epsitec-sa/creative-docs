//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CurrentChangingEventArgs</c> class provides information for the
	/// <c>CurrentChanging</c> event.
	/// </summary>
	public class CurrentChangingEventArgs : Support.CancelEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CurrentChangingEventArgs"/> class,
		/// defaulting to a cancelable event.
		/// </summary>
		public CurrentChangingEventArgs()
			: this (true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CurrentChangingEventArgs"/> class.
		/// </summary>
		/// <param name="isCancelable">if set to <c>true</c>, the event is cancelable.</param>
		public CurrentChangingEventArgs(bool isCancelable)
		{
			this.isCancelable = isCancelable;
		}

		/// <summary>
		/// Gets a value indicating whether the event is cancelable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the event is cancelable; otherwise, <c>false</c>.
		/// </value>
		public bool IsCancelable
		{
			get
			{
				return this.isCancelable;
			}
		}

		private bool isCancelable;
	}
}
