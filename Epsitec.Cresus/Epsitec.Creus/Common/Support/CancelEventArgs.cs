//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CancelEventArgs</c> class provides data for a cancelable event.
	/// </summary>
	public class CancelEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CancelEventArgs"/> class.
		/// </summary>
		public CancelEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CancelEventArgs"/> class.
		/// </summary>
		/// <param name="cancel">The default value of the <see cref="Cancel"/> property.</param>
		public CancelEventArgs(bool cancel)
		{
			this.Cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the action associated with the event
		/// should be cancelled.
		/// </summary>
		/// <value>
		///   <c>true</c> if the event should be cancelled; otherwise, <c>false</c>.
		/// </value>
		public bool Cancel
		{
			get;
			set;
		}
	}
}
