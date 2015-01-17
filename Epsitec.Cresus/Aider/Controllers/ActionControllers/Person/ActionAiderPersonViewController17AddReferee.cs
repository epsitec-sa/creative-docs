//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	[ControllerSubType (17)]
	public sealed class ActionAiderPersonViewController17AddReferee : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter une répondance rapide...");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupEntity, EmployeeReferenceType> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			if(user.IsAdmin ())
			{
				form
					.Title ("Ajouter une répondance")
					.Field<AiderGroupEntity> ()
						.Title ("Région")
					.End ()
					.Field<EmployeeReferenceType> ()
						.Title ("Répondance")
					.End ()
				.End ();
			}
			else
			{
				var region = user.Office.ParishGroup.GetRootGroup ();
				form
					.Title ("Ajouter une répondance")
					.Field<AiderGroupEntity> ()
						.Title ("Région")
						.InitialValue (this.BusinessContext.GetLocalEntity (region))
						.ReadOnly ()
					.End ()
					.Field<EmployeeReferenceType> ()
						.Title ("Répondance")
					.End ()
				.End ();			
			}
			
		}

		private void Execute(AiderGroupEntity group, EmployeeReferenceType referenceType)
		{
			if (group.IsNull ())
			{
				throw new BusinessRuleException ("Le groupe manque.");
			}
			if (group.IsRegion () == false)
			{
				throw new BusinessRuleException ("Le groupe ne fait pas référence à une région");
			}

			var employee = AiderEmployeeEntity.Create (this.BusinessContext, this.Entity, null, EmployeeType.None, "", EmployeeActivity.None, "");
			
			var referee  = this.BusinessContext.CreateAndRegisterEntity<AiderRefereeEntity> ();

			referee.Employee = employee;
			referee.ReferenceType = referenceType;
			referee.Group = group;
		}
	}
}
