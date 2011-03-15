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
		public TypeEditorEnum(Module module)
		{
			this.module = module;

			//	Crée la toolbar.
			this.groupToolbar = new ResetBox(this);
			this.groupToolbar.IsPatch = this.module.IsPatch;
			this.groupToolbar.Dock = DockStyle.StackBegin;
			this.groupToolbar.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.toolbar = new HToolBar(this.groupToolbar.GroupBox);
			this.toolbar.Dock = DockStyle.Fill;

			this.buttonCreate = new IconButton();
			this.buttonCreate.CaptionId = Res.Captions.Editor.Type.Create.Id;
			this.buttonCreate.Clicked += this.HandleButtonClicked;
			this.toolbar.Items.Add(this.buttonCreate);

			this.buttonDelete = new IconButton();
			this.buttonDelete.CaptionId = Res.Captions.Editor.Type.Delete.Id;
			this.buttonDelete.Clicked += this.HandleButtonClicked;
			this.toolbar.Items.Add(this.buttonDelete);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionId = Res.Captions.Editor.Type.Prev.Id;
			this.buttonPrev.Clicked += this.HandleButtonClicked;
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionId = Res.Captions.Editor.Type.Next.Id;
			this.buttonNext.Clicked += this.HandleButtonClicked;
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonGoto = this.CreateLocatorGotoButton(null);
			this.buttonGoto.Clicked += this.HandleButtonClicked;
			this.toolbar.Items.Add(this.buttonGoto);

			this.slider = new HSlider(toolbar);
			this.slider.PreferredWidth = 80;
			this.slider.Margins = new Margins(2, 2, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 50.0M;
			this.slider.SmallChange = 5.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += this.HandleSliderChanged;
			this.slider.Value = (decimal) TypeEditorEnum.arrayLineHeight;
			this.slider.Dock = DockStyle.Right;

			//	Crée l'en-tête du tableau.
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

			//	Crée le tableau principal.
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
			this.array.ColumnsWidthChanged += this.HandleArrayColumnsWidthChanged;
			this.array.CellCountChanged += this.HandleArrayCellCountChanged;
			this.array.CellsContentChanged += this.HandleArrayCellsContentChanged;
			this.array.SelectedRowChanged += this.HandleArraySelectedRowChanged;
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupToolbar.ResetButton.Clicked -= this.HandleResetButtonClicked;

				this.buttonCreate.Clicked -= this.HandleButtonClicked;
				this.buttonDelete.Clicked -= this.HandleButtonClicked;
				this.buttonPrev.Clicked -= this.HandleButtonClicked;
				this.buttonNext.Clicked -= this.HandleButtonClicked;
				this.buttonGoto.Clicked -= this.HandleButtonClicked;

				this.slider.ValueChanged -= this.HandleSliderChanged;

				this.array.ColumnsWidthChanged -= this.HandleArrayColumnsWidthChanged;
				this.array.CellCountChanged -= this.HandleArrayCellCountChanged;
				this.array.CellsContentChanged -= this.HandleArrayCellsContentChanged;
				this.array.SelectedRowChanged -= this.HandleArraySelectedRowChanged;
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			object value = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				IList<StructuredData> list = value as IList<StructuredData>;
				builder.Append(list.Count.ToString());
				builder.Append("×: ");

				for (int i=0; i<list.Count; i++)
				{
					StructuredData data = list[i];

					Druid druid = (Druid) data.GetValue(Support.Res.Fields.EnumValue.CaptionId);
					//	La valeur est forcément dans le même module que l'énumération.
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
			//	Met à jour le contenu de l'éditeur.
			this.array.SelectedRow = -1;
			this.UpdateArray();
			this.UpdateButtons();
		}


		protected void UpdateButtons()
		{
			//	Met à jour les boutons.
			bool native = this.IsNativeEnum;
			bool prev   = false;
			bool next   = false;
			bool lgoto  = false;
			bool patch  = false;

			int sel = this.array.SelectedRow;
			if (sel != -1)
			{
				prev = sel > 0;
				next = sel < this.EnumCount-1;

				Druid druid;
				CultureMapSource source;
				this.GetDruid(sel, out druid, out source);  // Druid de la ligne sélectionnée

				lgoto = druid.IsValid;
				patch = this.module.IsPatch && source == CultureMapSource.ReferenceModule;
			}

			this.buttonCreate.Enable = !native;
			this.buttonDelete.Enable = !native && !patch && sel != -1;
			this.buttonPrev.Enable   = prev;
			this.buttonNext.Enable   = next;
			this.buttonGoto.Enable   = lgoto;
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				Druid druid;
				CultureMapSource source;
				this.GetDruid(first+i, out druid, out source);

				if (druid.IsValid)
				{
					//	La valeur est forcément dans le même module que l'énumération.
					CultureMap cultureMap = this.module.AccessValues.Accessor.Collection[druid];
					if (cultureMap != null)
					{
						StructuredData data = cultureMap.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
						if (data != null)
						{
							string name = cultureMap.Name;
							string text = ResourceAccess.GetCaptionNiceDescription (data, this.array.LineHeight);

							string icon = data.GetValue (Support.Res.Fields.ResourceCaption.Icon) as string;
							if (!string.IsNullOrEmpty (icon))
							{
								icon = Misc.ImageFull (icon);
							}

							Color color = this.module.IsPatch ? Misc.SourceColor (source) : Color.Empty;

							this.array.SetLineString (0, first+i, name);
							this.array.SetLineState (0, first+i, StringList.CellState.Normal);
							this.array.SetLineColor (0, first+i, color);

							this.array.SetLineString (1, first+i, text);
							this.array.SetLineState (1, first+i, StringList.CellState.Normal);
							this.array.SetLineColor (1, first+i, color);

							this.array.SetLineString (2, first+i, icon);
							this.array.SetLineState (2, first+i, StringList.CellState.Normal);
							this.array.SetLineColor (2, first+i, color);

							continue;
						}
					}
				}

				this.array.SetLineString (0, first+i, "");
				this.array.SetLineState (0, first+i, StringList.CellState.Disabled);
				this.array.SetLineColor (0, first+i, Color.Empty);

				this.array.SetLineString (1, first+i, "");
				this.array.SetLineState (1, first+i, StringList.CellState.Disabled);
				this.array.SetLineColor (1, first+i, Color.Empty);

				this.array.SetLineString (2, first+i, "");
				this.array.SetLineState (2, first+i, StringList.CellState.Disabled);
				this.array.SetLineColor (2, first+i, Color.Empty);
			}

			this.array.TotalRows = this.EnumCount;

			this.UpdateReset();
		}

		protected void UpdateReset()
		{
			//	Met à jour la couleur de fond du bouton 'reset'.
			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);
			bool usesOriginalData;
			this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values, out usesOriginalData);
			this.ColorizeResetBox(this.groupToolbar, source, usesOriginalData);
		}

		protected void ArrayCreate()
		{
			//	Crée une nouvelle valeur dans l'énumération.
			string name = this.GetNewName();
			name = this.module.DesignerApplication.DlgResourceName(Dialogs.ResourceNameDialog.Operation.Create, Dialogs.ResourceNameDialog.Type.Value, name);
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			
			if (!Misc.IsValidLabel(ref name, false))
			{
				this.module.DesignerApplication.DialogError(Res.Strings.Error.Name.Invalid);
				return;
			}

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				sel = this.array.TotalRows-1;
			}

			//	Code complexe écrit par Pierre permettant de créer une nouvelle valeur pour une énumération.
			IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;

			Support.ResourceAccessors.AnyTypeResourceAccessor accessor = this.module.AccessTypes.Accessor as Support.ResourceAccessors.AnyTypeResourceAccessor;
			CultureMap valueCultureMap = accessor.CreateValueItem(this.cultureMap);
			valueCultureMap.Name = name;

			IResourceAccessor valueAccessor = this.module.AccessValues.Accessor;
			valueAccessor.Collection.Add(valueCultureMap);

			IDataBroker broker = accessor.GetDataBroker(this.structuredData, Support.Res.Fields.ResourceEnumType.Values);
			StructuredData newValue = broker.CreateData(this.cultureMap);

			Druid druid = valueCultureMap.Id;
			newValue.SetValue(Support.Res.Fields.EnumValue.CaptionId, druid);

			list.Insert(sel+1, newValue);

			this.UpdateArray();
			this.UpdateButtons();

			this.module.AccessTypes.SetLocalDirty();
			this.OnContentChanged();
		}

		protected void ArrayDelete()
		{
			//	Supprime une valeur dans l'énumération.
			int sel = this.array.SelectedRow;
			System.Diagnostics.Debug.Assert(sel != -1);
			string name = this.array.GetLineString(0, sel);
			string question = string.Format(Res.Strings.Viewers.Types.Enum.Question.Delete, name);
			var result = this.module.DesignerApplication.DialogQuestion(question);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			//	Supprime la ressource "valeur".
			//	Cette ressource est forcément dans le même module.
			Druid druid;
			CultureMapSource source;
			this.GetDruid(sel, out druid, out source);

			CultureMap cultureMap = this.module.AccessValues.Accessor.Collection[druid];
			System.Diagnostics.Debug.Assert(cultureMap != null);
			this.module.AccessValues.Accessor.Collection.Remove(cultureMap);

			//	Supprime la valeur de la liste de l'énumération.
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
			//	Déplace une valeur dans l'énumération.
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
			//	Va sur la valeur de l'énumération sélectionnée.
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
			//	Retourne le nombre de valeurs de l'énumération.
			get
			{
				IList<StructuredData> list = this.structuredData.GetValue(Support.Res.Fields.ResourceEnumType.Values) as IList<StructuredData>;
				return list.Count;
			}
		}

		protected void GetDruid(int index, out Druid druid, out CultureMapSource source)
		{
			//	Retourne le Druid d'une valeur de l'énumération.
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
			//	Cherche un nouveau nom jamais utilisé.
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
			//	Indique s'il s'agit d'une énumération native.
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
			//	Met à jour la géométrie.
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
			//	Appelé lorsque le slider a été déplacé.
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
			//	La largeur des colonnes a changé.
			this.UpdateClientGeometry();
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

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupToolbar.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceEnumType.Values);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected static double					arrayLineHeight = 20;

		protected ResetBox						groupToolbar;
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
