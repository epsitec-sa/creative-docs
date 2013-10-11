//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (6)]
	public sealed class ActionAiderPersonWarningViewController62ProcessDeparture : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("La personne a déménagé");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool, bool, bool> (this.Execute);
		}

		private void Execute(bool confirmAddress, bool hidePerson, bool suppressSubscription)
		{
			var warning    = this.Entity;
			var person     = warning.Person;

			if (!confirmAddress && !hidePerson)
			{
				var message = "Il faut au moins choisir une options";

				throw new BusinessRuleException (message);
			}


			if (suppressSubscription)
			{
				var subscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, person.MainContact.Household);
				if (subscription.IsNotNull ())
				{
					AiderSubscriptionEntity.Delete (this.BusinessContext, subscription);
				}
			}

			if (hidePerson)
			{
				person.Visibility = PersonVisibilityStatus.Hidden;
			}

			this.ClearWarningAndRefreshCaches ();
			

#if false
			var warning    = this.Entity;
			var person     = warning.Person;
			var households = new HashSet<AiderHouseholdEntity> (person.Households);
			var members    = households.SelectMany (x => x.Members).Distinct ().ToList ();

			if (applyForAll == false)
			{
				members.Clear ();
				members.Add (person);
			}

			foreach (var member in members)
			{
				if (setInvisible)
				{
					member.Visibility = PersonVisibilityStatus.Hidden;
				}

				if (suppressSubscription && applyForAll)
				{
					foreach (var household in member.Households)
					{
						var subscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, household);
							
						if (subscription.IsNotNull ())
						{
							AiderSubscriptionEntity.Delete (this.BusinessContext, subscription);
						}
					}
				}

				if (removeFromHousehold)
				{
					var contacts = person.Contacts.Where (x => x.ContactType == ContactType.PersonHousehold);
					var contact = contacts.FirstOrDefault (x => x.Household == household);

					if (contacts.Count == 1)
					{
						var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
						AiderContactEntity.Create (this.BusinessContext, person, newHousehold, true);
					}

					if (contact.IsNotNull ())
					{
						AiderContactEntity.Delete (this.BusinessContext, contact);
					}
				}
			}

				//Auto-delete empty household
				foreach (var household in this.Entity.Person.Households)
				{
					if (household.Members.Count == 0)
					{
						this.BusinessContext.DeleteEntity (household);
					}
				}
			}
			else
			{
				if (setInvisible)
				{
					this.Entity.Person.Visibility = PersonVisibilityStatus.Hidden;
				}

				foreach (var household in this.Entity.Person.Households)
				{
					if (removeFromHousehold)
					{
						var person = this.Entity.Person;
						var contacts = person.Contacts;
						var contact = contacts.FirstOrDefault (x => x.Household == household);

						if (contacts.Count == 1)
						{
							var newHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
							AiderContactEntity.Create (this.BusinessContext, person, newHousehold, true);
						}

						if (contact.IsNotNull ())
						{
							AiderContactEntity.Delete (this.BusinessContext, contact);
						}
					}

					if (suppressSubscription)
					{
						var subscription = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, household);
						if (subscription.IsNotNull ())
						{
							AiderSubscriptionEntity.Delete (this.BusinessContext, subscription);
						}
					}
				}

				//Auto-delete empty household
				foreach (var household in this.Entity.Person.Households)
				{
					if (household.Members.Count == 0)
					{
						this.BusinessContext.DeleteEntity (household);
					}
				}
			}

			this.ClearWarningAndRefreshCaches ();
#endif
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			form
				.Title (this.GetTitle ())
				.Field<bool> ()
					.Title ("J'ai mis à jour l'adresse de la personne")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Cacher la personne")
					.InitialValue (false)
				.End ()
				.Field<bool> ()
					.Title ("Conserver l'abonnement Bonne Nouvelle")
					.InitialValue (false)
				.End ()
			.End ();
		}
	}
}
