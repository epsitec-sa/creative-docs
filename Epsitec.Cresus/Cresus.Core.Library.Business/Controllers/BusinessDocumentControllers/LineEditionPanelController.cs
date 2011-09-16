//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public sealed class LineEditionPanelController
	{
		/// <summary>
		/// Contrôleur gérant un panneau permattant de créer/modifier une ligne d'article
		/// (AbstractDocumentItemEntity).
		/// </summary>
		public LineEditionPanelController(AccessData accessData)
		{
			this.accessData = accessData;
		}


		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = 200,  // hauteur par défaut pour tous les éditeurs de lignes
			};

			this.titleFrame = new FrameBox
			{
				PreferredHeight = 28,
				Margins         = new Margins (0, 0, 0, -1),
				Parent          = frame,
				Dock            = DockStyle.Top,
				DrawFullFrame   = true,
				BackColor       = Color.FromName ("White"),
			};

			this.titleText = new StaticText
			{
				Parent           = this.titleFrame,
				Dock             = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Margins          = new Margins (10, 10, 0, 0),
			};

			this.lineEditorTile = new FrameBox
			{
				Parent        = frame,
				Dock          = DockStyle.Fill,
				DrawFullFrame = true,
			};
		}


		public void UpdateUI()
		{
			if (this.lineEditorController != null)
			{
				this.lineEditorController.UpdateUI ();
			}
		}

		public void UpdateUI(EditMode editMode, Line info)
		{
			//	Met à jour l'éditeur en fonction de la sélection en cours.
			this.editMode = editMode;

			this.ClearLineEditor ();

			if (info != null)
			{
				this.CreateLineEditor (info);

				if (this.lineEditorController != null)
				{
					this.lineEditorController.SetInitialFocus ();
				}
			}

			this.UpdateLineEditorTitle ();
		}

		public void SetError(DocumentItemAccessorError error)
		{
			this.error = error;
			this.UpdateLineEditorTitle ();
		}

		private void ClearLineEditor()
		{
			this.lineEditorController = null;
			this.lineEditorTile.Children.Clear ();
		}
		
		private void CreateLineEditor(Line info)
		{
			if (info.DocumentItem is ArticleDocumentItemEntity)
			{
				if ((info.SublineIndex == 0) ||
					(info.IsQuantity == false))
				{
					this.lineEditorController = new ArticleLineEditorController (this.accessData, this.editMode);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
				}
				else
				{
					this.lineEditorController = new QuantityLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.ArticleQuantity);
				}
			}

			if (info.DocumentItem is TextDocumentItemEntity)
			{
				this.lineEditorController = new TextLineEditorController (this.accessData);
				this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
			}

			if (info.DocumentItem is TaxDocumentItemEntity)
			{
				this.lineEditorController = new TaxLineEditorController (this.accessData);
				this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
			}

			if (info.DocumentItem is SubTotalDocumentItemEntity)
			{
				this.lineEditorController = new SubTotalLineEditorController (this.accessData);
				this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
			}

			if (info.DocumentItem is EndTotalDocumentItemEntity)
			{
				this.lineEditorController = new EndTotalLineEditorController (this.accessData);
				this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
			}
		}
		
		private void UpdateLineEditorTitle()
		{
			//	Met à jour le titre de l'éditeur.
			
			var text  = FormattedText.Empty;
			var color = this.GetLineEditorTitleColor ();

			if (this.lineEditorController != null)
			{
				text = this.lineEditorController.TileTitle.ApplyBold ();

				if (this.error != DocumentItemAccessorError.None)
				{
					text  = FormattedText.Concat (text, " — ", DocumentItemAccessor.GetErrorDescription (this.error));
					color = Color.FromName ("Gold");
				}

				text.ApplyFontSizePercent (120);
			}

			this.titleText.FormattedText = text;
			this.titleFrame.BackColor    = color;
		}


		private Color GetLineEditorTitleColor()
		{
			return (this.accessData.DocumentMetadata.DocumentState == DocumentState.Inactive) ? Color.FromBrightness (0.8) : Color.FromBrightness (1.0);
		}
		
		private readonly AccessData				accessData;

		private EditMode						editMode;
		private FrameBox						titleFrame;
		private StaticText						titleText;
		private FrameBox						lineEditorTile;
		private AbstractLineEditorController	lineEditorController;
		private DocumentItemAccessorError		error;
	}
}
