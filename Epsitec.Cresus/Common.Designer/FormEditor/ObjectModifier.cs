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
	/// La classe ObjectModifier permet de g�rer les 'widgets' de Designer.
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
			//	Indique si l'objet correspond � un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Field;
		}

		public bool IsBox(Widget obj)
		{
			//	Indique si l'objet correspond � une bo�te.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.BoxBegin;
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


		public FieldDescription.SeparatorType GetSeparator(Widget obj)
		{
			//	Retourne le type du s�parateur d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field == null)
			{
				return FieldDescription.SeparatorType.Normal;
			}
			else
			{
				return field.Separator;
			}
		}

		public void SetSeparator(Widget obj, FieldDescription.SeparatorType sep)
		{
			//	Choix du type du s�parateur d'un champ.
			FieldDescription field = this.GetFormDescription(obj);
			if (field != null)
			{
				field.Separator = sep;
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


		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les bo�tes.
			if (this.IsBox(obj))
			{
				FieldDescription field = this.GetFormDescription(obj);
				return field.ContainerMargins;
			}

			return Margins.Zero;
		}

		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les bo�tes.
			System.Diagnostics.Debug.Assert(this.IsBox(obj));

			FieldDescription field = this.GetFormDescription(obj);
			field.ContainerMargins = margins;
		}


		public Widget GetWidget(string druidPath)
		{
			//	Cherche le widget correspondant � un chemin de Druids.
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				if (druidPath == field.GetPath(null))
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
			//	Cherche le widget correspondant � un identificateur unique.
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


		public FieldDescription GetFormDescription(Widget obj)
		{
			//	Retourne un champ d'apr�s l'identificateur unique d'un widget.
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
			//	Retourne l'index d'un champ d'apr�s l'identificateur unique d'un widget.
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

		public int GetFormDescriptionIndex(string druidsPath)
		{
			//	Retourne l'index d'un champ d'apr�s le chemin de Druis.
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
			//	Le rectangle rendu est toujours valide, quel que soit le mode d'attachement.
			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != null && obj != this.formEditor.Panel)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}


		#region TableContent
		public void UpdateTableContent(List<string> entityDruidsPath)
		{
			//	Met � jour la liste qui refl�te le contenu de la table des champs, visible en haut � droite.
			this.tableContent.Clear();

			if (this.formEditor.Form == null || entityDruidsPath == null)
			{
				return;
			}

			//	Construit la liste des chemins de Druids, en commen�ant par ceux qui font
			//	partie du masque de saisie.
			foreach (FieldDescription field in this.formEditor.Form.Fields)
			{
				TableItem item = new TableItem();
				item.DruidsPath = field.GetPath(null);
				item.Used = true;

				this.tableContent.Add(item);
			}

			//	Compl�te ensuite par tous les autres.
			foreach (string druidPath in entityDruidsPath)
			{
				if (this.GetTableContentIndex(druidPath) == -1)
				{
					TableItem item = new TableItem();
					item.DruidsPath = druidPath;
					item.Used = false;

					this.tableContent.Add(item);
				}
			}
		}

		public int GetTableContentIndex(string druidsPath)
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
			//	Cette structure repr�sente un �l�ment dans la liste de droite des champs.
			public string DruidsPath;
			public bool Used;

			public bool IsEmpty
			{
				get
				{
					return this.DruidsPath == null;
				}
			}

			public static TableItem Empty
			{
				get
				{
					return new TableItem();
				}
			}
		}
		#endregion


		protected Editor				formEditor;
		protected List<TableItem>		tableContent;
	}
}
