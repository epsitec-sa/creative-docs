//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le ruban supérieur de la comptabilité.
	/// </summary>
	public class RibbonController
	{
		public RibbonController(Application app)
		{
			this.app = app;

			this.sectionGroupFrames = new List<FrameBox> ();
			this.sectionIconFrames  = new List<FrameBox> ();
			this.sectionTitleFrames = new List<StaticText> ();
			this.sectionTitles      = new List<FormattedText> ();

			this.ribbonViewMode = RibbonViewMode.Default;
		}


		public void CreateUI(Widget parent)
		{
			//	Construit le faux ruban.
			this.container = new GradientFrameBox
			{
				Parent              = parent,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				BackColor1          = RibbonController.GetBackgroundColor1 (),
				BackColor2          = RibbonController.GetBackgroundColor2 (),
				IsVerticalGradient  = true,
				BottomPercentOffset = 1.0 - 0.15,  // ombre dans les 15% supérieurs
				Dock                = DockStyle.Top,
				Margins             = new Margins (-1, -1, 0, 0),
			};

			var separator = new Separator
			{
				Parent           = parent,
				PreferredHeight  = 1,
				IsHorizontalLine = true,
				Dock             = DockStyle.Bottom,
			};

			this.sectionGroupFrames.Clear ();
			this.sectionIconFrames.Clear ();
			this.sectionTitleFrames.Clear ();
			this.sectionTitles.Clear ();

			//	|-->
			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Comptabilité");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Open, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Login, RibbonController.ButtonLargeWidth, RibbonController.IconSmallWidth, isActivable: true);

				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.Save, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);

				UIBuilder.CreateButton (section, Res.Commands.Présentation.Print, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth, isActivable: true);
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Présentation");

				UIBuilder.CreateButton (section, Res.Commands.Présentation.Journal, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth, isActivable: true);
				UIBuilder.CreateButton (section, Res.Commands.Présentation.Extrait, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth, isActivable: true);
				//?section.Children.Add (this.CreateGap ());

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Balance, RibbonController.ButtonLargeWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Bilan, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.PP, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Exploitation, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);

				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.Budgets, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.Change, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.RésuméPériodique, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.RésuméTVA, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.DécompteTVA, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);

				//?section.Children.Add (this.CreateGap ();
				this.présentationMenuButton = UIBuilder.CreateButton (section, Res.Commands.Présentation.New, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Réglages");

				UIBuilder.CreateButton (section, Res.Commands.Présentation.PlanComptable, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth, isActivable: true);

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Libellés, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Modèles, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Journaux, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (topSection, Res.Commands.Présentation.Périodes, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);

				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.PiècesGenerator, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.CodesTVA, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.Utilisateurs, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Présentation.Réglages, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth, isActivable: true);
			}

			{
				Widget topSection, bottomSection;
				var section = this.CreateSection (this.container, DockStyle.Left, "Repère");

				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Select.Up, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Select.Down, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Select.Home, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Edition");

				UIBuilder.CreateButton (section, Res.Commands.Edit.Create, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);
				UIBuilder.CreateButton (section, Res.Commands.Edit.Accept, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);
				UIBuilder.CreateButton (section, Res.Commands.Edit.Cancel, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);
				//?section.Children.Add (this.CreateGap ();

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Edit.Duplicate, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (topSection, Res.Commands.Edit.Delete, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (topSection, Res.Commands.Edit.Up, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);

				UIBuilder.CreateButton (bottomSection, Res.Commands.Edit.Undo, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Edit.Redo, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Edit.Down, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Ecriture");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				UIBuilder.CreateButton (topSection, Res.Commands.Multi.Insert, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (topSection, Res.Commands.Multi.Up, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (topSection, Res.Commands.Multi.Swap, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);

				UIBuilder.CreateButton (bottomSection, Res.Commands.Multi.Delete, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Multi.Down, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
				UIBuilder.CreateButton (bottomSection, Res.Commands.Multi.Auto, RibbonController.ButtonSmallWidth, RibbonController.IconSmallWidth);
			}

			//	<--|
			{
				var section = this.CreateSection (this.container, DockStyle.Right, "Navigation");

				UIBuilder.CreateButton (section, Res.Commands.Navigator.Prev, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);
				UIBuilder.CreateButton (section, Res.Commands.Navigator.Next, RibbonController.ButtonLargeWidth, RibbonController.IconLargeWidth);

				this.navigatorMenuButton = new GlyphButton
				{
					Parent          = section,
					CommandObject   = Res.Commands.Navigator.Menu,
					GlyphShape      = GlyphShape.Menu,
					ButtonStyle     = ButtonStyle.ToolItem,
					PreferredHeight = 18,
					Anchor          = AnchorStyles.Bottom | AnchorStyles.LeftAndRight,
					Margins         = new Margins (33, 33, 0, 0),  // pour avoir une taille de 18x18
				};
			}

			this.UpdateRibbon ();


			//	Bouton 'v'
			var showRibbonButton = new GlyphButton
			{
				Parent        = container.Window.Root,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (14, 14),
				Margins       = new Margins (0, -1, -1, 0),
				GlyphShape    = GlyphShape.Menu,
				ButtonStyle   = ButtonStyle.Icon,
			};

			ToolTip.Default.SetToolTip (showRibbonButton, "Mode d'affichage de la barre d'icônes");

			showRibbonButton.Clicked += delegate
			{
				this.ShowRibbonModeMenu (showRibbonButton);
			};
		}

		public GlyphButton NavigatorMenuButton
		{
			get
			{
				return this.navigatorMenuButton;
			}
		}

		public IconButton PrésentationMenuButton
		{
			get
			{
				return this.présentationMenuButton;
			}
		}


		#region Ribbon mode menu
		private void ShowRibbonModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode pour le ruban.
			var menu = new VMenu ();

			this.AddRibbonModeToMenu (menu, "Pas de barre d'icônes",           RibbonViewMode.Hide);
			menu.Items.Add (new MenuSeparator ());
			this.AddRibbonModeToMenu (menu, "Barre d'icônes minimaliste",      RibbonViewMode.Minimal);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes compacte",         RibbonViewMode.Compact);
			menu.Items.Add (new MenuSeparator ());
			this.AddRibbonModeToMenu (menu, "Barre d'icônes standard",         RibbonViewMode.Default);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes aérée",            RibbonViewMode.Large);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes pour grand écran", RibbonViewMode.Hires);

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = this.container;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddRibbonModeToMenu(VMenu menu, FormattedText text, RibbonViewMode mode)
		{
			bool selected = (this.ribbonViewMode == mode);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetRadioStateIconUri (selected),
				FormattedText = text,
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				this.ribbonViewMode = (RibbonViewMode) System.Enum.Parse (typeof (RibbonViewMode), item.Name);
				this.UpdateRibbon ();
			};

			menu.Items.Add (item);
		}
		#endregion


		private void UpdateRibbon()
		{
			//	Met à jour le faux ruban en fonction du RibbonViewMode en cours.
			var mode = this.ribbonViewMode;

			if (mode == RibbonViewMode.Hide)
			{
				this.container.Visibility = false;
			}
			else
			{
				this.container.Visibility = true;

				double  frameGap    = 0;
				Margins iconMargins = Margins.Zero;
				double  buttonWidth = 0;
				double  gapWidth    = 0;
				double  titleHeight = 0;
				double  titleSize   = 0;

				switch (mode)
				{
					case RibbonViewMode.Minimal:
						frameGap    = -1;  // les sections se chevauchent
						iconMargins = new Margins (0);
						buttonWidth = RibbonController.ButtonLargeWidth;
						gapWidth    = 3;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Compact:
						frameGap    = -1;  // les sections se chevauchent
						iconMargins = new Margins (3);
						buttonWidth = RibbonController.ButtonLargeWidth;
						gapWidth    = 5;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Default:
						frameGap    = 2;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = RibbonController.ButtonLargeWidth;
						gapWidth    = 6;
						titleHeight = 11;
						titleSize   = 8;
						break;

					case RibbonViewMode.Large:
						frameGap    = 3;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = RibbonController.ButtonLargeWidth+2;
						gapWidth    = 8;
						titleHeight = 14;
						titleSize   = 10;
						break;

					case RibbonViewMode.Hires:
						frameGap    = 4;
						iconMargins = new Margins (5, 5, 5, 5-1);
						buttonWidth = RibbonController.ButtonLargeWidth+6;
						gapWidth    = 10;
						titleHeight = 18;
						titleSize   = 12;
						break;
				}

				for (int i = 0; i < this.sectionGroupFrames.Count; i++)
				{
					//	Met à jour le panneau du groupe de la section.
					{
						var groupFrame = this.sectionGroupFrames[i];

						double leftMargin  = (groupFrame.Dock == DockStyle.Right) ? frameGap : 0;
						double rightMargin = (groupFrame.Dock == DockStyle.Left) ? frameGap : 0;

						groupFrame.Margins = new Margins (leftMargin, rightMargin, -1, -1);
					}

					//	Met à jour le panneau des icônes de la section.
					{
						var iconFrame = this.sectionIconFrames[i];

						iconFrame.Padding = iconMargins;

						foreach (var gap in iconFrame.FindAllChildren ().Where (x => x.Name == "Gap"))
						{
							gap.PreferredWidth = gapWidth;
						}

						foreach (var widget in iconFrame.FindAllChildren ())
						{
							if (widget is IconButton || widget is RibbonIconButton)
							{
								var button = widget as IconButton;

								if (button.PreferredIconSize.Width == RibbonController.IconSmallWidth)
								{
									button.PreferredSize = new Size (buttonWidth/2, buttonWidth/2);
								}
								else
								{
									button.PreferredSize = new Size (buttonWidth, buttonWidth);
								}
							}
						}
					}

					//	Met à jour le titre de la section.
					{
						var titleFrame = this.sectionTitleFrames[i];
						var title = this.sectionTitles[i].ApplyFontSize (titleSize).ApplyFontColor (Color.FromBrightness (1.0)).ApplyBold ();

						titleFrame.Visibility      = (mode != RibbonViewMode.Minimal && mode != RibbonViewMode.Compact);
						titleFrame.FormattedText   = title;
						titleFrame.PreferredHeight = titleHeight;
					}
				}
			}
		}

	
		private Widget CreateSection(Widget frame, DockStyle dockStyle, FormattedText description)
		{
			//	Crée une section dans le faux ruban.
			var groupFrame = new FrameBox
			{
				Parent              = frame,
				DrawFullFrame       = true,
				BackColor           = RibbonController.GetSectionBackgroundColor (),
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 10,
				Dock                = dockStyle,
			};

			var iconFrame = new FrameBox
			{
				Parent              = groupFrame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Fill,
			};

			var titleFrame = new StaticText
			{
				Parent           = groupFrame,
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				BackColor        = RibbonController.GetTitleBackgroundColor (),
				PreferredWidth   = 10,
				Dock             = DockStyle.Bottom,
				Margins          = new Margins (1, 1, 0, 1),
			};

			this.sectionGroupFrames.Add (groupFrame);
			this.sectionIconFrames.Add (iconFrame);
			this.sectionTitleFrames.Add (titleFrame);
			this.sectionTitles.Add (description);

			return iconFrame;
		}

		private void CreateSubsections(Widget section, out Widget topSection, out Widget bottomSection)
		{
			//	Crée deux sous-sections dans le faux ruban.
			var frame = new FrameBox
			{
				Parent              = section,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.StackBegin,
			};

			topSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Top,
			};

			bottomSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Bottom,
			};
		}

		private Widget CreateGap()
		{
			var gap = new FrameBox
			{
				Name = "Gap",
				Dock = DockStyle.StackBegin,
			};

			return gap;
		}


		#region Color manager
		private static Color GetBackgroundColor1()
		{
			//	Couleur pour l'ombre en haut des zones libres du ruban.
			return RibbonController.GetColor (RibbonController.GetBaseColor (), saturation: 0.06, value: 0.9);
		}

		private static Color GetBackgroundColor2()
		{
			//	Couleur pour le fond des zones libres du ruban.
			return RibbonController.GetColor (RibbonController.GetBaseColor (), saturation: 0.06, value: 0.7);
		}

		private static Color GetSectionBackgroundColor()
		{
			//	Couleur pour le fond d'une section du ruban.
			return RibbonController.GetColor (RibbonController.GetBaseColor (), saturation: 0.02, value: 0.95);
		}

		private static Color GetTitleBackgroundColor()
		{
			//	Couleur pour le fond du titre d'une section du ruban.
			return RibbonController.GetColor (RibbonController.GetBaseColor (), saturation: 0.2, value: 0.7);
		}

		private static Color GetBaseColor()
		{
			//	Couleur de base pour le ruban, dont on utilise la teinte (hue).
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			return adorner.ColorBorder;
		}

		private static Color GetColor(Color color, double? saturation = null, double? value = null)
		{
			//	Retourne une couleur en forçant éventuellement la saturation et la valeur.
			double h, s, v;
			Color.ConvertRgbToHsv (color.R, color.G, color.B, out h, out s, out v);

			if (saturation.HasValue)
			{
				s = saturation.Value;
			}

			if (value.HasValue)
			{
				v = value.Value;
			}

			double r, g, b;
			Color.ConvertHsvToRgb (h, s, v, out r, out g, out b);

			return new Color (r, g, b);
		}
		#endregion




		private enum RibbonViewMode
		{
			Hide,		// pas de ruban
			Minimal,	// sans titres et très compact
			Compact,	// sans titres et compact
			Default,	// mode standard
			Large,		// aéré
			Hires,		// pour grand écran
		}

		
		private const double ButtonLargeWidth	= 2 * ((RibbonController.IconLargeWidth + 1) / 2 + 5);
		private const double ButtonSmallWidth	= 2 * ((RibbonController.IconSmallWidth + 1) / 2 + 5);

		private const int IconSmallWidth		= 20;
		private const int IconLargeWidth		= 32;

		private readonly Application				app;
		private readonly List<FrameBox>				sectionGroupFrames;
		private readonly List<FrameBox>				sectionIconFrames;
		private readonly List<StaticText>			sectionTitleFrames;
		private readonly List<FormattedText>		sectionTitles;

		private Widget								container;
		private RibbonViewMode						ribbonViewMode;
		private IconButton							présentationMenuButton;
		private GlyphButton							navigatorMenuButton;
	}
}
