//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil pour choisir les panneaux visibles.
	/// </summary>
	public class PanelsToolbarController
	{
		public PanelsToolbarController(AbstractController controller)
		{
			this.controller = controller;
		}


		public void CreateUI(FrameBox parent)
		{
#if true
			this.optionsButton = this.CreateButton (parent, Res.Commands.Panel.Options, -1, UIBuilder.OptionsBackColor);
			this.filterButton  = this.CreateButton (parent, Res.Commands.Panel.Filter,  -1, UIBuilder.FilterBackColor);
			this.searchButton  = this.CreateButton (parent, Res.Commands.Panel.Search,   0, UIBuilder.SearchBackColor);
#else
			this.optionsButton = this.CreateButton (parent, Res.Commands.Panel.Options, -1, UIBuilder.PanelButtonSoftHiliteColor);
			this.filterButton  = this.CreateButton (parent, Res.Commands.Panel.Filter,  -1, UIBuilder.PanelButtonSoftHiliteColor);
			this.searchButton  = this.CreateButton (parent, Res.Commands.Panel.Search,   0, UIBuilder.PanelButtonSoftHiliteColor);
#endif

			this.optionsMarker = this.CreateMarker (this.optionsButton);
			this.filterMarker  = this.CreateMarker (this.filterButton);
			this.searchMarker  = this.CreateMarker (this.searchButton);

			this.optionsButton.Entered += delegate
			{
				this.controller.LinkHiliteOptionsPanel (true);
			};

			this.optionsButton.Exited += delegate
			{
				this.controller.LinkHiliteOptionsPanel (false);
			};

			this.filterButton.Entered += delegate
			{
				this.controller.LinkHiliteFilterPanel (true);
			};

			this.filterButton.Exited += delegate
			{
				this.controller.LinkHiliteFilterPanel (false);
			};

			this.searchButton.Entered += delegate
			{
				this.controller.LinkHiliteSearchPanel (true);
			};

			this.searchButton.Exited += delegate
			{
				this.controller.LinkHiliteSearchPanel (false);
			};
		}

		public bool SearchEnable
		{
			//	Indique si les recherches sont actives. Si oui, un petit (!) rouge  s'affiche en surimpression sur le bouton.
			get
			{
				return this.searchMarker.Visibility;
			}
			set
			{
				this.searchMarker.Visibility = value;
			}
		}

		public bool FilterEnable
		{
			//	Indique si le filtre est actif. Si oui, un petit filtre (!) s'affiche en surimpression sur le bouton.
			get
			{
				return this.filterMarker.Visibility;
			}
			set
			{
				this.filterMarker.Visibility = value;
			}
		}

		public bool OptionsEnable
		{
			//	Indique si les options sont actives. Si oui, un petit (!) rouge s'affiche en surimpression sur le bouton.
			get
			{
				return this.optionsMarker.Visibility;
			}
			set
			{
				this.optionsMarker.Visibility = value;
			}
		}


		public void LinkHiliteOptionsButton(bool hilite)
		{
			//?this.optionsButton.BackColor = hilite ? UIBuilder.PanelButtonLinkHiliteColor : UIBuilder.PanelButtonSoftHiliteColor;
		}

		public void LinkHiliteFilterButton(bool hilite)
		{
			//?this.filterButton.BackColor = hilite ? UIBuilder.PanelButtonLinkHiliteColor : UIBuilder.PanelButtonSoftHiliteColor;
		}

		public void LinkHiliteSearchButton(bool hilite)
		{
			//?this.searchButton.BackColor = hilite ? UIBuilder.PanelButtonLinkHiliteColor : UIBuilder.PanelButtonSoftHiliteColor;
		}

	
		private IconButton CreateButton(Widget parent, Command command, double rightMargin, Color backColor)
		{
			return new BackIconButton
			{
				Parent            = parent,
				CommandObject     = command,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (PanelsToolbarController.toolbarHeight, PanelsToolbarController.toolbarHeight),
				BackColor         = backColor,
				Dock              = DockStyle.Left,
				Name              = command.Name,
				AutoFocus         = false,
				Margins           = new Margins (0, rightMargin, 0, 0),
			};
		}

		private StaticText CreateMarker(Widget parent)
		{
			//	Crée le petit 'vu' vert en surimpression d'un bouton. Par chance, le widget StaticText ne capture
			//	pas les événements souris !
			return new StaticText
			{
				Parent           = parent,
				Text             = UIBuilder.GetIconTag ("Panel.Active"),
				ContentAlignment = ContentAlignment.BottomRight,
				Anchor           = AnchorStyles.All,
				Visibility       = false,
			};
		}


		private static readonly double			toolbarHeight = 24;

		private readonly AbstractController		controller;

		private IconButton						searchButton;
		private IconButton						filterButton;
		private IconButton						optionsButton;

		private StaticText						searchMarker;
		private StaticText						filterMarker;
		private StaticText						optionsMarker;
	}
}
