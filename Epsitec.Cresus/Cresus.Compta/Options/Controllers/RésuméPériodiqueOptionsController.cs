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

			this.CreateMainUI (this.mainFrame);

			this.UpdateWidgets ();
		}

		protected void CreateMainUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				TabIndex        = ++this.tabIndex,
			};

			this.CreateGraphUI (frame);

			new StaticText
			{
				Parent         = frame,
				Text           = "Périodicité",
				PreferredWidth = 60,
				Dock           = DockStyle.Left,
			};

			this.monthsField = new TextFieldCombo
			{
				Parent          = frame,
				PreferredWidth  = 100,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
				Margins         = new Margins (0, 20, 0, 0),
			};

			for (int i = 0; i < 5; i++)
			{
				int months = RésuméPériodiqueOptionsController.IndexToMonths (i);
				this.monthsField.Items.Add (RésuméPériodiqueOptions.MonthsToDescription (months));
			}

			this.cumulButton = new CheckButton
			{
				Parent         = frame,
				Text           = "Chiffres cumulés",
				PreferredWidth = 120,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.nullButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche en blanc les montants nuls",
				PreferredWidth = 200,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.graphLabel = new StaticText
			{
				Parent         = frame,
				FormattedText  = "Graphiques",
				PreferredWidth = 65,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.stackedGraphButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Cumulés",
				PreferredWidth = 70,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.sideBySideGraphButton = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Côte à côte",
				PreferredWidth = 90,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

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

			this.nullButton.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.ZeroDisplayedInWhite = (this.nullButton.ActiveState == ActiveState.Yes);
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

			using (this.ignoreChanges.Enter ())
			{
				this.cumulButton.Visibility           = !this.options.ViewGraph;
				this.nullButton.Visibility            = !this.options.ViewGraph;
				this.graphLabel.Visibility            = !this.options.ViewGraph;
				this.stackedGraphButton.Visibility    = !this.options.ViewGraph;
				this.sideBySideGraphButton.Visibility = !this.options.ViewGraph;

				this.monthsField.Text = RésuméPériodiqueOptions.MonthsToDescription (this.Options.NumberOfMonths);
				this.cumulButton.ActiveState           = this.Options.Cumul                ? ActiveState.Yes : ActiveState.No;
				this.nullButton.ActiveState            = this.Options.ZeroDisplayedInWhite ? ActiveState.Yes : ActiveState.No;
				this.stackedGraphButton.ActiveState    = this.Options.HasStackedGraph      ? ActiveState.Yes : ActiveState.No;
				this.sideBySideGraphButton.ActiveState = this.Options.HasSideBySideGraph   ? ActiveState.Yes : ActiveState.No;
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
		private CheckButton			nullButton;
		private StaticText			graphLabel;
		private CheckButton			stackedGraphButton;
		private CheckButton			sideBySideGraphButton;
	}
}
