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
	public class EditionArticleLineController
	{
		public EditionArticleLineController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public void CreateUI(Widget parent)
		{
			var tile = new FrameBox
			{
				PreferredHeight = 50,
				Margins = new Margins (0, 0, 10, 0),
				Padding = new Margins (5),
				Parent = parent,
				Dock = DockStyle.Bottom,
				DrawFullFrame = true,
			};

			var line1 = new FrameBox
			{
				Parent = tile,
				Dock = DockStyle.Top,
			};

			var line2 = new FrameBox
			{
				Parent = tile,
				Margins = new Margins (0, 0, 5, 0),
				Dock = DockStyle.Top,
			};

			new StaticText
			{
				Text = "Article",
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 50,
				Margins = new Margins (0, 5, 0, 0),
				Parent = line1,
				Dock = DockStyle.Left,
			};

			new TextFieldCombo
			{
				Parent = line1,
				PreferredWidth = 100,
				Dock = DockStyle.Left,
			};

			new StaticText
			{
				Text = "Prix unitaire",
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 100,
				Margins = new Margins (0, 5, 0, 0),
				Parent = line1,
				Dock = DockStyle.Left,
			};

			new TextField
			{
				Parent = line1,
				PreferredWidth = 70,
				Dock = DockStyle.Left,
			};

			new StaticText
			{
				Text = "Quantité",
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 50,
				Margins = new Margins (0, 5, 0, 0),
				Parent = line2,
				Dock = DockStyle.Left,
			};

			new TextField
			{
				Parent = line2,
				PreferredWidth = 50,
				Dock = DockStyle.Left,
			};

			new StaticText
			{
				Text = "Unité",
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 60,
				Margins = new Margins (0, 5, 0, 0),
				Parent = line2,
				Dock = DockStyle.Left,
			};

			new TextFieldCombo
			{
				Parent = line2,
				PreferredWidth = 60,
				Dock = DockStyle.Left,
			};
		}

	
		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;
	}
}
