//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Ce contrôleur permet de saisir un texte libre pour un paramètre d'article, dans une ligne d'article
	/// d'une facture.
	/// </summary>
	public class FreeTextValueArticleParameterController : AbstractArticleParameterController
	{
		public FreeTextValueArticleParameterController(IArticleDefinitionParameters article, int parameterIndex, TileContainer tileContainer)
			: base (article, parameterIndex, tileContainer)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var editor = new TextFieldEx
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			//	Utilise Marshaler/TextValueController pour permettre une édtion multilingue.
			//	Par exemple, le changement de langue dans le ruban doit se refléter dans
			//	l'édition du texte.
			var marshaler = Marshaler.Create (() => this.FreeText, x => this.FreeText = x);
			var controller = new TextValueController (marshaler);
			controller.Attach (editor);
			this.tileContainer.Add (controller);
		}


		private FormattedText FreeText
		{
			get
			{
				string value = this.ParameterValue;

				if (string.IsNullOrEmpty (value))
				{
					var freeTextParameter = this.ParameterDefinition as FreeTextValueArticleParameterDefinitionEntity;
					value = freeTextParameter.ShortText.ToString ();
				}

				return value;
			}
			set
			{
				this.ParameterValue = value.ToString ();
			}
		}
	}
}
