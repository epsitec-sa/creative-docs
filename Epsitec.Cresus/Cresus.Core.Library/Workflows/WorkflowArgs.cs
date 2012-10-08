//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowArgs</c> class is used to manipulate arguments associated with
	/// a <see cref="WorfklowThreadEntity"/>, stored as a <see cref="SettingsCollection"/>
	/// serialized into an <see cref="XmlBlobEntity"/>.
	/// </summary>
	public static class WorkflowArgs
	{
		public static int? GetActiveVariantId(SettingsCollection args = null)
		{
			return WorkflowArgs.GetIntValue (args, Keys.ActiveVariant);
		}

		public static void SetActiveVariantId(int? value, SettingsCollection args = null)
		{
			WorkflowArgs.SetIntValue (args, Keys.ActiveVariant, value);
		}

		
		private static int? GetIntValue(SettingsCollection args, string key)
		{
			if (args == null)
			{
				args = WorkflowExecutionEngine.Current.Transition.Thread.GetArgs ();
			}

			return args.GetIntValue (key);
		}

		private static void SetIntValue(SettingsCollection args, string key, int? value)
		{
			var transition      = WorkflowExecutionEngine.Current.Transition;
			var businessContext = transition.BusinessContext;
			var thread          = transition.Thread;

			if (args == null)
			{
				args = thread.GetArgs ();
				args.SetIntValue (key, value);
				thread.SetArgs (businessContext, args);
			}
			else
			{
				args.SetIntValue (key, value);
			}
		}

		#region Static Keys Class

		private static class Keys
		{
			public static readonly string		ActiveVariant = "ActiveVariant";
		}

		#endregion
	}
}
