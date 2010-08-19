//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryRelationViewController : SummaryViewController<Entities.RelationEntity>
	{
		public SummaryRelationViewController(string name, Entities.RelationEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIRelation (data);
				this.CreateUIMailContacts (data);
				this.CreateUITelecomContacts (data);
				this.CreateUIUriContacts (data);
				this.CreateUIAffairs (data);
			}
		}

		protected override void OnChildItemCreated(AbstractEntity entity)
		{
			this.SetupNewContact (entity as AbstractContactEntity);
			base.OnChildItemCreated (entity);
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			
			this.UpdateRelationDefaultAddress ();
			
			context.UpdateEmptyEntityStatus (entity, isEmpty);
			context.UpdateEmptyEntityStatus (entity.Person, entity.Person.IsEmpty ());
		}

		private void CreateUIRelation(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Customer",
					IconUri				= "Data.Customer",
					Title				= TextFormatter.FormatText ("Client"),
					CompactTitle		= TextFormatter.FormatText ("Client"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", this.PersonText, "\n", "Représentant: ~", this.SalesRepresentativeText)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", this.PersonCompactText)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		private FormattedText PersonText
		{
			get
			{
				if (this.Entity.Person is Entities.NaturalPersonEntity)
				{
					var x = this.Entity.Person as Entities.NaturalPersonEntity;

					return TextFormatter.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")");
				}

				if (this.Entity.Person is Entities.LegalPersonEntity)
				{
					var x = this.Entity.Person as Entities.LegalPersonEntity;

					return TextFormatter.FormatText (x.Name);
				}

				return FormattedText.Empty;
			}
		}

		private FormattedText PersonCompactText
		{
			get
			{
				if (this.Entity.Person is Entities.NaturalPersonEntity)
				{
					var x = this.Entity.Person as Entities.NaturalPersonEntity;

					return TextFormatter.FormatText (x.Title.ShortName, x.Firstname, x.Lastname);
				}

				if (this.Entity.Person is Entities.LegalPersonEntity)
				{
					var x = this.Entity.Person as Entities.LegalPersonEntity;

					return TextFormatter.FormatText (x.Name);
				}

				return FormattedText.Empty;
			}
		}

		private FormattedText SalesRepresentativeText
		{
			get
			{
				if (this.Entity.SalesRepresentative is Entities.NaturalPersonEntity)
				{
					var x = this.Entity.SalesRepresentative as Entities.NaturalPersonEntity;

					return TextFormatter.FormatText (x.Title.ShortName, x.Firstname, x.Lastname);
				}

				return FormattedText.Empty;
			}
		}


		private void CreateUIMailContacts(SummaryDataItems data)
		{
			Common.CreateUIMailContacts (this.DataContext, data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			Common.CreateUITelecomContacts (this.DataContext, data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			Common.CreateUIUriContacts (this.DataContext, data, this.EntityGetter, x => x.Person.Contacts);
		}


		private void CreateUIAffairs(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Affair",
					IconUri		 = "Data.Affair",
					Title		 = TextFormatter.FormatText ("Affaires"),
					CompactTitle = TextFormatter.FormatText ("Affaires"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			var template = new CollectionTemplate<AffairEntity> ("Affair", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (GetAffairsSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (x.IdA));
			template.DefineSetupItem   (x => x.IdA = CoreProgram.Application.Data.GetNewAffairId ());

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Affairs, template));
		}

		private static string GetAffairsSummary(AffairEntity affairEntity)
		{
			int count = affairEntity.Events.Count;

			if (count == 0)
			{
				return affairEntity.IdA;
			}
			else
			{
				string date = Misc.GetDateTimeShortDescription (affairEntity.Events[0].Date);  // date du premier événement

				return string.Format ("{0} {1} ({2} év.)", date, affairEntity.IdA, count);
			}
		}


		private void UpdateRelationDefaultAddress()
		{
			var customer   = this.Entity;
			var oldAddress = customer.DefaultAddress;
			var newAddress = customer.Person.Contacts.Where (x => x is Entities.MailContactEntity).Cast<Entities.MailContactEntity> ().Select (x => x.Address).FirstOrDefault ();

			if (oldAddress.RefEquals (newAddress) == false)
			{
				customer.DefaultAddress = newAddress;
			}
		}

		private void SetupNewContact(AbstractContactEntity contact)
		{
			if (contact != null)
			{
				var naturalPerson = this.Entity.Person as NaturalPersonEntity;
				var legalPerson   = this.Entity.Person as LegalPersonEntity;

				//	Attach the contact to the customer's person entity, which can be either a natural
				//	person or a legal person:

				if (naturalPerson != null)
				{
					contact.NaturalPerson = naturalPerson;
				}
				if (legalPerson != null)
				{
					contact.LegalPerson = legalPerson;
				}
			}
		}
	}
}
