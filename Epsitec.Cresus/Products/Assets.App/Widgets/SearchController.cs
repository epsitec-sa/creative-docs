//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Petit widget présent généralement dans les toolbars, permettant d'effectuer une
	/// recherche textuelle en avant ou en arrière dans le DataFiller d'un TreeTable.
	/// </summary>
	public class SearchController
	{
		public SearchDefinition					Definition
		{
			get
			{
				if (this.textField == null)
				{
					return SearchDefinition.Default;
				}
				else
				{
					return SearchDefinition.Default.FromPattern (this.textField.Text);
				}
			}
			set
			{
				if (this.textField != null)
				{
					this.textField.Text = value.Pattern;
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			const int margin = 4;

			this.textField = new TextField
			{
				Parent          = parent,
				TextDisplayMode = TextFieldDisplayMode.ActiveHint,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Fill,
			};

			this.nextButton = new GlyphButton
			{
				Parent          = parent,
				GlyphShape      = GlyphShape.TriangleDown,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			this.prevButton = new GlyphButton
			{
				Parent          = parent,
				GlyphShape      = GlyphShape.TriangleUp,
				ButtonStyle     = ButtonStyle.ToolItem,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			this.clearButton = new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri ("Field.Delete"),  // gomme
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			ToolTip.Default.SetToolTip (this.clearButton, Res.Strings.SearchController.Tooltip.Clear.ToString ());
			ToolTip.Default.SetToolTip (this.prevButton,  Res.Strings.SearchController.Tooltip.Prev.ToString ());
			ToolTip.Default.SetToolTip (this.nextButton,  Res.Strings.SearchController.Tooltip.Next.ToString ());

			//	Connexion des événements.
			this.textField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.clearButton.Clicked += delegate
			{
				this.Definition = SearchDefinition.Default;
				this.UpdateWidgets ();
			};

			this.prevButton.Clicked += delegate
			{
				this.OnSearch (this.Definition, -1);  // cherche en arrière
			};

			this.nextButton.Clicked += delegate
			{
				this.OnSearch (this.Definition, 1);  // cherche en avant
			};

			this.UpdateWidgets ();
		}


		private void UpdateWidgets()
		{
			bool enable = this.Definition.IsActive;

			if (enable)
			{
				this.textField.HintText = null;
			}
			else
			{
				this.textField.HintText = Res.Strings.SearchController.Hint.ToString ();
			}

			this.clearButton.Enable = enable;
			this.prevButton.Enable = enable;
			this.nextButton.Enable = enable;
		}


		#region Events handler
		private void OnSearch(SearchDefinition definition, int direction)
		{
			this.Search.Raise (this, definition, direction);
		}

		public event EventHandler<SearchDefinition, int> Search;
		#endregion


		private const int buttonWidth = 20;

		private TextField						textField;
		private IconButton						clearButton;
		private GlyphButton						prevButton;
		private GlyphButton						nextButton;
	}
}