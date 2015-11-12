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
	public partial class AiderOfficeProcessEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}

		public override FormattedText GetSummary()
		{
			var subject = new FormattedText ();
			switch (this.Type)
			{
				case OfficeProcessType.PersonsOutputProcess:
					subject = this.GetSourceEntity<AiderPersonEntity> (this.GetDataContext ()).GetSummary ();
					break;

			}
			return TextFormatter.FormatText (this.Type, "\n", subject);
		}

		public AiderOfficeTaskEntity StartTaskInOffice(
			BusinessContext businessContext, 
			OfficeTaskKind kind, 
			AiderOfficeManagementEntity office,
			AbstractEntity source)
		{
			var task = AiderOfficeTaskEntity.Create (businessContext, kind, office, this, source);
			this.Tasks.Add (task);

			return task;
		}

		public void SetNextStatus ()
		{
			if (this.Tasks.All (t => t.IsDone == false))
			{
				this.Status = OfficeProcessStatus.Started;
			}

			if (this.Tasks.Any (t => t.IsDone == true))
			{
				this.Status = OfficeProcessStatus.InProgress;
			}

			if (this.Tasks.All (t => t.IsDone == true))
			{
				this.Status = OfficeProcessStatus.Ended;
			}
		}

		public AiderEntity GetSourceEntity<AiderEntity>(DataContext dataContext)
			where AiderEntity : AbstractEntity
		{
			return (AiderEntity) dataContext.GetPersistedEntity (this.SourceId);
		}

		public static AiderOfficeProcessEntity Create(BusinessContext businessContext, OfficeProcessType type, AbstractEntity source)
		{
			switch (type)
			{
				case OfficeProcessType.PersonsOutputProcess:
					if (source.GetType () != typeof (AiderPersonEntity))
					{
						throw new BusinessRuleException ("Le type d'entité fournit ne correspond pas au processus métier");
					}
				break;

			}

			var process = businessContext.CreateAndRegisterEntity<AiderOfficeProcessEntity> ();
			process.CreationDate = System.DateTime.Now;
			process.Type  = type;
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
