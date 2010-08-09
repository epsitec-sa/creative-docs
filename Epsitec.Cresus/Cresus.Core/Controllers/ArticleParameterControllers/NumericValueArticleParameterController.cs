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
	public class NumericValueArticleParameterController : AbstractArticleParameterController
	{
		public NumericValueArticleParameterController(ArticleDocumentItemEntity article, int parameterIndex)
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
			};

			//	Initialise le contenu initial.
			string initialValue = this.ParameterValue;

			if (string.IsNullOrEmpty (initialValue))
			{
				initialValue = numericParameter.DefaultValue.ToString ();
			}

			editor.Text = initialValue;

			//	Initialise le menu des valeurs préférées.
			string[] preferred = numericParameter.PreferredValues.Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
			foreach (var v in preferred)
			{
				editor.Items.Add (v, v);
			}

			editor.ValueToDescriptionConverter = value => this.GetUserText (value as string);
			editor.HintComparer = (value, text) => this.MatchUserText (value as string, text);
			editor.HintComparisonConverter = x => TextConverter.ConvertToLowerAndStripAccents (x);

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

			editor.AcceptingEdition += delegate
			{
				int    index = editor.SelectedItemIndex;
				string key   = index < 0 ? null : editor.Items.GetKey (index);
				//?
				this.ParameterValue = editor.Text;
			};

			menuButton.Clicked += delegate
			{
				editor.SelectAll ();
				editor.Focus ();
				editor.OpenComboMenu ();
			};
		}

		private FormattedText GetUserText(string value)
		{
			return UIBuilder.FormatText (value);
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
