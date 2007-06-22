using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
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

			this.columnsSeparatorRelative = 0.5;
			this.isRoot = false;
			this.isExtended = false;

			this.connectionListBt = new List<ObjectConnection>();
			this.connectionListBb = new List<ObjectConnection>();
			this.connectionListC = new List<ObjectConnection>();
			this.connectionListD = new List<ObjectConnection>();

			this.parents = new List<ObjectBox>();
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

			this.Title = this.cultureMap.Name;

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			this.fields.Clear();
			if (dataFields != null)
			{
				for (int i=0; i<dataFields.Count; i++)
				{
					Field field = new Field(this.editor);
					this.UpdateField(dataFields[i], field);
					this.fields.Add(field);
				}
			}

			this.UpdateFields();
			this.UpdateSources();
		}

		public ObjectComment Comment
		{
			//	Commentaire lié.
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public override Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
		}

		public void SetBounds(Rectangle bounds)
		{
			//	Modifie la boîte de l'objet.
			Point p1 = this.bounds.TopLeft;
			this.bounds = bounds;
			Point p2 = this.bounds.TopLeft;

			//	S'il existe un commentaire associé, il doit aussi être déplacé.
			if (this.comment != null)
			{
				Rectangle rect = this.comment.InternalBounds;
				rect.Offset(p2-p1);
				this.comment.SetBounds(rect);
			}
		}

		public override MainColor BackgroundMainColor
		{
			//	Couleur de fond de la boîte.
			get
			{
				return this.boxColor;
			}
			set
			{
				if (this.boxColor != value)
				{
					this.boxColor = value;

					foreach (Field field in this.fields)
					{
						if (field.Connection != null)
						{
							field.Connection.BackgroundMainColor = this.boxColor;
						}
					}

					this.editor.Invalidate();
					this.editor.DirtySerialization = true;
				}
			}
		}

		public List<Field> Fields
		{
			get
			{
				return this.fields;
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

		public bool IsConnectedToRoot
		{
			//	Indique si cet objet est connecté à la racine (flag temporaire).
			get
			{
				return this.isConnectedToRoot;
			}
			set
			{
				this.isConnectedToRoot = value;
			}
		}


		public List<ObjectConnection> ConnectionListBt
		{
			get
			{
				return this.connectionListBt;
			}
		}

		public List<ObjectConnection> ConnectionListBb
		{
			get
			{
				return this.connectionListBb;
			}
		}

		public List<ObjectConnection> ConnectionListC
		{
			get
			{
				return this.connectionListC;
			}
		}

		public List<ObjectConnection> ConnectionListD
		{
			get
			{
				return this.connectionListD;
			}
		}

		public List<ObjectBox> Parents
		{
			get
			{
				return this.parents;
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return AbstractObject.headerHeight + ObjectBox.fieldHeight*this.fields.Count + AbstractObject.footerHeight + 20;
			}
			else
			{
				return AbstractObject.headerHeight;
			}
		}

		public double GetConnectionSrcVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			//	Il s'agit toujours de la position de départ d'une liaison.
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

		public Point GetConnectionDstPosition(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			//	Il s'agit toujours de la position d'arrivée d'une liaison.
			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= this.bounds.Bottom+ObjectBox.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectBox.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, false))
					{
						return new Point(this.bounds.Left, posv);
					}

					if (this.isExtended && this.sourcesClosedCount > 0)
					{
						//	En dessous du moignon "source".
						return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight);
					}
					else
					{
						return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight*0.5);
					}


				case ConnectionAnchor.Right:
					if (posv >= this.bounds.Bottom+ObjectBox.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectBox.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, true))
					{
						return new Point(this.bounds.Right, posv);
					}

					return new Point(this.bounds.Right, this.bounds.Top-AbstractObject.headerHeight*0.5);

				case ConnectionAnchor.Bottom:
					return new Point(this.bounds.Center.X, this.bounds.Bottom);

				case ConnectionAnchor.Top:
					return new Point(this.bounds.Center.X, this.bounds.Top);
			}

			return Point.Zero;
		}

		protected bool IsVerticalPositionFree(double posv, bool right)
		{
			//	Cherche si une position verticale n'est occupée par aucun départ de liaison.
			if (!right && this.isExtended && this.sourcesClosedCount > 0)
			{
				double y = this.bounds.Top-AbstractObject.headerHeight*0.5;
				if (posv >= y-ObjectBox.fieldHeight/2 && posv <= y+ObjectBox.fieldHeight/2)  // sur le moignon "source" ?
				{
					return false;
				}
			}

			for (int i=0; i<this.fields.Count; i++)
			{
				Field field = this.fields[i];
				ObjectConnection connection = field.Connection;

				if (field.Relation != FieldRelation.None && connection != null)
				{
					Rectangle rect = this.GetFieldBounds(i);
					if (posv >= rect.Bottom && posv <= rect.Top)
					{
						if (field.IsExplored)
						{
							if (field.IsAttachToRight)
							{
								if (right)  return false;
							}
							else
							{
								if (!right)  return false;
							}
						}
						else
						{
							if (right)  return false;
						}
					}
				}
			}

			return true;
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDragging || this.isFieldMoving || this.isChangeWidth || this.isMoveColumnsSeparator || this.isSourcesMenu)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case AbstractObject.ActiveElement.BoxHeader:
					if (this.editor.BoxCount == 1)
					{
						return null;
					}
					else
					{
						return "Déplace l'entité";
					}

				case AbstractObject.ActiveElement.BoxSources:
					if (this.sourcesList.Count == 0)
					{
						return null;
					}
					else
					{
						return "Ouvre une entité source à choix";
					}

				case AbstractObject.ActiveElement.BoxExtend:
					if (this.isExtended)
					{
						return "Compactifie l'entité";
					}
					else
					{
						return "Etend l'entité";
					}

				case AbstractObject.ActiveElement.BoxComment:
					if (this.comment == null)
					{
						return "Montre le commentaire associé";
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format("Montre le commentaire associé<br/><b>{0}</b>", this.comment.Text);
					}
					else
					{
						return "Cache le commentaire associé";
					}

				case AbstractObject.ActiveElement.BoxClose:
					if (this.isRoot)
					{
						return null;
					}
					else
					{
						return "Ferme l'entité";
					}
			}

			return base.GetToolTipText(element);
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

				this.SetBounds(bounds);
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
				this.SetBounds(bounds);
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isMoveColumnsSeparator)
			{
				Rectangle rect = this.Bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				this.columnsSeparatorRelative = (pos.X-rect.Left)/rect.Width;
				this.columnsSeparatorRelative = System.Math.Max(this.columnsSeparatorRelative, 0.2);
				this.columnsSeparatorRelative = System.Math.Min(this.columnsSeparatorRelative, 1.0);
				this.editor.Invalidate();
				return true;
			}
			else if (this.isSourcesMenu)
			{
				Rectangle rect = this.RectangleSourcesMenu;
				int sel = -1;
				if (rect.Contains(pos))
				{
					sel = (int) ((pos.Y-rect.Bottom)/ObjectBox.sourcesMenuHeight);
					sel = this.sourcesList.Count-sel-1;
				}

				if (this.sourcesMenuSelected != sel)
				{
					this.sourcesMenuSelected = sel;
					this.editor.Invalidate();
				}

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
			if (this.isSourcesMenu)  // menu resté du clic précédent ?
			{
				if (this.sourcesMenuSelected == -1)  // clic en dehors ?
				{
					this.isSourcesMenu = false;  // ferme le menu
					this.editor.LockObject(null);
					this.editor.Invalidate();
				}
				return;
			}

			if (this.hilitedElement == ActiveElement.BoxHeader && this.editor.BoxCount > 1)
			{
				this.isDragging = true;
				this.draggingPos = pos;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.BoxFieldMovable)
			{
				this.isFieldMoving = true;
				this.fieldInitialRank = this.hilitedFieldRank;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.BoxChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.BoxMoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = true;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.BoxSources)
			{
				this.isSourcesMenu = true;
				this.sourcesMenuSelected = -1;
				this.editor.Invalidate();
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
				this.editor.DirtySerialization = true;
			}
			else if (this.isFieldMoving)
			{
				if (this.hilitedElement == ActiveElement.BoxFieldMoving)
				{
					this.MoveField(this.fieldInitialRank, this.hilitedFieldRank);
				}
				this.isFieldMoving = false;
				this.editor.LockObject(null);
				this.editor.DirtySerialization = true;
			}
			else if (this.isChangeWidth)
			{
				this.editor.UpdateAfterMoving(this);
				this.isChangeWidth = false;
				this.editor.LockObject(null);
				this.editor.DirtySerialization = true;
			}
			else if (this.isMoveColumnsSeparator)
			{
				this.isMoveColumnsSeparator = false;
				this.editor.LockObject(null);
				this.editor.DirtySerialization = true;
			}
			else if (this.isSourcesMenu)
			{
				if (this.sourcesMenuSelected != -1)
				{
					SourceInfo info = this.sourcesList[this.sourcesMenuSelected];
					if (!info.Opened)
					{
						this.OpenSource(info.CultureMap, info.Rank);
					}

					this.isSourcesMenu = false;  // ferme le menu
					this.editor.LockObject(null);
					this.editor.Invalidate();
				}
				//	Si on est pas dans une case valide (par exemple si on a cliqué sans bouger
				//	dans l'en-tête), le menu reste.
			}
			else
			{
				if (this.hilitedElement == ActiveElement.BoxExtend)
				{
					this.IsExtended = !this.IsExtended;
					this.editor.UpdateAfterGeometryChanged(this);
					this.editor.DirtySerialization = true;
				}

				if (this.hilitedElement == ActiveElement.BoxClose)
				{
					if (!this.isRoot)
					{
						this.editor.CloseBox(this);
						this.editor.UpdateAfterAddOrRemoveConnection(null);
					}
				}

				if (this.hilitedElement == ActiveElement.BoxFieldRemove)
				{
					this.RemoveField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldAdd)
				{
					this.AddField(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldName)
				{
					this.ChangeFieldName(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldType)
				{
					this.ChangeFieldType(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxComment)
				{
					this.AddComment();
				}

				if (this.hilitedElement == ActiveElement.BoxColor1)
				{
					this.BackgroundMainColor = MainColor.Blue;
				}

				if (this.hilitedElement == ActiveElement.BoxColor2)
				{
					this.BackgroundMainColor = MainColor.Green;
				}

				if (this.hilitedElement == ActiveElement.BoxColor3)
				{
					this.BackgroundMainColor = MainColor.Red;
				}

				if (this.hilitedElement == ActiveElement.BoxColor4)
				{
					this.BackgroundMainColor = MainColor.Grey;
				}
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;
			this.SetConnectionsHilited(false);

			if (this.isSourcesMenu)
			{
				return false;
			}

			if (pos.IsZero)
			{
				//	Si l'une des connection est dans l'état ConnectionOpen*, il faut afficher
				//	aussi les petits cercles de gauche.
				if (this.IsConnectionReadyForOpen())
				{
					this.SetConnectionsHilited(true);
				}
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
						element = ActiveElement.BoxFieldMoving;
						fieldRank = i;
						return true;
					}
				}
			}
			else
			{
				//	Souris dans le bouton compact/étendu ?
				if (this.DetectRoundButton(this.PositionExtendButton, pos))
				{
					element = ActiveElement.BoxExtend;
					return true;
				}

				//	Souris dans le bouton de fermeture ?
				if (this.DetectRoundButton(this.PositionCloseButton, pos))
				{
					element = ActiveElement.BoxClose;
					return true;
				}

				//	Souris dans le bouton des sources ?
				if (this.sourcesList.Count > 0)
				{
					if (this.DetectRoundButton(this.PositionSourcesButton, pos))
					{
						element = ActiveElement.BoxSources;
						return true;
					}
				}

				//	Souris dans le bouton des commentaires ?
				if (this.DetectRoundButton(this.PositionCommentButton, pos))
				{
					element = ActiveElement.BoxComment;
					return true;
				}

				//	Souris dans le bouton des couleurs ?
				if (this.DetectSquareButton(this.PositionColorButton(0), pos))
				{
					element = ActiveElement.BoxColor1;
					return true;
				}

				if (this.DetectSquareButton(this.PositionColorButton(1), pos))
				{
					element = ActiveElement.BoxColor2;
					return true;
				}

				if (this.DetectSquareButton(this.PositionColorButton(2), pos))
				{
					element = ActiveElement.BoxColor3;
					return true;
				}

				if (this.DetectSquareButton(this.PositionColorButton(3), pos))
				{
					element = ActiveElement.BoxColor4;
					return true;
				}

				if (this.isExtended)
				{
					//	Souris dans le bouton pour changer la largeur ?
					//	Souris dans le bouton pour déplacer le séparateur des colonnes ?
					double d1 = Point.Distance(this.PositionChangeWidthButton, pos);
					double d2 = Point.Distance(this.PositionMoveColumnsButton, pos);

					if (d1 < d2)
					{
						if (d1 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.BoxChangeWidth;
							return true;
						}
					}
					else
					{
						if (d2 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.BoxMoveColumnsSeparator;
							return true;
						}
					}

					//	Souris dans l'en-tête ?
					if (this.bounds.Contains(pos) && 
						(pos.Y >= this.bounds.Top-AbstractObject.headerHeight ||
						 pos.Y <= this.bounds.Bottom+AbstractObject.footerHeight))
					{
						element = ActiveElement.BoxHeader;
						this.SetConnectionsHilited(true);
						return true;
					}

					//	Souris entre deux champs ?
					for (int i=-1; i<this.fields.Count; i++)
					{
						rect = this.GetFieldAddBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldAdd;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}
					}

					//	Souris sur le séparateur des colonnes ?
					double sep = this.ColumnsSeparatorAbsolute;
					if (this.columnsSeparatorRelative < 1.0 && pos.X >= sep-4 && pos.X <= sep+4 &&
						pos.Y >= this.bounds.Bottom+AbstractObject.footerHeight &&
						pos.Y <= this.bounds.Top-AbstractObject.headerHeight)
					{
						element = ActiveElement.BoxMoveColumnsSeparator;
						this.SetConnectionsHilited(true);
						return true;
					}

					//	Souris dans un champ ?
					for (int i=0; i<this.fields.Count; i++)
					{
						rect = this.GetFieldRemoveBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldRemove;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldMovableBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldMovable;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldNameBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldName;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldTypeBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldType;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}
					}
				}
				else  // boîte compactée ?
				{
					if (this.bounds.Contains(pos))
					{
						element = ActiveElement.BoxHeader;
						return true;
					}
				}
			}

			if (!this.bounds.Contains(pos))
			{
				return false;
			}

			element = ActiveElement.BoxInside;
			this.SetConnectionsHilited(true);
			return true;
		}

		protected void SetConnectionsHilited(bool isHilited)
		{
			//	Modifie l'état 'hilited' de toutes les connections qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons fermées ne sont affichés qu'à droite.
			foreach (Field field in this.fields)
			{
				if (field.Connection != null)
				{
					field.Connection.IsSrcHilited = isHilited;
				}
			}
		}

		protected bool IsConnectionReadyForOpen()
		{
			//	Indique si l'une des connections qui partent de l'objet est en mode ConnectionOpen*.
			foreach (Field field in this.fields)
			{
				if (field.Connection != null)
				{
					ActiveElement ae = field.Connection.HilitedElement;
					if (ae == ActiveElement.ConnectionOpenLeft ||
						ae == ActiveElement.ConnectionOpenRight)
					{
						return true;
					}
				}
			}

			return false;
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
			rect.Right = this.ColumnsSeparatorAbsolute;

			return rect;
		}

		protected Rectangle GetFieldTypeBounds(int rank)
		{
			//	Retourne le rectangle occupé par le type d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Left = this.ColumnsSeparatorAbsolute+1;

			return rect;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - AbstractObject.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
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
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(null);

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

			Common.Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Common.Support.ResourceAccessors.StructuredTypeResourceAccessor;
			CultureMap fieldCultureMap = accessor.CreateFieldItem(this.cultureMap);
			fieldCultureMap.Name = name;
			accessor.FieldAccessor.Collection.Add(fieldCultureMap);
			accessor.FieldAccessor.PersistChanges();

			//?IResourceAccessor fieldAccessor = accessor.FieldAccessor;
			//?CultureMap fieldCultureMap = fieldAccessor.Collection[fieldCaptionId];
			//?StructuredData fieldData = fieldCultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.Fields.ToString());
			StructuredData newField = broker.CreateData(this.cultureMap);

			Druid druid = fieldCultureMap.Id;  //?this.CreateFieldCaption(name);
			newField.SetValue(Support.Res.Fields.Field.CaptionId, druid);

			dataFields.Insert(rank+1, newField);
			accessor.PersistChanges();

			Field field = new Field(this.editor);
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
			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

			Common.Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Common.Support.ResourceAccessors.StructuredTypeResourceAccessor;
			IResourceAccessor fieldAccessor = accessor.FieldAccessor;
			
			CultureMap fieldCultureMap = fieldAccessor.Collection[fieldCaptionId];
			string name = fieldCultureMap.Name;
			
			Module module = this.editor.Module;
			name = module.MainWindow.DlgFieldName(name);
			if (string.IsNullOrEmpty(name))
			{
				this.hilitedElement = ActiveElement.None;
				return;
			}

			fieldCultureMap.Name = name;
			fieldAccessor.PersistChanges();
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
			druid = module.MainWindow.DlgResourceSelector(module, ResourceAccess.Type.Types2, TypeCode.Invalid, druid, null, null);
			if (druid.IsEmpty)
			{
				return;
			}

			if (this.fields[rank].Relation != FieldRelation.None && this.fields[rank].IsExplored)
			{
				ObjectBox dst = this.fields[rank].DstBox;
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(dst);
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
			//?Druid sourceId = (Druid) dataField.GetValue(Support.Res.Fields.Field.SourceFieldId);
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			
			Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
			CultureMap dstItem = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
			if (dstItem == null)
			{
				rel = FieldRelation.None;  // ce n'est pas une vraie relation !
			}

			Common.Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Common.Support.ResourceAccessors.StructuredTypeResourceAccessor;
			IResourceAccessor fieldAccessor = accessor.FieldAccessor;
			CultureMap fieldCultureMap = fieldAccessor.Collection[fieldCaptionId];
			//StructuredData fieldData = fieldCultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			//?Caption fieldCaption = this.editor.Module.AccessEntities.DirectGetCaption(fieldCaptionId);
			//?Caption typeCaption = typeId.IsEmpty ? null : this.editor.Module.AccessEntities.DirectGetCaption(typeId);

			Module typeModule = this.editor.Module.MainWindow.SearchModule(typeId);
			CultureMap typeCultureMap = null;
			if (typeModule != null)
			{
				typeCultureMap = typeModule.AccessTypes2.Accessor.Collection[typeId];
				if (typeCultureMap == null)
				{
					typeCultureMap = typeModule.AccessEntities.Accessor.Collection[typeId];
				}
			}

			field.CaptionId = fieldCaptionId;
			//?field.FieldName = (fieldCaption == null) ? "" : fieldCaption.Name;
			//?field.TypeName = (typeCaption == null) ? "" : typeCaption.Name;
			field.FieldName = (fieldCultureMap == null) ? "" : fieldCultureMap.Name;
			field.TypeName = (typeCultureMap == null) ? "" : typeCultureMap.Name;
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

#if false
		protected Druid CreateFieldCaption(string text)
		{
			//	Crée un nouveau Caption de type Field (dont le nom commence par "Fld.").
			//	TODO: remplacer cette cuisine par les nouveaux mécanismes...
			string name = string.Concat(this.Title, ".", text);

			//	Crée un Caption de type Field (dont le nom commence par "Fld.").
			ResourceAccess access = this.editor.Module.AccessCaptions;

			access.BypassFilterOpenAccess(ResourceAccess.Type.Fields, TypeCode.Invalid, null, null);
			Druid druid = access.BypassFilterCreate(access.GetCultureBundle(null), name, text);
			access.BypassFilterCloseAccess();

			return druid;
		}
#endif

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


		protected void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor);
				this.comment.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Width = System.Math.Max(rect.Width, AbstractObject.commentMinWidth);
				rect.Bottom = rect.Top+20;
				rect.Height = 50;  // hauteur arbitraire
				this.comment.SetBounds(rect);
				this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment(this.comment);
				this.editor.UpdateAfterCommentChanged();

				this.comment.EditComment();  // édite tout de suite le texte du commentaire
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.DirtySerialization = true;
		}


		/// <summary>
		/// Informations sur une entité source, ouverte ou fermée.
		/// </summary>
		protected class SourceInfo
		{
			public string ModuleName;
			public string FieldName;
			public CultureMap CultureMap;
			public int Rank;
			public bool Opened;
		}

		public void UpdateAfterOpenOrCloseBox()
		{
			//	Appelé après avoir ajouté ou supprimé une boîte.
			this.sourcesClosedCount = 0;
			for (int i=0; i<this.sourcesList.Count; i++)
			{
				SourceInfo info = this.sourcesList[i];

				info.Opened = false;
				foreach (ObjectBox box in this.editor.Boxes)
				{
					if (box.cultureMap == info.CultureMap)
					{
						info.Opened = true;
						break;
					}
				}

				if (!info.Opened)
				{
					this.sourcesClosedCount++;  // màj le nombre de sources fermées
				}
			}
		}

		protected void UpdateSources()
		{
			//	Met à jour la liste de toutes les sources potentielles de l'entité courante.
			this.sourcesList = new List<SourceInfo>();

			List<Module> modules = this.editor.Module.MainWindow.Modules;
			foreach (Module module in modules)
			{
				foreach (CultureMap cultureMap in module.AccessEntities.Accessor.Collection)
				{
					StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
					IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

					if (dataFields != null)
					{
						int i = 0;
						foreach (StructuredData dataField in dataFields)
						{
							Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
							Module dstModule = this.editor.Module.MainWindow.SearchModule(typeId);
							CultureMap fieldCultureMap = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
							
							if (fieldCultureMap == this.cultureMap && !this.IsExistingSourceInfo(cultureMap))
							{
								SourceInfo info = new SourceInfo();
								
								info.CultureMap = cultureMap;
								info.ModuleName = module.ModuleInfo.Name;
								info.FieldName = cultureMap.Name;
								info.Rank = i;
								info.Opened = false;

								this.sourcesList.Add(info);
							}

							i++;
						}
					}
				}
			}

			this.sourcesClosedCount = this.sourcesList.Count;
		}

		protected bool IsExistingSourceInfo(CultureMap cultureMap)
		{
			foreach (SourceInfo info in this.sourcesList)
			{
				if (info.CultureMap == cultureMap)
				{
					return true;
				}
			}
			return false;
		}

		protected void OpenSource(CultureMap cultureMap, int rank)
		{
			//	Ouvre une entité source.
			ObjectBox box = this.editor.SearchBox(cultureMap.Name);
			if (box == null)
			{
				//	Ouvre la connection sur une nouvelle boîte.
				box = new ObjectBox(this.editor);
				box.BackgroundMainColor = this.boxColor;
				box.SetContent(cultureMap);

				Field field = box.Fields[rank];
				field.DstBox = this;
				field.IsAttachToRight = true;
				field.IsExplored = true;

				this.editor.AddBox(box);
				this.editor.UpdateGeometry();

				//	Essaie de trouver une place libre, pour déplacer le moins possible d'éléments.
				double oy = box.bounds.Top - box.GetConnectionSrcVerticalPosition(rank) - AbstractObject.headerHeight/2;

				Rectangle bounds = new Rectangle(this.bounds.Left-50-box.Bounds.Width, this.bounds.Top+oy-box.bounds.Height, box.Bounds.Width, box.Bounds.Height);
				bounds.Inflate(50, Editor.pushMargin);

				for (int i=0; i<1000; i++)
				{
					if (this.editor.IsEmptyArea(bounds))
					{
						break;
					}
					bounds.Offset(-1, 0);
				}

				bounds.Deflate(50, Editor.pushMargin);
				box.SetBounds(bounds);
			}

			this.editor.UpdateAfterAddOrRemoveConnection(box);
			this.editor.DirtySerialization = true;
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.BoxHeader);

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
			Color c1 = this.GetColorMain(dragging ? 0.6 : 0.4);
			Color c2 = this.GetColorMain(dragging ? 0.2 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = Color.FromBrightness(0.9);
			if (dragging)
			{
				colorLine = this.GetColorMain(0.3);
			}

			Color colorFrame = dragging ? this.GetColorMain() : Color.FromBrightness(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = this.GetColorMain(dragging ? 0.2 : 0.1);
				Color ci2 = this.GetColorMain(0.0);
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				//	Trait vertical de séparation.
				if (this.columnsSeparatorRelative < 1.0)
				{
					double posx = System.Math.Floor(this.ColumnsSeparatorAbsolute)+0.5;
					graphics.AddLine(posx, this.bounds.Bottom+AbstractObject.footerHeight+0.5, posx, this.bounds.Top-AbstractObject.headerHeight-0.5);
					graphics.RenderSolid(colorLine);
				}

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			rect = new Rectangle(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight, this.bounds.Width, AbstractObject.headerHeight);
			rect.Deflate(4, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness(0), GlyphPaintStyle.Normal);

			//	Dessine le bouton compact/étendu.
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			if (this.hilitedElement == ActiveElement.BoxExtend)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, false, false);
			}

			//	Dessine le bouton de fermeture.
			if (this.hilitedElement == ActiveElement.BoxClose)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false, !this.isRoot);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false, !this.isRoot);
			}

			//	Dessine le moignon pour les sources à gauche.
			if (this.sourcesClosedCount > 0)
			{
				Point p1 = this.PositionSourcesButton;
				Point p2 = p1;
				p1.X = this.bounds.Left-1-AbstractObject.lengthClose;
				p2.X = this.bounds.Left-1;
				graphics.LineWidth = 2;
				graphics.AddLine(p1, p2);
				this.DrawEndingArrow(graphics, p1, p2, FieldRelation.Reference);
				graphics.LineWidth = 1;
				graphics.RenderSolid(colorFrame);
			}

			//	Dessine le bouton des sources.
			if (this.hilitedElement == ActiveElement.BoxSources)
			{
				this.DrawRoundButton(graphics, this.PositionSourcesButton, AbstractObject.buttonRadius, GlyphShape.TriangleDown, true, false, this.sourcesList.Count > 0);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionSourcesButton, AbstractObject.buttonRadius, GlyphShape.TriangleDown, false, false, this.sourcesList.Count > 0);
			}

			//	Dessine le bouton des commentaires.
			if (this.hilitedElement == ActiveElement.BoxComment)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", false, false);
			}

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				for (int i=0; i<this.fields.Count; i++)
				{
					Color colorName = Color.FromBrightness(0);
					Color colorType = Color.FromBrightness(0);

					if (this.hilitedElement == ActiveElement.BoxFieldName && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldNameBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain());

						colorName = Color.FromBrightness(1);
					}

					if (this.hilitedElement == ActiveElement.BoxFieldType && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldTypeBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain());

						colorType = Color.FromBrightness(1);
					}

					if ((this.hilitedElement == ActiveElement.BoxFieldRemove || this.hilitedElement == ActiveElement.BoxFieldMovable) && this.hilitedFieldRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain(0.3));
					}

					if (this.isFieldMoving && this.fieldInitialRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain(0.3));
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

				if (this.hilitedElement == ActiveElement.BoxFieldMoving)
				{
					Point p1 = this.GetFieldBounds(this.fieldInitialRank).Center;
					Point p2 = this.GetFieldMovingBounds(this.hilitedFieldRank).Center;
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					this.DrawMovingArrow(graphics, p1, p2);
				}

				if (this.hilitedElement != ActiveElement.None &&
					this.hilitedElement != ActiveElement.BoxChangeWidth &&
					this.hilitedElement != ActiveElement.BoxMoveColumnsSeparator &&
					!this.IsHeaderHilite && !this.isFieldMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator)
				{
					//	Dessine la glissière à gauche pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(-1).Center;
					Point p2 = this.GetFieldAddBounds(this.fields.Count-1).Center;
					bool hilited = this.hilitedElement == ActiveElement.BoxFieldAdd || this.hilitedElement == ActiveElement.BoxFieldRemove;
					this.DrawEmptySlider(graphics, p1, p2, hilited);

					//	Dessine la glissière à droite pour suggérer les boutons Movable des champs.
					if (this.fields.Count != 0)
					{
						p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
						hilited = this.hilitedElement == ActiveElement.BoxFieldMovable;
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
				if (this.hilitedElement == ActiveElement.BoxFieldRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Minus, true, true);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldAdd)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorMain());

					rect = this.GetFieldAddBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Plus, true, true);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldMovable)
				{
					rect = this.GetFieldMovableBounds(this.hilitedFieldRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.VerticalMove, true, true);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldMoving)
				{
					rect = this.GetFieldBounds(this.hilitedFieldRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorMain());
				}
			}

			//	Dessine le bouton des couleurs.
			if (this.hilitedElement == ActiveElement.BoxColor1)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(0), MainColor.Blue, this.boxColor == MainColor.Blue, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(0), MainColor.Blue, this.boxColor == MainColor.Blue, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor2)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(1), MainColor.Green, this.boxColor == MainColor.Green, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(1), MainColor.Green, this.boxColor == MainColor.Green, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor3)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(2), MainColor.Red, this.boxColor == MainColor.Red, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(2), MainColor.Red, this.boxColor == MainColor.Red, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor4)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(3), MainColor.Grey, this.boxColor == MainColor.Grey, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(3), MainColor.Grey, this.boxColor == MainColor.Grey, false);
			}

			if (this.isExtended)
			{
				//	Dessine le bouton pour déplacer le séparateur des colonnes.
				if (this.hilitedElement == ActiveElement.BoxMoveColumnsSeparator)
				{
					double sep = this.ColumnsSeparatorAbsolute;
					graphics.LineWidth = 4;
					graphics.AddLine(sep, this.bounds.Bottom+AbstractObject.footerHeight+3, sep, this.bounds.Top-AbstractObject.headerHeight-3);
					graphics.LineWidth = 1;
					graphics.RenderSolid(this.GetColorMain());

					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.BoxHeader && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}

				//	Dessine le bouton pour changer la largeur.
				if (this.hilitedElement == ActiveElement.BoxChangeWidth)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.hilitedElement == ActiveElement.BoxHeader && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}
			}
		}

		protected bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				return (this.hilitedElement == ActiveElement.BoxHeader ||
						this.hilitedElement == ActiveElement.BoxSources ||
						this.hilitedElement == ActiveElement.BoxComment ||
						this.hilitedElement == ActiveElement.BoxColor1 ||
						this.hilitedElement == ActiveElement.BoxColor2 ||
						this.hilitedElement == ActiveElement.BoxColor3 ||
						this.hilitedElement == ActiveElement.BoxColor4 ||
						this.hilitedElement == ActiveElement.BoxExtend ||
						this.hilitedElement == ActiveElement.BoxClose);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
			if (this.isSourcesMenu)
			{
				this.DrawSourcesMenu(graphics, this.PositionSourcesMenu);
			}
		}

		protected void DrawSourcesMenu(Graphics graphics, Point pos)
		{
			//	Dessine le menu pour choisir une entité source.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			double h = ObjectBox.sourcesMenuHeight;
			Rectangle box = this.RectangleSourcesMenu;

			//	Dessine la boîte vide ombrée.
			Rectangle big = box;
			big.Inflate(7);

			Rectangle rs = big;
			rs.Inflate(2);
			rs.Offset(8, -8);
			this.DrawShadow(graphics, rs, 18, 10, 0.6);

			Path path = this.PathRoundRectangle(big, 10);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));
			graphics.Rasterizer.AddSurface(path);
			this.RenderHorizontalGradient(graphics, big, this.GetColorMain(0.6), this.GetColorMain(0.2));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(Color.FromBrightness(0));

			graphics.AddFilledRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'en-tête.
			Rectangle rect = box;
			rect.Bottom = rect.Top-h-1;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.GetColorMain());

			Rectangle gr = new Rectangle(this.PositionSourcesButton.X-AbstractObject.buttonRadius, this.PositionSourcesButton.Y-AbstractObject.buttonRadius, AbstractObject.buttonRadius*2, AbstractObject.buttonRadius*2);
			adorner.PaintGlyph(graphics, gr, WidgetPaintState.Enabled, Color.FromBrightness(1), GlyphShape.TriangleDown, PaintTextStyle.Button);
			
			graphics.AddText(rect.Left+AbstractObject.buttonRadius*2+5, rect.Bottom+1, rect.Width-(AbstractObject.buttonRadius*2+10), rect.Height, "Entités sources", Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleLeft);
			graphics.RenderSolid(Color.FromBrightness(1));
			
			rect = box;
			rect.Top = rect.Bottom+h;
			rect.Offset(0, h*(this.sourcesList.Count-1));

			//	Dessine les lignes du menu.
			for (int i=0; i<this.sourcesList.Count; i++)
			{
				SourceInfo info = this.sourcesList[i];

				if (info.Opened)
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(Color.FromBrightness(0.9));
				}
				else if (i == this.sourcesMenuSelected)
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.GetColorMain(0.2));
				}

				string text = string.Concat(info.ModuleName, ": ", info.FieldName);
				graphics.AddText(rect.Left+5, rect.Bottom, rect.Width-10, rect.Height, text, Font.DefaultFont, 10, ContentAlignment.MiddleLeft);
				graphics.RenderSolid(Color.FromBrightness(info.Opened ? 0.3 : 0));

				graphics.AddLine(rect.TopLeft, rect.TopRight);
				graphics.RenderSolid(Color.FromBrightness(0));

				rect.Offset(0, -h);
			}

			//	Dessine le cadre du menu.
			graphics.AddRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(0));
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
				hiliteColor = this.GetColorMain();
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
			graphics.RenderSolid(this.GetColorMain());
		}

		protected Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point(this.bounds.Right-AbstractObject.buttonRadius-6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionExtendButton
		{
			//	Retourne la position du bouton pour étendre.
			get
			{
				return new Point(this.bounds.Right-AbstractObject.buttonRadius*3-8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionChangeWidthButton
		{
			//	Retourne la position du bouton pour changer la largeur.
			get
			{
				return new Point(this.bounds.Right-1, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
			}
		}

		protected Point PositionMoveColumnsButton
		{
			//	Retourne la position du bouton pour déplacer le séparateur des colonnes.
			get
			{
				return new Point(this.ColumnsSeparatorAbsolute, this.bounds.Bottom+AbstractObject.footerHeight/2+1);
			}
		}

		protected Point PositionSourcesButton
		{
			//	Retourne la position du bouton pour montrer les sources.
			get
			{
				return new Point(this.bounds.Left+AbstractObject.buttonRadius+6, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer les sources.
			get
			{
				return new Point(this.bounds.Left+AbstractObject.buttonRadius*3+8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			if (this.IsExtended)
			{
				return new Point(this.bounds.Left-2+(AbstractObject.buttonSquare+0.5)*(rank+1)*2, this.bounds.Bottom+4+AbstractObject.buttonSquare);
			}
			else
			{
				return Point.Zero;
			}
		}

		protected Point PositionSourcesMenu
		{
			//	Retourne la position du menu pour montrer les sources.
			get
			{
				Point pos = this.PositionSourcesButton;
				pos.X -= AbstractObject.buttonRadius;
				pos.Y += AbstractObject.buttonRadius;
				return pos;
			}
		}

		protected Rectangle RectangleSourcesMenu
		{
			//	Retourne le rectangle du menu pour montrer les sources.
			get
			{
				Point pos = this.PositionSourcesMenu;
				double h = ObjectBox.sourcesMenuHeight*(this.sourcesList.Count+1);
				Rectangle rect = new Rectangle(pos.X, pos.Y-h, 200, h);
				rect.Inflate(0.5);
				return rect;
			}
		}

		protected double ColumnsSeparatorAbsolute
		{
			//	Retourne la position absolue du séparateur des colonnes.
			get
			{
				Rectangle rect = this.bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				return rect.Left + rect.Width*this.columnsSeparatorRelative;
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
			//	Sérialise toutes les informations de la boîte et de ses champs.
			writer.WriteStartElement(Xml.Box);
			
			writer.WriteElementString(Xml.Druid, this.cultureMap.Id.ToString());
			writer.WriteElementString(Xml.Bounds, this.bounds.ToString());
			writer.WriteElementString(Xml.IsExtended, this.isExtended.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.columnsSeparatorRelative != 0.5)
			{
				writer.WriteElementString(Xml.ColumnsSeparatorRelative, this.columnsSeparatorRelative.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
			
			writer.WriteElementString(Xml.Color, this.boxColor.ToString());

			foreach (Field field in this.fields)
			{
				field.WriteXml(writer);
			}

			if (this.comment != null && this.comment.IsVisible)  // commentaire associé ?
			{
				this.comment.WriteXml(writer);
			}
			
			writer.WriteEndElement();
		}

		public void ReadXml(XmlReader reader)
		{
			this.fields.Clear();

			reader.Read();

			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == Xml.Field)
					{
						Field field = new Field(this.editor);
						field.ReadXml(reader);
						reader.Read();
						this.fields.Add(field);
					}
					else if (name == Xml.Comment)
					{
						this.comment = new ObjectComment(this.editor);
						this.comment.ReadXml(reader);
						this.comment.AttachObject = this;
						this.comment.UpdateHeight();  // adapte la hauteur en fonction du contenu
						this.editor.AddComment(this.comment);
						reader.Read();
					}
					else
					{
						string element = reader.ReadElementString();

						if (name == Xml.Druid)
						{
							Druid druid = Druid.Parse(element);
							if (druid.IsValid)
							{
								this.cultureMap = this.editor.Module.AccessEntities.Accessor.Collection[druid];
							}
						}
						else if (name == Xml.Bounds)
						{
							this.bounds = Rectangle.Parse(element);
						}
						else if (name == Xml.IsExtended)
						{
							this.isExtended = bool.Parse(element);
						}
						else if (name == Xml.ColumnsSeparatorRelative)
						{
							this.columnsSeparatorRelative = double.Parse(element);
						}
						else if (name == Xml.Color)
						{
							this.boxColor = (MainColor) System.Enum.Parse(typeof(MainColor), element);
						}
						else
						{
							throw new System.NotSupportedException(string.Format("Unexpected XML node {0} found in box", name));
						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					System.Diagnostics.Debug.Assert(reader.Name == Xml.Box);
					break;
				}
				else
				{
					reader.Read();
				}
			}
		}

		public void AdjustAfterRead()
		{
			//	Ajuste le contenu de la boîte après sa désérialisation.
			this.Title = this.cultureMap.Name;
			this.isRoot = (this == this.editor.Boxes[0]);  // la première boîte est toujours la boîte racine

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			//	La liste des champs (this.fields) désérialisée n'est pas utilisable telle quelle.
			//	En effet, des champs peuvent avoir été ajoutés, supprimés ou permutés. Il faut donc
			//	générer une liste propre, comme SetContent, puis repiquer les informations utiles
			//	dans la liste désérialisée.
			List<Field> newFields = new List<Field>();  // nouvelle liste propre
			if (dataFields != null)
			{
				for (int i=0; i<dataFields.Count; i++)
				{
					Field field = new Field(this.editor);
					this.UpdateField(dataFields[i], field);

					Druid fieldCaptionId = (Druid) dataFields[i].GetValue(Support.Res.Fields.Field.CaptionId);
					Field rField = this.AdjustAfterReadSearchField(fieldCaptionId);  // cherche le champ correspondant désérialisé
					if (rField != null)
					{
						rField.DeserializeCopyTo(field);  // repique les informations utiles

						if (rField.IsExplored)  // champ avec connection explorée ?
						{
							if (field.Destination == rField.Destination)  // (*)
							{
								field.IsExplored = true;  // ObjectConnection sera créé par Editor.CreateConnections
								field.DstBox = this.AdjustAfterReadSearchBox(rField.Destination);
							}
						}
					}

					newFields.Add(field);  // ajoute le champ dans la nouvelle liste propre
				}
			}
			this.fields = newFields;  // la nouvelle liste propre remplace la liste désérialisée

			this.UpdateFields();
			this.UpdateSources();
		}

		// (*)	Si ce test n'est pas vrai, il s'agit d'un champ relation dont on a modifié l'entité
		//		destination entre la sérialisation et la présente désérialisation. Le Editor.CloseBox()
		//		fermera les entités que plus personne ne pointe (field.IsExplored = false dans ce cas).

		protected Field AdjustAfterReadSearchField(Druid druid)
		{
			foreach (Field field in this.fields)
			{
				if (field.CaptionId == druid)
				{
					return field;
				}
			}

			return null;
		}

		protected ObjectBox AdjustAfterReadSearchBox(Druid druid)
		{
			foreach (ObjectBox box in this.editor.Boxes)
			{
				if (box.CultureMap.Id == druid)
				{
					return box;
				}
			}

			return null;
		}
		#endregion


		protected static readonly double roundFrameRadius = 12;
		protected static readonly double shadowOffset = 6;
		protected static readonly double textMargin = 13;
		protected static readonly double fieldHeight = 20;
		protected static readonly double sourcesMenuHeight = 20;

		protected CultureMap cultureMap;
		protected ObjectComment comment;
		protected Rectangle bounds;
		protected double columnsSeparatorRelative;
		protected bool isRoot;
		protected bool isExtended;
		protected bool isConnectedToRoot;
		protected string titleString;
		protected TextLayout title;
		protected List<Field> fields;
		protected List<SourceInfo> sourcesList;
		protected int sourcesClosedCount;
		protected List<ObjectConnection> connectionListBt;
		protected List<ObjectConnection> connectionListBb;
		protected List<ObjectConnection> connectionListC;
		protected List<ObjectConnection> connectionListD;
		protected List<ObjectBox> parents;

		protected bool isDragging;
		protected Point draggingPos;

		protected bool isFieldMoving;
		protected int fieldInitialRank;

		protected bool isChangeWidth;
		protected double changeWidthPos;
		protected double changeWidthInitial;

		protected bool isMoveColumnsSeparator;

		protected bool isSourcesMenu;
		protected int sourcesMenuSelected;
	}
}
