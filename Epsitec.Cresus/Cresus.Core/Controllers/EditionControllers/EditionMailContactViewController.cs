//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Library;
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
			this.SetupDefaultCountry ();

			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIRoles (data);
				this.CreateUICommon (data);

				if (this.Entity.NaturalPerson.IsNull () &&
					this.Entity.LegalPerson.IsNotNull ())
				{
					this.CreateUICountry (data);
					this.CreateUIMain (data);
					this.CreateUILocation (data);
				}
				else
				{
					this.CreateTabBook (data);
					this.CreateTabBookLocalPage (data);
					this.CreateTabBookGlobalPage (data);
				}

				this.CreateUIComments (data);
			}
		}

		private void SetupDefaultCountry()
		{
			this.defaultCountry = this.BusinessContext.GetAllEntities<CountryEntity> ().FirstOrDefault (x => x.CountryCode == "CH");
		}

		private void CreateUIRoles(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "MailContactRoles",
				IconUri	        = "Data.MailContact",
				Title	        = TextFormatter.FormatText ("Adresse"),
				CompactTitle    = TextFormatter.FormatText ("Adresse"),
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					var controller = new SelectionController<ContactGroupEntity> (this.BusinessContext)
					{
						CollectionValueGetter    = () => this.Entity.ContactGroups,
						ToFormattedTextConverter = x => TextFormatter.FormatText (x.Name)
					};

					builder.CreateEditionDetailedItemPicker (tile, "ContactRoles", this.Entity, "Rôles souhaités", controller, EnumValueCardinality.Any, ViewControllerMode.Summary, 3);
				}
			};

			data.Add (tileData);
		}

		private void CreateUICommon(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "MailContactCommon",
				Frameless       = true,
				CreateEditionUI = (tile, builder) =>
				{
					builder.CreateTextFieldMulti (tile, 52, "Complément 1", Marshaler.Create (() => this.Entity.Complement, x => this.Entity.Complement = x));
				}
			};

			data.Add (tileData);
		}

		private enum TabPageId
		{
			Local,
			Global,
		}

		private void CreateTabBook(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "MailContactBook",
				Frameless       = true,
				CreateEditionUI = this.CreateTabBook,
			};

			data.Add (tileData);
		}

		private void CreateTabBook(EditionTile tile, UIBuilder builder)
		{
			var localPage  = TabPageDef.Create (TabPageId.Local,  "Adresse spécifique",   this.HandleSelectTabPageLocal);
			var globalPage = TabPageDef.Create (TabPageId.Global, "Entreprise existante", this.HandleSelectTabPageGlobal);

			var selectedPage = this.Entity.LegalPerson.IsNull () ? localPage : globalPage;

			this.tabBookContainer = builder.CreateTabBook (tile, selectedPage, localPage, globalPage);
		}

		private void CreateTabBookLocalPage(TileDataItems data)
		{
			bool visible = this.Entity.LegalPerson.IsNull ();

			this.CreateUICountry (data, visible);
			this.CreateUIMain (data, visible);
			this.CreateUILocation (data, visible);
		}

		private void CreateTabBookGlobalPage(TileDataItems data)
		{
			bool visible = this.Entity.LegalPerson.IsNotNull ();

			this.CreateUILegalPerson (data, visible);
			this.CreateUIAddress (data, visible);
		}
		
		private void HandleSelectTabPageLocal()
		{
			this.ShowTabPage (TabPageId.Local);

			if (this.Entity.LegalPerson.IsNotNull ())
			{
				this.Entity.LegalPerson = this.DataContext.CreateNullEntity<LegalPersonEntity> ();
				this.Entity.Address     = this.BusinessContext.CreateEntityAndRegisterAsEmpty<AddressEntity> ();
				this.selectedCountry    = this.defaultCountry;

				this.TileContainer.UpdateAllWidgets ();
			}
		}

		private void HandleSelectTabPageGlobal()
		{
			this.ShowTabPage (TabPageId.Global);
		}

		private void ShowTabPage(TabPageId page)
		{
			this.TileContainer.SetTileVisibility ("MailContactCountry",  page == TabPageId.Local);
			this.TileContainer.SetTileVisibility ("MailContactMain",     page == TabPageId.Local);
			this.TileContainer.SetTileVisibility ("MailContactLocation", page == TabPageId.Local);

			this.TileContainer.SetTileVisibility ("MailContactPerson",   page == TabPageId.Global);
			this.TileContainer.SetTileVisibility ("MailContactAddress",  page == TabPageId.Global);
		}


		private void CreateUILegalPerson(TileDataItems data, bool visibility = true)
		{
			var tileData = new TileDataItem
			{
				Name              = "MailContactPerson",
				Frameless         = true,
				InitialVisibility = visibility,
				CreateEditionUI   = (tile, builder) =>
				{
					var controller = new SelectionController<LegalPersonEntity> (this.BusinessContext)
					{
						ValueGetter         = () => this.Entity.LegalPerson,
						ValueSetter         = x => this.SetLegalPerson (x),
						ReferenceController = this.GetLegalPersonReferenceController (),
						PossibleItemsGetter = () => this.Data.GetAllEntities<LegalPersonEntity> (dataContext: this.DataContext),
					};

					builder.CreateAutoCompleteTextField (tile, "Entreprise (personne morale)", controller);
				}
			};

			data.Add (tileData);
		}

		private void SetLegalPerson(LegalPersonEntity person)
		{
			this.Entity.LegalPerson = person;
			this.UpdateLegalPersonAddresses ();
		}

		private void UpdateLegalPersonAddresses()
		{
			System.Diagnostics.Debug.Assert (this.addressTextField != null);
			
			this.addressTextField.Text = null;  // on efface l'adresse si on change d'entreprise
			this.addressTextField.Items.Clear ();

			var addressGetter = this.GetLegalPersonAddressGetter ();

			foreach (var item in addressGetter)
			{
				this.addressTextField.Items.Add (item);
			}
		}

		private void CreateUIAddress(TileDataItems data, bool visibility = true)
		{
			var tileData = new TileDataItem
			{
				Name              = "MailContactAddress",
				Frameless         = true,
				InitialVisibility = visibility,
				CreateEditionUI   = (tile, builder) =>
				{
					var controller = new SelectionController<AddressEntity> (this.BusinessContext)
					{
						ValueGetter         = () => this.Entity.Address,
						ValueSetter         = x => this.Entity.Address = x,
						ReferenceController = this.GetAddressReferenceController (),
						PossibleItemsGetter = () => this.GetLegalPersonAddressGetter (),
					};

					this.addressTextField = builder.CreateAutoCompleteTextField (tile, "Adresse de l'entreprise", controller);
				}
			};

			data.Add (tileData);
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
				entity => this.GetCustomers (entity.LegalPerson).FirstOrDefault (),
				creator: this.CreateNewLegalPerson);
		}

		private ReferenceController GetAddressReferenceController()
		{
			return ReferenceController.Create (
				this.EntityGetter,
				entity => entity.Address,
				entity => this.GetCustomers (entity.LegalPerson).FirstOrDefault ());
		}

		private IEnumerable<RelationEntity> GetCustomers(AbstractPersonEntity person)
		{
			var repository = new RelationEntity.Repository (this.Data, this.DataContext);
			var example = repository.CreateExample ();
			example.Person = person;
			return repository.GetByExample (example);
		}

		private NewEntityReference CreateNewLegalPerson(DataContext context)
		{
			var customer = context.CreateEntityAndRegisterAsEmpty<RelationEntity> ();
			var person   = context.CreateEntityAndRegisterAsEmpty<LegalPersonEntity> ();

			customer.Person = person;
			customer.FirstContactDate = Date.Today;

			return new NewEntityReference (person, customer);
		}

		private void CreateUIMain(TileDataItems data, bool visibility = true)
		{
			var tileData = new TileDataItem
			{
				Name              = "MailContactMain",
				Frameless         = true,
				InitialVisibility = visibility,
				CreateEditionUI   = (tile, builder) =>
				{
					builder.CreateTextField (tile, 0, "Rue", Marshaler.Create (() => this.Entity.Address.Street.StreetName, x => this.Entity.Address.Street.StreetName = x));
					builder.CreateTextFieldMulti (tile, 52, "Complément 2", Marshaler.Create (() => this.Entity.Address.Street.Complement, x => this.Entity.Address.Street.Complement = x));
					builder.CreateTextField (tile, 0, "Boîte postale", Marshaler.Create (() => this.Entity.Address.PostBox.Number, x => this.Entity.Address.PostBox.Number = x));
				}
			};

			data.Add (tileData);
		}


		private void CreateUICountry(TileDataItems data, bool visibility = true)
		{
			var tileData = new TileDataItem
			{
				Name              = "MailContactCountry",
				Frameless         = true,
				InitialVisibility = visibility,
				CreateEditionUI   = (tile, builder) =>
				{
					if (this.Entity.Address.Location.Country.IsNull ())
					{
						this.selectedCountry = this.defaultCountry;
					}
					else
					{
						this.selectedCountry = this.Entity.Address.Location.Country;
					}

					var controller = new SelectionController<CountryEntity> (this.BusinessContext)
					{
						ValueGetter = () => this.Country,
						ValueSetter = x => this.Country = x,
						ReferenceController = new ReferenceController (() => this.Country, creator: this.CreateNewCountry),
					};

					this.countryTextField = builder.CreateAutoCompleteTextField (tile, "Nom et code du pays", controller);
				}
			};

			data.Add (tileData);
		}

		private void CreateUILocation(TileDataItems data, bool visibility = true)
		{
			var tileData = new TileDataItem
			{
				Name              = "MailContactLocation",
				Frameless         = true,
				InitialVisibility = visibility,
				CreateEditionUI   = (tile, builder) =>
				{
					var controller = new SelectionController<LocationEntity> (this.BusinessContext)
					{
						ValueGetter = () => this.Location,
						ValueSetter = x => this.Location = x,
						ReferenceController = new ReferenceController (() => this.Location, creator: this.CreateNewLocation),
						PossibleItemsGetter = () => this.LocationGetter,
					};

					this.locationTextField = builder.CreateAutoCompleteTextField (tile, "Numéro postal et ville", controller);
				}
			};

			data.Add (tileData);
		}

		private IEnumerable<LocationEntity> LocationGetter
		{
			//	Retourne les localités du pays choisi.
			get
			{
				if (this.selectedCountry.IsNull ())
				{
					return this.Data.GetAllEntities<LocationEntity> (dataContext: this.DataContext);
				}
				else
				{
					return this.Data.GetAllEntities<LocationEntity> (dataContext: this.DataContext).Where (x => x.Country.CountryCode == this.selectedCountry.CountryCode);
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
#if false
			if (string.IsNullOrEmpty (this.Entity.Address.Location.Country.CountryCode))  // pays indéfini ?
			{
				foreach (var country in this.Data.GetAllEntities<CountryEntity> (dataContext: this.DataContext))
				{
					if (country.CountryCode == "CH")
					{
						var localCountry = this.BusinessContext.GetLocalEntity (country);

						this.Entity.Address.Location.Country = localCountry;  // met la Suisse par défaut
						break;
					}
				}
			}
#endif
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
					this.Entity.Address.Location = this.DataContext.CreateNullEntity<LocationEntity> ();
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


		private void CreateUIComments(TileDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}


		private TileTabBook<TabPageId>					tabBookContainer;
		private AutoCompleteTextField					addressTextField;
		private AutoCompleteTextField					countryTextField;
		private AutoCompleteTextField					locationTextField;
		private CountryEntity							selectedCountry;
		private CountryEntity							defaultCountry;
	}
}
