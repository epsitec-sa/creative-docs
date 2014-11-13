//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP
using System.Linq;
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
	[ControllerSubType (5)]
	public sealed class ActionAiderGroupDefViewController5DeleteFunction : ActionViewController<AiderGroupDefEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Supprimer cette fonction";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool,bool>(this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<bool> ()
					.Title ("Forcer la suppresion (les participations seront supprimées)")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Confirmer la suppresion ?")
					.InitialValue (false)
				.End ()
			.End ();
		}

		private void Execute(bool force, bool confirmed)
		{
			if (confirmed)
			{
				AiderGroupDefEntity.DeleteFunctionSubGroup (this.BusinessContext, this.Entity, force);
			}	
		}
	}
}
