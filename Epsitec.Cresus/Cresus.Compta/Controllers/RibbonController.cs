//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Compta
{
	/// <summary>
	/// Ce contrôleur gère le ruban supérieur de la comptabilité.
	/// </summary>
	public class RibbonController
	{
		public RibbonController(CoreApp app)
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

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Edition");

				section.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Edit.Accept));
				section.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Edit.Cancel));
				section.Children.Add (this.CreateGap ());

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Edit.Duplicate, large: false));
				bottomSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Edit.Delete, large: false));
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Ecriture multiple");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Insert, large: false));
				topSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Up, large: false));
				topSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Swap, large: false));

				bottomSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Delete, large: false));
				bottomSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Down, large: false));
				bottomSection.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Multi.Auto, large: false));
			}

			{
				var section = this.CreateSection (this.container, DockStyle.Left, "Présentation");

				foreach (var command in this.PrésentationCommands)
				{
					section.Children.Add (this.CreateButton (command));
				}

				section.Children.Add (this.CreateButton (Cresus.Compta.Res.Commands.Présentation.New));
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

		private IEnumerable<Command> PrésentationCommands
		{
			get
			{
				yield return Cresus.Compta.Res.Commands.Présentation.Journal;
				yield return Cresus.Compta.Res.Commands.Présentation.PlanComptable;
				yield return Cresus.Compta.Res.Commands.Présentation.Balance;
				yield return Cresus.Compta.Res.Commands.Présentation.Extrait;
				yield return Cresus.Compta.Res.Commands.Présentation.Bilan;
				yield return Cresus.Compta.Res.Commands.Présentation.PP;
				yield return Cresus.Compta.Res.Commands.Présentation.Exploitation;
				yield return Cresus.Compta.Res.Commands.Présentation.Budgets;
				yield return Cresus.Compta.Res.Commands.Présentation.Change;
				yield return Cresus.Compta.Res.Commands.Présentation.RésuméPériodique;
				yield return Cresus.Compta.Res.Commands.Présentation.RésuméTVA;
				yield return Cresus.Compta.Res.Commands.Présentation.DécompteTVA;
			}
		}

		public void PrésentationCommandsUpdate(Command c)
		{
			foreach (var command in this.PrésentationCommands)
			{
				CommandState cs = this.app.CommandContext.GetCommandState (command);
				cs.ActiveState = (command == c) ? ActiveState.Yes : ActiveState.No;
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
				IconUri       = Misc.GetResourceIconUri(selected ? "Button.RadioYes" : "Button.RadioNo"),
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
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 3;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Compact:
						frameGap    = -1;  // les sections se chevauchent
						iconMargins = new Margins (3);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 5;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Default:
						frameGap    = 2;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 6;
						titleHeight = 11;
						titleSize   = 8;
						break;

					case RibbonViewMode.Large:
						frameGap    = 3;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth+2;
						gapWidth    = 8;
						titleHeight = 14;
						titleSize   = 10;
						break;

					case RibbonViewMode.Hires:
						frameGap    = 4;
						iconMargins = new Margins (5, 5, 5, 5-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth+6;
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

								if (button.PreferredIconSize.Width == Library.UI.Constants.IconSmallWidth)
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

		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, bool large = true, bool isActivable = false)
		{
			if (command != null && handler != null)
			{
				this.app.CommandDispatcher.Register (command, handler);
			}

			double buttonWidth = large ? Library.UI.Constants.ButtonLargeWidth : Library.UI.Constants.ButtonLargeWidth/2;
			double iconWidth   = large ? Library.UI.Constants.IconLargeWidth   : Library.UI.Constants.IconSmallWidth;

			if (isActivable)
			{
				return new IconButton
				{
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = dockStyle,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
			else
			{
				return new RibbonIconButton
				{
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = dockStyle,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
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


		private readonly static double IconSize = 40;

		private readonly CoreApp					app;
		private readonly List<FrameBox>				sectionGroupFrames;
		private readonly List<FrameBox>				sectionIconFrames;
		private readonly List<StaticText>			sectionTitleFrames;
		private readonly List<FormattedText>		sectionTitles;

		private Widget								container;
		private RibbonViewMode						ribbonViewMode;
	}
}
