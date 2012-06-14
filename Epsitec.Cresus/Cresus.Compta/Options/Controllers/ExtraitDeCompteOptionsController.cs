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
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptionsController : AbstractOptionsController
	{
		public ExtraitDeCompteOptionsController(AbstractController controller)
			: base (controller)
		{
		}


		public override void UpdateContent()
		{
			base.UpdateContent ();

			if (this.showPanel)
			{
				this.UpdateWidgets ();
			}
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			base.CreateUI (parent, optionsChanged);

			this.CreateEditionUI (this.firstFrame);
		}

		private void CreateEditionUI(FrameBox parent)
		{
			this.CreateGraphUI (parent);
			this.CreateCompteUI (parent);
			this.CreateHasGraphicColumnUI (parent);

			this.UpdateWidgets ();
		}

		private void CreateCompteUI(FrameBox parent)
		{
			var box = this.CreateBox (parent);

			var label = new StaticText
			{
				Parent          = box,
				FormattedText   = FormattedText.Concat ("Compte"),
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			UIBuilder.AdjustWidth (label);

			this.compteController = UIBuilder.CreateAutoCompleteField (this.controller, box, this.NuméroCompte, "Compte", this.ValidateCompteAction, this.CompteChangedAction);
			this.compteController.Box.Dock = DockStyle.Left;

			this.summaryLabel = new StaticText
			{
				Parent          = box,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock            = DockStyle.Left,
				Margins         = new Margins (10, 0, 0, 0),
			};

			this.UpdateComptes ();
			this.UpdateSummary ();
		}

		private void UpdateComptes()
		{
			UIBuilder.UpdateAutoCompleteTextField (this.compteController.EditWidget as AutoCompleteTextField, this.compta.PlanComptable);

			using (this.ignoreChanges.Enter ())
			{
				this.compteController.EditionData.Text = this.NuméroCompte;
			}
		}

		private void UpdateSummary()
		{
			var compte = this.compta.PlanComptable.Where (x => x.Numéro == this.NuméroCompte).FirstOrDefault ();

			if (compte == null)
			{
				if (this.NuméroCompte.IsNullOrEmpty ())
				{
					this.summaryLabel.FormattedText = FormattedText.Concat ("Indéfini").ApplyFontColor (Color.FromName ("Red"));
				}
				else
				{
					this.summaryLabel.FormattedText = FormattedText.Concat ("Inconnu").ApplyFontColor (Color.FromName ("Red"));
				}
			}
			else
			{
				this.summaryLabel.FormattedText = compte.Titre;
			}

			this.summaryLabel.PreferredWidth = System.Math.Min (this.summaryLabel.GetBestFitSize ().Width, 150);
		}

		private void ValidateCompteAction(EditionData data)
		{
			data.ClearError ();
		}

		private void CompteChangedAction(int line, ColumnType columnType)
		{
			if (this.ignoreChanges.IsZero)
			{
				this.NuméroCompte = this.compteController.EditionData.Text;
				this.UpdateSummary ();
				this.OptionsChanged ();
			}
		}

		private FormattedText NuméroCompte
		{
			get
			{
				return this.Options.NuméroCompte.ToSimpleText ();
			}
			set
			{
				this.Options.NuméroCompte = value;
			}
		}


		protected override bool HasBeginnerSpecialist
		{
			get
			{
				return false;
			}
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateGraphWidgets ();
			this.UpdateHasGraphicColumn ();

			this.hasGraphicColumnButton.Visibility = !this.options.ViewGraph;

			base.UpdateWidgets ();
		}


		private ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}


		private AutoCompleteFieldController		compteController;
		private StaticText						summaryLabel;
	}
}
