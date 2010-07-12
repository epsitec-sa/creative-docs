//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Adresse");

				this.CreateUIRoles  (builder);
				this.CreateUICommon (builder);

				if (this.Entity.NaturalPerson.IsNull () &&
					this.Entity.LegalPerson.IsActive ())
				{
					this.CreateUICountry  (builder);
					this.CreateUIMain     (builder);
					this.CreateUILocation (builder);
				}
				else
				{
					this.CreateTabBook (builder);

					//	Crée le contenu de la page "local".
					this.localPageContent = new List<Epsitec.Common.Widgets.Widget> ();
					builder.ContentList = this.localPageContent;

					this.CreateUICountry  (builder);
					this.CreateUIMain     (builder);
					this.CreateUILocation (builder);

					//	Crée le contenu de la page "global".
					this.globalPageContent = new List<Epsitec.Common.Widgets.Widget> ();
					builder.ContentList = this.globalPageContent;

					this.CreateUILegalPerson (builder);
					this.CreateUIAddress     (builder);

					builder.ContentList = null;

					if (this.Entity.LegalPerson.IsNull ())
					{
						this.SelectTabPage ("local");  // montre l'onglet "local"
					}
					else
					{
						this.SelectTabPage ("global");  // montre l'onglet "global"
					}
				}

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIComments (data);

			containerController.GenerateTiles ();
		}

		protected override EditionStatus GetEditionStatus()
		{
			if ((string.IsNullOrWhiteSpace (this.Entity.Complement)) &&
				(this.Entity.NaturalPerson.UnwrapNullEntity () == null || this.Entity.LegalPerson.UnwrapNullEntity () == null) &&
				(this.Entity.Roles.Count == 0) &&
				(this.Entity.Comments.Count == 0) &&
				(this.Entity.Address.IsEmpty ()))
			{
				return EditionStatus.Empty;
			}
			else
			{
				return EditionStatus.Valid;
			}
		}

		protected override void UpdateEmptyEntityStatus(DataContext context, bool isEmpty)
		{
			var contact = this.Entity;

			context.UpdateEmptyEntityStatus (contact, isEmpty);

			context.UpdateEmptyEntityStatus (contact.Address,
				context.UpdateEmptyEntityStatus (contact.Address.Street,  x => x.IsEmpty ()),
				context.UpdateEmptyEntityStatus (contact.Address.PostBox, x => x.IsEmpty ()),
				contact.Address.Location.IsEmpty ());
		}

		private void CreateUIRoles(UIBuilder builder)
		{
			var controller = new SelectionController<ContactRoleEntity>
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetRoles (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			builder.CreateEditionDetailedCheck (0, "Choix du ou des rôles souhaités", controller);
		}

		private void CreateUICommon(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextFieldMulti (tile, 52, "Complément 1", Marshaler.Create (() => this.Entity.Complement, x => this.Entity.Complement = x));
			builder.CreateMargin         (tile, horizontalSeparator: false);
		}


		private void CreateTabBook(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			List<string> pagesDescription = new List<string>();
			pagesDescription.Add ("local.Adresse spécifique");
			pagesDescription.Add ("global.Entreprise existante");
			this.tabBookContainer = builder.CreateTabBook (tile, pagesDescription, "local", this.HandleTabBookAction);
		}

		private void SelectTabPage(string tabPageName)
		{
			foreach (TilePage page in this.tabBookContainer.Children)
			{
				if (page != null)
				{
					page.SetSelected (page.Name == tabPageName);
				}
			}

			this.HandleTabBookAction (tabPageName);
		}

		private void HandleTabBookAction(string tabPageName)
		{
			foreach (var widget in this.localPageContent)
			{
				widget.Visibility = tabPageName == "local";
			}

			foreach (var widget in this.globalPageContent)
			{
				widget.Visibility = tabPageName == "global";
			}

			if (tabPageName == "local")
			{
				if (this.Entity.LegalPerson.IsActive ())
				{
					this.Entity.LegalPerson = EntityNullReferenceVirtualizer.CreateEmptyEntity<LegalPersonEntity> ();
					this.Entity.Address = this.DataContext.CreateRegisteredEmptyEntity<AddressEntity> ();
					this.InitializeDefaultCountry ();  // met "Suisse" si rien
					this.selectedCountry = this.Entity.Address.Location.Country;

					//	Grace aux WidgetUpdaters, on peut remettre à jour tous les widgets éditables en
					//	fonction du nouveau contenu de l'entité.
					foreach (var updater in this.tileContainer.WidgetUpdaters)
					{
						updater.Update ();
					}
				}
			}
		}


		private void CreateUILegalPerson(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Entreprise (personne morale)",
				new SelectionController<LegalPersonEntity>
				{
					ValueGetter         = () => this.Entity.LegalPerson,
					ValueSetter         = x => this.Entity.LegalPerson = x.WrapNullEntity (),
					ReferenceController = this.GetLegalPersonReferenceController (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLegalPersons (),

					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name),
				});

			textField.SelectedItemChanged +=
				delegate
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
			this.addressTextField = builder.CreateAutoCompleteTextField ("Adresse de l'entreprise",
				new SelectionController<AddressEntity>
				{
					ValueGetter         = () => this.Entity.Address,
					ValueSetter         = x => this.Entity.Address = x.WrapNullEntity (),
					ReferenceController = this.GetAddressReferenceController (),
					PossibleItemsGetter = () => this.GetLegalPersonAddressGetter (),

					ToTextArrayConverter     = x => new string[] { x.Street.StreetName, x.Location.PostalCode, x.Location.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Street.StreetName, ", ", x.Location.PostalCode, x.Location.Name),
				});
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
			var customer = context.CreateRegisteredEmptyEntity<RelationEntity> ();
			var person   = context.CreateRegisteredEmptyEntity<LegalPersonEntity> ();

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

			this.countryTextField = builder.CreateAutoCompleteTextField ("Nom et code du pays",
				new SelectionController<CountryEntity>
				{
					ValueGetter = () => this.Country,
					ValueSetter = x => this.Country = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Country, creator: this.CreateNewCountry),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetCountries (),

					ToTextArrayConverter     = x => new string[] { x.Code, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")"),
				});
		}

		private void CreateUILocation(UIBuilder builder)
		{
			this.locationTextField = builder.CreateAutoCompleteTextField ("Numéro postal et ville",
				new SelectionController<LocationEntity>
				{
					ValueGetter = () => this.Location,
					ValueSetter = x => this.Location = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Location, creator: this.CreateNewLocation),
					PossibleItemsGetter = () => this.LocationGetter,

					ToTextArrayConverter     = x => new string[] { x.Country.Code, x.PostalCode, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Country.Code, "-", x.PostalCode, x.Name),
				});
		}

		private IEnumerable<LocationEntity> LocationGetter
		{
			//	Retourne les localités du pays choisi.
			get
			{
				return CoreProgram.Application.Data.GetLocations (this.selectedCountry);
			}
		}

		private NewEntityReference CreateNewCountry(DataContext context)
		{
			var country = context.CreateRegisteredEmptyEntity<CountryEntity> ();

			return country;
		}

		private NewEntityReference CreateNewLocation(DataContext context)
		{
			var location = context.CreateRegisteredEmptyEntity<LocationEntity> ();

			location.Country = this.selectedCountry;

			return location;
		}

		private void InitializeDefaultCountry()
		{
			if (string.IsNullOrEmpty (this.Entity.Address.Location.Country.Code))  // pays indéfini ?
			{
				foreach (var country in CoreProgram.Application.Data.GetCountries ().ToList ())
				{
					if (country.Code == "CH")
					{
						this.Entity.Address.Location.Country = country;  // met la Suisse par défaut
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
				if (this.selectedCountry.CompareWith (value) == false)
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
				if (this.Entity.Address.Location.CompareWith (value) == false)
				{
					this.Entity.Address.Location = value;

					if (this.Entity.Address.Location.IsActive ())
					{
						this.countryTextField.SelectedItemIndex = this.countryTextField.Items.FindIndexByValue (this.Entity.Address.Location.Country);
					}
				}
			}
		}


		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (data, this.EntityGetter, x => x.Comments);
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


		private TileContainer							tileContainer;
		private Epsitec.Common.Widgets.FrameBox			tabBookContainer;
		private List<Epsitec.Common.Widgets.Widget>		localPageContent;
		private List<Epsitec.Common.Widgets.Widget>		globalPageContent;
		private AutoCompleteTextField					addressTextField;
		private AutoCompleteTextField					countryTextField;
		private AutoCompleteTextField					locationTextField;
		private CountryEntity							selectedCountry;
	}
}
