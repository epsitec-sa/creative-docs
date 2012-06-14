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
	/// Ce contrôleur gère les options d'affichage du résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueOptionsController : AbstractOptionsController
	{
		public RésuméPériodiqueOptionsController(AbstractController controller)
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

			this.CreateMainUI (this.firstFrame);

			var line = this.CreateSpecialistFrameUI ();
			this.CreateCatégoriesUI (line);

			line = this.CreateSpecialistFrameUI ();
			this.CreateDeepUI (line);
			this.CreateZeroUI (line);

			this.UpdateWidgets ();
		}

		protected void CreateMainUI(FrameBox parent)
		{
			this.CreateGraphUI (parent);

			{
				var box = this.CreateBox (parent);

				var lp = new StaticText
				{
					Parent         = box,
					Text           = "Périodicité",
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 10, 0, 0),
				};
				UIBuilder.AdjustWidth (lp);

				this.monthsField = new TextFieldCombo
				{
					Parent          = box,
					PreferredWidth  = 100,
					PreferredHeight = 20,
					MenuButtonWidth = UIBuilder.ComboButtonWidth,
					IsReadOnly      = true,
					Dock            = DockStyle.Left,
					TabIndex        = ++this.tabIndex,
				};

				for (int i = 0; i < 5; i++)
				{
					int months = RésuméPériodiqueOptionsController.IndexToMonths (i);
					this.monthsField.Items.Add (RésuméPériodiqueOptions.MonthsToDescription (months));
				}

				this.cumulButton = new CheckButton
				{
					Parent         = box,
					Text           = "Chiffres cumulés",
					Dock           = DockStyle.Left,
					Margins         = new Margins (20, 0, 0, 0),
					TabIndex       = ++this.tabIndex,
				};
				UIBuilder.AdjustWidth (this.cumulButton);
			}

			{
				this.graphBox = this.CreateBox (parent);

				var label = new StaticText
				{
					Parent         = this.graphBox,
					FormattedText  = "Graphiques",
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 10, 0, 0),
					TabIndex       = ++this.tabIndex,
				};
				UIBuilder.AdjustWidth (label);

				this.stackedGraphButton = new CheckButton
				{
					Parent         = this.graphBox,
					FormattedText  = "cumulés",
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 10, 0, 0),
					TabIndex       = ++this.tabIndex,
				};
				UIBuilder.AdjustWidth (this.stackedGraphButton);

				this.sideBySideGraphButton = new CheckButton
				{
					Parent         = this.graphBox,
					FormattedText  = "côte à côte",
					Dock           = DockStyle.Left,
					TabIndex       = ++this.tabIndex,
				};
				UIBuilder.AdjustWidth (this.sideBySideGraphButton);
			}

			this.monthsField.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.NumberOfMonths = RésuméPériodiqueOptionsController.IndexToMonths (this.monthsField.SelectedItemIndex);
					this.OptionsChanged ();
				}
			};

			this.cumulButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.Cumul = (this.cumulButton.ActiveState == ActiveState.Yes);
					this.OptionsChanged ();
				}
			};

			this.stackedGraphButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.HasStackedGraph = (this.stackedGraphButton.ActiveState == ActiveState.Yes);

					if (this.Options.HasStackedGraph)
					{
						this.Options.HasSideBySideGraph = false;
					}

					this.OptionsChanged ();
				}
			};

			this.sideBySideGraphButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.HasSideBySideGraph = (this.sideBySideGraphButton.ActiveState == ActiveState.Yes);

					if (this.Options.HasSideBySideGraph)
					{
						this.Options.HasStackedGraph = false;
					}

					this.OptionsChanged ();
				}
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
		}

		protected override void UpdateWidgets()
		{
			this.UpdateGraphWidgets ();
			this.UpdateCatégories ();
			this.UpdateDeep ();
			this.UpdateZero ();

			using (this.ignoreChanges.Enter ())
			{
				this.cumulButton.Visibility        = !this.options.ViewGraph;
				this.zeroFilteredButton.Visibility = !this.options.ViewGraph;
				this.graphBox.Visibility           = !this.options.ViewGraph;

				this.monthsField.Text = RésuméPériodiqueOptions.MonthsToDescription (this.Options.NumberOfMonths);
				this.cumulButton.ActiveState           = this.Options.Cumul              ? ActiveState.Yes : ActiveState.No;
				this.stackedGraphButton.ActiveState    = this.Options.HasStackedGraph    ? ActiveState.Yes : ActiveState.No;
				this.sideBySideGraphButton.ActiveState = this.Options.HasSideBySideGraph ? ActiveState.Yes : ActiveState.No;
			}

			base.UpdateWidgets ();
		}


		private static int IndexToMonths(int index)
		{
			switch (index)
			{
				case 0:
					return 1;  // mensuel

				case 1:
					return 2;  // bimestriel

				case 2:
					return 3;  // trimestriel

				case 3:
					return 6;  // semestriel

				case 4:
					return 12;  // annuel

				default:
					return 0;  // inconnu
			}
		}


		private RésuméPériodiqueOptions Options
		{
			get
			{
				return this.options as RésuméPériodiqueOptions;
			}
		}


		private TextFieldCombo		monthsField;
		private CheckButton			cumulButton;
		private FrameBox			graphBox;
		private CheckButton			stackedGraphButton;
		private CheckButton			sideBySideGraphButton;
	}
}
