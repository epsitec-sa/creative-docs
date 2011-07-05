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
	public class BusinessDocumentLinesController
	{
		public BusinessDocumentLinesController(DocumentMetadataEntity documentMetadataEntity, BusinessDocumentEntity businessDocumentEntity)
		{
			this.documentMetadataEntity = documentMetadataEntity;
			this.businessDocumentEntity = businessDocumentEntity;
		}


		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			{
				//	Crée la toolbar.
				double buttonSize = Library.UI.ButtonLargeWidth;

				var toolbar = UIBuilder.CreateMiniToolbar (frame, buttonSize);
				toolbar.Dock = DockStyle.Top;
				toolbar.Margins = new Margins (0, 0, 0, -1);

				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateArticle));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateText));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateTitle));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateDiscount));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateTax));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateQuantity));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateGroup));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.CreateGroupSeparator));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateSeparator ());
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Duplicate));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Delete));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateSeparator ());
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Group));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Ungroup));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateSeparator ());

				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Cancel, DockStyle.Right));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateButton (Library.Business.Res.Commands.Lines.Ok, DockStyle.Right));
				toolbar.Children.Add (BusinessDocumentLinesController.CreateSeparator (DockStyle.Right));
			}

			//	Crée la liste.
			this.articleLinesController = new ArticleLinesController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.articleLinesController.CreateUI (frame);

			this.editionArticleLineController = new EditionArticleLineController (this.documentMetadataEntity, this.businessDocumentEntity);
			this.editionArticleLineController.CreateUI (frame);
		}

		private static IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.Left, bool large = true, bool isActivable = false)
		{
			//?double buttonWidth = large ? Library.UI.ButtonLargeWidth : Library.UI.ButtonSmallWidth;
			double buttonWidth = large ? Library.UI.IconLargeWidth+4 : Library.UI.IconSmallWidth+3;
			double iconWidth   = large ? Library.UI.IconLargeWidth : Library.UI.IconSmallWidth;

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

		private static Separator CreateSeparator(DockStyle dockStyle = DockStyle.Left, double width = 10)
		{
			return new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = width,
				Dock = dockStyle,
			};
		}

		public void UpdateUI(int? sel = null)
		{
			this.articleLinesController.UpdateUI (this.GetCellContent, sel);
		}

		private FormattedText GetCellContent(int row, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var line = this.businessDocumentEntity.Lines[row];

			if (columnType == ColumnType.Quantity)
			{
				var quantity = BusinessDocumentLinesController.GetArticleQuantity (line as ArticleDocumentItemEntity);

				if (quantity != null)
				{
					return quantity.ToString ();
				}
			}

			if (columnType == ColumnType.Description)
			{
				return BusinessDocumentLinesController.GetArticleDescription (line);
			}

			if (columnType == ColumnType.Price)
			{
				var price = BusinessDocumentLinesController.GetArticlePrice (line as ArticleDocumentItemEntity);

				if (price != null)
				{
					return  Misc.PriceToString (price);
				}
			}

			return null;
		}


#if false
		[Command (Library.Business.Res.Commands.Lines.CreateArticle)]
		public void ProcessCreateArticle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}
#endif

	
		#region ArticleDocumentItemEntity extensions
		private static decimal? GetArticleQuantity(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;

				decimal quantity = 0;

				foreach (var articleQuantity in article.ArticleQuantities)
				{
					quantity += articleQuantity.Quantity;
				}

				return quantity;
			}

			return null;
		}

		private static FormattedText GetArticleDescription(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return Helpers.ArticleDocumentItemHelper.GetArticleDescription (article, replaceTags: true, shortDescription: true);
			}

			if (line is TextDocumentItemEntity)
			{
				var text = line as TextDocumentItemEntity;
				return text.Text;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;

				if (tax.Text.IsNullOrEmpty)
				{
					return "TVA";
				}
				else
				{
					return tax.Text;
				}
			}

			if (line is SubTotalDocumentItemEntity)
			{
				return "Sous-total";
			}

			if (line is EndTotalDocumentItemEntity)
			{
				return "Grand total";
			}

			return null;
		}

		private static decimal? GetArticlePrice(AbstractDocumentItemEntity line)
		{
			if (line is ArticleDocumentItemEntity)
			{
				var article = line as ArticleDocumentItemEntity;
				return article.PrimaryLinePriceBeforeTax;
			}

			if (line is TaxDocumentItemEntity)
			{
				var tax = line as TaxDocumentItemEntity;
				return tax.ResultingTax;
			}

			if (line is SubTotalDocumentItemEntity)
			{
				var total = line as SubTotalDocumentItemEntity;
				return total.FinalPriceBeforeTax;
			}

			if (line is EndTotalDocumentItemEntity)
			{
				var total = line as EndTotalDocumentItemEntity;
				return total.PriceAfterTax;
			}

			return null;
		}
		#endregion


		private static readonly double lineHeight = 17;

		private readonly DocumentMetadataEntity documentMetadataEntity;
		private readonly BusinessDocumentEntity businessDocumentEntity;

		private ArticleLinesController articleLinesController;
		private EditionArticleLineController editionArticleLineController;
	}
}
