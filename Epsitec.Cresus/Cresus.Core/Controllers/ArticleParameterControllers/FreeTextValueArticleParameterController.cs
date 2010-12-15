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
		public FreeTextValueArticleParameterController(IArticleDefinitionParameters article, int parameterIndex)
			: base (article, parameterIndex)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var freeTextParameter = this.ParameterDefinition as FreeTextValueArticleParameterDefinitionEntity;
			double buttonWidth = 14;

			//	Ligne éditable.
			var editor = new AutoCompleteTextField
			{
				Parent = parent,
				MenuButtonWidth = buttonWidth-1,
				Dock = DockStyle.Fill,
				HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
				TabIndex = 1,
			};

			//	Initialise le contenu par défaut.
			string initialValue = this.ParameterValue;

			if (string.IsNullOrEmpty (initialValue))
			{
				initialValue = freeTextParameter.ShortText.ToString ();
			}

			editor.Text = initialValue;

			editor.EditionAccepted += delegate
			{
				this.ParameterValue = editor.Text;
			};
		}

		private FormattedText GetUserText(string value)
		{
			return TextFormatter.FormatText (value);
		}

		private HintComparerResult MatchUserText(string value, string userText)
		{
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var itemText = TextConverter.ConvertToLowerAndStripAccents (value);
			return AutoCompleteTextField.Compare (itemText, userText);
		}
	}
}
