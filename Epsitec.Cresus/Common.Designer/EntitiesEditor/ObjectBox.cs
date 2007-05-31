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
			for (int i=0; i<dataFields.Count; i++)
			{
				Field field = this.CreateField(dataFields[i]);
				this.fields.Add(field);
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
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
					{
						return new Point(bounds.Left, posv);
					}
					else
					{
						return new Point(bounds.Left, bounds.Center.Y);
					}

				case ConnectionAnchor.Right:
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
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
				this.columnsSeparator = System.Math.Min(this.columnsSeparator, 0.8);
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
			if (this.hilitedElement == ActiveElement.HeaderDragging)
			{
				this.isDragging = true;
				this.draggingPos = pos;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.FieldSelect)
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
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.bounds.Contains(pos))
			{
				return false;
			}

			Rectangle rect;

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
				Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);
				double d = Point.Distance(center, pos);
				if (d <= ObjectBox.buttonRadius+3)
				{
					element = ActiveElement.ExtendButton;
					return true;
				}

				//	Souris dans le bouton pour changer la largeur ?
				rect = new Rectangle(this.bounds.Right-ObjectBox.buttonRadius*2-8, this.bounds.Bottom, ObjectBox.buttonRadius*2+8, ObjectBox.footerHeight);
				if (rect.Contains(pos))
				{
					element = ActiveElement.ChangeWidth;
					return true;
				}

				//	Souris dans le bouton pour déplacer le séparateur des colonnes ?
				rect = new Rectangle(this.ColumnsSeparator-ObjectBox.buttonRadius, this.bounds.Bottom, ObjectBox.buttonRadius*2, ObjectBox.footerHeight);
				if (rect.Contains(pos))
				{
					element = ActiveElement.MoveColumnsSeparator;
					return true;
				}

				//	Souris dans l'en-tête ?
				if (pos.Y >= this.bounds.Top-ObjectBox.headerHeight ||
				pos.Y <= this.bounds.Bottom+ObjectBox.footerHeight)
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

					rect = this.GetFieldBounds(i);
					if (rect.Contains(pos))
					{
						element = ActiveElement.FieldSelect;
						fieldRank = i;
						return true;
					}
				}
			}

			element = ActiveElement.Inside;
			return true;
		}

		protected Rectangle GetFieldRemoveBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (-) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Left = rect.Right-rect.Height;
			rect.Offset(-3, 0);
			
			return rect;
		}

		protected Rectangle GetFieldAddBounds(int rank)
		{
			//	Retourne le rectangle occupé par le bouton (+) d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Left = rect.Right-rect.Height;
			rect.Bottom -= 6;
			rect.Height = 6*2;
			rect.Offset(-3, 0);

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
				this.editor.UpdateAfterAddOrRemoveConnection();
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
				this.editor.UpdateAfterAddOrRemoveConnection();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void AddField(int rank)
		{
			//	Ajoute un nouveau champ.
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			IResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor;
			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.Fields.ToString());
			StructuredData newField = broker.CreateData(this.cultureMap);

			Druid druid = this.CreateFieldCaption();
			newField.SetValue(Support.Res.Fields.Field.CaptionId, druid);

			dataFields.Insert(rank+1, newField);

			Field field = this.CreateField(newField);
			this.fields.Insert(rank+1, field);

			this.UpdateFields();
			this.editor.UpdateAfterAddOrRemoveConnection();
			this.hilitedElement = ActiveElement.None;
		}

		protected Field CreateField(StructuredData data)
		{
			//	Crée un nouvelle instance de la classe Field, initialisée selon le StructuredData d'un champ.
			System.Diagnostics.Debug.Assert(!data.IsEmpty);

			Druid fieldCaptionId = (Druid) data.GetValue(Support.Res.Fields.Field.CaptionId);
			FieldMembership membership = (FieldMembership) data.GetValue(Support.Res.Fields.Field.Membership);
			FieldRelation rel = (FieldRelation) data.GetValue(Support.Res.Fields.Field.Relation);
			Druid sourceId = (Druid) data.GetValue(Support.Res.Fields.Field.SourceFieldId);
			Druid typeId = (Druid) data.GetValue(Support.Res.Fields.Field.TypeId);
			
			Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
			CultureMap dstItem = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
			if (dstItem == null)
			{
				rel = FieldRelation.None;  // ce n'est pas une vraie relation !
			}

			Caption fieldCaption = this.editor.Module.AccessEntities.DirectGetCaption(fieldCaptionId);
			Caption typeCaption = this.editor.Module.AccessEntities.DirectGetCaption(typeId);

			Field field = new Field();
			field.FieldName = fieldCaption == null ? "" : fieldCaption.Name;
			field.TypeName = typeCaption == null ? "" : typeCaption.Name;
			field.Relation = rel;
			field.Destination = typeId;
			field.SrcBox = this;

			return field;
		}

		protected void UpdateFields()
		{
			//	Met à jour les liaisons des champs.
			for (int i=0; i<this.fields.Count; i++)
			{
				this.fields[i].IsSourceExpanded = this.isExtended;
				this.fields[i].Rank = i;
			}
		}

		protected Druid CreateFieldCaption()
		{
			//	Crée un nouveau Caption de type Field (dont le nom commence par "Fld.").
			//	TODO: remplacer cette cuisine par les nouveaux mécanismes...
			string text = this.GetNewName();
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
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.HeaderDragging);

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.DrawShadow(graphics, rect, ObjectBox.roundRectRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle(rect, ObjectBox.roundRectRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = adorner.ColorCaption;
			Color c2 = adorner.ColorCaption;
			c1.A = dragging ? 0.6 : 0.4;
			c2.A = dragging ? 0.2 : 0.1;
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = Color.FromBrightness(0.9);
			if (dragging)
			{
				colorLine = adorner.ColorCaption;
				colorLine.A = 0.3;
			}

			Color colorFrame = dragging ? adorner.ColorCaption : Color.FromBrightness(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+ObjectBox.footerHeight, this.bounds.Width-2, this.bounds.Height-ObjectBox.footerHeight-ObjectBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = adorner.ColorCaption;
				Color ci2 = adorner.ColorCaption;
				ci1.A = dragging ? 0.2 : 0.1;
				ci2.A = 0.0;
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				//	Trait vertical de séparation.
				double posx = System.Math.Floor(this.ColumnsSeparator)+0.5;
				graphics.AddLine(posx, this.bounds.Bottom+ObjectBox.footerHeight+0.5, posx, this.bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.RenderSolid(colorLine);

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-ObjectBox.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			rect = new Rectangle(this.bounds.Left+4, this.bounds.Top-ObjectBox.headerHeight+2, this.bounds.Width-ObjectBox.buttonRadius*2-5-5, ObjectBox.headerHeight-2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics);

			//	Dessine le bouton compact/étendu.
			Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			bool hilited = (this.hilitedElement == ActiveElement.ExtendButton);
			this.DrawRoundButton(graphics, center, ObjectBox.buttonRadius, shape, hilited, false);

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-ObjectBox.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+ObjectBox.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+ObjectBox.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				for (int i=0; i<this.fields.Count; i++)
				{
					rect = this.GetFieldBounds(i);

					if (this.hilitedElement == ActiveElement.FieldSelect && this.hilitedFieldRank == i)
					{
						Color hiliteColor = adorner.ColorCaption;
						hiliteColor.A = 0.1;

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(hiliteColor);
					}

					if (this.hilitedElement == ActiveElement.FieldRemove && this.hilitedFieldRank == i)
					{
						Color hiliteColor = adorner.ColorCaption;
						hiliteColor.A = 0.3;

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(hiliteColor);
					}

					if (this.isFieldMoving && this.fieldInitialRank == i)
					{
						Color hiliteColor = adorner.ColorCaption;
						hiliteColor.A = 0.3;

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(hiliteColor);
					}

					//	Affiche le nom du champ.
					rect.Deflate(ObjectBox.textMargin, 0);
					Rectangle part = rect;
					part.Right = this.ColumnsSeparator-2;
					this.fields[i].TextLayoutField.LayoutSize = part.Size;
					this.fields[i].TextLayoutField.Paint(part.BottomLeft, graphics);

					//	Affiche le type du champ.
					part = rect;
					part.Left = this.ColumnsSeparator+2;
					this.fields[i].TextLayoutType.LayoutSize = part.Size;
					this.fields[i].TextLayoutType.Paint(part.BottomLeft, graphics);

					rect = this.GetFieldBounds(i);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(colorLine);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					Point p1 = this.GetFieldBounds(this.fieldInitialRank).Center;
					Point p2 = this.GetFieldMovingBounds(this.hilitedFieldRank).Center;
					this.DrawMovingArrow(graphics, p1, p2);
				}

				if (this.hilitedElement != ActiveElement.None &&
					this.hilitedElement != ActiveElement.HeaderDragging &&
					this.hilitedElement != ActiveElement.ExtendButton &&
					this.hilitedElement != ActiveElement.ChangeWidth &&
					this.hilitedElement != ActiveElement.MoveColumnsSeparator &&
					!this.isFieldMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator)
				{
					//	Dessine le rectangle à droite pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(-1).Center;
					Point p2 = this.GetFieldAddBounds(this.fields.Count-1).Center;
					rect = new Rectangle(p1, p2);
					rect.Inflate(2.5+6);
					this.DrawShadow(graphics, rect, rect.Width/2, 6, 0.2);
					rect.Deflate(6);
					Path pathButtons = this.PathRoundRectangle(rect, rect.Width/2);

					Color hiliteColor = Color.FromBrightness(1);
					if (this.hilitedElement == ActiveElement.FieldAdd ||
						this.hilitedElement == ActiveElement.FieldRemove)
					{
						hiliteColor = adorner.ColorCaption;
						hiliteColor.R = 1-(1-hiliteColor.R)*0.2;
						hiliteColor.G = 1-(1-hiliteColor.G)*0.2;
						hiliteColor.B = 1-(1-hiliteColor.B)*0.2;
					}

					graphics.Rasterizer.AddSurface(pathButtons);
					graphics.RenderSolid(hiliteColor);

					graphics.Rasterizer.AddOutline(pathButtons);
					graphics.RenderSolid(Color.FromBrightness(0));
				}

				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, rect.Width/2, GlyphShape.Minus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					graphics.LineWidth = 3;
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.LineWidth = 1;
					graphics.RenderSolid(adorner.ColorCaption);

					rect = this.GetFieldAddBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, rect.Width/2, GlyphShape.Plus, true, true);
				}

				if (this.hilitedElement == ActiveElement.FieldMoving)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					graphics.LineWidth = 5;
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.LineWidth = 1;
					graphics.RenderSolid(adorner.ColorCaption);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, 2);
			graphics.RenderSolid(colorFrame);

			if (this.isExtended)
			{
				//	Dessine les petits traits verticaux.
				double posx;

				posx = System.Math.Floor(this.ColumnsSeparator)+0.5;
				graphics.AddLine(posx, this.bounds.Bottom+2, posx, this.bounds.Bottom+ObjectBox.footerHeight);

				posx = this.bounds.Right-ObjectBox.buttonRadius*2-8.5;
				graphics.AddLine(posx, this.bounds.Bottom+2, posx, this.bounds.Bottom+ObjectBox.footerHeight);

				graphics.RenderSolid(colorFrame);

				if (this.hilitedElement == ActiveElement.MoveColumnsSeparator)
				{
					//	Dessine le bouton pour déplacer le séparateur des colonnes.
					center = new Point(this.ColumnsSeparator, this.bounds.Bottom+ObjectBox.footerHeight/2);
					this.DrawRoundButton(graphics, center, ObjectBox.buttonRadius, GlyphShape.TriangleRight, true, false);
				}

				if (this.hilitedElement == ActiveElement.ChangeWidth)
				{
					//	Dessine le bouton pour changer la largeur.
					center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Bottom+ObjectBox.footerHeight/2);
					this.DrawRoundButton(graphics, center, ObjectBox.buttonRadius, GlyphShape.TriangleRight, true, false);
				}
			}
		}

		protected void DrawMovingArrow(Graphics graphics, Point p1, Point p2)
		{
			//	Dessine une flèche pendant le déplacement d'un champ.
			if (System.Math.Abs(p1.Y-p2.Y) < ObjectBox.fieldHeight)
			{
				return;
			}

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			p2 = Point.Move(p2, p1, 2);
			double d = (p1.Y > p2.Y) ? -10 : 10;

			Path path = new Path();
			path.MoveTo(p2);
			path.LineTo(p2.X-d*2, p2.Y-d*2);
			path.LineTo(p2.X-d, p2.Y-d*2);
			path.LineTo(p1.X-d, p1.Y);
			path.LineTo(p1.X+d, p1.Y);
			path.LineTo(p2.X+d, p2.Y-d*2);
			path.LineTo(p2.X+d*2, p2.Y-d*2);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(adorner.ColorCaption);
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
			protected bool isExplored;
			protected bool isSourceExpanded;
		}


		protected static readonly double roundRectRadius = 12;
		protected static readonly double shadowOffset = 6;
		protected static readonly double headerHeight = 32;
		protected static readonly double textMargin = 10;
		protected static readonly double footerHeight = 10;
		protected static readonly double buttonRadius = 10;
		protected static readonly double fieldHeight = 20;

		protected CultureMap cultureMap;
		protected Rectangle bounds;
		protected double columnsSeparator;
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
