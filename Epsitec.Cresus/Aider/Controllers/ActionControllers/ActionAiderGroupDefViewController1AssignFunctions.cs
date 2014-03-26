//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (1)]
	public sealed class ActionAiderGroupDefViewController1AssignFunctions : ActionViewController<AiderGroupDefEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Assigner des fonctions à ce groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupDefEntity> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupDefEntity> ()
					.Title ("Groupe fonctionnel")
					.WithSpecialField<AiderGroupDefSpecialField<AiderGroupDefEntity>>()
				.End ()
			.End ();
		}

		private void Execute(AiderGroupDefEntity functionalGroupDef)
		{
			if (functionalGroupDef.Subgroups.Count == 0)
			{
				throw new BusinessRuleException ("Ce groupe fonctionnel ne comporte aucune fonctions");
			}

			//Set functional group def
			this.Entity.Function = functionalGroupDef;
			//Create functional subgroups at right place
			var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate);
			foreach (var group in groupToComplete)
			{
				foreach (var function in functionalGroupDef.Subgroups)
				{
					group.CreateSubgroup (this.BusinessContext, function);
				}			
			}

		}
	}
}
