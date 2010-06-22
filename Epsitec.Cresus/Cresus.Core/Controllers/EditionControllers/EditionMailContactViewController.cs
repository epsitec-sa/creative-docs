﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			List<string> texts = new List<string>();
			texts.Add ("local.Adresse spécifique");
			texts.Add ("global.Adresse existante");
			this.tabBookContainer = builder.CreateTabBook (tile, texts, "local", this.HandleTabBookAction);
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
			builder.CreateAutoCompleteTextField ("Entreprise (personne morale)",
				new SelectionController<Entities.LegalPersonEntity>
				{
					ValueGetter = () => this.Entity.LegalPerson,
					ValueSetter = x => this.Entity.LegalPerson = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLegalPersons (),

					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name),
				});
		}

		private void CreateUIAddress(UIBuilder builder)
		{
			IEnumerable<Entities.AddressEntity> xx = this.Entity.LegalPerson.Contacts.Where (x => x is Entities.MailContactEntity).Cast<Entities.MailContactEntity> ().Select (x => x.Address);

			builder.CreateAutoCompleteTextField ("Adresse de l'entreprise",
				new SelectionController<Entities.AddressEntity>
				{
					ValueGetter = () => this.Entity.Address,
					ValueSetter = x => this.Entity.Address = x,
					PossibleItemsGetter = () => xx,

					ToTextArrayConverter     = x => new string[] { x.Street.StreetName, x.Location.PostalCode, x.Location.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Street.StreetName, ", ", x.Location.PostalCode, x.Location.Name),
				});
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
			builder.CreateAutoCompleteTextField ("Nom et code du pays",
				new SelectionController<Entities.CountryEntity>
				{
					ValueGetter = () => this.Entity.Address.Location.Country,
					ValueSetter = x => this.Entity.Address.Location.Country = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetCountries (),

					ToTextArrayConverter     = x => new string[] { x.Code, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name, "(", x.Code, ")"),
				});
		}

		private void CreateUILocation(UIBuilder builder)
		{
			builder.CreateAutoCompleteTextField ("Numéro postal et ville",
				new SelectionController<Entities.LocationEntity>
				{
					ValueGetter = () => this.Entity.Address.Location,
					ValueSetter = x => this.Entity.Address.Location = x,
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetLocations (),

					ToTextArrayConverter     = x => new string[] { x.PostalCode, x.Name },
					ToFormattedTextConverter = x => UIBuilder.FormatText (x.PostalCode, x.Name),
				});
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
	}
}
