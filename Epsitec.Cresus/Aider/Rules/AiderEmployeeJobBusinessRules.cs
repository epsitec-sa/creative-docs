//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderEmployeeJobBusinessRules : GenericBusinessRule<AiderEmployeeJobEntity>
	{
		public override void ApplyValidateRule(AiderEmployeeJobEntity entity)
		{

		}

		public override void ApplyUpdateRule(AiderEmployeeJobEntity entity)
		{
			this.EnsureCorrectUserOffice (entity);
			this.EnsureCorrectUserParish (entity);
		}

		private void EnsureCorrectUserOffice(AiderEmployeeJobEntity entity)
		{
			if (entity.EmployeeJobFunction == Enumerations.EmployeeJobFunction.GestionnaireAIDER)
			{
				var user = entity.Employee.User;
				if (user.Office != entity.Office)
				{
					user.Office = entity.Office;
				}
			}
		}

		private void EnsureCorrectUserParish(AiderEmployeeJobEntity entity)
		{
			if (entity.EmployeeJobFunction == Enumerations.EmployeeJobFunction.UtilisateurAIDER)
			{
				var user = entity.Employee.User;
			}
		}
	}
}

