//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Linq;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Aider.Data.Common;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (3)]
	public sealed class ActionAiderGroupDefViewController3Move : ActionViewController<AiderGroupDefEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Déplacer la définition et les groupes";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<AiderGroupDefEntity> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
		{
			var count = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate).Count;
			form
				.Title (this.GetTitle ())
				.Field<AiderGroupDefEntity> ()
					.Title ("Parent")
					.WithSpecialField<AiderGroupDefSpecialField<AiderGroupDefEntity>>()
				.End ()
			.End ();
		}

		private void Execute(AiderGroupDefEntity newParentGroupDef)
		{
			if (newParentGroupDef == this.Entity)
			{
				var message = "Un groupe ne peut pas être déplacé dans lui-même";

				throw new BusinessRuleException (message);
			}

			if (newParentGroupDef.Level > AiderGroupIds.MaxGroupLevel)
			{
				var message = "Impossible de créer plus de " + (AiderGroupIds.MaxGroupLevel + 1) + " niveaux de groupes";

				throw new BusinessRuleException (message);
			}

			if (newParentGroupDef.IsChildOf (this.Entity))
			{
				var message = "Un groupe ne peut pas être déplacé dans un de ses sous groupes.";

				throw new BusinessRuleException (message);
			}

			var currentParent = AiderGroupDefEntity.FindParent (this.BusinessContext, this.Entity);


			if (AiderGroupIds.IsWithinRegion (currentParent.PathTemplate.Replace('_','0')) && !AiderGroupIds.IsWithinRegion (newParentGroupDef.PathTemplate.Replace('_','0')))
			{
				throw new BusinessRuleException ("Opération impossible, le groupe parent ne provient pas d'une région");
			}

			if (!AiderGroupIds.IsWithinRegion (currentParent.PathTemplate.Replace ('_', '0')) && AiderGroupIds.IsWithinRegion (newParentGroupDef.PathTemplate.Replace ('_', '0')))
			{
				throw new BusinessRuleException ("Opération impossible, le groupe parent provient d'une région");
			}

			if (AiderGroupIds.IsWithinParish (currentParent.PathTemplate.Replace ('_', '0')) && !AiderGroupIds.IsWithinParish (newParentGroupDef.PathTemplate.Replace ('_', '0')))
			{
				throw new BusinessRuleException ("Opération impossible, le groupe parent ne provient pas d'une paroisse");
			}

			if (!AiderGroupIds.IsWithinParish (currentParent.PathTemplate.Replace ('_', '0')) && AiderGroupIds.IsWithinParish (newParentGroupDef.PathTemplate.Replace ('_', '0')))
			{
				throw new BusinessRuleException ("Opération impossible, le groupe parent provient d'une paroisse");
			}

			var groupsToMove = AiderGroupEntity.FindGroupsFromGroupDef (this.BusinessContext, this.Entity);

			currentParent.Subgroups.Remove(this.Entity);
			
			var number = AiderGroupIds.FindNextSubGroupDefNumber (newParentGroupDef.Subgroups.Select (s => s.PathTemplate), 'D');
			this.Entity.PathTemplate = AiderGroupIds.CreateDefinitionSubgroupPath (newParentGroupDef.PathTemplate, number);
			this.Entity.Level = newParentGroupDef.Level + 1;

			var newParents = AiderGroupEntity.FindGroupsFromGroupDef (this.BusinessContext, newParentGroupDef);

			newParentGroupDef.SubgroupsAllowed = true;
			newParentGroupDef.Subgroups.Add (this.Entity);
			foreach (var newParent in newParents)
			{
				foreach (var group in groupsToMove)
				{
					if (AiderGroupIds.IsWithinRegion (group.Path))
					{
						if (AiderGroupIds.IsWithinSameRegion (group.Path, newParent.Path))
						{
							if (AiderGroupIds.IsWithinParish (group.Path))
							{
								if (AiderGroupIds.IsWithinSameParish (group.Path, newParent.Path))
								{
									group.Move (newParent, false);
								}
							}
							else
							{
								group.Move (newParent,false);
							}
						}
					}
					else
					{
						group.Move (newParent,false);
					}
				}
			}
		}
	}
}
