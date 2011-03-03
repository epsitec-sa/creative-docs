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
	/// Ce contrôleur permet de saisir une valeur numérique pour un paramètre d'article, dans une ligne d'article
	/// d'une facture.
	/// </summary>
	public class NumericValueArticleParameterController : AbstractArticleParameterController
	{
		public NumericValueArticleParameterController(IArticleDefinitionParameters article, int parameterIndex)
			: base (article, parameterIndex)
		{
		}


		public override void CreateUI(FrameBox parent)
		{
			var numericParameter = this.ParameterDefinition as NumericValueArticleParameterDefinitionEntity;
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
				initialValue = numericParameter.DefaultValue.ToString ();
			}

			editor.Text = initialValue;

			//	Initialise le menu des valeurs préférées.
			string[] preferred = (numericParameter.PreferredValues ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			foreach (var v in preferred)
			{
				if (!string.IsNullOrEmpty (v))
				{
					editor.Items.Add (v, v);
				}
			}

			editor.ValueToDescriptionConverter = value => this.GetUserText (value as string);
			editor.HintComparer = (value, text) => this.MatchUserText (value as string, text);
			editor.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);
			editor.ContentValidator = x => this.ContentValidator (x);

			if (editor.Items.Count != 0)
			{
				//	Ce bouton vient juste après (et tout contre) la ligne éditable.
				var menuButton = new GlyphButton
				{
					Parent = parent,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = buttonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};

				menuButton.Clicked += delegate
				{
					editor.SelectAll ();
					editor.Focus ();
					editor.OpenComboMenu ();
				};
			}

			editor.EditionAccepted += delegate
			{
				string value = editor.Text;

				if (!this.ContentValidator (value))
				{
					value = numericParameter.DefaultValue.ToString ();
				}

				this.ParameterValue = value;
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

		private bool ContentValidator(string value)
		{
			if (string.IsNullOrWhiteSpace (value))
			{
				return true;
			}

			decimal d;
			if (decimal.TryParse (value.Trim (), out d))
			{
				var numericParameter = this.ParameterDefinition as NumericValueArticleParameterDefinitionEntity;

				if (numericParameter.MinValue.HasValue)
				{
					if (d < numericParameter.MinValue)
					{
						return false;
					}
				}

				if (numericParameter.MaxValue.HasValue)
				{
					if (d > numericParameter.MaxValue)
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
