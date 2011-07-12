//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class LineEditorController
	{
		/// <summary>
		/// Editeur permettant de créer/modifier une ligne d'article (AbstractDocumentItemEntity).
		/// </summary>
		public LineEditorController(AccessData accessData)
		{
			this.accessData = accessData;
		}


		public void CreateUI(Widget parent)
		{
			this.editorTile = new FrameBox
			{
				PreferredHeight = 100,
				Margins = new Margins (0, 0, 0, 0),
				Padding = new Margins (5),
				Parent = parent,
				Dock = DockStyle.Bottom,
				DrawFullFrame = true,
			};

			var titleFrame = new FrameBox
			{
				PreferredHeight = 28,
				Margins = new Margins (0, 0, 10, -1),
				Parent = parent,
				Dock = DockStyle.Bottom,
				DrawFullFrame = true,
				BackColor = Color.FromName ("White"),
			};

			this.titleText = new StaticText
			{
				Parent = titleFrame,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Margins = new Margins (10, 10, 0, 0),
			};
		}


		public void UpdateUI(LineInformations info)
		{
			//	Met à jour l'éditeur en fonction de la sélection en cours.
			this.lineEditorController = null;
			this.editorTile.Children.Clear ();

			if (info != null)
			{
				if (info.AbstractDocumentItemEntity is ArticleDocumentItemEntity)
				{
					if (info.SublineIndex == 0)
					{
						this.lineEditorController = new ArticleLineEditorController (this.accessData);
						this.lineEditorController.CreateUI (this.editorTile, info.AbstractDocumentItemEntity);
					}
					else
					{
						this.lineEditorController = new QuantityLineEditorController (this.accessData);
						this.lineEditorController.CreateUI (this.editorTile, info.ArticleQuantityEntity);
					}
				}

				if (info.AbstractDocumentItemEntity is TextDocumentItemEntity)
				{
					this.lineEditorController = new TextLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.editorTile, info.AbstractDocumentItemEntity);
				}

				if (info.AbstractDocumentItemEntity is TaxDocumentItemEntity)
				{
					this.lineEditorController = new TaxLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.editorTile, info.AbstractDocumentItemEntity);
				}

				if (info.AbstractDocumentItemEntity is SubTotalDocumentItemEntity)
				{
					this.lineEditorController = new SubTotalLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.editorTile, info.AbstractDocumentItemEntity);
				}

				if (info.AbstractDocumentItemEntity is EndTotalDocumentItemEntity)
				{
					this.lineEditorController = new EndTotalLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.editorTile, info.AbstractDocumentItemEntity);
				}
			}

			this.UpdateTitle ();
		}

		private void UpdateTitle()
		{
			//	Met à jour le ttre de l'éditeur.
			FormattedText text = "";

			if (this.lineEditorController != null)
			{
				text = this.lineEditorController.TitleTile;
			}

			this.titleText.FormattedText = text.ApplyBold ().ApplyFontSizePercent (120);
		}


		private readonly AccessData						accessData;

		private StaticText								titleText;
		private FrameBox								editorTile;
		private AbstractLineEditorController			lineEditorController;
	}
}
