//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class LineRibbonController
	{
		public LineRibbonController(CoreData coreData, DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.coreData               = coreData;
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;

			this.userManager = coreData.GetComponent<UserManager> ();
		}


		public Widget CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateArticle));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateQuantity));
			frame.Children.Add (this.CreateGap ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateText));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTitle));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroup));
			frame.Children.Add (this.CreateGap ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateDiscount));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTax));
			frame.Children.Add (this.CreateSeparator ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.MoveUp));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.MoveDown));
			frame.Children.Add (this.CreateGap ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Duplicate));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Delete));
			frame.Children.Add (this.CreateSeparator ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Group));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ungroup));
			frame.Children.Add (this.CreateGap ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Split));
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Combine));
			frame.Children.Add (this.CreateGap ());
			frame.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Flat));

			return frame;
		}

		private IconButton CreateButton(Command command = null)
		{
			double buttonWidth = Library.UI.Constants.ButtonLargeWidth;
			double iconWidth   = Library.UI.Constants.IconLargeWidth;

			return new IconButtonWithText
			{
				CommandObject       = command,
				PreferredIconSize   = new Size (iconWidth, iconWidth),
				PreferredSize       = new Size (buttonWidth, buttonWidth+6),
				MaxAdditionnalWidth = 20,	// après CommandObject et PreferredSize !
				Dock                = DockStyle.Left,
				Name                = (command == null) ? null : command.Name,
				VerticalAlignment   = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				AutoFocus           = false,
			};
		}

		private FrameBox CreateGap()
		{
			return new FrameBox
			{
				PreferredWidth = 5,
				Dock           = DockStyle.Left,
			};
		}

		private Separator CreateSeparator()
		{
			return new Separator
			{
				PreferredWidth   = 1,
				IsHorizontalLine = false,
				Dock             = DockStyle.Left,
				Margins          = new Margins (2, 2, 0, 0),
			};
		}


		private readonly CoreData				coreData;
		private readonly DocumentMetadataEntity	documentMetadataEntity;
		private readonly BusinessDocumentEntity	businessDocumentEntity;
		private readonly UserManager			userManager;
	}
}
