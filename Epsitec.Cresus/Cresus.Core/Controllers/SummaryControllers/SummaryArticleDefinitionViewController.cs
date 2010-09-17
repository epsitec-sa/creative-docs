//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleDefinitionViewController : SummaryViewController<Entities.ArticleDefinitionEntity>
	{
		public SummaryArticleDefinitionViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleDefinition (data);
				this.CreateUIParameters        (data);
				this.CreateUIPrices            (data);
				this.CreateUIComments          (data);
			}
		}

		private void CreateUIArticleDefinition(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "ArticleDefinition",
					IconUri				= "Data.ArticleDefinition",
					Title				= TextFormatter.FormatText ("Article", "(", string.Join (", ", this.Entity.ArticleGroups.Select (g => g.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Article"),
					TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.ShortDescription)),
					CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.LongDescription)),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIParameters(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleParameterDefinition",
					IconUri		 = "Data.ArticleParameter",
					Title		 = TextFormatter.FormatText ("Paramètres"),
					CompactTitle = TextFormatter.FormatText ("Paramètres"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractArticleParameterDefinitionEntity> ("ArticleParameterDefinition", data.Controller, this.DataContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());
			template.DefineCreateItem  (this.CreateParameter);  // le bouton [+] crée une ligne d'article

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticleParameterDefinitions, template));
		}

		private void CreateUIPrices(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticlePrice",
					IconUri		 = "Data.ArticlePrice",
					Title		 = TextFormatter.FormatText ("Prix"),
					CompactTitle = TextFormatter.FormatText ("Prix"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ArticlePriceEntity> ("ArticlePrice", data.Controller, this.DataContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());
			template.DefineCreateItem  (this.CreateArticlePrice);

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.ArticlePrices, template));
		}
		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.DataContext, data, this.EntityGetter, x => x.Comments);
		}


		private NumericValueArticleParameterDefinitionEntity CreateParameter()
		{
			//	Crée une nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			return this.DataContext.CreateEntityAndRegisterAsEmpty<NumericValueArticleParameterDefinitionEntity> ();
		}

		private ArticlePriceEntity CreateArticlePrice()
		{
			//	Crée une nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			var price = this.DataContext.CreateEntityAndRegisterAsEmpty<ArticlePriceEntity> ();

			price.CurrencyCode = Business.Finance.CurrencyCode.Chf;
			price.MinQuantity = 1;

			var now = System.DateTime.Now;
			var begin = this.OldestPriceDate;

			if (begin == System.DateTime.MinValue)
			{
				begin = new System.DateTime (now.Year, 1, 1);  // le premier janvier de l'année en cours
			}
			else
			{
				begin = new System.DateTime (begin.Ticks + 10000000);  // begin + une seconde
			}

			price.BeginDate = begin;
			price.EndDate = new System.DateTime (begin.Year+10, 12, 31, 23, 59, 59);

			return price;
		}

		private System.DateTime OldestPriceDate
		{
			get
			{
				System.DateTime oldest = System.DateTime.MinValue;

				foreach (var price in this.Entity.ArticlePrices)
				{
					if (price.EndDate.HasValue && price.EndDate.Value > oldest)
					{
						oldest = price.EndDate.Value;
					}
				}

				return oldest;
			}
		}


		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}
	}
}
