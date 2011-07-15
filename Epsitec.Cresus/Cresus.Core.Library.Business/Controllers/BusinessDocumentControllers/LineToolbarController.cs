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


		public void CreateUI(Widget parent)
		{
			double buttonSize = Library.UI.Constants.ButtonSmallWidth;

			var toolbar = UIBuilder.CreateMiniToolbar (parent, buttonSize);
			toolbar.Dock = DockStyle.Top;
			toolbar.Margins = new Margins (0, 0, 0, -1);

			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewCompact,  large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewDefault,  large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewFull,     large: false, isActivable: true));
			toolbar.Children.Add (this.CreateSeparator ());
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.EditInternal, large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.EditPublic,   large: false, isActivable: true));
		}

		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.Left, bool large = true, bool isActivable = false)
		{
			double buttonWidth = large ? Library.UI.Constants.ButtonLargeWidth : Library.UI.Constants.ButtonSmallWidth;
			double iconWidth   = large ? Library.UI.Constants.IconLargeWidth : Library.UI.Constants.IconSmallWidth;

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

		private FrameBox CreateSeparator(DockStyle dockStyle = DockStyle.Left, double width = 10)
		{
			return new FrameBox
			{
				PreferredWidth = width,
				Dock = dockStyle,
			};
		}

	
		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;
	}
}
