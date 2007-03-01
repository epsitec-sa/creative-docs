//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>IWorkInProgressReport</c> interface is used by a worker thread
	/// to interact with a <see cref="WorkInProgressDialog"/> dialog.
	/// </summary>
	public interface IWorkInProgressReport
	{
		void DefineOperation(string formattedText);
		void DefineProgress(double value, string formattedText);

		bool Cancelled
		{
			get;
		}
	}
}
