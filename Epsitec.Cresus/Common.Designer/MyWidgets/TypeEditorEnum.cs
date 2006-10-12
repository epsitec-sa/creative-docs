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

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionDruid = Res.Captions.Editor.Type.Remove.Druid;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

			this.array = new StringArray(this);
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.4);
			this.array.SetColumnsRelativeWidth(1, 0.5);
			this.array.SetColumnsRelativeWidth(2, 0.1);
			this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
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

			//	Cherche tous les Druids de type Values existants.
			this.resourceAccess.BypassFilterOpenAccess(ResourceAccess.Type.Values, null);
			int count = this.resourceAccess.BypassFilterCount;
			this.allDruids = new List<Druid>(count);
			for (int i=0; i<count; i++)
			{
				this.allDruids.Add(this.resourceAccess.BypassFilterGetDruid(i));
			}
			this.resourceAccess.BypassFilterCloseAccess();

			//	Construit le contenu de la liste.
			this.listDruids = new List<Druid>();
			List<int> sels = new List<int>();

			int sel = 0;
			foreach (EnumValue value in collection)
			{
				this.listDruids.Add(value.Caption.Druid);
				sels.Add(sel++);
			}

			foreach (Druid druid in this.allDruids)
			{
				if (!this.listDruids.Contains(druid))
				{
					this.listDruids.Add(druid);
				}
			}

			this.array.TotalRows = this.listDruids.Count;
			this.array.AllowMultipleSelection = true;
			this.array.SelectedRows = sels;

			this.ignoreChange = true;
			this.UpdateArray();
			this.UpdateButtons();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			Types.Collections.EnumValueCollection collection = this.Collection;
			int sel = this.array.SelectedRow;

			this.buttonPrev.Enable = (sel != -1 && sel > 0);
			this.buttonNext.Enable = (sel != -1 && sel < collection.Count-1);
			this.buttonRemove.Enable = (sel != -1);
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.listDruids.Count)
				{
					Druid druid = this.listDruids[first+i];
					Caption caption = this.resourceAccess.DirectGetCaption(druid);

					//	Ne surtout pas utiliser caption.Name ou value.Name, car cette
					//	information n'est pas mise à jour pendant l'utilisation de
					//	Designer, mais seulement lors de l'enregistrement
					//	(dans ResourceAccess.AdjustBundlesBeforeSave).
					string name = this.resourceAccess.DirectGetDisplayName(druid);
					string text = ResourceAccess.GetCaptionNiceDescription(caption);

					this.array.SetLineString(0, first+i, name);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, text);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					string icon = caption.Icon;
					if (string.IsNullOrEmpty(icon))
					{
						this.array.SetLineString(2, first+i, "");
					}
					else
					{
						this.array.SetLineString(2, first+i, Misc.ImageFull(icon));
					}
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans l'énumération.
			Types.Collections.EnumValueCollection collection = this.Collection;

#if false
			int sel = this.array.SelectedRow;
			if (sel > collection.Count-1)
			{
				sel = collection.Count-1;
			}

			List<Druid> exclude = new List<Druid>();
			foreach (EnumValue value in collection)
			{
				exclude.Add(value.Caption.Druid);
			}

			Druid druid = Druid.Empty;
			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Values, druid, exclude);
			if (druid.IsEmpty)
			{
				return;
			}

			Caption caption = this.module.ResourceManager.GetCaption(druid);
			System.Diagnostics.Debug.Assert(caption != null);

			EnumValue item = new EnumValue(0, caption);
			collection.Insert(sel+1, item);
			this.RenumCollection();

			this.UpdateArray();
			this.UpdateButtons();

			this.array.SelectedRow = sel+1;
			this.array.ShowSelectedRow();

			this.OnContentChanged();
#else
			int sel = this.array.SelectedRow;
			if (sel > collection.Count-1)
			{
				sel = collection.Count-1;
			}

			List<Druid> druids = new List<Druid>();
			List<Druid> exclude = new List<Druid>();
			foreach (EnumValue value in collection)
			{
				exclude.Add(value.Caption.Druid);
			}

			druids = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Values, druids, exclude);
			if (druids == null)
			{
				return;
			}

			foreach (Druid druid in druids)
			{
				Caption caption = this.module.ResourceManager.GetCaption(druid);
				System.Diagnostics.Debug.Assert(caption != null);

				EnumValue item = new EnumValue(0, caption);
				sel++;
				collection.Insert(sel, item);
			}

			this.RenumCollection();

			this.UpdateArray();
			this.UpdateButtons();

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

			this.OnContentChanged();
#endif
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

			collection.RemoveAt(sel);
			this.RenumCollection();

			this.UpdateArray();
			this.UpdateButtons();

			if (sel > collection.Count-1)
			{
				sel = collection.Count-1;
			}
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

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

			EnumValue value = collection[sel];
			collection.RemoveAt(sel);
			collection.Insert(sel+direction, value);
			this.RenumCollection();

			this.UpdateArray();
			this.UpdateButtons();

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();

			this.OnContentChanged();
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

			if (sender == this.buttonPrev)
			{
				this.ArrayMove(-1);
			}

			if (sender == this.buttonNext)
			{
				this.ArrayMove(1);
			}

			if (sender == this.buttonRemove)
			{
				this.ArrayRemove();
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
		}


		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonRemove;
		protected MyWidgets.StringArray			array;
		protected List<Druid>					allDruids;
		protected List<Druid>					listDruids;
	}
}
