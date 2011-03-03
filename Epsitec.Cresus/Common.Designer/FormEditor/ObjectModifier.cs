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


		public Editor FormEditor
		{
			get
			{
				return this.formEditor;
			}
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
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Field;
		}

		public bool IsCommand(Widget obj)
		{
			//	Indique si l'objet correspond à une commande.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Command;
		}

		public bool IsBox(Widget obj)
		{
			//	Indique si l'objet correspond à une boîte.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.BoxBegin || field.Type == FieldDescription.FieldType.SubForm;
		}

		public bool IsGlue(Widget obj)
		{
			//	Indique si l'objet correspond à de la colle.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Glue;
		}

		public bool IsTitle(Widget obj)
		{
			//	Indique si l'objet correspond à un titre.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Title;
		}

		public bool IsLine(Widget obj)
		{
			//	Indique si l'objet correspond à une ligne.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return false;
			}

			return field.Type == FieldDescription.FieldType.Line;
		}


		public int GetColumnsRequired(Widget obj)
		{
			//	Retourne le nombre de colonnes requises.
			FieldDescription field = this.GetFieldDescription(obj);
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

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.ColumnsRequired);
				field.ColumnsRequired = columnsRequired;
				this.DeltaUpdate(field);
			}
		}

		public int GetRowsRequired(Widget obj)
		{
			//	Retourne le nombre de lignes requises.
			FieldDescription field = this.GetFieldDescription(obj);
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

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.RowsRequired);
				field.RowsRequired = rowsRequired;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.SeparatorType GetSeparatorBottom(Widget obj)
		{
			//	Retourne le type du séparateur d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
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

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.SeparatorBottom);
				field.SeparatorBottom = sep;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.BoxLayoutType GetBoxLayout(Widget obj)
		{
			//	Retourne le type du contenu d'une boîte.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.BoxLayoutType.Grid;
			}
			else
			{
				return field.BoxLayout;
			}
		}

		public void SetBoxLayout(Widget obj, FieldDescription.BoxLayoutType type)
		{
			//	Choix du type du contenu d'une boîte.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.BoxLayout);
				field.BoxLayout = type;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.BoxPaddingType GetBoxPadding(Widget obj)
		{
			//	Retourne le type des marges intérieures d'une boîte.
			FieldDescription field = this.GetFieldDescription(obj);
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

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.BoxPadding);
				field.BoxPadding = type;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.BackColorType GetBackColor(Widget obj)
		{
			//	Retourne la couleur de fond d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
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

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.BackColor);
				field.BackColor = color;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.FontColorType GetLabelFontColor(Widget obj)
		{
			//	Retourne la couleur de la police d'une étiquette.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontColorType.Default;
			}
			else
			{
				return field.LabelFontColor;
			}
		}

		public void SetLabelFontColor(Widget obj, FieldDescription.FontColorType color)
		{
			//	Choix de la couleur de la police d'une étiquette.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LabelFontColor);
				field.LabelFontColor = color;
				this.DeltaUpdate(field);
			}
		}

		public FieldDescription.FontColorType GetFieldFontColor(Widget obj)
		{
			//	Retourne la couleur de la police d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontColorType.Default;
			}
			else
			{
				return field.FieldFontColor;
			}
		}

		public void SetFieldFontColor(Widget obj, FieldDescription.FontColorType color)
		{
			//	Choix de la couleur de la police d'un champ.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.FieldFontColor);
				field.FieldFontColor = color;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.FontFaceType GetLabelFontFace(Widget obj)
		{
			//	Retourne le nom de la police d'une étiquette.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontFaceType.Default;
			}
			else
			{
				return field.LabelFontFace;
			}
		}

		public void SetLabelFontFace(Widget obj, FieldDescription.FontFaceType face)
		{
			//	Choix du nom de la police d'une étiquette.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LabelFontFace);
				field.LabelFontFace = face;
				this.DeltaUpdate(field);
			}
		}

		public FieldDescription.FontFaceType GetFieldFontFace(Widget obj)
		{
			//	Retourne le nom de la police d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontFaceType.Default;
			}
			else
			{
				return field.FieldFontFace;
			}
		}

		public void SetFieldFontFace(Widget obj, FieldDescription.FontFaceType face)
		{
			//	Choix du nom de la police d'un champ.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.FieldFontFace);
				field.FieldFontFace = face;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.FontStyleType GetLabelFontStyle(Widget obj)
		{
			//	Retourne le style de la police d'une étiquette.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontStyleType.Normal;
			}
			else
			{
				return field.LabelFontStyle;
			}
		}

		public void SetLabelFontStyle(Widget obj, FieldDescription.FontStyleType style)
		{
			//	Choix du style de la police d'une étiquette.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LabelFontStyle);
				field.LabelFontStyle = style;
				this.DeltaUpdate(field);
			}
		}

		public FieldDescription.FontStyleType GetFieldFontStyle(Widget obj)
		{
			//	Retourne le style de la police d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontStyleType.Normal;
			}
			else
			{
				return field.FieldFontStyle;
			}
		}

		public void SetFieldFontStyle(Widget obj, FieldDescription.FontStyleType style)
		{
			//	Choix du style de la police d'un champ.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.FieldFontStyle);
				field.FieldFontStyle = style;
				this.DeltaUpdate(field);
			}
		}


		public FieldDescription.FontSizeType GetLabelFontSize(Widget obj)
		{
			//	Retourne la taille de la police d'une étiquette.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontSizeType.Normal;
			}
			else
			{
				return field.LabelFontSize;
			}
		}

		public void SetLabelFontSize(Widget obj, FieldDescription.FontSizeType size)
		{
			//	Choix de la taille de la police d'une étiquette.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LabelFontSize);
				field.LabelFontSize = size;
				this.DeltaUpdate(field);
			}
		}

		public FieldDescription.FontSizeType GetFieldFontSize(Widget obj)
		{
			//	Retourne la taille de la police d'un champ.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.FontSizeType.Normal;
			}
			else
			{
				return field.FieldFontSize;
			}
		}

		public void SetFieldFontSize(Widget obj, FieldDescription.FontSizeType size)
		{
			//	Choix de la taille de la police d'un champ.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.FieldFontSize);
				field.FieldFontSize = size;
				this.DeltaUpdate(field);
			}
		}

		public FieldDescription.CommandButtonClass GetCommandButtonClass(Widget obj)
		{
			//	Retourne le type d'une commande.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return FieldDescription.CommandButtonClass.Default;
			}
			else
			{
				return field.CommandButtonClassValue;
			}
		}

		public void SetCommandButtonClass(Widget obj, FieldDescription.CommandButtonClass type)
		{
			//	Choix du type d'une commande.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.CommandButtonClass);
				field.CommandButtonClassValue = type;
				this.DeltaUpdate(field);
			}
		}


		public FrameState GetBoxFrameState(Widget obj)
		{
			//	Retourne le type du cadre d'une boîte.
			FieldDescription field = this.GetFieldDescription(obj);
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
			//	Choix du type du cadre d'une boîte.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.BoxFrameState);
				field.BoxFrameState = state;
				this.DeltaUpdate(field);
			}
		}


		public double GetBoxFrameWidth(Widget obj)
		{
			//	Retourne l'épaisseur du cadre d'une boîte.
			FieldDescription field = this.GetFieldDescription(obj);
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
			//	Choix de l'épaisseur du cadre d'une boîte.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.BoxFrameWidth);
				field.BoxFrameWidth = 2*width-1;
				this.DeltaUpdate(field);
			}
		}


		public double GetLineWidth(Widget obj)
		{
			//	Retourne la largeur d'un séparateur.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return 0.0;
			}
			else
			{
				return (field.LineWidth+1)/2;
			}
		}

		public void SetLineWidth(Widget obj, double width)
		{
			//	Choix de la largeur d'un séparateur.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LineWidth);
				field.LineWidth = 2*width-1;
				this.DeltaUpdate(field);
			}
		}


		public double GetPreferredWidth(Widget obj)
		{
			//	Retourne la largeur préférentielle
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return 0.0;
			}
			else
			{
				return field.PreferredWidth;
			}
		}

		public void SetPreferredWidth(Widget obj, double width)
		{
			//	Choix de la largeur préférentielle.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.PreferredWidth);
				field.PreferredWidth = width;
				this.DeltaUpdate(field);
			}
		}


		public Druid GetLabelReplacement(Widget obj)
		{
			//	Retourne la largeur préférentielle.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return Druid.Empty;
			}
			else
			{
				return field.LabelReplacement;
			}
		}

		public void SetLabelReplacement(Widget obj, Druid captionId)
		{
			//	Choix du Druid optionnel du caption qui remplace le texte par défaut.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.LabelReplacement, false);
				field.LabelReplacement = captionId;
				this.DeltaUpdate(field);
			}
		}

		public UI.Verbosity GetVerbosity(Widget obj)
		{
			//	Retourne le mode pour le label d'un placeholder.
			FieldDescription field = this.GetFieldDescription(obj);
			if (field == null)
			{
				return UI.Verbosity.Default;
			}
			else
			{
				return field.Verbosity;
			}
		}

		public void SetVerbosity(Widget obj, UI.Verbosity verbosity)
		{
			//	Choix du mode pour le label d'un placeholder.
			if (this.IsReadonly)
			{
				return;
			}

			FieldDescription field = this.GetFieldDescription(obj);
			if (field != null)
			{
				this.UndoMemorize(Res.Strings.Undo.Action.Verbosity);
				field.Verbosity = verbosity;
				this.DeltaUpdate(field);
			}
		}


		protected void UndoMemorize(string actionName)
		{
			//	Mémorise l'état actuel, avant d'effectuer une modification dans le masque.
			this.UndoMemorize(actionName, true);
		}

		protected void UndoMemorize(string actionName, bool merge)
		{
			//	Mémorise l'état actuel, avant d'effectuer une modification dans le masque.
			this.formEditor.ViewersForms.UndoMemorize(actionName, merge);
		}

		protected void DeltaUpdate(FieldDescription field)
		{
			//	Si on est dans un masque delta, le champ modifié doit être copié dans la liste delta.
			if (this.IsDelta)
			{
				int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, field.Guid);
				if (index == -1)
				{
					FieldDescription copy = new FieldDescription(field);
					copy.DeltaModified = true;
					this.formEditor.WorkingForm.Fields.Add(copy);

					this.formEditor.OnUpdateCommands();  // pour mettre à jour le bouton fieldsButtonReset
				}
				else
				{
					FieldDescription original = this.formEditor.WorkingForm.Fields[index];

					FieldDescription copy = new FieldDescription(field);
					copy.DeltaModified = true;
					copy.DeltaMoved = original.DeltaMoved;
					copy.DeltaAttachGuid = original.DeltaAttachGuid;
					this.formEditor.WorkingForm.Fields[index] = copy;
				}
			}
		}


		public Widget GetWidget(System.Guid guid)
		{
			//	Cherche le widget correspondant à un Guid.
			if (this.formEditor.FinalFields != null)
			{
				foreach (FieldDescription field in this.formEditor.FinalFields)
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


		public int GetFieldCount
		{
			//	Retourne le nombre de champs.
			get
			{
				return this.formEditor.FinalFields.Count;
			}
		}

		public FieldDescription GetFieldDescription(TableItem item)
		{
			//	Retourne un champ d'après son TableItem.
			int index = this.GetFieldDescriptionIndex(item.Guid);

			if (index == -1)
			{
				index = this.GetFieldDescriptionIndex(item.DruidsPath);
			}

			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.FinalFields[index];
			}
		}

		public FieldDescription GetFieldDescription(Widget obj)
		{
			//	Retourne un champ d'après l'identificateur unique d'un widget.
			int index = this.GetFieldDescriptionIndex(obj);
			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.FinalFields[index];
			}
		}

		public FieldDescription GetFieldDescription(System.Guid guid)
		{
			//	Retourne un champ d'après l'identificateur unique d'un widget.
			int index = this.GetFieldDescriptionIndex(guid);
			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.FinalFields[index];
			}
		}

		public FieldDescription GetFieldDescription(int index)
		{
			//	Retourne un champ d'après l'identificateur unique d'un widget.
			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.formEditor.FinalFields[index];
			}
		}

		public int GetFieldDescriptionIndex(Widget obj)
		{
			//	Retourne l'index d'un champ d'après l'identificateur unique d'un widget.
			if (string.IsNullOrEmpty(obj.Name))
			{
				return -1;
			}

			System.Guid guid = new System.Guid(obj.Name);
			return this.GetFieldDescriptionIndex(guid);
		}

		public int GetFieldDescriptionIndex(System.Guid guid)
		{
			//	Retourne l'index d'un champ d'après le Guid.
			if (this.formEditor.FinalFields != null)
			{
				for (int i=0; i<this.formEditor.FinalFields.Count; i++)
				{
					FieldDescription field = this.formEditor.FinalFields[i];

					if (field.Guid == guid)
					{
						return i;
					}
				}
			}

			return -1;
		}

		protected int GetFieldDescriptionIndex(string druidsPath)
		{
			//	Retourne l'index d'un champ d'après le chemin de Druis.
			if (this.formEditor.FinalFields != null)
			{
				for (int i=0; i<this.formEditor.FinalFields.Count; i++)
				{
					FieldDescription field = this.formEditor.FinalFields[i];

					if (field.GetPath(null) == druidsPath)
					{
						return i;
					}
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
			//	Indique si Designer est en mode "consultation", lorsque l'identificateur est anonyme
			//	ou lorsqu'on est en mode "bloqué".
			get
			{
				return this.formEditor.Module.DesignerApplication.IsReadonly;
			}
		}

		public bool IsDelta
		{
			//	Indique si l'on est dans un masque delta.
			get
			{
				if (this.formEditor.WorkingForm == null)
				{
					return false;
				}
				else
				{
					return this.formEditor.WorkingForm.IsDelta;
				}
			}
		}


#if false
		public void ChangeForwardTab(System.Guid fieldGuid, System.Guid forwardTabGuid)
		{
			//	Modifie l'élément suivant pour la navigation avec Tab.
			this.formEditor.ViewersForms.UndoMemorize(Res.Strings.Undo.Action.ForwardTab, false);

			FieldDescription field = this.GetFieldDescription(fieldGuid);
			int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, fieldGuid);
			if (index == -1)
			{
				if (forwardTabGuid != System.Guid.Empty)
				{
					FieldDescription copy = new FieldDescription(field);
					copy.DeltaForwardTab = true;
					copy.ForwardTabGuid = forwardTabGuid;

					this.formEditor.WorkingForm.Fields.Add(copy);  // met l'élément à la fin de la liste delta
				}
			}
			else
			{
				if (forwardTabGuid == System.Guid.Empty)
				{
					this.formEditor.WorkingForm.Fields.RemoveAt(index);  // supprime l'élément dans la liste delta
				}
				else
				{
					field.ForwardTabGuid = forwardTabGuid;
				}
			}

			this.formEditor.Module.AccessForms.SetLocalDirty();
#endif


		#region TableContent
		public void FormDeltaMove(int index, int direction)
		{
			//	Déplace un élément en le montant ou en le descendant d'une ligne. Pour cela, les déplacements
			//	de tous les éléments sont recalculés, à partir de la situation initiale.
			//	this.formEditor.BaseFields			= liste initiale
			//	this.formEditor.WorkingForm.Fields	= liste delta
			//	this.formEditor.FinalFields			= liste résultante

			//	Construit la liste originale.
			List<System.Guid> original = new List<System.Guid>();
			foreach (FieldDescription field in this.formEditor.BaseFields)
			{
				original.Add(field.Guid);
			}

			//	Construit la liste finale souhaitée.
			List<System.Guid> final = new List<System.Guid>();
			foreach (FieldDescription field in this.formEditor.FinalFields)
			{
				final.Add(field.Guid);
			}
			System.Guid guid = final[index];
			final.RemoveAt(index);
			final.Insert(index+direction, guid);  // effectue le mouvement (monter ou descendre un élément)

			//	Construit la liste de tous les éléments déplacés.
			List<System.Guid> moved = new List<System.Guid>();

			moved.Add(guid);  // ce n'est pas grave si cet élément est 2x dans la liste

			foreach (FieldDescription field in this.formEditor.WorkingForm.Fields)
			{
				if (field.DeltaMoved || field.DeltaInserted)
				{
					moved.Add(field.Guid);
				}
			}

			//	Si l'élément déplacé est cassé, enlève immédiatement son état cassé.
			index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, guid);
			if (index != -1)
			{
				this.formEditor.WorkingForm.Fields[index].DeltaBrokenAttach = false;
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

			//	Supprime tous les déplacements dans la liste delta actuelle, mais en conservant les éléments.
			foreach (FieldDescription field in this.formEditor.WorkingForm.Fields)
			{
				field.DeltaMoved = false;
				field.DeltaInserted = false;
			}

			//	Remet tous les déplacements selon la liste des opérations.
			foreach (LinkAfter item in oper)
			{
				int i = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, item.Element);
				if (i == -1)  // élément pas (ou pas encore) adapté ?
				{
					i = FormEngine.Arrange.IndexOfGuid(this.formEditor.BaseFields, item.Element);
					if (i != -1)  // élément dans la liste de référence ?
					{
						FieldDescription field = new FieldDescription(this.formEditor.BaseFields[i]);  // copie de l'élément

						field.DeltaMoved = !item.IsInserted;
						field.DeltaInserted = item.IsInserted;
						field.DeltaAttachGuid = item.Link;

						this.formEditor.WorkingForm.Fields.Add(field);  // met l'élément à la fin de la liste delta
					}
				}
				else  // élément déjà dans la liste delta ?
				{
					FieldDescription field = this.formEditor.WorkingForm.Fields[i];
					this.formEditor.WorkingForm.Fields.RemoveAt(i);  // supprime l'élément dans la liste delta

					field.DeltaMoved = !item.IsInserted;
					field.DeltaInserted = item.IsInserted;
					field.DeltaAttachGuid = item.Link;

					this.formEditor.WorkingForm.Fields.Add(field);  // remet l'élément à la fin de la liste delta
				}
			}

			//	Supprime réellement tous les éléments dans la liste delta qui n'ont plus aucune fonction.
			index = 0;
			while (index < this.formEditor.WorkingForm.Fields.Count)
			{
				FieldDescription field = this.formEditor.WorkingForm.Fields[index];

				if (!field.Delta)
				{
					this.formEditor.WorkingForm.Fields.RemoveAt(index);
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

		public string GetLabelReplacementNameToCreate(Widget obj)
		{
			//	Retourne le préfixe à utiliser pour un Caption permettant de remplacer le label d'un champ dans un Form.
			CultureMap cultureMap = this.formEditor.Module.AccessForms.CollectionView.CurrentItem as CultureMap;

			FieldDescription field = this.GetFieldDescription(obj);
			int index = this.GetFieldDescriptionIndex(field.Guid);
			ObjectModifier.TableItem item = this.TableContent[index];
			string name;
			if (item.FieldType == FieldDescription.FieldType.Title)
			{
				name = "Title";
			}
			else
			{
				name = this.FormEditor.Module.AccessCaptions.GetFieldNames(item.DruidsPath);
			}

			return string.Concat("Form.LabelReplacement.", cultureMap.FullName, ".", name);
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

			FieldDescription field = this.formEditor.ObjectModifier.GetFieldDescription(item);

			string name = this.formEditor.Module.AccessFields.GetFieldNames(item.DruidsPath);
			if (name == null || (field != null && field.Type == FieldDescription.FieldType.Command))
			{
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

			if (this.IsTableContentInheritHidden(item))
			{
				builder.Append(Res.Strings.Viewers.Forms.TableContent.Hide);
			}

			return builder.ToString();
		}

		public string GetTableContentIcon1(TableItem item)
		{
			//	Retourne le texte permettant de décrire l'opération delta d'un TableItem dans une liste.
			string icon = null;

			if (this.formEditor.WorkingForm != null)
			{
				if (this.IsDelta)
				{
					int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, item.Guid);
					if (index != -1)
					{
						if (!this.IsTableContentInheritHidden(item))
						{
							if (this.formEditor.WorkingForm.Fields[index].DeltaHidden)
							{
								icon = Misc.Image("FormDeltaHidden");  // peu prioritaire à cause du fond rouge
							}

							if (this.formEditor.WorkingForm.Fields[index].DeltaShowed)
							{
								icon = Misc.Image("FormDeltaShowed");  // peu prioritaire à cause du fond rouge
							}
						}

						if (this.formEditor.WorkingForm.Fields[index].DeltaInserted)
						{
							icon = Misc.Image("FormDeltaInserted");  // peu prioritaire à cause du fond vert
						}

						if (this.formEditor.WorkingForm.Fields[index].DeltaMoved)
						{
							icon = Misc.Image("FormDeltaMoved");  // prioritaire, car pas de fond coloré
						}
					}
				}
				else
				{
					int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, item.Guid);
					if (index != -1)
					{
						if (this.formEditor.WorkingForm.Fields[index].DeltaHidden)
						{
							icon = Misc.Image("FormDeltaHidden");  // peu prioritaire à cause du fond rouge
						}
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

			if (this.formEditor.WorkingForm != null)
			{
				if (this.IsDelta)
				{
					int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, item.Guid);
					if (index != -1)
					{
						if (!this.IsTableContentInheritHidden(item))
						{
							if (this.formEditor.WorkingForm.Fields[index].DeltaHidden ||
								this.formEditor.WorkingForm.Fields[index].DeltaShowed)
							{
								color = Color.FromAlphaRgb(0.3, 1, 0, 0);  // rouge = champ caché
							}
						}

						if (this.formEditor.WorkingForm.Fields[index].DeltaModified)
						{
							color = Color.FromAlphaRgb(0.3, 1, 1, 0);  // jaune = champ modifié
						}

						if (this.formEditor.WorkingForm.Fields[index].DeltaInserted)
						{
							color = Color.FromAlphaRgb(0.3, 0, 1, 0);  // vert = champ inséré
						}


						if (this.formEditor.WorkingForm.Fields[index].DeltaBrokenAttach)
						{
							color = Color.FromAlphaRgb(0.8, 1, 0, 0);  // rouge foncé = lien cassé
						}
					}
				}
				else
				{
					int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.WorkingForm.Fields, item.Guid);
					if (index != -1)
					{
						if (this.formEditor.WorkingForm.Fields[index].DeltaHidden)
						{
							color = Color.FromAlphaRgb(0.3, 1, 0, 0);  // rouge = champ caché
						}
					}
				}
			}

			return color;
		}

		public bool IsTableContentInheritHidden(TableItem item)
		{
			//	Indique si un champ est caché par un delta plus profond que simplement le précédent.
			//	Dans ce cas, il ne faut pas afficher le champ sur un fond rosé, mais simplement ajouter
			//	l'indication "(caché)" après son nom.
			if (this.IsDelta)
			{
				int index = FormEngine.Arrange.IndexOfGuid(this.formEditor.FinalFields, item.Guid);
				if (index != -1 && this.formEditor.FinalFields[index].DeltaHidden)
				{
					index = FormEngine.Arrange.IndexOfGuid(this.formEditor.BaseFields, item.Guid);
					if (index != -1 && this.formEditor.BaseFields[index].DeltaHidden)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void UpdateTableContent()
		{
			//	Met à jour la liste qui reflète le contenu de la table des champs, visible en haut à droite.
			this.tableContent.Clear();

			if (this.formEditor.WorkingForm == null)
			{
				return;
			}

			//	Construit la liste des chemins de Druids, en commençant par ceux qui font
			//	partie du masque de saisie.
			int level = 0;
			foreach (FieldDescription field in this.formEditor.FinalFields)
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
							prefix = string.Concat(module.ModuleInfo.SourceNamespaceDefault, ".", cultureMap.Prefix, ".");
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

			foreach (FieldDescription field in this.formEditor.FinalFields)
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
