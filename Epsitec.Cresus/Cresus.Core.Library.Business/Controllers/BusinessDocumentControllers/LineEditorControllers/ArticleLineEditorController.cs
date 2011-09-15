//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

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
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public sealed class ArticleLineEditorController : AbstractLineEditorController
	{
		public ArticleLineEditorController(AccessData accessData, EditMode editMode)
			: base (accessData)
		{
			this.editMode = editMode;
		}

		
		public override FormattedText			TileTitle
		{
			get
			{
				return this.IsTax ? "Frais" : "Article";
			}
		}

		private bool							IsEditName
		{
			get
			{
				return this.editMode == EditMode.Name;
			}
		}

		private ArticleQuantityEntity			Quantity
		{
			get
			{
				if (this.Item.ArticleQuantities.Count == 0)
				{
					return null;
				}
				else
				{
					return this.Item.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == this.accessData.DocumentLogic.MainArticleQuantityType).FirstOrDefault ();
				}
			}
		}

		private string							DiscountLineValue
		{
			get
			{
				return this.GetDiscountValue (DiscountPolicy.OnLinePrice);
			}
			set
			{
				this.SetDiscountValue (value, DiscountPolicy.OnLinePrice);
			}
		}

		private FormattedText					DiscountLineText
		{
			get
			{
				return this.GetDiscountText (DiscountPolicy.OnLinePrice);
			}
			set
			{
				this.SetDiscountText (value, DiscountPolicy.OnLinePrice);
			}
		}

		private string							DiscountUnitValue
		{
			get
			{
				return this.GetDiscountValue (DiscountPolicy.OnUnitPrice);
			}
			set
			{
				this.SetDiscountValue (value, DiscountPolicy.OnUnitPrice);
			}
		}

		private FormattedText					DiscountUnitText
		{
			get
			{
				return this.GetDiscountText (DiscountPolicy.OnUnitPrice);
			}
			set
			{
				this.SetDiscountText (value, DiscountPolicy.OnUnitPrice);
			}
		}

		private bool							IsTax
		{
			get
			{
				return this.Item.GroupIndex == 0;
			}
		}

		private ArticleDocumentItemEntity		Item
		{
			get
			{
				return this.entity as ArticleDocumentItemEntity;
			}
		}



		private FormattedText GetDiscountText(DiscountPolicy policy)
		{
			var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

			if (discount.IsNotNull ())
			{
				return discount.Text;
			}

			return null;
		}

		private void SetDiscountText(FormattedText value, DiscountPolicy policy)
		{
			using (this.accessData.BusinessContext.SuspendUpdates ())
			{
				if (value.IsNullOrWhiteSpace)
				{
					var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

					if (discount.IsNotNull ())
					{
						discount.Text = FormattedText.Empty;
						this.RemoveEmptyDiscounts ();
					}
				}
				else
				{
					this.CreateDefaultDiscount (policy);

					var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

					if (discount.IsNotNull ())
					{
						discount.Text = value;
					}
				}
			}
		}

		
		private string GetDiscountValue(DiscountPolicy policy)
		{
			var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

			if (discount.IsNotNull ())
			{
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

		private void SetDiscountValue(string value, DiscountPolicy policy)
		{
			using (this.accessData.BusinessContext.SuspendUpdates ())
			{
				if (string.IsNullOrWhiteSpace (value))
				{
					var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

					if (discount.IsNotNull ())
					{
						discount.DiscountRate = null;
						discount.Value = null;
						this.RemoveEmptyDiscounts ();
					}
				}
				else
				{
					this.CreateDefaultDiscount (policy);

					var discount = this.Item.Discounts.FirstOrDefault (x => x.DiscountPolicy == policy);

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
								discount.DiscountRate = PriceCalculator.ClipPercentValue (d/100);
								discount.Value = null;
							}
						}
						else
						{
							decimal d;
							if (decimal.TryParse (value, out d))
							{
								discount.DiscountRate = null;
								discount.Value = PriceCalculator.ClipPriceValue (d);
							}
						}
					}
				}
			}
		}

		
		private void RemoveEmptyDiscounts()
		{
			for (int i = 0; i < this.Item.Discounts.Count; )
			{
				var discount = this.Item.Discounts[i];

				if ((discount.Text.IsNullOrEmpty) &&
					(discount.Value == null) &&
					(discount.DiscountRate == null))
				{
					this.Item.Discounts.RemoveAt (i);
					continue;
				}

				i++;
			}
		}
		

		protected override void CreateUI(UIBuilder builder)
		{
			var leftFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.GetNextTabIndex (),
			};

			var rightFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				PreferredWidth = 360,
				TabIndex = this.GetNextTabIndex (),
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
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsLinesEditionEnabled,
				};

				var articleController = new SelectionController<ArticleDefinitionEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Item.ArticleDefinition,
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
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsArticleParametersEditionEnabled,
				};

				this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.tileContainer, line);
				this.parameterController.CallbackParameterChanged = this.ParameterChanged;
				var box = this.parameterController.CreateUI (line, labelWidth: labelWidth, labelToRight: true);
				box.Margins = new Margins (0);
				box.TabIndex = this.GetNextTabIndex ();

				this.parameterController.UpdateUI (this.Item);
			}

			//	Texte de remplacement.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Fill,
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsArticleParametersEditionEnabled,
				};

				var replacementBox = new FrameBox ();

				this.toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				var toolbar = this.toolbarController.CreateUI (replacementBox, null);
				toolbar.Margins = new Margins (0, 0, 0, -1);

				this.articleDescriptionTextField = builder.CreateTextFieldMulti (replacementBox, DockStyle.None, 0, Marshaler.Create (() => this.GetArticleDescription (this.IsEditName), x => this.SetArticleDescription (x, this.IsEditName)));
				this.articleDescriptionTextField.Dock = DockStyle.StackFill;
				this.articleDescriptionTextField.Margins = new Margins (0);

				this.toolbarController.UpdateUI (this.Item, this.articleDescriptionTextField);

				var text = this.IsEditName ? "Désignation courte" : "Désignation longue";
				this.PlaceLabelAndField (line, labelWidth, 0, text, replacementBox);
			}

			//	Boutons pour choisir le mode Name/Description.
			{
				double buttonWidth = Library.UI.Constants.ButtonLargeWidth;
				double iconWidth   = Library.UI.Constants.IconLargeWidth;

				new IconButton
				{
					Parent            = parent,
					CommandObject     = Library.Business.Res.Commands.Lines.EditName,
					PreferredIconSize = new Size (iconWidth, iconWidth),
					PreferredSize     = new Size (buttonWidth, buttonWidth),
					Margins           = new Margins (0, 0, 0, 0),
					Anchor            = AnchorStyles.BottomLeft,
					AutoFocus         = false,
				};

				new IconButton
				{
					Parent            = parent,
					CommandObject     = Library.Business.Res.Commands.Lines.EditDescription,
					PreferredIconSize = new Size (iconWidth, iconWidth),
					PreferredSize     = new Size (buttonWidth, buttonWidth),
					Margins           = new Margins (buttonWidth, 0, 0, 0),
					Anchor            = AnchorStyles.BottomLeft,
					AutoFocus         = false,
				};
			}
		}

		private void CreateUIRightFrame(UIBuilder builder, FrameBox parent)
		{
			var topFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				PreferredHeight = 10+20+10,
				Padding = new Margins (0, 0, 10, 10),
				TabIndex = this.GetNextTabIndex (),
				Enable = this.accessData.DocumentLogic.MainArticleQuantityType != ArticleQuantityType.None,
			};

			var separator = new Separator
			{
				IsHorizontalLine = true,
				PreferredHeight = 1,
				Parent = parent,
				Dock = DockStyle.Top,
			};

			var bottomFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				PreferredWidth = 360,
				Padding = new Margins (0, 0, 10, 10),
				TabIndex = this.GetNextTabIndex (),
			};

			this.CreateUIRightTopFrame (builder, topFrame);
			this.CreateUIRightBottomFrame (builder, bottomFrame);
		}

		private void CreateUIRightTopFrame(UIBuilder builder, FrameBox parent)
		{
			if (this.Quantity != null)
			{
				//	Quantité.
				var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Quantity.Quantity, x => this.Quantity.Quantity = x));
				this.PlaceLabelAndField (parent, 50, 80, "Quantité", quantityField);

				//	Unité.
				var unitController = new SelectionController<UnitOfMeasureEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Quantity.Unit,
					ValueSetter         = x => this.Quantity.Unit = x,
					ReferenceController = new ReferenceController (() => this.Quantity.Unit),
				};

				var unitField = builder.CreateCompactAutoCompleteTextField (null, "", unitController);
				this.PlaceLabelAndField (parent, 35, 80, "Unité", unitField.Parent);

				if (this.accessData.DocumentLogic.MainArticleQuantityType != ArticleQuantityType.None)
				{
					var quantityEntity = this.accessData.DocumentLogic.GetArticleQuantityColumnEntity (this.accessData.DocumentLogic.MainArticleQuantityType);

					var text = FormattedText.Concat ("   ", quantityEntity.Name);
					this.CreateStaticText (parent, 70, text);
				}
			}
		}

		private void CreateUIRightBottomFrame(UIBuilder builder, FrameBox parent)
		{
			//	Deuxième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsPriceEditionEnabled,
				};
			}

			//	Deuxième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsPriceEditionEnabled,
				};

				if (this.billingMode == Business.Finance.BillingMode.ExcludingTax)
				{
					var unit1Field = builder.CreateTextField (Marshaler.Create (() => this.Item.UnitPriceBeforeTax1, x => this.Item.UnitPriceBeforeTax1 = x));
					var discount   = builder.CreateTextField (Marshaler.Create (() => this.DiscountUnitValue, x => this.DiscountUnitValue = x));
					var unit2Field = builder.CreateTextField (Marshaler.Create (() => this.Item.UnitPriceBeforeTax2, x => this.Item.UnitPriceBeforeTax2 = x));

					this.BindPriceField (unit1Field, ArticleDocumentItemAttributes.FixedUnitPrice1);
					this.BindPriceField (unit2Field, ArticleDocumentItemAttributes.FixedUnitPrice2);

					this.PlacePriceEditionWidgets (line, "Prix unitaire", unit1Field, discount, unit2Field);
				}
				else
				{
					var unit1Field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Item.UnitPriceAfterTax1, x => this.Item.UnitPriceAfterTax1 = x));
					var unit1Box = this.PlaceLabelAndField (line, 130, 100, "Prix u. cat.", unit1Field);
					var unit2Field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Item.UnitPriceAfterTax2, x => this.Item.UnitPriceAfterTax2 = x));
					var unit2Box = this.PlaceLabelAndField (line, 130, 100, "Prix unitaire", unit2Field);
				}
			}

			//	Troisième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsPriceEditionEnabled,
				};

				if (this.billingMode == Business.Finance.BillingMode.ExcludingTax)
				{
					var line1Field = builder.CreateTextField (Marshaler.Create (() => this.Item.LinePriceBeforeTax1, x => this.Item.LinePriceBeforeTax1 = x));
					var discount   = builder.CreateTextField (Marshaler.Create (() => this.DiscountLineValue, x => this.DiscountLineValue = x));
					var line2Field = builder.CreateTextField (Marshaler.Create (() => this.Item.LinePriceBeforeTax2, x => this.Item.LinePriceBeforeTax2 = x));

					this.PlacePriceEditionWidgets (line, "Prix ligne", line1Field, discount, line2Field);
				}
				else
				{
					var unit1Field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Item.UnitPriceAfterTax1, x => this.Item.UnitPriceAfterTax1 = x));
					var unit1Box = this.PlaceLabelAndField (line, 130, 100, "Prix u. cat.", unit1Field);
					var unit2Field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Item.UnitPriceAfterTax2, x => this.Item.UnitPriceAfterTax2 = x));
					var unit2Box = this.PlaceLabelAndField (line, 130, 100, "Prix unitaire", unit2Field);
				}
			}

			//	Troisième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.GetNextTabIndex (),
					Enable = this.accessData.DocumentLogic.IsDiscountEditionEnabled,
				};

				var neverButton = new CheckButton
				{
					Parent = line,
					Text = "Jamais de rabais",
					ActiveState = this.Item.NeverApplyDiscount ? ActiveState.Yes : ActiveState.No,
					Dock = DockStyle.Fill,
					Margins = new Margins (10, 0, 0, 0),
				};

				neverButton.ActiveStateChanged += delegate
				{
					this.Item.NeverApplyDiscount = (neverButton.ActiveState == ActiveState.Yes);
					this.UpdateDiscountBox ();
				};

				this.discountBox = line;
			}

			//	Quatrième ligne à droite.
			{
				var line = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Top,
					PreferredHeight = 20,
					Margins = new Margins (0, 0, 0, 5),
					TabIndex = this.GetNextTabIndex (),
				};

				//	Rabais.
				var discountField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.DiscountLineText, x => this.DiscountLineText = x));
				this.PlaceLabelAndField (line, 130, 200, "Description du rabais", discountField);
			}

			this.UpdateDiscountBox ();
		}

		private void BindPriceField(TextFieldEx field, ArticleDocumentItemAttributes attribute)
		{
			field.EditionAccepted +=
				delegate
				{
					if (field.Text.IsNullOrWhiteSpace ())
					{
						this.Item.ArticleAttributes &= ~attribute;
					}
					else
					{
						this.Item.ArticleAttributes |= attribute;
					}
				};

			if (Item.ArticleAttributes.HasFlag (attribute))
			{
				field.TextDisplayMode = TextFieldDisplayMode.OverriddenValue;
			}
		}


		private void PlacePriceEditionWidgets(Widget container, FormattedText caption, AbstractTextField field1, AbstractTextField fieldDiscount, AbstractTextField field2)
		{
			container.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			new StaticText ()
			{
				Parent = container,
				Dock = DockStyle.Stacked,
				PreferredWidth = 80,
				FormattedText = caption,
				ContentAlignment = ContentAlignment.TopRight,
				Margins = new Margins (0, 5, 2, 0),
			};

			field1.Dock = DockStyle.Stacked;
			field1.PreferredWidth = 80;
			field1.Parent = container;
			field1.Margins = Margins.Zero;

			new StaticText ()
			{
				Parent = container,
				Dock = DockStyle.Stacked,
				PreferredWidth = 20,
				FormattedText = "−",
				ContentAlignment = ContentAlignment.TopCenter,
				Margins = new Margins (0, 0, 2, 0),
			};

			fieldDiscount.Dock = DockStyle.Stacked;
			fieldDiscount.PreferredWidth = 70;
			fieldDiscount.Parent = container;
			fieldDiscount.Margins = Margins.Zero;

			new StaticText ()
			{
				Parent = container,
				Dock = DockStyle.Stacked,
				PreferredWidth = 20,
				FormattedText = "→",
				ContentAlignment = ContentAlignment.TopCenter,
				Margins = new Margins (0, 0, 2, 0),
			};

			field2.Dock = DockStyle.Stacked;
			field2.PreferredWidth = 80;
			field2.Parent = container;
			field2.Margins = new Margins (0, 5, 0, 0);
		}


		private void UpdateDiscountBox()
		{
			this.discountBox.Enable = !this.Item.NeverApplyDiscount;
		}



		private void ResetArticleDefinition(ArticleDefinitionEntity article)
		{
			if (this.Item.ArticleDefinition.RefEquals (article))
			{
				return;
			}

			var businessContext = this.accessData.BusinessContext;
			var item = this.Item;

			item.ArticleAttributes           = ArticleDocumentItemAttributes.Dirty;
			item.ArticleDefinition           = article;
			item.ArticleParameters           = null;
			item.ArticleAccountingDefinition = null;
			item.VatRateA                    = 0;
			item.VatRateB                    = 0;
			item.VatRatio                    = 1;
			item.UnitPriceBeforeTax1         = null;
			item.UnitPriceBeforeTax2         = null;
			item.UnitPriceAfterTax1          = null;
			item.UnitPriceAfterTax2          = null;
			item.LinePriceBeforeTax1         = null;
			item.LinePriceBeforeTax2         = null;
			item.LinePriceAfterTax1          = null;
			item.LinePriceAfterTax2          = null;
			item.TotalRevenueAfterTax        = null;
			item.TotalRevenueAccounted       = null;
			item.ArticleNameCache            = null;
			item.ArticleDescriptionCache     = null;
			item.ReplacementName             = null;
			item.ReplacementDescription      = null;

			businessContext.ClearAndDeleteEntities (item.ArticleTraceabilityDetails);
			businessContext.ClearAndDeleteEntities (item.Discounts);
			
			//	Initialise la description de l'article.
			this.SetArticleDescription (this.GetArticleDescription (true), true);
			this.SetArticleDescription (this.GetArticleDescription (false), false);

			//	Initialise le prix de base de l'article.
#if false
			//	@PA: reset du prix unitaire et du mode TTC/HT unitaire

			if (article.ArticlePrices.Any ())
			{
				var articlePrice = article.ArticlePrices.First ();
				if (articlePrice.ValueIncludesTaxes)
				{
					item.UnitPriceAfterTax1 = articlePrice.Value;
				}
				else
				{
					item.UnitPriceBeforeTax1 = articlePrice.Value;
				}
			}
#endif

			//	Initialise l'unité par défaut.
			UnitOfMeasureEntity unit = null;

			if (unit == null && article.Units != null && article.Units.Units.Count != 0)
			{
				unit = article.Units.Units[0];
			}

			if (unit != null && item.ArticleQuantities.Count != 0)
			{
				item.ArticleQuantities[0].Unit = unit;
			}

			this.parameterController.UpdateUI (this.Item);
			this.toolbarController.UpdateUI (this.Item, this.articleDescriptionTextField);
		}

		private FormattedText GetArticleDescription(bool shortDescription)
		{
			return ArticleDocumentItemHelper.GetArticleText (this.Item, shortDescription: shortDescription);
		}

		private void SetArticleDescription(FormattedText value, bool shortDescription)
		{
			//	The replacement text of the article item might be defined in several different
			//	languages; compare and replace only the text for the active language :
			var replacementText = shortDescription ? this.Item.ReplacementName : this.Item.ReplacementDescription;

			string articleText = value.IsNull ? null : TextFormatter.ConvertToText (value);
			string defaultText = TextFormatter.ConvertToText (shortDescription ? this.Item.ArticleDefinition.Name : this.Item.ArticleDefinition.Description);
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
					this.Item.ReplacementName = text.GetGlobalText ();
				}
				else
				{
					this.Item.ReplacementDescription = text.GetGlobalText ();
				}
			}

			//	Met à jour le texte du cache.
			if (shortDescription)
			{
				this.Item.ArticleNameCache = ArticleDocumentItemHelper.GetArticleText (this.Item, replaceTags: true, shortDescription: true);
			}
			else
			{
				this.Item.ArticleDescriptionCache = ArticleDocumentItemHelper.GetArticleText (this.Item, replaceTags: true, shortDescription: false);
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
			ArticleParameterControllers.ArticleParameterToolbarController.UpdateTextFieldParameter (this.Item, this.articleDescriptionTextField);
		}

	
		private void CreateDefaultDiscount(DiscountPolicy policy)
		{
			//	S'il n'existe aucun rabais, crée les entités requises.
			if (this.Item.Discounts.Any (x => x.DiscountPolicy == policy))
			{
				return;
			}
			
			var newDiscount = this.accessData.BusinessContext.CreateEntity<PriceDiscountEntity> ();

			newDiscount.DiscountPolicy = policy;

			this.Item.Discounts.Add (newDiscount);
		}



		private readonly EditMode												editMode;
		private ArticleParameterControllers.ValuesArticleParameterController	parameterController;
		private ArticleParameterControllers.ArticleParameterToolbarController	toolbarController;
		private TextFieldMultiEx												articleDescriptionTextField;
		private FrameBox														discountBox;
	}
}
