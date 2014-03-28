//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderGroupDefViewController2DeleteAll : ActionViewController<AiderGroupDefEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Supprimer la définition et les groupes";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool,bool> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
		{
			var count = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate).Count;
			form
				.Title (this.GetTitle ())
				.Field<bool> ()
					.Title ("Forcer la suppresion (les sous-groupes et participations seront supprimées)")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Confirmer la suppresion de " + count + " groupe(s) ?")
					.InitialValue (false)
				.End ()
			.End ();
		}

		private void Execute(bool force,bool confirmed)
		{
			if (confirmed)
			{
				var groupsToRemove = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate);
				foreach (var group in groupsToRemove)
				{
					AiderGroupEntity.Delete (this.BusinessContext, group, force);
				}

				this.BusinessContext.DeleteEntity (this.Entity);
			}

		}
	}
}
