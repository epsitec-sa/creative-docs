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

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeProcessEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}

		public AiderOfficeTaskEntity StartTaskInOffice(BusinessContext businessContext, AiderOfficeManagementEntity office)
		{
			var task = businessContext.CreateAndRegisterEntity<AiderOfficeTaskEntity> ();
			task.Process = this;
			task.Office  = office;
			task.IsDone  = false;

			this.Tasks.Add (task);
			return task;
		}

		public AiderEntity GetSourceEntity<AiderEntity>(BusinessContext businessContext)
			where AiderEntity : AbstractEntity
		{
			return (AiderEntity) businessContext.DataContext.GetPersistedEntity (this.SourceId);
		}

		public static AiderOfficeProcessEntity Create(BusinessContext businessContext, OfficeProcessType type, AbstractEntity source)
		{
			var process = businessContext.CreateAndRegisterEntity<AiderOfficeProcessEntity> ();
			process.CreationDate = System.DateTime.Now;
			process.Type		 = type;
			process.Status       = OfficeProcessStatus.Started;
			process.SourceId     = businessContext.DataContext.GetPersistedId (source);
			return process;
		}

		public static void Delete(BusinessContext context, AiderOfficeProcessEntity process)
		{
			foreach (var task in process.Tasks)
			{
				context.DeleteEntity (task);
			}

			context.DeleteEntity (process);
		}
	}
}
