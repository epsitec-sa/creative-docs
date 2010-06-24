//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionMailContactViewController : EditionViewController<Entities.MailContactEntity>
	{
		public EditionMailContactViewController(string name, Entities.MailContactEntity entity)
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
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Mail", "Adresse");

				this.CreateUIRoles  (builder);
				this.CreateUICommon (builder);

				if (EntityNullReferenceVirtualizer.IsNullEntity (this.Entity.NaturalPerson) &&
					!EntityNullReferenceVirtualizer.IsNullEntity (this.Entity.LegalPerson))
				//?if (this.IsMailUsedByLegalPerson)
				{
					this.CreateUICountry  (builder);
					this.CreateUIMain     (builder);
					this.CreateUILocation (builder);
				}
				else
				{
					this.CreateTabBook (builder);

					//	Crée le contenu de la page "local".
					this.localPageContent = new List<Common.Widgets.Widget> ();
					builder.ContentList = this.localPageContent;

					this.CreateUICountry  (builder);
					this.CreateUIMain     (builder);
					this.CreateUILocation (builder);

					//	Crée le contenu de la page "global".
					this.globalPageContent = new List<Common.Widgets.Widget> ();
					builder.ContentList = this.globalPageContent;

					this.CreateUILegalPerson (builder);
					this.CreateUIAddress     (builder);

					builder.ContentList = null;

					if (EntityNullReferenceVirtualizer.IsNullEntity (this.Entity.LegalPerson))
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
		}


		private void CreateUIRoles(UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactRoleEntity>
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
			builder.CreateMargin         (tile, false);
		}


		private void CreateTabBook(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			List<string> pagesDescription = new List<string>();
			pagesDescription.Add ("local.Adresse spécifique");
			pagesDescription.Add ("global.Adresse existante");
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
				// TODO:
			}
		}


		private void CreateUILegalPerson(UIBuilder builder)
		{
			var textField = builder.CreateAutoCompleteTextField ("Entreprise (personne morale)",
				new SelectionController<Entities.LegalPersonEntity>
				{
					ValueGetter = () => this.Entity.LegalPerson,
					ValueSetter = x => this.Entity.LegalPerson = x,
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

					var addressGetter = this.LegalPersonAddressGetter;
					foreach (var item in addressGetter)
					{
						this.addressTextField.Items.Add (item);
					}
				};
		}

		private void CreateUIAddress(UIBuilder builder)
		{
			this.addressTextField = builder.CreateAutoCompleteTextField ("Adresse de l'entreprise",
				new SelectionController<Entities.AddressEntity>
				{
					ValueGetter = () => this.Entity.Address,
					ValueSetter = x => this.Entity.Address = x,
					PossibleItemsGetter = () => this.LegalPersonAddressGetter,

					ToTextArrayConverter     = x => new string[] { x.Street.StreetName, x.Location.PostalCode, x.Location.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Street.StreetName, ", ", x.Location.PostalCode, x.Location.Name),
				});
		}

		private IEnumerable<Entities.AddressEntity> LegalPersonAddressGetter
		{
			//	Retourne les adresses de l'entreprise choisie.
			get
			{
				return this.Entity.LegalPerson.Contacts
					.Where (x => x is Entities.MailContactEntity)	// on exclut les TelecomContactEntity et UriContactEntity
					.Cast<Entities.MailContactEntity> ()			// les AbstractContactEntity deviennent des MailContactEntity
					.Select (x => x.Address)						// on s'intéresse à l'entité Address de MailContact
					.ToList ();										// on veut une liste statique
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin         (tile, true);
			builder.CreateTextField      (tile,  0, "Rue",           Marshaler.Create (() => this.Entity.Address.Street.StreetName, x => this.Entity.Address.Street.StreetName = x));
			builder.CreateTextFieldMulti (tile, 52, "Complément 2",  Marshaler.Create (() => this.Entity.Address.Street.Complement, x => this.Entity.Address.Street.Complement = x));
			builder.CreateTextField      (tile,  0, "Boîte postale", Marshaler.Create (() => this.Entity.Address.PostBox.Number,    x => this.Entity.Address.PostBox.Number = x));
			builder.CreateMargin         (tile, true);
		}


		private void CreateUICountry(UIBuilder builder)
		{
			this.selectedCountry = this.Entity.Address.Location.Country;

			this.countryTextField = builder.CreateAutoCompleteTextField ("Nom et code du pays",
				new SelectionController<Entities.CountryEntity>
				{
					ValueGetter = () => this.Country,
					ValueSetter = x => this.Country = x ?? EntityNullReferenceVirtualizer.CreateEmptyEntity<Entities.CountryEntity> (),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetCountries (),

					ToTextArrayConverter     = x => new string[] { x.Code, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")"),
				});

#if false
			this.countryTextField.SelectedItemChanged +=
				delegate
				{
					System.Diagnostics.Debug.Assert (this.addressTextField != null);
					this.locationTextField.Text = null;  // on efface la ville si on change de pays
					this.locationTextField.Items.Clear ();

					var locationGetter = this.LocationGetter;
					foreach (var item in locationGetter)
					{
						this.locationTextField.Items.Add (item);
					}
				};
#endif
		}

		private void CreateUILocation(UIBuilder builder)
		{
			this.locationTextField = builder.CreateAutoCompleteTextField ("Numéro postal et ville",
				new SelectionController<Entities.LocationEntity>
				{
					ValueGetter = () => this.Location,
					ValueSetter = x => this.Location = x ?? EntityNullReferenceVirtualizer.CreateEmptyEntity<Entities.LocationEntity> (),
					PossibleItemsGetter = () => this.LocationGetter,

					ToTextArrayConverter     = x => new string[] { x.Country.Code, x.PostalCode, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Country.Code, "-", x.PostalCode, x.Name),
				});
		}

		private IEnumerable<Entities.LocationEntity> LocationGetter
		{
			//	Retourne les localités du pays choisi.
			get
			{
				return CoreProgram.Application.Data.GetLocations (this.selectedCountry);
			}
		}

		private Entities.CountryEntity Country
		{
			get
			{
				return this.selectedCountry;
			}
			set
			{
				//?if (this.selectedCountry != value)
				if (Misc.CompareEntities (this.selectedCountry, value) == false)
				{
					this.selectedCountry = value;

					// On efface la ville si on change de pays.
					this.Entity.Address.Location = EntityNullReferenceVirtualizer.CreateEmptyEntity<Entities.LocationEntity> ();
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

		private Entities.LocationEntity Location
		{
			get
			{
				return this.Entity.Address.Location;
			}
			set
			{
				//?if (this.Entity.Address.Location != value)
				if (Misc.CompareEntities (this.Entity.Address.Location, value) == false)
				{
					this.Entity.Address.Location = value;

					this.countryTextField.SelectedItemIndex = this.countryTextField.Items.FindIndexByValue (this.Entity.Address.Location.Country);
				}
			}
		}


		private bool IsMailUsedByLegalPerson
		{
			get
			{
				if (this.Entity.LegalPerson != null)
				{
					foreach (var contact in this.Entity.LegalPerson.Contacts)
					{
						if (contact is Entities.MailContactEntity)
						{
							var mail = contact as Entities.MailContactEntity;

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


		private Common.Widgets.FrameBox			tabBookContainer;
		private List<Common.Widgets.Widget>		localPageContent;
		private List<Common.Widgets.Widget>		globalPageContent;
		private AutoCompleteTextField			addressTextField;
		private AutoCompleteTextField			countryTextField;
		private AutoCompleteTextField			locationTextField;
		private Entities.CountryEntity			selectedCountry;
	}
}
