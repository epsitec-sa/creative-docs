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
	/// Ce contrôleur gère les options d'affichage des soldes de la comptabilité.
	/// </summary>
	public class SoldesOptionsController : AbstractOptionsController
	{
		public SoldesOptionsController(AbstractController controller)
			: base (controller)
		{
			this.soldesColumnControllers = new List<SoldesColumnController> ();
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
				Dock            = DockStyle.Top,
			};

			var leftFrame = new FrameBox
			{
				Parent          = frame,
				Dock            = DockStyle.Fill,
			};

			this.rightFrame = new FrameBox
			{
				Parent          = frame,
				Dock            = DockStyle.Right,
			};

			var line1 = new FrameBox
			{
				Parent          = leftFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.CreateGraphUI (line1);

			new StaticText
			{
				Parent         = line1,
				Text           = "Résolution",
				PreferredWidth = 60,
				Dock           = DockStyle.Left,
			};

			this.resolutionField = new TextFieldCombo
			{
				Parent          = line1,
				PreferredWidth  = 70,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
				Margins         = new Margins (0, 10, 0, 0),
			};

			for (int i = 0; i < 5; i++)
			{
				int resolution = SoldesOptionsController.IndexToResolution (i);
				this.resolutionField.Items.Add (SoldesOptions.ResolutionToDescription (resolution));
			}

			new StaticText
			{
				Parent         = line1,
				Text           = "Tranches",
				PreferredWidth = 50,
				Dock           = DockStyle.Left,
			};

			this.countField = new TextFieldEx
			{
				Parent                       = line1,
				PreferredWidth               = 60,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Left,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex                     = ++this.tabIndex,
				Margins                      = new Margins (0, 20, 0, 0),
			};

			this.cumulButton = new CheckButton
			{
				Parent         = line1,
				FormattedText  = "Chiffres cumulés",
				PreferredWidth = 100,
				Dock           = DockStyle.Left,
				TabIndex       = ++this.tabIndex,
			};

			this.resolutionField.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Options.Resolution = SoldesOptionsController.IndexToResolution (this.resolutionField.SelectedItemIndex);
					this.OptionsChanged ();
				}
			};

			this.countField.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					int? value = Converters.ParseInt (this.countField.Text);
					if (value.HasValue)
					{
						this.Options.Count = value.Value;
						this.OptionsChanged ();
					}
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

		protected override void LevelChangedAction()
		{
			base.LevelChangedAction ();
		}

		protected override void UpdateWidgets()
		{
			this.UpdateGraphWidgets ();

			using (this.ignoreChanges.Enter ())
			{
				this.resolutionField.Text = SoldesOptions.ResolutionToDescription (this.Options.Resolution);
				this.countField.Text = Converters.IntToString (this.Options.Count);
				this.cumulButton.ActiveState = this.Options.Cumul ? ActiveState.Yes : ActiveState.No;

				this.CreateControllers ();
				this.UpdateControllers ();
			}

			base.UpdateWidgets ();
		}

		private void CreateControllers()
		{
			int count = this.Options.SoldesColumns.Count;

			if (this.soldesColumnControllers.Count == count)
			{
				return;
			}

			this.soldesColumnControllers.Clear ();
			this.rightFrame.Children.Clear ();

			for (int i = 0; i < count; i++)
			{
				var c = new SoldesColumnController (this.controller);
				var frame = c.CreateUI (this.rightFrame, i, this.AddSubController, this.ControllerChanged);
				frame.Margins = new Margins (0, 0, i == 0 ? 0 : -1, 0);

				this.soldesColumnControllers.Add (c);
			}
		}

		private void UpdateControllers()
		{
			for (int i = 0; i < this.soldesColumnControllers.Count; i++)
			{
				var c = this.soldesColumnControllers[i];
				c.SoldesColumn = this.Options.SoldesColumns[i];
			}
		}

		private void AddSubController(int rank)
		{
			if (rank == 0)  // add ?
			{
				var n = new SoldesColumn ();

				if (this.Options.SoldesColumns.Any ())
				{
					n.DateDébut = this.Options.SoldesColumns.Last ().DateDébut;
				}

				this.Options.SoldesColumns.Add (n);
			}
			else  // sub ?
			{
				this.Options.SoldesColumns.RemoveAt (rank);
			}

			this.CreateControllers ();
			this.UpdateControllers ();

			this.OptionsChanged ();
		}

		private void ControllerChanged(int rank)
		{
			var c = this.soldesColumnControllers[rank];

			this.Options.SoldesColumns[rank].NuméroCompte = c.SoldesColumn.NuméroCompte;
			this.Options.SoldesColumns[rank].DateDébut    = c.SoldesColumn.DateDébut;

			this.OptionsChanged ();
		}


		private static int IndexToResolution(int index)
		{
			switch (index)
			{
				case 0:
					return 1;  // journalier

				case 1:
					return 7;  // hebdomadaire

				case 2:
					return 10;

				case 3:
					return 20;

				case 4:
					return 30;

				default:
					return 0;  // inconnu
			}
		}


		private SoldesOptions Options
		{
			get
			{
				return this.options as SoldesOptions;
			}
		}


		private readonly List<SoldesColumnController>	soldesColumnControllers;

		private TextFieldCombo							resolutionField;
		private TextFieldEx								countField;
		private CheckButton								cumulButton;
		private FrameBox								rightFrame;
	}
}
