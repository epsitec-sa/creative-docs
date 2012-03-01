//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Permanents.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Permanents.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les paramètress permanent d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeComptePermanentsController : AbstractPermanentsController
	{
		public ExtraitDeComptePermanentsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void UpdateContent()
		{
			this.UpdateWidgets ();
		}


		public override void CreateUI(FrameBox parent, System.Action permanentsChanged)
		{
			base.CreateUI (parent, permanentsChanged);

			this.CreateEditionUI (this.toolbar);
		}

		private void CreateEditionUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				TabIndex = ++this.tabIndex,
				Padding  = new Margins (5, 5, 0, 0),
			};

			new StaticText
			{
				Parent           = frame,
				FormattedText    = "Compte",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = UIBuilder.LeftLabelWidth-10,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			AbstractTextField field;
			UIBuilder.CreateAutoCompleteTextField (frame, out this.compteFrame, out field);
			this.compteField = field as AutoCompleteTextField;
			this.compteFrame.PreferredWidth = 150;
			this.compteFrame.Dock = DockStyle.Left;
			this.compteFrame.Margins = new Margins (0, 1, 0, 0);
			this.compteFrame.TabIndex = ++this.tabIndex;

			this.comboModeFrame = UIBuilder.CreatePseudoCombo (frame, out this.comboModeField, out this.comboModeButton);
			this.comboModeFrame.PreferredWidth = 120;

			this.UpdateWidgets ();

			ToolTip.Default.SetToolTip (this.compteFrame,    "Choix du compte");
			ToolTip.Default.SetToolTip (this.comboModeFrame, "Filtre pour le choix du compte");

			//	Connexion des événements.
			this.compteField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.NuméroCompte = this.compteField.FormattedText;
					this.PermanentsChanged ();
				}
			};

			this.comboModeField.Clicked += delegate
			{
				this.ShowComboModeMenu (this.comboModeFrame);
			};

			this.comboModeButton.Clicked += delegate
			{
				this.ShowComboModeMenu (this.comboModeFrame);
			};
		}

		protected override void PermanentsChanged()
		{
			this.UpdateWidgets ();
			base.PermanentsChanged ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateComptes ();
			this.comboModeField.Text = this.ComboModeDescription;

			base.UpdateWidgets ();
		}


		#region Combo mode menu
		private void ShowComboModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode pour le filtre.
			var menu = new VMenu ();

			this.AddComboModeToMenu (menu, CatégorieDeCompte.Tous);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Actif);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Passif);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Charge);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Produit);
			this.AddComboModeToMenu (menu, CatégorieDeCompte.Exploitation);

			menu.Items.Add (new MenuSeparator ());

			this.AddComboModeToMenu (menu, "Comptes vides",           () => this.Permanents.MontreComptesVides,           x => this.Permanents.MontreComptesVides           = x);
			this.AddComboModeToMenu (menu, "Comptes centralisateurs", () => this.Permanents.MontreComptesCentralisateurs, x => this.Permanents.MontreComptesCentralisateurs = x);

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddComboModeToMenu(VMenu menu, CatégorieDeCompte catégorie)
		{
			bool selected = (this.Permanents.CatégorieMontrée == catégorie);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetRadioStateIconUri (selected),
				FormattedText = ExtraitDeComptePermanentsController.GetCatégorieDescription (catégorie),
				Name          = catégorie.ToString (),
			};

			item.Clicked += delegate
			{
				this.Permanents.CatégorieMontrée = (CatégorieDeCompte) System.Enum.Parse (typeof (CatégorieDeCompte), item.Name);

				this.UpdateWidgets ();
				this.PermanentsChanged ();
			};

			menu.Items.Add (item);
		}

		private void AddComboModeToMenu(VMenu menu, FormattedText text, System.Func<bool> getter, System.Action<bool> setter)
		{
			bool selected = getter ();

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (selected),
				FormattedText = text,
			};

			item.Clicked += delegate
			{
				setter (!getter ());

				this.UpdateWidgets ();
				this.PermanentsChanged ();
			};

			menu.Items.Add (item);
		}
		#endregion


		private string ComboModeDescription
		{
			get
			{
				string text = " " + ExtraitDeComptePermanentsController.GetCatégorieDescription (this.Permanents.CatégorieMontrée);

				if (this.Permanents.MontreComptesVides ||
					this.Permanents.MontreComptesCentralisateurs)
				{
					text += " (+";

					if (this.Permanents.MontreComptesVides)
					{
						text += "v";
					}

					if (this.Permanents.MontreComptesCentralisateurs)
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
			var comptes = this.compta.PlanComptable.Where (x => this.CompteFilter (x));
			UIBuilder.UpdateAutoCompleteTextField (this.compteField, comptes);

			using (this.ignoreChanges.Enter ())
			{
				this.compteField.FormattedText = this.NuméroCompte;
				this.compteFrame.Enable = comptes.Any ();
			}
		}

		private bool CompteFilter(ComptaCompteEntity compte)
		{
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return false;
			}

			if (this.Permanents.CatégorieMontrée != CatégorieDeCompte.Tous && compte.Catégorie != this.Permanents.CatégorieMontrée)
			{
				return false;
			}

			if (!this.Permanents.MontreComptesVides)
			{
				var solde = this.controller.DataAccessor.SoldesJournalManager.GetSolde (compte);
				if (solde.GetValueOrDefault () == 0)
				{
					return false;
				}
			}

			if (!this.Permanents.MontreComptesCentralisateurs && compte.Type == TypeDeCompte.Groupe)
			{
				return false;
			}

			return true;
		}

		private FormattedText NuméroCompte
		{
			get
			{
				return this.Permanents.NuméroCompte;
			}
			set
			{
				this.Permanents.NuméroCompte = PlanComptableDataAccessor.GetCompteNuméro (value);
			}
		}

		private ExtraitDeComptePermanents Permanents
		{
			get
			{
				return this.permanents as ExtraitDeComptePermanents;
			}
		}


		private FrameBox						compteFrame;
		private AutoCompleteTextField			compteField;

		private FrameBox						comboModeFrame;
		private StaticText						comboModeField;
		private GlyphButton						comboModeButton;
	}
}
