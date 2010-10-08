//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ObjectNode : AbstractObject
	{
		public enum EdgeAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectNode(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);

			this.title = new TextLayout();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.subtitle = new TextLayout();
			this.subtitle.DefaultFontSize = 9;
			this.subtitle.Alignment = ContentAlignment.MiddleCenter;
			this.subtitle.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.edges = new List<Edge>();

			this.columnsSeparatorRelative1 = 0.5;
			this.isRoot = false;
			this.isExtended = false;

			this.edgeListBt = new List<ObjectEdge>();
			this.edgeListBb = new List<ObjectEdge>();
			this.edgeListC  = new List<ObjectEdge>();
			this.edgeListD  = new List<ObjectEdge>();

			this.parents = new List<ObjectNode>();

			this.UpdateTitle ();
			this.UpdateSubtitle ();

			foreach (var entityEdge in this.Entity.Edges)
			{
				var edge = new Edge (this.editor, entityEdge);
				this.edges.Add (edge);
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
					foreach (Edge edge in this.edges)
					{
						if (edge.ObjectEdge != null)
						{
							edge.ObjectEdge.BackgroundMainColor = this.boxColor;
						}
					}

					//	Change la couleur des informations liées.
					if (this.info != null)
					{
						this.info.BackgroundMainColor = this.boxColor;
					}

					this.editor.Invalidate();
					this.editor.SetLocalDirty ();
				}
			}
		}

		public List<Edge> Edges
		{
			get
			{
				return this.edges;
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

					this.UpdateEdgesLink();
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
			this.Title = this.Entity.Name.ToString ();
		}


		public List<ObjectEdge> EdgeListBt
		{
			get
			{
				return this.edgeListBt;
			}
		}

		public List<ObjectEdge> EdgeListBb
		{
			get
			{
				return this.edgeListBb;
			}
		}

		public List<ObjectEdge> EdgeListC
		{
			get
			{
				return this.edgeListC;
			}
		}

		public List<ObjectEdge> EdgeListD
		{
			get
			{
				return this.edgeListD;
			}
		}

		public List<ObjectNode> Parents
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
				return AbstractObject.headerHeight + ObjectNode.edgeHeight*this.edges.Count + AbstractObject.footerHeight + 20;
			}
			else
			{
				return AbstractObject.headerHeight;
			}
		}

		public double GetEdgeSrcVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			//	Il s'agit toujours de la position de départ d'une liaison.
			if (this.isExtended && rank < this.edges.Count)
			{
				Rectangle rect = this.GetFieldBounds(rank);
				return rect.Center.Y;
			}
			else
			{
				return this.bounds.Center.Y;
			}
		}

		public Point GetEdgeDstPosition(double posv, EdgeAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			//	Il s'agit toujours de la position d'arrivée d'une liaison.
			switch (anchor)
			{
				case EdgeAnchor.Left:
					if (posv >= this.bounds.Bottom+ObjectNode.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectNode.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, false))
					{
						return new Point(this.bounds.Left, posv);
					}

					if (this.isExtended)
					{
						//	En dessous du glyph 'o--' et du moignon "source".
						return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight-12);
					}
					else
					{
						return new Point(this.bounds.Left, this.bounds.Top-AbstractObject.headerHeight*0.5);
					}


				case EdgeAnchor.Right:
					if (posv >= this.bounds.Bottom+ObjectNode.roundFrameRadius &&
						posv <= this.bounds.Top-ObjectNode.roundFrameRadius &&
						this.IsVerticalPositionFree(posv, true))
					{
						return new Point(this.bounds.Right, posv);
					}

					return new Point(this.bounds.Right, this.bounds.Top-AbstractObject.headerHeight*0.5);

				case EdgeAnchor.Bottom:
					return new Point(this.bounds.Center.X, this.bounds.Bottom);

				case EdgeAnchor.Top:
					return new Point(this.bounds.Center.X, this.bounds.Top);
			}

			return Point.Zero;
		}

		protected bool IsVerticalPositionFree(double posv, bool right)
		{
			//	Cherche si une position verticale n'est occupée par aucun départ de liaison.
			if (!right && this.isExtended)
			{
				double y = this.bounds.Top-AbstractObject.headerHeight;
				if (posv >= y-ObjectNode.edgeHeight/2 && posv <= y+ObjectNode.edgeHeight/2)  // sur le moignon "source" ?
				{
					return false;
				}
			}

			if (!right && this.isExtended)
			{
				double y = this.bounds.Top-AbstractObject.headerHeight*0.5;
				if (posv >= y-ObjectNode.edgeHeight/2 && posv <= y+ObjectNode.edgeHeight/2)  // sur le glyph 'o--' ?
				{
					return false;
				}
			}

			for (int i=0; i<this.edges.Count; i++)
			{
				Edge edge = this.edges[i];
				ObjectEdge objectEdge = edge.ObjectEdge;

				if (edge.Relation != FieldRelation.None && objectEdge != null)
				{
					Rectangle rect = this.GetFieldBounds(i);
					if (posv >= rect.Bottom && posv <= rect.Top)
					{
						if (edge.IsExplored)
						{
							if (edge.IsAttachToRight)
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
			if (this.isDragging || this.isEdgeMoving || this.isChangeWidth || this.isMoveColumnsSeparator1)
			{
				return null;  // pas de tooltip
			}

#if false
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
						return string.Format(Res.Strings.Entities.Action.BoxInfo1, this.GetInformations(true));
					}
					else
					{
						return Res.Strings.Entities.Action.BoxInfo2;
					}

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
#endif

			return base.GetToolTipText(element, fieldRank);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				Rectangle bounds = this.editor.NodeGridAlign (new Rectangle (pos-this.draggingOffset, this.Bounds.Size));
				this.SetBounds(bounds);
				this.editor.UpdateEdges();
				return true;
			}
			else if (this.isEdgeMoving)
			{
				return base.MouseMove(message, pos);
			}
			else if (this.isChangeWidth)
			{
				Rectangle bounds = this.Bounds;
				bounds.Width = this.editor.GridAlign(System.Math.Max(pos.X-this.changeWidthPos+this.changeWidthInitial, 120));
				this.SetBounds(bounds);
				this.editor.UpdateEdges();
				return true;
			}
			else if (this.isMoveColumnsSeparator1)
			{
				Rectangle rect = this.Bounds;
				rect.Deflate(ObjectNode.textMargin, 0);
				pos.X = System.Math.Min(pos.X, this.ColumnsSeparatorAbsolute(1));
				this.columnsSeparatorRelative1 = (pos.X-rect.Left)/rect.Width;
				this.columnsSeparatorRelative1 = System.Math.Max(this.columnsSeparatorRelative1, 0.2);
				this.editor.Invalidate();
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
			if (this.hilitedElement == ActiveElement.NodeHeader && this.editor.NodeCount > 1 && !this.editor.IsLocateActionHeader(message))
			{
				this.isDragging = true;
				this.draggingOffset = pos-this.bounds.BottomLeft;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.NodeEdgeMovable)
			{
				this.isEdgeMoving = true;
				this.edgeInitialRank = this.hilitedEdgeRank;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.NodeChangeWidth)
			{
				this.isChangeWidth = true;
				this.changeWidthPos = pos.X;
				this.changeWidthInitial = this.bounds.Width;
				this.editor.LockObject(this);
			}

			if (this.hilitedElement == ActiveElement.NodeMoveColumnsSeparator1)
			{
				this.isMoveColumnsSeparator1 = true;
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
				this.editor.SetLocalDirty ();
			}
			else if (this.isEdgeMoving)
			{
				if (this.hilitedElement == ActiveElement.NodeEdgeMoving)
				{
					this.MoveField(this.edgeInitialRank, this.hilitedEdgeRank);
				}
				this.isEdgeMoving = false;
				this.editor.LockObject(null);
			}
			else if (this.isChangeWidth)
			{
				this.editor.UpdateAfterMoving(this);
				this.isChangeWidth = false;
				this.editor.LockObject(null);
				this.editor.SetLocalDirty ();
			}
			else if (this.isMoveColumnsSeparator1)
			{
				this.isMoveColumnsSeparator1 = false;
				this.editor.LockObject(null);
				this.editor.SetLocalDirty ();
			}
			else
			{
				if (this.hilitedElement == ActiveElement.NodeHeader && this.editor.IsLocateActionHeader(message) && !this.isRoot)
				{
					this.LocateEntity();
				}

				if (this.hilitedElement == ActiveElement.NodeExtend)
				{
					this.IsExtended = !this.IsExtended;
					this.editor.UpdateAfterGeometryChanged(this);
					this.editor.SetLocalDirty ();
				}

				if (this.hilitedElement == ActiveElement.NodeClose)
				{
					if (!this.isRoot)
					{
						this.editor.CloseNode(this);
						this.editor.UpdateAfterAddOrRemoveEdge(null);
					}
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeRemove)
				{
					this.RemoveField(this.hilitedEdgeRank);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeAdd)
				{
					this.AddField(this.hilitedEdgeRank);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeAddInterface)
				{
					this.AddInterface();
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeRemoveInterface)
				{
					this.RemoveInterface(this.hilitedEdgeRank);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeName)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateField(this.hilitedEdgeRank);
					}
					else
					{
						if (this.IsMousePossible(this.hilitedElement, this.hilitedEdgeRank))
						{
							this.ChangeFieldType(this.hilitedEdgeRank);
						}
					}
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeType)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateType(this.hilitedEdgeRank);
					}
					else
					{
						if (this.IsMousePossible(this.hilitedElement, this.hilitedEdgeRank))
						{
							this.ChangeFieldType(this.hilitedEdgeRank);
						}
					}
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeTitle)
				{
					if (this.editor.IsLocateAction(message))
					{
						this.LocateTitle(this.hilitedEdgeRank);
					}
				}

				if (this.hilitedElement == ActiveElement.NodeComment)
				{
					this.AddComment();
				}

				if (this.hilitedElement == ActiveElement.NodeInfo)
				{
					this.AddInfo();
				}

				if (this.hilitedElement == ActiveElement.NodeColor5)
				{
					this.BackgroundMainColor = MainColor.Yellow;
				}

				if (this.hilitedElement == ActiveElement.NodeColor6)
				{
					this.BackgroundMainColor = MainColor.Orange;
				}

				if (this.hilitedElement == ActiveElement.NodeColor3)
				{
					this.BackgroundMainColor = MainColor.Red;
				}

				if (this.hilitedElement == ActiveElement.NodeColor7)
				{
					this.BackgroundMainColor = MainColor.Lilac;
				}

				if (this.hilitedElement == ActiveElement.NodeColor8)
				{
					this.BackgroundMainColor = MainColor.Purple;
				}

				if (this.hilitedElement == ActiveElement.NodeColor1)
				{
					this.BackgroundMainColor = MainColor.Blue;
				}

				if (this.hilitedElement == ActiveElement.NodeColor2)
				{
					this.BackgroundMainColor = MainColor.Green;
				}

				if (this.hilitedElement == ActiveElement.NodeColor4)
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
			this.SetEdgesHilited(false);

			if (pos.IsZero)
			{
				//	Si l'une des connection est dans l'état EdgeOpen*, il faut afficher
				//	aussi les petits cercles de gauche.
				if (this.IsEdgeReadyForOpen())
				{
					this.SetEdgesHilited(true);
				}
				return false;
			}

			Rectangle rect;

			if (this.isEdgeMoving)
			{
				//	Souris entre deux champs ?
				for (int i=-1; i<this.edges.Count; i++)
				{
					rect = this.GetFieldMovingBounds(i);
					if (rect.Contains(pos))
					{
						element = ActiveElement.NodeEdgeMoving;
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
					element = ActiveElement.NodeExtend;
					return true;
				}

				//	Souris dans le bouton de fermeture ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionCloseButton, pos))
				{
					element = ActiveElement.NodeClose;
					return true;
				}

				//	Souris dans le bouton des commentaires ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionCommentButton, pos))
				{
					element = ActiveElement.NodeComment;
					return true;
				}

				//	Souris dans le bouton des informations ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectRoundButton(this.PositionInfoButton, pos))
				{
					element = ActiveElement.NodeInfo;
					return true;
				}

				//	Souris dans le bouton des couleurs ?
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(0), pos))
				{
					element = ActiveElement.NodeColor5;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(1), pos))
				{
					element = ActiveElement.NodeColor6;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(2), pos))
				{
					element = ActiveElement.NodeColor3;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton(this.PositionColorButton(3), pos))
				{
					element = ActiveElement.NodeColor7;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (4), pos))
				{
					element = ActiveElement.NodeColor8;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (5), pos))
				{
					element = ActiveElement.NodeColor1;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (6), pos))
				{
					element = ActiveElement.NodeColor2;
					return true;
				}

				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.DetectSquareButton (this.PositionColorButton (7), pos))
				{
					element = ActiveElement.NodeColor4;
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
							element = ActiveElement.NodeChangeWidth;
							return true;
						}
					}
					else
					{
						if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && d2 <= AbstractObject.buttonRadius+1)
						{
							element = ActiveElement.NodeMoveColumnsSeparator1;
							return true;
						}
					}

					//	Souris dans l'en-tête ?
					if (this.bounds.Contains(pos) && 
						(pos.Y >= this.bounds.Top-AbstractObject.headerHeight ||
						 pos.Y <= this.bounds.Bottom+AbstractObject.footerHeight))
					{
						element = ActiveElement.NodeHeader;
						this.SetEdgesHilited(true);
						return true;
					}

					//	Souris entre deux champs ?
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						for (int i=-1; i<this.edges.Count; i++)
						{
							rect = this.GetFieldAddBounds(i);
							if (rect.Contains(pos))
							{
								element = ActiveElement.NodeEdgeAdd;
								fieldRank = i;
								this.SetEdgesHilited(true);
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
						element = ActiveElement.NodeMoveColumnsSeparator1;
						this.SetEdgesHilited(true);
						return true;
					}

					//	Souris dans un champ ?
					for (int i=0; i<this.edges.Count; i++)
					{
						rect = this.GetFieldRemoveBounds(i);
						if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked && rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeRemove;
							fieldRank = i;
							this.SetEdgesHilited(true);
							return true;
						}

						rect = this.GetFieldMovableBounds(i);
						if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked && rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeMovable;
							fieldRank = i;
							this.SetEdgesHilited(true);
							return true;
						}

						rect = this.GetFieldNameBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeName;
							fieldRank = i;
							this.SetEdgesHilited(true);
							return true;
						}

						rect = this.GetFieldTypeBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeType;
							fieldRank = i;
							this.SetEdgesHilited(true);
							return true;
						}

						rect = this.GetFieldExpressionBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeExpression;
							fieldRank = i;
							this.SetEdgesHilited(true);
							return true;
						}
					}

					for (int i=0; i<this.edges.Count; i++)
					{
						rect = this.GetFieldGroupBounds(i);
						if (rect.Contains(pos))
						{
							element = ActiveElement.NodeEdgeGroup;
							fieldRank = i;
							return true;
						}
					}
				}
				else  // boîte compactée ?
				{
					if (this.bounds.Contains(pos))
					{
						element = ActiveElement.NodeHeader;
						return true;
					}
				}
			}

			if (!this.bounds.Contains(pos))
			{
				return false;
			}

			element = ActiveElement.NodeInside;
			this.SetEdgesHilited(true);
			return true;
		}

		public override bool IsMousePossible(ActiveElement element, int fieldRank)
		{
			//	Indique si l'opération est possible.
			return true;
		}

		protected void SetEdgesHilited(bool isHilited)
		{
			//	Modifie l'état 'hilited' de toutes les connections qui partent de l'objet.
			//	Avec false, les petits cercles des liaisons fermées ne sont affichés qu'à droite.
			if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
			{
				isHilited = false;
			}

			foreach (Edge edge in this.edges)
			{
				if (edge.ObjectEdge != null)
				{
					edge.ObjectEdge.IsSrcHilited = isHilited;
				}
			}
		}

		protected bool IsEdgeReadyForOpen()
		{
			//	Indique si l'une des connections qui partent de l'objet est en mode EdgeOpen*.
			foreach (Edge edge in this.edges)
			{
				if (edge.ObjectEdge != null)
				{
					ActiveElement ae = edge.ObjectEdge.HilitedElement;
					if (ae == ActiveElement.EdgeOpenLeft ||
						ae == ActiveElement.EdgeOpenRight)
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
			
			rect.Bottom -= ObjectNode.edgeHeight/2;
			rect.Height = ObjectNode.edgeHeight;

			return rect;
		}

		protected Rectangle GetFieldNameBounds(int rank)
		{
			//	Retourne le rectangle occupé par le nom d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);

			rect.Deflate(ObjectNode.textMargin, 0);
			rect.Right = this.ColumnsSeparatorAbsolute(0);

			return rect;
		}

		protected Rectangle GetFieldTypeBounds(int rank)
		{
			//	Retourne le rectangle occupé par le type d'un champ.
			Rectangle rect = this.GetFieldBounds(rank);
			
			rect.Deflate(ObjectNode.textMargin, 0);
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
			Rectangle rect = this.GetFieldBounds(rank);

			for (int i=rank+1; i<this.edges.Count; i++)
			{
				rect = Rectangle.Union(rect, this.GetFieldBounds(i));
			}

			rect.Deflate(9.0, 0.0);
			return rect;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;
			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - AbstractObject.headerHeight - ObjectNode.edgeHeight*(rank+1) - 12;
			rect.Height = ObjectNode.edgeHeight;

			return rect;
		}


		protected void LocateEntity()
		{
			//	Montre l'entité cliquée avec le bouton de droite.
		}

		protected void LocateTitle(int rank)
		{
			//	Montre l'entité héritée ou l'interface cliquée avec le bouton de droite.
		}

		protected void LocateField(int rank)
		{
			//	Montre le champ cliqué avec le bouton de droite.
		}

		protected void LocateType(int rank)
		{
			//	Montre le type cliqué avec le bouton de droite.
		}


		protected void MoveField(int srcRank, int dstRank)
		{
			//	Déplace un champ.
		}

		protected void RemoveField(int rank)
		{
			//	Supprime un champ.
		}

		protected void AddField(int rank)
		{
			//	Ajoute un nouveau champ.
		}

		protected void AddInterface()
		{
			//	Ajoute une interface à l'entité.
		}

		protected void RemoveInterface(int rank)
		{
			//	Supprime une interface de l'entité.
		}

		protected void ChangeFieldType(int rank)
		{
			//	Choix du type pour un champ.
		}

		protected void UpdateFieldsContent()
		{
			//	Crée tous les champs de titrage.
		}

		protected void UpdateInformations()
		{
			//	Met à jour les informations de l'éventuel ObjectInfo lié.
			if (this.info != null)  // existe un ObjectInfo lié ?
			{
				this.info.Text = this.GetInformations (false);
				this.editor.UpdateAfterCommentChanged ();
			}
		}

		protected string GetInformations(bool resume)
		{
			//	Retourne les informations pour l'ObjectInfo lié.
			return null;
		}

		protected void UpdateEdgesLink()
		{
			//	Met à jour toutes les liaisons des champs.
			for (int i=0; i<this.edges.Count; i++)
			{
				this.edges[i].Index = i;
				this.edges[i].IsSourceExpanded = this.isExtended;
			}
		}


		protected void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment (this.editor, null);
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

			this.editor.SetLocalDirty ();
		}

		protected void AddInfo()
		{
			//	Ajoute une information à la boîte.
			if (this.info == null)
			{
				this.info = new ObjectInfo (this.editor, null);
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

			this.editor.SetLocalDirty ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.NodeHeader);

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.isRoot)
			{
				rect.Inflate(2);
			}
			rect.Offset(ObjectNode.shadowOffset, -(ObjectNode.shadowOffset));
			this.DrawShadow(graphics, rect, ObjectNode.roundFrameRadius+ObjectNode.shadowOffset, (int)ObjectNode.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle(rect, ObjectNode.roundFrameRadius);

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
			if (this.hilitedElement == ActiveElement.NodeExtend)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionExtendButton, AbstractObject.buttonRadius, shape, false, false);
			}

			//	Dessine le bouton de fermeture.
			if (this.hilitedElement == ActiveElement.NodeClose)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, true, false, !this.isRoot);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCloseButton, AbstractObject.buttonRadius, GlyphShape.Close, false, false, !this.isRoot);
			}

			//	Dessine le moignon pour les sources à gauche.
			if (this.hilitedElement != ActiveElement.None)
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

			//	Dessine le bouton des commentaires.
			if (this.hilitedElement == ActiveElement.NodeComment)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionCommentButton, AbstractObject.buttonRadius, "C", false, false);
			}

			//	Dessine le bouton des informations.
			if (this.hilitedElement == ActiveElement.NodeInfo)
			{
				this.DrawRoundButton(graphics, this.PositionInfoButton, AbstractObject.buttonRadius, "i", true, false);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawRoundButton(graphics, this.PositionInfoButton, AbstractObject.buttonRadius, "i", false, false);
			}

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-AbstractObject.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-AbstractObject.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+AbstractObject.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+AbstractObject.footerHeight+0.5);
				graphics.RenderSolid(colorFrame);

				//	Dessine toutes les lignes, titres ou simples champs.
				for (int i=0; i<this.edges.Count; i++)
				{
					Color colorName = this.GetColor(0);
					Color colorType = this.GetColor(0);
					Color colorExpr = this.GetColor(0);

					if (this.hilitedElement == ActiveElement.NodeEdgeName && this.hilitedEdgeRank == i)
					{
						rect = this.GetFieldNameBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain());

						colorName = this.GetColor(1);
					}

					if (this.hilitedElement == ActiveElement.NodeEdgeType && this.hilitedEdgeRank == i)
					{
						rect = this.GetFieldTypeBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain());

						colorType = this.GetColor(1);
					}

					if (this.hilitedElement == ActiveElement.NodeEdgeExpression && this.hilitedEdgeRank == i)
					{
						rect = this.GetFieldExpressionBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain());

						colorExpr = this.GetColor(1);
					}

					if ((this.hilitedElement == ActiveElement.NodeEdgeRemove || this.hilitedElement == ActiveElement.NodeEdgeMovable) && this.hilitedEdgeRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain(0.3));
					}

					if (this.isEdgeMoving && this.edgeInitialRank == i)
					{
						rect = this.GetFieldBounds(i);

						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.GetColorMain(0.3));
					}

					//	Affiche le nom du champ.
					rect = this.GetFieldNameBounds(i);
					rect.Right -= 2;
					this.edges[i].TextLayoutField.LayoutSize = rect.Size;
					this.edges[i].TextLayoutField.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorName, GlyphPaintStyle.Normal);

					//	Affiche le type du champ.
					rect = this.GetFieldTypeBounds(i);
					rect.Left += 1;
					if (rect.Width > 10)
					{
						this.edges[i].TextLayoutType.LayoutSize = rect.Size;
						this.edges[i].TextLayoutType.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, colorType, GlyphPaintStyle.Normal);
					}

					rect = this.GetFieldBounds(i);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(colorLine);
				}

				//	Dessine tous les cadres liés aux titres.
				for (int i=0; i<this.edges.Count; i++)
				{
					if (i < this.edges.Count-1)
					{
						rect = this.GetFieldBounds(i);
						rect.Deflate(9.5, 0.5);
						Path dashedPath = new Path();

						dashedPath.MoveTo(rect.Left, rect.Bottom);
						dashedPath.LineTo(rect.Right, rect.Bottom);
						graphics.Rasterizer.AddOutline(dashedPath);
						graphics.RenderSolid(this.GetColorMain(0.8));
					}
				}

				//	Met en évidence le groupe survolé.
				if (this.hilitedElement == ActiveElement.NodeEdgeGroup)
				{
					rect = this.GetFieldGroupBounds(this.hilitedEdgeRank);
					rect.Deflate(1.5);
					rect.Bottom += 1.0;
					Path roundedPath = this.PathRoundRectangle(rect, ObjectNode.roundInsideRadius, false, true);

					graphics.Rasterizer.AddSurface(roundedPath);
					graphics.RenderSolid(this.GetColorMain(0.1));

					graphics.Rasterizer.AddOutline(roundedPath, 3);
					graphics.RenderSolid(this.GetColorMain(0.5));
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeMoving)
				{
					Point p1 = this.GetFieldBounds(this.edgeInitialRank).Center;
					Point p2 = this.GetFieldMovingBounds(this.hilitedEdgeRank).Center;
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					this.DrawMovingArrow(graphics, p1, p2);
				}

				if (this.hilitedElement != ActiveElement.None &&
					this.hilitedElement != ActiveElement.NodeChangeWidth &&
					this.hilitedElement != ActiveElement.NodeMoveColumnsSeparator1 &&
					this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked &&
					!this.IsHeaderHilite && !this.isEdgeMoving && !this.isChangeWidth && !this.isMoveColumnsSeparator1)
				{
					//	Dessine la glissière à gauche pour suggérer les boutons Add/Remove des champs.
					Point p1 = this.GetFieldAddBounds(-1).Center;
					Point p2 = this.GetFieldAddBounds(this.edges.Count-1).Center;
					bool hilited = this.hilitedElement == ActiveElement.NodeEdgeAdd || this.hilitedElement == ActiveElement.NodeEdgeRemove;
					this.DrawEmptySlider(graphics, p1, p2, hilited);

					//	Dessine la glissière à droite pour suggérer les boutons Movable des champs.
					p1.X = p2.X = this.GetFieldMovableBounds(0).Center.X;
					hilited = this.hilitedElement == ActiveElement.NodeEdgeMovable;
					this.DrawEmptySlider(graphics, p1, p2, hilited);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, this.isRoot ? 6 : 2);
			graphics.RenderSolid(colorFrame);

			//	Dessine les boutons sur les glissières.
			if (this.isExtended)
			{
				if (this.hilitedElement == ActiveElement.NodeEdgeRemove)
				{
					rect = this.GetFieldRemoveBounds(this.hilitedEdgeRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Minus, true, true);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeAdd)
				{
					rect = this.GetFieldBounds(this.hilitedEdgeRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorMain());

					rect = this.GetFieldAddBounds(this.hilitedEdgeRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.Plus, true, true);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeMovable)
				{
					rect = this.GetFieldMovableBounds(this.hilitedEdgeRank);
					this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, GlyphShape.VerticalMove, true, true);
				}

				if (this.hilitedElement == ActiveElement.NodeEdgeMoving)
				{
					rect = this.GetFieldBounds(this.hilitedEdgeRank);
					rect.Deflate(this.isRoot ? 3.5 : 1.5, 0.5);
					this.DrawDashLine(graphics, rect.BottomRight, rect.BottomLeft, this.GetColorMain());
				}

				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
				{
					if (this.hilitedElement == ActiveElement.NodeEdgeRemoveInterface ||
						this.hilitedElement == ActiveElement.NodeEdgeTitle)
					{
						rect = this.GetFieldMovableBounds(this.hilitedEdgeRank);
						this.DrawRoundButton(graphics, rect.Center, AbstractObject.buttonRadius, "-", this.hilitedElement == ActiveElement.NodeEdgeRemoveInterface, true);
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
			if (this.hilitedElement == ActiveElement.NodeColor5)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (0), MainColor.Yellow, this.boxColor == MainColor.Yellow, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor6)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (1), MainColor.Orange, this.boxColor == MainColor.Orange, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor3)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (2), MainColor.Red, this.boxColor == MainColor.Red, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor7)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (3), MainColor.Lilac, this.boxColor == MainColor.Lilac, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor8)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton (graphics, this.PositionColorButton (4), MainColor.Purple, this.boxColor == MainColor.Purple, false);
			}
			
			if (this.hilitedElement == ActiveElement.NodeColor1)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(5), MainColor.Blue, this.boxColor == MainColor.Blue, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor2)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, true);
			}
			else if (this.IsHeaderHilite && !this.isDragging)
			{
				this.DrawSquareButton(graphics, this.PositionColorButton(6), MainColor.Green, this.boxColor == MainColor.Green, false);
			}

			if (this.hilitedElement == ActiveElement.NodeColor4)
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
				if (this.hilitedElement == ActiveElement.NodeMoveColumnsSeparator1)
				{
					double sep = this.ColumnsSeparatorAbsolute(0);
					graphics.LineWidth = 4;
					graphics.AddLine(sep, this.bounds.Bottom+AbstractObject.footerHeight+3, sep, this.bounds.Top-AbstractObject.headerHeight-3);
					graphics.LineWidth = 1;
					graphics.RenderSolid(this.GetColorMain());

					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton(0), AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.NodeHeader && !this.isDragging)
				{
					this.DrawRoundButton(graphics, this.PositionMoveColumnsButton(0), AbstractObject.buttonRadius, GlyphShape.HorizontalMove, false, false);
				}

				//	Dessine le bouton pour changer la largeur.
				if (this.hilitedElement == ActiveElement.NodeChangeWidth)
				{
					this.DrawRoundButton(graphics, this.PositionChangeWidthButton, AbstractObject.buttonRadius, GlyphShape.HorizontalMove, true, false);
				}
				if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked && this.hilitedElement == ActiveElement.NodeHeader && !this.isDragging)
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

				return (this.hilitedElement == ActiveElement.NodeHeader ||
						this.hilitedElement == ActiveElement.NodeComment ||
						this.hilitedElement == ActiveElement.NodeInfo ||
						this.hilitedElement == ActiveElement.NodeColor1 ||
						this.hilitedElement == ActiveElement.NodeColor2 ||
						this.hilitedElement == ActiveElement.NodeColor3 ||
						this.hilitedElement == ActiveElement.NodeColor4 ||
						this.hilitedElement == ActiveElement.NodeColor5 ||
						this.hilitedElement == ActiveElement.NodeColor6 ||
						this.hilitedElement == ActiveElement.NodeColor7 ||
						this.hilitedElement == ActiveElement.NodeColor8 ||
						this.hilitedElement == ActiveElement.NodeExtend ||
						this.hilitedElement == ActiveElement.NodeClose);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
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
			if (System.Math.Abs(p1.Y-p2.Y) < ObjectNode.edgeHeight)
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
				return new Point(this.bounds.Left+AbstractObject.buttonRadius*5+10, this.bounds.Top-AbstractObject.headerHeight/2);
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

		protected string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip à afficher pour un groupe.
			return null;  // pas de tooltip
		}

		protected double ColumnsSeparatorAbsolute(int rank)
		{
			//	Retourne la position absolue du séparateur des colonnes.
			Rectangle rect = this.bounds;
			rect.Deflate(ObjectNode.textMargin, 0);

			double max = rect.Left + System.Math.Floor(rect.Width-ObjectNode.expressionWidth);

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
			this.Subtitle = null;
			this.isDimmed = false;
		}


		private WorkflowNodeEntity Entity
		{
			get
			{
				return this.entity as WorkflowNodeEntity;
			}
		}


		#region Serialization
		public void WriteXml(XmlWriter writer)
		{
			//	Sérialise toutes les informations de la boîte et de ses champs.
#if false
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
#endif
		}

		public void ReadXml(XmlReader reader)
		{
#if false
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
#endif
		}

		public void AdjustAfterRead()
		{
			//	Ajuste le contenu de la boîte après sa désérialisation.
		}
		#endregion


		public static readonly double roundFrameRadius = 12;
		protected static readonly double roundInsideRadius = 8;
		protected static readonly double shadowOffset = 6;
		protected static readonly double textMargin = 13;
		protected static readonly double expressionWidth = 20;
		protected static readonly double edgeHeight = 20;
		protected static readonly double sourcesMenuHeight = 20;
		protected static readonly double indentWidth = 2;

		private ObjectComment comment;
		private ObjectInfo info;
		private Rectangle bounds;
		private double columnsSeparatorRelative1;
		private bool isRoot;
		private bool isExtended;
		private bool isConnectedToRoot;
		private string titleString;
		private string subtitleString;
		private TextLayout title;
		private TextLayout subtitle;
		private List<Edge> edges;
		private List<ObjectEdge> edgeListBt;
		private List<ObjectEdge> edgeListBb;
		private List<ObjectEdge> edgeListC;
		private List<ObjectEdge> edgeListD;
		private List<ObjectNode> parents;

		private bool isDragging;
		private Point draggingOffset;

		private bool isEdgeMoving;
		private int edgeInitialRank;

		private bool isChangeWidth;
		private double changeWidthPos;
		private double changeWidthInitial;

		private bool isMoveColumnsSeparator1;
	}
}
