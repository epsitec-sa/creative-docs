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
	public class ObjectNode : LinkableObject
	{
		public ObjectNode(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);

			this.title = new TextLayout();
			this.title.DefaultFontSize = 20;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			if (this.Entity.Name.IsNullOrWhiteSpace)
			{
				this.TitleNumber = this.editor.GetNodeTitleNumbrer ();
			}

			this.UpdateTitle ();

			this.SetBounds (new Rectangle (0, 0, ObjectNode.frameRadius*2, ObjectNode.frameRadius*2));
		}


		public int TitleNumber
		{
			//	Titre au sommet de la boîte (nom du noeud).
			get
			{
				int value;
				if (int.TryParse (this.Entity.Name.ToSimpleText (), out value))
				{
					return value;
				}
				else
				{
					return -1;
				}
			}
			set
			{
				this.Entity.Name = value.ToString ();
				this.UpdateTitle ();
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

		public override Rectangle ExtendedBounds
		{
			//	Retourne la boîte de l'objet, éventuellement agrandie si l'objet est étendu.
			get
			{
				var box = this.bounds;

				if (this.isExtended)
				{
					box = new Rectangle (box.Left, box.Bottom-ObjectNode.extendedHeight, box.Width, box.Height+ObjectNode.extendedHeight);
				}

				return box;
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
			this.UpdateButtonsGeometry ();
		}

		public override void CreateInitialLinks()
		{
			this.objectLinks.Clear ();

			foreach (var entityEdge in this.Entity.Edges)
			{
				var link = new ObjectLink (this.editor, this.Entity);
				link.SrcObject = this;
				link.DstObject = this.editor.SearchInitialObject (entityEdge);

				this.objectLinks.Add (link);
			}
		}


		public override void UpdateObject()
		{
			this.UpdateAttachObject ();
		}

		public override void SetBoundsAtEnd(Point start, Point end)
		{
			Point center = Point.Move (end, start, -ObjectNode.frameRadius);
			Rectangle rect = new Rectangle (center.X-ObjectNode.frameRadius, center.Y-ObjectNode.frameRadius, ObjectNode.frameRadius*2, ObjectNode.frameRadius*2);

			this.SetBounds (rect);
		}


		public override void RemoveEntityLink(LinkableObject dst)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowEdgeEntity);
			System.Diagnostics.Debug.Assert (this.Entity.Edges.Contains (dst.AbstractEntity as WorkflowEdgeEntity));

			this.Entity.Edges.Remove (dst.AbstractEntity as WorkflowEdgeEntity);
		}

		public override void AddEntityLink(LinkableObject dst)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowEdgeEntity);

			this.Entity.Edges.Add (dst.AbstractEntity as WorkflowEdgeEntity);
		}


		public override Vector GetLinkVector(double angle, bool isDst)
		{
			Point c = this.bounds.Center;
			double r = this.isRoot ? ObjectNode.frameRadius+2 : ObjectNode.frameRadius;

			Point p1 = Transform.RotatePointDeg (c, angle, new Point (c.X+r, c.Y));
			Point p2 = Transform.RotatePointDeg (c, angle, new Point (c.X+r+1, c.Y));

			return new Vector (p1, p2);
		}

		public override Point GetLinkStumpPos(double angle)
		{
			Point c = this.bounds.Center;
			return Transform.RotatePointDeg (c, angle, new Point (c.X+ObjectNode.frameRadius+ObjectLink.lengthStumpLink, c.Y));
		}


		public bool IsRoot
		{
			//	Indique s'il s'agit de la boîte racine, c'est-à-dire de la boîte affichée avec un cadre gras.
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


		public override void AcceptEdition()
		{
			this.Entity.Name = this.editingTextField.Text;
			this.UpdateTitle ();

			this.StopEdition ();
		}

		public override void CancelEdition()
		{
			this.StopEdition ();
		}

		public override void StartEdition()
		{
			this.StartEdition (ActiveElement.NodeHeader);
		}

		private void StartEdition(ActiveElement element)
		{
			Rectangle rect = this.RectangleEditName;

			Point p1 = this.editor.ConvEditorToWidget (rect.TopLeft);
			Point p2 = this.editor.ConvEditorToWidget (rect.BottomRight);
			double width  = System.Math.Max (p2.X-p1.X, 30);
			double height = System.Math.Max (p1.Y-p2.Y, 20);
			
			rect = new Rectangle (new Point (p1.X, p1.Y-height), new Size (width, height));

			this.editingTextField = new TextField ();
			this.editingTextField.Parent = this.editor;
			this.editingTextField.SetManualBounds (rect);
			this.editingTextField.Text = this.Entity.Name.ToString ();
			this.editingTextField.TabIndex = 1;
			this.editingTextField.SelectAll ();
			this.editingTextField.Focus ();

			this.editor.EditingObject = this;
			this.editor.ClearHilited ();
			this.UpdateButtonsState ();
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextField);
			this.editingTextField = null;

			this.editor.EditingObject = null;
			this.UpdateButtonsState ();
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDragging)
			{
				return null;  // pas de tooltip
			}

			switch (element)
			{
				case ActiveElement.NodeExtend:
					return this.isExtended ? "Réduit la boîte" : "Etend la boîte";

				case ActiveElement.NodeClose:
					return "Supprime le noeud";

				case ActiveElement.NodeComment:
					return "Ajoute un commentaire au noeud";

				case ActiveElement.NodeInfo:
					return "Monte ou cache les informations du noeud";

				case ActiveElement.NodeAuto:
					return this.Entity.IsAuto ? "Change de \"automatique\" à \"manuel\"" : "Change de \"manuel\" à \"automatique\"";

				case ActiveElement.NodePublic:
					return this.Entity.IsPublic ? "Change de \"public\" à \"privé\"" : "Change de \"privé\" à \"public\"";

				case ActiveElement.NodeOpenLink:
					return "Crée une nouvelle connexion";

				case ActiveElement.NodeColor1:
					return "Jaune";

				case ActiveElement.NodeColor2:
					return "Orange";

				case ActiveElement.NodeColor3:
					return "Rouge";

				case ActiveElement.NodeColor4:
					return "Lilas";

				case ActiveElement.NodeColor5:
					return "Violet";

				case ActiveElement.NodeColor6:
					return "Bleu";

				case ActiveElement.NodeColor7:
					return "Vert";

				case ActiveElement.NodeColor8:
					return "Gris";
			}

			return base.GetToolTipText (element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (this.isDragging)
			{
				this.DraggingMouseMove (pos);
				return true;
			}

			return base.MouseMove (message, pos);
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			this.initialPos = pos;

			if (this.hilitedElement == ActiveElement.NodeHeader && this.editor.LinkableObjectsCount > 1)
			{
				this.isDragging = true;
				this.UpdateButtonsState ();
				this.draggingOffset = pos-this.bounds.BottomLeft;
				this.editor.Invalidate();
				this.editor.LockObject(this);
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (pos == this.initialPos)
			{
				if (this.hilitedElement == ActiveElement.NodeHeader)
				{
					if (this.isDragging)
					{
						this.editor.UpdateAfterGeometryChanged (this);
						this.isDragging = false;
						this.UpdateButtonsState ();
						this.editor.LockObject (null);
						this.editor.SetLocalDirty ();
					}

					this.StartEdition (this.hilitedElement);
					return;
				}
			}

			if (this.isDragging)
			{
				this.editor.UpdateAfterGeometryChanged (this);
				this.isDragging = false;
				this.UpdateButtonsState ();
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.hilitedElement == ActiveElement.NodeExtend)
			{
				if (this.IsExtended)
				{
					this.IsExtended = false;
				}
				else
				{
					this.editor.CompactAll ();
					this.IsExtended = true;
				}
				this.editor.UpdateAfterCommentChanged ();
			}

			if (this.hilitedElement == ActiveElement.NodeOpenLink)
			{
				//	Crée un moignon de lien o--->
				var link = new ObjectLink (this.editor, this.entity);
				link.SrcObject = this;
				link.SetStumpAngle (this.ComputeBestStumpAngle ());
				link.UpdateLink ();

				this.objectLinks.Add (link);
				this.editor.UpdateAfterGeometryChanged (null);
			}

			if (this.hilitedElement == ActiveElement.NodeComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.NodeClose)
			{
				if (!this.isRoot)
				{
					this.editor.CloseObject (this);
					this.editor.UpdateAfterGeometryChanged (null);
				}
			}

			if (this.hilitedElement == ActiveElement.NodeInfo)
			{
				this.AddInfo ();
			}

			if (this.hilitedElement == ActiveElement.NodeAuto)
			{
				this.Entity.IsAuto = !this.Entity.IsAuto;
				this.UpdateButtonsState ();
				this.editor.SetLocalDirty ();
			}

			if (this.hilitedElement == ActiveElement.NodePublic)
			{
				this.Entity.IsPublic = !this.Entity.IsPublic;
				this.UpdateButtonsState ();
				this.editor.SetLocalDirty ();
			}

			if (this.hilitedElement == ActiveElement.NodeColor1)
			{
				this.BackgroundColorItem = ColorItem.Yellow;
			}

			if (this.hilitedElement == ActiveElement.NodeColor2)
			{
				this.BackgroundColorItem = ColorItem.Orange;
			}

			if (this.hilitedElement == ActiveElement.NodeColor3)
			{
				this.BackgroundColorItem = ColorItem.Red;
			}

			if (this.hilitedElement == ActiveElement.NodeColor4)
			{
				this.BackgroundColorItem = ColorItem.Lilac;
			}

			if (this.hilitedElement == ActiveElement.NodeColor5)
			{
				this.BackgroundColorItem = ColorItem.Purple;
			}

			if (this.hilitedElement == ActiveElement.NodeColor6)
			{
				this.BackgroundColorItem = ColorItem.Blue;
			}

			if (this.hilitedElement == ActiveElement.NodeColor7)
			{
				this.BackgroundColorItem = ColorItem.Green;
			}

			if (this.hilitedElement == ActiveElement.NodeColor8)
			{
				this.BackgroundColorItem = ColorItem.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (this.ExtendedBounds.Contains (pos))
			{
				return ActiveElement.NodeHeader;
			}

			if (this.ExtendedBounds.Contains (pos))
			{
				return ActiveElement.NodeInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				return this.DetectButtons (pos);
			}

			return ActiveElement.None;
		}


		private void UpdateAttachObject()
		{
			if (this.info != null)
			{
				this.info.UpdateAfterAttachChanged ();
			}
		}


		public override string DebugInformations
		{
			get
			{
				return string.Format ("Node: {0} {1}", this.Entity.Name.ToString (), this.DebugInformationsObjectLinks);
			}
		}

		public override string DebugInformationsBase
		{
			get
			{
				return string.Format ("{0}", this.Entity.Name.ToString ());
			}
		}

	
		private void AddComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment (this.editor, this.Entity);
				this.comment.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Top = rect.Top+50;
				rect.Width = 200;
				this.comment.SetBounds (rect);
				this.comment.UpdateHeight ();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment (this.comment);
				this.editor.UpdateAfterCommentChanged ();
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.SetLocalDirty ();
		}

		private void AddInfo()
		{
			//	Ajoute une information à la boîte.
			if (this.info == null)
			{
				this.info = new ObjectInfo (this.editor, this.Entity);
				this.info.AttachObject = this;

				Rectangle rect = this.bounds;
				rect.Bottom = rect.Top+20;
				rect.Width = 200;
				this.info.SetBounds (rect);
				this.info.UpdateAfterAttachChanged ();

				this.editor.AddInfo (this.info);
				this.editor.UpdateAfterCommentChanged ();
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
			Color c1, c2;

			bool dragging = (this.hilitedElement == ActiveElement.NodeHeader || this.isHilitedForLinkChanging);
			Color colorFrame = dragging ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

			var extendedRect = new Rectangle (this.bounds.Left, this.bounds.Bottom-ObjectNode.extendedHeight, this.bounds.Width, this.bounds.Height+ObjectNode.extendedHeight-5);
			extendedRect.Deflate (2, 0);
			if (this.isRoot)
			{
				extendedRect.Inflate (2);
			}

			//	Dessine le boîte étendue.
			if (this.isExtended)
			{
				//	Dessine l'ombre.
				rect = extendedRect;
				rect.Offset (ObjectNode.shadowOffset, -(ObjectNode.shadowOffset));
				this.DrawRoundShadow (graphics, rect, ObjectNode.frameRadius, (int) ObjectNode.shadowOffset, 0.2);

				rect = extendedRect;
				rect.Deflate (0.5);
				Path extendedPath = this.PathRoundRectangle (rect, ObjectNode.frameRadius);

				//	Dessine l'intérieur en blanc.
				graphics.Rasterizer.AddSurface (extendedPath);
				graphics.RenderSolid (this.colorFactory.GetColor (1));

				//	Dessine l'intérieur en dégradé.
				graphics.Rasterizer.AddSurface (extendedPath);
				c1 = this.colorFactory.GetColorMain (dragging ? 0.3 : 0.2);
				c2 = this.colorFactory.GetColorMain (dragging ? 0.1 : 0.0);
				this.RenderHorizontalGradient (graphics, this.bounds, c1, c2);

				graphics.Rasterizer.AddOutline (extendedPath, 1);
				graphics.RenderSolid (colorFrame);
			}

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.isRoot)
			{
				rect.Inflate(2);
			}
			rect.Offset(ObjectNode.shadowOffset, -(ObjectNode.shadowOffset));
			this.DrawNodeShadow (graphics, rect, (int) ObjectNode.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathNodeRectangle (rect);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.colorFactory.GetColor(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			c1 = this.colorFactory.GetColorMain (dragging ? 0.8 : 0.4);
			c2 = this.colorFactory.GetColorMain (dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			Color colorLine = this.colorFactory.GetColor (0.9);
			if (dragging)
			{
				colorLine = this.colorFactory.GetColorMain (0.3);
			}

			//	Dessine le titre.
			Color titleColor = dragging ? this.colorFactory.GetColor (1) : this.colorFactory.GetColor (0);

			rect = this.bounds;
			rect.Offset (0, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, this.isRoot ? 6 : 2);
			graphics.RenderSolid (colorFrame);

			if (this.Entity.IsPublic)
			{
				var circlePath = new Path ();
				circlePath.AppendCircle (this.bounds.Center, ObjectNode.frameRadius+2.5);

				Misc.DrawPathDash (graphics, circlePath, 1, 5, 5, false, colorFrame);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine tous les boutons.
			this.DrawButtons (graphics);
		}


		private bool IsHeaderHilite
		{
			//	Indique si la souris est dans l'en-tête.
			get
			{
				if (this.editor.CurrentModifyMode == Editor.ModifyMode.Locked)
				{
					return false;
				}

				return (this.hilitedElement == ActiveElement.NodeHeader ||
						this.hilitedElement == ActiveElement.NodeExtend ||
						this.hilitedElement == ActiveElement.NodeComment ||
						this.hilitedElement == ActiveElement.NodeClose ||
						this.hilitedElement == ActiveElement.NodeInfo ||
						this.hilitedElement == ActiveElement.NodeAuto ||
						this.hilitedElement == ActiveElement.NodePublic ||
						this.hilitedElement == ActiveElement.NodeColor1 ||
						this.hilitedElement == ActiveElement.NodeColor2 ||
						this.hilitedElement == ActiveElement.NodeColor3 ||
						this.hilitedElement == ActiveElement.NodeColor4 ||
						this.hilitedElement == ActiveElement.NodeColor5 ||
						this.hilitedElement == ActiveElement.NodeColor6 ||
						this.hilitedElement == ActiveElement.NodeColor7 ||
						this.hilitedElement == ActiveElement.NodeColor8 ||
						this.hilitedElement == ActiveElement.NodeOpenLink);
			}
		}

		private Rectangle RectangleEditName
		{
			get
			{
				Rectangle rect = this.bounds;
				rect.Deflate (10, 15);

				return rect;
			}
		}

		private Point PositionExtendButton
		{
			//	Retourne la position du bouton.
			get
			{
				return this.bounds.Center;
			}
		}

		private Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point (this.bounds.Center.X-ActiveButton.buttonRadius-1, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius*5+6);
			}
		}

		private Point PositionInfoButton
		{
			//	Retourne la position du bouton.
			get
			{
				return new Point (this.bounds.Center.X+ActiveButton.buttonRadius+1, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius*5+6);
			}
		}

		private Point PositionAutoButton
		{
			//	Retourne la position du bouton.
			get
			{
				return new Point (this.bounds.Center.X-ActiveButton.buttonRadius-1, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius*3+4);
			}
		}

		private Point PositionPublicButton
		{
			//	Retourne la position du bouton.
			get
			{
				return new Point (this.bounds.Center.X+ActiveButton.buttonRadius+1, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius*3+4);
			}
		}

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Center.X, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius+3);
			}
		}

		private Point PositionOpenLinkButton
		{
			//	Retourne la position du bouton pour ouvrir.
			//	Le bouton est placé dans la direction où sera ouvert la connexion.
			get
			{
				if (!this.HasNoneDstObject)
				{
					Point c = this.bounds.Center;
					double a = this.ComputeBestStumpAngle ();

					return Transform.RotatePointDeg (c, a, new Point (c.X+ObjectNode.frameRadius, c.Y));
				}
				else
				{
					return Point.Zero;
				}
			}
		}

		private Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			return new Point (this.bounds.Center.X + (2*ActiveButton.buttonSquare+1)*(rank-3.5) + 0.5, this.bounds.Bottom-16);
		}

		private string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip à afficher pour un groupe.
			return null;  // pas de tooltip
		}


		private void UpdateTitle()
		{
			//	Met à jour le titre du noeud.
			this.title.Text = Misc.Bold (this.Entity.Name.ToString ());
		}


		public WorkflowNodeEntity Entity
		{
			get
			{
				return this.entity as WorkflowNodeEntity;
			}
		}


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.NodeOpenLink, this.colorFactory, GlyphShape.Plus,    this.UpdateButtonGeometryOpenLink, this.UpdateButtonStateOpenLink));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeExtend,   this.colorFactory, GlyphShape.ArrowUp, this.UpdateButtonGeometryExtend,   this.UpdateButtonStateExtend));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeComment,  this.colorFactory, "C",                this.UpdateButtonGeometryComment,  this.UpdateButtonStateComment));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeInfo,     this.colorFactory, "i",                this.UpdateButtonGeometryInfo,     this.UpdateButtonStateInfo));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeAuto,     this.colorFactory, "A",                this.UpdateButtonGeometryAuto,     this.UpdateButtonStateAuto));
			this.buttons.Add (new ActiveButton (ActiveElement.NodePublic,   this.colorFactory, "P",                this.UpdateButtonGeometryPublic,   this.UpdateButtonStatePublic));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeClose,    this.colorFactory, GlyphShape.Close,   this.UpdateButtonGeometryClose,    this.UpdateButtonStateClose));

			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor1, this.colorFactory, ColorItem.Yellow, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor2, this.colorFactory, ColorItem.Orange, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor3, this.colorFactory, ColorItem.Red,    this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor4, this.colorFactory, ColorItem.Lilac,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor5, this.colorFactory, ColorItem.Purple, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor6, this.colorFactory, ColorItem.Blue,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor7, this.colorFactory, ColorItem.Green,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.NodeColor8, this.colorFactory, ColorItem.Grey,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
		}

		private void UpdateButtonGeometryOpenLink(ActiveButton button)
		{
			button.Center = this.PositionOpenLinkButton;
		}

		private void UpdateButtonGeometryExtend(ActiveButton button)
		{
			button.Center = this.PositionExtendButton;
		}

		private void UpdateButtonGeometryClose(ActiveButton button)
		{
			button.Center = this.PositionCloseButton;
		}

		private void UpdateButtonGeometryComment(ActiveButton button)
		{
			button.Center = this.PositionCommentButton;
		}

		private void UpdateButtonGeometryInfo(ActiveButton button)
		{
			button.Center = this.PositionInfoButton;
		}

		private void UpdateButtonGeometryAuto(ActiveButton button)
		{
			button.Center = this.PositionAutoButton;
		}

		private void UpdateButtonGeometryPublic(ActiveButton button)
		{
			button.Center = this.PositionPublicButton;
		}

		private void UpdateButtonGeometryColor(ActiveButton button)
		{
			int rank = button.Element - ActiveElement.NodeColor1;

			button.Center = this.PositionColorButton (rank);
		}

		private void UpdateButtonStateOpenLink(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateExtend(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.Glyph = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateAuto(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private void UpdateButtonStatePublic(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private void UpdateButtonStateClose(ActiveButton button)
		{
			button.State.Enable = !this.isRoot;
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private void UpdateButtonStateComment(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private void UpdateButtonStateInfo(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private void UpdateButtonStateColor(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Selected = this.colorFactory.ColorItem == button.ColorItem;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging && this.isExtended;
			button.State.Detectable = this.isExtended;
		}

		private bool IsDragging
		{
			get
			{
				return this.isDragging || this.editor.IsEditing;
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
							this.boxColor = (ColorItem) System.Enum.Parse(typeof(ColorItem), element);
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


		private static readonly double			frameRadius = 25;
		private static readonly double			shadowOffset = 6;
		private static readonly double			extendedHeight = 90;

		private bool							isRoot;
		private TextLayout						title;

		private bool							isDragging;
		private Point							initialPos;

		private AbstractTextField				editingTextField;
	}
}
