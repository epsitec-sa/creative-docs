using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorEnum : AbstractTypeEditor
	{
		public TypeEditorEnum()
		{
			//	Cr�e la toolbar.
			this.toolbar = new HToolBar(this);
			this.toolbar.Dock = DockStyle.StackBegin;

			this.buttonCreate = new IconButton();
			this.buttonCreate.CaptionId = Res.Captions.Editor.Type.Create.Id;
			this.buttonCreate.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonCreate);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonAdd = new IconButton();
			this.buttonAdd.CaptionId = Res.Captions.Editor.Type.Add.Id;
			this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonAdd);

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionId = Res.Captions.Editor.Type.Remove.Id;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionId = Res.Captions.Editor.Type.Prev.Id;
			this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionId = Res.Captions.Editor.Type.Next.Id;
			this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonSort = new IconButton();
			this.buttonSort.CaptionId = Res.Captions.Editor.Type.Sort.Id;
			this.buttonSort.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSort);

			this.buttonGoto = this.CreateLocatorGotoButton(null);
			this.buttonGoto.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonGoto);

			this.fieldSearch = new TextFieldCombo();
			this.fieldSearch.PreferredWidth = 200;
			this.fieldSearch.Margins = new Margins(30, 0, 1, 1);
			Misc.ComboMenuFromList(this.fieldSearch, TypeEditorEnum.fieldSearchList);
			this.toolbar.Items.Add(this.fieldSearch);

			this.buttonSearchPrev = new IconButton();
			this.buttonSearchPrev.CaptionId = Res.Captions.Editor.Type.SearchPrev.Id;
			this.buttonSearchPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSearchPrev);

			this.buttonSearchNext = new IconButton();
			this.buttonSearchNext.CaptionId = Res.Captions.Editor.Type.SearchNext.Id;
			this.buttonSearchNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonSearchNext);

			this.slider = new HSlider(toolbar);
			this.slider.PreferredWidth = 80;
			this.slider.Margins = new Margins(2, 2, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 50.0M;
			this.slider.SmallChange = 5.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			this.slider.Value = (decimal) TypeEditorEnum.arrayLineHeight;
			this.slider.Dock = DockStyle.Right;

			//	Cr�e l'en-t�te du tableau.
			this.header = new FrameBox(this);
			this.header.Dock = DockStyle.StackBegin;
			this.header.Margins = new Margins(0, 0, 4, 0);

			this.headerUse = new HeaderButton(this.header);
			this.headerUse.Text = "";
			this.headerUse.Style = HeaderButtonStyle.Top;
			this.headerUse.Dock = DockStyle.Left;

			this.headerName = new HeaderButton(this.header);
			this.headerName.Text = Res.Strings.Viewers.Types.Enum.RowName;
			this.headerName.Style = HeaderButtonStyle.Top;
			this.headerName.Dock = DockStyle.Left;

			this.headerDescription = new HeaderButton(this.header);
			this.headerDescription.Text = Res.Strings.Viewers.Types.Enum.RowDescription;
			this.headerDescription.Style = HeaderButtonStyle.Top;
			this.headerDescription.Dock = DockStyle.Left;

			this.headerIcon = new HeaderButton(this.header);
			this.headerIcon.Text = Res.Strings.Viewers.Types.Enum.RowIcon;
			this.headerIcon.Style = HeaderButtonStyle.Top;
			this.headerIcon.Dock = DockStyle.Left;

			//	Cr�e le tableau principal.
			this.array = new StringArray(this);
			this.array.Columns = 4;
			this.array.SetColumnsRelativeWidth(0, 0.05);
			this.array.SetColumnsRelativeWidth(1, 0.35);
			this.array.SetColumnsRelativeWidth(2, 0.50);
			this.array.SetColumnsRelativeWidth(3, 0.10);
			this.array.SetColumnAlignment(0, ContentAlignment.MiddleCenter);
			this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(2, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(3, ContentAlignment.MiddleCenter);
			this.array.SetColumnBreakMode(1, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.array.LineHeight = TypeEditorEnum.arrayLineHeight;
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 220;
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
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
				this.buttonCreate.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonAdd.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonPrev.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonNext.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonRemove.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonSort.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonGoto.Clicked -= new MessageEventHandler(this.HandleButtonClicked);

				this.slider.ValueChanged -= new EventHandler(this.HandleSliderChanged);

				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged -= new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du r�sum�.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

#if true
			object value = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				IList<StructuredData> list = value as IList<StructuredData>;
				builder.Append(list.Count.ToString());
				builder.Append("�: ");

				for (int i=0; i<list.Count; i++)
				{
					StructuredData data = list[i];

					Druid druid = (Druid) data.GetValue(Support.Res.Fields.EnumValue.CaptionId);
					Module module = this.designerApplication.SearchModule(druid);
					if (module != null)
					{
						CultureMap cultureMap = module.AccessValues2.Accessor.Collection[druid];
						string name = (cultureMap == null) ? "" : cultureMap.Name;
						builder.Append(name);
					}

					if (i < list.Count-1)
					{
						builder.Append(", ");
					}
				}
			}
#else
			builder.Append(this.selDruids.Count.ToString());
			builder.Append("�: ");

			for (int i=0; i<this.selDruids.Count; i++)
			{
				Druid druid = this.selDruids[i];
				Caption caption = this.module.ResourceManager.GetCaption(druid);

				string icon = caption.Icon;
				if (string.IsNullOrEmpty(icon))
				{
					builder.Append(ResourceAccess.GetCaptionShortDescription(caption));
				}
				else
				{
					builder.Append(Misc.ImageFull(icon, -5));
				}

				if (i < this.selDruids.Count-1)
				{
					builder.Append(", ");
				}
			}
#endif
			
			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
#if true
			this.allDruids = new List<Druid>();
			this.selDruids = new List<Druid>();
			this.listDruids = new List<Druid>();

			CollectionView collection = this.module.AccessValues2.CollectionView;

			for (int i=0; i<collection.Count; i++)
			{
				CultureMap item = collection.Items[i] as CultureMap;

				if (item.Prefix == this.cultureMap.Name)
				{
					this.allDruids.Add(item.Id);
				}
			}

			object value = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				IList<StructuredData> list = value as IList<StructuredData>;
				for (int i=0; i<list.Count; i++)
				{
					StructuredData data = list[i];

					Druid druid = (Druid) data.GetValue(Support.Res.Fields.EnumValue.CaptionId);
					this.selDruids.Add(druid);
					this.listDruids.Add(druid);
				}
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
#else
			Types.Collections.EnumValueCollection collection = this.Collection;

			this.allDruids = new List<Druid>();
			this.selDruids = new List<Druid>();
			this.listDruids = new List<Druid>();

			//	Cherche tous les Druids de type Values existants.
			if (!this.IsNativeEnum)
			{
				this.resourceAccess.BypassFilterOpenAccess(ResourceAccess.Type.Values, TypeCode.Invalid, null, null);
				int count = this.resourceAccess.BypassFilterCount;
				for (int i=0; i<count; i++)
				{
					Druid druid = this.resourceAccess.BypassFilterGetDruid(i);
					Caption caption = this.module.ResourceManager.GetCaption(druid);
					if (caption != null)
					{
						if (!EnumType.GetIsNative(caption))
						{
							this.allDruids.Add(druid);
						}
					}
				}
				this.resourceAccess.BypassFilterCloseAccess();
			}

			//	Construit le contenu de la liste.
			foreach (EnumValue value in collection)
			{
				Druid druid = value.Caption.Id;
				if (druid.IsValid)
				{
					this.selDruids.Add(druid);
					this.listDruids.Add(druid);
				}
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
#endif
		}

		protected void UpdateButtons()
		{
			bool add    = false;
			bool remove = false;
			bool prev   = false;
			bool next   = false;
			bool lgoto  = false;

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				Druid druid = this.listDruids[sel];  // Druid de la ligne s�lectionn�e

				if (this.selDruids.Contains(druid))  // fait partie de l'�num�ration ?
				{
					remove = !this.IsNativeEnum;

					int index = this.selDruids.IndexOf(druid);
					prev = index > 0;
					next = index < this.selDruids.Count-1;
				}
				else  // pas dans l'�num�ration ?
				{
					add = !this.IsNativeEnum;
				}

				lgoto = druid.IsValid;
			}

			this.buttonAdd.Enable    = add;
			this.buttonRemove.Enable = remove;
			this.buttonPrev.Enable   = prev;
			this.buttonNext.Enable   = next;
			this.buttonSort.Enable   = !this.IsNativeEnum;
			this.buttonGoto.Enable   = lgoto;
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.listDruids.Count)
				{
					Druid druid = this.listDruids[first+i];
					Caption caption = this.module.ResourceManager.GetCaption(druid);

					bool active = this.selDruids.Contains(druid);

					this.array.SetLineString(0, first+i, active ? Misc.Image("TypeEnumYes") : "");
					this.array.SetLineState(0, first+i, StringList.CellState.Normal);

					string name = caption.Name;
					string text = ResourceAccess.GetCaptionNiceDescription(caption, this.array.LineHeight);

					this.array.SetLineString(1, first+i, name);
					this.array.SetLineState(1, first+i, StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, text);
					this.array.SetLineState(2, first+i, StringList.CellState.Normal);

					string icon = caption.Icon;
					if (string.IsNullOrEmpty(icon))
					{
						this.array.SetLineString(3, first+i, "");
					}
					else
					{
						this.array.SetLineString(3, first+i, Misc.ImageFull(icon));
					}
					this.array.SetLineState(3, first+i, StringList.CellState.Normal);
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

		protected void ArrayCreate()
		{
			//	Cr�e une nouvelle valeur dans l'�num�ration.
			Module module = this.module;
			string name = this.GetNewName();
			name = module.DesignerApplication.DlgResourceName(Dialogs.ResourceName.Operation.Create, Dialogs.ResourceName.Type.Value, name);
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			
			if (!Misc.IsValidLabel(ref name))
			{
				module.DesignerApplication.DialogError(Res.Strings.Error.Name.Invalid);
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				sel = this.array.TotalRows-1;
			}

			//	TODO: � partir d'ici, le code �crit est vraissemblablement tout faux, car je n'y comprends rien.
			//	Il s'agit de cr�er une nouvelle valeur pour une �num�ration...
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;

			Support.ResourceAccessors.AnyTypeResourceAccessor accessor = module.AccessTypes2.Accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
			CultureMap valueCultureMap = accessor.CreateValueItem(this.cultureMap);
			valueCultureMap.Name = name;

			IResourceAccessor valueAccessor = module.AccessValues2.Accessor;
			valueAccessor.Collection.Add(valueCultureMap);

			IDataBroker broker = accessor.GetDataBroker(this.structuredData, Support.Res.Fields.ResourceEnumType.Values.ToString());
			StructuredData newValue = broker.CreateData(this.cultureMap);

			Druid druid = valueCultureMap.Id;
			newValue.SetValue(Support.Res.Fields.EnumValue.CaptionId, druid);

			list.Insert(sel+1, newValue);

			this.BuildCollection();
			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans l'�num�ration.
			if (this.IsNativeEnum)
			{
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne s�lectionn�e
			System.Diagnostics.Debug.Assert(!this.selDruids.Contains(druid));
			this.selDruids.Add(druid);

			this.BuildCollection();
			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de l'�num�ration.
			if (this.IsNativeEnum)
			{
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne s�lectionn�e
			System.Diagnostics.Debug.Assert(this.selDruids.Contains(druid));
			this.selDruids.Remove(druid);

			this.BuildCollection();
			this.UpdateArray();
			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayMove(int direction)
		{
			//	D�place une valeur dans l'�num�ration.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			Druid druid = this.listDruids[sel];  // Druid de la ligne s�lectionn�e
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
			//	Met les valeurs de l'�num�ration en t�te de liste.
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

		protected void ArrayGoto()
		{
			int sel = this.array.SelectedRow;
			Druid druid = (sel == -1) ? Druid.Empty : this.listDruids[sel];

			Module module = this.designerApplication.SearchModule(druid);
			if (module == null)
			{
				return;
			}

			this.designerApplication.LocatorGoto(module.ModuleInfo.Name, ResourceAccess.Type.Values, -1, druid, this.Window.FocusedWidget);
		}

		protected void ArraySearch(string searching, int direction)
		{
			//	Cherche en avant ou en arri�re.
			searching = Searcher.RemoveAccent(searching.ToLower());

			int sel = this.array.SelectedRow;

			for (int i=0; i<this.listDruids.Count; i++)
			{
				sel += direction;  // suivant, en avant ou en arri�re

				if (sel >= this.listDruids.Count)  // fin d�pass�e ?
				{
					sel = 0;  // revient au d�but
				}

				if (sel < 0)  // d�but d�pass� ?
				{
					sel = this.listDruids.Count-1;  // va � la fin
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

			this.designerApplication.DialogMessage(Res.Strings.Dialog.Search.Message.Error);
		}

		protected void BuildCollection()
		{
			//	Construit toute la collection en fonction des ressources s�lectionn�es
			//	dans la liste puis renum�rote toute la collection.
#if true
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
			if (list != null)
			{
				list.Clear();

				Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.module.AccessTypes2.Accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
				foreach (Druid druid in this.selDruids)
				{
					IDataBroker dataBroker = accessor.GetDataBroker(this.structuredData, Support.Res.Fields.ResourceEnumType.Values.ToString());
					StructuredData dataValue = dataBroker.CreateData(this.cultureMap);
					dataValue.SetValue(Support.Res.Fields.EnumValue.CaptionId, druid);
					list.Add(dataValue);
				}
			}

			this.module.AccessTypes2.SetLocalDirty();
#else
			Types.Collections.EnumValueCollection collection = this.Collection;
			collection.Clear();

			int rank = 0;
			foreach (Druid druid in this.selDruids)
			{
				Caption caption = this.module.ResourceManager.GetCaption(druid);
				System.Diagnostics.Debug.Assert(caption != null);

				EnumValue item = new EnumValue(rank++, caption);
				collection.Add(item);
			}
#endif
		}


#if false
		protected Types.Collections.EnumValueCollection Collection
		{
			//	Retourne la collection de l'�num�ration.
			get
			{
				EnumType type = this.AbstractType as EnumType;
				if (type == null)
				{
					return null;
				}

				type.MakeEditable();
				return type.EnumValues;
			}
		}
#endif


		protected string GetNewName()
		{
			//	Cherche un nouveau nom jamais utilis�.
			for (int i=1; i<10000; i++)
			{
				string name = string.Format("Value", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(name))
				{
					return name;
				}
			}
			return null;
		}

		protected bool IsExistingName(string name)
		{
			//	Indique si un nom existe.
			CollectionView collection = this.module.AccessValues2.CollectionView;

			for (int i=0; i<collection.Count; i++)
			{
				CultureMap item = collection.Items[i] as CultureMap;

				if (item.Prefix == this.cultureMap.Name)
				{
					if (name == item.Name)
					{
						return true;
					}
				}
			}

			return false;
		}

		
		protected bool IsNativeEnum
		{
			//	Indique s'il s'agit d'une �num�ration native.
			get
			{
#if true
				return false;
#else
				EnumType type = this.AbstractType as EnumType;
				return type.IsNativeEnum;
#endif
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if (this.header == null)
			{
				return;
			}

			double w1 = this.array.GetColumnsAbsoluteWidth(0);
			double w2 = this.array.GetColumnsAbsoluteWidth(1);
			double w3 = this.array.GetColumnsAbsoluteWidth(2);
			double w4 = this.array.GetColumnsAbsoluteWidth(3);

			this.headerUse.PreferredWidth = w1;
			this.headerName.PreferredWidth = w2;
			this.headerDescription.PreferredWidth = w3;
			this.headerIcon.PreferredWidth = w4+1;
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonCreate)
			{
				this.ArrayCreate();
			}

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

			if (sender == this.buttonGoto)
			{
				this.ArrayGoto();
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
			//	Appel� lorsque le slider a �t� d�plac�.
			if (this.array == null)
			{
				return;
			}

			HSlider slider = sender as HSlider;
			TypeEditorEnum.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = TypeEditorEnum.arrayLineHeight;
		}

		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a chang�.
			this.UpdateClientGeometry();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a chang�.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
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
		protected IconButton					buttonCreate;
		protected IconButton					buttonAdd;
		protected IconButton					buttonRemove;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonSort;
		protected IconButton					buttonGoto;
		protected TextFieldCombo				fieldSearch;
		protected IconButton					buttonSearchPrev;
		protected IconButton					buttonSearchNext;
		protected HSlider						slider;

		protected FrameBox						header;
		protected HeaderButton					headerUse;
		protected HeaderButton					headerName;
		protected HeaderButton					headerDescription;
		protected HeaderButton					headerIcon;
		protected MyWidgets.StringArray			array;

		protected List<Druid>					allDruids;
		protected List<Druid>					selDruids;
		protected List<Druid>					listDruids;
	}
}
