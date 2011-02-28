using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
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

			this.subtitle = new TextLayout();
			this.subtitle.DefaultFontSize = 9;
			this.subtitle.Alignment = ContentAlignment.MiddleCenter;
			this.subtitle.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.fields = new List<Field>();

			this.columnsSeparatorRelative1 = 0.5;
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
			//	Titre au sommet de la boîte (nom de l'entité).
			get
			{
				return this.titleString;
			}
			set
			{
				if (this.titleString != value)
				{
					this.titleString = value;

					this.title.Text = Misc.Bold(this.titleString);
				}
			}
		}

		protected string Subtitle
		{
			//	Sous-titre au sommet de la boîte, juste sous le titre (nom du module).
			get
			{
				return this.subtitleString;
			}
			set
			{
				if (this.subtitleString != value)
				{
					this.subtitleString = value;

					if (string.IsNullOrEmpty(this.subtitleString))
					{
						this.subtitle.Text = null;
					}
					else
					{
						this.subtitle.Text = Misc.Italic(this.subtitleString);
					}
				}
			}
		}

		public void SetContent(CultureMap cultureMap)
		{
			//	Initialise le contenu de la boîte.
			this.cultureMap = cultureMap;

			this.Title = this.cultureMap.Name;
			this.UpdateSubtitle();

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			this.fields.Clear();
			if (dataFields != null)
			{
				for (int i=0; i<dataFields.Count; i++)
				{
					Field field = new Field(this.editor);
					field.Initialize(this, dataFields[i]);
					this.fields.Add(field);
				}
			}

			this.UpdateFieldsContent();
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

		public ObjectInfo Info
		{
			//	Informations liées.
			get
			{
				return this.info;
			}
			set
			{
				this.info = value;
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

			//	S'il existe des informations associées, elles doivent aussi être déplacées.
			if (this.info != null)
			{
				Rectangle rect = this.info.InternalBounds;
				rect.Offset(p2-p1);
				this.info.SetBounds(rect);
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

					//	Change la couleur de toutes les connections liées.
					foreach (Field field in this.fields)
					{
						if (field.Connection != null)
						{
							field.Connection.BackgroundMainColor = this.boxColor;
						}
					}

					//	Change la couleur des informations liées.
					if (this.info != null)
					{
						this.info.BackgroundMainColor = this.boxColor;
					}

					this.editor.Invalidate();
					this.editor.Module.AccessEntities.SetLocalDirty();
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

					this.UpdateFieldsLink();
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


		public void UpdateTitle()
		{
			//	Met à jour le titre de la boîte.
			this.Title = this.cultureMap.Name;
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

					if (this.isExtended && this.sourcesClosedCount > 0 && this.IsInterface)
					{
						//	En dessous du glyph 'o--' et du moignon "source".
						return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight-12);
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
				double y = this.bounds.Top-AbstractObject.headerHeight;
				if (posv >= y-ObjectBox.fieldHeight/2 && posv <= y+ObjectBox.fieldHeight/2)  // sur le moignon "source" ?
				{
					return false;
				}
			}

			if (!right && this.isExtended && this.IsInterface)
			{
				double y = this.bounds.Top-AbstractObject.headerHeight*0.5;
				if (posv >= y-ObjectBox.fieldHeight/2 && posv <= y+ObjectBox.fieldHeight/2)  // sur le glyph 'o--' ?
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


		protected override string GetToolTipText(ActiveElement element, int fieldRank)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDragging || this.isFieldMoving || this.isChangeWidth || this.isMoveColumnsSeparator1 || this.isSourcesMenu)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case ActiveElement.BoxHeader:
					if (this.editor.BoxCount == 1)
					{
						return null;
					}
					else
					{
						if (this.isRoot)
						{
							return Res.Strings.Entities.Action.BoxHeader;
						}
						else if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
						{
							return Res.Strings.Entities.Action.BoxHeader1;
						}
						else
						{
							return Res.Strings.Entities.Action.BoxHeader2;
						}
					}

				case ActiveElement.BoxSources:
					if (this.sourcesList.Count == 0)
					{
						return null;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxSources1;
					}

				case ActiveElement.BoxExtend:
					if (this.isExtended)
					{
						return Res.Strings.Entities.Action.BoxExtend1;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxExtend2;
					}

				case ActiveElement.BoxComment:
					if (this.comment == null)
					{
						return Res.Strings.Entities.Action.BoxComment1;
					}
					else if (!this.comment.IsVisible)
					{
						return string.Format(Res.Strings.Entities.Action.BoxComment2, this.comment.Text);
					}
					else
					{
						return Res.Strings.Entities.Action.BoxComment3;
					}

				case ActiveElement.BoxInfo:
					if (this.info == null || !this.info.IsVisible)
					{
						return string.Format (Res.Strings.Entities.Action.BoxInfo1, this.GetInformations (true));
					}
					else
					{
						return Res.Strings.Entities.Action.BoxInfo2;
					}

				case ActiveElement.BoxParameters:
					return Res.Strings.Entities.Action.BoxParameters;

				case ActiveElement.BoxClose:
					if (this.isRoot)
					{
						return null;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxClose;
					}

				case ActiveElement.BoxFieldGroup:
					return this.GetGroupTooltip(fieldRank);

				case ActiveElement.BoxFieldExpression:
					string expression = this.fields[fieldRank].LocalExpression;
					string deepExpression = this.fields[fieldRank].InheritedExpression;
					if (!string.IsNullOrEmpty(expression))
					{
						return string.Format(Res.Strings.Entities.Action.BoxFieldExpression1, expression);
					}
					else if (expression != "" && !string.IsNullOrEmpty(deepExpression))
					{
						return string.Format(Res.Strings.Entities.Action.BoxFieldExpression1, deepExpression);
					}
					else
					{
						return Res.Strings.Entities.Action.BoxFieldExpression;
					}
			}

			return base.GetToolTipText(element, fieldRank);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				Rectangle bounds = this.editor.BoxGridAlign (new Rectangle (pos-this.draggingOffset, this.Bounds.Size));
				this.SetBounds(bounds);
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isFieldMoving)
			{
				return base.MouseMove(message, pos);
			}
			else if (this.isChangeWidth)
			{
				Rectangle bounds = this.Bounds;
				bounds.Width = this.editor.GridAlign(System.Math.Max(pos.X-this.changeWidthPos+this.changeWidthInitial, 120));
				this.SetBounds(bounds);
				this.editor.UpdateConnections();
				return true;
			}
			else if (this.isMoveColumnsSeparator1)
			{
				Rectangle rect = this.Bounds;
				rect.Deflate(ObjectBox.textMargin, 0);
				pos.X = System.Math.Min(pos.X, this.ColumnsSeparatorAbsolute(1));
				this.columnsSeparatorRelative1 = (pos.X-rect.Left)/rect.Width;
				this.columnsSeparatorRelative1 = System.Math.Max(this.columnsSeparatorRelative1, 0.2);
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
				return base.MouseMove(message, pos);
			}
		}

		public override void MouseDown(Message message, Point pos)
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

			if (this.hilitedElement == ActiveElement.BoxHeader && this.editor.BoxCount > 1 && !this.editor.IsLocateActionHeader(message))
			{
				this.isDragging = true;
				this.draggingOffset = pos-this.bounds.BottomLeft;
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

			if (this.hilitedElement == ActiveElement.BoxMoveColumnsSeparator1)
			{
				this.isMoveColumnsSeparator1 = true;
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

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.isDragging)
			{
				this.editor.UpdateAfterMoving(this);
				this.isDragging = false;
				this.editor.LockObject(null);
				this.editor.Module.AccessEntities.SetLocalDirty();
			}
			else if (this.isFieldMoving)
			{
				if (this.hilitedElement == ActiveElement.BoxFieldMoving)
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
				this.editor.Module.AccessEntities.SetLocalDirty();
			}
			else if (this.isMoveColumnsSeparator1)
			{
				this.isMoveColumnsSeparator1 = false;
				this.editor.LockObject(null);
				this.editor.Module.AccessEntities.SetLocalDirty();
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
				if (this.hilitedElement == ActiveElement.BoxHeader && this.editor.IsLocateActionHeader(message) && !this.isRoot)
				{
					this.LocateEntity();
				}

				if (this.hilitedElement == ActiveElement.BoxExtend)
				{
					this.IsExtended = !this.IsExtended;
					this.editor.UpdateAfterGeometryChanged(this);
					this.editor.Module.AccessEntities.SetLocalDirty();
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

				if (this.hilitedElement == ActiveElement.BoxFieldAddInterface)
				{
					this.AddInterface();
				}

				if (this.hilitedElement == ActiveElement.BoxFieldRemoveInterface)
				{
					this.RemoveInterface(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldName)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateField(this.hilitedFieldRank);
					}
					else
					{
						if (this.IsMousePossible(this.hilitedElement, this.hilitedFieldRank))
						{
							this.ChangeFieldType(this.hilitedFieldRank);
						}
					}
				}

				if (this.hilitedElement == ActiveElement.BoxFieldType)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateType(this.hilitedFieldRank);
					}
					else
					{
						if (this.IsMousePossible(this.hilitedElement, this.hilitedFieldRank))
						{
							this.ChangeFieldType(this.hilitedFieldRank);
						}
					}
				}

				if (this.hilitedElement == ActiveElement.BoxFieldExpression)
				{
					this.EditExpression(this.hilitedFieldRank);
				}

				if (this.hilitedElement == ActiveElement.BoxFieldTitle)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateTitle(this.hilitedFieldRank);
					}
				}

				if (this.hilitedElement == ActiveElement.BoxComment)
				{
					this.AddComment();
				}

				if (this.hilitedElement == ActiveElement.BoxInfo)
				{
					this.AddInfo ();
				}

				if (this.hilitedElement == ActiveElement.BoxParameters)
				{
					this.ChangeParameters ();
				}

				if (this.hilitedElement == ActiveElement.BoxColor5)
				{
					this.BackgroundMainColor = MainColor.Yellow;
				}

				if (this.hilitedElement == ActiveElement.BoxColor6)
				{
					this.BackgroundMainColor = MainColor.Orange;
				}

				if (this.hilitedElement == ActiveElement.BoxColor3)
				{
					this.BackgroundMainColor = MainColor.Red;
				}

				if (this.hilitedElement == ActiveElement.BoxColor7)
				{
					this.BackgroundMainColor = MainColor.Lilac;
				}

				if (this.hilitedElement == ActiveElement.BoxColor8)
				{
					this.BackgroundMainColor = MainColor.Purple;
				}

				if (this.hilitedElement == ActiveElement.BoxColor1)
				{
					this.BackgroundMainColor = MainColor.Blue;
				}

				if (this.hilitedElement == ActiveElement.BoxColor2)
				{
					this.BackgroundMainColor = MainColor.Green;
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
					if (i >= this.skippedField-1 && rect.Contains(pos))
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
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionExtendButton, pos))
				{
					element = ActiveElement.BoxExtend;
					return true;
				}

				//	Souris dans le bouton de fermeture ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionCloseButton, pos))
				{
					element = ActiveElement.BoxClose;
					return true;
				}

				//	Souris dans le bouton des sources ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.sourcesList.Count > 0)
				{
					if (this.DetectRoundButton(this.PositionSourcesButton, pos))
					{
						element = ActiveElement.BoxSources;
						return true;
					}
				}

				//	Souris dans le bouton des commentaires ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionCommentButton, pos))
				{
					element = ActiveElement.BoxComment;
					return true;
				}

				//	Souris dans le bouton des informations ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionInfoButton, pos))
				{
					element = ActiveElement.BoxInfo;
					return true;
				}

				//	Souris dans le bouton des paramètres ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton (this.PositionParametersButton, pos))
				{
					element = ActiveElement.BoxParameters;
					return true;
				}

				//	Souris dans le bouton des couleurs ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(0), pos))
				{
					element = ActiveElement.BoxColor5;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(1), pos))
				{
					element = ActiveElement.BoxColor6;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(2), pos))
				{
					element = ActiveElement.BoxColor3;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(3), pos))
				{
					element = ActiveElement.BoxColor7;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (4), pos))
				{
					element = ActiveElement.BoxColor8;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (5), pos))
				{
					element = ActiveElement.BoxColor1;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (6), pos))
				{
					element = ActiveElement.BoxColor2;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (7), pos))
				{
					element = ActiveElement.BoxColor4;
					return true;
				}

				if (this.isExtended)
				{
					//	Souris dans le bouton pour changer la largeur ?
					//	Souris dans le bouton pour déplacer le séparateur des colonnes ?
					double d1 = Point.Distance(this.PositionChangeWidthButton, pos);
					double d2;
					
					d2 = Point.Distance(this.PositionMoveColumnsButton(0), pos);
					if (d1 < d2)
					{
						if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && d1 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.BoxChangeWidth;
							return true;
						}
					}
					else
					{
						if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && d2 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.BoxMoveColumnsSeparator1;
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

					//	Souris dans un titre ?
					for (int i=0; i<this.fields.Count; i++)
					{
						if (this.fields[i].IsTitle || this.fields[i].IsSubtitle)
						{
							if (this.fields[i].IsTitle)
							{
								rect = this.GetFieldMovableBounds(i);
								if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked &&
									this.fields[i].IsInterfaceOrInterfaceTitle &&
									(!this.editor.Module.IsPatch || this.fields[i].CultureMapSource != CultureMapSource.ReferenceModule) &&
									rect.Contains(pos))
								{
									element = ActiveElement.BoxFieldRemoveInterface;
									fieldRank = i;
									return true;
								}
							}

							rect = this.GetFieldBounds(i);
							if (rect.Contains(pos))
							{
								element = ActiveElement.BoxFieldTitle;
								fieldRank = i;
								return true;
							}
						}
					}

					//	Souris entre deux champs ?
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						for (int i=-1; i<this.fields.Count; i++)
						{
							rect = this.GetFieldAddBounds(i);
							if (i >= this.skippedField-1 && rect.Contains(pos))
							{
								element = ActiveElement.BoxFieldAdd;
								fieldRank = i;
								this.SetConnectionsHilited(true);
								return true;
							}
						}
					}

					//	Souris sur le séparateur des colonnes ?
					double sep = this.ColumnsSeparatorAbsolute(0);
					if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked &&
						this.columnsSeparatorRelative1 < 1.0 && pos.X >= sep-4 && pos.X <= sep+4 &&
						pos.Y >= this.bounds.Bottom+AbstractObject.footerHeight &&
						pos.Y <= this.bounds.Top-AbstractObject.headerHeight)
					{
						element = ActiveElement.BoxMoveColumnsSeparator1;
						this.SetConnectionsHilited(true);
						return true;
					}

					//	Souris sur le bouton des interfaces ?
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						rect = this.GetFieldInterfaceBounds();
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldAddInterface;
							return true;
						}
					}

					//	Souris dans un champ ?
					for (int i=0; i<this.fields.Count; i++)
					{
						rect = this.GetFieldRemoveBounds(i);
						if ((!this.editor.Module.IsPatch || this.fields[i].CultureMapSource == CultureMapSource.PatchModule) &&
							this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked && i >= this.skippedField && rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldRemove;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldMovableBounds(i);
						if ((!this.editor.Module.IsPatch || this.fields[i].CultureMapSource == CultureMapSource.PatchModule) &&
							this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked && i >= this.skippedField && rect.Contains(pos) && this.Fields.Count-this.skippedField > 1)
						{
							element = ActiveElement.BoxFieldMovable;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldNameBounds(i);
						if (i >= this.skippedField && rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldName;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldTypeBounds(i);
						if (i >= this.skippedField && rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldType;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}

						rect = this.GetFieldExpressionBounds(i);
						if (this.fields[i].IsEditExpressionEnabled && rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldExpression;
							fieldRank = i;
							this.SetConnectionsHilited(true);
							return true;
						}
					}

					for (int i=0; i<this.fields.Count; i++)
					{
						rect = this.GetFieldGroupBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.BoxFieldGroup;
							fieldRank = i;
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

		public override bool IsMousePossible(ActiveElement element, int fieldRank)
		{
			//	Indique si l'opération est possible.
			//	Dans un module de patch, un champ provenant du module de référence ne doit pas pouvoir
			//	être modifié, mais la détection (BoxFieldName et BoxFieldType) doit tout de même fonctionner,
			//	pour permettre la navigation. Cette méthode indique si l'opération de modification est
			//	possible.
			if (element == ActiveElement.BoxFieldName ||
				element == ActiveElement.BoxFieldType)
			{
				if (this.editor.Module.IsPatch && fieldRank != -1)
				{
					return this.fields[fieldRank].CultureMapSource == CultureMapSource.PatchModule;
				}
			}

			return true;
		}

		protected void SetConnectionsHilited(bool isHilited)
		{
			//	Modifie l'état 'hilited' de toutes les connections qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons fermées ne sont affichés qu'à droite.
			if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				isHilited = false;
			}

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

		protected Rectangle GetFieldInterfaceBounds()
		{
			//	Retourne le rectangle occupé par le bouton interface.
			Rectangle rect = this.GetFieldBounds(this.skippedField-1);
			
			rect.Left = rect.Right-rect.Height;
			rect.Bottom -= 6;
			rect.Height = 6*2;

			return rect;
		}

		protected Rectangle GetFieldNameBounds(int rank)
		{
			//	Retourne le rectangle occupé par le nom d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Right = this.ColumnsSeparatorAbsolute(0);

			return rect;
		}

		protected Rectangle GetFieldTypeBounds(int rank)
		{
			//	Retourne le rectangle occupé par le type d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(ObjectBox.textMargin, 0);
			rect.Left = this.ColumnsSeparatorAbsolute(0)+1;
			rect.Right = this.ColumnsSeparatorAbsolute(1);

			return rect;
		}

		protected Rectangle GetFieldExpressionBounds(int rank)
		{
			//	Retourne le rectangle occupé par l'expression d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(9.5, 0);
			rect.Left = this.ColumnsSeparatorAbsolute(1)+1;

			return rect;
		}

		protected Rectangle GetFieldGroupBounds(int rank)
		{
			//	Retourne le rectangle occupé par un groupe, c'est-à-dire un ensemble de champs IsReadOnly
			//	ayant le même DeepDefiningTypeId.
			if (rank == 0 || !this.fields[rank].IsReadOnly)
			{
				return Rectangle.Empty;
			}

			if (this.IsSameGroup(rank-1, rank))
			{
				return Rectangle.Empty;
			}

			Rectangle rect = this.GetFieldBounds(rank);

			for (int i=rank+1; i<this.fields.Count; i++)
			{
				if (!this.IsSameGroup(i-1, i))
				{
					break;
				}

				rect = Rectangle.Union(rect, this.GetFieldBounds(i));
			}

			rect.Deflate(9.0, 0.0);
			return rect;
		}

		protected bool IsSameGroup(int i, int j)
		{
			return !this.fields[i].IsTitle && !this.fields[j].IsTitle &&
					this.fields[i].IsInherited == this.fields[j].IsInherited &&
					this.fields[i].IsInterfaceOrInterfaceTitle == this.fields[j].IsInterfaceOrInterfaceTitle &&
					this.fields[i].DefiningRootEntityId == this.fields[j].DefiningRootEntityId;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ, en tenant compte du niveau.
			Rectangle rect = this.bounds;
			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - AbstractObject.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			if (rank >= 0 && rank < this.fields.Count)  // rang d'un champ existant ?
			{
				rect.Deflate(ObjectBox.indentWidth*this.fields[rank].Level, 0);  // plus étroit si Level > 0
			}

			return rect;
		}


		protected bool IsInterface
		{
			//	Indique si l'entité est une interface.
			get
			{
				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				StructuredTypeClass typeClass = (StructuredTypeClass) data.GetValue(Support.Res.Fields.ResourceStructuredType.Class);
				return typeClass == StructuredTypeClass.Interface;
			}
		}

		protected void LocateEntity()
		{
			//	Montre l'entité cliquée avec le bouton de droite.
			Module module = this.SearchModule(this.cultureMap.Id);

			if (module != null)
			{
				this.Application.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Entities, -1, this.cultureMap.Id, null);
			}
		}

		protected void LocateTitle(int rank)
		{
			//	Montre l'entité héritée ou l'interface cliquée avec le bouton de droite.
			System.Diagnostics.Debug.Assert(this.fields[rank].IsTitle || this.fields[rank].IsSubtitle);
			Druid druid = this.fields[rank].CaptionId;
			System.Diagnostics.Debug.Assert(druid.IsValid);

			Module module = this.SearchModule(druid);

			if (module != null)
			{
				this.Application.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Entities, -1, druid, null);
			}
		}

		protected void LocateField(int rank)
		{
			//	Montre le champ cliqué avec le bouton de droite.
			int fieldRank = this.fields[rank].Rank;
			if (fieldRank == -1)
			{
				return;
			}

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[fieldRank];
			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);

			Module module = this.SearchModule(fieldCaptionId);

			if (module != null)
			{
				this.Application.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Fields, -1, fieldCaptionId, null);
			}
		}

		protected void LocateType(int rank)
		{
			//	Montre le type cliqué avec le bouton de droite.
			int fieldRank = this.fields[rank].Rank;
			if (fieldRank == -1)
			{
				return;
			}

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[fieldRank];
			Druid typeId = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);

			Module module = this.SearchModule(typeId);

			if (module != null)
			{
				ResourceAccess.Type access = (rel == FieldRelation.None) ? ResourceAccess.Type.Types : ResourceAccess.Type.Entities;
				this.Application.LocatorGoto(module.ModuleId.Name, access, -1, typeId, null);
			}
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

				int fieldSrcRank = this.fields[srcRank].Rank;
				int fieldDstRank = this.fields[dstRank].Rank;

				StructuredData movingfield = dataFields[fieldSrcRank];
				dataFields.RemoveAt(fieldSrcRank);
				dataFields.Insert(fieldDstRank, movingfield);

				Field movingFld = this.fields[srcRank];
				this.fields.RemoveAt(srcRank);
				this.fields.Insert(dstRank, movingFld);

				this.UpdateFieldsLink();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.editor.Module.AccessEntities.SetLocalDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void RemoveField(int rank)
		{
			//	Supprime un champ.
			string question = string.Format(Res.Strings.Entities.Question.RemoveField.Base, this.fields[rank].FieldName);
			if (this.Application.DialogQuestion(question) == Epsitec.Common.Dialogs.DialogResult.Yes)
			{
				Druid fieldId = this.fields[rank].CaptionId;

				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(null);

				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;
				int fieldRank = this.fields[rank].Rank;
				dataFields.RemoveAt(fieldRank);

				this.fields.RemoveAt(rank);

				CultureMap fieldCaptionItem = this.editor.Module.AccessFields.Accessor.Collection[fieldId];
				this.editor.Module.AccessFields.Accessor.Collection.Remove(fieldCaptionItem);
				this.editor.Module.AccessFields.SetLocalDirty();

				this.UpdateFieldsLink();
				this.editor.Entities.UpdateReset();
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.editor.Module.AccessEntities.SetLocalDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		protected void AddField(int rank)
		{
			//	Ajoute un nouveau champ.
			Module module = this.editor.Module;
			string fieldName = this.GetNewName();
			Druid druid = Druid.Empty;
			bool isNullable = false;
			bool isPrivate = false;
			bool isCollection = false;

			Common.Dialogs.DialogResult result = module.DesignerApplication.DlgEntityField(module, ResourceAccess.Type.Types, this.Title, ref fieldName, ref druid, ref isNullable, ref isCollection, ref isPrivate);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Support.ResourceAccessors.StructuredTypeResourceAccessor;
			CultureMap fieldCultureMap = accessor.CreateFieldItem(this.cultureMap);
			fieldCultureMap.Name = fieldName;

			IResourceAccessor fieldAccessor = this.editor.Module.AccessFields.Accessor;
			fieldAccessor.Collection.Add(fieldCultureMap);

			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.Fields);
			StructuredData newField = broker.CreateData(this.cultureMap);

			Druid newDruid = fieldCultureMap.Id;
			newField.SetValue(Support.Res.Fields.Field.CaptionId, newDruid);

			int fieldRank = (rank == -1) ? -1 : this.fields[rank].Rank;
			dataFields.Insert(fieldRank+1, newField);

			Field field = new Field(this.editor);
			field.Initialize(this, newField);
			this.fields.Insert(rank+1, field);

			StructuredData dataField = dataFields[fieldRank+1];

			dataField.SetValue(Support.Res.Fields.Field.TypeId, druid);

			Druid typeId = druid;
			Module typeModule = this.SearchModule(typeId);
			System.Diagnostics.Debug.Assert(typeId.IsValid);
			System.Diagnostics.Debug.Assert(typeModule != null);

			if (typeModule.AccessEntities.Accessor.Collection[typeId] != null)
			{
				if (isCollection)
				{
					dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Collection);
				}
				else
				{
					dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Reference);
				}
			}
			else
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.None);
			}

			FieldOptions fieldOptions = (FieldOptions) dataField.GetValue(Support.Res.Fields.Field.Options);

			if (isNullable)
			{
				fieldOptions |= FieldOptions.Nullable;
			}
			else
			{
				fieldOptions &= ~FieldOptions.Nullable;
			}

			if (isPrivate)
			{
				fieldOptions |= FieldOptions.PrivateRelation;
			}
			else
			{
				fieldOptions &= ~FieldOptions.PrivateRelation;
			}

			dataField.SetValue(Support.Res.Fields.Field.Options, fieldOptions);

			this.fields[rank+1].Initialize(this, dataField);

			this.UpdateFieldsLink();
			this.editor.Entities.UpdateReset();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
			this.editor.Module.AccessEntities.SetLocalDirty();
			this.hilitedElement = ActiveElement.None;
		}

		protected void AddInterface()
		{
			//	Ajoute une interface à l'entité.
			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataInterfaces = data.GetValue(Support.Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

			List<Druid> exclude = new List<Druid>();
			foreach (StructuredData dataInterface in dataInterfaces)
			{
				Druid di = (Druid) dataInterface.GetValue(Support.Res.Fields.InterfaceId.CaptionId);
				exclude.Add(di);
			}

			Druid druid = Druid.Empty;
			bool isNullable = false;
			Module module = this.editor.Module;
			StructuredTypeClass typeClass = StructuredTypeClass.Interface;
			Common.Dialogs.DialogResult result = module.DesignerApplication.DlgResourceSelector(Dialogs.ResourceSelector.Operation.InterfaceEntities, module, ResourceAccess.Type.Entities, ref typeClass, ref druid, ref isNullable, exclude, Druid.Empty);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Support.ResourceAccessors.StructuredTypeResourceAccessor;
			IDataBroker broker = accessor.GetDataBroker(data, Support.Res.Fields.ResourceStructuredType.InterfaceIds);
			StructuredData newInterface = broker.CreateData(this.cultureMap);
			newInterface.SetValue(Support.Res.Fields.InterfaceId.CaptionId, druid);
			dataInterfaces.Add(newInterface);

			this.SetContent(this.cultureMap);
			this.editor.UpdateAfterAddOrRemoveConnection(this);
			this.editor.Module.AccessEntities.SetLocalDirty();
			this.hilitedElement = ActiveElement.None;
		}

		protected void RemoveInterface(int rank)
		{
			//	Supprime une interface de l'entité.
			string question = string.Format(Res.Strings.Entities.Question.RemoveInterface.Base, this.fields[rank].FieldName);
			if (this.Application.DialogQuestion(question) == Epsitec.Common.Dialogs.DialogResult.Yes)
			{
				int count = this.GroupLineCount(rank);
				for (int i=rank+1; i<rank+1+count; i++)
				{
					this.fields[i].IsExplored = false;
					this.fields[i].DstBox = null;
				}
				this.editor.CloseBox(null);

				Druid druid = this.fields[rank].CaptionId;

				StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				IList<StructuredData> dataInterfaces = data.GetValue(Support.Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

				for (int i=0; i<dataInterfaces.Count; i++)
				{
					Druid di = (Druid) dataInterfaces[i].GetValue(Support.Res.Fields.InterfaceId.CaptionId);
					if (di == druid)
					{
						dataInterfaces.RemoveAt(i);
						break;
					}
				}
				
				Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Support.ResourceAccessors.StructuredTypeResourceAccessor;
				accessor.RefreshFields(this.cultureMap);

				this.SetContent(this.cultureMap);
				this.editor.UpdateAfterAddOrRemoveConnection(this);
				this.editor.Module.AccessEntities.SetLocalDirty();
			}

			this.hilitedElement = ActiveElement.None;
		}

		public bool EditExpression(Druid fieldId)
		{
			//	Edite l'expresion associée à un champ.
			Field field = this.AdjustAfterReadSearchField(fieldId);
			if (field == null)
			{
				return false;
			}

			this.EditExpression(field.Index);
			return true;
		}

		protected void EditExpression(int rank)
		{
			//	Edite l'expresion associée à un champ.

			if (rank < 0 || rank >= this.fields.Count)
			{
				return;
			}

			Field field = this.fields[rank];
			int fieldRank = field.Rank;

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[fieldRank];
			string localSource = TextLayout.ConvertToTaggedText(field.LocalExpression);
			string inheritedSource = TextLayout.ConvertToTaggedText(field.InheritedExpression);
			FieldMembership membership = field.Membership;
			bool isInterface = field.IsInterfaceLocal;
			bool isOverridable = isInterface || membership != FieldMembership.Local;
			bool isUnchanged = field.IsUnchangedInterfaceField;
			bool isPatchModule = field.IsPatch && field.CultureMapSource != CultureMapSource.PatchModule;

			if (isUnchanged)
			{
				//	Le champ reprend de manière transparente ce qui est défini au
				//	niveau de l'interface. Le source local est donc à ignorer ici.
				localSource = null;
			}
			
			//	Edition de l'expression.
			if (!this.Application.DlgEntityExpression(isOverridable, isPatchModule, inheritedSource, ref localSource))
			{
				return;
			}

			string encoded;
			if (localSource == null)
			{
				encoded = null;
			}
			else
			{
				localSource = TextLayout.ConvertToSimpleText(localSource);
				EntityExpression expression = EntityExpression.FromSourceCode(EntityExpressionEncoding.LambdaCSharpSourceCode, localSource);
				encoded = expression.GetEncodedExpression();
				localSource = localSource.Trim();

				if (localSource.Length == 0)
				{
					//	L'utilisateur désire utiliser une valeur plutôt qu'une expression
					//	pour ce champ. Vérifie que ce soit autorisé dans ce contexte : il
					//	faut que ce soit un champ local, sinon c'est illégal de changer
					//	d'expression à valeur.
					if (membership != FieldMembership.Local ||
						(field.IsPatch && !string.IsNullOrEmpty(inheritedSource)))
					{
						this.Application.DialogError(Res.Strings.Error.Entities.ReplaceExpression);
						return;
					}
				}
			}

			if (field.IsPatch && field.CultureMapSource != CultureMapSource.PatchModule)
			{
				if (encoded == null)
				{
					//	L'utilisateur a décidé de supprimer le réglage local au module
					//	de patch pour reprendre les réglages du module de référence. Il
					//	suffit pour cela de reprendre les valeurs originales.
					dataField.ResetToOriginalValue (Support.Res.Fields.Field.IsInterfaceDefinition);
					dataField.ResetToOriginalValue (Support.Res.Fields.Field.Expression);
					dataField.ResetToOriginalValue (Support.Res.Fields.Field.Source);

					//	Il ne faut pas oublier de réinitialiser la source du champ de
					//	manière à ce que l'accesseur considère que le champ provient
					//	du module de référence et n'est plus le résultat d'une fusion:
					dataField.SetValue (Support.Res.Fields.Field.CultureMapSource, CultureMapSource.ReferenceModule);

					goto end;
				}
			}
			
			if (isInterface)
			{
				//	Le champ provient d'une interface locale...
				if (encoded == null)
				{
					//	La définition doit être reprise dans l'interface; il n'y a plus de définition
					//	locale.
					dataField.SetValue(Support.Res.Fields.Field.IsInterfaceDefinition, true);
				}
				else
				{
					//	La définition ne doit plus être reprise dans l'interface, mais bien dans les
					//	réglages locaux.
					dataField.SetValue(Support.Res.Fields.Field.IsInterfaceDefinition, false);

					if (localSource.Length == 0)
					{
						//	L'utilisateur n'a pas spécifié de source, ce qui implique que ce n'est
						//	pas une expression, mais un valeur qu'il faut utiliser ici.
						dataField.SetValue(Support.Res.Fields.Field.Expression, UndefinedValue.Value);
						dataField.SetValue(Support.Res.Fields.Field.Source, FieldSource.Value);
					}
					else
					{
						//	L'utilisateur a spécifié une expression qui remplace celle qui était
						//	définie au niveau de l'interface.
						dataField.SetValue(Support.Res.Fields.Field.Expression, encoded);
						dataField.SetValue(Support.Res.Fields.Field.Source, FieldSource.Expression);
					}
				}
			}
			else
			{
				if (encoded == null)
				{
					if (membership == FieldMembership.LocalOverride)
					{
						//	Ce champ redéfinissait une expression locale; supprime cette
						//	définition locale et reprend la valeur héritée.
						dataField.SetValue(Support.Res.Fields.Field.IsInterfaceDefinition, UndefinedValue.Value);
						dataField.SetValue(Support.Res.Fields.Field.Membership, FieldMembership.Inherited);
					}
				}
				else
				{
					if (membership != FieldMembership.Local)
					{
						//	Ce champ n'est pas un champ local, mais un champ hérité ou qui
						//	redéfinissait déjà l'expression héritée; dans tous les cas, note
						//	que maintenant, le champ redéfinit localement l'expression héritée.
						dataField.SetValue(Support.Res.Fields.Field.IsInterfaceDefinition, false);
						dataField.SetValue(Support.Res.Fields.Field.Membership, FieldMembership.LocalOverride);
					}

					if (string.IsNullOrEmpty(localSource))
					{
						//	Aucune expression n'a été spécifiée; force l'utilisation d'une
						//	valeur.
						dataField.SetValue(Support.Res.Fields.Field.Expression, UndefinedValue.Value);
						dataField.SetValue(Support.Res.Fields.Field.Source, FieldSource.Value);
					}
					else
					{
						//	Une expression a été spécifiée; force l'utilisation de celle-ci
						//	localement.
						dataField.SetValue(Support.Res.Fields.Field.Expression, encoded);
						dataField.SetValue(Support.Res.Fields.Field.Source, FieldSource.Expression);
					}
				}
			}

			//	Termine en mettant à jour les champs hérités...

		end:

			Support.ResourceAccessors.StructuredTypeResourceAccessor accessor = this.editor.Module.AccessEntities.Accessor as Support.ResourceAccessors.StructuredTypeResourceAccessor;
			accessor.RefreshFields(this.cultureMap);

			this.SetContent(this.cultureMap);
			this.editor.Module.AccessEntities.SetLocalDirty();
		}

		protected void ChangeFieldType(int rank)
		{
			//	Choix du type pour un champ.
			int fieldRank = this.fields[rank].Rank;
			if (fieldRank == -1)
			{
				return;
			}

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
			IList<StructuredData> dataFields = data.GetValue(Support.Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

			StructuredData dataField = dataFields[fieldRank];
			Druid druid = (Druid) dataField.GetValue(Support.Res.Fields.Field.TypeId);
			Druid initialDruid = druid;
			FieldOptions options = (FieldOptions) dataField.GetValue(Support.Res.Fields.Field.Options);
			FieldRelation rel = (FieldRelation) dataField.GetValue(Support.Res.Fields.Field.Relation);
	
			Druid fieldCaptionId = (Druid) dataField.GetValue(Support.Res.Fields.Field.CaptionId);
			IResourceAccessor fieldAccessor = this.editor.Module.AccessFields.Accessor;
			CultureMap fieldCultureMap = fieldAccessor.Collection[fieldCaptionId];
			System.Diagnostics.Debug.Assert(fieldCultureMap != null);
			string fieldName = fieldCultureMap.Name;
			
			bool isNullable = (options & FieldOptions.Nullable) != 0;
			bool isPrivate = (options & FieldOptions.PrivateRelation) != 0;
			bool isCollection = (rel == FieldRelation.Collection);
			Module module = this.editor.Module;

			Common.Dialogs.DialogResult result = module.DesignerApplication.DlgEntityField(module, ResourceAccess.Type.Types, fieldCultureMap.Prefix, ref fieldName, ref druid, ref isNullable, ref isCollection, ref isPrivate);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			fieldCultureMap.Name = fieldName;

			if (this.fields[rank].Relation != FieldRelation.None && this.fields[rank].IsExplored && druid != initialDruid)
			{
				ObjectBox dst = this.fields[rank].DstBox;
				this.fields[rank].IsExplored = false;
				this.fields[rank].DstBox = null;
				this.editor.CloseBox(dst);  // ferme les boîtes pointées
			}

			dataField.SetValue(Support.Res.Fields.Field.TypeId, druid);

			Druid typeId = druid;
			Module typeModule = this.SearchModule(typeId);
			System.Diagnostics.Debug.Assert(typeId.IsValid);
			System.Diagnostics.Debug.Assert(typeModule != null);
			
			if (typeModule.AccessEntities.Accessor.Collection[typeId] != null)
			{
				if (isCollection)
				{
					dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Collection);
				}
				else
				{
					dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.Reference);
				}
			}
			else
			{
				dataField.SetValue(Support.Res.Fields.Field.Relation, FieldRelation.None);
			}

			FieldOptions fieldOptions = (FieldOptions) dataField.GetValue(Support.Res.Fields.Field.Options);
			
			if (isNullable)
			{
				fieldOptions |= FieldOptions.Nullable;
			}
			else
			{
				fieldOptions &= ~FieldOptions.Nullable;
			}
			
			if (isPrivate)
			{
				fieldOptions |= FieldOptions.PrivateRelation;
			}
			else
			{
				fieldOptions &= ~FieldOptions.PrivateRelation;
			}

			dataField.SetValue(Support.Res.Fields.Field.Options, fieldOptions);

			this.fields[rank].Initialize(this, dataField);
			this.editor.Module.AccessEntities.SetLocalDirty();
			this.editor.UpdateAfterAddOrRemoveConnection(this);
		}

		protected void UpdateFieldsContent()
		{
			//	Crée tous les champs de titrage.
			this.skippedField = 0;
			for (int i=0; i<this.fields.Count; i++)
			{
				if (this.fields[i].IsReadOnly)
				{
					this.skippedField++;  // nombre de champs hérités ou provenant d'une interface, au début de la liste
				}
			}

			StructuredData data = this.cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);

			//	Ajoute le titre de l'entité dont on hérite, s'il existe.
			Druid title = (Druid) data.GetValue(Support.Res.Fields.ResourceStructuredType.BaseType);
			if (this.skippedField > 0 && title.IsValid)
			{
				Field field = new Field(this.editor);
				field.IsTitle = true;
				field.IsInherited = true;
				field.CaptionId = title;

				Module module = this.SearchModule(title);
				CultureMap cultureMap = module.AccessEntities.Accessor.Collection[title];
				if (cultureMap != null)
				{
					field.FieldName = Misc.Bold(cultureMap.Name);
				}

				this.fields.Insert(0, field);
				this.skippedField++;  // compte le titre lui-même
			}

			//	Ajoute les titres des interfaces, si elles existent.
			Druid last = Druid.Empty;
			for (int i=0; i<this.fields.Count; i++)
			{
				if (this.fields[i].IsTitle)
				{
					continue;
				}

				if (this.fields[i].DefiningRootEntityId.IsValid)  // champ d'une interface ?
				{
					if (last != this.fields[i].DefiningRootEntityId && this.fields[i].DefiningRootEntityId != title)
					{
						last = this.fields[i].DefiningRootEntityId;

						Module module = this.SearchModule(last);
						CultureMap entity = module.AccessEntities.Accessor.Collection[last];

						StructuredData entityData = entity.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
						StructuredTypeClass entityTypeClass = (StructuredTypeClass) entityData.GetValue(Support.Res.Fields.ResourceStructuredType.Class);

						if (entityTypeClass == StructuredTypeClass.Interface)
						{
							Field field = new Field(this.editor);
							if (this.fields[i].Membership == FieldMembership.Local)
							{
								field.IsTitle = true;  // interface ajoutée à cette entitié
								field.IsInterfaceOrInterfaceTitle = true;
							}
							else
							{
								field.IsSubtitle = true;  // interface héritée
								field.IsInterfaceOrInterfaceTitle = true;
							}
							field.CaptionId = last;
							field.FieldName = Misc.Bold(entity.Name);

							this.fields.Insert(i, field);
							this.skippedField++;  // compte le titre lui-même
							i++;
						}
					}
				}
			}

			//	Pour chaque case, initialise les propriétés Level, IsGroupTop et IsGroupBottom.
			for (int i=0; i<this.fields.Count; i++)
			{
				if (this.fields[i].IsTitle)
				{
					int j = i + this.GroupLineCount(i);

					this.fields[i].IsGroupTop = true;
					this.fields[j].IsGroupBottom = true;
				}

				if (this.fields[i].IsSubtitle)
				{
					int j = i + this.SubgroupLineCount(i);

					this.fields[i].IsGroupTop = true;
					this.fields[j].IsGroupBottom = true;

					for (int k=i; k<=j; k++)
					{
						this.fields[k].Level++;
					}
				}
			}

			IList<StructuredData> dataInterfaces = data.GetValue(Support.Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
			for (int i=0; i<dataInterfaces.Count; i++)
			{
				Druid di = (Druid) dataInterfaces[i].GetValue(Support.Res.Fields.InterfaceId.CaptionId);
				Field field = this.SearchDruid(di);
				if (field != null)
				{
					field.CultureMapSource = (CultureMapSource) dataInterfaces[i].GetValue(Support.Res.Fields.InterfaceId.CultureMapSource);
				}
			}

			this.UpdateInformations();
			this.UpdateFieldsLink();
		}

		protected void UpdateInformations()
		{
			//	Met à jour les informations de l'éventuel ObjectInfo lié.
			if (this.info != null)  // existe un ObjectInfo lié ?
			{
				this.info.Text = this.GetInformations(false);
				this.editor.UpdateAfterCommentChanged();
			}
		}

		protected string GetInformations(bool resume)
		{
			//	Retourne les informations pour l'ObjectInfo lié.
			List<string> listInherited = new List<string>();
			List<string> listInterface = new List<string>();

			//	Cherche récursivement toutes les entités parentes, pour l'héritage.
			CultureMap cultureMap = this.cultureMap;
			while (cultureMap != null)
			{
				StructuredData data = cultureMap.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				cultureMap = null;
				Druid druid = (Druid) data.GetValue(Support.Res.Fields.ResourceStructuredType.BaseType);
				if (druid.IsValid)
				{
					Module module = this.SearchModule(druid);
					cultureMap = module.AccessEntities.Accessor.Collection[druid];
					if (cultureMap != null)
					{
						string name;

						if (module == this.editor.Module)  // dans le même module ?
						{
							name = cultureMap.Name;
						}
						else  // dans un autre module ?
						{
							name = string.Concat(module.ModuleId.Name, ".", cultureMap.Name);
						}

						if (!listInherited.Contains(name))
						{
							listInherited.Add(name);
						}
					}
				}
			}

			//	Cherche toutes les interfaces utilisées par les champs.
			for (int i=0; i<this.fields.Count; i++)
			{
#if false
				if (this.fields[i].DefiningType == StructuredTypeClass.Entity)
				{
					if (!listInherited.Contains(this.fields[i].DefiningName))
					{
						listInherited.Add(this.fields[i].DefiningName);
					}
				}
#endif

				if (this.fields[i].DefiningEntityClass == StructuredTypeClass.Interface)
				{
					if (!listInterface.Contains(this.fields[i].DefiningEntityName))
					{
						listInterface.Add(this.fields[i].DefiningEntityName);
					}
				}
			}

			listInherited.Sort();
			listInterface.Sort();

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if (listInherited.Count == 0)
			{
				builder.Append(Misc.Italic(Res.Strings.Entities.Info.InheritNull));
			}
			else
			{
				if (listInherited.Count == 1)
				{
					builder.Append(Res.Strings.Entities.Info.InheritFromOne);
				}
				else
				{
					builder.Append(Res.Strings.Entities.Info.InheritFromMany);
				}

				for (int i=0; i<listInherited.Count; i++)
				{
					if (i != 0 && i == listInherited.Count-1)
					{
						builder.Append(Res.Strings.Entities.Info.InheritAnd);
					}
					else if (i > 0)
					{
						builder.Append(", ");
					}

					builder.Append(listInherited[i]);
				}
			}

			builder.Append("<br/>");

			if (!resume && (listInherited.Count != 0 || listInterface.Count != 0))
			{
				builder.Append("----------<br/>");
			}

			if (listInterface.Count == 0)
			{
				builder.Append(Misc.Italic(Res.Strings.Entities.Info.InterfaceNull));
			}
			else
			{
				if (listInterface.Count == 1)
				{
					builder.Append(Res.Strings.Entities.Info.InterfaceFromOne);
				}
				else
				{
					builder.Append(Res.Strings.Entities.Info.InterfaceFromMany);
				}

				for (int i=0; i<listInterface.Count; i++)
				{
					if (i != 0 && i == listInterface.Count-1)
					{
						builder.Append(Res.Strings.Entities.Info.InterfaceAnd);
					}
					else if (i > 0)
					{
						builder.Append(", ");
					}

					builder.Append(listInterface[i]);
				}
			}

			return builder.ToString();
		}

		protected void UpdateFieldsLink()
		{
			//	Met à jour toutes les liaisons des champs.
			int rank = 0;
			for (int i=0; i<this.fields.Count; i++)
			{
				this.fields[i].Index = i;

				if (!this.fields[i].IsTitle && !this.fields[i].IsSubtitle)
				{
					this.fields[i].IsSourceExpanded = this.isExtended;
					this.fields[i].Rank = rank++;
				}
			}
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


		protected void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment(this.editor);
				this.comment.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Left = rect.Right+30;
				rect.Width = System.Math.Max(this.bounds.Width, AbstractObject.infoMinWidth);
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

			this.editor.Module.AccessEntities.SetLocalDirty();
		}

		protected void AddInfo()
		{
			//	Ajoute une information à la boîte.
			if (this.info == null)
			{
				this.info = new ObjectInfo(this.editor);
				this.info.AttachObject = this;
				this.info.BackgroundMainColor = this.BackgroundMainColor;

				Rectangle rect = this.bounds;
				rect.Width = System.Math.Max(rect.Width, AbstractObject.commentMinWidth);
				rect.Bottom = rect.Top+20;
				rect.Height = 50;  // hauteur arbitraire
				this.info.SetBounds(rect);
				this.info.UpdateHeight();  // adapte la hauteur en fonction du contenu

				this.UpdateInformations();

				this.editor.AddInfo(this.info);
				this.editor.UpdateAfterCommentChanged();
			}
			else
			{
				this.info.IsVisible = !this.info.IsVisible;
			}

			this.editor.Module.AccessEntities.SetLocalDirty();
		}

		protected void ChangeParameters()
		{
			//	Ouvre le dialogue pour modifier les paramètres de l'entité.
			var dialog = this.editor.Module.DesignerApplication.GetDlgEntityParameters ();

			dialog.DataLifetimeExpectancy = DataLifetimeExpectancy.Unknown;
			dialog.StructuredTypeFlags = StructuredTypeFlags.None;

			dialog.Show ();

			if (dialog.IsEditOk)
			{
				// TODO:
			}
		}

		private Module SearchModule(Druid id)
		{
			return this.Application.SearchModule(id);
		}

		private Field SearchDruid(Druid druid)
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

			List<Module> modules = this.Application.Modules;
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
							Module dstModule = this.SearchModule(typeId);
							CultureMap fieldCultureMap = (dstModule == null) ? null : dstModule.AccessEntities.Accessor.Collection[typeId];
							
							if (fieldCultureMap == this.cultureMap && !this.IsExistingSourceInfo(cultureMap))
							{
								SourceInfo info = new SourceInfo();
								
								info.CultureMap = cultureMap;
								info.ModuleName = module.ModuleId.Name;
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
			this.editor.Module.AccessEntities.SetLocalDirty();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
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
			graphics.RenderSolid(this.GetColor(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = this.GetColorMain(dragging ? 0.8 : 0.4);
			Color c2 = this.GetColorMain(dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = this.GetColor(0.9);
			if (dragging)
			{
				colorLine = this.GetColorMain(0.3);
			}

			Color colorFrame = dragging ? this.GetColorMain() : this.GetColor(0);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+AbstractObject.footerHeight, this.bounds.Width-2, this.bounds.Height-AbstractObject.footerHeight-AbstractObject.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(this.GetColor(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = this.GetColorMain(dragging ? 0.2 : 0.1);
				Color ci2 = this.GetColorMain(0.0);
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				//	Trait vertical de séparation.
				if (this.columnsSeparatorRelative1 < 1.0)
				{
					double posx = this.ColumnsSeparatorAbsolute(0)+0.5;
					graphics.AddLine(posx, this.bounds.Bottom+AbstractObject.footerHeight+0.5, posx, this.bounds.Top-AbstractObject.headerHeight-0.5);
					graphics.RenderSolid(colorLine);
				}

				{
					double posx = this.ColumnsSeparatorAbsolute(1)+0.5;
					graphics.AddLine(posx, this.bounds.Bottom+AbstractObject.footerHeight+0.5, posx, this.bounds.Top-AbstractObject.headerHeight-0.5);
					graphics.RenderSolid(colorLine);
				}

				//	Ombre supérieure.
				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-AbstractObject.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			Color titleColor = dragging ? this.GetColor(1) : this.GetColor(0);

			if (string.IsNullOrEmpty(this.subtitleString))
			{
				rect = new Rectangle(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight, this.bounds.Width, AbstractObject.headerHeight);
				rect.Deflate(4, 2);
				this.title.LayoutSize = rect.Size;
				this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);
			}
			else
			{
				rect = new Rectangle(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight+10, this.bounds.Width, AbstractObject.headerHeight-10);
				rect.Deflate(4, 0);
				this.title.LayoutSize = rect.Size;
				this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);
				
				rect = new Rectangle(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight+4, this.bounds.Width, 10);
				rect.Deflate(4, 0);
				this.subtitle.LayoutSize = rect.Size;
				this.subtitle.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);
			}

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
			if (this.sourcesClosedCount > 0 && this.hilitedElement != ActiveElement.None)
			{
				Point p1 = this.PositionSourcesButton;
				p1.Y = this.bounds.Top-AbstractObject.headerHeight;
				Point p2 = p1;
				p1.X = this.bounds.Left-1-AbstractObject.lengthClose;
				p2.X = this.bounds.Left-1;
				graphics.LineWidth = 2;
				graphics.AddLine(p1, p2);
				AbstractObject.DrawEndingArrow(graphics, p1, p2, FieldRelation.Reference, false);
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
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxComment, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxComment, false, false);
			}

			//	Dessine le bouton des informations.
			if (this.hilitedElement == ActiveElement.BoxInfo)
			{
				this.DrawRoundButton (graphics, this.PositionInfoButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxInfo, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionInfoButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxInfo, false, false);
			}

			//	Dessine le bouton des paramètres.
			if (this.hilitedElement == ActiveElement.BoxParameters)
			{
				this.DrawRoundButton (graphics, this.PositionParametersButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxParameters, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton (graphics, this.PositionParametersButton, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxParameters, false, false);
			}

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				//	Dessine le glyph 'o--' pour les interfaces.
				if (this.IsInterface)
				{
					rect = new Rectangle(this.bounds.Left-25, this.bounds.Top-AbstractObject.headerHeight+10, 25, 8);
					this.DrawGlyphInterface(graphics, rect, 2, colorFrame);
				}

				//	Dessine toutes les lignes, titres ou simples champs.
				for (int i=0; i<this.fields.Count; i++)
				{
					if (this.editor.Module.IsPatch)
					{
						Color sourceColor = Misc.SourceColor(this.fields[i].CultureMapSource);
						if (!sourceColor.IsEmpty)
						{
							rect = this.GetFieldBounds(i);
							rect.Deflate(9.0, 0.0);
							rect.Bottom += 1.0;
							Path roundedPath = this.PathRoundRectangle(rect, ObjectBox.roundInsideRadius, this.fields[i].IsGroupTop, this.fields[i].IsGroupBottom);
							graphics.Rasterizer.AddSurface(roundedPath);
							graphics.RenderSolid(sourceColor);
						}
					}

					if (this.fields[i].IsTitle || this.fields[i].IsSubtitle)
					{
						bool hilite = this.hilitedElement == ActiveElement.BoxFieldTitle && this.hilitedFieldRank == i;
						rect = this.GetFieldBounds(i);
						rect.Deflate(9.5, 0.5);
						if (this.fields[i].IsSubtitle)
						{
							rect.Top -= 1.0;
						}
						Path roundedPath = this.PathRoundRectangle(rect, ObjectBox.roundInsideRadius, true, false);
						graphics.Rasterizer.AddSurface(roundedPath);
						Color ci1 = this.GetColorMain(hilite ? 0.5 : (dragging ? 0.2 : 0.1));
						Color ci2 = this.GetColorMain(hilite ? 0.3 : (dragging ? 0.1 : 0.0));
						this.RenderVerticalGradient(graphics, rect, ci1, ci2);

						rect = this.GetFieldBounds(i);
						rect.Deflate(ObjectBox.textMargin, 2);
						this.fields[i].TextLayoutField.LayoutSize = rect.Size;
						this.fields[i].TextLayoutField.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, hilite ? this.GetColor(1) : this.GetColorMain(0.8), GlyphPaintStyle.Normal);

						rect = this.GetFieldBounds(i);
						graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
						graphics.RenderSolid(colorLine);
					}
					else
					{
						Color colorName = this.GetColor(0);
						Color colorType = this.GetColor(0);
						Color colorExpr = this.GetColor(0);

						if (this.hilitedElement == ActiveElement.BoxFieldName && this.hilitedFieldRank == i)
						{
							rect = this.GetFieldNameBounds(i);

							graphics.AddFilledRectangle(rect);
							graphics.RenderSolid(this.GetColorMain());

							colorName = this.GetColor(1);
						}

						if (this.hilitedElement == ActiveElement.BoxFieldType && this.hilitedFieldRank == i)
						{
							rect = this.GetFieldTypeBounds(i);

							graphics.AddFilledRectangle(rect);
							graphics.RenderSolid(this.GetColorMain());

							colorType = this.GetColor(1);
						}

						if (this.hilitedElement == ActiveElement.BoxFieldExpression && this.hilitedFieldRank == i)
						{
							rect = this.GetFieldExpressionBounds(i);

							graphics.AddFilledRectangle(rect);
							graphics.RenderSolid(this.GetColorMain());

							colorExpr = this.GetColor(1);
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

						//	Affiche l'expression du champ.
						if (!string.IsNullOrEmpty(this.fields[i].LocalExpression))
						{
							rect = this.GetFieldExpressionBounds(i);
							rect.Right -= 2;
							graphics.AddText(rect.Left, rect.Bottom+1, rect.Width, rect.Height, Res.Strings.Entities.Icon.Expression, Font.DefaultFont, 14, ContentAlignment.MiddleCenter);
							graphics.RenderSolid(colorExpr);
						}
						else if (this.fields[i].LocalExpression != "" && !string.IsNullOrEmpty(this.fields[i].InheritedExpression))
						{
							//	L'expression n'est héritée que si elle n'est pas remplacée par
							//	une valeur au niveau du champ, ce qui est encodé ici avec une
							//	expression locale égale à "".
							rect = this.GetFieldExpressionBounds(i);
							rect.Right -= 2;
							graphics.AddText(rect.Left, rect.Bottom+1, rect.Width, rect.Height, Res.Strings.Entities.Icon.DeepExpression, Font.DefaultFont, 14, ContentAlignment.MiddleCenter);
							graphics.RenderSolid(colorExpr);
						}

						rect = this.GetFieldBounds(i);
						graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
						graphics.RenderSolid(colorLine);
					}
				}

				//	Dessine tous les cadres liés aux titres.
				for (int i=0; i<this.fields.Count; i++)
				{
					if (this.fields[i].IsTitle)
					{
						int j = i + this.GroupLineCount(i);

						rect = Rectangle.Union(this.GetFieldBounds(i), this.GetFieldBounds(j));
						rect.Deflate(9.5, 1.5);
						rect.Top += 1.0;
						Path dashedPath = this.PathRoundRectangle(rect, ObjectBox.roundInsideRadius);

						rect = this.GetFieldBounds(i);
						rect.Deflate(9.5, 0.5);

						if (this.fields[i].IsInherited)
						{
							dashedPath.MoveTo(rect.Left+2, rect.Bottom);
							dashedPath.LineTo(rect.Right-1, rect.Bottom);
							Misc.DrawPathDash(graphics, dashedPath, 1, 0, 2, false, this.GetColorMain(0.8));
						}
						else
						{
							dashedPath.MoveTo(rect.Left, rect.Bottom);
							dashedPath.LineTo(rect.Right, rect.Bottom);
							graphics.Rasterizer.AddOutline(dashedPath);
							graphics.RenderSolid(this.GetColorMain(0.8));
						}

						if (this.fields[i].IsInterfaceOrInterfaceTitle)
						{
							rect = this.GetFieldBounds(i);
							rect.Deflate(9.5, 0.5);
							rect = new Rectangle(rect.Left-25, rect.Center.Y-5, 25, 6);
							this.DrawGlyphInterface(graphics, rect, 1, this.GetColorMain(0.8));
						}
					}
					else if (this.fields[i].IsSubtitle)
					{
						int j = i + this.SubgroupLineCount(i);
						double indent = ObjectBox.indentWidth*this.fields[i].Level;

						rect = Rectangle.Union(this.GetFieldBounds(i), this.GetFieldBounds(j));
						rect.Deflate(9.5, 1.5);
						Path dashedPath = this.PathRoundRectangle(rect, ObjectBox.roundInsideRadius);

						rect = this.GetFieldBounds(i);
						rect.Deflate(9.5, 0.5);

						if (this.fields[i].IsInherited)
						{
							dashedPath.MoveTo(rect.Left+2, rect.Bottom);
							dashedPath.LineTo(rect.Right-1, rect.Bottom);
							Misc.DrawPathDash(graphics, dashedPath, 1, 0, 2, false, this.GetColorMain(0.8));
						}
						else
						{
							dashedPath.MoveTo(rect.Left, rect.Bottom);
							dashedPath.LineTo(rect.Right, rect.Bottom);
							graphics.Rasterizer.AddOutline(dashedPath);
							graphics.RenderSolid(this.GetColorMain(0.8));
						}

						if (this.fields[i].IsInterfaceOrInterfaceTitle)
						{
							rect = this.GetFieldBounds(i);
							rect.Deflate(9.5, 0.5);
							rect = new Rectangle(rect.Left-25-indent, rect.Center.Y-5, 25+indent, 6);
							this.DrawGlyphInterface(graphics, rect, 1, this.GetColorMain(0.8));
						}
					}
					else
					{
						if (i < this.fields.Count-1 &&
							this.fields[i].IsInherited == this.fields[i+1].IsInherited &&
							this.fields[i].IsInterfaceOrInterfaceTitle == this.fields[i+1].IsInterfaceOrInterfaceTitle &&
							this.fields[i].Level == this.fields[i+1].Level &&
							this.fields[i].DefiningRootEntityId != this.fields[i+1].DefiningRootEntityId &&
							!this.fields[i+1].IsTitle)
						{
							rect = this.GetFieldBounds(i);
							rect.Deflate(9.5, 0.5);
							Path dashedPath = new Path();

							if (this.fields[i].IsInherited)
							{
								dashedPath.MoveTo(rect.Left+2, rect.Bottom);
								dashedPath.LineTo(rect.Right-1, rect.Bottom);
								Misc.DrawPathDash(graphics, dashedPath, 1, 0, 2, false, this.GetColorMain(0.8));
							}
							else
							{
								dashedPath.MoveTo(rect.Left, rect.Bottom);
								dashedPath.LineTo(rect.Right, rect.Bottom);
								graphics.Rasterizer.AddOutline(dashedPath);
								graphics.RenderSolid(this.GetColorMain(0.8));
							}
						}
					}
				}

				//	Met en évidence le groupe survolé.
				if (this.hilitedElement == ActiveElement.BoxFieldGroup)
				{
					rect = this.GetFieldGroupBounds(this.hilitedFieldRank);
					rect.Deflate(1.5);
					rect.Bottom += 1.0;
					Path roundedPath = this.PathRoundRectangle(rect, ObjectBox.roundInsideRadius, false, true);

					graphics.Rasterizer.AddSurface(roundedPath);
					graphics.RenderSolid(this.GetColorMain(0.1));

					graphics.Rasterizer.AddOutline(roundedPath, 3);
					graphics.RenderSolid(this.GetColorMain(0.5));
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
					this.hilitedElement != ActiveElement.BoxMoveColumnsSeparator1 &&
					this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked &&
					!this.IsHeaderHilite && !this.isFieldMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator1)
				{
					//	Dessine la glissière à gauche pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(this.skippedField-1).Center;
					Point p2 = this.GetFieldAddBounds(this.fields.Count-1).Center;
					bool hilited = this.hilitedElement == ActiveElement.BoxFieldAdd || this.hilitedElement == ActiveElement.BoxFieldRemove;
					this.DrawEmptySlider(graphics, p1, p2, hilited);

					//	Dessine la glissière à droite pour suggérer les boutons Movable des champs.
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					hilited = this.hilitedElement == ActiveElement.BoxFieldMovable;
					this.DrawEmptySlider(graphics, p1, p2, hilited);
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

				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
				{
					if (this.hilitedElement == ActiveElement.BoxFieldAddInterface || this.IsHeaderHilite)
					{
						rect = this.GetFieldInterfaceBounds();
						this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxFieldAddInterface, this.hilitedElement == ActiveElement.BoxFieldAddInterface, true);
					}

					if (this.hilitedElement == ActiveElement.BoxFieldRemoveInterface ||
						(this.hilitedElement == ActiveElement.BoxFieldTitle && this.fields[this.hilitedFieldRank].IsInterfaceOrInterfaceTitle) && (!this.editor.Module.IsPatch || this.fields[this.hilitedFieldRank].CultureMapSource == CultureMapSource.PatchModule))
					{
						rect = this.GetFieldMovableBounds(this.hilitedFieldRank);
						this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxFieldRemoveInterface, this.hilitedElement == ActiveElement.BoxFieldRemoveInterface, true);
					}

#if false
					//	Si la souris est dans la barre de titre, montre les boutons pour les interfaces.
					if (this.IsHeaderHilite)
					{
						for (int i=0; i<this.fields.Count; i++)
						{
							if (this.fields[i].IsTitle &&
								this.fields[i].IsInterface &&
								(!this.editor.Module.IsPatch || this.fields[i].CultureMapSource != CultureMapSource.ReferenceModule))
							{
								rect = this.GetFieldMovableBounds(i);
								this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxFieldRemoveInterface, false, true);
							}
						}

						rect = this.GetFieldInterfaceBounds();
						this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, Res.Strings.Entities.Button.BoxFieldAddInterface, false, true);
					}
#endif
				}
			}

			//	Dessine le bouton des couleurs.
			if (this.hilitedElement == ActiveElement.BoxColor5)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor6)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor3)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor7)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor8)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, false);
			}
			
			if (this.hilitedElement == ActiveElement.BoxColor1)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor2)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, false);
			}

			if (this.hilitedElement == ActiveElement.BoxColor4)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(7), MainColor.Grey, this.boxColor == MainColor.Grey, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(7), MainColor.Grey, this.boxColor == MainColor.Grey, false);
			}

			if (this.isExtended)
			{
				//	Dessine le bouton pour déplacer le séparateur des colonnes.
				if (this.hilitedElement == ActiveElement.BoxMoveColumnsSeparator1)
				{
					double sep = this.ColumnsSeparatorAbsolute(0);
					graphics.LineWidth = 4;
					graphics.AddLine(sep, this.bounds.Bottom+AbstractObject.footerHeight+3, sep, this.bounds.Top-AbstractObject.headerHeight-3);
					graphics.LineWidth = 1;
					graphics.RenderSolid(this.GetColorMain());

					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton(0), AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.BoxHeader && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton(0), AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}

				//	Dessine le bouton pour changer la largeur.
				if (this.hilitedElement == ActiveElement.BoxChangeWidth)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.BoxHeader && !this.isDragging)
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
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.BoxHeader ||
						this.hilitedElement == ActiveElement.BoxSources ||
						this.hilitedElement == ActiveElement.BoxComment ||
						this.hilitedElement == ActiveElement.BoxInfo ||
						this.hilitedElement == ActiveElement.BoxParameters ||
						this.hilitedElement == ActiveElement.BoxColor1 ||
						this.hilitedElement == ActiveElement.BoxColor2 ||
						this.hilitedElement == ActiveElement.BoxColor3 ||
						this.hilitedElement == ActiveElement.BoxColor4 ||
						this.hilitedElement == ActiveElement.BoxColor5 ||
						this.hilitedElement == ActiveElement.BoxColor6 ||
						this.hilitedElement == ActiveElement.BoxColor7 ||
						this.hilitedElement == ActiveElement.BoxColor8 ||
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
			graphics.RenderSolid(this.GetColor(1));
			graphics.Rasterizer.AddSurface(path);
			this.RenderHorizontalGradient(graphics, big, this.GetColorMain(0.6), this.GetColorMain(0.2));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.GetColor(0));

			graphics.AddFilledRectangle(box);
			graphics.RenderSolid(this.GetColor(1));

			//	Dessine l'en-tête.
			Rectangle rect = box;
			rect.Bottom = rect.Top-h-1;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.GetColorMain());

			Rectangle gr = new Rectangle(this.PositionSourcesButton.X-AbstractObject.buttonRadius, this.PositionSourcesButton.Y-AbstractObject.buttonRadius, AbstractObject.buttonRadius*2, AbstractObject.buttonRadius*2);
			adorner.PaintGlyph(graphics, gr, WidgetPaintState.Enabled, this.GetColor(1), GlyphShape.TriangleDown, PaintTextStyle.Button);
			
			graphics.AddText(rect.Left+AbstractObject.buttonRadius*2+5, rect.Bottom+1, rect.Width-(AbstractObject.buttonRadius*2+10), rect.Height, Res.Strings.Entities.Menu.Sources.Title, Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleLeft);
			graphics.RenderSolid(this.GetColor(1));
			
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
					graphics.RenderSolid(this.GetColor(0.9));
				}
				else if (i == this.sourcesMenuSelected)
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.GetColorMain(0.2));
				}

				string text = string.Concat(info.ModuleName, ": ", info.FieldName);
				graphics.AddText(rect.Left+5, rect.Bottom, rect.Width-10, rect.Height, text, Font.DefaultFont, 10, ContentAlignment.MiddleLeft);
				graphics.RenderSolid(this.GetColor(info.Opened ? 0.3 : 0));

				graphics.AddLine(rect.TopLeft, rect.TopRight);
				graphics.RenderSolid(this.GetColor(0));

				rect.Offset(0, -h);
			}

			//	Dessine le cadre du menu.
			graphics.AddRectangle(box);
			graphics.RenderSolid(this.GetColor(0));
		}

		protected void DrawGlyphInterface(Graphics graphics, Rectangle rect, double lineWidth, Color color)
		{
			//	Dessine le glyph 'o--' d'une interface.
			double y = System.Math.Floor(rect.Center.Y)+(lineWidth%2)/2;
			double radius = rect.Height/2;

			graphics.LineWidth = lineWidth;

			graphics.AddFilledCircle(rect.Left+radius, y, radius);
			graphics.RenderSolid(this.GetColor(1));

			graphics.AddCircle(rect.Left+radius, y, radius);
			graphics.AddLine(rect.Left+radius*2, y, rect.Right, y);
			graphics.RenderSolid(color);

			graphics.LineWidth = 1;
		}

		protected void DrawDashLine(Graphics graphics, Point p1, Point p2, Color color)
		{
			//	Dessine un large traitillé.
			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			Misc.DrawPathDash(graphics, path, 3, 5, 5, false, color);
		}

		protected void DrawEmptySlider(Graphics graphics, Point p1, Point p2, bool hilited)
		{
			//	Dessine une glissère vide, pour suggérer les boutons qui peuvent y prendre place.
			Rectangle rect = new Rectangle(p1, p2);
			rect.Inflate(2.5+6);
			this.DrawShadow(graphics, rect, rect.Width/2, 6, 0.2);
			rect.Deflate(6);
			Path path = this.PathRoundRectangle(rect, rect.Width/2);

			Color hiliteColor = this.GetColor(1);
			if (hilited)
			{
				hiliteColor = this.GetColorMain();
				hiliteColor = Color.FromAlphaRgb(hiliteColor.A, 1-(1-hiliteColor.R)*0.2, 1-(1-hiliteColor.G)*0.2, 1-(1-hiliteColor.B)*0.2);
			}

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(hiliteColor);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.GetColor(0));
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

		protected Point PositionMoveColumnsButton(int rank)
		{
			//	Retourne la position du bouton pour déplacer le séparateur des colonnes.
			return new Point(this.ColumnsSeparatorAbsolute(rank), this.bounds.Bottom+AbstractObject.footerHeight/2+1);
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
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point(this.bounds.Left+AbstractObject.buttonRadius*3+8, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionInfoButton
		{
			//	Retourne la position du bouton pour montrer les informations.
			get
			{
				return new Point (this.bounds.Left+AbstractObject.buttonRadius*5+10, this.bounds.Top-AbstractObject.headerHeight/2);
			}
		}

		protected Point PositionParametersButton
		{
			//	Retourne la position du bouton des paramètres.
			get
			{
				return new Point (this.bounds.Left+AbstractObject.buttonRadius*7+12, this.bounds.Top-AbstractObject.headerHeight/2);
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

		protected int GroupLineCount(int titleRank)
		{
			//	Retourne le nombre de ligne d'un groupe (héritage ou interface) d'après le rang de son titre.
			for (int i=titleRank+1; i<this.fields.Count; i++)
			{
				if (this.fields[i].IsSubtitle)
				{
					continue;
				}

				if (this.fields[i].IsTitle || this.fields[i].DefiningEntityId.IsEmpty)
				{
					return i-titleRank-1;
				}
			}

			return this.fields.Count-titleRank-1;
		}

		protected int SubgroupLineCount(int subtitleRank)
		{
			//	Retourne le nombre de ligne d'un sous-groupe (héritage ou interface) d'après le rang de son sous-titre.
			Druid druid = this.fields[subtitleRank+1].DefiningRootEntityId;

			for (int i=subtitleRank+1; i<this.fields.Count; i++)
			{
				if (this.fields[i].IsSubtitle || this.fields[i].DefiningRootEntityId != druid)
				{
					return i-subtitleRank-1;
				}
			}

			return this.fields.Count-subtitleRank-1;
		}

		protected string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip à afficher pour un groupe.
			if (this.fields[rank].DefiningEntityClass == StructuredTypeClass.Interface)
			{
				return string.Format(Res.Strings.Entities.Action.BoxGroup.Interface, this.fields[rank].DefiningEntityName);
			}

			if (this.fields[rank].DefiningEntityClass == StructuredTypeClass.Entity)
			{
				return string.Format(Res.Strings.Entities.Action.BoxGroup.Inherit, this.fields[rank].DefiningEntityName);
			}

			return null;  // pas de tooltip
		}

		protected double ColumnsSeparatorAbsolute(int rank)
		{
			//	Retourne la position absolue du séparateur des colonnes.
			Rectangle rect = this.bounds;
			rect.Deflate(ObjectBox.textMargin, 0);

			double max = rect.Left + System.Math.Floor(rect.Width-ObjectBox.expressionWidth);

			if (rank == 0)
			{
				double pos = rect.Left + System.Math.Floor(rect.Width*this.columnsSeparatorRelative1);
				return System.Math.Min(pos, max);
			}
			else
			{
				return max;
			}
		}


		protected void UpdateSubtitle()
		{
			//	Met à jour le sous-titre de l'entité (nom du module).
			Module module = this.SearchModule(this.cultureMap.Id);
			if (module == this.Application.CurrentModule)
			{
				this.Subtitle = null;
				this.isDimmed = false;
			}
			else
			{
				this.Subtitle = module.ModuleId.Name;
				this.isDimmed = true;
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

			if (this.columnsSeparatorRelative1 != 0.5)
			{
				writer.WriteElementString(Xml.ColumnsSeparatorRelative1, this.columnsSeparatorRelative1.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
			
			if (this.info != null && this.info.IsVisible)  // informations associées ?
			{
				this.info.WriteXml(writer);
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
					else if (name == Xml.Info)
					{
						this.info = new ObjectInfo(this.editor);
						this.info.ReadXml(reader);
						this.info.AttachObject = this;
						this.info.UpdateHeight();  // adapte la hauteur en fonction du contenu
						this.editor.AddInfo(this.info);
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
								Module module = this.SearchModule(druid);
								this.cultureMap = module.AccessEntities.Accessor.Collection[druid];
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
						else if (name == Xml.ColumnsSeparatorRelative1)
						{
							this.columnsSeparatorRelative1 = double.Parse(element);
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
			this.UpdateSubtitle();
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
					field.Initialize(this, dataFields[i]);

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

			this.UpdateFieldsContent();
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


		public static readonly double roundFrameRadius = 12;
		protected static readonly double roundInsideRadius = 8;
		protected static readonly double shadowOffset = 6;
		protected static readonly double textMargin = 13;
		protected static readonly double expressionWidth = 20;
		protected static readonly double fieldHeight = 20;
		protected static readonly double sourcesMenuHeight = 20;
		protected static readonly double indentWidth = 2;

		protected CultureMap cultureMap;
		protected ObjectComment comment;
		protected ObjectInfo info;
		protected Rectangle bounds;
		protected double columnsSeparatorRelative1;
		protected bool isRoot;
		protected bool isExtended;
		protected bool isConnectedToRoot;
		protected string titleString;
		protected string subtitleString;
		protected TextLayout title;
		protected TextLayout subtitle;
		protected List<Field> fields;
		protected int skippedField;
		protected List<SourceInfo> sourcesList;
		protected int sourcesClosedCount;
		protected List<ObjectConnection> connectionListBt;
		protected List<ObjectConnection> connectionListBb;
		protected List<ObjectConnection> connectionListC;
		protected List<ObjectConnection> connectionListD;
		protected List<ObjectBox> parents;

		protected bool isDragging;
		protected Point draggingOffset;

		protected bool isFieldMoving;
		protected int fieldInitialRank;

		protected bool isChangeWidth;
		protected double changeWidthPos;
		protected double changeWidthInitial;

		protected bool isMoveColumnsSeparator1;

		protected bool isSourcesMenu;
		protected int sourcesMenuSelected;
	}
}
