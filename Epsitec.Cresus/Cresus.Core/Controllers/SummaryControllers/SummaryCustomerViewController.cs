//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryCustomerViewController : SummaryViewController<Entities.CustomerEntity>
	{
		public SummaryCustomerViewController(string name, Entities.CustomerEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUICustomer (data);
			this.CreateUIMailContacts (data);
			this.CreateUITelecomContacts (data);
			this.CreateUIUriContacts (data);

			containerController.GenerateTiles ();
		}


		private void CreateUICustomer(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Customer",
					IconUri				= "Data.Customer",
					Title				= UIBuilder.FormatText ("Client"),
					CompactTitle		= UIBuilder.FormatText ("Client"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("N°", x.Id, "\n", this.PersonText, "\n", "~Représentant: ", this.SalesRepresentativeText)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("N°", x.Id, "\n", this.PersonCompactText)),
					EntityAccessor		= this.EntityGetter,
				});
		}

		private FormattedText PersonText
		{
			get
			{
				if (this.Entity.Person is Entities.NaturalPersonEntity)
				{
					var x = this.Entity.Person as Entities.NaturalPersonEntity;

					return UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate);
				}

				if (this.Entity.Person is Entities.LegalPersonEntity)
				{
					var x = this.Entity.Person as Entities.LegalPersonEntity;

					return UIBuilder.FormatText (x.Name);
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

					return UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname);
				}

				if (this.Entity.Person is Entities.LegalPersonEntity)
				{
					var x = this.Entity.Person as Entities.LegalPersonEntity;

					return UIBuilder.FormatText (x.Name);
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

					return UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname);
				}

				return FormattedText.Empty;
			}
		}


		private void CreateUIMailContacts(SummaryDataItems data)
		{
			Common.CreateUIMailContacts (data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			Common.CreateUITelecomContacts (data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			Common.CreateUIUriContacts (data, this.EntityGetter, x => x.Person.Contacts);
		}
	}
}
