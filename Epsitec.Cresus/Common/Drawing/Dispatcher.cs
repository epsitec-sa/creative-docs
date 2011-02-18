//	Copyright © 2007-2010, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

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
			Dispatcher.DummyHandleEater (Dispatcher.form.Handle);
			
			System.Diagnostics.Debug.Assert (Dispatcher.form.IsHandleCreated);
		}

		static void DummyHandleEater(System.IntPtr handle)
		{
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

		public static void Join(System.Threading.Thread thread)
		{
			int counter = Dispatcher.invokeCounter;
			int timeout = 0;

			while (thread.Join (timeout) == false)
			{
				if (Dispatcher.invokeCounter != counter)
				{
					counter = Dispatcher.invokeCounter;
					System.Windows.Forms.Application.DoEvents ();
				}

				timeout = System.Math.Min (10, timeout + 1);
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
				lock (Dispatcher.exclusion)
				{
					try
					{
						Dispatcher.isInvoking = true;
						Dispatcher.invokeCounter++;
						Dispatcher.form.Invoke (callback);
					}
					finally
					{
						Dispatcher.isInvoking = false;
					}
				}
			}
			else
			{
				callback.DynamicInvoke ();
			}
		}

#pragma warning disable 414

		private static System.Windows.Forms.Form form;
		private static object exclusion = new object ();
		private static bool isInvoking;
		private static int invokeCounter;
	}
}
