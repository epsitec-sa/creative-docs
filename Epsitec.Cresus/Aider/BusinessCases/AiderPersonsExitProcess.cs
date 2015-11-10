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
			var process = AiderOfficeProcessEntity
							.Create (businessContext, OfficeProcessType.PersonsOutputProcess, person);
			
			var offices        = businessContext.GetAllEntities<AiderOfficeManagementEntity> ();
			var officesGroups  = offices.Select (o => o.ParishGroup);
			var groups   = person.GetParticipations ().Select (p => p.Group);
			
			foreach (var group in groups)
			{
				var searchPath = Enumerable.Repeat (group, 1).Union (group.Parents);
				var matchingGroups = searchPath.Intersect (officesGroups);
				if (matchingGroups.Any ())
				{
					var office = offices.Single (o => o.ParishGroup == matchingGroups.First ());
					process.StartTaskInOffice (businessContext, office);
				}
				else
				{

				}
			}

		}
	}
}
