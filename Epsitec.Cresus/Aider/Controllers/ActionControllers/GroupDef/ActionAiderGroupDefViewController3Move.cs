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

		private void Execute(AiderGroupDefEntity parent)
		{
			var currentParent = AiderGroupDefEntity.FindParent(this.BusinessContext,this.Entity);
			currentParent.Subgroups.Remove(this.Entity);
			parent.SubgroupsAllowed = true;
			parent.Subgroups.Add (this.Entity);

			var groupsToMove = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate);
			
			
			var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (s => s.PathTemplate), 'D');
			this.Entity.PathTemplate = AiderGroupIds.CreateDefinitionSubgroupPath (parent.PathTemplate, number);
			var newParent = AiderGroupEntity.FindGroups (this.BusinessContext, AiderGroupIds.GetParentPath (this.Entity.PathTemplate)).Single ();
			
			foreach (var group in groupsToMove)
			{
				group.Move (newParent);
			}

			
		}
	}
}
