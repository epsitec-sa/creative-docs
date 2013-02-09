using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderHouseholdViewController0 : BrickDeletionViewController<AiderHouseholdEntity>
	{
		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> action)
		{
			action
				.Title ("Détruire le ménage")
				.Text ("Êtes vous sûr de vouloir détruire ce ménage ?")
				.Field<string> ()
					.Title ("Intitulé")
					.InitialValue (x => x.DisplayName)
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
			var household = this.Entity;

			household.Delete (this.BusinessContext);
		}
	}
}
