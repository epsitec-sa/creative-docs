//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class CreationArticleParameterDefinitionViewController : CreationViewController<AbstractArticleParameterDefinitionEntity>
	{
		public CreationArticleParameterDefinitionViewController()
		{
		}
		
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
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity, EnumValueArticleParameterDefinitionEntity> (this, "Enumération", help);
		}

		private void CreateUINewOptionButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra par exemple de déterminer de la présence ou non d'une option.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity, OptionValueArticleParameterDefinitionEntity> (this, "Option", help);
		}

		private void CreateUINewNumericButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra par exemple de spécifier une dimension.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity, NumericValueArticleParameterDefinitionEntity> (this, "Numérique", help);
		}

		private void CreateUINewFreeTextButton(UIBuilder builder)
		{
			var help = "Crée un paramètre qui permettra d'entrer un texte libre.";
			builder.CreateCreationButton<AbstractArticleParameterDefinitionEntity, FreeTextValueArticleParameterDefinitionEntity> (this, "Texte libre", help);
		}
	}
}
