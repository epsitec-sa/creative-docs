//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
	public sealed class ActionAiderEventViewController3Validate : ActionViewController<AiderEventEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return this.Entity.State == Enumerations.EventState.ToValidate;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Valider définitivement l'acte");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create(this.Execute);
		}

		private void Execute()
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			if (user.CanValidateEvents () || user.IsAdmin ())
			{
				// check for existing act for main actors in the registry
				if ((this.Entity.Type != Enumerations.EventType.Marriage) || 
					(this.Entity.Type != Enumerations.EventType.CelebrationRegisteredPartners))
				{
					var echActors = this.Entity.GetMainActors ()
									.Where (a => a.IsExternal == false)
									.Select (a => a.Person)
									.Where (p => p.IsGovernmentDefined);
					echActors.ForEach (p =>
					{
						if (p.Events.Any (e => e.Type == this.Entity.Type))
						{
							throw new BusinessRuleException (string.Format ("Un acte existe dejà pour {0} dans le registre", p.GetFullName ()));
						}
					});
				}

				this.Entity.State     = Enumerations.EventState.Validated;
				this.Entity.Validator = this.BusinessContext.DataContext.GetLocalEntity (user);
				this.Entity.GetMainActors ().ForEach ((a) =>
				{
					if (a.IsExternal == false)
					{
						a.Person.Events.Add (this.Entity);
					}		
				});
				this.Entity.ApplyParticipantsInfo ();
				this.Entity.BuildMainActorsSummary ();
				var previousAct = AiderEventOfficeReportEntity.GetByEvent (this.BusinessContext, this.Entity);
				if(previousAct.IsNotNull ())
				{
					this.BusinessContext.DeleteEntity (previousAct);
				}
				this.BusinessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.None);

				var act        = AiderEventOfficeReportEntity.Create (this.BusinessContext, this.Entity);
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
				act.ProcessorUrl		= act.GetProcessorUrl (this.BusinessContext, "eventofficereport");
				this.Entity.Report = act;
				this.BusinessContext.SaveChanges (LockingPolicy.ReleaseLock);
			}
			else
			{
				Logic.BusinessRuleException ("Vous n'avez pas le droit de valider un acte");
			}
		}
	}
}