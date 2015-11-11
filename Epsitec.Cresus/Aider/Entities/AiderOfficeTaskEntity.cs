//	Copyright © 2014-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Enumerations;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeTaskEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Kind);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Process.Type, ":\n", this.Kind);
		}

		public AiderEntity GetSourceEntity<AiderEntity>(DataContext dataContext)
			where AiderEntity : AbstractEntity
		{
			return (AiderEntity) dataContext.GetPersistedEntity (this.SourceId);
		}

		public static AiderOfficeTaskEntity Create(
			BusinessContext businessContext, 
			OfficeTaskKind kind, 
			AiderOfficeManagementEntity office, 
			AiderOfficeProcessEntity process, 
			AbstractEntity source)
		{

			switch (kind)
			{
				case OfficeTaskKind.CheckParticipation:
					if (source.GetType () != typeof (AiderGroupParticipantEntity))
					{
						throw new BusinessRuleException ("Le type d'entité fournit ne correspond pas au genre de tâche");
					}
					break;
			}

			var task = businessContext.CreateAndRegisterEntity<AiderOfficeTaskEntity> ();
			task.Process  = process;
			task.Office   = office;
			task.Kind     = kind;
			task.IsDone   = false;
			task.SourceId = businessContext.DataContext.GetPersistedId (source);
			return task;
		}
	}
}
