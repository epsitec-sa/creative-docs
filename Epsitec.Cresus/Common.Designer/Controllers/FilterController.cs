﻿//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Designer.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la ligne qui contient "Filtrer [    ] x v".
	/// Le bouton 'x' efface le filtre.
	/// Le bouton 'v' montre un menu avec les options.
	/// </summary>
	public class FilterController
	{
		public FilterController(System.Func<string, string> filterUnifier = null)
		{
			this.filterUnifier = filterUnifier;
		}

		public FrameBox CreateUI(Widget parent, double labelWidth = 36)  // 36 = largeur pour "Filtrer"
		{
			labelWidth = System.Math.Max (labelWidth, 36);  // garde-fou: jamais moins

			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
			};

			this.label = new StaticText
			{
				Parent = frame,
				Text = Res.Strings.Controller.Filter.Label,
				PreferredWidth = labelWidth,
				Dock = DockStyle.Left,
			};

			this.field = new TextField
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				TabIndex = 1,
			};

			this.menuButton = new GlyphButton
			{
				Parent = frame,
				GlyphShape = GlyphShape.Menu,
				ButtonStyle = ButtonStyle.ToolItem,
				Dock = DockStyle.Right,
				Margins = new Margins (1, 0, 0, 0),
			};

			this.clearButton = new IconButton
			{
				Parent = frame,
				IconUri = Misc.Icon ("Reset"),
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
			};

			//	Connexion des événements.
			this.field.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.Filter = this.field.Text;
				}
			};

			this.clearButton.Clicked += delegate
			{
				this.ClearFilter ();
				this.SetFocus ();
			};

			this.menuButton.Clicked += delegate
			{
				this.ShowMenu ();
			};

			this.UpdateFilter ();
			this.UpdateWidgets ();

			return frame;
		}


		public void ClearFilter()
		{
			//	Efface le filtre.
			this.Filter = null;
		}

		public bool IsFiltered(string text)
		{
			//	Retourne true si un texte doit être exclu par le filtre.
			if (this.HasFilter && !string.IsNullOrEmpty (text))
			{
				return this.IsBaseFiltered (text);
			}

			return false;
		}

		public bool HasFilter
		{
			//	Retourne true s'il existe un filtre.
			get
			{
				return !string.IsNullOrEmpty (this.filterString);
			}
		}

		public string Filter
		{
			//	Texte brut du filtre.
			get
			{
				return this.filterString;
			}
			set
			{
				if (this.filterString != value)
				{
					this.filterString = value;

					this.UpdateFilter ();
					this.UpdateWidgets ();
					this.OnFilterChanged ();
				}
			}
		}

		public Searcher.SearchingMode Mode
		{
			//	Mode du filtre.
			get
			{
				return this.filterMode;
			}
			set
			{
				if (this.filterMode != value)
				{
					this.filterMode = value;

					this.UpdateFilter ();
					this.UpdateWidgets ();
					this.OnFilterChanged ();
				}
			}
		}

		public void SetFocus()
		{
			//	Met le focus dans la ligne éditable du filtre.
			this.field.SelectAll ();
			this.field.Focus ();
		}


		private void UpdateWidgets()
		{
			this.ignoreChange = true;
			this.field.Text = this.filterString;
			this.ignoreChange = false;

			this.clearButton.Enable = this.HasFilter;
		}

		private void UpdateFilter()
		{
			//	Met à jour les formes dérivées du filtre. Doit être appelé lorsque le texte ou le mode du filtre ont changé.
			if (string.IsNullOrEmpty (this.filterString))
			{
				this.filterStringPrepared = null;
				this.filterStringRegex = null;
			}
			else
			{
				string s = this.filterString;

				if (this.filterUnifier != null)
				{
					s = this.filterUnifier (s);
				}

				if ((this.Mode & Searcher.SearchingMode.CaseSensitive) == 0)
				{
					this.filterStringPrepared = Searcher.RemoveAccent (s.ToLowerInvariant ());
				}
				else
				{
					this.filterStringPrepared = s;
				}

				if ((this.Mode & Searcher.SearchingMode.Joker) == 0)
				{
					this.filterStringRegex = null;
				}
				else
				{
					this.filterStringRegex = RegexFactory.FromSimpleJoker (this.filterStringPrepared, RegexFactory.Options.None);
				}
			}
		}

		private bool IsBaseFiltered(string text)
		{
			if (this.filterStringRegex == null)  // filtre sans jokers ?
			{
				var mode = this.Mode;
				int start = 0;

				if ((mode & Searcher.SearchingMode.AtEnding) != 0)
				{
					mode |= Searcher.SearchingMode.Reverse;
					start = text.Length;
				}

				int index = Searcher.IndexOf (text, this.filterStringPrepared, start, mode);

				if (index == -1)
				{
					return true;  // rejeté
				}

				if ((this.Mode & Searcher.SearchingMode.AtBeginning) != 0 && index != 0)
				{
					return true;  // rejeté
				}

				if ((this.Mode & Searcher.SearchingMode.AtEnding) != 0 && index != text.Length-this.filterStringPrepared.Length)
				{
					return true;  // rejeté
				}

				return false;  // accepté
			}
			else  // filtre avec jokers ?
			{
				if (this.filterUnifier != null)
				{
					text = this.filterUnifier (text);
				}

				if ((this.Mode & Searcher.SearchingMode.CaseSensitive) == 0)
				{
					text = Searcher.RemoveAccent (text.ToLowerInvariant ());
				}

				return !this.filterStringRegex.IsMatch (text);
			}
		}


		#region Menu
		private void ShowMenu()
		{
			var menu = new VMenu ();

			bool begin = (this.Mode & Searcher.SearchingMode.AtBeginning) != 0;
			bool end   = (this.Mode & Searcher.SearchingMode.AtEnding) != 0;
			bool joker = (this.Mode & Searcher.SearchingMode.Joker) != 0;
			bool any   = !begin && !end && !joker;

			this.AddItemToMenu (menu, true, any,   "Any",   Res.Strings.Dialog.Filter.Radio.Any);
			this.AddItemToMenu (menu, true, begin, "Begin", Res.Strings.Dialog.Filter.Radio.Begin);
			this.AddItemToMenu (menu, true, end,   "End",   Res.Strings.Dialog.Filter.Radio.End);
			this.AddItemToMenu (menu, true, joker, "Joker", Res.Strings.Dialog.Filter.Radio.Joker);

			menu.Items.Add (new MenuSeparator ());
			
			this.AddItemToMenu (menu, false, (this.Mode & Searcher.SearchingMode.CaseSensitive) != 0, "Case", Res.Strings.Dialog.Filter.Check.Case);

			if (!joker)
			{
				this.AddItemToMenu (menu, false, (this.Mode & Searcher.SearchingMode.WholeWord) != 0, "Word", Res.Strings.Dialog.Filter.Check.Word);
			}

			this.ShowMenu (menu, this.menuButton);
		}

		private void ShowMenu(VMenu menu, Widget parent)
		{
			TextFieldCombo.AdjustComboSize (parent, menu, false);

			menu.Host = parent;
			menu.ShowAsComboList (parent, Point.Zero, parent);
		}

		private void AddItemToMenu(VMenu menu, bool radio, bool check, string name, string text)
		{
			string icon = radio ? Misc.GetMenuIconRadioState (check) : Misc.GetMenuIconCheckState (check);
			var item = new MenuItem (name, icon, text, null, name);

			item.Clicked += delegate
			{
				this.MenuAction (name);
			};

			menu.Items.Add (item);
		}

		private void MenuAction(string name)
		{
			Searcher.SearchingMode mode;

			switch (name)
			{
				case "Any":
					mode = this.Mode;
					mode &= ~Searcher.SearchingMode.AtBeginning;
					mode &= ~Searcher.SearchingMode.AtEnding;
					mode &= ~Searcher.SearchingMode.Joker;
					this.Mode = mode;
					break;

				case "Begin":
					mode = this.Mode;
					mode |=  Searcher.SearchingMode.AtBeginning;
					mode &= ~Searcher.SearchingMode.AtEnding;
					mode &= ~Searcher.SearchingMode.Joker;
					this.Mode = mode;
					break;

				case "End":
					mode = this.Mode;
					mode &= ~Searcher.SearchingMode.AtBeginning;
					mode |=  Searcher.SearchingMode.AtEnding;
					mode &= ~Searcher.SearchingMode.Joker;
					this.Mode = mode;
					break;

				case "Joker":
					mode = this.Mode;
					mode &= ~Searcher.SearchingMode.AtBeginning;
					mode &= ~Searcher.SearchingMode.AtEnding;
					mode |=  Searcher.SearchingMode.Joker;
					this.Mode = mode;
					break;

				case "Case":
					this.Mode ^= Searcher.SearchingMode.CaseSensitive;
					break;

				case "Word":
					this.Mode ^= Searcher.SearchingMode.WholeWord;
					break;
			}
		}
		#endregion


		private void OnFilterChanged()
		{
			var handler = this.FilterChanged;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event Support.EventHandler	FilterChanged;

		private readonly System.Func<string, string> filterUnifier;

		private string						filterString;
		private Searcher.SearchingMode		filterMode;
		private string						filterStringPrepared;
		private Regex						filterStringRegex;

		private StaticText					label;
		private TextField					field;
		private IconButton					clearButton;
		private GlyphButton					menuButton;

		private bool						ignoreChange;
	}
}
