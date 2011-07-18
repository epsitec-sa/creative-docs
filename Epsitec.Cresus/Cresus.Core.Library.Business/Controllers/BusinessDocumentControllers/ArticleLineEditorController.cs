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

		public EditMode CurrentEditMode
		{
			get
			{
				return this.editMode;
			}
			set
			{
				this.editMode = value;
			}
		}

		private bool IsEditName
		{
			get
			{
				return this.editMode == EditMode.Name;
			}
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var leftFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
			};

			var rightFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				PreferredWidth = 360,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
			};

			var separator = new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = 1,
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
			};

			this.CreateUILeftFrame (builder, leftFrame);
			this.CreateUIRightFrame (builder, rightFrame);
		}

		private void CreateUILeftFrame(UIBuilder builder, FrameBox parent)
		{
			int labelWidth = 80;

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
					PossibleItemsFilter = x => this.ArticleFilter (x),
				};

				var articleField = builder.CreateCompactAutoCompleteTextField (null, "", articleController);
				this.PlaceLabelAndField (line, labelWidth, 0, this.IsTax ? "Frais" : "Article", articleField.Parent);

				this.firstFocusedWidget = articleField;
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

				this.articleDescriptionTextField = builder.CreateTextFieldMulti (replacementBox, DockStyle.None, 0, Marshaler.Create (() => this.GetArticleDescription (this.IsEditName), x => this.SetArticleDescription (x, this.IsEditName)));
				this.articleDescriptionTextField.Dock = DockStyle.StackFill;
				this.articleDescriptionTextField.Margins = new Margins (0);

				this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);

				var text = this.IsEditName ? "Désignation courte" : "Désignation longue";
				this.PlaceLabelAndField (line, labelWidth, 0, text, replacementBox);

#if false
				var icon = new StaticText
				{
					Parent = line,
					Anchor = AnchorStyles.BottomLeft,
					PreferredSize = new Size (41, 41),
					Margins = new Margins (30, 0, 0, 10),
					Text = Misc.GetResourceIconImageTag(this.IsEditName ? "Lines.EditName" : "Lines.EditDescription", 0, new Size (31, 31)),
				};
#else
				double buttonWidth = Library.UI.Constants.ButtonLargeWidth;
				double iconWidth   = Library.UI.Constants.IconLargeWidth;

				new IconButton
				{
					Parent            = line,
					CommandObject     = Library.Business.Res.Commands.Lines.EditName,
					PreferredIconSize = new Size (iconWidth, iconWidth),
					PreferredSize     = new Size (buttonWidth, buttonWidth),
					Margins           = new Margins (0, 0, 0, 0),
					Anchor            = AnchorStyles.BottomLeft,
					AutoFocus         = false,
				};

				new IconButton
				{
					Parent            = line,
					CommandObject     = Library.Business.Res.Commands.Lines.EditDescription,
					PreferredIconSize = new Size (iconWidth, iconWidth),
					PreferredSize     = new Size (buttonWidth, buttonWidth),
					Margins           = new Margins (buttonWidth, 0, 0, 0),
					Anchor            = AnchorStyles.BottomLeft,
					AutoFocus         = false,
				};
#endif
			}
		}

		private void CreateUIRightFrame(UIBuilder builder, FrameBox parent)
		{
			//	Première ligne à droite.
			if (this.Quantity != null)
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 20),
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

			//	Deuxième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.NextTabIndex,
				};

				//	Prix unitaire.
				var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.PrimaryUnitPriceBeforeTax, x => this.Entity.PrimaryUnitPriceBeforeTax = x));
				this.PlaceLabelAndField (line, 130, 100, "Prix unitaire HT", quantityField);
			}

			//	Troisième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.NextTabIndex,
				};

				//	Rabais.
				var discountField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.DiscountValue, x => this.DiscountValue = x));
				this.PlaceLabelAndField (line, 130, 100, "Rabais en % ou en francs", discountField);
			}

			//	Quatrième ligne à droite.
#if false  // on ne peut pas saisir la description du rabais, car on ne sait pas où la faire figurer !
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.NextTabIndex,
				};

				//	Rabais.
				var discountField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.DiscountText, x => this.DiscountText = x));
				this.PlaceLabelAndField (line, 130, 200, "Description du rabais", discountField);
			}
#endif
		}


		public override FormattedText TitleTile
		{
			get
			{
				return this.IsTax ? "Frais" : "Article";
			}
		}


		private void ResetArticleDefinition(ArticleDefinitionEntity article)
		{
			if (this.Entity.ArticleDefinition.RefEquals (article))
			{
				return;
			}

			var item = this.Entity;

			item.ArticleAttributes |= ArticleDocumentItemAttributes.DirtyArticlePrices | ArticleDocumentItemAttributes.DirtyArticleNotDiscountable;
			item.ArticleDefinition = article;
			item.ArticleParameters = null;
			item.VatCode = article.GetOutputVatCode ();
			item.TaxRate1 = null;
			item.TaxRate2 = null;
			item.FixedPrice = null;
			item.ResultingLinePriceBeforeTax = null;
			item.ResultingLineTax1 = null;
			item.ResultingLineTax2 = null;
			item.FinalLinePriceBeforeTax = null;
			item.ArticleNameCache = null;
			item.ArticleDescriptionCache = null;
			item.ReplacementName = null;
			item.ReplacementDescription = null;

			//	Initialise la description de l'article.
			this.SetArticleDescription (this.GetArticleDescription (true), true);
			this.SetArticleDescription (this.GetArticleDescription (false), false);

			//	Initialise le prix de base de l'article.
			if (article.ArticlePrices.Count != 0)
			{
				item.PrimaryUnitPriceBeforeTax = article.ArticlePrices[0].Value;
			}

			//	Initialise l'unité par défaut.
			UnitOfMeasureEntity unit = null;

			if (unit == null && article.Units != null && article.Units.Units.Count != 0)
			{
				unit = article.Units.Units[0];
			}

			if (unit == null && article.BillingUnit != null)
			{
				unit = article.BillingUnit;
			}

			if (unit != null && item.ArticleQuantities.Count != 0)
			{
				item.ArticleQuantities[0].Unit = unit;
			}

			this.parameterController.UpdateUI (this.Entity);
			this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);
		}

		private FormattedText GetArticleDescription(bool shortDescription)
		{
			return ArticleDocumentItemHelper.GetArticleText (this.Entity, shortDescription: shortDescription);
		}

		private void SetArticleDescription(FormattedText value, bool shortDescription)
		{
			//	The replacement text of the article item might be defined in several different
			//	languages; compare and replace only the text for the active language :
			var replacementText = shortDescription ? this.Entity.ReplacementName : this.Entity.ReplacementDescription;

			string articleText = value.IsNull ? null : TextFormatter.ConvertToText (value);
			string defaultText = TextFormatter.ConvertToText (shortDescription ? this.Entity.ArticleDefinition.Name : this.Entity.ArticleDefinition.Description);
			string currentReplacement = replacementText.IsNull ? null : TextFormatter.ConvertToText (replacementText);

			if (articleText == defaultText)  // texte standard ?
			{
				articleText = null;
			}

			if (currentReplacement != articleText)
			{
				MultilingualText text = new MultilingualText (replacementText);
				text.SetText (TextFormatter.CurrentLanguageId, articleText);

				if (shortDescription)
				{
					this.Entity.ReplacementName = text.GetGlobalText ();
				}
				else
				{
					this.Entity.ReplacementDescription = text.GetGlobalText ();
				}
			}

			//	Met à jour le texte du cache.
			if (shortDescription)
			{
				this.Entity.ArticleNameCache = ArticleDocumentItemHelper.GetArticleText (this.Entity, replaceTags: true, shortDescription: true);
			}
			else
			{
				this.Entity.ArticleDescriptionCache = ArticleDocumentItemHelper.GetArticleText (this.Entity, replaceTags: true, shortDescription: false);
			}
		}

		private bool ArticleFilter(ArticleDefinitionEntity article)
		{
			if (this.IsTax)
			{
				return ArticleDocumentItemHelper.IsFixedTax (article);
			}
			else
			{
				return true;
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

		private string DiscountValue
		{
			get
			{
				if (this.Entity.Discounts.Count != 0)
				{
					var discount = this.Entity.Discounts[0];

					if (discount.DiscountRate.HasValue && discount.DiscountRate.Value != 0)
					{
						return Misc.PercentToString (discount.DiscountRate);
					}

					if (discount.Value.HasValue && discount.Value.Value != 0)
					{
						return Misc.PriceToString (discount.Value);
					}
				}

				return null;
			}
			set
			{
				this.CreateDefaultDiscount ();
				var discount = this.Entity.Discounts[0];

				if (string.IsNullOrEmpty (value))
				{
					discount.DiscountRate = null;
					discount.Value = null;
				}
				else
				{
					if (value.Contains ("%"))
					{
						value = value.Replace ("%", "");

						decimal d;
						if (decimal.TryParse (value, out d))
						{
							discount.DiscountRate = d/100;
							discount.Value = null;
						}
					}
					else
					{
						decimal d;
						if (decimal.TryParse (value, out d))
						{
							discount.DiscountRate = null;
							discount.Value = d;
						}
					}
				}
			}
		}

		private FormattedText DiscountText
		{
			get
			{
				if (this.Entity.Discounts.Count != 0)
				{
					var discount = this.Entity.Discounts[0];

					return discount.Text;
				}

				return null;
			}
			set
			{
				this.CreateDefaultDiscount ();
				var discount = this.Entity.Discounts[0];

				discount.Text = value;
			}
		}

		private void CreateDefaultDiscount()
		{
			//	S'il n'existe aucun rabais, crée les entités requises.
			if (this.Entity.Discounts.Count == 0)
			{
				var newDiscount = this.accessData.BusinessContext.CreateEntity<PriceDiscountEntity> ();
				this.Entity.Discounts.Add (newDiscount);

				// TODO: faut-il créer PriceRoundingModeEntity, et si oui comment ?
			}
		}


		private bool IsTax
		{
			get
			{
				return this.Entity.GroupIndex == 0;
			}
		}


		private ArticleDocumentItemEntity Entity
		{
			get
			{
				return this.entity as ArticleDocumentItemEntity;
			}
		}


		private EditMode														editMode;
		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
		private ArticleParameterControllers.ArticleParameterToolbarController	toolbarController;
		private TextFieldMultiEx												articleDescriptionTextField;
	}
}
