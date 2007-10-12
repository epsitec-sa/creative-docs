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

			this.buttonDelete = new IconButton();
			this.buttonDelete.CaptionId = Res.Captions.Editor.Type.Delete.Id;
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonDelete);

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

			this.buttonGoto = this.CreateLocatorGotoButton(null);
			this.buttonGoto.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonGoto);

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
			this.array.Columns = 3;
			this.array.SetColumnsRelativeWidth(0, 0.40);
			this.array.SetColumnsRelativeWidth(1, 0.50);
			this.array.SetColumnsRelativeWidth(2, 0.10);
			this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);
			this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
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
				this.buttonDelete.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonPrev.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonNext.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
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
					//	La valeur est forc�ment dans le m�me module que l'�num�ration.
					CultureMap cultureMap = this.module.AccessValues.Accessor.Collection[druid];
					string name = (cultureMap == null) ? "" : cultureMap.Name;
					builder.Append(name);

					if (i < list.Count-1)
					{
						builder.Append(", ");
					}
				}
			}
			
			return builder.ToString();
		}

		public override void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
			this.array.SelectedRow = -1;
			this.UpdateArray();
			this.UpdateButtons();
		}


		protected void UpdateButtons()
		{
			//	Met � jour les boutons.
			bool native = this.IsNativeEnum;
			bool prev   = false;
			bool next   = false;
			bool lgoto  = false;

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				prev = sel > 0;
				next = sel < this.EnumCount-1;

				Druid druid;
				CultureMapSource source;
				this.GetDruid(sel, out druid, out source);  // Druid de la ligne s�lectionn�e

				lgoto = druid.IsValid;
			}

			this.buttonCreate.Enable = !native;
			this.buttonDelete.Enable = !native && sel != -1;
			this.buttonPrev.Enable   = prev;
			this.buttonNext.Enable   = next;
			this.buttonGoto.Enable   = lgoto;
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				Druid druid;
				CultureMapSource source;
				this.GetDruid(first+i, out druid, out source);

				if (druid.IsEmpty)
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, StringList.CellState.Disabled);
					this.array.SetLineColor(0, first+i, Color.Empty);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, StringList.CellState.Disabled);
					this.array.SetLineColor(1, first+i, Color.Empty);

					this.array.SetLineString(2, first+i, "");
					this.array.SetLineState(2, first+i, StringList.CellState.Disabled);
					this.array.SetLineColor(2, first+i, Color.Empty);
				}
				else
				{
					//	La valeur est forc�ment dans le m�me module que l'�num�ration.
					CultureMap cultureMap = this.module.AccessValues.Accessor.Collection[druid];
					StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

					string name = cultureMap.Name;
					string text = ResourceAccess.GetCaptionNiceDescription(data, this.array.LineHeight);

					string icon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon) as string;
					if (!string.IsNullOrEmpty(icon))
					{
						icon = Misc.ImageFull(icon);
					}

					Color color = this.module.IsPatch ? Misc.SourceColor(source) : Color.Empty;

					this.array.SetLineString(0, first+i, name);
					this.array.SetLineState(0, first+i, StringList.CellState.Normal);
					this.array.SetLineColor(0, first+i, color);

					this.array.SetLineString(1, first+i, text);
					this.array.SetLineState(1, first+i, StringList.CellState.Normal);
					this.array.SetLineColor(1, first+i, color);

					this.array.SetLineString(2, first+i, icon);
					this.array.SetLineState(2, first+i, StringList.CellState.Normal);
					this.array.SetLineColor(2, first+i, color);
				}
			}

			this.array.TotalRows = this.EnumCount;
		}

		protected void ArrayCreate()
		{
			//	Cr�e une nouvelle valeur dans l'�num�ration.
			string name = this.GetNewName();
			name = this.module.DesignerApplication.DlgResourceName(Dialogs.ResourceName.Operation.Create, Dialogs.ResourceName.Type.Value, name);
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			
			if (!Misc.IsValidLabel(ref name))
			{
				this.module.DesignerApplication.DialogError(Res.Strings.Error.Name.Invalid);
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				sel = this.array.TotalRows-1;
			}

			//	Code complexe �crit par Pierre permettant de cr�er une nouvelle valeur pour une �num�ration.
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;

			Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.module.AccessTypes.Accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
			CultureMap valueCultureMap = accessor.CreateValueItem(this.cultureMap);
			valueCultureMap.Name = name;

			IResourceAccessor valueAccessor = this.module.AccessValues.Accessor;
			valueAccessor.Collection.Add(valueCultureMap);

			IDataBroker broker = accessor.GetDataBroker(this.structuredData, Support.Res.Fields.ResourceEnumType.Values.ToString());
			StructuredData newValue = broker.CreateData(this.cultureMap);

			Druid druid = valueCultureMap.Id;
			newValue.SetValue(Support.Res.Fields.EnumValue.CaptionId, druid);

			list.Insert(sel+1, newValue);
			accessor.CreateMissingValueItems(this.cultureMap);

			this.UpdateArray();
			this.UpdateButtons();

			this.module.AccessTypes.SetLocalDirty();
			this.OnContentChanged();
		}

		protected void ArrayDelete()
		{
			//	Supprime une valeur dans l'�num�ration.
			int sel = this.array.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);
			string name = this.array.GetLineString(0, sel);
			string question = string.Format("Voulez-vous supprimer la valeur <b>{0}</b> de l'�num�ration ?", name);
			Common.Dialogs.DialogResult result = this.module.DesignerApplication.DialogQuestion(question);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			//	Supprime la ressource "valeur".
			//	Cette ressource est forc�ment dans le m�me module.
			Druid druid;
			CultureMapSource source;
			this.GetDruid(sel, out druid, out source);

			CultureMap cultureMap = this.module.AccessValues.Accessor.Collection[druid];
			System.Diagnostics.Debug.Assert(cultureMap != null);
			this.module.AccessValues.Accessor.Collection.Remove(cultureMap);

			//	Supprime la valeur de la liste de l'�num�ration.
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
			list.RemoveAt(sel);

			if (sel > list.Count-1)
			{
				this.array.SelectedRow = list.Count-1;
			}

			this.UpdateArray();
			this.UpdateButtons();

			this.module.AccessTypes.SetLocalDirty();
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

			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
			StructuredData data = list[sel];
			list.RemoveAt(sel);
			list.Insert(sel+direction, data);

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();
			this.UpdateArray();

			this.module.AccessTypes.SetLocalDirty();
			this.OnContentChanged();
		}

		protected void ArrayGoto()
		{
			//	Va sur la valeur de l'�num�ration s�lectionn�e.
			Druid druid;
			CultureMapSource source;
			this.GetDruid(this.array.SelectedRow, out druid, out source);

			Module module = this.designerApplication.SearchModule(druid);
			if (module == null)
			{
				return;
			}

			this.designerApplication.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Values, -1, druid, this.Window.FocusedWidget);
		}


		protected int EnumCount
		{
			//	Retourne le nombre de valeurs de l'�num�ration.
			get
			{
				IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
				return list.Count;
			}
		}

		protected void GetDruid(int index, out Druid druid, out CultureMapSource source)
		{
			//	Retourne le Druid d'une valeur de l'�num�ration.
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;

			if (index < 0 || index >= list.Count)
			{
				druid = Druid.Empty;
				source = CultureMapSource.Invalid;
			}
			else
			{
				druid = (Druid) list[index].GetValue(Support.Res.Fields.EnumValue.CaptionId);
				source = (CultureMapSource) list[index].GetValue(Support.Res.Fields.EnumValue.CultureMapSource);
			}
		}

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
			CollectionView collection = this.module.AccessValues.CollectionView;

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
				System.Type systemType = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.SystemType) as System.Type;
				if (systemType == null || systemType == typeof(NotAnEnum))
				{
					return false;
				}
				else
				{
					return true;
				}
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

			this.headerName.PreferredWidth = w1;
			this.headerDescription.PreferredWidth = w2;
			this.headerIcon.PreferredWidth = w3+1;
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.buttonCreate)
			{
				this.ArrayCreate();
			}

			if (sender == this.buttonDelete)
			{
				this.ArrayDelete();
			}

			if (sender == this.buttonPrev)
			{
				this.ArrayMove(-1);
			}

			if (sender == this.buttonNext)
			{
				this.ArrayMove(1);
			}

			if (sender == this.buttonGoto)
			{
				this.ArrayGoto();
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
		}


		protected static double					arrayLineHeight = 20;

		protected HToolBar						toolbar;
		protected IconButton					buttonCreate;
		protected IconButton					buttonDelete;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonGoto;
		protected HSlider						slider;

		protected FrameBox						header;
		protected HeaderButton					headerName;
		protected HeaderButton					headerDescription;
		protected HeaderButton					headerIcon;
		protected MyWidgets.StringArray			array;
	}
}
