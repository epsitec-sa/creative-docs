//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Petit widget présent généralement dans les toolbars, permettant d'effectuer une
	/// recherche textuelle en avant ou en arrière dans le DataFiller d'un TreeTable.
	/// </summary>
	public class SearchController
	{
		public SearchController(SearchKind kind)
		{
			this.kind = kind;
		}


		public void CreateUI(Widget parent)
		{
			const int margin = 4;

			this.textField = new TextFieldCombo
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
				Margins         = new Margins (0, AbstractScroller.DefaultBreadth, margin, margin),
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
				this.Definition = this.Definition.FromPattern (null);
				this.UpdateWidgets ();
				this.textField.Focus ();
			};

			this.prevButton.Clicked += delegate
			{
				this.AddLastPattern ();
				this.OnSearch (this.Definition, -1);  // cherche en arrière
			};

			this.nextButton.Clicked += delegate
			{
				this.AddLastPattern ();
				this.OnSearch (this.Definition, 1);  // cherche en avant
			};

			this.optionsButton.Clicked += delegate
			{
				this.ShowOptionsPopup (this.optionsButton);
			};

			this.UpdateWidgets ();
			this.InitializeCombo ();
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
			if (this.textField.Text != this.Definition.Pattern)
			{
				this.textField.Text = this.Definition.Pattern;
			}

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


		private void AddLastPattern()
		{
			var pattern = this.textField.Text;
			var list = this.LastPatterns;

			if (list.Contains (pattern))
			{
				list.Remove (pattern);
			}

			list.Insert (0, pattern);

			while (list.Count > 20)
			{
				list.RemoveAt (list.Count-1);
			}

			this.InitializeCombo ();
		}

		private void InitializeCombo()
		{
			this.textField.Items.Clear ();
			this.textField.Items.AddRange (this.LastPatterns);
		}

		private List<string> LastPatterns
		{
			get
			{
				return LocalSettings.GetSearchInfo (this.kind).LastPatterns;
			}
		}

		private SearchDefinition Definition
		{
			get
			{
				return LocalSettings.GetSearchInfo (this.kind).Definition;
			}
			set
			{
				var info = LocalSettings.GetSearchInfo (this.kind);
				info = info.FromDefinition (value);
				LocalSettings.SetSearchInfo (this.kind, info);
			}
		}


		#region Events handler
		private void OnSearch(SearchDefinition definition, int direction)
		{
			this.Search.Raise (this, definition, direction);
		}

		public event EventHandler<SearchDefinition, int> Search;
		#endregion


		private const int buttonWidth = 18;

		private readonly SearchKind				kind;

		private TextFieldCombo					textField;
		private IconButton						clearButton;
		private IconButton						prevButton;
		private IconButton						nextButton;
		private IconButton						optionsButton;
	}
}