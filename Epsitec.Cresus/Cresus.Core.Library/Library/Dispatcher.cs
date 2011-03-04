//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public static class Dispatcher
	{
		public static void Queue(System.Action action)
		{
//-			CoreApplication.QueueAsyncCallback (action);
		}

		public static void RequestRefreshUI()
		{
//-			CoreProgram.Application.MainWindowController.Update ();
		}

		public static void QueueTasklets(string name, params TaskletJob[] jobs)
		{
//-			CoreApplication.QueueTasklets (name, jobs);
		}

		public static void ExecutePending()
		{
//-			CoreApplication.ExecuteAsyncCallbacks ();
		}
	}
}
