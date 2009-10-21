//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>IDialog</c> interface must be implemented by every dialog.
	/// </summary>
	public interface IDialog
	{
		/// <summary>
		/// Opens the dialog (creates the window and makes it visible). If the
		/// dialog is modal, this method won't return until the user closes
		/// the dialog.
		/// </summary>
		void OpenDialog();

		/// <summary>
		/// Gets or sets the owner window for this dialog.
		/// </summary>
		/// <value>The owner window.</value>
		Window OwnerWindow
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		DialogResult Result
		{
			get;
		}
	}
}
