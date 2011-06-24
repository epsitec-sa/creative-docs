//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationArticleParameterDefinitionViewController : CreationViewController<AbstractArticleParameterDefinitionEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreatePanelTitleTile ("Data.ArticleParameterDefinition", "Paramètre d'article à créer...");

				this.CreateUINewEnumButton (builder);
				this.CreateUINewOptionButton (builder);
				this.CreateUINewNumericButton (builder);
				this.CreateUINewFreeTextButton (builder);
				
				builder.EndPanelTitleTile ();
			}
		}


		private void CreateUINewEnumButton(UIBuilder builder)
		{
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Enumération", "Crée un paramètre énuméré", this.SetupEnum);
		}

		private void CreateUINewOptionButton(UIBuilder builder)
		{
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Option", "Crée un paramètre optionnel", this.SetupOption);
		}

		private void CreateUINewNumericButton(UIBuilder builder)
		{
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Numérique", "Crée un paramètre numérique", this.SetupNumeric);
		}

		private void CreateUINewFreeTextButton(UIBuilder builder)
		{
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Texte libre", "Crée un paramètre texte libre", this.SetupFreeText);
		}


		private void SetupEnum(BusinessContext context, AbstractArticleParameterDefinitionEntity param)
		{
			//?param = context.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();
		}

		private void SetupOption(BusinessContext context, AbstractArticleParameterDefinitionEntity param)
		{
			//?param = context.CreateEntity<OptionValueArticleParameterDefinitionEntity> ();
		}

		private void SetupNumeric(BusinessContext context, AbstractArticleParameterDefinitionEntity param)
		{
			//?param = context.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
		}

		private void SetupFreeText(BusinessContext context, AbstractArticleParameterDefinitionEntity param)
		{
			//?param = context.CreateEntity<FreeTextValueArticleParameterDefinitionEntity> ();
		}

	}
}
