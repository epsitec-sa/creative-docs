//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowEntity
	{
		public WorkflowEventEntity CreationEvent
		{
			get
			{
				return this.Events.FirstOrDefault ().WrapNullEntity ();
			}
		}

		public FormattedText GetCompactSummary()
		{
			return this.GetWorkflowName ();
		}

		public FormattedText GetSummary()
		{
			var    name    = this.GetWorkflowName ();
			string version = this.Id == 0 ? "" : string.Format ("Variante {0}", this.Id);

			return TextFormatter.FormatText (name, "\n", version);
		}
		
		private FormattedText GetWorkflowName()
		{
			FormattedText name;

			if (this.ActiveNodes.Count == 0)
			{
				name = TextFormatter.FormatText ("Workflow sans nom");
			}
			else
			{
				name = this.ActiveNodes[0].Name;
			}
			return name;
		}
	}
}
