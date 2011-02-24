//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentReminderDefinitionViewController : EditionViewController<Entities.PaymentReminderDefinitionEntity>
	{
		public EditionPaymentReminderDefinitionViewController(string name, Entities.PaymentReminderDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PaymentReminderDefinition", "Rappel");

				this.CreateUIMain (builder);
				this.CreateUITaxArticle (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 100, "Code",                    Marshaler.Create (() => this.Entity.Code,             x => this.Entity.Code = x));
			builder.CreateTextField      (tile,   0, "Nom",                     Marshaler.Create (() => this.Entity.Name,             x => this.Entity.Name = x));
			builder.CreateTextFieldMulti (tile,  72, "Description",             Marshaler.Create (() => this.Entity.Description,      x => this.Entity.Description = x));
			builder.CreateTextField      (tile, 100, "Terme (nombre de jours)", Marshaler.Create (() => this.Entity.ExtraPaymentTerm, x => this.Entity.ExtraPaymentTerm = x));
		}

		private void CreateUITaxArticle(UIBuilder builder)
		{
			throw new System.NotImplementedException ();
#if false
			var controller = new SelectionController<ArticleDefinitionEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.AdministrativeTaxArticle,
				ValueSetter         = x => this.Entity.AdministrativeTaxArticle = x,
				ReferenceController = new ReferenceController (() => this.Entity.AdministrativeTaxArticle, creator: this.CreateNewTaxArticleDefinition),

				ToTextArrayConverter     = x => x.GetEntityKeywords (),
				ToFormattedTextConverter = x => x.GetCompactSummary ()
			};

			builder.CreateAutoCompleteTextField ("Article pour facturer une taxe", controller);
#endif
		}


		private NewEntityReference CreateNewTaxArticleDefinition(DataContext context)
		{
			var article = context.CreateEntityAndRegisterAsEmpty<ArticleDefinitionEntity> ();
			return article;
		}
	}
}
