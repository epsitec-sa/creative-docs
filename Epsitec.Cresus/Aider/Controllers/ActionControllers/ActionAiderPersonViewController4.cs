//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (4)]
	public sealed class ActionAiderPersonViewController4 : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Ajouter une adresse");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Enumerations.AddressType> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			var person = this.Entity;

			form
				.Title ("Ajouter une adresse suppl�mentaire")
				.Text (TextFormatter.FormatText ("Vous �tes sur le point de cr�er une adresse suppl�mentaire pour", person.DisplayName, "."))
				.Field<Enumerations.AddressType> ()
					.Title ("Type d'adresse")
					.InitialValue (Enumerations.AddressType.Secondary)
				.End ()
			.End ();
		}

		private void Execute(Enumerations.AddressType addressType)
		{
			var person     = this.Entity;
			var newContact = this.BusinessContext.CreateAndRegisterEntity<AiderContactEntity> ();
			var newAddress = this.BusinessContext.CreateAndRegisterEntity<AiderAddressEntity> ();

			newContact.Person = person;
			newContact.Address = newAddress;
			newContact.AddressType = addressType;
			newContact.ContactType = Enumerations.ContactType.PersonAddress;
		}
	}
}