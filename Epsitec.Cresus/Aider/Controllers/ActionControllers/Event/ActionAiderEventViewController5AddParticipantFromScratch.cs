//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;
using System.Collections.Generic;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Aider.Data.Job;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (5)]
	public sealed class ActionAiderEventViewController5AddParticipantFromScratch : ActionViewController<AiderEventEntity>
	{
		public override FormattedText GetTitle()
		{
			return "Ajouter et créer une personne";
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Enumerations.PersonMrMrs, string, string, Date, Enumerations.EventParticipantRole> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderEventEntity, SimpleBrick<AiderEventEntity>> form)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;
			var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);

			form
				.Title ("Créer une personne")
				.Field<Enumerations.PersonMrMrs> ()
					.Title ("Titre")
				.End ()
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<string> ()
					.Title ("Prénom")
				.End ()
				.Field<Date> ()
					.Title ("Date de naissance")
					.InitialValue (Date.Today)
				.End ()
				.Field<Enumerations.EventParticipantRole> ()
					.Title ("Rôle")
				.End ()
			.End ();
		}

		private void Execute(Enumerations.PersonMrMrs mrMrs, string name, string firstName,Date dateOfBirst, Enumerations.EventParticipantRole role)
		{
			if (mrMrs == Enumerations.PersonMrMrs.None)
			{
				throw new BusinessRuleException ("impossible de determiner le sexe de la personne avec ce titre");
			}

			if(string.IsNullOrEmpty (name) || string.IsNullOrEmpty (firstName))
			{
				throw new BusinessRuleException ("un nom et un prénom et obligatoire");
			}

			var sex = Enumerations.PersonSex.Female;
			if (mrMrs == Enumerations.PersonMrMrs.Monsieur) 
			{
				sex = Enumerations.PersonSex.Male;
			}

			var eChPerson = new EChPerson (	"",
											name, 
											firstName,
											dateOfBirst, 
											sex, 
											Enumerations.PersonNationalityStatus.Unknown, 
											"", 
											Enumerable.Empty <EChPlace> (), 
											Enumerations.PersonMaritalStatus.None);

			var eChPersonEntity = EChDataHelpers.CreateEChPersonEntity (this.BusinessContext, 
																		eChPerson, 
																		Enumerations.DataSource.Undefined, 
																		Enumerations.PersonDeclarationStatus.NotDeclared);

			var person = AiderPersonEntity.Create (this.BusinessContext, eChPersonEntity, mrMrs);
			var contact = AiderContactEntity.Create (this.BusinessContext, person, Enumerations.AddressType.Default);
			AiderEventParticipantEntity.Create (this.BusinessContext, this.Entity, person, role);			
		}
	}
}
