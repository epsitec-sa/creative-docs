//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>IWorkInProgressReport</c> interface is used by a worker thread
	/// to interact with a <see cref="WorkInProgressDialog"/> dialog.
	/// </summary>
	public interface IWorkInProgressReport
	{
		/// <summary>
		/// Defines the operation which is currently being executed.
		/// </summary>
		/// <param name="formattedText">The formatted text.</param>
		void DefineOperation(string formattedText);

		/// <summary>
		/// Defines the progress of the operation, both using a value between
		/// 0 and 1 and a text message.
		/// </summary>
		/// <param name="value">The value between 0 and 1.</param>
		/// <param name="formattedText">The formatted text.</param>
		void DefineProgress(double value, string formattedText);

		/// <summary>
		/// Gets a value indicating whether the operation should be canceled.
		/// This will be true if the user pressed the cancel button.
		/// </summary>
		/// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
		bool Canceled
		{
			get;
		}
	}
}
