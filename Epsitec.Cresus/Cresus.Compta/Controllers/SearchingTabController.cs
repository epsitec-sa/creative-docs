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
			var frameBox = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.searchingField = new TextField
			{
				Parent          = frameBox,
				Text            = this.tabData.SearchingText,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
				Visibility      = !bigDataInterface,
			};

			this.searchingFieldEx = new TextFieldEx
			{
				Parent                       = frameBox,
				Text                         = this.tabData.SearchingText,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Fill,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex                     = 1,
				Visibility                   = bigDataInterface,
			};

			this.addRemoveButton = new GlyphButton
			{
				Parent          = frameBox,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
			};

			this.columnField = new TextFieldCombo
			{
				Parent          = frameBox,
				IsReadOnly      = true,
				PreferredWidth  = 140,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Margins         = new Margins (1, 0, 0, 0),
				TabIndex        = 2,
			};

			this.InitializeCombo ();
			this.UpdateButtons ();

			this.searchingField.TextChanged += delegate
			{
				this.tabData.SearchingText = this.searchingField.Text;
				searchStartAction ();
			};

			this.searchingFieldEx.EditionAccepted += delegate
			{
				this.tabData.SearchingText = this.searchingFieldEx.Text;
				searchStartAction ();
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

					searchStartAction ();
				}
			};

			this.addRemoveButton.Clicked += delegate
			{
				addRemoveAction (this.Index);
			};

			string textField = "Critère de recherche<br/>Pour un invervalle, donnez deux nombres séparés par un espace";
			ToolTip.Default.SetToolTip (this.searchingField,   textField);
			ToolTip.Default.SetToolTip (this.searchingFieldEx, textField);
			ToolTip.Default.SetToolTip (this.columnField,      "Colonne où chercher");

			return frameBox;
		}

		public void SetFocus()
		{
			if (this.searchingFieldEx.Visibility)
			{
				this.searchingFieldEx.Focus ();
			}
			else
			{
				this.searchingField.Focus ();
			}
		}

		public void UpdateColumns(List<ColumnMapper> columnMappers)
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.columnMappers = columnMappers;

			this.InitializeCombo ();
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


		private void InitializeCombo()
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

		private void UpdateButtons()
		{
			this.addRemoveButton.GlyphShape = this.addAction ? GlyphShape.Plus : GlyphShape.Minus;

			ToolTip.Default.SetToolTip (this.addRemoveButton, this.addAction ? "Ajoute un nouveau critère de recherche" : "Supprime le critère de recherche");
		}


		private readonly SearchingTabData		tabData;
		private readonly List<int>				columnIndexes;

		private List<ColumnMapper>				columnMappers;
		private TextField						searchingField;
		private TextFieldEx						searchingFieldEx;
		private TextFieldCombo					columnField;
		private GlyphButton						addRemoveButton;

		private bool							addAction;
		private bool							ignoreChange;
	}
}
