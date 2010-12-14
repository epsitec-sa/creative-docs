﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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

namespace Epsitec.Cresus.Core.Controllers.ArticleParameterControllers
{
	/// <summary>
	/// Ce contrôleur permet de saisir une valeur numérique pour un paramètre d'article, dans une ligne d'article
	/// d'une facture.
	/// </summary>
	public class OptionValueArticleParameterController : AbstractArticleParameterController
	{
		public OptionValueArticleParameterController(ArticleDocumentItemEntity article, int parameterIndex)
			: base (article, parameterIndex)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var optionParameter = this.ParameterDefinition as OptionValueArticleParameterDefinitionEntity;

			// TODO:
		}
	}
}
