//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderGroupViewController0CreateSubGroup : TemplateActionViewController<AiderGroupEntity, AiderGroupEntity>
	{
		public override bool RequiresAdditionalEntity
		{
			get
			{
				return false;
			}
		}


		public override FormattedText GetTitle()
		{
			return "Créer un sous groupe";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}


		protected override void GetForm(ActionBrick<AiderGroupEntity, SimpleBrick<AiderGroupEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<string> ()
					.Title ("Nom du sous groupe")
				.End ()
			.End ();
		}

		private void Execute(string name)
		{
			if (string.IsNullOrWhiteSpace (name))
			{
				throw new BusinessRuleException ("Le nom ne peut pas être vide");
			}

			this.Entity.CreateSubgroup (this.BusinessContext, name);
		}
	}
}