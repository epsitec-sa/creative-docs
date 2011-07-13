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
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class ArticleLineEditorController : AbstractLineEditorController
	{
		public ArticleLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var leftFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				TabIndex = this.NextTabIndex,
			};

			var rightFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				PreferredWidth = 350,
				TabIndex = this.NextTabIndex,
			};

			var separator = new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = 1,
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 10, 0, 0),
			};

			this.CreateUILeftFrame (builder, leftFrame);
			this.CreateUIRightFrame (builder, rightFrame);
		}

		private void CreateUILeftFrame(UIBuilder builder, FrameBox parent)
		{
			int labelWidth = 75;

			//	Article.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 10),
					TabIndex = this.NextTabIndex,
				};

				var articleController = new SelectionController<ArticleDefinitionEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Entity.ArticleDefinition,
					ValueSetter         = x => this.ResetArticleDefinition (x),
					ReferenceController = new ReferenceController (() => this.Entity.ArticleDefinition),
				};

				var articleField = builder.CreateCompactAutoCompleteTextField (null, "", articleController);
				this.PlaceLabelAndField (line, labelWidth, 0, "Article", articleField.Parent);
			}

			//	Choix des paramètres.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.NextTabIndex,
				};

				this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.tileContainer, line);
				this.parameterController.CallbackParameterChanged = this.ParameterChanged;
				var box = this.parameterController.CreateUI (line, labelWidth: labelWidth, labelToRight: true);
				box.Margins = new Margins (0);
				box.TabIndex = this.NextTabIndex;

				this.parameterController.UpdateUI (this.Entity);
			}

			//	Texte de remplacement.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Fill,
					TabIndex = this.NextTabIndex,
				};

				var replacementBox = new FrameBox ();

				this.toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				var toolbar = this.toolbarController.CreateUI (replacementBox, null);
				toolbar.Margins = new Margins (0, 0, 0, -1);

				this.articleDescriptionTextField = builder.CreateTextFieldMulti (replacementBox, DockStyle.None, 0, Marshaler.Create (() => this.GetArticleDescription (), this.SetArticleDescription));
				this.articleDescriptionTextField.Dock = DockStyle.StackFill;
				this.articleDescriptionTextField.Margins = new Margins (0);

				this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);

				this.PlaceLabelAndField (line, labelWidth, 0, "Désignation", replacementBox);
			}
		}

		private void CreateUIRightFrame(UIBuilder builder, FrameBox parent)
		{
			if (this.Quantity != null)
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.NextTabIndex,
				};

				//	Quantité.
				var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Quantity.Quantity, x => this.Quantity.Quantity = x));
				this.PlaceLabelAndField (line, 50, 80, "Quantité", quantityField);

				//	Unité.
				var unitController = new SelectionController<UnitOfMeasureEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Quantity.Unit,
					ValueSetter         = x => this.Quantity.Unit = x,
					ReferenceController = new ReferenceController (() => this.Quantity.Unit),
				};

				var unitField = builder.CreateCompactAutoCompleteTextField (null, "", unitController);
				this.PlaceLabelAndField (line, 35, 80, "Unité", unitField.Parent);

				this.CreateStaticText (line, 70, "   (commandé)");
			}
		}


		public override FormattedText TitleTile
		{
			get
			{
				return "Article";
			}
		}


		private void ResetArticleDefinition(ArticleDefinitionEntity value)
		{
			if (this.Entity.ArticleDefinition.RefEquals (value))
			{
				return;
			}

			var item = this.Entity;

			item.ArticleDefinition = value;
			item.ArticleParameters = null;
			item.VatCode = value.GetOutputVatCode ();
			item.NeverApplyDiscount = false;
			item.TaxRate1 = null;
			item.TaxRate2 = null;
			item.FixedLinePrice = null;
			item.FixedLinePriceIncludesTaxes = false;
			item.ResultingLinePriceBeforeTax = null;
			item.ResultingLineTax1 = null;
			item.ResultingLineTax2 = null;
			item.FinalLinePriceBeforeTax = null;
			item.ArticleShortDescriptionCache = null;
			item.ArticleLongDescriptionCache = null;
			item.ReplacementText = null;

			this.SetArticleDescription (this.GetArticleDescription ());

			this.parameterController.UpdateUI (this.Entity);
			this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);
		}

		private FormattedText GetArticleDescription()
		{
			return ArticleDocumentItemHelper.GetArticleDescription (this.Entity);
		}

		private void SetArticleDescription(FormattedText value)
		{
			//	The replacement text of the article item might be defined in several different
			//	languages; compare and replace only the text for the active language :

			string articleDescription = value.IsNull ? null : TextFormatter.ConvertToText (value);
			string defaultDescription = TextFormatter.ConvertToText (this.Entity.ArticleDefinition.Description);
			string currentReplacement = this.Entity.ReplacementText.IsNull ? null : TextFormatter.ConvertToText (this.Entity.ReplacementText);

			if (articleDescription == defaultDescription)  // description standard ?
			{
				articleDescription = null;
			}

			if (currentReplacement != articleDescription)
			{
				MultilingualText text = new MultilingualText (this.Entity.ReplacementText);
				text.SetText (TextFormatter.CurrentLanguageId, articleDescription);
				this.Entity.ReplacementText = text.GetGlobalText ();
			}
		}


		private void ParameterChanged(AbstractArticleParameterDefinitionEntity parameterDefinitionEntity)
		{
			//	Cette méthode est appelée lorsqu'un paramètre a été changé.
			ArticleParameterControllers.ArticleParameterToolbarController.UpdateTextFieldParameter (this.Entity, this.articleDescriptionTextField);
		}

	
		private ArticleQuantityEntity Quantity
		{
			get
			{
				if (this.Entity.ArticleQuantities.Count == 0)
				{
					return null;
				}
				else
				{
					return this.Entity.ArticleQuantities[0];
				}
			}
		}

		private ArticleDocumentItemEntity Entity
		{
			get
			{
				return this.entity as ArticleDocumentItemEntity;
			}
		}


		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
		private ArticleParameterControllers.ArticleParameterToolbarController	toolbarController;
		private TextFieldMultiEx												articleDescriptionTextField;
	}
}
