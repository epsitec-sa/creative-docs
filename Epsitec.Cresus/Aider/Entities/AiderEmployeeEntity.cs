//	Copyright © 2014-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEmployeeEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Person.GetCompactSummary (), "\n", this.EmployeeType, "(~", this.Description, "~)", "\n", this.EmployeeActivity);
		}

		public FormattedText GetEmployeeSummary()
		{
			var offices = FormattedText.Join ("\n", this.EmployeeJobs.Select (x => x.GetCompactSummary ()));

			return TextFormatter.FormatText (this.GetCompactSummary (), "\n", offices);
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

		public static AiderEmployeeEntity Create(BusinessContext businessContext, AiderPersonEntity person, AiderUserEntity user, EmployeeType employeeType, string function, EmployeeActivity employeeActivity, string navs13)
		{
			var employee    = businessContext.CreateAndRegisterEntity<AiderEmployeeEntity> ();

			employee.Person           = person;
			employee.PersonContact    = person.MainContact;
			employee.EmployeeType     = employeeType;
			employee.EmployeeActivity = employeeActivity;
			employee.Description      = function;
			employee.Navs13           = navs13;
			employee.User             = user;
			employee.ParishGroupPath  = user.ParishGroupPathCache;
			return employee;
		}

		public static void Delete(BusinessContext businessContext, AiderEmployeeEntity employee)
		{
			if (employee.IsNotNull ())
			{
				//Delete each jobs
				foreach (var job in employee.EmployeeJobs)
				{
					businessContext.DeleteEntity (job);
				}

				//Delete each referee
				foreach (var referee in employee.RefereeEntries)
				{
					businessContext.DeleteEntity (referee);
				}

				//Finally delete entity
				businessContext.DeleteEntity (employee);
			}			
		}

		public bool IsMinister()
		{
			switch (this.EmployeeType)
			{
				case Enumerations.EmployeeType.Diacre:
				case Enumerations.EmployeeType.Pasteur:
					return true;
			}

			return false;
		}

		private IList<AiderEmployeeJobEntity>		jobs;
		private IList<AiderRefereeEntity>			refereeEntries;
	}
}
