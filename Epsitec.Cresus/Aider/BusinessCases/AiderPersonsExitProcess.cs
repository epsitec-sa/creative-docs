//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Reporting;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Types;

namespace Epsitec.Aider.BusinessCases
{
	/// <summary>
	/// Exit Process Business Case
	/// </summary>
	public static class AiderPersonsExitProcess
	{
		public static void StartProcess(BusinessContext businessContext, AiderPersonEntity person)
		{
			/*var process = AiderOfficeProcessEntity
							.Create (businessContext, OfficeProcessType.PersonsExitProcess, person);
			*/
			var officesGroups  = businessContext.GetAllEntities<AiderOfficeManagementEntity> ().Select (o => o.ParishGroup);
			var groups   = person.GetParticipations ().Select (p => p.Group);
			
			foreach (var group in groups)
			{
				var search = Enumerable.Repeat (group, 1).Union (group.Parents);
				var office = search.Intersect (officesGroups);
				if (office.Any ())
				{

				}
				else
				{

				}
			}

		}
	}
}
