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
	public class TypeEditorStructured : AbstractTypeEditor
	{
		public TypeEditorStructured()
		{
			//	Crée la toolbar.
			this.toolbar = new HToolBar(this);
			this.toolbar.Dock = DockStyle.StackBegin;

			this.buttonAdd = new IconButton();
			this.buttonAdd.CaptionId = Res.Captions.Editor.Structured.Add.Id;
			this.buttonAdd.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonAdd);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonPrev = new IconButton();
			this.buttonPrev.CaptionId = Res.Captions.Editor.Structured.Prev.Id;
			this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonPrev);

			this.buttonNext = new IconButton();
			this.buttonNext.CaptionId = Res.Captions.Editor.Structured.Next.Id;
			this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonNext);

			this.toolbar.Items.Add(new IconSeparator());

			this.buttonRemove = new IconButton();
			this.buttonRemove.CaptionId = Res.Captions.Editor.Structured.Remove.Id;
			this.buttonRemove.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRemove);

			Widget sep = new Widget();
			sep.PreferredWidth = 50;
			this.toolbar.Items.Add(sep);

			this.buttonRelationRef = new IconButton();
			this.buttonRelationRef.CaptionId = Res.Captions.Editor.Structured.Relation.Reference.Id;
			this.buttonRelationRef.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRelationRef.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRelationRef);

			this.buttonRelationCol = new IconButton();
			this.buttonRelationCol.CaptionId = Res.Captions.Editor.Structured.Relation.Collection.Id;
			this.buttonRelationCol.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRelationCol.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRelationCol);

			this.buttonRelationInc = new IconButton();
			this.buttonRelationInc.CaptionId = Res.Captions.Editor.Structured.Relation.Inclusion.Id;
			this.buttonRelationInc.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRelationInc.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.toolbar.Items.Add(this.buttonRelationInc);

			this.slider = new HSlider(toolbar);
			this.slider.PreferredWidth = 80;
			this.slider.Margins = new Margins(2, 2, 4, 4);
			this.slider.MinValue = 20.0M;
			this.slider.MaxValue = 50.0M;
			this.slider.SmallChange = 5.0M;
			this.slider.LargeChange = 10.0M;
			this.slider.Resolution = 1.0M;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderChanged);
			this.slider.Value = (decimal) TypeEditorStructured.arrayLineHeight;
			this.slider.Dock = DockStyle.Right;

			//	Crée l'en-tête du tableau.
			this.header = new Widget(this);
			this.header.Dock = DockStyle.StackBegin;
			this.header.Margins = new Margins(0, 0, 4, 0);

			this.headerCaption = new HeaderButton(this.header);
			this.headerCaption.Text = Res.Strings.Viewers.Types.Structured.Caption;
			this.headerCaption.Style = HeaderButtonStyle.Top;
			this.headerCaption.Dock = DockStyle.Left;

			this.headerType = new HeaderButton(this.header);
			this.headerType.Text = Res.Strings.Viewers.Types.Structured.Type;
			this.headerType.Style = HeaderButtonStyle.Top;
			this.headerType.Dock = DockStyle.Left;

			this.headerInc = new HeaderButton(this.header);
			this.headerInc.Text = Res.Strings.Viewers.Types.Structured.Inclusion;
			this.headerInc.Style = HeaderButtonStyle.Top;
			this.headerInc.Dock = DockStyle.Left;

			//	Crée le tableau principal.
			this.array = new StringArray(this);
			this.array.Columns = 6;
			this.array.SetColumnsRelativeWidth(0, 0.30);
			this.array.SetColumnsRelativeWidth(1, 0.06);
			this.array.SetColumnsRelativeWidth(2, 0.30);
			this.array.SetColumnsRelativeWidth(3, 0.06);
			this.array.SetColumnsRelativeWidth(4, 0.30);
			this.array.SetColumnsRelativeWidth(5, 0.06);
			this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(1, ContentAlignment.MiddleCenter);
			this.array.SetColumnAlignment(2, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(3, ContentAlignment.MiddleCenter);
			this.array.SetColumnAlignment(4, ContentAlignment.MiddleLeft);
			this.array.SetColumnAlignment(5, ContentAlignment.MiddleCenter);
			this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split);
			this.array.SetColumnBreakMode(2, TextBreakMode.Ellipsis | TextBreakMode.Split);
			this.array.SetColumnBreakMode(4, TextBreakMode.Ellipsis | TextBreakMode.Split);
			this.array.SetDynamicToolTips(0, false);
			this.array.SetDynamicToolTips(1, false);
			this.array.SetDynamicToolTips(2, false);
			this.array.SetDynamicToolTips(3, false);
			this.array.SetDynamicToolTips(4, false);
			this.array.SetDynamicToolTips(5, false);
			this.array.LineHeight = TypeEditorStructured.arrayLineHeight;
			this.array.Dock = DockStyle.StackBegin;
			this.array.PreferredHeight = 200;
			this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
			this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);

			//	Crée le pied pour éditer la ligne sélectionnée.
			this.footer = new Widget(this);
			this.footer.Dock = DockStyle.StackBegin;
			this.footer.Margins = new Margins(0, 0, 5, 0);

			this.buttonCaptionGoto = this.CreateLocatorGotoButton(this.footer);
			this.buttonCaptionGoto.Margins = new Margins(1, 0, 0, 0);
			this.buttonCaptionGoto.Dock = DockStyle.Left;
			this.buttonCaptionGoto.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.blankCaption = new Widget(this.footer);
			this.blankCaption.Margins = new Margins(1, 0, 0, 0);
			this.blankCaption.Dock = DockStyle.Left;

			this.buttonTypeGoto = this.CreateLocatorGotoButton(this.footer);
			this.buttonTypeGoto.Margins = new Margins(1, 0, 0, 0);
			this.buttonTypeGoto.Dock = DockStyle.Left;
			this.buttonTypeGoto.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonType = new Button(this.footer);
			this.buttonType.CaptionId = Res.Captions.Editor.Structured.ChangeType.Id;
			this.buttonType.Margins = new Margins(1, 0, 0, 0);
			this.buttonType.Dock = DockStyle.Left;
			this.buttonType.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonInc = new Button(this.footer);
			this.buttonInc.CaptionId = Res.Captions.Editor.Structured.ChangeInclusion.Id;
			this.buttonInc.Margins = new Margins(1, 0, 0, 0);
			this.buttonInc.Dock = DockStyle.Left;
			this.buttonInc.Clicked += new MessageEventHandler(this.HandleButtonClicked);
		}

		public TypeEditorStructured(Widget embedder) : this()
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
				this.buttonRelationRef.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.buttonRelationCol.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.buttonRelationInc.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.slider.ValueChanged -= new EventHandler(this.HandleSliderChanged);

				this.array.ColumnsWidthChanged -= new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked -= new EventHandler(this.HandleArraySelectedRowDoubleClicked);

				this.buttonCaptionGoto.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonTypeGoto.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonType.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.buttonInc.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			builder.Append(this.fields.Count.ToString());
			builder.Append("×: ");

			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField field = this.fields[i];
				builder.Append(field.Id);

				if (i < this.fields.Count-1)
				{
					builder.Append(", ");
				}
			}
			
			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			this.FieldsInput();
			this.UpdateArray();
			this.UpdateButtons();
			this.ignoreChange = false;
		}

		protected void UpdateButtons()
		{
			//	Met à jour tous les boutons en fonction de la ligne sélectionnée dans le tableau.
			int sel = this.array.SelectedRow;

			this.buttonPrev.Enable = (sel != -1 && sel > 0);
			this.buttonNext.Enable = (sel != -1 && sel < this.fields.Count-1);
			this.buttonRemove.Enable = (sel != -1);

			StructuredTypeField field = null;
			bool st = false;
			Druid typeDruid = Druid.Empty;
			if (sel != -1)
			{
				field = this.fields[sel];
				st = (field.Type is StructuredType);
			
				AbstractType type = field.Type as AbstractType;
				if (type != null)
				{
					typeDruid = type.Caption.Id;
				}
			
			}

			this.buttonRelationRef.Enable = st;
			this.buttonRelationCol.Enable = st;
			this.buttonRelationInc.Enable = st;

			if (st)
			{
				this.buttonRelationRef.ActiveState = (field.Relation == Relation.Reference ) ? ActiveState.Yes : ActiveState.No;
				this.buttonRelationCol.ActiveState = (field.Relation == Relation.Collection) ? ActiveState.Yes : ActiveState.No;
				this.buttonRelationInc.ActiveState = (field.Relation == Relation.Inclusion ) ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.buttonRelationRef.ActiveState = ActiveState.No;
				this.buttonRelationCol.ActiveState = ActiveState.No;
				this.buttonRelationInc.ActiveState = ActiveState.No;
			}

			this.buttonCaptionGoto.Enable = (sel != -1);

			this.buttonTypeGoto.Enable = (sel != -1 && !typeDruid.IsEmpty);
			this.buttonType.Enable = (sel != -1);
			
			this.buttonInc.Enable = (sel != -1 && st && field.Relation == Relation.Inclusion);
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.fields.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.fields.Count)
				{
					StructuredTypeField field = this.fields[first+i];
					string iconRelation = "";

					string captionType = "";
					string iconType = "";
					AbstractType type = field.Type as AbstractType;
					if (type != null)
					{
						if (type is StructuredType)
						{
							if (field.Relation == Relation.Reference )  iconRelation = Misc.Image("RelationReference");
							if (field.Relation == Relation.Inclusion )  iconRelation = Misc.Image("RelationInclusion");
							if (field.Relation == Relation.Collection)  iconRelation = Misc.Image("RelationCollection");
						}

						Caption caption = this.module.ResourceManager.GetCaption(type.Caption.Id);

						string text = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
						if (this.array.LineHeight >= 30)  // assez de place pour 2 lignes ?
						{
							captionType = string.Concat(caption.Name, ":<br/>", text);
						}
						else if (string.IsNullOrEmpty(text))
						{
							captionType = caption.Name;
						}
						else
						{
							captionType = text;
						}

						iconType = this.resourceAccess.DirectGetIcon(caption.Id);
						if (!string.IsNullOrEmpty(iconType))
						{
							iconType = Misc.ImageFull(iconType);
						}
					}

					string captionText = "";
					string iconText = "";
					Druid druid = field.CaptionId;
					if (druid.IsValid)
					{
						Caption caption = this.module.ResourceManager.GetCaption(druid);

						string text = ResourceAccess.GetCaptionNiceDescription(caption, 0);  // texte sur 1 ligne
						if (this.array.LineHeight >= 30)  // assez de place pour 2 lignes ?
						{
							captionText = string.Concat(caption.Name, ":<br/>", text);
						}
						else if (string.IsNullOrEmpty(text))
						{
							captionType = caption.Name;
						}
						else
						{
							captionText = text;
						}

						if (!string.IsNullOrEmpty(caption.Icon))
						{
							iconText = Misc.ImageFull(caption.Icon);
						}
					}

					string captionInc = field.SourceFieldId;

					this.array.SetLineString(0, first+i, captionText);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, iconText);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(2, first+i, captionType);
					this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(3, first+i, iconType);
					this.array.SetLineState(3, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(4, first+i, captionInc);
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(5, first+i, iconRelation);
					this.array.SetLineState(5, first+i, MyWidgets.StringList.CellState.Normal);
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

					this.array.SetLineString(4, first+i, "");
					this.array.SetLineState(4, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(5, first+i, "");
					this.array.SetLineState(5, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}


		protected void ArrayAdd()
		{
			//	Ajoute une nouvelle valeur dans la structure.
			int sel = this.array.SelectedRow;
			
			AbstractType type = null;
			if (sel != -1)
			{
				StructuredTypeField actualField = this.fields[sel];
				type = actualField.Type as AbstractType;
			}

			string text = this.GetNewName();
			//?string name = string.Concat(this.AbstractType.Caption.Name, ".", text);
			string name = string.Concat(this.SelectedName, ".", text);

			//	Crée un Caption de type Field (dont le nom commence par "Fld.").
			this.resourceAccess.BypassFilterOpenAccess(ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, null, null);
			Druid druid = this.resourceAccess.BypassFilterCreate(this.resourceAccess.GetCultureBundle(null), name, text);
			this.resourceAccess.BypassFilterCloseAccess();

			StructuredTypeField newField = new StructuredTypeField(null, type, druid, 0);
			this.fields.Insert(sel+1, newField);

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel+1;
			this.array.ShowSelectedRow();

			this.UpdateButtons();

			this.OnContentChanged();
		}

		protected void ArrayRemove()
		{
			//	Supprime une valeur de la structure.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			this.fields.RemoveAt(sel);

			if (sel > this.fields.Count-1)
			{
				sel = this.fields.Count-1;
			}

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.OnContentChanged();
		}

		protected void ArrayMove(int direction)
		{
			//	Déplace une valeur dans la structure.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField field = this.fields[sel];
			this.fields.RemoveAt(sel);
			this.fields.Insert(sel+direction, field);

			this.FieldsOutput();
			this.UpdateArray();

			this.array.SelectedRow = sel+direction;
			this.array.ShowSelectedRow();

			this.UpdateButtons();
			this.OnContentChanged();
		}

		protected void ArrayRelation(Relation relation)
		{
			//	Change le type de relation.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];

			string sourceField = null;
			if (relation == Relation.Inclusion)
			{
				StructuredType type = actualField.Type as StructuredType;
				if (type != null)
				{
					foreach (string id in type.GetFieldIds())
					{
						sourceField = id;
						break;
					}
				}

				System.Diagnostics.Debug.Assert(sourceField != null);
			}

			StructuredTypeField newField = new StructuredTypeField(null, actualField.Type, actualField.CaptionId, actualField.Rank, relation, sourceField);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.UpdateButtons();
			this.OnContentChanged();
		}

		protected void GotoCaption()
		{
			//	Va sur la définition du Caption.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			Druid druid = actualField.CaptionId;

			this.module.LocatorGoto(this.module.ModuleInfo.Name, ResourceAccess.Type.Fields, druid);
		}

#if false
		protected void ChangeCaption()
		{
			//	Choix du caption.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			Druid druid = actualField.CaptionId;
			string includePrefix = string.Concat(this.AbstractType.Caption.Name, ".");

			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, druid, null, includePrefix);
			if (druid.IsEmpty)
			{
				return;
			}

			StructuredTypeField newField = new StructuredTypeField(null, actualField.Type, druid, actualField.Rank, actualField.Relation, actualField.SourceFieldId);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.OnContentChanged();
		}
#endif

		protected void GotoType()
		{
			//	Va sur la définition du type.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			AbstractType type = actualField.Type as AbstractType;
			Druid druid = (type == null) ? Druid.Empty : type.Caption.Id;
			if (druid.IsEmpty)
			{
				return;
			}

			this.module.LocatorGoto(this.module.ModuleInfo.Name, ResourceAccess.Type.Types, druid);
		}

		protected void ChangeType()
		{
			//	Choix du type.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			AbstractType type = actualField.Type as AbstractType;
			Druid druid = (type == null) ? Druid.Empty : type.Caption.Id;

			druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Types, ResourceAccess.TypeType.None, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			type = TypeRosetta.GetTypeObject(this.module.ResourceManager.GetCaption(druid));
			Relation relation = actualField.Relation;
			string sourceField = actualField.SourceFieldId;
			if (type is StructuredType)
			{
			}
			else
			{
				relation = Relation.None;
				sourceField = null;
			}
			System.Diagnostics.Debug.Assert(type != null);
			StructuredTypeField newField = new StructuredTypeField(null, type, actualField.CaptionId, actualField.Rank, relation, sourceField);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.UpdateButtons();
			this.OnContentChanged();
		}

		protected void ChangeInclusion()
		{
			//	Choix de l'inclusion.
			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			StructuredTypeField actualField = this.fields[sel];
			if (actualField.Relation != Relation.Inclusion)
			{
				return;
			}

			StructuredType st = actualField.Type as StructuredType;
			string field = this.mainWindow.DlgResourceStructuredTypeField(st, actualField.SourceFieldId);
			if (string.IsNullOrEmpty(field))
			{
				return;
			}

			StructuredTypeField newField = new StructuredTypeField(null, actualField.Type, actualField.CaptionId, actualField.Rank, actualField.Relation, field);
			this.fields[sel] = newField;

			this.FieldsOutput();
			this.UpdateArray();
			this.OnContentChanged();
		}


		protected string GetNewName()
		{
			//	Cherche un nouveau nom jamais utilisé.
			for (int i=1; i<10000; i++)
			{
				string name = string.Format(Res.Strings.Viewers.Types.Structured.NewName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if (!this.IsExistingName(name, -1))
				{
					return name;
				}
			}
			return null;
		}

		protected bool IsExistingName(string name, int exclude)
		{
			//	Indique si un nom existe.
			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField field = this.fields[i];
				Druid druid = field.CaptionId;
				System.Diagnostics.Debug.Assert(druid.IsValid);
				Caption caption = this.module.ResourceManager.GetCaption(druid);
				System.Diagnostics.Debug.Assert(caption != null);

				if (i != exclude && name == caption.Name)
				{
					return true;
				}
			}

			return false;
		}


		protected void FieldsInput()
		{
			//	Lit le dictionnaire des rubriques (StructuredTypeField) dans la liste
			//	interne (this.fields).
			if (this.lastIndex != this.resourceSelected)  // pas encore lu ?
			{
				this.lastIndex = this.resourceSelected;

				StructuredType type = this.AbstractType as StructuredType;
				this.fields = new List<StructuredTypeField>();

				foreach (string id in type.GetFieldIds())
				{
					StructuredTypeField field = type.Fields[id];
					this.fields.Add(field);
				}

				this.array.SelectedRow = -1;
			}
		}

		protected void FieldsOutput()
		{
			//	Le dictionnaire des rubriques (StructuredTypeField) est régénéré à partir
			//	de la liste interne (this.fields). 
			StructuredType type = this.AbstractType as StructuredType;
			type.Fields.Clear();

			for (int i=0; i<this.fields.Count; i++)
			{
				StructuredTypeField actualField = this.fields[i];
				StructuredTypeField newField = new StructuredTypeField(null, actualField.Type, actualField.CaptionId, i, actualField.Relation, actualField.SourceFieldId);

				type.Fields.Add(newField);
			}
		}


		protected void UpdateColumnsWidth()
		{
			//	Place les widgets en dessus et en dessous du tableau en fonction des
			//	largeurs des colonnes.
			double w1 = this.array.GetColumnsAbsoluteWidth(0) + this.array.GetColumnsAbsoluteWidth(1);
			double w2 = this.array.GetColumnsAbsoluteWidth(2) + this.array.GetColumnsAbsoluteWidth(3);
			double w3 = this.array.GetColumnsAbsoluteWidth(4) + this.array.GetColumnsAbsoluteWidth(5);

			this.headerCaption.PreferredWidth = w1;
			this.headerType.PreferredWidth = w2;
			this.headerInc.PreferredWidth = w3+1;

			this.blankCaption.PreferredWidth = w1-1-this.buttonCaptionGoto.PreferredWidth;
			this.buttonType.PreferredWidth = w2-this.buttonTypeGoto.PreferredWidth;
			this.buttonInc.PreferredWidth = w3+1;
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

			if (sender == this.buttonRelationRef)
			{
				this.ArrayRelation(Relation.Reference);
			}

			if (sender == this.buttonRelationCol)
			{
				this.ArrayRelation(Relation.Collection);
			}

			if (sender == this.buttonRelationInc)
			{
				this.ArrayRelation(Relation.Inclusion);
			}

			if (sender == this.buttonCaptionGoto)
			{
				this.GotoCaption();
			}

			if (sender == this.buttonTypeGoto)
			{
				this.GotoType();
			}

			if (sender == this.buttonType)
			{
				this.ChangeType();
			}

			if (sender == this.buttonInc)
			{
				this.ChangeInclusion();
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
			TypeEditorStructured.arrayLineHeight = (double) slider.Value;
			this.array.LineHeight = TypeEditorStructured.arrayLineHeight;
		}

		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateColumnsWidth();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.FieldsInput();
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	Une ligne a été double-cliquée.
			int column = this.array.SelectedColumn;

			if (column == 2 || column == 3)
			{
				this.ChangeType();
			}

			if (column == 4 || column == 5)
			{
				this.ChangeInclusion();
			}
		}


		protected static double					arrayLineHeight = 20;

		protected HToolBar						toolbar;
		protected IconButton					buttonAdd;
		protected IconButton					buttonPrev;
		protected IconButton					buttonNext;
		protected IconButton					buttonRemove;
		protected IconButton					buttonRelationRef;
		protected IconButton					buttonRelationCol;
		protected IconButton					buttonRelationInc;
		protected HSlider						slider;

		protected Widget						header;
		protected HeaderButton					headerCaption;
		protected HeaderButton					headerType;
		protected HeaderButton					headerInc;
		protected MyWidgets.StringArray			array;

		protected Widget						footer;
		protected IconButton					buttonCaptionGoto;
		protected Widget						blankCaption;
		protected IconButton					buttonTypeGoto;
		protected Button						buttonType;
		protected Button						buttonInc;

		protected int							lastIndex = -1;
		protected List<StructuredTypeField>		fields;
	}
}
