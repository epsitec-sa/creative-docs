//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	public static class WorkflowArgs
	{
		public static int? GetActiveVariantId(SettingsCollection args = null)
		{
			return WorkflowArgs.GetIntValue (args, "ActiveVariant");
		}

		public static int? GetIntValue(SettingsCollection args, string key)
		{
			if (args == null)
			{
				args = WorkflowExecutionEngine.Current.Transition.Thread.GetArgs ();
			}

			return args.GetIntValue (key);
		}
	}
}
