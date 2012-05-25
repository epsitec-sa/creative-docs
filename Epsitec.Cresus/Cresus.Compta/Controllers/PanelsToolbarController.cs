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
			var temporalButton = this.CreateButton (parent, Res.Commands.Panel.Temporal, 5, UIBuilder.TemporalBackColor);
			                     this.CreateButton (parent, Res.Commands.Panel.Options, -1, UIBuilder.OptionsBackColor);
			var filterButton   = this.CreateButton (parent, Res.Commands.Panel.Filter,  -1, UIBuilder.FilterBackColor);
			var searchButton   = this.CreateButton (parent, Res.Commands.Panel.Search,   0, UIBuilder.SearchBackColor);

			this.searchMarker   = this.CreateMarker (searchButton);
			this.filterMarker   = this.CreateMarker (filterButton);
			this.temporalMarker = this.CreateMarker (temporalButton);
		}

		public bool SearchEnable
		{
			//	Indique si les recherches sont actives. Si oui, un petit 'vu' vert s'affiche en surimpression du bouton.
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
			//	Indique si le filtre est actif. Si oui, un petit 'vu' vert s'affiche en surimpression du bouton.
			get
			{
				return this.filterMarker.Visibility;
			}
			set
			{
				this.filterMarker.Visibility = value;
			}
		}

		public bool TemporalEnable
		{
			//	Indique si le filtre temporel est actif. Si oui, un petit 'vu' vert s'affiche en surimpression du bouton.
			get
			{
				return this.temporalMarker.Visibility;
			}
			set
			{
				this.temporalMarker.Visibility = value;
			}
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
				Text             = UIBuilder.GetTextIconUri("Panel.Active"),
				ContentAlignment = ContentAlignment.BottomRight,
				Anchor           = AnchorStyles.All,
				Visibility       = false,
			};
		}


		private static readonly double			toolbarHeight = 24;

		private readonly AbstractController		controller;

		private StaticText						searchMarker;
		private StaticText						filterMarker;
		private StaticText						temporalMarker;
	}
}
