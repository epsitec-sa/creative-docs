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


		public FrameBox CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			var titleFrame = new FrameBox
			{
				PreferredHeight = 28,
				Margins         = new Margins (0, 0, 0, -1),
				Parent          = frame,
				Dock            = DockStyle.Top,
				DrawFullFrame   = false,
			};

			this.titleDocumentFrame = new FrameBox
			{
				PreferredWidth  = 130,
				PreferredHeight = 28,
				Margins         = new Margins (0, 0, 0, 0),
				Parent          = titleFrame,
				Dock            = DockStyle.Left,
				DrawFullFrame   = true,
				BackColor       = Color.FromName ("White"),
			};

			this.titleLineFrame = new FrameBox
			{
				PreferredHeight = 28,
				Margins         = new Margins (-1, 0, 0, 0),
				Parent          = titleFrame,
				Dock            = DockStyle.Fill,
				DrawFullFrame   = true,
				BackColor       = Color.FromName ("White"),
			};

			this.titleDocumentText = new StaticText
			{
				Parent           = this.titleDocumentFrame,
				Dock             = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (10, 1, 0, 0),
			};

			this.titleLineText = new StaticText
			{
				Parent           = this.titleLineFrame,
				Dock             = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (10, 1, 0, 0),
			};

			this.lineEditorTile = new FrameBox
			{
				Parent        = frame,
				Dock          = DockStyle.Fill,
				DrawFullFrame = true,
			};

			return frame;
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
			this.CreateLineEditor (info);

			if (this.lineEditorController != null)
			{
				this.lineEditorController.SetInitialFocus ();
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
			if (info == null)
			{
				this.lineEditorController = new DeselectedEditorController (this.accessData);
				this.lineEditorController.CreateUI (this.lineEditorTile, null);
			}
			else
			{
				if (info.DocumentItem is ArticleDocumentItemEntity)
				{
					if (info.SublineIndex == 0 || info.IsQuantity == false)
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
				else if (info.DocumentItem is TextDocumentItemEntity)
				{
					this.lineEditorController = new TextLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
				}
				else if (info.DocumentItem is TaxDocumentItemEntity)
				{
					this.lineEditorController = new TaxLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
				}
				else if (info.DocumentItem is SubTotalDocumentItemEntity)
				{
					this.lineEditorController = new SubTotalLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
				}
				else if (info.DocumentItem is EndTotalDocumentItemEntity)
				{
					this.lineEditorController = new EndTotalLineEditorController (this.accessData);
					this.lineEditorController.CreateUI (this.lineEditorTile, info.DocumentItem);
				}
			}
		}
		
		private void UpdateLineEditorTitle()
		{
			//	Met à jour le titre de l'éditeur.
			var lineText      = FormattedText.Empty;
			var documentText  = this.accessData.DocumentMetadata.DocumentStateLongDescription.ApplyBold ();
			var lineColor     = Color.FromBrightness (1.0);  // blanc
			var documentColor = Color.FromBrightness (1.0);  // blanc

			switch (this.accessData.DocumentMetadata.DocumentState)
			{
				case DocumentState.Active:
					documentColor = Color.FromHexa ("e2eeff");  // bleu clair
					break;

				case DocumentState.Inactive:
					documentColor = Color.FromBrightness (0.8);  // gris clair
					break;
			}

			if (this.lineEditorController != null)
			{
				lineText = this.lineEditorController.TileTitle.ApplyBold ();

				if (this.error != DocumentItemAccessorError.None)  // y a-t-il une erreur ?
				{
					lineText  = FormattedText.Concat (lineText, " — ", DocumentItemAccessor.GetErrorDescription (this.error));
					lineColor = Color.FromName ("Gold");
				}

				lineText.ApplyFontSizePercent (120);
			}

			this.titleDocumentText.FormattedText = documentText.ApplyBold ();
			this.titleDocumentFrame.BackColor    = documentColor;
			this.titleLineText.FormattedText     = lineText;
			this.titleLineFrame.BackColor        = lineColor;
		}


		private readonly AccessData				accessData;

		private EditMode						editMode;
		private FrameBox						titleLineFrame;
		private StaticText						titleLineText;
		private FrameBox						titleDocumentFrame;
		private StaticText						titleDocumentText;
		private FrameBox						lineEditorTile;
		private AbstractLineEditorController	lineEditorController;
		private DocumentItemAccessorError		error;
	}
}
