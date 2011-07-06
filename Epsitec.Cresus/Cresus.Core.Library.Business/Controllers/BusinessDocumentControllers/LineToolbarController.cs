//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// Barre d'icône permettant de gérer une liste de lignes d'articles (AbstractDocumentItemEntity).
	/// </summary>
	public class LineToolbarController
	{
		public LineToolbarController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public void CreateUI(Widget parent, System.Action<string> action)
		{
			this.action = action;

			double buttonSize = Library.UI.ButtonLargeWidth;

			var toolbar = UIBuilder.CreateMiniToolbar (parent, buttonSize);
			toolbar.Dock = DockStyle.Top;
			toolbar.Margins = new Margins (0, 0, 0, -1);

			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateArticle));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateText));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTitle));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateDiscount));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTax));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateQuantity));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroup));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroupSeparator));
			toolbar.Children.Add (this.CreateSeparator ());
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Duplicate));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Delete));
			toolbar.Children.Add (this.CreateSeparator ());
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Group));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ungroup));
			toolbar.Children.Add (this.CreateSeparator ());

			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Cancel, DockStyle.Right));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ok, DockStyle.Right));
			toolbar.Children.Add (this.CreateSeparator (DockStyle.Right));
		}

		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.Left, bool large = true, bool isActivable = false)
		{
			//?double buttonWidth = large ? Library.UI.ButtonLargeWidth : Library.UI.ButtonSmallWidth;
			double buttonWidth = large ? Library.UI.IconLargeWidth+2 : Library.UI.IconSmallWidth+2;
			double iconWidth   = large ? Library.UI.IconLargeWidth : Library.UI.IconSmallWidth;

			IconButton button;

			if (isActivable)
			{
				button = new IconButton
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
				button = new RibbonIconButton
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

			//	Câblage très provisoire des commandes !
			button.Clicked += delegate
			{
				this.action (button.Name);
			};

			return button;
		}

		private Separator CreateSeparator(DockStyle dockStyle = DockStyle.Left, double width = 10)
		{
			return new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = width,
				Dock = dockStyle,
			};
		}

	
		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;

		private System.Action<string> action;
	}
}
