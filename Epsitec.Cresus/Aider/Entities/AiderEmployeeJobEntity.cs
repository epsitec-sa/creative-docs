//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;


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
            var about = TextFormatter.FormatText (this.Description, "~,~", this.EmployeeJobFunction);
            return TextFormatter.FormatText (this.Office.OfficeName, "(~", about, "~)");
        }

        public FormattedText GetAddressLabelText(PostalAddressType type = PostalAddressType.Default)
        {
            return this.Employee.PersonContact.GetAddressLabelText (type);
        }

        public bool IsUser()
        {
            return this.EmployeeJobFunction == EmployeeJobFunction.GestionnaireAIDER
                || this.EmployeeJobFunction == EmployeeJobFunction.SuppléantAIDER
                || this.EmployeeJobFunction == EmployeeJobFunction.UtilisateurAIDER;
        }

        public void Delete(BusinessContext context)
        {
            if (this.IsUser ())
            {
                Logic.BusinessRuleException (this, Resources.Text ("Un poste de type bénévole AIDER ne peut pas être supprimé ainsi."));
            }
            else
            {
                context.DeleteEntity (this);
            }
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
			job.ParishGroupPath = office.ParishGroupPathCache;

			return job;
		}

		public static AiderEmployeeJobEntity CreateOfficeUser(
			BusinessContext context, 
			AiderEmployeeEntity employee, 
			AiderOfficeManagementEntity office,
			string detail)
		{
			var job = context.CreateAndRegisterEntity<AiderEmployeeJobEntity> ();

			job.Employee = employee;
			job.EmployeeJobFunction = EmployeeJobFunction.UtilisateurAIDER;
			job.Employer = EmployeeEmployer.None;
			job.Office = office;
			job.Description = detail;
			job.ParishGroupPath = office.ParishGroupPathCache;

			return job;
		}

		public static AiderEmployeeJobEntity CreateOfficeManager(
			BusinessContext context,
			AiderEmployeeEntity employee,
			AiderOfficeManagementEntity office,
			string detail)
		{
			var job = context.CreateAndRegisterEntity<AiderEmployeeJobEntity> ();

			job.Employee = employee;
			job.EmployeeJobFunction = EmployeeJobFunction.GestionnaireAIDER;
			job.Employer = EmployeeEmployer.None;
			job.Office = office;
			job.Description = detail;
			job.ParishGroupPath = office.ParishGroupPathCache;

			return job;
		}
	}
}

