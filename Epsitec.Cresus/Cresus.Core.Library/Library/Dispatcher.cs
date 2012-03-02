//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public static class Dispatcher
	{
		public static void Queue(SimpleCallback action)
		{
			Application.QueueAsyncCallback (action);
		}

		public static void RemoveFromQueue(SimpleCallback action)
		{
			Application.RemoveQueuedAsyncCallback (action);
		}

		public static void RequestRefreshUI()
		{
//-			CoreProgram.Application.MainWindowController.Update ();
		}

		public static void QueueTasklets(string name, params TaskletJob[] jobs)
		{
			Application.QueueTasklets (name, jobs);
		}

		public static void ExecutePending()
		{
			Application.ExecuteAsyncCallbacks ();
		}
	}
}
