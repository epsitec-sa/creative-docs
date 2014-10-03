//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Petit widget présent généralement à droite dans les toolbars, permettant d'effectuer
	/// une recherche textuelle en avant ou en arrière dans le DataFiller d'un TreeTable.
	/// </summary>
	public class SearchController
	{
		public SearchController(CommandDispatcher commandDispatcher, CommandContext commandContext, SearchKind kind)
		{
			this.commandDispatcher = commandDispatcher;
			this.commandContext    = commandContext;
			this.kind              = kind;

			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}


		public void CreateUI(Widget parent)
		{
			//	Crée la UI dans un FrameBox spécifique qu'il faut avoir créé au préalable,
			//	et qui est donné en tant que parent.
			const int margin = 4;

			CommandDispatcher.SetDispatcher (parent, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

			this.textField = new TextFieldCombo
			{
				Parent          = parent,
				TextDisplayMode = TextFieldDisplayMode.ActiveHint,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Fill,
			};

			new IconButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Search.Options,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, AbstractScroller.DefaultBreadth, margin, margin),
				Dock            = DockStyle.Right,
			};

			new IconButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Search.Next,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			new IconButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Search.Prev,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			new IconButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Search.Clear,
				AutoFocus       = false,
				PreferredWidth  = SearchController.buttonWidth,
				Margins         = new Margins (0, 0, margin, margin),
				Dock            = DockStyle.Right,
			};

			//	Connexion des événements.
			this.textField.PreProcessing += delegate (object sender, MessageEventArgs e)
			{
				if (e.Message.MessageType == MessageType.KeyDown)
				{
					if (e.Message.KeyCode == KeyCode.Return ||
						e.Message.KeyCode == KeyCode.NumericEnter)
					{
						if (e.Message.IsShiftPressed)
						{
							this.DoPrev ();
						}
						else
						{
							this.DoNext ();
						}
						e.Cancel = true;
					}
				}
			};

			this.textField.TextChanged += delegate
			{
				this.Definition = this.Definition.FromPattern (this.textField.Text);
				this.UpdateWidgets ();
			};

			this.UpdateWidgets ();
			this.InitializeCombo ();
		}


		[Command (Res.CommandIds.Search.Clear)]
		private void DoClear()
		{
			this.UpdateWidgets ();
			this.textField.SelectAll ();
			this.textField.Focus ();
		}

		[Command (Res.CommandIds.Search.Prev)]
		private void DoPrev()
		{
			this.AddLastPattern ();
			this.OnSearch (this.Definition, -1);  // cherche en arrière
		}

		[Command (Res.CommandIds.Search.Next)]
		private void DoNext()
		{
			this.AddLastPattern ();
			this.OnSearch (this.Definition, 1);  // cherche en avant
		}

		[Command (Res.CommandIds.Search.Options)]
		private void DoOptions(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = AbstractCommandToolbar.GetTarget (this.commandDispatcher, e);
			this.ShowOptionsPopup (target);
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

			this.commandContext.GetCommandState (Res.Commands.Search.Prev).Enable = enable;
			this.commandContext.GetCommandState (Res.Commands.Search.Next).Enable = enable;
		}


		private void AddLastPattern()
		{
			//	Ajoute le pattern utilisé dans la liste des derniers partterns.
			//	S'il y était déjà, il repasse en tête de liste.
			var pattern = this.textField.Text;
			var list = this.LastPatterns;

			if (list.Contains (pattern))
			{
				list.Remove (pattern);
			}

			list.Insert (0, pattern);  // insère au début

			while (list.Count > 20)  // répète tant qu'il y en a trop
			{
				list.RemoveAt (list.Count-1);  // supprime le dernier
			}

			this.InitializeCombo ();
		}

		private void InitializeCombo()
		{
			this.textField.Items.Clear ();
			this.textField.Items.AddRange (this.LastPatterns);
		}

		private List<string>					LastPatterns
		{
			get
			{
				return LocalSettings.GetSearchInfo (this.kind).LastPatterns;
			}
		}

		private SearchDefinition				Definition
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

		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;
		private readonly SearchKind				kind;

		private TextFieldCombo					textField;
	}
}