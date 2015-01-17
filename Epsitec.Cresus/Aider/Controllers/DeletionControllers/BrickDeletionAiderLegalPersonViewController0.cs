//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderLegalPersonViewController0 : BrickDeletionViewController<AiderLegalPersonEntity>
	{
		protected override void GetForm(ActionBrick<AiderLegalPersonEntity, SimpleBrick<AiderLegalPersonEntity>> action)
		{
			action
				.Title ("Détruire la personne morale")
				.Text ("Êtes vous sûr de vouloir détruire cette personne morale ?")
				.Field<string> ()
					.Title ("Intitulé")
					.InitialValue (x => x.Name)
					.ReadOnly ()
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
					.InitialValue (x => x.Address.Town)
					.ReadOnly ()
				.End ()
				.Field<string> ()
					.Title ("Rue")
					.InitialValue (x => x.Address.Street)
					.ReadOnly ()
				.End ()
				.Field<string> ()
					.Title ("Numéro de maison (avec complément)")
					.InitialValue (x => x.Address.HouseNumberAndComplement)
					.ReadOnly ()
				.End ()
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, AiderTownEntity, string, string> (this.Execute);
		}

		private void Execute(string _1, AiderTownEntity _2, string _3, string _4)
		{
			var legalPerson = this.Entity;

			AiderLegalPersonEntity.Delete (this.BusinessContext, legalPerson);
		}
	}
}
