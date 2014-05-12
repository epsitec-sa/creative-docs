//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEmployeeEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.EmployeeType, "\n", this.Description, "~\n", this.EmployeeActivity);
		}

		public FormattedText GetEmployeeSummary()
		{
			var offices = this.EmployeeJobs.Select (x => x.Office.OfficeName);

			return TextFormatter.FormatText (this.EmployeeType, "\n", offices);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.EmployeeType, "(~", this.Description, "~)");
		}


		partial void GetEmployeeJobs(ref IList<AiderEmployeeJobEntity> value)
		{
			value = this.GetVirtualCollection (ref this.jobs, x => x.Employee = this).AsReadOnlyCollection ();
		}

		partial void GetRefereeEntries(ref IList<AiderRefereeEntity> value)
		{
			value = this.GetVirtualCollection (ref this.refereeEntries, x => x.Employee = this).AsReadOnlyCollection ();
		}
		
		
		private IList<AiderEmployeeJobEntity>		jobs;
		private IList<AiderRefereeEntity>			refereeEntries;
	}
}
