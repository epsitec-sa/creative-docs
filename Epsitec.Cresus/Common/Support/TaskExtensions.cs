//	Copyright © 2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Linq;
using System.Threading.Tasks;

namespace Epsitec.Common.Support
{
	public static class TaskExtensions
	{
		public static void ForgetSafely(this Task task)
		{
			task.ContinueWith (t => TaskExtensions.HandleException (t));
		}

		private static void HandleException(Task task)
		{
			if (task.IsCanceled)
			{
				System.Diagnostics.Debug.WriteLine ("Task was properly canceled");
			}
			if (task.Exception != null)
			{
				System.Diagnostics.Trace.WriteLine ("Asynchronous exception swallowed:\n" + task.Exception.GetFullText ());
			}
		}
	}
}
