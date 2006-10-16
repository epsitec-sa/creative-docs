using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorEnum : AbstractTypeEditor
	{
		public TypeEditorEnum()
		{
			this.toolbar = new HToolBar(this);
			this.toolbar.Dock = DockStyle.StackBegin;

			this.buttonAdd = new IconButton();
			this.buttonAdd.CaptionDruid = Res.Captions.Editor.Type.Add.Druid;
			this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonAdd);

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionDruid = Res.Captions.Editor.Type.Remove.Druid;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionDruid = Res.Captions.Editor.Type.Prev.Druid;
			this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionDruid = Res.Captions.Editor.Type.Next.Druid;
			this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonSort = new IconButton();
			this.buttonSort.CaptionDruid = Res.Captions.Editor.Type.Sort.Druid;
			this.buttonSort.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSort);

			this.fieldSearch = new TextFieldCombo();
			this.fieldSearch.PreferredWidth = 200;
			this.fieldSearch.Margins = new Margins(30, 0, 1, 1);
			Misc.ComboMenuFromList(this.fieldSearch, TypeEditorEnum.fieldSearchList);
			this.toolbar.Items.Add(this.fieldSearch);

			this.buttonSearchPrev = new IconButton();
			this.buttonSearchPrev.CaptionDruid = Res.Captions.Editor.Type.SearchPrev.Druid;
			this.buttonSearchPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSearchPrev);

			this.buttonSearchNext = new IconButton();
			this.buttonSearchNext.CaptionDruid = Res.Captions.Editor.Type.SearchNext.Druid;
			this.buttonSearchNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSearchNext);

			HSlider slider = new HSlider(toolbar);
			slider.PreferredWidth = 80;
			slider.Margins = new Margins(2, 2, 4, 4);
			slider.MinValue = 20.0M;
			slider.MaxValue = 50.0M;
			slider.SmallChange = 5.0M;
			slider.LargeChange = 10.0M;
			slider.Resolution = 1.0M;
			slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			slider.Value = (decimal) TypeEditorEnum.arrayLineHeight;
			slider.Dock = DockStyle.Right;

			this.array = new StringArray(this);
			this.array.Columns = 4;
			this.array.SetColumnsRelativeWidth(0, 0.05);
			this.array.SetColumnsRelativeWidth(1, 0.40);
			this.array.SetColumnsRelativeWidth(2, 0.50);
			this.array.SetColumnsRelativeWidth(3, 0.05);
			this.array.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
			this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(2, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(3, ContentAlignment.MiddleCenter);
			this.array.LineHeight = TypeEditorEnum.arrayLineHeight;
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 360;
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
		}

		public TypeEditorEnum(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.buttonAdd.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonPrev.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonNext.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonRemove.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonSort.Clicked -= new MessageEventHandler(this.HandleButtonClicked);

				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			Types.Collections.EnumValueCollection collection = this.Collection;

			this.allDruids = new List<Druid>();
			this.selDruids = new List<Druid>();
			this.listDruids = new List<Druid>();

			//	Cherche tous les Druids de type Values existants.
			if (!this.IsNativeEnum)
			{
				this.resourceAccess.BypassFilterOpenAccess(ResourceAccess.Type.Values, null);
				int count = this.resourceAccess.BypassFilterCount;
				for (int i=0; i<count; i++)
				{
					Druid druid = this.resourceAccess.BypassFilterGetDruid(i);
					Caption caption = this.module.ResourceManager.GetCaption(druid);
					if (caption != null)
					{
						this.allDruids.Add(druid);
					}
				}
				this.resourceAccess.BypassFilterCloseAccess();
			}

			//	Construit le contenu de la liste.
			foreach (EnumValue value in collection)
			{
				this.selDruids.Add(value.Caption.Druid);
				this.listDruids.Add(value.Caption.Druid);
			}

			foreach (Druid druid in this.allDruids)
			{
				if (!this.listDruids.Contains(druid))
				{
					this.listDruids.Add(druid);
				}
			}

			this.array.TotalRows = this.listDruids.Count;
			this.array.FirstVisibleRow = 0;
			this.array.SelectedRow = -1;

			this.ignoreChange = true;
			this.UpdateArray();
			this.UpdateButtons();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			bool add    = false;
			bool remove = false;
			bool prev   = false;
			bool next   = false;

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				Druid druid = this.listDruids[sel];  // Druid de la ligne sélectionnée

				if (this.selDruids.Contains(druid))  // fait partie de l'énumération ?
				{
					remove = !this.IsNativeEnum;

					int index = this.selDruids.IndexOf(druid);
					prev = index > 0;
					next = index < this.selDruids.Count-1;
				}
				else  // pas dans l'énumération ?
				{
					add = !this.IsNativeEnum;
				}
			}

			this.buttonAdd.Enable    = add;
			this.buttonRemove.Enable = remove;
			this.buttonPrev.Enable   = prev;
			this.buttonNext.Enable   = next;
			this.buttonSort.Enable   = !this.IsNativeEnum;
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			Types.Collections.EnumValueCollection collection = this.Collection;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.listDruids.Count)
				{
					Druid druid = this.listDruids[first+i];
					Caption caption = this.module.ResourceManager.GetCaption(druid);

					bool active = this.selDruids.Contains(druid);
					StringList.CellState cs = active ? StringList.CellState.Normal : StringList.CellState.Unused;

					this.array.SetLineString(0, first+i, active ? Misc.Image("TypeEnumYes") : "");
					this.array.SetLineState(0, first+i, cs);

					//	Ne surtout pas utiliser caption.Name ou value.Name, car cette
					//	information n'est pas mise à jour pendant l'utilisation de
					//	Designer, mais seulement lors de l'enregistrement
					//	(dans ResourceAccess.AdjustBundlesBeforeSave).
					string name = this.resourceAccess.DirectGetDisplayName(druid);
					string text = ResourceAccess.GetCaptionNiceDescription(caption, this.array.LineHeight);

					this.array.SetLineString(1, first+i, name);
					this.array.SetLineState(1, first+i, cs);

					this.array.SetLineString(2, first+i, text);
					this.array.SetLineState(2, first+i, cs);

					string icon = caption.Icon;
					if (string.IsNullOrEmpty(icon))
					{
						this.array.SetLineString(3, first+i, "");
					}
					else
					{
						this.array.SetLineString(3, first+i, Misc.ImageFull(icon));
					}
					this.array.SetLineState(3, first+i, cs);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, StringList.CellState.Disabled);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, StringList.CellState.Disabled);

					this.array.SetLineString(3, first+i, "");
					this.array.SetLineState(3, first+i, StringList.CellState.Disabled);
				}
			}
		}

		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans l'énumération.
			if (this.IsNativeEnum)
			{
				return;
			}

			Types.Collections.EnumValueCollection collection = this.Collection;

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne sélectionnée
			System.Diagnostics.Debug.Assert(!this.selDruids.Contains(druid));
			this.selDruids.Add(druid);

			this.BuildCollection();
			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de l'énumération.
			if (this.IsNativeEnum)
			{
				return;
			}

			Types.Collections.EnumValueCollection collection = this.Collection;

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne sélectionnée
			System.Diagnostics.Debug.Assert(this.selDruids.Contains(druid));
			this.selDruids.Remove(druid);

			this.BuildCollection();
			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayMove(int direction)
		{
			//	Déplace une valeur dans l'énumération.
			Types.Collections.EnumValueCollection collection = this.Collection;

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne sélectionnée
			System.Diagnostics.Debug.Assert(this.selDruids.Contains(druid));

			int index = this.selDruids.IndexOf(druid);
			this.selDruids.RemoveAt(index);
			this.selDruids.Insert(index+direction, druid);

			this.BuildCollection();
			this.UpdateContent();

			this.array.SelectedRow = this.listDruids.IndexOf(druid);
			this.array.ShowSelectedRow();

			this.OnContentChanged();
		}

		protected void ArraySort()
		{
			//	Met les valeurs de l'énumération en tête de liste.
			Types.Collections.EnumValueCollection collection = this.Collection;

			int sel = this.array.SelectedRow;
			Druid druid = (sel == -1) ? Druid.Empty : this.listDruids[sel];

			this.UpdateContent();

			if (druid.IsEmpty)
			{
				this.array.FirstVisibleRow = 0;
			}
			else
			{
				this.array.SelectedRow = this.listDruids.IndexOf(druid);
				this.array.ShowSelectedRow();
			}

			this.OnContentChanged();
		}

		protected void ArraySearch(string searching, int direction)
		{
			//	Cherche en avant ou en arrière.
			searching = Searcher.RemoveAccent(searching.ToLower());

			int sel = this.array.SelectedRow;

			for (int i=0; i<this.listDruids.Count; i++)
			{
				sel += direction;  // suivant, en avant ou en arrière

				if (sel >= this.listDruids.Count)  // fin dépassée ?
				{
					sel = 0;  // revient au début
				}

				if (sel < 0)  // début dépassé ?
				{
					sel = this.listDruids.Count-1;  // va à la fin
				}

				string name = this.resourceAccess.DirectGetDisplayName(this.listDruids[sel]);
				name = Searcher.RemoveAccent(name.ToLower());

				if (name.Contains(searching))
				{
					this.array.SelectedRow = sel;
					this.array.ShowSelectedRow();
					return;
				}
			}

			this.mainWindow.DialogMessage(Res.Strings.Dialog.Search.Message.Error);
		}

		protected void BuildCollection()
		{
			//	Construit toute la collection en fonction des ressources sélectionnées
			//	dans la liste puis renumérote toute la collection.
			Types.Collections.EnumValueCollection collection = this.Collection;
			collection.Clear();

			foreach (Druid druid in this.selDruids)
			{
				Caption caption = this.module.ResourceManager.GetCaption(druid);
				System.Diagnostics.Debug.Assert(caption != null);

				EnumValue item = new EnumValue(0, caption);
				collection.Add(item);
			}

			for (int rank=0; rank<collection.Count; rank++)
			{
				EnumValue item = collection[rank];
				item.DefineRank(rank);
			}
		}


		protected Types.Collections.EnumValueCollection Collection
		{
			//	Retourne la collection de l'énumération.
			get
			{
				EnumType type = this.AbstractType as EnumType;
				type.MakeEditable();
				return type.EnumValues;
			}
		}

		protected bool IsNativeEnum
		{
			//	Indique s'il s'agit d'une énumération native.
			get
			{
				EnumType type = this.AbstractType as EnumType;
				return type.IsNativeEnum;
			}
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonAdd)
			{
				this.ArrayAdd();
			}

			if (sender == this.buttonRemove)
			{
				this.ArrayRemove();
			}

			if (sender == this.buttonPrev)
			{
				this.ArrayMove(-1);
			}

			if (sender == this.buttonNext)
			{
				this.ArrayMove(1);
			}

			if (sender == this.buttonSort)
			{
				this.ArraySort();
			}

			if (sender == this.buttonSearchPrev)
			{
				Misc.ComboMenuAdd(this.fieldSearch);
				Misc.ComboMenuToList(this.fieldSearch, TypeEditorEnum.fieldSearchList);
				this.ArraySearch(this.fieldSearch.Text, -1);
			}

			if (sender == this.buttonSearchNext)
			{
				Misc.ComboMenuAdd(this.fieldSearch);
				Misc.ComboMenuToList(this.fieldSearch, TypeEditorEnum.fieldSearchList);
				this.ArraySearch(this.fieldSearch.Text, 1);
			}
		}

		private void HandleSliderChanged(object sender)
		{
			//	Appelé lorsque le slider a été déplacé.
			if (this.array == null)
			{
				return;
			}

			HSlider slider = sender as HSlider;
			TypeEditorEnum.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = TypeEditorEnum.arrayLineHeight;
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();

			if (this.array.SelectedColumn == 0)
			{
				if (this.buttonAdd.Enable)
				{
					this.ArrayAdd();
				}
				else
				{
					this.ArrayRemove();
				}
			}
		}


		protected static double					arrayLineHeight = 20;
		protected static List<string>			fieldSearchList = new List<string>();

		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonRemove;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonSort;
		protected TextFieldCombo				fieldSearch;
		protected IconButton					buttonSearchPrev;
		protected IconButton					buttonSearchNext;
		protected MyWidgets.StringArray			array;
		protected List<Druid>					allDruids;
		protected List<Druid>					selDruids;
		protected List<Druid>					listDruids;
	}
}
