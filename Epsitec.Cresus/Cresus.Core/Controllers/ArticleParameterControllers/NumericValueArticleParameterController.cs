//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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
			double buttonWidth = 14;

			var editor = new AutoCompleteTextField
			{
				Parent = parent,
				MenuButtonWidth = buttonWidth-1,
				Dock = DockStyle.Fill,
				Text = this.ParameterValue,
				HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
			};

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
				this.ParameterValue = editor.Text;
			};

			menuButton.Clicked += delegate
			{
				editor.SelectAll ();
				editor.Focus ();
				editor.OpenComboMenu ();
			};
		}
	}
}
