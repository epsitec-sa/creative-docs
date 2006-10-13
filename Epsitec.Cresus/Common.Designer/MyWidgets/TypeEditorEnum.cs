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
			this.array.LineHeight = 30;  // plus haut, à cause des descriptions et des icônes
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 200;
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
			this.resourceAccess.BypassFilterOpenAccess(ResourceAccess.Type.Values, null);
			int count = this.resourceAccess.BypassFilterCount;
			for (int i=0; i<count; i++)
			{
				this.allDruids.Add(this.resourceAccess.BypassFilterGetDruid(i));
			}
			this.resourceAccess.BypassFilterCloseAccess();

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
					remove = true;

					int index = this.selDruids.IndexOf(druid);
					prev = index > 0;
					next = index < this.selDruids.Count-1;
				}
				else  // hors de l'énumération ?
				{
					add = true;
				}
			}

			this.buttonAdd.Enable    = add;
			this.buttonRemove.Enable = remove;
			this.buttonPrev.Enable   = prev;
			this.buttonNext.Enable   = next;
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
					Caption caption = this.resourceAccess.DirectGetCaption(druid);

					bool active = this.selDruids.Contains(druid);
					this.array.SetLineString(0, first+i, active ? Misc.Image("TypeEnumYes") : "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					//	Ne surtout pas utiliser caption.Name ou value.Name, car cette
					//	information n'est pas mise à jour pendant l'utilisation de
					//	Designer, mais seulement lors de l'enregistrement
					//	(dans ResourceAccess.AdjustBundlesBeforeSave).
					string name = this.resourceAccess.DirectGetDisplayName(druid);
					string text = ResourceAccess.GetCaptionNiceDescription(caption);

					this.array.SetLineString(1, first+i, name);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, text);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

					string icon = caption.Icon;
					if (string.IsNullOrEmpty(icon))
					{
						this.array.SetLineString(3, first+i, "");
					}
					else
					{
						this.array.SetLineString(3, first+i, Misc.ImageFull(icon));
					}
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(3, first+i, "");
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans l'énumération.
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
			this.RenumCollection();

			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de l'énumération.
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
			this.RenumCollection();

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
			this.RenumCollection();

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

		protected void BuildCollection()
		{
			//	Renumérote toute la collection.
			Types.Collections.EnumValueCollection collection = this.Collection;
			collection.Clear();

			foreach (Druid druid in this.selDruids)
			{
				Caption caption = this.module.ResourceManager.GetCaption(druid);
				System.Diagnostics.Debug.Assert(caption != null);

				EnumValue item = new EnumValue(0, caption);
				collection.Add(item);
			}
		}

		protected void RenumCollection()
		{
			//	Renumérote toute la collection.
			Types.Collections.EnumValueCollection collection = this.Collection;

			for (int rank=0; rank<collection.Count; rank++)
			{
				EnumValue value = collection[rank];
				value.DefineRank(rank);
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


		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonRemove;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonSort;
		protected MyWidgets.StringArray			array;
		protected List<Druid>					allDruids;
		protected List<Druid>					selDruids;
		protected List<Druid>					listDruids;
	}
}
