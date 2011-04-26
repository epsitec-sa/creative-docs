//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowThreadEntity
	{
		public WorkflowStepEntity CreationEvent
		{
			get
			{
				WorkflowStepEntity step = this.History.FirstOrDefault ();
				DataContext dataContext = this.GetDataContext ();

				return dataContext.WrapNullEntity (step);
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("Créé le ", this.History.First ().Date);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("Créé le ", this.History.First ().Date);
		}
	}
}
