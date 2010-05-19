//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : SummaryAbstractPersonViewController<Entities.NaturalPersonEntity>
	{
		public SummaryNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}


		public override void CreateUI(Common.Widgets.Widget container)
		{
//			base.CreateUI (container);

			var builder = new UIBuilder (container, this);
			var items   = new List<SummaryData> ();


			items.Add (
				new SummaryData
				{
					Rank				= 1000,
					Name				= "NaturalPerson",
					IconUri				= "Data.NaturalPerson",
					Title				= new FormattedText ("Personne physique"),
					CompactTitle		= new FormattedText ("Personne"),
					TextAccessor		= Accessor.Create (this.Entity, x => UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate)),
					CompactTextAccessor = Accessor.Create (this.Entity, x => UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname)),
				});


			var acc1 = IndirectAccessor<Entities.MailContactEntity>.Create (x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));
			var acc2 = acc1.GetAccessor (this.Entity.Contacts[0] as Entities.MailContactEntity);

			var template1 = new CollectionTemplate<Entities.MailContactEntity>
			{
				Filter				= x => x is Entities.MailContactEntity,
				TextAccessor		= IndirectAccessor<Entities.MailContactEntity>.Create (x => UIBuilder.FormatText (x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name)),
				CompactTextAccessor = IndirectAccessor<Entities.MailContactEntity>.Create (x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name)),
			};
			
			var res1 = acc2.ExecuteGetter ();

			CollectionAccessor.Create (this.Entity, template1, x => x.Contacts);

			for (int i = 0; i < this.Entity.Contacts.Count; i++)
			{
				var contact = this.Entity.Contacts[i];

				if (template1.IsCompatible (contact))
				{
					var data =
						new SummaryData
						{
							Rank				= 2000 + i,
							Name				= string.Format (System.Globalization.CultureInfo.InvariantCulture, "MailContact.{0}", i),
							IconUri				= "Data.Mail",
							Title				= new FormattedText ("Adresse"),
							CompactTitle		= new FormattedText ("Adresse"),
						};

					template1.BindSummaryData (data, contact);

					items.Add (data);
				}
			}


			builder.MapDataToTiles (items);
		}

		protected override void CreatePersonTile(UIBuilder builder)
		{
			var group = builder.CreateSummaryGroupingTile ("Data.NaturalPerson", "Personne physique");

			var accessor = new Accessors.NaturalPersonAccessor (this.Entity)
			{
				ViewControllerMode = ViewControllerMode.Edition
			};

			builder.CreateSummaryTile (group, accessor);
		}
	}
}
