//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage du résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVAOptionsController : AbstractOptionsController
	{
		public RésuméTVAOptionsController(AbstractController controller)
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

			this.CreateCheckUI (this.firstFrame);
			this.CreateDifférenceUI ();

			this.UpdateWidgets ();
		}

		protected void CreateCheckUI(FrameBox parent)
		{
			var box = this.CreateBox (parent);

			this.montreEcrituresButton = new CheckButton
			{
				Parent         = box,
				FormattedText  = "Montre les écritures",
				ActiveState    = this.Options.MontreEcritures ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			this.montantTTCButton = new CheckButton
			{
				Parent         = box,
				FormattedText  = "Affiche les montants TTC",
				ActiveState    = this.Options.MontantTTC ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 20, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			this.parCodeTVAButton0 = new RadioButton
			{
				Parent         = box,
				FormattedText  = "Résumé par comptes",
				ActiveState    = !this.Options.ParCodesTVA ? ActiveState.Yes : ActiveState.No,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			this.parCodeTVAButton1 = new RadioButton
			{
				Parent         = box,
				FormattedText  = "Résumé par codes TVA",
				ActiveState    = this.Options.ParCodesTVA ? ActiveState.Yes : ActiveState.No,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			UIBuilder.AdjustWidth (this.montreEcrituresButton);
			UIBuilder.AdjustWidth (this.montantTTCButton);
			UIBuilder.AdjustWidth (this.parCodeTVAButton0);
			UIBuilder.AdjustWidth (this.parCodeTVAButton1);

			this.montreEcrituresButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.MontreEcritures = !this.Options.MontreEcritures;
					this.OptionsChanged ();
				}
			};

			this.montantTTCButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.MontantTTC = !this.Options.MontantTTC;
					this.OptionsChanged ();
				}
			};

			this.parCodeTVAButton0.Clicked += delegate
			{
				this.Options.ParCodesTVA = false;
				this.parCodeTVAButton0.ActiveState = ActiveState.Yes;
				this.parCodeTVAButton1.ActiveState = ActiveState.No;
				this.OptionsChanged ();
			};

			this.parCodeTVAButton1.Clicked += delegate
			{
				this.Options.ParCodesTVA = true;
				this.parCodeTVAButton0.ActiveState = ActiveState.No;
				this.parCodeTVAButton1.ActiveState = ActiveState.Yes;
				this.OptionsChanged ();
			};
		}

		protected void CreateDifférenceUI()
		{
			var frame = this.CreateSpecialistFrameUI ();
			this.différenceFrame = this.CreateBox (frame);

			this.différenceButton = new CheckButton
			{
				Parent         = this.différenceFrame,
				FormattedText  = "Ignore les différences inférieures ou égales à",
				ActiveState    = (this.Options.MontantLimite.HasValue || this.Options.PourcentLimite.HasValue) ? ActiveState.Yes : ActiveState.No,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};
			UIBuilder.AdjustWidth (this.différenceButton);

			this.limiteField = new TextFieldEx
			{
				Parent                       = this.différenceFrame,
				PreferredWidth               = 70,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Left,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				Margins                      = new Margins (0, 5, 0, 0),
			};

			this.limiteMontantRadio = new RadioButton
			{
				Parent          = this.différenceFrame,
				Text            = "CHF",
				PreferredHeight = 20,
				AutoToggle      = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			UIBuilder.AdjustWidth (this.limiteMontantRadio);

			this.limitePourcentRadio = new RadioButton
			{
				Parent          = this.différenceFrame,
				Text            = "%",
				PreferredHeight = 20,
				AutoToggle      = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
			UIBuilder.AdjustWidth (this.limitePourcentRadio);

			this.différenceButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					if (this.Options.MontantLimite.HasValue || this.Options.PourcentLimite.HasValue)
					{
						this.Options.MontantLimite  = null;
						this.Options.PourcentLimite = null;
					}
					else
					{
						this.Options.MontantLimite = 0.05m;  // 5 centimes
					}

					this.OptionsChanged ();
				}
			};

			this.limiteField.AcceptingEdition += delegate
			{
				var m = Converters.ParseMontant (this.limiteField.Text);
				if (m.HasValue)
				{
					this.Options.MontantLimite  = m;
					this.Options.PourcentLimite = null;

					this.OptionsChanged ();
				}
				else
				{
					m = Converters.ParsePercent (this.limiteField.Text);
					if (m.HasValue)
					{
						this.Options.MontantLimite  = null;
						this.Options.PourcentLimite = m;

						this.OptionsChanged ();
					}
				}
			};

			this.limiteMontantRadio.Clicked += delegate
			{
				this.Options.MontantLimite  = 0.05m;  // 5 centimes
				this.Options.PourcentLimite = null;

				this.OptionsChanged ();
			};

			this.limitePourcentRadio.Clicked += delegate
			{
				this.Options.MontantLimite  = null;
				this.Options.PourcentLimite = 0.01m;  // 1%

				this.OptionsChanged ();
			};
		}


		protected override bool HasBeginnerSpecialist
		{
			get
			{
				return true;
			}
		}

		protected override void OptionsChanged()
		{
			this.UpdateWidgets ();
			base.OptionsChanged ();
		}

		protected override void LevelChangedAction()
		{
			base.LevelChangedAction ();

			this.différenceFrame.Visibility = this.topPanelLeftController.Specialist;
		}

		protected override void UpdateWidgets()
		{
			this.différenceFrame.Visibility = this.topPanelLeftController.Specialist;
			this.différenceFrame.Enable = this.Options.MontreEcritures;

			using (this.ignoreChanges.Enter ())
			{
				this.montreEcrituresButton.ActiveState =  this.Options.MontreEcritures ? ActiveState.Yes : ActiveState.No;
				this.montantTTCButton.ActiveState      =  this.Options.MontantTTC      ? ActiveState.Yes : ActiveState.No;
				this.parCodeTVAButton0.ActiveState     = !this.Options.ParCodesTVA     ? ActiveState.Yes : ActiveState.No;
				this.parCodeTVAButton1.ActiveState     =  this.Options.ParCodesTVA     ? ActiveState.Yes : ActiveState.No;

				bool hasMontantLimit  = this.Options.MontantLimite.HasValue;
				bool hasPerrcentLimit = this.Options.PourcentLimite.HasValue;

				if (hasMontantLimit)
				{
					this.différenceButton.ActiveState = ActiveState.Yes;

					this.limiteField.Enable = true;
					this.limiteField.Text = Converters.MontantToString (this.Options.MontantLimite, this.compta.Monnaies[0]);
					
					this.limiteMontantRadio.ActiveState = ActiveState.Yes;
					this.limiteMontantRadio.Enable = true;
					
					this.limitePourcentRadio.ActiveState = ActiveState.No;
					this.limitePourcentRadio.Enable = true;
				}
				else if (hasPerrcentLimit)
				{
					this.différenceButton.ActiveState = ActiveState.Yes;
					
					this.limiteField.Enable = true;
					this.limiteField.Text = Converters.PercentToString (this.Options.PourcentLimite);
					
					this.limiteMontantRadio.ActiveState = ActiveState.No;
					this.limiteMontantRadio.Enable = true;
					
					this.limitePourcentRadio.ActiveState = ActiveState.Yes;
					this.limitePourcentRadio.Enable = true;
				}
				else
				{
					this.différenceButton.ActiveState = ActiveState.No;
					
					this.limiteField.Enable = false;
					this.limiteField.Text = null;
					
					this.limiteMontantRadio.ActiveState = ActiveState.No;
					this.limiteMontantRadio.Enable = false;
					
					this.limitePourcentRadio.ActiveState = ActiveState.No;
					this.limitePourcentRadio.Enable = false;
				}
			}

			base.UpdateWidgets ();
		}

		private RésuméTVAOptions Options
		{
			get
			{
				return this.options as RésuméTVAOptions;
			}
		}


		private CheckButton			montreEcrituresButton;
		private CheckButton			montantTTCButton;
		private RadioButton			parCodeTVAButton0;
		private RadioButton			parCodeTVAButton1;

		private FrameBox			différenceFrame;
		private CheckButton			différenceButton;
		private TextFieldEx			limiteField;
		private RadioButton			limiteMontantRadio;
		private RadioButton			limitePourcentRadio;
	}
}
