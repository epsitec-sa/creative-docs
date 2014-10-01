//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Petit widget présent généralement dans les toolbars, permettant d'effectuer une
	/// recherche textuelle en avant ou en arrière dans le DataFiller d'un TreeTable.
	/// </summary>
	public class SearchController
	{
		public SearchController()
		{
			this.definition = SearchDefinition.Default;
		}

		public SearchDefinition					Definition
		{
			get
			{
				return this.definition;
			}
			set
			{
				this.definition = value;
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

			this.optionsButton = new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri ("Search.Options"),
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			this.nextButton = new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri ("Search.Next"),
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			this.prevButton = new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri ("Search.Prev"),
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			this.clearButton = new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri ("Search.Clear"),  // gomme
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			ToolTip.Default.SetToolTip (this.clearButton,   Res.Strings.SearchController.Tooltip.Clear.ToString ());
			ToolTip.Default.SetToolTip (this.prevButton,    Res.Strings.SearchController.Tooltip.Prev.ToString ());
			ToolTip.Default.SetToolTip (this.nextButton,    Res.Strings.SearchController.Tooltip.Next.ToString ());
			ToolTip.Default.SetToolTip (this.optionsButton, Res.Strings.SearchController.Tooltip.Options.ToString ());

			//	Connexion des événements.
			this.textField.TextChanged += delegate
			{
				this.Definition = this.Definition.FromPattern (this.textField.Text);
				this.UpdateWidgets ();
			};

			this.clearButton.Clicked += delegate
			{
				this.textField.Text = null;
				this.Definition = this.Definition.FromPattern (null);
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

			this.optionsButton.Clicked += delegate
			{
				this.ShowOptionsPopup (this.optionsButton);
			};

			this.UpdateWidgets ();
		}


		private void ShowOptionsPopup(Widget target)
		{
			var popup = new SearchOptionsPopup (null)
			{
				Options = this.Definition.Options,
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.Definition = this.Definition.FromOptions (popup.Options);
				}
			};
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


		private const int buttonWidth = 18;

		private SearchDefinition				definition;
		private TextField						textField;
		private IconButton						clearButton;
		private IconButton						prevButton;
		private IconButton						nextButton;
		private IconButton						optionsButton;
	}
}