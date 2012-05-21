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
			this.options    = controller.DataAccessor.Options.GraphOptions;

			this.ignoreChanges = new SafeCounter ();
		}


		public void CreateUI(Widget parent, System.Action optionsChangedAction)
		{
			this.optionsChangedAction = optionsChangedAction;

			var toolbar = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
			};

			this.commonFrame = new FrameBox
			{
				Parent  = toolbar,
				Dock    = DockStyle.Top,
			};

			this.separator = new Separator
			{
				Parent          = toolbar,
				PreferredHeight = 1,
				Dock            = DockStyle.Top,
				Visibility      = false,
			};

			this.detailedFrame = new FrameBox
			{
				Parent     = toolbar,
				Dock       = DockStyle.Top,
				Visibility = false,
			};

			this.CreateCommonUI (commonFrame);
			this.CreateDetailedUI (detailedFrame);
			this.Update ();
		}

		private void CreateCommonUI(Widget parent)
		{
			var leftFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 40,
				Dock            = DockStyle.Left,
				Padding         = new Margins (5),
			};

			new Separator
			{
				Parent         = parent,
				PreferredWidth = 1,
				Dock           = DockStyle.Left,
			};

			var middleFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 40,
				Dock            = DockStyle.Left,
				Padding         = new Margins (5),
			};

			var rightFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 40,
				Dock            = DockStyle.Left,
				Padding         = new Margins (5),
			};

			var topFrame = new FrameBox
			{
				Parent          = middleFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 1),
			};

			var bottomFrame = new FrameBox
			{
				Parent          = middleFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.sideBySideModeButton = this.CreateButton (leftFrame, "Graph.SideBySide", GraphOptions.GetDescription (GraphMode.SideBySide));
			this.stackedModeButton    = this.CreateButton (leftFrame, "Graph.Stacked",    GraphOptions.GetDescription (GraphMode.Stacked));
			this.linesModeButton      = this.CreateButton (leftFrame, "Graph.Lines",      GraphOptions.GetDescription (GraphMode.Lines));
			this.pieModeButton        = this.CreateButton (leftFrame, "Graph.Pie",        GraphOptions.GetDescription (GraphMode.Pie));
			this.arrayModeButton      = this.CreateButton (leftFrame, "Graph.Array",      GraphOptions.GetDescription (GraphMode.Array));

			//	Partie centrale.
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

				this.hasThresholdButton0 = new CheckButton
				{
					Parent         = topFrame,
					FormattedText  = "Seuil",
					PreferredWidth = 50,
					AutoToggle     = false,
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 0, 0, 0),
				};

				this.thresholdValueField0 = new TextFieldEx
				{
					Parent                       = topFrame,
					PreferredWidth               = 70,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Left,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				this.thresholdAddButton0 = new GlyphButton
				{
					Parent          = topFrame,
					GlyphShape      = GlyphShape.Plus,
					ButtonStyle     = ButtonStyle.ToolItem,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins        = new Margins (1, 0, 0, 0),
				};

				this.thresholdSubButton0 = new GlyphButton
				{
					Parent          = topFrame,
					GlyphShape      = GlyphShape.Minus,
					ButtonStyle     = ButtonStyle.ToolItem,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins        = new Margins (-1, 0, 0, 0),
				};
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

				this.hasThresholdButton1 = new CheckButton
				{
					Parent         = bottomFrame,
					FormattedText  = "Seuil",
					PreferredWidth = 50,
					AutoToggle     = false,
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 0, 0, 0),
				};

				this.thresholdValueField1 = new TextFieldEx
				{
					Parent                       = bottomFrame,
					PreferredWidth               = 70,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Left,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				this.thresholdAddButton1 = new GlyphButton
				{
					Parent          = bottomFrame,
					GlyphShape      = GlyphShape.Plus,
					ButtonStyle     = ButtonStyle.ToolItem,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins        = new Margins (1, 0, 0, 0),
				};

				this.thresholdSubButton1 = new GlyphButton
				{
					Parent          = bottomFrame,
					GlyphShape      = GlyphShape.Minus,
					ButtonStyle     = ButtonStyle.ToolItem,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins        = new Margins (-1, 0, 0, 0),
				};
			}

			//	Partie droite.
			this.swapButton = new IconButton
			{
				Parent          = rightFrame,
				IconUri         = UIBuilder.GetResourceIconUri ("Graph.Dimensions.Swap"),
				PreferredWidth  = 40,
				PreferredHeight = 40,
				Dock            = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.swapButton, "Permute les axes");

			//	Connexion des événements.
			this.sideBySideModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.SideBySide;
				this.OptionsChanged ();
			};

			this.stackedModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Stacked;
				this.OptionsChanged ();
			};

			this.linesModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Lines;
				this.OptionsChanged ();
			};

			this.pieModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Pie;
				this.OptionsChanged ();
			};

			this.arrayModeButton.Clicked += delegate
			{
				this.options.Mode = GraphMode.Array;
				this.OptionsChanged ();
			};

			this.swapButton.Clicked += delegate
			{
				var d                           = this.options.PrimaryDimension;
				this.options.PrimaryDimension   = this.options.SecondaryDimension;
				this.options.SecondaryDimension = d;

				var h                      = this.options.HasThreshold0;
				this.options.HasThreshold0 = this.options.HasThreshold1;
				this.options.HasThreshold1 = h;

				var v                        = this.options.ThresholdValue0;
				this.options.ThresholdValue0 = this.options.ThresholdValue1;
				this.options.ThresholdValue1 = v;

				var t = new List<FormattedText> ();
				t.AddRange (this.options.PrimaryFilter);
				this.options.PrimaryFilter.Clear ();
				this.options.PrimaryFilter.AddRange (this.options.SecondaryFilter);
				this.options.SecondaryFilter.Clear ();
				this.options.SecondaryFilter.AddRange (t);

				this.OptionsChanged ();
			};

			this.primaryDimensionCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.PrimaryDimension = this.TextToDimension (this.primaryDimensionCombo.FormattedText);
					this.options.PrimaryFilter.Clear ();

					this.OptionsChanged ();
				}
			};

			this.secondaryDimensionCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.SecondaryDimension = this.TextToDimension (this.secondaryDimensionCombo.FormattedText);
					this.options.SecondaryFilter.Clear ();

					this.OptionsChanged ();
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
		}

		private void CreateDetailedUI(Widget parent)
		{
			var frame1 = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 210,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			new Separator
			{
				Parent         = parent,
				PreferredWidth = 1,
				Dock           = DockStyle.Left,
			};

			var frame2 = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 210,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			new Separator
			{
				Parent         = parent,
				PreferredWidth = 1,
				Dock           = DockStyle.Left,
			};

			var frame3 = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 210,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			new Separator
			{
				Parent         = parent,
				PreferredWidth = 1,
				Dock           = DockStyle.Left,
			};

			var frame4 = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5),
			};

			//	Colonne 1.
			{
				var line = new FrameBox
				{
					Parent          = frame1,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
				};

				new StaticText
				{
					Parent         = line,
					FormattedText  = "Style",
					PreferredWidth = 30,
					Dock           = DockStyle.Left,
				};

				this.styleCombo = new TextFieldCombo
				{
					Parent          = line,
					IsReadOnly      = true,
					PreferredWidth  = 110,
					PreferredHeight = 20,
					MenuButtonWidth = UIBuilder.ComboButtonWidth,
					Dock            = DockStyle.Left,
				};
			}

			this.legendButton = new CheckButton
			{
				Parent         = frame1,
				FormattedText  = "Légendes",
				PreferredWidth = 80,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
				Margins        = new Margins (0, 0, 5, 0),
			};

			//	Colonne 2.
			this.startAtZeroButton = new CheckButton
			{
				Parent         = frame2,
				FormattedText  = "Zéro inclu",
				PreferredWidth = 80,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
				Margins        = new Margins (0, 0, 0, 5),
			};

			this.piePercentsButton = new CheckButton
			{
				Parent         = frame2,
				FormattedText  = "Pourcentages",
				PreferredWidth = 90,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
			};

			this.pieValuesButton = new CheckButton
			{
				Parent         = frame2,
				FormattedText  = "Montants",
				PreferredWidth = 80,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
			};

			this.linesButton = new CheckButton
			{
				Parent         = frame2,
				FormattedText  = "Traits",
				PreferredWidth = 60,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
			};

			{
				var line = new FrameBox
				{
					Parent          = frame2,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
				};

				this.pointLabel = new StaticText
				{
					Parent         = line,
					FormattedText  = "Points",
					PreferredWidth = 40,
					Dock           = DockStyle.Left,
				};

				this.pointCombo = new TextFieldCombo
				{
					Parent          = line,
					IsReadOnly      = true,
					PreferredWidth  = 110,
					PreferredHeight = 20,
					MenuButtonWidth = UIBuilder.ComboButtonWidth,
					Dock            = DockStyle.Left,
				};
			}

			//	Colonne 3.
			this.fontSizeController = new SliderController ();
			this.fontSizeController.CreateUI (frame3, 4.0, 30.0, 1.0, 10.0, "Taille police", delegate
			{
				this.options.FontSize = this.fontSizeController.Value;
				this.OptionsChanged ();
			});

			this.borderThicknessController = new SliderController ();
			this.borderThicknessController.CreateUI (frame3, 0.0, 3.0, 0.5, 1.0, "Contours", delegate
			{
				this.options.BorderThickness = this.borderThicknessController.Value;
				this.OptionsChanged ();
			});

			this.barThicknessController = new SliderController ();
			this.barThicknessController.CreateUI (frame3, 0.1, 1.0, 0.1, 0.8, "Barres", delegate
			{
				this.options.BarThickness = this.barThicknessController.Value;
				this.OptionsChanged ();
			});

			this.barOverlapController = new SliderController ();
			this.barOverlapController.CreateUI (frame3, 0.0, 1.0, 0.1, 0.0, "Chevauchement", delegate
			{
				this.options.BarOverlap = this.barOverlapController.Value;
				this.OptionsChanged ();
			});

			this.lineAlphaController = new SliderController ();
			this.lineAlphaController.CreateUI (frame3, 0.1, 1.0, 0.1, 1.0, "Opacité", delegate
			{
				this.options.LineAlpha = this.lineAlphaController.Value;
				this.OptionsChanged ();
			});

			this.lineWidthController = new SliderController ();
			this.lineWidthController.CreateUI (frame3, 1.0, 20.0, 1.0, 2.0, "Taille traits", delegate
			{
				this.options.LineWidth = this.lineWidthController.Value;
				this.OptionsChanged ();
			});

			this.pointWidthController = new SliderController ();
			this.pointWidthController.CreateUI (frame3, 1.0, 20.0, 1.0, 4.0, "Taille points", delegate
			{
				this.options.PointWidth = this.pointWidthController.Value;
				this.OptionsChanged ();
			});

			this.explodedPieFactorController = new SliderController ();
			this.explodedPieFactorController.CreateUI (frame3, 0.0, 4.0, 0.5, 1.0, "Eloignement", delegate
			{
				this.options.ExplodedPieFactor = this.explodedPieFactorController.Value;
				this.OptionsChanged ();
			});

			//	Colonne 4.
			new StaticText
			{
				Parent        = frame4,
				FormattedText = "Titre",
				Dock          = DockStyle.Top,
				Margins       = new Margins (0, 0, 0, 2),
			};

			this.titleField = new TextFieldMultiEx
			{
				Parent                       = frame4,
				MinHeight                    = 20,
				Dock                         = DockStyle.Fill,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Connexion des événements.
			this.styleCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					options.Style = GraphOptionsController.TextToStyle (this.styleCombo.FormattedText);
					this.OptionsChanged ();
				}
			};

			this.piePercentsButton.Clicked += delegate
			{
				this.options.PiePercents = !this.options.PiePercents;

				if (this.options.PiePercents)
				{
					this.options.PieValues = false;
				}

				this.OptionsChanged ();
			};

			this.pieValuesButton.Clicked += delegate
			{
				this.options.PieValues = !this.options.PieValues;

				if (this.options.PieValues)
				{
					this.options.PiePercents = false;
				}

				this.OptionsChanged ();
			};

			this.linesButton.Clicked += delegate
			{
				this.options.HasLines = !this.options.HasLines;

				if (!this.options.HasLines && this.options.GraphPoints == GraphPoint.None)
				{
					this.options.GraphPoints = GraphPoint.Circle;
				}

				this.OptionsChanged ();
			};

			this.pointCombo.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					options.GraphPoints = GraphOptionsController.TextToPoint (this.pointCombo.FormattedText);

					if (this.options.GraphPoints == GraphPoint.None)
					{
						this.options.HasLines = true;
					}

					this.OptionsChanged ();
				}
			};

			this.legendButton.Clicked += delegate
			{
				this.options.HasLegend = !this.options.HasLegend;
				this.OptionsChanged ();
			};

			this.startAtZeroButton.Clicked += delegate
			{
				this.options.StartAtZero = !this.options.StartAtZero;
				this.OptionsChanged ();
			};

			this.hasThresholdButton0.Clicked += delegate
			{
				this.options.HasThreshold0 = !this.options.HasThreshold0;
				this.OptionsChanged ();
			};

			this.hasThresholdButton1.Clicked += delegate
			{
				this.options.HasThreshold1 = !this.options.HasThreshold1;
				this.OptionsChanged ();
			};

			this.thresholdValueField0.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					var value = Converters.ParsePercent (this.thresholdValueField0.Text);
					if (value.HasValue)
					{
						this.options.ThresholdValue0 = value.Value;
						this.OptionsChanged ();
					}
				}
			};

			this.thresholdValueField1.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					var value = Converters.ParsePercent (this.thresholdValueField1.Text);
					if (value.HasValue)
					{
						this.options.ThresholdValue1 = value.Value;
						this.OptionsChanged ();
					}
				}
			};

			this.thresholdAddButton0.Clicked += delegate
			{
				this.options.ThresholdValue0 = System.Math.Min (this.options.ThresholdValue0+0.01m, 1.0m);
				this.OptionsChanged ();
			};

			this.thresholdAddButton1.Clicked += delegate
			{
				this.options.ThresholdValue1 = System.Math.Min (this.options.ThresholdValue1+0.01m, 1.0m);
				this.OptionsChanged ();
			};

			this.thresholdSubButton0.Clicked += delegate
			{
				this.options.ThresholdValue0 = System.Math.Max (this.options.ThresholdValue0-0.01m, 0.0m);
				this.OptionsChanged ();
			};

			this.thresholdSubButton1.Clicked += delegate
			{
				this.options.ThresholdValue1 = System.Math.Max (this.options.ThresholdValue1-0.01m, 0.0m);
				this.OptionsChanged ();
			};

			this.titleField.EditionAccepted += delegate
			{
				this.options.TitleText = this.titleField.FormattedText;
				this.OptionsChanged ();
			};
		}


		private void OptionsChanged()
		{
			this.UpdateWidgets ();
			this.optionsChangedAction ();
		}


		public bool Detailed
		{
			get
			{
				return this.detailedFrame.Visibility;
			}
			set
			{
				this.detailedFrame.Visibility = value;
				this.separator.Visibility = value;
			}
		}

		private void ClearAction()
		{
			this.options.Clear ();
			this.OptionsChanged ();
		}

	
		public void Update()
		{
			this.InitComboDimension (this.primaryDimensionCombo);
			this.InitComboDimension (this.secondaryDimensionCombo);
			this.InitComboPoint (this.pointCombo);
			this.InitComboStyle (this.styleCombo);
			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			using (this.ignoreChanges.Enter())
			{
				this.startAtZeroButton.Visibility           = this.options.Mode == GraphMode.SideBySide || this.options.Mode == GraphMode.Lines;
				this.linesButton.Visibility                 = this.options.Mode == GraphMode.Lines;
				this.pointLabel.Visibility                  = this.options.Mode == GraphMode.Lines;
				this.pointCombo.Visibility                  = this.options.Mode == GraphMode.Lines;
				this.legendButton.Visibility                = this.options.Mode != GraphMode.Array;
				this.borderThicknessController.Visibility   = this.options.Mode != GraphMode.Array;
				this.barThicknessController.Visibility      = this.options.Mode == GraphMode.SideBySide || this.options.Mode == GraphMode.Stacked;
				this.barOverlapController.Visibility        = this.options.Mode == GraphMode.SideBySide;
				this.lineAlphaController.Visibility         = this.options.Mode == GraphMode.Lines;
				this.lineWidthController.Visibility         = this.options.Mode == GraphMode.Lines && this.options.HasLines;
				this.pointWidthController.Visibility        = this.options.Mode == GraphMode.Lines && this.options.GraphPoints != GraphPoint.None;
				this.piePercentsButton.Visibility           = this.options.Mode == GraphMode.Pie;
				this.pieValuesButton.Visibility             = this.options.Mode == GraphMode.Pie;
				this.explodedPieFactorController.Visibility = this.options.Mode == GraphMode.Pie;

				this.sideBySideModeButton.ActiveState = (this.options.Mode == GraphMode.SideBySide) ? ActiveState.Yes : ActiveState.No;
				this.stackedModeButton.ActiveState    = (this.options.Mode == GraphMode.Stacked   ) ? ActiveState.Yes : ActiveState.No;
				this.linesModeButton.ActiveState      = (this.options.Mode == GraphMode.Lines     ) ? ActiveState.Yes : ActiveState.No;
				this.pieModeButton.ActiveState        = (this.options.Mode == GraphMode.Pie       ) ? ActiveState.Yes : ActiveState.No;
				this.arrayModeButton.ActiveState      = (this.options.Mode == GraphMode.Array     ) ? ActiveState.Yes : ActiveState.No;

				this.linesButton.ActiveState         = this.options.HasLines      ? ActiveState.Yes : ActiveState.No;
				this.piePercentsButton.ActiveState   = this.options.PiePercents   ? ActiveState.Yes : ActiveState.No;
				this.pieValuesButton.ActiveState     = this.options.PieValues     ? ActiveState.Yes : ActiveState.No;
				this.legendButton.ActiveState        = this.options.HasLegend     ? ActiveState.Yes : ActiveState.No;
				this.startAtZeroButton.ActiveState   = this.options.StartAtZero   ? ActiveState.Yes : ActiveState.No;
				this.hasThresholdButton0.ActiveState = this.options.HasThreshold0 ? ActiveState.Yes : ActiveState.No;
				this.hasThresholdButton1.ActiveState = this.options.HasThreshold1 ? ActiveState.Yes : ActiveState.No;

				this.thresholdAddButton0.Enable  = this.options.HasThreshold0;
				this.thresholdSubButton0.Enable  = this.options.HasThreshold0;
				this.thresholdValueField0.Enable = this.options.HasThreshold0;
				this.thresholdValueField0.Text = Converters.PercentToString (this.options.ThresholdValue0);

				this.thresholdAddButton1.Enable  = this.options.HasThreshold1;
				this.thresholdSubButton1.Enable  = this.options.HasThreshold1;
				this.thresholdValueField1.Enable = this.options.HasThreshold1;
				this.thresholdValueField1.Text = Converters.PercentToString (this.options.ThresholdValue1);

				this.primaryDimensionCombo.FormattedText   = this.DimensionToText (this.options.PrimaryDimension);
				this.secondaryDimensionCombo.FormattedText = this.DimensionToText (this.options.SecondaryDimension);

				this.fontSizeController.Value          = this.options.FontSize;
				this.borderThicknessController.Value   = this.options.BorderThickness;
				this.barThicknessController.Value      = this.options.BarThickness;
				this.barOverlapController.Value        = this.options.BarOverlap;
				this.lineAlphaController.Value         = this.options.LineAlpha;
				this.lineWidthController.Value         = this.options.LineWidth;
				this.pointWidthController.Value        = this.options.PointWidth;
				this.explodedPieFactorController.Value = this.options.ExplodedPieFactor;

				this.titleField.FormattedText = this.options.TitleText;

				if (this.cube.Dimensions != 0)
				{
					this.primaryFilterField.FormattedText   = GraphOptionsController.GetFilterSummary (this.options.PrimaryFilter,   this.cube.GetCount (this.options.PrimaryDimension));
					this.secondaryFilterField.FormattedText = GraphOptionsController.GetFilterSummary (this.options.SecondaryFilter, this.cube.GetCount (this.options.SecondaryDimension));
				}

				this.pointCombo.FormattedText = GraphOptionsController.PointToText (this.options.GraphPoints);
				this.styleCombo.FormattedText = GraphOptionsController.StyleToText (this.options.Style);
			}
		}

		private BackIconButton CreateButton(Widget parent, string icon, FormattedText description)
		{
			var button = new BackIconButton
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

			ToolTip.Default.SetToolTip (button, description);

			return button;
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


		private void InitComboPoint(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var point in GraphOptionsController.Points)
			{
				combo.Items.Add (GraphOptionsController.PointToText (point));
			}
		}

		private static GraphPoint TextToPoint(FormattedText text)
		{
			foreach (var point in GraphOptionsController.Points)
			{
				if (text == GraphOptionsController.PointToText (point))
				{
					return point;
				}
			}

			return GraphPoint.None;
		}

		private static FormattedText PointToText(GraphPoint point)
		{
			switch (point)
			{
				case GraphPoint.None:
					return "Aucun";

				case GraphPoint.Mix:
					return "Mélangés";

				case GraphPoint.Circle:
					return "Cercles";

				case GraphPoint.Square:
					return "Carrés";

				case GraphPoint.TriangleUp:
					return "Triangles haut";

				case GraphPoint.Diamond:
					return "Losanges";

				case GraphPoint.TriangleDown:
					return "Triangles bas";

				case GraphPoint.Cross:
					return "Croix";

				default:
					return "?";
			}
		}

		private static IEnumerable<GraphPoint> Points
		{
			get
			{
				yield return GraphPoint.None;
				yield return GraphPoint.Mix;
				yield return GraphPoint.Circle;
				yield return GraphPoint.Square;
				yield return GraphPoint.Diamond;
				yield return GraphPoint.TriangleUp;
				yield return GraphPoint.TriangleDown;
				yield return GraphPoint.Cross;
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
					this.OptionsChanged ();
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

				this.OptionsChanged ();
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

				this.OptionsChanged ();
			};

			menu.Items.Add (item);
		}


		private readonly AbstractController		controller;
		private readonly Cube					cube;
		private readonly GraphOptions			options;
		private readonly SafeCounter			ignoreChanges;

		private FrameBox						commonFrame;
		private Separator						separator;
		private FrameBox						detailedFrame;

		private System.Action					optionsChangedAction;
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
		private StaticText						pointLabel;
		private TextFieldCombo					pointCombo;
		private TextFieldCombo					styleCombo;
		private CheckButton						linesButton;
		private CheckButton						legendButton;
		private CheckButton						startAtZeroButton;
		private CheckButton						piePercentsButton;
		private CheckButton						pieValuesButton;
		private CheckButton						hasThresholdButton0;
		private TextFieldEx						thresholdValueField0;
		private GlyphButton						thresholdAddButton0;
		private GlyphButton						thresholdSubButton0;
		private CheckButton						hasThresholdButton1;
		private TextFieldEx						thresholdValueField1;
		private GlyphButton						thresholdAddButton1;
		private GlyphButton						thresholdSubButton1;
		private SliderController				fontSizeController;
		private SliderController				borderThicknessController;
		private SliderController				barThicknessController;
		private SliderController				barOverlapController;
		private SliderController				lineAlphaController;
		private SliderController				lineWidthController;
		private SliderController				pointWidthController;
		private SliderController				explodedPieFactorController;
		private TextFieldMultiEx				titleField;
	}
}
