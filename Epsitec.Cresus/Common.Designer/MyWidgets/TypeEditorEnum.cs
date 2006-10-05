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
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
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
			Types.Collections.EnumValueCollection collection = this.Collection;

			this.array.TotalRows = collection.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < collection.Count)
				{
					EnumValue value = collection[first+i];
					Caption caption = value.Caption;
					string name = caption.Name;
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

			int sel = this.array.SelectedRow;

			Druid druid = Druid.Empty;
			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Values, druid);

			EnumValue item = new EnumValue(0, druid);
			collection.Insert(sel+1, item);

			this.array.SelectedRow = sel+1;
			this.array.ShowSelectedRow();

			this.UpdateArray();
			this.UpdateButtons();
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

			if (sel > collection.Count-1)
			{
				sel = collection.Count-1;
			}
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

			this.UpdateArray();
			this.UpdateButtons();
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

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();

			this.UpdateArray();
			this.UpdateButtons();
		}


		protected Types.Collections.EnumValueCollection Collection
		{
			//	Retourne la collection de l'énumération.
			get
			{
				EnumType type = this.type as EnumType;
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
	}
}
