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
			var line1 = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
			};

			//	Article.
			{
				var articleController = new SelectionController<ArticleDefinitionEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Entity.ArticleDefinition,
					ValueSetter         = x => this.Entity.ArticleDefinition = x,
					ReferenceController = new ReferenceController (() => this.Entity.ArticleDefinition),
				};

				var articleField = builder.CreateCompactAutoCompleteTextField (null, "", articleController);
				this.PlaceLabelAndField (line1, 70, 400, "Article", articleField.Parent);
			}

			if (this.Quantity != null)
			{
				//	Quantité.
				var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Quantity.Quantity, x => this.Quantity.Quantity = x));
				this.PlaceLabelAndField (line1, 50, 80, "Quantité", quantityField);

				//	Unité.
				var unitController = new SelectionController<UnitOfMeasureEntity> (this.accessData.BusinessContext)
				{
					ValueGetter         = () => this.Quantity.Unit,
					ValueSetter         = x => this.Quantity.Unit = x,
					ReferenceController = new ReferenceController (() => this.Quantity.Unit),
				};

				var unitField = builder.CreateCompactAutoCompleteTextField (null, "", unitController);
				this.PlaceLabelAndField (line1, 25, 80, "Unité", unitField.Parent);
			}

			//	Choix des paramètres.
			var line2 = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 259, 0, 0),  // TODO: dépend de la largeur totale (800) et de la largeur des widgets éditables (400) !
			};

			{
				this.parameterController = new ArticleParameterControllers.ValuesArticleParameterController (this.tileContainer, null);
				this.parameterController.CallbackParameterChanged = this.ParameterChanged;
				this.parameterController.CreateUI (line2);
				this.parameterController.UpdateUI (this.Entity);
			}

			//	Texte de remplacement.
			var line3 = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Top,
				PreferredHeight = 80,
			};

			{
				var replacementBox = new FrameBox ();

				this.toolbarController = new ArticleParameterControllers.ArticleParameterToolbarController (this.tileContainer);
				this.toolbarController.CreateUI (replacementBox, null);

				this.articleDescriptionTextField = builder.CreateTextFieldMulti (replacementBox, DockStyle.None, 0, Marshaler.Create (() => this.GetArticleDescription (), this.SetArticleDescription));
				this.articleDescriptionTextField.Dock = DockStyle.StackFill;

				this.toolbarController.UpdateUI (this.Entity, this.articleDescriptionTextField);

				this.PlaceLabelAndField (line3, 70, 400, "Désignation", replacementBox);
			}
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Article";
			}
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
