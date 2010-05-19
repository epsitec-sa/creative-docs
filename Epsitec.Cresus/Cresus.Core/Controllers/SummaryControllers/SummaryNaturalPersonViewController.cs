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

			for (int i = 0; i < this.Entity.Contacts.Count; i++)
			{
				var contact = this.Entity.Contacts[i] as Entities.MailContactEntity;
                
				if (contact == null)
                {
					continue;
                }

				items.Add (
					new SummaryData
					{
						Rank				= 2000 + i,
						Name				= string.Format (System.Globalization.CultureInfo.InvariantCulture, "MailContact{0}", i),
						IconUri				= "Data.Mail",
						Title				= new FormattedText ("Adresse"),
						CompactTitle		= new FormattedText ("Adresse"),
						TextAccessor		= Accessor.Create (contact, x => UIBuilder.FormatText (x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name)),
						CompactTextAccessor = Accessor.Create (contact, x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name)),
					});
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
