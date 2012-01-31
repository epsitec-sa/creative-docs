//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptionsController : AbstractOptionsController
	{
		public ExtraitDeCompteOptionsController(ComptaEntity comptaEntity, ExtraitDeCompteOptions options)
			: base (comptaEntity, options)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateEditionUI (this.mainFrame, optionsChanged);
		}

		private void CreateEditionUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Compte",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			FrameBox container;
			AbstractTextField field;
			//?var marshaler = Marshaler.Create<FormattedText> (() => this.NuméroCompte, x => this.NuméroCompte = x);
			UIBuilder.CreateAutoCompleteTextField (frame, null, out container, out field);
			this.fieldCompte = field as AutoCompleteTextField;
			this.fieldCompte.FormattedText = this.NuméroCompte;
			container.PreferredWidth = 100;
			container.Dock = DockStyle.Left;
			container.Margins = new Margins (0, 1, 0, 0);
			container.TabIndex = ++this.tabIndex;

			{
				var comboFrame = new FrameBox
				{
					Parent          = frame,
					DrawFullFrame   = true,
					BackColor       = Color.FromBrightness (0.96),
					PreferredWidth  = 100,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (1, 0, 0, 0),
				};

				this.comboModeField = new StaticText
				{
					Parent           = comboFrame,
					Text             = this.ComboModeDescription,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
					PreferredHeight  = 20,
					Dock             = DockStyle.Fill,
				};

				this.comboModeButton = new GlyphButton
				{
					Parent          = frame,
					GlyphShape      = GlyphShape.Menu,
					PreferredWidth  = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (-1, 20, 0, 0),
				};
			}

			var graphicsButton = new CheckButton
			{
				Parent         = frame,
				Text           = "Graphique du solde",
				PreferredWidth = 120,
				ActiveState    = this.Options.HasGraphics ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (container,            "Choix du compte");
			ToolTip.Default.SetToolTip (this.comboModeField,  "Filtre pour le choix du compte");
			ToolTip.Default.SetToolTip (this.comboModeButton, "Filtre pour le choix du compte");

			//	Connexion des événements.
			this.fieldCompte.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.NuméroCompte = this.fieldCompte.FormattedText;
					optionsChanged ();
				}
			};

			this.comboModeField.Clicked += delegate
			{
				this.ShowComboModeMenu (this.comboModeField);
			};

			this.comboModeButton.Clicked += delegate
			{
				this.ShowComboModeMenu (this.comboModeField);
			};

			graphicsButton.ActiveStateChanged += delegate
			{
				this.Options.HasGraphics = (graphicsButton.ActiveState == ActiveState.Yes);
				optionsChanged ();
			};

			this.UpdateComptes ();
		}


		#region Combo mode menu
		private void ShowComboModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode pour le filtre.
			var menu = new VMenu ();

			this.AddComboModeToMenu (menu, CatégorieDeCompte.Inconnu);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Actif);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Passif);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Charge);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Produit);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Exploitation);

			menu.Items.Add (new MenuSeparator ());

			this.AddComboModeToMenu (menu, "Comptes vides",           () => this.Options.MontreComptesVides,           x => this.Options.MontreComptesVides           = x);
			this.AddComboModeToMenu (menu, "Comptes centralisateurs", () => this.Options.MontreComptesCentralisateurs, x => this.Options.MontreComptesCentralisateurs = x);

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddComboModeToMenu(VMenu menu, CatégorieDeCompte catégorie)
		{
			bool selected = (this.Options.CatégorieMontrée == catégorie);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (selected ? "Button.RadioYes" : "Button.RadioNo"),
				FormattedText = ExtraitDeCompteOptionsController.GetCatégorieDescription (catégorie),
				Name          = catégorie.ToString (),
			};

			item.Clicked += delegate
			{
				this.Options.CatégorieMontrée = (CatégorieDeCompte) System.Enum.Parse (typeof (CatégorieDeCompte), item.Name);

				this.comboModeField.Text = this.ComboModeDescription;
				this.UpdateComptes ();
			};

			menu.Items.Add (item);
		}

		private void AddComboModeToMenu(VMenu menu, FormattedText text, System.Func<bool> getter, System.Action<bool> setter)
		{
			bool selected = getter ();

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (selected ? "Button.CheckYes" : "Button.CheckNo"),
				FormattedText = text,
			};

			item.Clicked += delegate
			{
				setter (!getter ());

				this.comboModeField.Text = this.ComboModeDescription;
				this.UpdateComptes ();
			};

			menu.Items.Add (item);
		}
		#endregion


		private string ComboModeDescription
		{
			get
			{
				string text = " " + ExtraitDeCompteOptionsController.GetCatégorieDescription (this.Options.CatégorieMontrée);

				if (this.Options.MontreComptesVides ||
					this.Options.MontreComptesCentralisateurs)
				{
					text += " (+";

					if (this.Options.MontreComptesVides)
					{
						text += "v";
					}

					if (this.Options.MontreComptesCentralisateurs)
					{
						text += "c";
					}

					text += ")";
				}

				return text;
			}
		}

		private static string GetCatégorieDescription(CatégorieDeCompte catégorie)
		{
			switch (catégorie)
			{
				case CatégorieDeCompte.Actif:
					return "Actifs";

				case CatégorieDeCompte.Passif:
					return "Passifs";

				case CatégorieDeCompte.Charge:
					return "Charges";

				case CatégorieDeCompte.Produit:
					return "Produits";

				case CatégorieDeCompte.Exploitation:
					return "Exploitations";

				default:
					return "Tous";
			}
		}


		private void UpdateComptes()
		{
			var comptes = this.comptaEntity.PlanComptable.Where (x => this.CompteFilter (x));

			this.fieldCompte.Items.Clear ();

			foreach (var compte in comptes)
			{
				this.fieldCompte.Items.Add (compte);
			}

			this.ignoreChange = true;
			this.fieldCompte.FormattedText = this.NuméroCompte;
			this.ignoreChange = false;
		}

		private bool CompteFilter(ComptaCompteEntity compte)
		{
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return false;
			}

			if (this.Options.CatégorieMontrée != CatégorieDeCompte.Inconnu && compte.Catégorie != this.Options.CatégorieMontrée)
			{
				return false;
			}

			if (!this.Options.MontreComptesVides)
			{
				var solde = this.comptaEntity.GetSoldeCompte (compte);
				if (solde.GetValueOrDefault () == 0)
				{
					return false;
				}
			}

			if (!this.Options.MontreComptesCentralisateurs && compte.Type == TypeDeCompte.Groupe)
			{
				return false;
			}

			return true;
		}

		private FormattedText NuméroCompte
		{
			get
			{
				return this.Options.NuméroCompte;
			}
			set
			{
				this.Options.NuméroCompte = PlanComptableDataAccessor.GetCompteNuméro (value);
			}
		}

		private ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}


		private AutoCompleteTextField			fieldCompte;
		private StaticText						comboModeField;
		private GlyphButton						comboModeButton;
		private bool							ignoreChange;
	}
}
