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


		public override bool IsReadyForDragging
		{
			//	Est-ce que l'objet est dragable ?
			//	Est-ce que la boîte est survolée par la souris ?
			//	Si la boîte est survolée, on peut la déplacer globalement.
			get
			{
				return this.hilitedElement == ActiveElement.HeaderDragging;
			}
		}

		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
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

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.bounds.Contains(pos))
			{
				return false;
			}

			//	Souris dans le bouton compact/étendu ?
			Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);
			double d = Point.Distance(center, pos);
			if (d <= ObjectBox.buttonRadius+3)
			{
				element = ActiveElement.ExtendButton;
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
				Rectangle rect = this.GetFieldAddBounds(i);
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
				Rectangle rect = this.GetFieldRemoveBounds(i);
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

			return false;
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

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - ObjectBox.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}


		protected void RemoveField(int rank)
		{
			//	Supprime un champ.
			string question = string.Format("Voulez-vous supprimer le champ <b>{0}</b> ?", this.fields[rank].Text);
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

			Caption caption = this.editor.Module.AccessEntities.DirectGetCaption(fieldCaptionId);

			Field field = new Field();
			field.Text = caption == null ? "" : caption.Name;
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
				if (name == this.fields[i].Text)
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

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.PaintShadow(graphics, rect, ObjectBox.roundRectRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

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
			c1.A = this.IsReadyForDragging ? 0.6 : 0.4;
			c2.A = this.IsReadyForDragging ? 0.2 : 0.1;
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+ObjectBox.footerHeight, this.bounds.Width-2, this.bounds.Height-ObjectBox.footerHeight-ObjectBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = adorner.ColorCaption;
				Color ci2 = adorner.ColorCaption;
				ci1.A = this.IsReadyForDragging ? 0.2 : 0.1;
				ci2.A = 0.0;
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-ObjectBox.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			rect = new Rectangle(this.bounds.Left+4, this.bounds.Top-ObjectBox.headerHeight+2, this.bounds.Width-ObjectBox.buttonRadius*2-5-6, ObjectBox.headerHeight-2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics);

			//	Dessine le bouton compact/étendu.
			Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			bool hilited = (this.hilitedElement == ActiveElement.ExtendButton);
			this.DrawRoundButton(graphics, center, ObjectBox.buttonRadius, shape, hilited);

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-ObjectBox.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+ObjectBox.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+ObjectBox.footerHeight+0.5);
				graphics.RenderSolid(this.IsReadyForDragging ? adorner.ColorCaption : Color.FromBrightness(0));

				Color color = Color.FromBrightness(0.9);
				if (this.IsReadyForDragging)
				{
					color = adorner.ColorCaption;
					color.A = 0.3;
				}

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

					rect.Deflate(10, 0);
					this.fields[i].TextLayout.LayoutSize = rect.Size;
					this.fields[i].TextLayout.Paint(rect.BottomLeft, graphics);

					rect.Inflate(10, 0);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(color);

					rect.Offset(0, -ObjectBox.fieldHeight);
				}

				if (this.hilitedElement == ActiveElement.FieldRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, rect.Width/2, GlyphShape.Minus, true);
				}

				if (this.hilitedElement == ActiveElement.FieldAdd)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					graphics.LineWidth = 3;
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.LineWidth = 1;
					graphics.RenderSolid(adorner.ColorCaption);

					rect = this.GetFieldAddBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, rect.Width/2, GlyphShape.Plus, true);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, 2);
			graphics.RenderSolid(this.IsReadyForDragging ? adorner.ColorCaption : Color.FromBrightness(0));
		}



		/// <summary>
		/// Cette classe contient toutes les informations relatives à une ligne, c'est-à-dire à un champ.
		/// </summary>
		public class Field
		{
			public Field()
			{
				this.textLayout = new TextLayout();
				this.textLayout.DefaultFontSize = 10;
				this.textLayout.Alignment = ContentAlignment.MiddleLeft;
				this.textLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				this.relation = FieldRelation.None;
				this.destination = Druid.Empty;
				this.rank = -1;
				this.isExplored = false;
				this.isSourceExpanded = false;
			}

			public string Text
			{
				//	Nom du champ.
				get
				{
					return this.textLayout.Text;
				}
				set
				{
					this.textLayout.Text = value;
				}
			}

			public TextLayout TextLayout
			{
				get
				{
					return this.textLayout;
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

			protected TextLayout textLayout;
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
		protected static readonly double footerHeight = 10;
		protected static readonly double buttonRadius = 10;
		protected static readonly double fieldHeight = 20;

		protected CultureMap cultureMap;
		protected Rectangle bounds;
		protected bool isExtended;
		protected string titleString;
		protected TextLayout title;
		protected List<Field> fields;
		protected Field parentField;
	}
}
