//	Copyright © 2007, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Drawing.Platform
{
	/// <summary>
	/// The <c>Dispatcher</c> class can be used to invoke code execution on the
	/// same thread as the one which is running the user interface.
	/// </summary>
	public static class Dispatcher
	{
		/// <summary>
		/// Initializes the dispatcher.
		/// </summary>
		public static void Initialize()
		{
		}
		
		static Dispatcher()
		{
			Dispatcher.form = new System.Windows.Forms.Form ();
			Dispatcher.form.CreateControl ();
		}

		/// <summary>
		/// Gets a value indicating whether a call to <c>Invoke</c> is required
		/// to execute user interface code.
		/// </summary>
		/// <value><c>true</c> if a call to <c>Invoke</c> is required; otherwise, <c>false</c>.</value>
		public static bool InvokeRequired
		{
			get
			{
				return Dispatcher.form.InvokeRequired;
			}
		}

		/// <summary>
		/// Invokes the specified callback on the main user interface thread.
		/// </summary>
		/// <param name="callback">The callback to execute synchronously.</param>
		public static void Invoke(System.Delegate callback)
		{
			if (Dispatcher.InvokeRequired)
			{
				Dispatcher.form.Invoke (callback);
			}
			else
			{
				callback.DynamicInvoke ();
			}
		}

		private static System.Windows.Forms.Form form;
	}
}
