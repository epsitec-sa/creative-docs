//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class SearchingTabController
	{
		public SearchingTabController(SearchingTabData tabData, List<ColumnMapper> columnMappers)
		{
			this.tabData       = tabData;
			this.columnMappers = columnMappers;

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
				this.searchingFromFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 1,
				};

				this.searchingFromLabel = new StaticText
				{
					Parent          = this.searchingFromFrame,
					Text            = "de",
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
				};

				this.searchingField1 = new TextField
				{
					Parent          = this.searchingFromFrame,
					Text            = this.tabData.SearchingText.FromText,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 1,
				};

				this.searchingFieldEx1 = new TextFieldEx
				{
					Parent                       = this.searchingFromFrame,
					Text                         = this.tabData.SearchingText.FromText,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Fill,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = 1,
				};
			}

			{
				this.searchingToFrame = new FrameBox
				{
					Parent          = frameBox,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (1, 0, 0, 0),
					TabIndex        = 2,
				};

				this.searchingToLabel = new StaticText
				{
					Parent          = this.searchingToFrame,
					Text            = "à",
					PreferredWidth  = 12,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (5, 0, 0, 0),
				};

				this.searchingField2 = new TextField
				{
					Parent          = this.searchingToFrame,
					Text            = this.tabData.SearchingText.ToText,
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					TabIndex        = 2,
				};

				this.searchingFieldEx2 = new TextFieldEx
				{
					Parent                       = this.searchingToFrame,
					Text                         = this.tabData.SearchingText.ToText,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Fill,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = 2,
				};
			}

			this.addRemoveButton = new GlyphButton
			{
				Parent          = frameBox,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (5, 0, 0, 0),
			};

			this.modeField = new TextField
			{
				Parent          = frameBox,
				Text            = this.ModeDescription,
				IsReadOnly      = true,
				PreferredWidth  = 110,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
				TabIndex        = 4,
			};

			this.columnField = new TextFieldCombo
			{
				Parent          = frameBox,
				IsReadOnly      = true,
				PreferredWidth  = 130,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
				TabIndex        = 3,
			};

			this.InitializeColumnsCombo ();
			this.UpdateFields ();
			this.UpdateButtons ();

			this.searchingField1.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.tabData.SearchingText.FromText = this.searchingField1.Text;
					this.searchStartAction ();
				}
			};

			this.searchingFieldEx1.EditionAccepted += delegate
			{
				this.tabData.SearchingText.FromText = this.searchingFieldEx1.Text;
				this.searchStartAction ();
			};

			this.searchingField2.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.tabData.SearchingText.ToText = this.searchingField2.Text;
					this.searchStartAction ();
				}
			};

			this.searchingFieldEx2.EditionAccepted += delegate
			{
				this.tabData.SearchingText.ToText = this.searchingFieldEx2.Text;
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
				this.ShowModeMenu (this.modeField);
			};

			this.addRemoveButton.Clicked += delegate
			{
				addRemoveAction (this.Index);
			};

			ToolTip.Default.SetToolTip (this.columnField, "Colonne où chercher ?");
			ToolTip.Default.SetToolTip (this.modeField,   "Comment chercher ?");

			return frameBox;
		}

		public void SetFocus()
		{
			if (this.searchingFieldEx1.Visibility)
			{
				this.searchingFieldEx1.Focus ();
			}
			else
			{
				this.searchingField1.Focus ();
			}
		}

		public void UpdateColumns(List<ColumnMapper> columnMappers)
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.columnMappers = columnMappers;

			this.InitializeColumnsCombo ();
		}

		public bool AddAction
		{
			get
			{
				return this.addAction;
			}
			set
			{
				if (this.addAction != value)
				{
					this.addAction = value;
					this.UpdateButtons ();
				}
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

				if (!mapper.HideForSearch)
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
			this.searchingToFrame.Visibility   = (this.tabData.SearchingText.Mode == SearchingMode.Interval);
			this.searchingFromLabel.Visibility = (this.tabData.SearchingText.Mode == SearchingMode.Interval);
			this.searchingToLabel.Visibility   = (this.tabData.SearchingText.Mode == SearchingMode.Interval);

			this.searchingField1.Visibility   = !this.bigDataInterface;
			this.searchingFieldEx1.Visibility =  this.bigDataInterface;
			this.searchingField2.Visibility   = !this.bigDataInterface;
			this.searchingFieldEx2.Visibility =  this.bigDataInterface;

			this.searchingField1.IsReadOnly = (this.tabData.SearchingText.Mode == SearchingMode.Empty);
			this.searchingFieldEx1.IsReadOnly = (this.tabData.SearchingText.Mode == SearchingMode.Empty);

			string textField;

			if (this.tabData.SearchingText.Mode == SearchingMode.Interval)
			{
				textField = "Que chercher (depuis, inclu) ?";
				ToolTip.Default.SetToolTip (this.searchingField1, textField);
				ToolTip.Default.SetToolTip (this.searchingFieldEx1, textField);

				textField = "Que chercher (jusqu'à, inclu) ?";
				ToolTip.Default.SetToolTip (this.searchingField2, textField);
				ToolTip.Default.SetToolTip (this.searchingFieldEx2, textField);
			}
			else if (this.tabData.SearchingText.Mode == SearchingMode.Empty)
			{
				this.searchingField1.Text   = null;
				this.searchingFieldEx1.Text = null;

				textField = "Cherche les données vides";
				ToolTip.Default.SetToolTip (this.searchingField1,   textField);
				ToolTip.Default.SetToolTip (this.searchingFieldEx1, textField);
			}
			else
			{
				textField = "Que chercher ?";
				ToolTip.Default.SetToolTip (this.searchingField1,   textField);
				ToolTip.Default.SetToolTip (this.searchingFieldEx1, textField);
			}
		}

		private void UpdateButtons()
		{
			this.addRemoveButton.GlyphShape = this.addAction ? GlyphShape.Plus : GlyphShape.Minus;

			ToolTip.Default.SetToolTip (this.addRemoveButton, this.addAction ? "Ajoute un nouveau critère de recherche" : "Supprime le critère de recherche");
		}


		#region Mode menu
		private void ShowModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode pour le ruban.
			var menu = new VMenu ();

			this.AddModeToMenu (menu, SearchingMode.Fragment);
			this.AddModeToMenu (menu, SearchingMode.WholeWord);
			this.AddModeToMenu (menu, SearchingMode.StartsWith);
			this.AddModeToMenu (menu, SearchingMode.EndsWith);
			this.AddModeToMenu (menu, SearchingMode.WholeContent);
			this.AddModeToMenu (menu, SearchingMode.Interval);
			this.AddModeToMenu (menu, SearchingMode.Empty);

			if (this.tabData.SearchingText.Mode != SearchingMode.Interval &&
				this.tabData.SearchingText.Mode != SearchingMode.Empty    )
			{
				menu.Items.Add (new MenuSeparator ());
				this.AddModeToMenu (menu, "a ≠ A", () => !this.tabData.SearchingText.ConvertCase, x => this.tabData.SearchingText.ConvertCase = !x);
				this.AddModeToMenu (menu, "é ≠ e", () => !this.tabData.SearchingText.ConvertDiacritics, x => this.tabData.SearchingText.ConvertDiacritics = !x);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddModeToMenu(VMenu menu, SearchingMode mode)
		{
			bool selected = (this.tabData.SearchingText.Mode == mode);

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (selected ? "Button.RadioYes" : "Button.RadioNo"),
				FormattedText = SearchingTabController.GetModeDescription (mode),
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				this.tabData.SearchingText.Mode = (SearchingMode) System.Enum.Parse (typeof (SearchingMode), item.Name);
				
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

		private string ModeDescription
		{
			get
			{
				string text = SearchingTabController.GetModeDescription (this.tabData.SearchingText.Mode);

				if (this.tabData.SearchingText.Mode != SearchingMode.Interval &&
					this.tabData.SearchingText.Mode != SearchingMode.Empty    )
				{
					if (!this.tabData.SearchingText.ConvertCase      ||
						!this.tabData.SearchingText.ConvertDiacritics)
					{
						text += " (";

						if (!this.tabData.SearchingText.ConvertCase)
						{
							text += "a≠A";
						}

						if (!this.tabData.SearchingText.ConvertDiacritics)
						{
							if (text[text.Length-1] != '(')
							{
								text += " ";
							}

							text += "é≠e";
						}

						text += ")";
					}
				}

				return text;
			}
		}

		private static string GetModeDescription(SearchingMode mode)
		{
			switch (mode)
			{
				case SearchingMode.Fragment:
					return "Fragment";

				case SearchingMode.WholeWord:
					return "Mot entier";

				case SearchingMode.StartsWith:
					return "Au début";

				case SearchingMode.EndsWith:
					return "À la fin";

				case SearchingMode.WholeContent:
					return "Tout";

				case SearchingMode.Interval:
					return "Intervalle";

				case SearchingMode.Empty:
					return "Vide";

				default:
					return "?";

			}
		}
		#endregion


		private readonly SearchingTabData		tabData;
		private readonly List<int>				columnIndexes;

		private List<ColumnMapper>				columnMappers;
		private System.Action					searchStartAction;
		private FrameBox						searchingFromFrame;
		private StaticText						searchingFromLabel;
		private TextField						searchingField1;
		private TextFieldEx						searchingFieldEx1;
		private FrameBox						searchingToFrame;
		private StaticText						searchingToLabel;
		private TextField						searchingField2;
		private TextFieldEx						searchingFieldEx2;
		private TextFieldCombo					columnField;
		private TextField						modeField;
		private GlyphButton						addRemoveButton;

		private bool							bigDataInterface;
		private bool							addAction;
		private bool							ignoreChange;
	}
}
