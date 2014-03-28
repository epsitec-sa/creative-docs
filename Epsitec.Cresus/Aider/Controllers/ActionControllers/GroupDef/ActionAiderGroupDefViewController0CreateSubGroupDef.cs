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
	[ControllerSubType (0)]
	public sealed class ActionAiderGroupDefViewController0CreateSubGroupDef : ActionViewController<AiderGroupDefEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Créer un nouvelle sous-définition et les sous-groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, Enumerations.GroupClassification, bool, bool, bool> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<string> ()
					.Title ("Nom du sous groupe")
				.End ()
				.Field<Enumerations.GroupClassification> ()
					.Title ("Classification")
				.End ()
				.Field<bool> ()
					.Title ("Membres autorisés")
				.End ()
				.Field<bool> ()
					.Title ("Sous-groupes autorisés")
				.End ()
				.Field<bool> ()
					.Title ("Sous-groupes modifiables")
				.End ()
			.End ();
		}

		private void Execute(string name, Enumerations.GroupClassification groupClass, bool membersAllowed, bool subgroupsAllowed, bool isMutable)
		{
			if (string.IsNullOrWhiteSpace (name))
			{
				throw new BusinessRuleException ("Le nom ne peut pas être vide");
			}

			if (this.Entity.Classification == GroupClassification.Function)
			{
				throw new BusinessRuleException ("Impossible de créer une fonction de cette manière, veuiller assigner une fonction à ce groupe");
			}

			var groupDef = AiderGroupDefEntity.CreateDefinitionSubGroup (
																			this.BusinessContext,
																			this.Entity,
																			name,
																			groupClass,
																			subgroupsAllowed,
																			membersAllowed,
																			isMutable ? Mutability.Customizable : Mutability.None);

			//Create groups at right place
			var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate);
			foreach (var group in groupToComplete)
			{
				group.CreateSubgroup (this.BusinessContext, groupDef);
			}

		}
	}
}
