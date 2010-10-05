﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionMailContactViewController : EditionViewController<MailContactEntity>
	{
		public EditionMailContactViewController(string name, MailContactEntity entity)
			: base (name, entity)
		{
		}

		//	Si this.Entity.NaturalPerson existe et this.Entity.LegalPerson nul :
		//		Les onglets sont présents et réglés sur "Adresse spécifique".
		//		On définit l'adresse spécifique d'une personne physique.
		//	
		//	Si this.Entity.NaturalPerson existe et this.Entity.LegalPerson existe :
		//		Les onglets sont présents et réglés sur "Adresse existante".
		//		On définit l'adresse existante (entreprise) d'une personne physique.
		//	
		//	Si this.Entity.NaturalPerson nul et this.Entity.LegalPerson existe :
		//		Pas d'onglets. On définit l'adresse spécifique d'une entreprise.

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Adresse");

				this.CreateUIRoles  (builder);
				this.CreateUICommon (builder);

				if (this.Entity.NaturalPerson.IsNull () &&
					this.Entity.LegalPerson.IsNotNull ())
				{
					this.CreateUICountry  (builder);
					this.CreateUIMain     (builder);
					this.CreateUILocation (builder);
				}
				else
				{
					this.CreateTabBook (builder);
					this.CreateTabBookLocalPage (builder);
					this.CreateTabBookGlobalPage (builder);

					if (this.Entity.LegalPerson.IsNull ())
					{
						this.tabBookContainer.SelectTabPage (TabPageId.Local);
					}
					else
					{
						this.tabBookContainer.SelectTabPage (TabPageId.Global);
					}
				}

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIComments (data);
			}
		}

		private void CreateUIRoles(UIBuilder builder)
		{
			var controller = new SelectionController<ContactRoleEntity> (this.BusinessContext)
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
			};

			builder.CreateEditionDetailedItemPicker ("ContactRoles", this.Entity, "Rôles souhaités", controller, Business.EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
		}

		private void CreateUICommon(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextFieldMulti (tile, 52, "Complément 1", Marshaler.Create (() => this.Entity.Complement, x => this.Entity.Complement = x));
			builder.CreateMargin         (tile, horizontalSeparator: false);
		}

		private enum TabPageId
		{
			Local,
			Global,
		}

		private void CreateTabBook(UIBuilder builder)
		{
			builder.CreateEditionTile ();

			this.tabBookContainer = builder.CreateTabBook (
				TabPageDef.Create (TabPageId.Local, "Adresse spécifique", this.HandleSelectTabPageLocal),
				TabPageDef.Create (TabPageId.Global, "Entreprise existante"));
		}

		private void CreateTabBookLocalPage(UIBuilder builder)
		{
			builder.BeginTileTabPage (TabPageId.Local);
			this.CreateUICountry (builder);
			this.CreateUIMain (builder);
			this.CreateUILocation (builder);
			builder.EndTileTabPage ();
		}
		
		private void CreateTabBookGlobalPage(UIBuilder builder)
		{
			builder.BeginTileTabPage (TabPageId.Global);
			this.CreateUILegalPerson (builder);
			this.CreateUIAddress (builder);
			builder.EndTileTabPage ();
		}
		
		private void HandleSelectTabPageLocal()
		{
			if (this.Entity.LegalPerson.IsNotNull ())
			{
				this.Entity.LegalPerson = EntityNullReferenceVirtualizer.CreateEmptyEntity<LegalPersonEntity> ();
				this.Entity.Address = this.DataContext.CreateEntityAndRegisterAsEmpty<AddressEntity> ();
				this.InitializeDefaultCountry ();  // met "Suisse" si rien
				this.selectedCountry = this.Entity.Address.Location.Country;

				this.TileContainer.UpdateAllWidgets ();
			}
		}


		private void CreateUILegalPerson(UIBuilder builder)
		{
			var controller = new SelectionController<LegalPersonEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.LegalPerson,
				ValueSetter         = x => this.Entity.LegalPerson = x,
				ReferenceController = this.GetLegalPersonReferenceController (),
				PossibleItemsGetter = () => this.Data.GetAllEntities<LegalPersonEntity> (),
			};

			var textField = builder.CreateAutoCompleteTextField ("Entreprise (personne morale)", controller);

			textField.SelectedItemChanged += delegate
			{
				System.Diagnostics.Debug.Assert (this.addressTextField != null);
				this.addressTextField.Text = null;  // on efface l'adresse si on change d'entreprise
				this.addressTextField.Items.Clear ();

				var addressGetter = this.GetLegalPersonAddressGetter ();
				foreach (var item in addressGetter)
				{
					this.addressTextField.Items.Add (item);
				}
			};
		}

		private void CreateUIAddress(UIBuilder builder)
		{
			var controller = new SelectionController<AddressEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.Address,
				ValueSetter         = x => this.Entity.Address = x,
				ReferenceController = this.GetAddressReferenceController (),
				PossibleItemsGetter = () => this.GetLegalPersonAddressGetter (),
			};

			this.addressTextField = builder.CreateAutoCompleteTextField ("Adresse de l'entreprise", controller);
		}

		private IEnumerable<AddressEntity> GetLegalPersonAddressGetter()
		{
			//	Retourne les adresses de l'entreprise choisie.
			return this.Entity.LegalPerson.Contacts
				.Where (x => x is MailContactEntity)	// on exclut les TelecomContactEntity et UriContactEntity
				.Cast<MailContactEntity> ()				// les AbstractContactEntity deviennent des MailContactEntity
				.Select (x => x.Address)				// on s'intéresse à l'entité Address de MailContact
				.ToList ();								// on veut une liste statique
		}

		private ReferenceController GetLegalPersonReferenceController()
		{
			return ReferenceController.Create (
				this.EntityGetter,
				entity => entity.LegalPerson,
				entity => CoreProgram.Application.Data.GetCustomers (entity.LegalPerson).FirstOrDefault (),
				creator: this.CreateNewLegalPerson);
		}

		private ReferenceController GetAddressReferenceController()
		{
			return ReferenceController.Create (
				this.EntityGetter,
				entity => entity.Address,
				entity => CoreProgram.Application.Data.GetCustomers (entity.LegalPerson).FirstOrDefault ());
		}

		private NewEntityReference CreateNewLegalPerson(DataContext context)
		{
			var customer = context.CreateEntityAndRegisterAsEmpty<RelationEntity> ();
			var person   = context.CreateEntityAndRegisterAsEmpty<LegalPersonEntity> ();

			customer.Person = person;
			customer.FirstContactDate = Date.Today;

			return new NewEntityReference (person, customer);
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,  0, "Rue",           Marshaler.Create (() => this.Entity.Address.Street.StreetName, x => this.Entity.Address.Street.StreetName = x));
			builder.CreateTextFieldMulti (tile, 52, "Complément 2",  Marshaler.Create (() => this.Entity.Address.Street.Complement, x => this.Entity.Address.Street.Complement = x));
			builder.CreateTextField      (tile,  0, "Boîte postale", Marshaler.Create (() => this.Entity.Address.PostBox.Number,    x => this.Entity.Address.PostBox.Number = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
		}


		private void CreateUICountry(UIBuilder builder)
		{
			this.InitializeDefaultCountry ();  // met "Suisse" si rien
			this.selectedCountry = this.Entity.Address.Location.Country;

			var controller = new SelectionController<CountryEntity> (this.BusinessContext)
			{
				ValueGetter = () => this.Country,
				ValueSetter = x => this.Country = x,
				ReferenceController = new ReferenceController (() => this.Country, creator: this.CreateNewCountry),
			};

			this.countryTextField = builder.CreateAutoCompleteTextField ("Nom et code du pays", controller);
		}

		private void CreateUILocation(UIBuilder builder)
		{
			var controller = new SelectionController<LocationEntity> (this.BusinessContext)
			{
				ValueGetter = () => this.Location,
				ValueSetter = x => this.Location = x,
				ReferenceController = new ReferenceController (() => this.Location, creator: this.CreateNewLocation),
				PossibleItemsGetter = () => this.LocationGetter,
			};

			this.locationTextField = builder.CreateAutoCompleteTextField ("Numéro postal et ville", controller);
		}

		private IEnumerable<LocationEntity> LocationGetter
		{
			//	Retourne les localités du pays choisi.
			get
			{
				if (this.selectedCountry == null)
				{
					return this.Data.GetAllEntities<LocationEntity> (dataContext: this.BusinessContext.DataContext);
				}
				else
				{
					return this.Data.GetAllEntities<LocationEntity> (dataContext: this.BusinessContext.DataContext).Where (x => x.Country.Code == this.selectedCountry.Code);
				}
			}
		}

		private NewEntityReference CreateNewCountry(DataContext context)
		{
			var country = context.CreateEntityAndRegisterAsEmpty<CountryEntity> ();

			return country;
		}

		private NewEntityReference CreateNewLocation(DataContext context)
		{
			var location = context.CreateEntityAndRegisterAsEmpty<LocationEntity> ();

			location.Country = this.selectedCountry;

			return location;
		}

		private void InitializeDefaultCountry()
		{
			if (string.IsNullOrEmpty (this.Entity.Address.Location.Country.Code))  // pays indéfini ?
			{
				foreach (var country in this.Data.GetAllEntities<CountryEntity> ().ToList ())
				{
					if (country.Code == "CH")
					{
						var localCountry = this.BusinessContext.GetLocalEntity (country);

						this.Entity.Address.Location.Country = localCountry;  // met la Suisse par défaut
						break;
					}
				}
			}
		}

		private CountryEntity Country
		{
			get
			{
				return this.selectedCountry;
			}
			set
			{
				if (this.selectedCountry.RefEquals (value) == false)
				{
					this.selectedCountry = value;

					// On efface la ville si on change de pays.
					this.Entity.Address.Location = EntityNullReferenceVirtualizer.CreateEmptyEntity<LocationEntity> ();
					this.locationTextField.Text = null;

					this.locationTextField.Items.Clear ();

					var locationGetter = this.LocationGetter;
					foreach (var item in locationGetter)
					{
						this.locationTextField.Items.Add (item);
					}
				}
			}
		}

		private LocationEntity Location
		{
			get
			{
				return this.Entity.Address.Location;
			}
			set
			{
				if (this.Entity.Address.Location.RefEquals (value) == false)
				{
					this.Entity.Address.Location = value;

					if (this.Entity.Address.Location.IsNotNull ())
					{
						this.countryTextField.SelectedItemIndex = this.countryTextField.Items.FindIndexByValue (this.Entity.Address.Location.Country);
					}
				}
			}
		}


		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}


		private bool IsMailUsedByLegalPerson
		{
			get
			{
				if (this.Entity.LegalPerson != null)
				{
					foreach (var contact in this.Entity.LegalPerson.Contacts)
					{
						if (contact is MailContactEntity)
						{
							var mail = contact as MailContactEntity;

							if (mail.Address == this.Entity.Address)
							{
								return true;
							}
						}
					}
				}

				return false;
			}
		}


		private TileTabBook<TabPageId>					tabBookContainer;
		private AutoCompleteTextField					addressTextField;
		private AutoCompleteTextField					countryTextField;
		private AutoCompleteTextField					locationTextField;
		private CountryEntity							selectedCountry;
	}
}
