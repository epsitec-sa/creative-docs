using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		public ObjectModifier(Editor formEditor)
		{
			//	Constructeur unique.
			this.formEditor = formEditor;

			this.tableContent = new List<TableItem>();
		}


		public List<TableItem> TableContent
		{
			get
			{
				return this.tableContent;
			}
		}


		public bool IsField(Widget obj)
		{
			//	Indique si l'objet correspond à un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Field;
		}

		public bool IsBox(Widget obj)
		{
			//	Indique si l'objet correspond à une boîte.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.BoxBegin || field.Type == FieldDescription.FieldType.SubForm;
		}

		public bool IsGlue(Widget obj)
		{
			//	Indique si l'objet correspond à de la colle.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Glue;
		}


		public int GetColumnsRequired(Widget obj)
		{
			//	Retourne le nombre de colonnes requises.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return 0;
			}
			else
			{
				return field.ColumnsRequired;
			}
		}

		public void SetColumnsRequired(Widget obj, int columnsRequired)
		{
			//	Choix du nombre de colonnes requises.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.ColumnsRequired = columnsRequired;
			}
		}

		public int GetRowsRequired(Widget obj)
		{
			//	Retourne le nombre de lignes requises.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return 0;
			}
			else
			{
				return field.RowsRequired;
			}
		}

		public void SetRowsRequired(Widget obj, int rowsRequired)
		{
			//	Choix du nombre de lignes requises.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.RowsRequired = rowsRequired;
			}
		}


		public FieldDescription.SeparatorType GetSeparatorBottom(Widget obj)
		{
			//	Retourne le type du séparateur d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FieldDescription.SeparatorType.Normal;
			}
			else
			{
				return field.SeparatorBottom;
			}
		}

		public void SetSeparatorBottom(Widget obj, FieldDescription.SeparatorType sep)
		{
			//	Choix du type du séparateur d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.SeparatorBottom = sep;
			}
		}


		public FieldDescription.BoxPaddingType GetBoxPadding(Widget obj)
		{
			//	Retourne le type des marges intérieures d'une boîte.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FieldDescription.BoxPaddingType.Normal;
			}
			else
			{
				return field.BoxPadding;
			}
		}

		public void SetBoxPadding(Widget obj, FieldDescription.BoxPaddingType type)
		{
			//	Choix du type des marges intérieures d'une boîte.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxPadding = type;
			}
		}


		public FieldDescription.BackColorType GetBackColor(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FieldDescription.BackColorType.None;
			}
			else
			{
				return field.BackColor;
			}
		}

		public void SetBackColor(Widget obj, FieldDescription.BackColorType color)
		{
			//	Choix de la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BackColor = color;
			}
		}


		public FrameState GetBoxFrameState(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FrameState.None;
			}
			else
			{
				return field.BoxFrameState;
			}
		}

		public void SetBoxFrameState(Widget obj, FrameState state)
		{
			//	Choix de la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxFrameState = state;
			}
		}


		public double GetBoxFrameWidth(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return 0.0;
			}
			else
			{
				return (field.BoxFrameWidth+1)/2;
			}
		}

		public void SetBoxFrameWidth(Widget obj, double width)
		{
			//	Choix de la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxFrameWidth = 2*width-1;
			}
		}


		public Widget GetWidget(System.Guid guid)
		{
			//	Cherche le widget correspondant à un Guid.
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				if (guid == field.Guid)
				{
					Widget obj = this.GetWidget(field.UniqueId);
					if (obj != null)
					{
						return obj;
					}
				}
			}

			return null;
		}

		public Widget GetWidget(int uniqueId)
		{
			//	Cherche le widget correspondant à un identificateur unique.
			return this.GetWidget(this.formEditor.Panel, uniqueId);
		}

		protected Widget GetWidget(Widget parent, int uniqueId)
		{
			if (parent.Index == uniqueId)
			{
				return parent;
			}

			Widget[] children = parent.Children.Widgets;
			for (int i=0; i<children.Length; i++)
			{
				Widget widget = this.GetWidget(children[i], uniqueId);
				if (widget != null)
				{
					return widget;
				}
			}

			return null;
		}


		public FieldDescription GetFormDescription(TableItem item)
		{
			int index = this.GetFormDescriptionIndex(item.Guid);

			if (index == -1)
			{
				index = this.GetFormDescriptionIndex(item.DruidsPath);
			}

			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.Form.Fields[index];
			}
		}

		public FieldDescription GetFormDescription(Widget obj)
		{
			//	Retourne un champ d'après l'identificateur unique d'un widget.
			int index = this.GetFormDescriptionIndex(obj);

			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.Form.Fields[index];
			}
		}

		public int GetFormDescriptionIndex(Widget obj)
		{
			//	Retourne l'index d'un champ d'après l'identificateur unique d'un widget.
			int uniqueId = obj.Index;

			if (uniqueId != -1)
			{
				for (int i=0; i<this.formEditor.Form.Fields.Count; i++)
				{
					FieldDescription field = this.formEditor.Form.Fields[i];

					if (field.UniqueId == uniqueId)
					{
						return i;
					}
				}
			}

			return -1;
		}

		public int GetFormDescriptionIndex(System.Guid guid)
		{
			//	Retourne l'index d'un champ d'après le Guid.
			for (int i=0; i<this.formEditor.Form.Fields.Count; i++)
			{
				FieldDescription field = this.formEditor.Form.Fields[i];

				if (field.Guid == guid)
				{
					return i;
				}
			}

			return -1;
		}

		protected int GetFormDescriptionIndex(string druidsPath)
		{
			//	Retourne l'index d'un champ d'après le chemin de Druis.
			for (int i=0; i<this.formEditor.Form.Fields.Count; i++)
			{
				FieldDescription field = this.formEditor.Form.Fields[i];

				if (field.GetPath(null) == druidsPath)
				{
					return i;
				}
			}

			return -1;
		}


		public Rectangle GetActualBounds(Widget obj)
		{
			//	Retourne la position et les dimensions actuelles de l'objet.
			Rectangle bounds = obj.Client.Bounds;
			bounds = this.AdjustBounds(obj, bounds);

			while (obj != null && obj != this.formEditor.Panel)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public Rectangle AdjustBounds(Widget obj, Rectangle bounds)
		{
			//	Retourne le rectangle ajusté d'un objet.
			if (obj.Name == "GlueNull")  // objet FieldDescription.FieldType.Glue avec un ColumnRequired = 1 ?
			{
				bounds.Width = 0;  // fait croire à un trait vertical calé à gauche
			}

			return bounds;
		}

		public Rectangle InflateMinimalSize(Rectangle rect)
		{
			//	Retourne le rectangle aggrandi pour avoir au moins la taille minimale.
			double ix = 0;
			if (rect.Width < this.formEditor.Context.MinimalSize)
			{
				ix = this.formEditor.Context.MinimalSize;
			}

			double iy = 0;
			if (rect.Height < this.formEditor.Context.MinimalSize)
			{
				iy = this.formEditor.Context.MinimalSize;
			}

			rect.Inflate(ix, iy);
			return rect;
		}


		#region TableContent
		public string GetTableContentDescription(TableItem item, bool showPrefix)
		{
			//	Retourne le texte permettant de décrire un TableItem dans une liste, avec un effet
			//	d'indentation pour ressembler aux arborescences de Vista.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			for (int i=0; i<item.Level; i++)
			{
				builder.Append(Misc.Image("TreeSpace"));
			}

			if (item.FieldType == FieldDescription.FieldType.BoxBegin)
			{
				builder.Append(Misc.Image("TreeBranch"));
			}
			else
			{
				builder.Append(Misc.Image("TreeMark"));
			}

			string name = this.formEditor.Module.AccessFields.GetFieldNames(item.DruidsPath);
			if (name == null)
			{
				FieldDescription field = this.formEditor.ObjectModifier.GetFormDescription(item);
				if (field != null)
				{
					if (field.Type == FieldDescription.FieldType.BoxBegin ||
						field.Type == FieldDescription.FieldType.SubForm)
					{
						name = Misc.Bold(field.Description);
					}
					else
					{
						name = Misc.Italic(field.Description);
					}
				}
			}
			else
			{
				if (item.Prefix != null && showPrefix)
				{
					name = string.Concat(item.Prefix, name);
				}
			}

			builder.Append(" ");
			builder.Append(name);

			return builder.ToString();
		}

		public string GetTableContentIcon(TableItem item)
		{
			//	Retourne le texte permettant de décrire larelation d'un TableItem dans une liste.
			string icon = null;

			if (item.FieldType == FieldDescription.FieldType.SubForm)
			{
				icon = Misc.Image("TreeSubForm");
			}

			return icon;
		}

		public void UpdateTableContent()
		{
			//	Met à jour la liste qui reflète le contenu de la table des champs, visible en haut à droite.
			if (this.formEditor.Form == null)
			{
				return;
			}

			this.tableContent.Clear();

			//	Construit la liste des chemins de Druids, en commençant par ceux qui font
			//	partie du masque de saisie.
			int level = 0;
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				string prefix = null;
				Druid druid = field.FieldIds[0];
				Module module = this.formEditor.Module.DesignerApplication.SearchModule(druid);
				if (module != null)
				{
					CultureMap cultureMap = module.AccessFields.Accessor.Collection[druid];
					if (cultureMap != null)
					{
						prefix = string.Concat(module.ModuleInfo.SourceNamespace, ".", cultureMap.Prefix, ".");
					}
				}

				TableItem item = new TableItem();
				item.Guid = field.Guid;
				item.FieldType = field.Type;
				item.SourceFieldType = field.SourceType;
				item.Prefix = prefix;
				item.DruidsPath = field.GetPath(null);
				item.Level = level;

				this.tableContent.Add(item);

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					level++;
				}

				if (field.Type == FieldDescription.FieldType.BoxEnd)
				{
					level--;
				}
			}
		}

		public int GetTableContentIndex(System.Guid guid)
		{
			//	Cherche l'index d'un Guid dans la table des champs.
			for (int i=0; i<this.tableContent.Count; i++)
			{
				if (guid == this.tableContent[i].Guid)
				{
					return i;
				}
			}

			return -1;
		}

		protected int GetTableContentIndex(string druidsPath)
		{
			//	Cherche l'index d'un chemin de Druids dans la table des champs.
			for (int i=0; i<this.tableContent.Count; i++)
			{
				if (druidsPath == this.tableContent[i].DruidsPath)
				{
					return i;
				}
			}

			return -1;
		}

		public struct TableItem
		{
			//	Cette structure représente un élément dans la liste de droite des champs.
			public System.Guid					Guid;
			public FieldDescription.FieldType	FieldType;
			public FieldDescription.FieldType	SourceFieldType;
			public string						Prefix;
			public string						DruidsPath;
			public int							Level;
		}
		#endregion


		#region TableRelation
		public List<RelationItem> TableRelations
		{
			//	Retourne la liste pour la table des relations. Chaque RelationItem correspondra à une ligne dans la table.
			get
			{
				return this.tableRelations;
			}
		}

		public string GetTableRelationDescription(int index)
		{
			//	Retourne le texte permettant de décrire une relation dans une liste, avec un effet
			//	d'indentation pour ressembler aux arborescences de Vista.
			string druidsPath = this.tableRelations[index].DruidsPath;
			string name = this.formEditor.Module.AccessFields.GetFieldNames(druidsPath);

			string nextDruidsPath = null;
			if (index+1 < this.tableRelations.Count)
			{
				nextDruidsPath = this.tableRelations[index+1].DruidsPath;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			for (int i=0; i<this.tableRelations[index].Level; i++)
			{
				builder.Append(Misc.Image("TreeSpace"));
			}

			if (this.tableRelations[index].Expandable)
			{
				if (this.tableRelations[index].Expanded)
				{
					builder.Append(Misc.Image("TreeBranch"));
					name = Misc.Bold(name);
				}
				else
				{
					builder.Append(Misc.Image("TreeCompact"));
				}
			}
			else
			{
				//?builder.Append(Misc.Image("TreeMark"));
				builder.Append(Misc.Image("TreeSpace"));
			}

			builder.Append(" ");
			builder.Append(name);

			return builder.ToString();
		}

		public string GetTableRelationRelIcon(int index)
		{
			//	Retourne le texte "icône" permettant de décrire une relation dans une liste.
			FieldRelation rel = this.tableRelations[index].Relation;

			string icon = null;

			if (rel == FieldRelation.Reference)
			{
				icon = Misc.Image("TreeRelationReference");
			}
			else if (rel == FieldRelation.Collection)
			{
				icon = Misc.Image("TreeRelationCollection");
			}

			return icon;
		}

		public string GetTableRelationUseIcon(int index)
		{
			//	Retourne le texte "icône" permettant de décrire l'utilisation dans une liste.
			string icon = null;

			if (this.IsTableRelationUsed(index))
			{
				icon = Misc.Image("ActiveYes");
			}
			else
			{
				icon = Misc.Image("ActiveNo");
			}

			return icon;
		}

		public Color GetTableRelationUseColor(int index)
		{
			//	Retourne la couleur décrivant l'utilisation d'une relation.
			if (this.IsTableRelationUsed(index))
			{
				return Color.FromAlphaRgb(0.3, 0, 1, 0);  // vert = champ utilisé
			}
			else
			{
				return Color.FromAlphaRgb(0.3, 1, 0, 0);  // rouge = champ inutilisé
			}
		}

		public bool IsTableRelationUseable(int index)
		{
			//	Indique si l'opération "utiliser" est autorisée.
			if (index == -1 || this.formEditor.Module.DesignerApplication.IsReadonly)
			{
				return false;
			}

			return !this.IsTableRelationUsed(index);
		}

		public bool IsTableRelationExpandable(int index)
		{
			//	Indique si l'opération "étendre" est autorisée.
			if (index == -1 || this.formEditor.Module.DesignerApplication.IsReadonly)
			{
				return false;
			}

			return this.tableRelations[index].Expandable && !this.tableRelations[index].Expanded;
		}

		public bool IsTableRelationCompactable(int index)
		{
			//	Indique si l'opération "compacter" est autorisée.
			if (index == -1 || this.formEditor.Module.DesignerApplication.IsReadonly)
			{
				return false;
			}

			return this.tableRelations[index].Expanded;
		}

		public void TableRelationExpand(int index)
		{
			//	Etend une relation.
			string druidsPath = this.tableRelations[index].DruidsPath;
			IList<StructuredData> dataFields = this.TableRelationSearchStructuredData(druidsPath);

			this.tableRelations[index].Expanded = true;
			int level = this.tableRelations[index].Level + 1;

			foreach (StructuredData dataField in dataFields)
			{
				FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
				Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

				RelationItem item = new RelationItem();
				item.DruidsPath = string.Concat(druidsPath, ".", fieldCaptionId.ToString());
				item.typeId = typeId;
				item.Relation = rel;
				item.Expandable = (rel != FieldRelation.None);
				item.Expanded = false;
				item.Level = level;

				index++;
				this.tableRelations.Insert(index, item);
			}
		}

		public void TableRelationCompact(int index)
		{
			//	Compacte une relation.
			string druidsPath = this.tableRelations[index].DruidsPath;
			IList<StructuredData> dataFields = this.TableRelationSearchStructuredData(druidsPath);

			this.tableRelations[index].Expanded = false;

			while (index+1 < this.tableRelations.Count)
			{
				if (this.tableRelations[index+1].DruidsPath.StartsWith(this.tableRelations[index].DruidsPath))
				{
					this.tableRelations.RemoveAt(index+1);
				}
				else
				{
					break;
				}
			}
		}

		public void UpdateTableRelation(Druid entityId, IList<StructuredData> entityFields, FormDescription form)
		{
			//	Initialise la table des relations possibles avec le premier niveau et les champs du masque.
			//	Tous les champs frères d'un champ faisant partie du masque sont également pris.
			this.UpdateTableRelation(entityId, entityFields);

			foreach (FieldDescription field in form.Fields)
			{
				if (field.Type != FieldDescription.FieldType.Field &&
					field.Type != FieldDescription.FieldType.SubForm)
				{
					continue;
				}

				string druidsPath = field.GetPath(null);
				int deep = Misc.DruidsPathLevel(druidsPath)-1;

				int existingIndex = this.TableRelationSearchIndex(Misc.DruidsPathPart(druidsPath, deep));
				if (existingIndex != -1 && this.tableRelations[existingIndex].Expanded)
				{
					continue;
				}

				int first = -1;
				for (int i=deep; i>0; i--)
				{
					string subDruidsPath = Misc.DruidsPathPart(druidsPath, i);
					int index = this.TableRelationSearchIndex(subDruidsPath);
					if (index != -1)
					{
						first = i;
					}
				}

				if (first == -1)
				{
					continue;
				}

				for (int i=first; i<=deep; i++)
				{
					string subDruidsPath = Misc.DruidsPathPart(druidsPath, i);
					int index = this.TableRelationSearchIndex(subDruidsPath);

					if (this.tableRelations[index].Expanded)
					{
						continue;
					}

					this.tableRelations[index].Expanded = true;

					IList<StructuredData> dataFields = this.TableRelationSearchStructuredData(subDruidsPath);
					foreach (StructuredData dataField in dataFields)
					{
						FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
						Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
						Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

						RelationItem item = new RelationItem();
						item.DruidsPath = string.Concat(subDruidsPath, ".", fieldCaptionId.ToString());
						item.typeId = typeId;
						item.Relation = rel;
						item.Expandable = (rel != FieldRelation.None);
						item.Expanded = false;
						item.Level = i;

						this.tableRelations.Insert(++index, item);
					}
				}
			}
		}

		public void UpdateTableRelation(Druid entityId, IList<StructuredData> entityFields)
		{
			//	Initialise la table des relations possibles avec le premier niveau
			//	(mais seulement les champs sans relation).
			this.entityId = entityId;

			this.tableRelations = new List<RelationItem>();

			if (entityFields == null)
			{
				return;
			}

			foreach (StructuredData dataField in entityFields)
			{
				FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
				Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

				RelationItem item = new RelationItem();
				item.DruidsPath = fieldCaptionId.ToString();
				item.typeId = typeId;
				item.Relation = rel;
				item.Expandable = (rel != FieldRelation.None);
				item.Expanded = false;
				item.Level = 0;

				this.tableRelations.Add(item);
			}
		}

		protected bool IsTableRelationUsed(int index)
		{
			//	Indique si une relation est utilisée dans le masque de saisie.
			string druidsPath = this.tableRelations[index].DruidsPath;

			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				if (field.GetPath(null) == druidsPath)
				{
					return true;
				}
			}

			return false;
		}

		protected int TableRelationSearchIndex(string druidsPath)
		{
			//	Cherche l'index correspondant à un chemin de Druids.
			for (int i=0; i<this.tableRelations.Count; i++)
			{
				if (this.tableRelations[i].DruidsPath == druidsPath)
				{
					return i;
				}
			}

			return -1;
		}

		protected IList<StructuredData> TableRelationSearchStructuredData(string druidsPath)
		{
			string[] druids = druidsPath.Split('.');

			IList<StructuredData> dataFields = this.formEditor.Module.AccessEntities.GetEntityDruidsPath(this.entityId);
			foreach (string druid in druids)
			{
				StructuredData dataField = this.TableRelationSearchStructuredData(dataFields, Druid.Parse(druid));
				Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
				Module typeModule = this.formEditor.Module.DesignerApplication.SearchModule(typeId);
				if (typeModule != null)
				{
					CultureMap typeCultureMap = typeModule.AccessEntities.Accessor.Collection[typeId];
					if (typeCultureMap != null)
					{
						StructuredData typeData = typeCultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
						dataFields = typeData.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
					}
				}
			}

			return dataFields;
		}

		protected StructuredData TableRelationSearchStructuredData(IList<StructuredData> dataFields, Druid fieldId)
		{
			foreach (StructuredData dataField in dataFields)
			{
				Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
				if (fieldCaptionId == fieldId)
				{
					return dataField;
				}
			}

			return null;
		}

		public class RelationItem
		{
			//	Cette classe représente une ligne dans la table des relations.
			public string						DruidsPath;
			public Druid						typeId;
			public FieldRelation				Relation;
			public bool							Expandable;
			public bool							Expanded;
			public int							Level;
		}
		#endregion


		protected Editor							formEditor;
		protected Druid								entityId;
		protected List<TableItem>					tableContent;
		protected List<RelationItem>				tableRelations;
	}
}
