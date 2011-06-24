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
				builder.CreatePanelTitleTile ("Data.AbstractArticleParameterDefinition", "Paramètre d'article à créer...");

				this.CreateUINewNumericButton (builder);
				this.CreateUINewEnumButton (builder);
				this.CreateUINewOptionButton (builder);
				this.CreateUINewFreeTextButton (builder);
				
				builder.EndPanelTitleTile ();
			}
		}


		private void CreateUINewEnumButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra par exemple de choisir une couleur parmi rouge, vert et bleu.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Enumération", help, this.SetupEnum);
		}

		private void CreateUINewOptionButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra par exemple de déterminer de la présence ou non d'une option.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Option", help, this.SetupOption);
		}

		private void CreateUINewNumericButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra par exemple de spécifier une dimension.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Numérique", help, this.SetupNumeric);
		}

		private void CreateUINewFreeTextButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra d'entrer un texte libre.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity> (this, "Texte libre", help, this.SetupFreeText);
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
