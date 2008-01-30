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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.ColumnsRequired = columnsRequired;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.RowsRequired = rowsRequired;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.SeparatorBottom = sep;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxPadding = type;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BackColor = color;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxFrameState = state;
				this.PatchUpdate(field);
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
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.BoxFrameWidth = 2*width-1;
				this.PatchUpdate(field);
			}
		}


		protected void PatchUpdate(FieldDescription field)
		{
			//	Si on est dans un masque de patch, le champ modifié doit être copié dans la liste de patch.
			if (this.IsPatch)
			{
				int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.Form.Fields, field.Guid);
				if (index == -1)
				{
					FieldDescription copy = new FieldDescription(field);
					copy.PatchModified = true;
					this.formEditor.Form.Fields.Add(copy);

					this.formEditor.OnUpdateCommands();  // pour mettre à jour le bouton fieldsButtonReset
				}
				else
				{
					FieldDescription original = this.formEditor.Form.Fields[index];

					FieldDescription copy = new FieldDescription(field);
					copy.PatchModified = true;
					copy.PatchMoved = original.PatchMoved;
					copy.PatchAttachGuid = original.PatchAttachGuid;
					this.formEditor.Form.Fields[index] = copy;
				}
			}
		}


		public Widget GetWidget(System.Guid guid)
		{
			//	Cherche le widget correspondant à un Guid.
			foreach (FieldDescription field in this.fields)
			{
				if (guid == field.Guid)
				{
					Widget obj = this.GetWidget(this.formEditor.Panel, field.Guid);
					if (obj != null)
					{
						return obj;
					}
				}
			}

			return null;
		}

		protected Widget GetWidget(Widget parent, System.Guid guid)
		{
			if (parent.Name == guid.ToString())
			{
				return parent;
			}

			Widget[] children = parent.Children.Widgets;
			for (int i=0; i<children.Length; i++)
			{
				Widget widget = this.GetWidget(children[i], guid);
				if (widget != null)
				{
					return widget;
				}
			}

			return null;
		}


		public int GetFormCount
		{
			//	Retourne le nombre de champs.
			get
			{
				return this.fields.Count;
			}
		}

		public FieldDescription GetFormDescription(TableItem item)
		{
			//	Retourne un champ d'après son TableItem.
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
				return this.fields[index];
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
				return this.fields[index];
			}
		}

		public int GetFormDescriptionIndex(Widget obj)
		{
			//	Retourne l'index d'un champ d'après l'identificateur unique d'un widget.
			if (string.IsNullOrEmpty(obj.Name))
			{
				return -1;
			}

			System.Guid guid = new System.Guid(obj.Name);
			return this.GetFormDescriptionIndex(guid);
		}

		public int GetFormDescriptionIndex(System.Guid guid)
		{
			//	Retourne l'index d'un champ d'après le Guid.
			for (int i=0; i<this.fields.Count; i++)
			{
				FieldDescription field = this.fields[i];

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
			for (int i=0; i<this.fields.Count; i++)
			{
				FieldDescription field = this.fields[i];

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
			if (obj.Index == FormEngine.Engine.GlueNull)  // objet FieldDescription.FieldType.Glue avec un ColumnRequired = 0 ?
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


		public bool IsReadonly
		{
			get
			{
				return this.formEditor.Module.DesignerApplication.IsReadonly;
			}
		}


		#region TableContent
		public void FormPatchMove(int index, int direction)
		{
			//	Déplace un élément en le montant ou en le descendant d'une ligne. Pour cela, les déplacements
			//	de tous les éléments sont recalculés, à partir de la situation initiale (non patchée).
			//	this.referenceFields		= liste initiale non patchée
			//	this.formEditor.Form.Fields	= liste de patch
			//	this.fields					= liste résultante

			//	Construit la liste originale.
			List<System.Guid> original = new List<System.Guid>();
			foreach (FieldDescription field in this.referenceFields)
			{
				original.Add(field.Guid);
			}

			//	Construit la liste finale souhaitée.
			List<System.Guid> final = new List<System.Guid>();
			foreach (FieldDescription field in this.fields)
			{
				final.Add(field.Guid);
			}
			System.Guid guid = final[index];
			final.RemoveAt(index);
			final.Insert(index+direction, guid);  // effectue le mouvement (monter ou descendre un élément)

			//	Construit la liste de tous les éléments déplacés.
			List<System.Guid> moved = new List<System.Guid>();

			moved.Add(guid);  // ce n'est pas grave si cet élément est 2x dans la liste

			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				if (field.PatchMoved || field.PatchInserted)
				{
					moved.Add(field.Guid);
				}
			}

			//	Si l'élément déplacé est cassé, enlève immédiatement son état cassé.
			index = FormEngine.Arrange.IndexOfGuid(this.formEditor.Form.Fields, guid);
			if (index != -1)
			{
				this.formEditor.Form.Fields[index].PatchBrokenAttach = false;
			}

			//	Génére la liste des opérations de déplacement nécessaires.
			//	1) Traite d'abord les éléments qu'on sait devoir déplacer (selon la liste moved).
			List<LinkAfter> oper = new List<LinkAfter>();
			index = 0;
			while (index < final.Count)
			{
				if (moved.Contains(final[index]) && (index >= original.Count || original[index] != final[index]))
				{
					System.Guid element = final[index];  // élément à déplacer
					System.Guid link = (index==0) ? System.Guid.Empty : final[index-1];  // *après* cet élément
					bool isInserted = !original.Contains(element);  // true = nouvel élément, false = élément existant
					oper.Insert(0, new LinkAfter(element, link, isInserted));  // insère au début

					final.RemoveAt(index);  // supprime l'élément déplacé

					int i = original.IndexOf(element);
					if (i != -1)
					{
						original.RemoveAt(i);  // supprime l'élément déplacé
					}
				}
				else
				{
					index++;
				}
			}

			//	2) Traite ensuite les éventuels autres éléments.
			index = 0;
			while (index < final.Count)
			{
				if (index < original.Count && final[index] == original[index])  // élément en place ?
				{
					index++;  // passe au suivant
				}
				else  // élément déplacé ?
				{
					System.Guid element = final[index];  // élément à déplacer
					System.Guid link = (index==0) ? System.Guid.Empty : final[index-1];  // *après* cet élément
					bool isInserted = !original.Contains(element);  // true = nouvel élément, false = élément existant
					oper.Insert(0, new LinkAfter(element, link, isInserted));  // insère au début

					final.RemoveAt(index);  // supprime l'élément déplacé

					int i = original.IndexOf(element);
					if (i != -1)
					{
						original.RemoveAt(i);  // supprime l'élément déplacé
					}
				}
			}

			//	Supprime tous les déplacements dans la liste de patch actuelle, mais en conservant les éléments.
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				field.PatchMoved = false;
				field.PatchInserted = false;
			}

			//	Remet tous les déplacements selon la liste des opérations.
			foreach (LinkAfter item in oper)
			{
				int i = FormEngine.Arrange.IndexOfGuid(this.formEditor.Form.Fields, item.Element);
				if (i == -1)  // élément pas (ou pas encore) patché ?
				{
					i = FormEngine.Arrange.IndexOfGuid(this.referenceFields, item.Element);
					if (i != -1)  // élément dans la liste de référence ?
					{
						FieldDescription field = new FieldDescription(this.referenceFields[i]);  // copie de l'élément

						field.PatchMoved = !item.IsInserted;
						field.PatchInserted = item.IsInserted;
						field.PatchAttachGuid = item.Link;

						this.formEditor.Form.Fields.Add(field);  // met l'élément à la fin de la liste de patch
					}
				}
				else  // élément déjà dans la liste de patch ?
				{
					FieldDescription field = this.formEditor.Form.Fields[i];
					this.formEditor.Form.Fields.RemoveAt(i);  // supprime l'élément dans la liste de patch

					field.PatchMoved = !item.IsInserted;
					field.PatchInserted = item.IsInserted;
					field.PatchAttachGuid = item.Link;

					this.formEditor.Form.Fields.Add(field);  // remet l'élément à la fin de la liste de patch
				}
			}

			//	Supprime réellement tous les éléments dans la liste de patch qui n'ont plus aucune fonction.
			index = 0;
			while (index < this.formEditor.Form.Fields.Count)
			{
				FieldDescription field = this.formEditor.Form.Fields[index];

				if (!field.PatchMoved && !field.PatchInserted && !field.PatchModified && !field.PatchHidden && !field.PatchBrokenAttach)
				{
					this.formEditor.Form.Fields.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}
		}

		protected struct LinkAfter
		{
			public LinkAfter(System.Guid element, System.Guid link, bool isInserted)
			{
				this.Element = element;
				this.Link = link;
				this.IsInserted = isInserted;
			}

			public System.Guid Element;  // élément à déplacer
			public System.Guid Link;  // *après* cet élément
			public bool IsInserted;  // true = nouvel élément, false = élément existant 
		}

		public string GetTableContentDescription(TableItem item, bool isImage, bool isShowPrefix, bool isShowGuid)
		{
			//	Retourne le texte permettant de décrire un TableItem dans une liste, avec un effet
			//	d'indentation pour ressembler aux arborescences de Vista.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if (isImage)
			{
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

				builder.Append(" ");
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
				if (item.Prefix != null && isShowPrefix)
				{
					name = string.Concat(item.Prefix, name);
				}

				if (isShowGuid)
				{
					name = string.Concat(item.Guid.ToString(), " ", name);
				}
			}

			builder.Append(name);

			return builder.ToString();
		}

		public string GetTableContentIcon1(TableItem item)
		{
			//	Retourne le texte permettant de décrire l'opération de patch d'un TableItem dans une liste.
			string icon = null;

			if (this.IsPatch)
			{
				int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.Form.Fields, item.Guid);
				if (index != -1)
				{
					if (this.formEditor.Form.Fields[index].PatchHidden)
					{
						icon = Misc.Image("FormPatchHidden");  // peu prioritaire à cause du fond rouge
					}

					if (this.formEditor.Form.Fields[index].PatchInserted)
					{
						icon = Misc.Image("FormPatchInserted");  // peu prioritaire à cause du fond vert
					}

					if (this.formEditor.Form.Fields[index].PatchMoved)
					{
						icon = Misc.Image("FormPatchMoved");  // prioritaire, car pas de fond coloré
					}
				}
			}

			return icon;
		}

		public string GetTableContentIcon2(TableItem item)
		{
			//	Retourne le texte permettant de décrire la relation d'un TableItem dans une liste.
			string icon = null;

			if (item.FieldType == FieldDescription.FieldType.SubForm)
			{
				icon = Misc.Image("TreeSubForm");
			}

			return icon;
		}

		public Color GetTableContentUseColor(TableItem item)
		{
			//	Retourne la couleur décrivant un TableItem dans une liste.
			Color color = Color.Empty;

			if (this.IsPatch)
			{
				int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.Form.Fields, item.Guid);
				if (index != -1)
				{
					if (this.formEditor.Form.Fields[index].PatchHidden)
					{
						color = Color.FromAlphaRgb(0.3, 1, 0, 0);  // rouge = champ caché
					}

					if (this.formEditor.Form.Fields[index].PatchModified)
					{
						color = Color.FromAlphaRgb(0.3, 1, 1, 0);  // jaune = champ modifié
					}

					if (this.formEditor.Form.Fields[index].PatchInserted)
					{
						color = Color.FromAlphaRgb(0.3, 0, 1, 0);  // vert = champ inséré
					}


					if (this.formEditor.Form.Fields[index].PatchBrokenAttach)
					{
						color = Color.FromAlphaRgb(0.8, 1, 0, 0);  // rouge foncé = lien cassé
					}
				}
			}

			return color;
		}

		public bool IsPatch
		{
			//	Indique si l'on est dans un masque de patch.
			get
			{
				if (this.formEditor.Form == null)
				{
					return false;
				}
				else
				{
					return this.formEditor.Form.IsPatch;
				}
			}
		}

		public void UpdateTableContent()
		{
			//	Met à jour la liste qui reflète le contenu de la table des champs, visible en haut à droite.
			if (this.formEditor.Form == null)
			{
				return;
			}

			if (this.IsPatch)  // masque de patch ?
			{
				FormEngine.FormDescription refForm = this.formEditor.Module.AccessForms.GetForm(this.formEditor.Form.FormIdToPatch);
				this.referenceFields = refForm.Fields;

				FormEngine.Engine engine = new FormEngine.Engine(this.formEditor.Module.FormResourceProvider);
				this.fields = engine.Arrange.Merge(this.referenceFields, this.formEditor.Form.Fields);
			}
			else  // masque normal ?
			{
				this.fields = this.formEditor.Form.Fields;
				this.referenceFields = null;
			}

			this.tableContent.Clear();

			//	Construit la liste des chemins de Druids, en commençant par ceux qui font
			//	partie du masque de saisie.
			int level = 0;
			foreach (FieldDescription field in this.fields)
			{
				string prefix = null;
				if (field.FieldIds != null)
				{
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
			if (index == -1 || this.IsReadonly)
			{
				return false;
			}

			return !this.IsTableRelationUsed(index);
		}

		public bool IsTableRelationExpandable(int index)
		{
			//	Indique si l'opération "étendre" est autorisée.
			if (index == -1 || this.IsReadonly)
			{
				return false;
			}

			return this.tableRelations[index].Expandable && !this.tableRelations[index].Expanded;
		}

		public bool IsTableRelationCompactable(int index)
		{
			//	Indique si l'opération "compacter" est autorisée.
			if (index == -1 || this.IsReadonly)
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

			foreach (FieldDescription field in this.fields)
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
		protected List<FieldDescription>			fields;
		protected List<FieldDescription>			referenceFields;
		protected List<TableItem>					tableContent;
		protected List<RelationItem>				tableRelations;
	}
}
