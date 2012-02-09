//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class SearchTabController
	{
		public SearchTabController(AbstractController controller, SearchTabData tabData, bool isFilter)
		{
			this.controller = controller;
			this.tabData    = tabData;
			this.isFilter   = isFilter;

			this.columnMappers = this.controller.ColumnMappers;

			this.columnIndexes = new List<int> ();
		}


		public FrameBox CreateUI(FrameBox parent, bool bigDataInterface, System.Action searchStartAction, System.Action<int> addRemoveAction)
		{
			this.bigDataInterface = bigDataInterface;
			this.searchStartAction = searchStartAction;

			var frameBox = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 20,
				Dock                = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			{
				this.searchFromFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 1,
				};

				this.searchFromLabel = new StaticText
				{
					Parent          = this.searchFromFrame,
					Text            = "De",
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
				};

				this.searchField1 = new TextField
				{
					Parent          = this.searchFromFrame,
					Text            = this.tabData.SearchText.FromText,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 1,
				};

				this.searchFieldEx1 = new TextFieldEx
				{
					Parent                       = this.searchFromFrame,
					Text                         = this.tabData.SearchText.FromText,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Fill,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = 1,
				};
			}

			{
				this.searchToFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (1, 0, 0, 0),
					TabIndex        = 2,
				};

				this.searchToLabel = new StaticText
				{
					Parent          = this.searchToFrame,
					Text            = "À",
					PreferredWidth  = 12,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (5, 0, 0, 0),
				};

				this.searchField2 = new TextField
				{
					Parent          = this.searchToFrame,
					Text            = this.tabData.SearchText.ToText,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 2,
				};

				this.searchFieldEx2 = new TextFieldEx
				{
					Parent                       = this.searchToFrame,
					Text                         = this.tabData.SearchText.ToText,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Fill,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = 2,
				};
			}

			this.addRemoveButton = new IconButton
			{
				Parent          = frameBox,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (5, 0, 0, 0),
			};

			{
				this.modeFrame = UIBuilder.CreatePseudoCombo (frameBox, out this.modeField, out this.modeButton);
				this.modeField.Text = this.ModeDescription;
				this.modeFrame.Dock = DockStyle.Right;
			}

			this.columnField = new TextFieldCombo
			{
				Parent          = frameBox,
				IsReadOnly      = true,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				PreferredWidth  = 130,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
				TabIndex        = 3,
			};

			this.InitializeColumnsCombo ();
			this.UpdateFields ();
			this.UpdateButtons ();

			this.searchField1.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.tabData.SearchText.FromText = this.searchField1.Text;
					this.searchStartAction ();
				}
			};

			this.searchFieldEx1.EditionAccepted += delegate
			{
				this.tabData.SearchText.FromText = this.searchFieldEx1.Text;
				this.searchStartAction ();
			};

			this.searchField2.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.tabData.SearchText.ToText = this.searchField2.Text;
					this.searchStartAction ();
				}
			};

			this.searchFieldEx2.EditionAccepted += delegate
			{
				this.tabData.SearchText.ToText = this.searchFieldEx2.Text;
				this.searchStartAction ();
			};

			this.columnField.SelectedItemChanged += delegate
			{
				if (!this.ignoreChange)
				{
					int sel = this.columnField.SelectedItemIndex - 1;
					if (sel < 0)
					{
						this.tabData.Column = ColumnType.None;
					}
					else
					{
						int column = this.columnIndexes[sel];
						this.tabData.Column = this.columnMappers[column].Column;
					}

					this.searchStartAction ();
				}
			};

			this.modeField.Clicked += delegate
			{
				this.ShowModeMenu (this.modeFrame);
			};

			this.modeButton.Clicked += delegate
			{
				this.ShowModeMenu (this.modeFrame);
			};

			this.addRemoveButton.Clicked += delegate
			{
				addRemoveAction (this.Index);
			};

			ToolTip.Default.SetToolTip (this.columnField, this.isFilter ? "Colonne à filtrer" : "Colonne où chercher ?");
			ToolTip.Default.SetToolTip (this.modeField,   this.isFilter ? "Comment filtrer"   : "Comment chercher ?");
			ToolTip.Default.SetToolTip (this.modeButton,  this.isFilter ? "Comment filtrer"   : "Comment chercher ?");

			if (this.bigDataInterface)
			{
				this.searchFieldEx1.SelectAll ();
				this.searchFieldEx1.Focus ();
			}
			else
			{
				this.searchField1.SelectAll ();
				this.searchField1.Focus ();
			}

			return frameBox;
		}

		public void SetFocus()
		{
			if (this.searchFieldEx1.Visibility)
			{
				this.searchFieldEx1.Focus ();
			}
			else
			{
				this.searchField1.Focus ();
			}
		}

		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.InitializeColumnsCombo ();
		}

		public void SetAddAction(bool add, bool enable)
		{
			if (this.addAction != add || this.addActionEnable != enable)
			{
				this.addAction       = add;
				this.addActionEnable = enable;

				this.UpdateButtons ();
			}
		}

		public int Index
		{
			get;
			set;
		}


		private void InitializeColumnsCombo()
		{
			this.columnField.Items.Clear ();
			this.columnIndexes.Clear ();

			int sel = 0;
			this.columnField.Items.Add ("Partout");
			int index = 1;

			for (int i = 0; i < this.columnMappers.Count; i++)
			{
				var mapper = this.columnMappers[i];

				if (!mapper.HideForSearch && (this.isFilter || mapper.Show))
				{
					FormattedText desc = mapper.Description;

					if (desc.IsNullOrEmpty)
					{
						desc = string.Format ("colonne n° {0}", (i+1).ToString ());
					}

					this.columnField.Items.Add ("Dans " + desc);

					if (mapper.Column == this.tabData.Column)
					{
						sel = index;
					}

					this.columnIndexes.Add (i);
					index++;
				}
			}

			this.ignoreChange = true;
			this.columnField.SelectedItemIndex = sel;
			this.ignoreChange = false;
		}


		private void UpdateFields()
		{
			this.searchToFrame.Visibility   = (this.tabData.SearchText.Mode == SearchMode.Interval);
			this.searchFromLabel.Visibility = (this.tabData.SearchText.Mode == SearchMode.Interval);
			this.searchToLabel.Visibility   = (this.tabData.SearchText.Mode == SearchMode.Interval);

			this.searchField1.Visibility   = !this.bigDataInterface;
			this.searchFieldEx1.Visibility =  this.bigDataInterface;
			this.searchField2.Visibility   = !this.bigDataInterface;
			this.searchFieldEx2.Visibility =  this.bigDataInterface;

			this.searchField1.IsReadOnly = (this.tabData.SearchText.Mode == SearchMode.Empty);
			this.searchFieldEx1.IsReadOnly = (this.tabData.SearchText.Mode == SearchMode.Empty);

			string textField;

			if (this.tabData.SearchText.Mode == SearchMode.Interval)
			{
				textField = this.isFilter ? "Que filtrer (depuis, inclu) ?" : "Que chercher (depuis, inclu) ?";
				ToolTip.Default.SetToolTip (this.searchField1, textField);
				ToolTip.Default.SetToolTip (this.searchFieldEx1, textField);

				textField = this.isFilter ? "Que filtrer (jusqu'à, inclu) ?" : "Que chercher (jusqu'à, inclu) ?";
				ToolTip.Default.SetToolTip (this.searchField2, textField);
				ToolTip.Default.SetToolTip (this.searchFieldEx2, textField);
			}
			else if (this.tabData.SearchText.Mode == SearchMode.Empty)
			{
				this.searchField1.Text   = null;
				this.searchFieldEx1.Text = null;

				textField = this.isFilter ? "Filtre les données vides" : "Cherche les données vides";
				ToolTip.Default.SetToolTip (this.searchField1,   textField);
				ToolTip.Default.SetToolTip (this.searchFieldEx1, textField);
			}
			else
			{
				textField = this.isFilter ? "Que filtrer ?" : "Que chercher ?";
				ToolTip.Default.SetToolTip (this.searchField1,   textField);
				ToolTip.Default.SetToolTip (this.searchFieldEx1, textField);
			}
		}

		private void UpdateButtons()
		{
			this.addRemoveButton.IconUri = UIBuilder.GetResourceIconUri (this.addAction ? "Multi.Insert" : "Multi.Delete");
			this.addRemoveButton.Enable = !this.addAction || this.addActionEnable;

			if (this.addAction)
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Ajoute un nouveau critère de filtre" : "Ajoute un nouveau critère de recherche");
			}
			else
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Supprime le critère de filtre" : "Supprime le critère de recherche");
			}
		}


		#region Mode menu
		private void ShowModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode.
			var menu = new VMenu ();

			this.AddModeToMenu (menu, SearchMode.Fragment);
			this.AddModeToMenu (menu, SearchMode.StartsWith);
			this.AddModeToMenu (menu, SearchMode.EndsWith);
			this.AddModeToMenu (menu, SearchMode.WholeContent);
			this.AddModeToMenu (menu, SearchMode.Interval);
			this.AddModeToMenu (menu, SearchMode.Empty);
			this.AddModeToMenu (menu, SearchMode.Jokers);

			bool separator = false;

			if (this.HasMatchCase)
			{
				if (!separator)
				{
					menu.Items.Add (new MenuSeparator ());
					separator = true;
				}

				this.AddModeToMenu (menu, "Respecter la casse", () => this.tabData.SearchText.MatchCase, x => this.tabData.SearchText.MatchCase = x);
			}

			if (this.HasWholeWord)
			{
				if (!separator)
				{
					menu.Items.Add (new MenuSeparator ());
					separator = true;
				}

				this.AddModeToMenu (menu, "Mot entier", () => this.tabData.SearchText.WholeWord, x => this.tabData.SearchText.WholeWord = x);
			}

			if (this.HasInvert)
			{
				if (!separator)
				{
					menu.Items.Add (new MenuSeparator ());
					separator = true;
				}

				this.AddModeToMenu (menu, "Inverser", () => this.tabData.SearchText.Invert, x => this.tabData.SearchText.Invert = x);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddModeToMenu(VMenu menu, SearchMode mode)
		{
			bool selected = (this.tabData.SearchText.Mode == mode);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (selected ? "Button.RadioYes" : "Button.RadioNo"),
				FormattedText = SearchTabController.GetModeDescription (mode),
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				this.tabData.SearchText.Mode = (SearchMode) System.Enum.Parse (typeof (SearchMode), item.Name);
				
				this.modeField.Text = this.ModeDescription;
				this.UpdateFields ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}

		private void AddModeToMenu(VMenu menu, FormattedText text, System.Func<bool> getter, System.Action<bool> setter)
		{
			bool selected = getter ();

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (selected ? "Button.CheckYes" : "Button.CheckNo"),
				FormattedText = text,
			};

			item.Clicked += delegate
			{
				setter (!getter ());

				this.modeField.Text = this.ModeDescription;
				this.UpdateFields ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}
		#endregion


		private string ModeDescription
		{
			//	Retourne un texte court qui résume le mode.
			get
			{
				string text = SearchTabController.GetModeDescription (this.tabData.SearchText.Mode);

				bool separator = false;

				if (this.tabData.SearchText.MatchCase && this.HasMatchCase)
				{
					if (!separator)
					{
						text += " (";
						separator = true;
					}

					text += "c";
				}

				if (this.tabData.SearchText.WholeWord && this.HasWholeWord)
				{
					if (!separator)
					{
						text += " (";
						separator = true;
					}

					text += "m";
				}

				if (this.tabData.SearchText.Invert && this.HasInvert)
				{
					if (!separator)
					{
						text += " (";
						separator = true;
					}

					text += "i";
				}

				if (separator)
				{
					text += ")";
				}

				return text;
			}
		}

		private static string GetModeDescription(SearchMode mode)
		{
			//	Retourne la description courte d'un mode.
			switch (mode)
			{
				case SearchMode.Fragment:
					return "Fragment";

				case SearchMode.StartsWith:
					return "Au début";

				case SearchMode.EndsWith:
					return "À la fin";

				case SearchMode.WholeContent:
					return "Tout";

				case SearchMode.Jokers:
					return "Jokers";

				case SearchMode.Interval:
					return "Intervalle";

				case SearchMode.Empty:
					return "Vide";

				default:
					return "?";

			}
		}

		private bool HasMatchCase
		{
			get
			{
				return this.tabData.SearchText.Mode != SearchMode.Interval &&
					   this.tabData.SearchText.Mode != SearchMode.Empty;
			}
		}

		private bool HasWholeWord
		{
			get
			{
				return this.tabData.SearchText.Mode != SearchMode.Jokers   &&
					   this.tabData.SearchText.Mode != SearchMode.Interval &&
					   this.tabData.SearchText.Mode != SearchMode.Empty;
			}
		}

		private bool HasInvert
		{
			get
			{
				return this.isFilter;
			}
		}


		private readonly AbstractController		controller;
		private readonly List<ColumnMapper>		columnMappers;
		private readonly SearchTabData			tabData;
		private readonly List<int>				columnIndexes;
		private readonly bool					isFilter;

		private System.Action					searchStartAction;
		private FrameBox						searchFromFrame;
		private StaticText						searchFromLabel;
		private TextField						searchField1;
		private TextFieldEx						searchFieldEx1;
		private FrameBox						searchToFrame;
		private StaticText						searchToLabel;
		private TextField						searchField2;
		private TextFieldEx						searchFieldEx2;
		private TextFieldCombo					columnField;
		private FrameBox						modeFrame;
		private StaticText						modeField;
		private GlyphButton						modeButton;
		private IconButton						addRemoveButton;

		private bool							bigDataInterface;
		private bool							addAction;
		private bool							addActionEnable;
		private bool							ignoreChange;
	}
}
