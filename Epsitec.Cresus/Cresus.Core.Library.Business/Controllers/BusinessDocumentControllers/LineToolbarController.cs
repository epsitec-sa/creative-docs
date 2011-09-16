﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	/// <summary>
	/// Barre d'icône permettant de gérer une liste de lignes d'articles (AbstractDocumentItemEntity).
	/// </summary>
	public sealed class LineToolbarController
	{
		public LineToolbarController(CoreData coreData, DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.coreData               = coreData;
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;

			this.userManager = coreData.GetComponent<UserManager> ();
		}


		public Widget CreateUI(Widget parent)
		{
			var buttonSize  = Library.UI.Constants.ButtonSmallWidth;
			var isDeveloper = this.userManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.Developer);

			var toolbar = UIBuilder.CreateMiniToolbar (parent, buttonSize);
			
			toolbar.Dock    = DockStyle.Top;
			toolbar.Margins = new Margins (0, 0, 0, -1);

			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Deselect,        large: false, isActivable: false));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.GroupSelect,     large: false, isActivable: false));

			toolbar.Children.Add (this.CreateSeparator ());
			
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewCompact,     large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewDefault,     large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewFull,        large: false, isActivable: true));
			
			if (isDeveloper)
			{
				toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.ViewDebug,   large: false, isActivable: true));
			}
			
			toolbar.Children.Add (this.CreateSeparator ());
			
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.EditName,        large: false, isActivable: true));
			toolbar.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.EditDescription, large: false, isActivable: true));

			return toolbar;
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
				Dock           = dockStyle,
			};
		}


		private readonly CoreData				coreData;
		private readonly DocumentMetadataEntity	documentMetadataEntity;
		private readonly BusinessDocumentEntity	businessDocumentEntity;
		private readonly UserManager			userManager;
	}
}
