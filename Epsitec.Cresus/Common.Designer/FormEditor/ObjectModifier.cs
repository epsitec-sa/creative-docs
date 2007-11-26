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

			this.formDruid = Druid.Empty;
			this.tableContent = new List<TableItem>();
			this.dicoLocalGuids = new Dictionary<string, System.Guid>();
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

			return field.Type == FieldDescription.FieldType.BoxBegin;
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


		public FrameState GetContainerFrameState(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FrameState.None;
			}
			else
			{
				return field.ContainerFrameState;
			}
		}

		public void SetContainerFrameState(Widget obj, FrameState state)
		{
			//	Choix de la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.ContainerFrameState = state;
			}
		}


		public double GetContainerFrameWidth(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return 0.0;
			}
			else
			{
				return (field.ContainerFrameWidth+1)/2;
			}
		}

		public void SetContainerFrameWidth(Widget obj, double width)
		{
			//	Choix de la couleur de fond d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.ContainerFrameWidth = 2*width-1;
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
			if (obj.Name == "GlueNull")  // objet FieldDescription.FieldType.Glue ?
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
		public string GetTableContentDescription(TableItem item)
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
					if (field.Type == FieldDescription.FieldType.BoxBegin)
					{
						name = Misc.Bold(field.Description);
					}
					else
					{
						name = Misc.Italic(field.Description);
					}
				}
			}

			builder.Append(" ");
			builder.Append(name);

			return builder.ToString();
		}

		public void UpdateTableContent(Druid formDruid, List<string> entityDruidsPath)
		{
			//	Met à jour la liste qui reflète le contenu de la table des champs, visible en haut à droite.
			if (this.formEditor.Form == null || entityDruidsPath == null)
			{
				return;
			}

			if (this.formDruid != formDruid)  // a-t-on changé de formulaire ?
			{
				this.formDruid = formDruid;

				//	Attribue un nouveau Guid pour chaque champ (défini par un chemin de Druids)
				//	trouvé dans l'entité. Ceci est fait pour tous les champs de l'entité, et non
				//	seulement pour ceux qui sont utilisés dans le Form.
				this.dicoLocalGuids.Clear();
				foreach (string druidPath in entityDruidsPath)
				{
					this.dicoLocalGuids.Add(druidPath, System.Guid.NewGuid());
				}
			}

			this.tableContent.Clear();

			//	Construit la liste des chemins de Druids, en commençant par ceux qui font
			//	partie du masque de saisie.
			int level = 0;
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				System.Guid guid = this.GetLocalGuid(field.GetPath(null));

				if (guid == System.Guid.Empty)
				{
					// Si le Guid n'a pas été défini localement, parce qu'il s'agit d'un séparateur ou d'un titre,
					// conserve le Guid défini dans le Form.
					guid = field.Guid;
				}
				else
				{
					// Si le Guid a été défini localement, utilise-le à la place de celui défini dans le Form.
					// On garanti ainsi que le champ aura toujours le même Guid, qu'il soit utilisé (c'est-à-dire
					// dans le Form) ou non.
					field.Guid = guid;
				}

				TableItem item = new TableItem();
				item.Guid = guid;
				item.FieldType = field.Type;
				item.DruidsPath = field.GetPath(null);
				item.Used = true;
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

			//	Complète ensuite par tous les autres.
			foreach (string druidPath in entityDruidsPath)
			{
				if (this.GetTableContentIndex(druidPath) == -1)
				{
					TableItem item = new TableItem();
					item.Guid = this.GetLocalGuid(druidPath);
					item.FieldType = FieldDescription.FieldType.Field;
					item.DruidsPath = druidPath;
					item.Used = false;

					this.tableContent.Add(item);
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
			public bool IsEmpty
			{
				get
				{
					return this.Guid == System.Guid.Empty;
				}
			}

			public static TableItem Empty
			{
				get
				{
					return new TableItem();
				}
			}

			public System.Guid					Guid;
			public FieldDescription.FieldType	FieldType;
			public string						DruidsPath;
			public bool							Used;
			public int							Level;
		}

		protected System.Guid GetLocalGuid(string druidsPath)
		{
			//	Cherche le Guid défini localement dans le dictionnaire.
			if (druidsPath == null)
			{
				return System.Guid.Empty;
			}
			else
			{
				return this.dicoLocalGuids[druidsPath];
			}
		}
		#endregion


		protected Editor							formEditor;
		protected Druid								formDruid;
		protected List<TableItem>					tableContent;
		protected Dictionary<string, System.Guid>	dicoLocalGuids;
	}
}
