//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphOptionsController
	{
		public GraphOptionsController(AbstractController controller)
		{
			this.controller = controller;
			this.cube       = controller.DataAccessor.Cube;
			this.options    = controller.DataAccessor.GraphOptions;

			this.ignoreChanges = new SafeCounter ();
		}


		public void CreateUI(Widget parent, System.Action optionsChangedAction)
		{
			this.optionsChangedAction = optionsChangedAction;

			var toolbar = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				BackColor     = Color.FromName ("White"),
				Dock          = DockStyle.Fill,
			};

			var frame = new FrameBox
			{
				Parent         = toolbar,
				Dock           = DockStyle.Fill,
			};

			var levelFrame = new FrameBox
			{
				Parent         = toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			this.beginnerFrame = new FrameBox
			{
				Parent  = frame,
				Dock    = DockStyle.Top,
				Padding = new Margins (5),
			};

			this.separator = new Separator
			{
				Parent          = frame,
				PreferredHeight = 1,
				Dock            = DockStyle.Top,
				Visibility      = false,
			};

			this.specialistFrame = new FrameBox
			{
				Parent     = frame,
				Dock       = DockStyle.Top,
				Padding    = new Margins (5),
				Visibility = false,
			};

			this.levelController = new LevelController (this.controller);
			this.levelController.CreateUI (levelFrame, "Remet les options standards", this.ClearAction, this.LevelChangedAction);
			this.levelController.Specialist = false;

			this.CreateBeginnerUI (beginnerFrame);
			this.CreateSpecialistUI (specialistFrame);
			this.Update ();
		}

		private void CreateBeginnerUI(Widget parent)
		{
			var leftFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 40,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 40,
				Dock            = DockStyle.Fill,
			};

			var topFrame = new FrameBox
			{
				Parent          = rightFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 1),
			};

			var bottomFrame = new FrameBox
			{
				Parent          = rightFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.sideBySideModeButton = this.CreateButton (leftFrame, "Graph.SideBySide");
			this.stackedModeButton    = this.CreateButton (leftFrame, "Graph.Stacked");
			this.linesModeButton      = this.CreateButton (leftFrame, "Graph.Lines");
			this.pieModeButton        = this.CreateButton (leftFrame, "Graph.Pie");
			this.arrayModeButton      = this.CreateButton (leftFrame, "Graph.Array");

			{
				new StaticText
				{
					Parent         = topFrame,
					FormattedText  = "Axe principal",
					PreferredWidth = 80,
					Dock           = DockStyle.Left,
				};

				this.primaryDimensionCombo = new TextFieldCombo
				{
					Parent          = topFrame,
					IsReadOnly      = true,
					PreferredWidth  = 120,
					PreferredHeight = 20,
					MenuButtonWidth = UIBuilder.ComboButtonWidth,
					Dock            = DockStyle.Left,
				};

				new StaticText
				{
					Parent         = topFrame,
					FormattedText  = "Montrer",
					PreferredWidth = 45,
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 0, 0, 0),
				};

				this.primaryFilterFrame = UIBuilder.CreatePseudoCombo (topFrame, out this.primaryFilterField, out this.primaryFilterButton);
				this.primaryFilterFrame.PreferredWidth = 80;

				this.swapButton = new IconButton
				{
					Parent          = topFrame,
					IconUri         = UIBuilder.GetResourceIconUri ("Graph.Dimensions.Swap"),
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (1, 0, 0, 0),
				};

				ToolTip.Default.SetToolTip (this.swapButton, "Permute les axes");
			}

			{
				new StaticText
				{
					Parent         = bottomFrame,
					FormattedText  = "Axe secondaire",
					PreferredWidth = 80,
					Dock           = DockStyle.Left,
				};

				this.secondaryDimensionCombo = new TextFieldCombo
				{
					Parent          = bottomFrame,
					IsReadOnly      = true,
					PreferredWidth  = 120,
					PreferredHeight = 20,
					MenuButtonWidth = UIBuilder.ComboButtonWidth,
					Dock            = DockStyle.Left,
				};

				new StaticText
				{
					Parent         = bottomFrame,
					FormattedText  = "Montrer",
					PreferredWidth = 45,
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 0, 0, 0),
				};

				this.secondaryFilterFrame = UIBuilder.CreatePseudoCombo (bottomFrame, out this.secondaryFilterField, out this.secondaryFilterButton);
				this.secondaryFilterFrame.PreferredWidth = 80;

				new StaticText
				{
					Parent          = bottomFrame,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (1, 0, 0, 0),
				};
			}

			new StaticText
			{
				Parent         = bottomFrame,
				FormattedText  = "Style",
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 0, 0, 0),
			};

			this.styleCombo = new TextFieldCombo
			{
				Parent          = bottomFrame,
				IsReadOnly      = true,
				PreferredWidth  = 110,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				Dock            = DockStyle.Left,
			};

			//	Connexion des événements.
			this.sideBySideModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.SideBySide;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.stackedModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Stacked;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.linesModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Lines;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.pieModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Pie;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.arrayModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Array;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.swapButton.Clicked += delegate
			{
				var d1 = this.options.PrimaryDimension;
				var d2 = this.options.SecondaryDimension;

				this.options.PrimaryDimension   = d2;
				this.options.SecondaryDimension = d1;

				var t = new List<FormattedText> ();
				t.AddRange (this.options.PrimaryFilter);
				this.options.PrimaryFilter.Clear ();
				this.options.PrimaryFilter.AddRange (this.options.SecondaryFilter);
				this.options.SecondaryFilter.Clear ();
				this.options.SecondaryFilter.AddRange (t);

				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.primaryDimensionCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.PrimaryDimension = this.TextToDimension (this.primaryDimensionCombo.FormattedText);
					this.options.PrimaryFilter.Clear ();

					this.UpdateWidgets ();
					this.optionsChangedAction ();
				}
			};

			this.secondaryDimensionCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.SecondaryDimension = this.TextToDimension (this.secondaryDimensionCombo.FormattedText);
					this.options.SecondaryFilter.Clear ();

					this.UpdateWidgets ();
					this.optionsChangedAction ();
				}
			};

			this.primaryFilterField.Clicked += delegate
			{
				this.ShowFilterMenu (this.primaryFilterFrame, this.options.PrimaryDimension, this.options.PrimaryFilter);
			};

			this.primaryFilterButton.Clicked += delegate
			{
				this.ShowFilterMenu (this.primaryFilterFrame, this.options.PrimaryDimension, this.options.PrimaryFilter);
			};

			this.secondaryFilterField.Clicked += delegate
			{
				this.ShowFilterMenu (this.secondaryFilterFrame, this.options.SecondaryDimension, this.options.SecondaryFilter);
			};

			this.secondaryFilterButton.Clicked += delegate
			{
				this.ShowFilterMenu (this.secondaryFilterFrame, this.options.SecondaryDimension, this.options.SecondaryFilter);
			};

			this.styleCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					options.Style = GraphOptionsController.TextToStyle (this.styleCombo.FormattedText);
					this.optionsChangedAction ();
				}
			};
		}

		private void CreateSpecialistUI(Widget parent)
		{
			this.hasThresholdButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Seuil",
				PreferredWidth = 50,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
			};

			this.thresholdValueField = new TextFieldEx
			{
				Parent                       = parent,
				PreferredWidth               = 70,
				AutoToggle                   = false,
				Dock                         = DockStyle.Left,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				Margins                      = new Margins (0, 10, 0, 0),
			};

			this.startAtZeroButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Zéro inclu",
				PreferredWidth = 80,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
			};

			this.explodedPieButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Secteurs éclatés",
				PreferredWidth = 110,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
			};

			this.legendButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Légendes",
				PreferredWidth = 70,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
			};

			//	Connexion des événements.
			this.legendButton.Clicked += delegate
			{
				this.options.HasLegend = !this.options.HasLegend;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.startAtZeroButton.Clicked += delegate
			{
				this.options.StartAtZero = !this.options.StartAtZero;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.explodedPieButton.Clicked += delegate
			{
				this.options.ExplodedPie = !this.options.ExplodedPie;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.hasThresholdButton.Clicked += delegate
			{
				this.options.HasThreshold = !this.options.HasThreshold;
				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			this.thresholdValueField.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					var value = Converters.ParsePercent (this.thresholdValueField.Text);
					if (value.HasValue)
					{
						this.options.ThresholdValue = value.Value;
						this.UpdateWidgets ();
						this.optionsChangedAction ();
					}
				}
			};
		}


		private void ClearAction()
		{
			this.options.Clear ();
			this.UpdateWidgets ();
			this.optionsChangedAction ();
		}

		private void LevelChangedAction()
		{
			this.isSpecialist = this.levelController.Specialist;

			this.specialistFrame.Visibility = this.levelController.Specialist;
			this.separator.Visibility = this.levelController.Specialist;
		}

	
		public void Update()
		{
			this.InitComboDimension (this.primaryDimensionCombo);
			this.InitComboDimension (this.secondaryDimensionCombo);
			this.InitComboStyle (this.styleCombo);
			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			using (this.ignoreChanges.Enter())
			{
				this.startAtZeroButton.Visibility = this.options.Mode == GraphMode.SideBySide || this.options.Mode == GraphMode.Lines;
				this.explodedPieButton.Visibility = this.options.Mode == GraphMode.Pie;
				this.legendButton.Visibility      = this.options.Mode != GraphMode.Array;

				this.sideBySideModeButton.ActiveState = (this.options.Mode == GraphMode.SideBySide) ? ActiveState.Yes : ActiveState.No;
				this.stackedModeButton.ActiveState    = (this.options.Mode == GraphMode.Stacked   ) ? ActiveState.Yes : ActiveState.No;
				this.linesModeButton.ActiveState      = (this.options.Mode == GraphMode.Lines     ) ? ActiveState.Yes : ActiveState.No;
				this.pieModeButton.ActiveState        = (this.options.Mode == GraphMode.Pie       ) ? ActiveState.Yes : ActiveState.No;
				this.arrayModeButton.ActiveState      = (this.options.Mode == GraphMode.Array     ) ? ActiveState.Yes : ActiveState.No;

				this.legendButton.ActiveState       = this.options.HasLegend    ? ActiveState.Yes : ActiveState.No;
				this.startAtZeroButton.ActiveState  = this.options.StartAtZero  ? ActiveState.Yes : ActiveState.No;
				this.explodedPieButton.ActiveState  = this.options.ExplodedPie  ? ActiveState.Yes : ActiveState.No;
				this.hasThresholdButton.ActiveState = this.options.HasThreshold ? ActiveState.Yes : ActiveState.No;

				this.thresholdValueField.Enable = this.options.HasThreshold;
				this.thresholdValueField.Text = Converters.PercentToString (this.options.ThresholdValue);

				this.primaryDimensionCombo.FormattedText   = this.DimensionToText (this.options.PrimaryDimension);
				this.secondaryDimensionCombo.FormattedText = this.DimensionToText (this.options.SecondaryDimension);

				if (this.cube.Dimensions != 0)
				{
					this.primaryFilterField.FormattedText   = GraphOptionsController.GetFilterSummary (this.options.PrimaryFilter,   this.cube.GetCount (this.options.PrimaryDimension));
					this.secondaryFilterField.FormattedText = GraphOptionsController.GetFilterSummary (this.options.SecondaryFilter, this.cube.GetCount (this.options.SecondaryDimension));
				}

				this.styleCombo.FormattedText = GraphOptionsController.StyleToText (this.options.Style);
			}
		}

		private BackIconButton CreateButton(Widget parent, string icon)
		{
			return new BackIconButton
			{
				Parent              = parent,
				IconUri             = UIBuilder.GetResourceIconUri (icon),
				BackColor           = UIBuilder.SelectionColor,
				PreferredIconSize   = new Size (32, 32),
				PreferredSize       = new Size (40, 40),
				Dock                = DockStyle.Left,
				VerticalAlignment   = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				AutoFocus           = false,
			};
		}


		private void InitComboDimension(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			for (int dimension = 0; dimension < cube.Dimensions; dimension++)
			{
				combo.Items.Add (this.cube.GetDimensionTitle (dimension));
			}
		}

		private int TextToDimension(FormattedText text)
		{
			for (int dimension = 0; dimension < cube.Dimensions; dimension++)
			{
				if (text == this.cube.GetDimensionTitle (dimension))
				{
					return dimension;
				}
			}

			return 0;
		}

		private FormattedText DimensionToText(int dimension)
		{
			if (dimension < this.cube.Dimensions)
			{
				return this.cube.GetDimensionTitle (dimension);
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		private void InitComboStyle(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var style in GraphOptionsController.Styles)
			{
				combo.Items.Add (GraphOptionsController.StyleToText (style));
			}
		}

		private static GraphStyle TextToStyle(FormattedText text)
		{
			foreach (var style in GraphOptionsController.Styles)
			{
				if (text == GraphOptionsController.StyleToText (style))
				{
					return style;
				}
			}

			return GraphStyle.Rainbow;
		}

		private static FormattedText StyleToText(GraphStyle style)
		{
			switch (style)
			{
				case GraphStyle.Rainbow:
					return "Arc-en-ciel";

				case GraphStyle.LightRainbow:
					return "Arc-en-ciel pastel";

				case GraphStyle.DarkRainbow:
					return "Arc-en-ciel foncé";

				case GraphStyle.Grey:
					return "Niveaux de gris";

				case GraphStyle.BlackAndWhite:
					return "Noir et blanc";

				case GraphStyle.Red:
					return "Rouge";

				case GraphStyle.Green:
					return "Vert";

				case GraphStyle.Blue:
					return "Bleu";

				case GraphStyle.Fire:
					return "Feu";

				default:
					return "?";
			}
		}

		private static IEnumerable<GraphStyle> Styles
		{
			get
			{
				yield return GraphStyle.Rainbow;
				yield return GraphStyle.LightRainbow;
				yield return GraphStyle.DarkRainbow;
				yield return GraphStyle.Red;
				yield return GraphStyle.Green;
				yield return GraphStyle.Blue;
				yield return GraphStyle.Fire;
				yield return GraphStyle.Grey;
				yield return GraphStyle.BlackAndWhite;
			}
		}


		private static FormattedText GetFilterSummary(List<FormattedText> filter, int count)
		{
			if (filter.Count == 0)
			{
				return "Tout";
			}
			else
			{
				int n = System.Math.Max (count-filter.Count, 0);
				if (n == 0)
				{
					return "Rien";
				}
				else if (n == 1)
				{
					return "1 élément";
				}
				else
				{
					return string.Format ("{0} éléments", n.ToString ());
				}
			}
		}


		private void ShowFilterMenu(Widget parentButton, int dimension, List<FormattedText> filter)
		{
			//	Affiche le menu permettant de choisir le filtre.
			var menu = new VMenu ();

			this.AddClearFilterToMenu (menu, dimension, filter);
			this.AddAllFilterToMenu (menu, dimension, filter);

			menu.Items.Add (new MenuSeparator ());

			int n = this.cube.GetCount (dimension);
			for (int i = 0; i < n; i++)
			{
				this.AddFilterToMenu (menu, dimension, filter, i);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddClearFilterToMenu(VMenu menu, int dimension, List<FormattedText> filter)
		{
			if (filter.Count != 0)
			{
				var item = new MenuItem ()
				{
					FormattedText = "Tout montrer",
				};

				item.Clicked += delegate
				{
					filter.Clear ();

					this.UpdateWidgets ();
					this.optionsChangedAction ();
				};

				menu.Items.Add (item);
			}
		}

		private void AddAllFilterToMenu(VMenu menu, int dimension, List<FormattedText> filter)
		{
			var item = new MenuItem ()
			{
				FormattedText = "Tout cacher",
			};

			item.Clicked += delegate
			{
				filter.Clear ();

				int n = this.cube.GetCount (dimension);
				for (int i = 0; i < n; i++)
				{
					filter.Add (this.cube.GetShortTitle (dimension, i));
				}

				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			menu.Items.Add (item);
		}

		private void AddFilterToMenu(VMenu menu, int dimension, List<FormattedText> filter, int index)
		{
			var shortTitle = this.cube.GetShortTitle (dimension, index);
			bool selected = !filter.Contains (shortTitle);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (selected),
				FormattedText = this.cube.GetTitle (dimension, index),
			};

			item.Clicked += delegate
			{
				if (filter.Contains (shortTitle))
				{
					filter.Remove (shortTitle);
				}
				else
				{
					filter.Add (shortTitle);
				}

				this.UpdateWidgets ();
				this.optionsChangedAction ();
			};

			menu.Items.Add (item);
		}


		private readonly AbstractController		controller;
		private readonly Cube					cube;
		private readonly GraphOptions			options;
		private readonly SafeCounter			ignoreChanges;

		private bool							isSpecialist;

		private FrameBox						beginnerFrame;
		private Separator						separator;
		private FrameBox						specialistFrame;

		private System.Action					optionsChangedAction;
		private LevelController					levelController;
		private BackIconButton					sideBySideModeButton;
		private BackIconButton					stackedModeButton;
		private BackIconButton					linesModeButton;
		private BackIconButton					pieModeButton;
		private BackIconButton					arrayModeButton;
		private TextFieldCombo					primaryDimensionCombo;
		private TextFieldCombo					secondaryDimensionCombo;
		private FrameBox						primaryFilterFrame;
		private StaticText						primaryFilterField;
		private GlyphButton						primaryFilterButton;
		private FrameBox						secondaryFilterFrame;
		private StaticText						secondaryFilterField;
		private GlyphButton						secondaryFilterButton;
		private IconButton						swapButton;
		private TextFieldCombo					styleCombo;
		private CheckButton						legendButton;
		private CheckButton						startAtZeroButton;
		private CheckButton						explodedPieButton;
		private CheckButton						hasThresholdButton;
		private TextFieldEx						thresholdValueField;
	}
}
