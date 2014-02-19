//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (0)]
	public sealed class ActionAiderGroupDefViewController0CreateSubGroups : TemplateActionViewController<AiderGroupDefEntity, AiderGroupDefEntity>
	{
		public override bool					RequiresAdditionalEntity
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


		protected override void GetForm(ActionBrick<AiderGroupDefEntity, SimpleBrick<AiderGroupDefEntity>> form)
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
			
			//TOFO INSTANCE
			//var groupDef = new AiderGroupDefEntity
			//Searching same levels group
			//var groupsToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (this.BusinessContext, this.Entity.Level, this.Entity.PathTemplate);
			//foreach (var group in groupsToComplete)
			//{
			//	
			//}
		}
	}
}
