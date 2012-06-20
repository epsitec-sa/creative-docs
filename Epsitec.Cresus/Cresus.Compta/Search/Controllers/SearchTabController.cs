//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Contrôleur gérant la saisie d'une ligne d'un critère SearchTabData pour les recherches ou les filtres.
	/// Il est composé d'un titre à gauche et de divers boutons à droite.
	/// </summary>
	public class SearchTabController
	{
		public SearchTabController(AbstractController controller, SearchNodeController parentController, SearchTabData tabData, bool isFilter)
		{
			this.controller       = controller;
			this.parentController = parentController;
			this.tabData          = tabData;
			this.isFilter         = isFilter;

			this.ignoreChanges = new SafeCounter ();
			this.columnMappers = this.controller.ColumnMappers;
		}


		public FrameBox CreateUI(FrameBox parent, bool bigDataInterface, System.Action searchStartAction, System.Action<int> addRemoveAction, System.Action swapNodeAction, System.Action swapTabAction)
		{
			this.bigDataInterface  = bigDataInterface;
			this.searchStartAction = searchStartAction;
			this.swapNodeAction    = swapNodeAction;
			this.swapTabAction     = swapTabAction;

			var frameBox = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 20,
				Dock                = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			{
				var labelFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Padding         = new Margins (5, 0, 0, 0),
				};

				this.CreateLabelUI (labelFrame);
			}

			this.intervalButton = new BackIconButton
			{
				Parent            = frameBox,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				BackColor         = Color.FromName ("White"),
				ActiveState       = ActiveState.Yes,
				AutoToggle        = false,
				AutoFocus         = false,
				Dock              = DockStyle.Left,
				Margins           = new Margins (0, -1, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.intervalButton, "Champ unique ou champs \"de ... à ...\"");

			//	Champ "De" ou champ unique.
			{
				this.searchFromFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 1,
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

			//	Champ "à".
			{
				this.searchToFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (-1, 0, 0, 0),
					TabIndex        = 2,
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

			this.activateButton = new BackIconButton
			{
				Parent          = frameBox,
				IconUri         = UIBuilder.GetResourceIconUri ("Search.ActivateTab"),
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (5, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.activateButton, "Active ou désactive cette ligne");

			{
				this.columnsFrame = UIBuilder.CreatePseudoCombo (frameBox, out this.columnsField, out this.columnsButton);
				this.columnsFrame.Dock           = DockStyle.Right;
				this.columnsFrame.PreferredWidth = 130;
				this.columnsFrame.Margins        = new Margins (1, 0, 0, 0);

				ToolTip.Default.SetToolTip (this.columnsFrame, this.isFilter ? "Colonnes à filtrer" : "Colonnes où chercher ?");
			}

			this.invertButton = new BackIconButton
			{
				Parent            = frameBox,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				BackColor         = Color.FromName ("White"),
				ActiveState       = ActiveState.Yes,
				AutoToggle        = false,
				AutoFocus         = false,
				Dock              = DockStyle.Right,
				Margins           = new Margins (-1, 0, 0, 0),
			};

			this.wholeWordButton = new BackIconButton
			{
				Parent            = frameBox,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				BackColor         = Color.FromName ("White"),
				ActiveState       = ActiveState.Yes,
				AutoToggle        = false,
				AutoFocus         = false,
				Dock              = DockStyle.Right,
				Margins           = new Margins (-1, 0, 0, 0),
			};

			this.matchCaseButton = new BackIconButton
			{
				Parent            = frameBox,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				BackColor         = Color.FromName ("White"),
				ActiveState       = ActiveState.Yes,
				AutoToggle        = false,
				AutoFocus         = false,
				Dock              = DockStyle.Right,
				Margins           = new Margins (-1, 0, 0, 0),
			};

			this.modeButton = new BackIconButton
			{
				Parent            = frameBox,
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				BackColor         = Color.FromName ("White"),
				ActiveState       = ActiveState.Yes,
				AutoToggle        = false,
				AutoFocus         = false,
				Dock              = DockStyle.Right,
				Margins           = new Margins (-1, 0, 0, 0),
			};

			UIBuilder.CreateMarker (this.modeButton, "Search.Mode.Marker");

			ToolTip.Default.SetToolTip (this.modeButton,      "Mode de recherche");
			ToolTip.Default.SetToolTip (this.matchCaseButton, "Respecter la casse");
			ToolTip.Default.SetToolTip (this.wholeWordButton, "Mot entier");
			ToolTip.Default.SetToolTip (this.invertButton,    "Inverser");

			this.UpdateFields ();
			this.UpdateButtons ();

			//	Connexions des événements.
			this.intervalButton.Clicked += delegate
			{
				if (this.tabData.SearchText.Mode == SearchMode.Interval)
				{
					this.tabData.SearchText.Mode = SearchMode.Fragment;
				}
				else
				{
					this.tabData.SearchText.Mode = SearchMode.Interval;
				}

				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.searchField1.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
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
				if (this.ignoreChanges.IsZero)
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

			this.columnsField.Clicked += delegate
			{
				this.ShowColumnsMenu (this.columnsFrame);
			};

			this.columnsButton.Clicked += delegate
			{
				this.ShowColumnsMenu (this.columnsFrame);
			};

			this.modeButton.Clicked += delegate
			{
				this.ShowModeMenu (this.modeButton);
			};

			this.matchCaseButton.Clicked += delegate
			{
				this.tabData.SearchText.MatchCase = !this.tabData.SearchText.MatchCase;
				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.wholeWordButton.Clicked += delegate
			{
				this.tabData.SearchText.WholeWord = !this.tabData.SearchText.WholeWord;
				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.invertButton.Clicked += delegate
			{
				this.tabData.SearchText.Invert = !this.tabData.SearchText.Invert;
				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.activateButton.Clicked += delegate
			{
				this.tabData.Active = !this.tabData.Active;
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.addRemoveButton.Clicked += delegate
			{
				addRemoveAction (this.index);
			};

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

		private void CreateLabelUI(FrameBox parent)
		{
			this.titleFrame = new FrameBox
			{
				Parent           = parent,
				PreferredWidth   = UIBuilder.LeftLabelWidth+1-10,
				PreferredHeight  = 20,
				Dock             = DockStyle.Top,
				Margins          = new Margins (0, 10, 0, 0),
			};
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
		}

		public void SetAddAction(int index, bool enable)
		{
			bool add = (index == 0);

			if (this.index != index || this.addAction != add || this.addActionEnable != enable)
			{
				this.index           = index;
				this.addAction       = add;
				this.addActionEnable = enable;

				this.UpdateButtons ();
			}
		}


		private void UpdateFields()
		{
			this.searchToFrame.Visibility = (this.tabData.SearchText.Mode == SearchMode.Interval);

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

		public void UpdateButtons()
		{
			this.titleFrame.Children.Clear ();

			if (this.addAction)
			{
				if (this.parentController.Index == 0)
				{
					this.CreateOrAndText (this.isFilter ? "Filtrer" : "Rechercher");
				}
				else
				{
					var text = this.CreateOrAndText (this.isFilter ? "filtrer" : "rechercher");
					text.PreferredWidth = this.isFilter ? 28 : 53;  // TODO: faire mieux !
					text.Dock = DockStyle.Right;

					this.CreateOrAndButton (this.parentController.ParentController.Data.OrMode ? "Ou" : "Et", node: true);
				}
			}
			else
			{
				this.CreateOrAndButton (this.parentController.NodeData.OrMode ? "ou" : "et", node: false);
			}

			this.activateButton.ActiveState = this.tabData.Active ? ActiveState.Yes : ActiveState.No;

			this.addRemoveButton.IconUri = UIBuilder.GetResourceIconUri (this.addAction ? "Search.AddTab" : "Search.SubTab");
			this.addRemoveButton.Enable = !this.addAction || this.addActionEnable;

			if (this.addAction)
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Ajoute un nouveau critère de filtre" : "Ajoute un nouveau critère de recherche");
			}
			else
			{
				ToolTip.Default.SetToolTip (this.addRemoveButton, this.isFilter ? "Supprime le critère de filtre" : "Supprime le critère de recherche");
			}

			bool interval     = this.tabData.SearchText.Mode == SearchMode.Interval;
			bool wholeContent = this.tabData.SearchText.Mode == SearchMode.WholeContent;
			bool jokers       = this.tabData.SearchText.Mode == SearchMode.Jokers;
			bool empty        = this.tabData.SearchText.Mode == SearchMode.Empty;

			this.intervalButton.IconUri  = UIBuilder.GetResourceIconUri (interval                          ? "Search.Interval.True"  : "Search.Interval.False");
			this.matchCaseButton.IconUri = UIBuilder.GetResourceIconUri (this.tabData.SearchText.MatchCase ? "Search.MatchCase.True" : "Search.MatchCase.False");
			this.wholeWordButton.IconUri = UIBuilder.GetResourceIconUri (this.tabData.SearchText.WholeWord ? "Search.WholeWord.True" : "Search.WholeWord.False");
			this.invertButton.IconUri    = UIBuilder.GetResourceIconUri (this.tabData.SearchText.Invert    ? "Search.Invert.True"    : "Search.Invert.False");
			this.modeButton.IconUri      = UIBuilder.GetResourceIconUri (SearchTabController.GetModeIcon (this.tabData.SearchText.Mode));

			this.modeButton.Visibility      = !interval;
			this.matchCaseButton.Visibility = this.tabData.SearchText.Additionnal && !interval && !empty;
			this.wholeWordButton.Visibility = this.tabData.SearchText.Additionnal && !interval && !empty && !wholeContent && !jokers;
			this.invertButton.Visibility    = this.tabData.SearchText.Additionnal && this.isFilter;

			var c = this.tabData.GetColumnsSummary (this.columnMappers);
			if (c.IsNullOrEmpty ())
			{
				c = "Partout";
			}
			this.columnsField.FormattedText = c;
		}

		private Button CreateOrAndButton(FormattedText text, bool node)
		{
			//	Crée un bouton discret affichant juste le texte "ou"/"et", qui ne se dévoile que lorsque la souris le survole.
			var button = new Button
			{
				Parent          = this.titleFrame,
				ButtonStyle     = ButtonStyle.ToolItem,
				FormattedText   = text,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
			};

			button.Clicked += delegate
			{
				if (node)
				{
					this.swapNodeAction ();
				}
				else
				{
					this.swapTabAction ();
				}

				this.UpdateButtons ();
			};

			ToolTip.Default.SetToolTip (button, "Permute les modes \"et\"/\"ou\"");

			return button;
		}

		private StaticText CreateOrAndText(FormattedText text)
		{
			return new StaticText
			{
				Parent           = this.titleFrame,
				FormattedText    = text,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredHeight  = 20,
				Dock             = DockStyle.Fill,
			};
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

			if (this.tabData.SearchText.Additionnal)
			{
				this.AddModeToMenu (menu, SearchMode.Empty);
				this.AddModeToMenu (menu, SearchMode.Jokers);
			}

			menu.Items.Add (new MenuSeparator ());
			this.AddModeToMenu (menu, "Modes additionnels", () => this.tabData.SearchText.Additionnal, x => this.tabData.SearchText.Additionnal = x);

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddModeToMenu(VMenu menu, SearchMode mode)
		{
			bool selected = (this.tabData.SearchText.Mode == mode);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetRadioStateIconUri (selected),
				FormattedText = UIBuilder.GetIconTag (SearchTabController.GetModeIcon (mode), verticalOffset: -4) + "  " + SearchTabController.GetModeDescription (mode),
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				this.tabData.SearchText.Mode = (SearchMode) System.Enum.Parse (typeof (SearchMode), item.Name);
				
				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}

		private void AddModeToMenu(VMenu menu, FormattedText text, System.Func<bool> getter, System.Action<bool> setter)
		{
			bool selected = getter ();

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (selected),
				FormattedText = text,
			};

			item.Clicked += delegate
			{
				setter (!getter ());

				this.UpdateFields ();
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}
		#endregion


		#region Columns menu
		private void ShowColumnsMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir les colonnes.
			var menu = new VMenu ();

			this.AddColumnToMenu (menu);
			menu.Items.Add (new MenuSeparator ());

			for (int i = 0; i < this.columnMappers.Count; i++)
			{
				var mapper = this.columnMappers[i];

				if (!mapper.HideForSearch && mapper.RelativeWidth != 0 && !mapper.Description.IsNullOrEmpty () && (this.isFilter || mapper.Show))
				{
					FormattedText desc = mapper.Description;

					if (desc.IsNullOrEmpty ())
					{
						desc = string.Format ("colonne n° {0}", (i+1).ToString ());
					}

					bool selected = this.tabData.Columns.Contains (mapper.Column);

					this.AddColumnToMenu (menu, "Dans " + desc, mapper.Column, selected);
				}
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddColumnToMenu(VMenu menu)
		{
			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (!this.tabData.Columns.Any ()),
				FormattedText = "Partout",
			};

			item.Clicked += delegate
			{
				this.tabData.Columns.Clear ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}

		private void AddColumnToMenu(VMenu menu, FormattedText text, ColumnType columnType, bool selected)
		{
			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (selected),
				FormattedText = text,
				Name          = columnType.ToString (),
			};

			item.Clicked += delegate
			{
				var type = (ColumnType) System.Enum.Parse (typeof (ColumnType), item.Name);

				if (this.tabData.Columns.Contains (type))
				{
					this.tabData.Columns.Remove (type);
				}
				else
				{
					this.tabData.Columns.Add (type);
					this.SortColumns ();
				}

				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}

		private void SortColumns()
		{
			var sorted = new List<ColumnType> ();

			foreach (var mapper in this.columnMappers)
			{
				if (this.tabData.Columns.Contains (mapper.Column))
				{
					sorted.Add (mapper.Column);
				}
			}

			this.tabData.Columns.Clear ();
			this.tabData.Columns.AddRange (sorted);
		}
		#endregion


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

		private static string GetModeIcon(SearchMode mode)
		{
			switch (mode)
			{
				case SearchMode.Fragment:
					return "Search.Mode.Fragment";

				case SearchMode.StartsWith:
					return "Search.Mode.StartsWith";

				case SearchMode.EndsWith:
					return "Search.Mode.EndsWith";

				case SearchMode.WholeContent:
					return "Search.Mode.WholeContent";

				case SearchMode.Jokers:
					return "Search.Mode.Jokers";

				case SearchMode.Empty:
					return "Search.Mode.Empty";

				default:
					return null;
			}
		}


		private readonly AbstractController				controller;
		private readonly List<ColumnMapper>				columnMappers;
		private readonly SearchNodeController			parentController;
		private readonly SearchTabData					tabData;
		private readonly bool							isFilter;
		private readonly SafeCounter					ignoreChanges;

		private System.Action							searchStartAction;
		private System.Action							swapNodeAction;
		private System.Action							swapTabAction;
		private int										index;
		private FrameBox								titleFrame;
		private BackIconButton							intervalButton;
		private FrameBox								searchFromFrame;
		private TextField								searchField1;
		private TextFieldEx								searchFieldEx1;
		private FrameBox								searchToFrame;
		private TextField								searchField2;
		private TextFieldEx								searchFieldEx2;
		private BackIconButton							modeButton;
		private BackIconButton							matchCaseButton;
		private BackIconButton							wholeWordButton;
		private BackIconButton							invertButton;
		private FrameBox								columnsFrame;
		private StaticText								columnsField;
		private GlyphButton								columnsButton;
		private BackIconButton							activateButton;
		private IconButton								addRemoveButton;

		private bool									bigDataInterface;
		private bool									addAction;
		private bool									addActionEnable;
	}
}
