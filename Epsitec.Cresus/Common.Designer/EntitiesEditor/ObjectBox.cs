using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class ObjectBox : AbstractObject
	{
		public enum ConnectionAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectBox(Editor editor) : base(editor)
		{
			this.title = new TextLayout();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.fields = new List<Field>();

			this.columnsSeparator = 0.5;
			this.isRoot = false;
			this.isExtended = false;
		}


		public CultureMap CultureMap
		{
			get
			{
				return this.cultureMap;
			}
		}

		public string Title
		{
			//	Titre au sommet de la boîte.
			get
			{
				return this.titleString;
			}
			set
			{
				if (this.titleString != value)
				{
					this.titleString = value;

					this.title.Text = string.Concat("<b>", this.titleString, "</b>");
				}
			}
		}

		public void SetContent(CultureMap cultureMap)
		{
			//	Initialise le contenu de la boîte.
			this.cultureMap = cultureMap;

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			this.fields.Clear();
			if (dataFields != null)
			{
				for (int i=0; i<dataFields.Count; i++)
				{
					Field field = new Field();
					this.UpdateField(dataFields[i], field);
					this.fields.Add(field);
				}
			}

			this.UpdateFields();
		}

		public Rectangle Bounds
		{
			//	Boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
			}
		}

		public List<Field> Fields
		{
			get
			{
				return this.fields;
			}
		}

		public Field ParentField
		{
			get
			{
				return this.parentField;
			}
			set
			{
				this.parentField = value;
			}
		}

		public bool IsRoot
		{
			//	Indique s'il s'agit de la boîte racine, c'est-à-dire de la boîte sélectionnée
			//	dans la liste de gauche.
			get
			{
				return this.isRoot;
			}
			set
			{
				if (this.isRoot != value)
				{
					this.isRoot = value;

					this.editor.Invalidate();
				}
			}
		}

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
			//	En mode compact, seul le titre est visible.
			//	En mode étendu, les champs sont visibles.
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;

					this.UpdateFields();
					this.editor.Invalidate();
				}
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return ObjectBox.headerHeight + ObjectBox.fieldHeight*this.fields.Count + ObjectBox.footerHeight + 20;
			}
			else
			{
				return ObjectBox.headerHeight;
			}
		}

		public double GetConnectionVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			if (this.isExtended && rank < this.fields.Count)
			{
				Rectangle rect = this.GetFieldBounds(rank);
				return rect.Center.Y;
			}
			else
			{
				return this.bounds.Center.Y;
			}
		}

		public Point GetConnectionDestination(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			Rectangle bounds = this.bounds;

			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= bounds.Bottom+ObjectBox.roundFrameRadius && posv <= bounds.Top-ObjectBox.roundFrameRadius)
					{
						return new Point(bounds.Left, posv);
					}
					else
					{
						return new Point(bounds.Left, bounds.Center.Y);
					}

				case ConnectionAnchor.Right:
					if (posv >= bounds.Bottom+ObjectBox.roundFrameRadius && posv <= bounds.Top-ObjectBox.roundFrameRadius)
					{
						return new Point(bounds.Right, posv);
					}
					else
					{
						return new Point(bounds.Right, bounds.Center.Y);
					}

				case ConnectionAnchor.Bottom:
					return new Point(bounds.Center.X, bounds.Bottom);

				case ConnectionAnchor.Top:
					return new Point(bounds.Center.X, bounds.Top);
			}

			return Point.Zero;
		}


		public override bool MouseMove(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				Rectangle bounds = this.Bounds;

				bounds.Offset(pos-this.draggingPos);
				this.draggingPos = pos;

				this.Bounds = bounds;
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isFieldMoving)
			{
				return base.MouseMove(pos);
			}
			else if (this.isChangeWidth)
			{
				Rectangle bounds = this.Bounds;
				bounds.Width = System.Math.Max(pos.X-this.changeWidthPos+this.changeWidthInitial, 100);
				this.Bounds = bounds;
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isMoveColumnsSeparator)
			{
				Rectangle rect = this.Bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				this.columnsSeparator = (pos.X-rect.Left)/rect.Width;
				this.columnsSeparator = System.Math.Max(this.columnsSeparator, 0.2);
				this.columnsSeparator = System.Math.Min(this.columnsSeparator, 1.0);
				this.editor.Invalidate();
				return true;
			}
			else
			{
				return base.MouseMove(pos);
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
			if (this.hilitedElement == ActiveElement.HeaderDragging && this.editor.BoxCount > 1)
			{
				this.isDragging = true;
				this.draggingPos = pos;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.FieldMovable)
			{
				this.isFieldMoving = true;
				this.fieldInitialRank = this.hilitedFieldRank;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.ChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.MoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = true;
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDragging)
			{
				this.editor.UpdateAfterMoving(this);
				this.isDragging = false;
				this.editor.LockObject(null);
			}
			else if (this.isFieldMoving)
			{
				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					this.MoveField(this.fieldInitialRank, this.hilitedFieldRank);
				}
				this.isFieldMoving = false;
				this.editor.LockObject(null);
			}
			else if (this.isChangeWidth)
			{
				this.editor.UpdateAfterMoving(this);
				this.isChangeWidth = false;
				this.editor.LockObject(null);
			}
			else if (this.isMoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = false;
				this.editor.LockObject(null);
			}
			else
			{
				if (this.hilitedElement == ActiveElement.ExtendButton)
				{
					this.IsExtended = !this.IsExtended;
					this.editor.UpdateAfterGeometryChanged(this);
				}

				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					this.RemoveField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					this.AddField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldNameSelect)
				{
					this.ChangeFieldName(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.FieldTypeSelect)
				{
					this.ChangeFieldType(this.hilitedFieldRank);
				}
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero)
			{
				return false;
			}

			Rectangle rect;
			Point center;

			if (this.isFieldMoving)
			{
				//	Souris entre deux champs ?
				for (int i=-1; i<this.fields.Count; i++)
				{
					rect = this.GetFieldMovingBounds(i);
					if (rect.Contains(pos))
					{
						element = ActiveElement.FieldMoving;
						fieldRank = i;
						return true;
					}
				}
			}
			else
			{
				//	Souris dans le bouton compact/étendu ?
				center = new Point(this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-ObjectBox.headerHeight/2);
				if (Point.Distance(center, pos) <= AbstractObject.buttonRadius+3)
				{
					element = ActiveElement.ExtendButton;
					return true;
				}

				if (this.isExtended)
				{
					//	Souris dans le bouton pour changer la largeur ?
					//	Souris dans le bouton pour déplacer le séparateur des colonnes ?
					center = new Point(this.bounds.Right-1, this.bounds.Bottom+ObjectBox.footerHeight/2+1);
					double d1 = Point.Distance(center, pos);

					center = new Point(this.ColumnsSeparator, this.bounds.Bottom+ObjectBox.footerHeight/2+1);
					double d2 = Point.Distance(center, pos);

					if (d1 < d2)
					{
						if (d1 <= AbstractObject.buttonRadius+3)
						{
							element = ActiveElement.ChangeWidth;
							return true;
						}
					}
					else
					{
						if (d2 <= AbstractObject.buttonRadius+3)
						{
							element = ActiveElement.MoveColumnsSeparator;
							return true;
						}
					}

					//	Souris dans l'en-tête ?
					if (this.bounds.Contains(pos) && 
					(pos.Y >= this.bounds.Top-ObjectBox.headerHeight ||
					 pos.Y <= this.bounds.Bottom+ObjectBox.footerHeight))
					{
						element = ActiveElement.HeaderDragging;
						return true;
					}

					//	Souris entre deux champs ?
					for (int i=-1; i<this.fields.Count; i++)
					{
						rect = this.GetFieldAddBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldAdd;
							fieldRank = i;
							return true;
						}
					}

					//	Souris dans un champ ?
					for (int i=0; i<this.fields.Count; i++)
					{
						rect = this.GetFieldRemoveBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldRemove;
							fieldRank = i;
							return true;
						}

						rect = this.GetFieldMovableBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldMovable;
							fieldRank = i;
							return true;
						}

						rect = this.GetFieldNameBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldNameSelect;
							fieldRank = i;
							return true;
						}

						rect = this.GetFieldTypeBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.FieldTypeSelect;
							fieldRank = i;
							return true;
						}
					}
				}
				else  // boîte compactée ?
				{
					if (this.bounds.Contains(pos))
					{
						element = ActiveElement.HeaderDragging;
						return true;
					}
				}
			}

			if (!this.bounds.Contains(pos))
			{
				return false;
			}

			element = ActiveElement.Inside;
			return true;
		}

		protected Rectangle GetFieldRemoveBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (-) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Width = rect.Height;
			
			return rect;
		}

		protected Rectangle GetFieldAddBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (+) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Width = rect.Height;
			rect.Bottom -= 6;
			rect.Height = 6*2;

			return rect;
		}

		protected Rectangle GetFieldMovableBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (|) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Left = rect.Right-rect.Height;
			
			return rect;
		}

		protected Rectangle GetFieldMovingBounds(int rank)
		{
			//	Retourne le rectangle occupé par la destination d'un déplacement de champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Bottom -= ObjectBox.fieldHeight/2;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}

		protected Rectangle GetFieldNameBounds(int rank)
		{
			//	Retourne le rectangle occupé par le nom d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Right = this.ColumnsSeparator;

			return rect;
		}

		protected Rectangle GetFieldTypeBounds(int rank)
		{
			//	Retourne le rectangle occupé par le type d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Left = this.ColumnsSeparator+1;

			return rect;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - ObjectBox.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}


		protected void MoveField(int srcRank, int dstRank)
		{
			//	Déplace un champ.
			if (dstRank != srcRank && dstRank != srcRank-1)
			{
				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				if (dstRank < srcRank)
				{
					dstRank++;
				}

				StructuredData movingfield = dataFields[srcRank];
				dataFields.RemoveAt(srcRank);
				dataFields.Insert(dstRank, movingfield);

				Field movingFld = this.fields[srcRank];
				this.fields.RemoveAt(srcRank);
				this.fields.Insert(dstRank, movingFld);

				this.UpdateFields();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.SetDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void RemoveField(int rank)
		{
			//	Supprime un champ.
			string question = string.Format("Voulez-vous supprimer le champ <b>{0}</b> ?", this.fields[rank].FieldName);
			if (this.editor.Module.MainWindow.DialogQuestion(question) == Epsitec.Common.Dialogs.DialogResult.Yes)
			{
				this.CloseBoxes(this.fields[rank].DstBox);

				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
				dataFields.RemoveAt(rank);

				this.fields.RemoveAt(rank);

				this.UpdateFields();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.SetDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void AddField(int rank)
		{
			//	Ajoute un nouveau champ.
			Module module = this.editor.Module;
			string name = this.GetNewName();
			name = module.MainWindow.DlgFieldName(name);
			if (string.IsNullOrEmpty(name))
			{
				this.hilitedElement = ActiveElement.None;
				return;
			}
			
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			IResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor;
			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.Fields.ToString());
			StructuredData newField = broker.CreateData(this.cultureMap);

			Druid druid = this.CreateFieldCaption(name);
			newField.SetValue(Support.Res.Fields.Field.CaptionId, druid);

			dataFields.Insert(rank+1, newField);

			Field field = new Field();
			this.UpdateField(newField, field);
			this.fields.Insert(rank+1, field);

			this.UpdateFields();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
			this.SetDirty();
			this.hilitedElement = ActiveElement.None;
		}

		protected void ChangeFieldName(int rank)
		{
			//	Choix du nom pour un champ.
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[rank];
			Druid druid = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

			Module module = this.editor.Module;
			druid = module.MainWindow.DlgResourceSelector(module, ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			dataField.SetValue(Support.Res.Fields.Field.CaptionId, druid);
			this.UpdateField(dataField, this.fields[rank]);
			this.SetDirty();
			this.editor.Invalidate();
		}

		protected void ChangeFieldType(int rank)
		{
			//	Choix du type pour un champ.
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[rank];
			Druid druid = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);

			Module module = this.editor.Module;
			druid = module.MainWindow.DlgResourceSelector(module, ResourceAccess.Type.Types, ResourceAccess.TypeType.None, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			if (this.fields[rank].Relation != FieldRelation.None)
			{
				ObjectBox dst = this.fields[rank].DstBox;
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.CloseBoxes(dst);
			}

			dataField.SetValue(Support.Res.Fields.Field.TypeId, druid);

			AbstractType type = TypeRosetta.GetTypeObject(module.ResourceManager.GetCaption(druid));
			if (type is StructuredType)
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Reference);
			}
			else
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.None);
			}
			
			this.UpdateField(dataField, this.fields[rank]);
			this.SetDirty();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
		}

		protected void UpdateField(StructuredData dataField, Field field)
		{
			//	Met à jour une instance de la classe Field, selon le StructuredData d'un champ.
			System.Diagnostics.Debug.Assert(!dataField.IsEmpty);

			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
			FieldMembership membership = (FieldMembership) dataField.GetValue(Support.Res.Fields.Field.Membership);
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
			Druid sourceId = (Druid) dataField.GetValue(Support.Res.Fields.Field.SourceFieldId);
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			
			Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
			CultureMap dstItem = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
			if (dstItem == null)
			{
				rel = FieldRelation.None;  // ce n'est pas une vraie relation !
			}

			Caption fieldCaption = this.editor.Module.AccessEntities.DirectGetCaption(fieldCaptionId);
			Caption typeCaption = typeId.IsEmpty ? null : this.editor.Module.AccessEntities.DirectGetCaption(typeId);

			field.FieldName = fieldCaption == null ? "" : fieldCaption.Name;
			field.TypeName = typeCaption == null ? "" : typeCaption.Name;
			field.Relation = rel;
			field.Destination = typeId;
			field.SrcBox = this;
		}

		protected void UpdateFields()
		{
			//	Met à jour toutes les liaisons des champs.
			for (int i=0; i<this.fields.Count; i++)
			{
				this.fields[i].IsSourceExpanded = this.isExtended;
				this.fields[i].Rank = i;
			}
		}

		protected Druid CreateFieldCaption(string text)
		{
			//	Crée un nouveau Caption de type Field (dont le nom commence par "Fld.").
			//	TODO: remplacer cette cuisine par les nouveaux mécanismes...
			string name = string.Concat(this.Title, ".", text);

			//	Crée un Caption de type Field (dont le nom commence par "Fld.").
			ResourceAccess access = this.editor.Module.AccessCaptions;

			access.BypassFilterOpenAccess(ResourceAccess.Type.Fields, ResourceAccess.TypeType.None, null, null);
			Druid druid = access.BypassFilterCreate(access.GetCultureBundle(null), name, text);
			access.BypassFilterCloseAccess();

			return druid;
		}

		protected string GetNewName()
		{
			//	Cherche un nouveau nom jamais utilisé.
			for (int i=1; i<10000; i++)
			{
				string name = string.Format(Res.Strings.Viewers.Types.Structured.NewName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
			for (int i=0; i<this.fields.Count; i++)
			{
				if (name == this.fields[i].FieldName)
				{
					return true;
				}
			}

			return false;
		}


		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.HeaderDragging);

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.isRoot)
			{
				rect.Inflate(2);
			}
			rect.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.DrawShadow(graphics, rect, ObjectBox.roundFrameRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle(rect, ObjectBox.roundFrameRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = this.GetColorCaption(dragging ? 0.6 : 0.4);
			Color c2 = this.GetColorCaption(dragging ? 0.2 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = Color.FromBrightness(0.9);
			if (dragging)
			{
				colorLine = this.GetColorCaption(0.3);
			}

			Color colorFrame = dragging ? this.GetColorCaption() : Color.FromBrightness(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+ObjectBox.footerHeight, this.bounds.Width-2, this.bounds.Height-ObjectBox.footerHeight-ObjectBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = this.GetColorCaption(dragging ? 0.2 : 0.1);
				Color ci2 = this.GetColorCaption(0.0);
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				//	Trait vertical de séparation.
				if (this.columnsSeparator < 1.0)
				{
					double posx = System.Math.Floor(this.ColumnsSeparator)+0.5;
					graphics.AddLine(posx, this.bounds.Bottom+ObjectBox.footerHeight+0.5, posx, this.bounds.Top-ObjectBox.headerHeight-0.5);
					graphics.RenderSolid(colorLine);
				}

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-ObjectBox.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			rect = new Rectangle(this.bounds.Left+4, this.bounds.Top-ObjectBox.headerHeight+2, this.bounds.Width-AbstractObject.buttonRadius*2-6-5, ObjectBox.headerHeight-2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(0), GlyphPaintStyle.Normal);

			//	Dessine le bouton compact/étendu.
			Point center = new Point(this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-ObjectBox.headerHeight/2);
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			bool hilited = (this.hilitedElement == ActiveElement.ExtendButton);
			this.DrawRoundButton(graphics, center, AbstractObject.buttonRadius, shape, hilited, false);

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-ObjectBox.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+ObjectBox.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+ObjectBox.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				for (int i=0; i<this.fields.Count; i++)
				{
					Color colorName = Color.FromBrightness(0);
					Color colorType = Color.FromBrightness(0);

					if (this.hilitedElement == ActiveElement.FieldNameSelect && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldNameBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption());

						colorName = Color.FromBrightness(1);
					}

					if (this.hilitedElement == ActiveElement.FieldTypeSelect && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldTypeBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption());

						colorType = Color.FromBrightness(1);
					}

					if ((this.hilitedElement == ActiveElement.FieldRemove || this.hilitedElement == ActiveElement.FieldMovable) && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption(0.3));
					}

					if (this.isFieldMoving && this.fieldInitialRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorCaption(0.3));
					}

					//	Affiche le nom du champ.
					rect = this.GetFieldNameBounds(i);
					rect.Right -= 2;
					this.fields[i].TextLayoutField.LayoutSize = rect.Size;
					this.fields[i].TextLayoutField.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorName, GlyphPaintStyle.Normal);

					//	Affiche le type du champ.
					rect = this.GetFieldTypeBounds(i);
					rect.Left += 1;
					if (rect.Width > 10)
					{
						this.fields[i].TextLayoutType.LayoutSize = rect.Size;
						this.fields[i].TextLayoutType.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorType, GlyphPaintStyle.Normal);
					}

					rect = this.GetFieldBounds(i);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(colorLine);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					Point p1 = this.GetFieldBounds(this.fieldInitialRank).Center;
					Point p2 = this.GetFieldMovingBounds(this.hilitedFieldRank).Center;
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					this.DrawMovingArrow(graphics, p1, p2);
				}

				if (this.hilitedElement != ActiveElement.None &&
					this.hilitedElement != ActiveElement.HeaderDragging &&
					this.hilitedElement != ActiveElement.ExtendButton &&
					this.hilitedElement != ActiveElement.ChangeWidth &&
					this.hilitedElement != ActiveElement.MoveColumnsSeparator &&
					!this.isFieldMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator)
				{
					//	Dessine la glissière à gauche pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(-1).Center;
					Point p2 = this.GetFieldAddBounds(this.fields.Count-1).Center;
					hilited = this.hilitedElement == ActiveElement.FieldAdd || this.hilitedElement == ActiveElement.FieldRemove;
					this.DrawEmptySlider(graphics, p1, p2, hilited);

					//	Dessine la glissière à droite pour suggérer les boutons Movable des champs.
					if (this.fields.Count != 0)
					{
						p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
						hilited = this.hilitedElement == ActiveElement.FieldMovable;
						this.DrawEmptySlider(graphics, p1, p2, hilited);
					}
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, this.isRoot ? 6 : 2);
			graphics.RenderSolid(colorFrame);

			//	Dessine les boutons sur les glissières.
			if (this.isExtended)
			{
				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Minus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorCaption());

					rect = this.GetFieldAddBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Plus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldMovable)
				{
					rect = this.GetFieldMovableBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.VerticalMove, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorCaption());
				}
			}

			if (this.isExtended)
			{
				//	Dessine le bouton pour déplacer le séparateur des colonnes.
				center = new Point(this.ColumnsSeparator, this.bounds.Bottom+ObjectBox.footerHeight/2+1);
				if (this.hilitedElement == ActiveElement.MoveColumnsSeparator)
				{
					this.DrawRoundButton(graphics, center, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.HeaderDragging && !this.isDragging)
				{
					this.DrawRoundButton(graphics, center, AbstractObject.bulletRadius, GlyphShape.None, false, false);
				}

				//	Dessine le bouton pour changer la largeur.
				center = new Point(this.bounds.Right-1, this.bounds.Bottom+ObjectBox.footerHeight/2+1);
				if (this.hilitedElement == ActiveElement.ChangeWidth)
				{
					this.DrawRoundButton(graphics, center, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.HeaderDragging && !this.isDragging)
				{
					this.DrawRoundButton(graphics, center, AbstractObject.bulletRadius, GlyphShape.None, false, false);
				}
			}
		}

		protected void DrawDashLine(Graphics graphics, Point p1, Point p2, Color color)
		{
			//	Dessine un large traitillé.
			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			Misc.DrawPathDash(graphics, path, 3, 5, 5, color);
		}

		protected void DrawEmptySlider(Graphics graphics, Point p1, Point p2, bool hilited)
		{
			//	Dessine une glissère vide, pour suggérer les boutons qui peuvent y prendre place.
			Rectangle rect = new Rectangle(p1, p2);
			rect.Inflate(2.5+6);
			this.DrawShadow(graphics, rect, rect.Width/2, 6, 0.2);
			rect.Deflate(6);
			Path path = this.PathRoundRectangle(rect, rect.Width/2);

			Color hiliteColor = Color.FromBrightness(1);
			if (hilited)
			{
				hiliteColor = this.GetColorCaption();
				hiliteColor.R = 1-(1-hiliteColor.R)*0.2;
				hiliteColor.G = 1-(1-hiliteColor.G)*0.2;
				hiliteColor.B = 1-(1-hiliteColor.B)*0.2;
			}

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(hiliteColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		protected void DrawMovingArrow(Graphics graphics, Point p1, Point p2)
		{
			//	Dessine une flèche pendant le déplacement d'un champ.
			if (System.Math.Abs(p1.Y-p2.Y) < ObjectBox.fieldHeight)
			{
				return;
			}

			p2 = Point.Move(p2, p1, 1);
			double d = (p1.Y > p2.Y) ? -6 : 6;
			double sx = 3;

			Path path = new Path();
			path.MoveTo(p2);
			path.LineTo(p2.X-d*3/sx, p2.Y-d*2);
			path.LineTo(p2.X-d/sx, p2.Y-d*2);
			path.LineTo(p1.X-d/sx, p1.Y);
			path.LineTo(p1.X+d/sx, p1.Y);
			path.LineTo(p2.X+d/sx, p2.Y-d*2);
			path.LineTo(p2.X+d*3/sx, p2.Y-d*2);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.GetColorCaption());
		}

		protected double ColumnsSeparator
		{
			//	Retourne la position absolue du séparateur des colonnes.
			get
			{
				Rectangle rect = this.bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				return rect.Left + rect.Width*this.columnsSeparator;
			}
		}

		public int ConnectionExploredCount
		{
			//	Retourne le nombre de connections ouvertes.
			get
			{
				int count = 0;
				
				foreach (Field field in this.fields)
				{
					if (field.IsExplored)
					{
						count++;
					}
				}

				return count;
			}
		}



		/// <summary>
		/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
		/// </summary>
		public class Field
		{
			public Field()
			{
				this.textLayoutField = new TextLayout();
				this.textLayoutField.DefaultFontSize = 10;
				this.textLayoutField.Alignment = ContentAlignment.MiddleLeft;
				this.textLayoutField.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				this.textLayoutType = new TextLayout();
				this.textLayoutType.DefaultFontSize = 10;
				this.textLayoutType.Alignment = ContentAlignment.MiddleLeft;
				this.textLayoutType.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				this.relation = FieldRelation.None;
				this.destination = Druid.Empty;
				this.rank = -1;
				this.isExplored = false;
				this.isSourceExpanded = false;
			}

			public string FieldName
			{
				//	Nom du champ.
				get
				{
					return this.textLayoutField.Text;
				}
				set
				{
					this.textLayoutField.Text = value;
				}
			}

			public string TypeName
			{
				//	Nom du type.
				get
				{
					return this.textLayoutType.Text;
				}
				set
				{
					this.textLayoutType.Text = value;
				}
			}

			public TextLayout TextLayoutField
			{
				get
				{
					return this.textLayoutField;
				}
			}

			public TextLayout TextLayoutType
			{
				get
				{
					return this.textLayoutType;
				}
			}

			public FieldRelation Relation
			{
				//	Type de la relation éventuelle du champ.
				get
				{
					return this.relation;
				}
				set
				{
					this.relation = value;
				}
			}

			public Druid Destination
			{
				//	Destination de la relation éventuelle du champ.
				get
				{
					return this.destination;
				}
				set
				{
					this.destination = value;
				}
			}

			public int Rank
			{
				get
				{
					return this.rank;
				}
				set
				{
					this.rank = value;
				}
			}

			public ObjectBox SrcBox
			{
				get
				{
					return this.srcBox;
				}
				set
				{
					this.srcBox = value;
				}
			}

			public ObjectBox DstBox
			{
				get
				{
					return this.dstBox;
				}
				set
				{
					this.dstBox = value;
				}
			}

			public ObjectConnection Connection
			{
				get
				{
					return this.connection;
				}
				set
				{
					this.connection = value;
				}
			}

			public bool IsExplored
			{
				//	Indique si une relation est explorée, c'est-à-dire si l'on voit l'entité destination.
				get
				{
					return this.isExplored;
				}
				set
				{
					this.isExplored = value;
				}
			}

			public bool IsSourceExpanded
			{
				//	Indique si la boîte source d'une relation est étendue.
				get
				{
					return this.isSourceExpanded;
				}
				set
				{
					this.isSourceExpanded = value;
				}
			}

			protected TextLayout textLayoutField;
			protected TextLayout textLayoutType;
			protected FieldRelation relation;
			protected Druid destination;
			protected int rank;
			protected ObjectBox srcBox;
			protected ObjectBox dstBox;
			protected ObjectConnection connection;
			protected bool isExplored;
			protected bool isSourceExpanded;
		}


		protected static readonly double roundFrameRadius = 12;
		protected static readonly double shadowOffset = 6;
		protected static readonly double headerHeight = 32;
		protected static readonly double textMargin = 13;
		protected static readonly double footerHeight = 13;
		protected static readonly double fieldHeight = 20;

		protected CultureMap cultureMap;
		protected Rectangle bounds;
		protected double columnsSeparator;
		protected bool isRoot;
		protected bool isExtended;
		protected string titleString;
		protected TextLayout title;
		protected List<Field> fields;
		protected Field parentField;

		protected bool isDragging;
		protected Point draggingPos;

		protected bool isFieldMoving;
		protected int fieldInitialRank;

		protected bool isChangeWidth;
		protected double changeWidthPos;
		protected double changeWidthInitial;

		protected bool isMoveColumnsSeparator;
	}
}
