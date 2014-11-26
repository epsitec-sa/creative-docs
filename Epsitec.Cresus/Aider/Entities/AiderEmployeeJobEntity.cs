//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public partial class AiderEmployeeJobEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Office.OfficeName, "\n", this.Description, "\n", this.EmployeeJobFunction, "\n", "Employeur :", this.Employer);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Office.OfficeName, "(~", this.Description, "~)");
		}
		
		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}

		public static AiderEmployeeJobEntity Create(
			BusinessContext context, 
			AiderEmployeeEntity employee, 
			Enumerations.EmployeeJobFunction function,
			Enumerations.EmployeeEmployer employer,
			AiderOfficeManagementEntity office,
			string detail)
		{
			var job = context.CreateAndRegisterEntity<AiderEmployeeJobEntity> ();

			job.Employee = employee;
			job.EmployeeJobFunction = function;
			job.Employer = employer;
			job.Office = office;
			job.Description = detail;

			return job;
		}
	}
}

