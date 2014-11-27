//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Aider.Reporting;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
	public sealed class ActionAiderOfficeManagementViewController3AddEmployeeAndJob : ActionViewController<AiderOfficeManagementEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter un collaborateur");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderContactEntity,
										 EmployeeType,
										 string,
										 EmployeeActivity,
										 string,
										 AiderUserEntity,
										 string,
										 EmployeeJobFunction,
										 EmployeeEmployer> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
		{
			form
				.Title ("Ajouter un collaborateur")
				.Field<AiderContactEntity> ()
					.Title ("Contact")
				.End()
				.Field<EmployeeType> ()
					.Title ("Type de collaborateur")
					.InitialValue (EmployeeType.Pasteur)
				.End ()
				.Field<string> ()
					.Title ("Fonction")
				.End ()
				.Field<EmployeeActivity> ()
					.Title ("Degré d'activité")
					.InitialValue (EmployeeActivity.Active)
				.End ()
				.Field<string> ()
					.Title ("Numéro AVS")
					.InitialValue ("756.")
				.End ()
				.Field<AiderUserEntity> ()
					.Title ("Utilisateur associé")
				.End ()
				.Text("<b>Détails du poste</b>")
				.Text("Lieu d'église: " + this.Entity.OfficeShortName)
				.Field<string> ()
					.Title ("Site ou secteur")
				.End ()
				.Field<EmployeeJobFunction> ()
					.Title ("Fonction")
				.End ()
				.Field<EmployeeEmployer> ()
					.Title ("Employeur")
					.InitialValue (EmployeeEmployer.CS)
				.End ()
			.End ();
		}

		private void Execute(AiderContactEntity contact, 
							 EmployeeType employeeType, 
							 string function,
							 EmployeeActivity employeeActivity,
							 string navs13, 
							 AiderUserEntity user,
							 string detail, 
							 EmployeeJobFunction jobFunction, 
					         EmployeeEmployer employer)
		{
			var person = contact.Person;

			if(person.Employee.IsNotNull ())
			{
				throw new BusinessRuleException ("Cette personne est déjà définie comme collaborateur, ajouter un poste depuis le contact");
			}
	
			var employee = AiderEmployeeEntity.Create (this.BusinessContext, person, user, employeeType, function, employeeActivity, navs13);
			
			var job = this.BusinessContext.CreateAndRegisterEntity<AiderEmployeeJobEntity> ();
			job.Employee = employee;
			job.EmployeeJobFunction = jobFunction;
			job.Employer = employer;
			job.Office = this.Entity;
			job.Description = detail;
		}
	}
}
