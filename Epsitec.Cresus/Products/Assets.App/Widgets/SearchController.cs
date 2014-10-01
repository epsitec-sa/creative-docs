//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Petit widget présent généralement dans les toolbars, permettant d'effectuer
	/// une recherche textuelle.
	/// </summary>
	public class SearchController
	{
		public string							SearchText
		{
			get
			{
				if (this.textField == null)
				{
					return null;
				}
				else
				{
					return this.textField.Text;
				}
			}
			set
			{
				if (this.textField != null)
				{
					this.textField.Text = value;
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
				IconUri         = Misc.GetResourceIconUri ("Field.Delete"),
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
				this.SearchText = null;
				this.UpdateWidgets ();
			};

			this.prevButton.Clicked += delegate
			{
				this.OnSearch (-1);
			};

			this.nextButton.Clicked += delegate
			{
				this.OnSearch (1);
			};

			this.UpdateWidgets ();
		}


		private void UpdateWidgets()
		{
			bool enable = !string.IsNullOrEmpty (this.SearchText);

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
		private void OnSearch(int direction)
		{
			this.Search.Raise (this, direction);
		}

		public event EventHandler<int> Search;
		#endregion


		private const int buttonWidth = 20;

		private TextField						textField;
		private IconButton						clearButton;
		private GlyphButton						prevButton;
		private GlyphButton						nextButton;
	}
}