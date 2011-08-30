//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Workflows;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Actions
{
	public static class DocumentActions
	{
		public static void ValidateDocument()
		{
		}

		public static void ShowPrintDialog()
		{
			throw new WorkflowException (WorkflowCancellation.Transition);
		}
	}
}
