//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
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
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split;

			if (this.Entity.Name.IsNullOrWhiteSpace)
			{
				this.TitleNumber = this.editor.GetNodeTitleNumbrer ();
			}

			this.UpdateTitle ();

			this.magnetConstrains.Add (new MagnetConstrain (isVertical: true));
			this.magnetConstrains.Add (new MagnetConstrain (isVertical: false));

			this.Bounds = new Rectangle (0, 0, ObjectNode.frameRadius*2, ObjectNode.frameRadius*2);
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

		public override Margins RedimMargin
		{
			get
			{
				if (this.isExtended)
				{
					return new Margins (AbstractObject.redimMargin, AbstractObject.redimMargin, AbstractObject.redimMargin, 0);
				}
				else
				{
					return new Margins (AbstractObject.redimMargin);
				}
			}
		}

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
			this.UpdateGeometry ();
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


		public override void ContextMenu()
		{
			if (!this.Entity.IsForeign)  // pas un noeud étranger ?
			{
				if (!this.Entity.IsPublic || this.editor.IsUnusedPublicNode (this.Entity))
				{
					this.editor.CreateMenuItem (!this.Entity.IsPublic, "Nœud privé",  "Node.Private");
					this.editor.CreateMenuItem (this.Entity.IsPublic,  "Nœud public", "Node.Public");
				}

				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (!this.Entity.IsAuto, "Nœud manuel",      "Node.Manuel");
				this.editor.CreateMenuItem (this.Entity.IsAuto,  "Nœud automatique", "Node.Auto");
			}

			this.editor.CreateMenuSeparator ();
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Yellow, "Jaune",  "Node.ColorYellow");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Orange, "Orange", "Node.ColorOrange");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Red,    "Rouge",  "Node.ColorRed");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Lilac,  "Lilas",  "Node.ColorLilac");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Purple, "Violet", "Node.ColorPurple");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Blue,   "Bleu",   "Node.ColorBlue");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Green,  "Vert",   "Node.ColorGreen");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Grey,   "Gris",   "Node.ColorGrey");

			if (this.IsButtonEnable (ActiveElement.NodeOpenLink))
			{
				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (null, "Crée une nouvelle connexion", "Node.OpenLink");
			}

			if (this.IsButtonEnable (ActiveElement.NodeComment) || this.IsButtonEnable (ActiveElement.NodeInfo))
			{
				this.editor.CreateMenuSeparator ();

				if (this.IsButtonEnable (ActiveElement.NodeComment))
				{
					this.editor.CreateMenuItem (null, this.comment == null ? "Ajoute un commentaire" : "Ferme le commentaire", "Node.Comment");
				}

				if (this.IsButtonEnable (ActiveElement.NodeInfo))
				{
					this.editor.CreateMenuItem (null, this.info == null ? "Monte les informations" : "Ferme les informations", "Node.Info");
				}
			}

			if (this.IsButtonEnable (ActiveElement.NodeClose))
			{
				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (null, "Supprime le nœud...", "Node.Delete");
			}
		}

		public override void MenuAction(string name)
		{
			switch (name)
            {
				case "Node.Private":
					this.Entity.IsPublic = false;
					this.editor.SetLocalDirty ();
					break;

				case "Node.Public":
					this.Entity.IsPublic = true;
					this.editor.SetLocalDirty ();
					break;


				case "Node.Manuel":
					this.Entity.IsAuto = false;
					this.editor.SetLocalDirty ();
					break;

				case "Node.Auto":
					this.Entity.IsAuto = true;
					this.editor.SetLocalDirty ();
					break;


				case "Node.ColorYellow":
					this.BackgroundColorItem = ColorItem.Yellow;
					break;

				case "Node.ColorOrange":
					this.BackgroundColorItem = ColorItem.Orange;
					break;

				case "Node.ColorRed":
					this.BackgroundColorItem = ColorItem.Red;
					break;

				case "Node.ColorLilac":
					this.BackgroundColorItem = ColorItem.Lilac;
					break;

				case "Node.ColorPurple":
					this.BackgroundColorItem = ColorItem.Purple;
					break;

				case "Node.ColorBlue":
					this.BackgroundColorItem = ColorItem.Blue;
					break;

				case "Node.ColorGreen":
					this.BackgroundColorItem = ColorItem.Green;
					break;

				case "Node.ColorGrey":
					this.BackgroundColorItem = ColorItem.Grey;
					break;


				case "Node.OpenLink":
					this.OpenLink ();
					break;

				case "Node.Comment":
					this.SwapComment ();
					break;

				case "Node.Info":
					this.SwapInfo ();
					break;

				case "Node.Delete":
					this.DeleteNode ();
					break;
			}
		}


		public override void UpdateObject()
		{
			this.UpdateAttachObject ();
		}

		public override void SetBoundsAtEnd(Point start, Point end)
		{
			Point center;

			if (start == end)
			{
				center = start;
			}
			else
			{
				center = Point.Move (end, start, -ObjectNode.frameRadius);
			}

			this.Bounds = new Rectangle (center.X-ObjectNode.frameRadius, center.Y-ObjectNode.frameRadius, ObjectNode.frameRadius*2, ObjectNode.frameRadius*2);
		}


		public override void RemoveEntityLink(LinkableObject dst, bool isContinuation)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowEdgeEntity);
			System.Diagnostics.Debug.Assert (this.Entity.Edges.Contains (dst.AbstractEntity as WorkflowEdgeEntity));

			this.Entity.Edges.Remove (dst.AbstractEntity as WorkflowEdgeEntity);
		}

		public override void AddEntityLink(LinkableObject dst, bool isContinuation)
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

		public override double GetLinkAngle(Point pos, bool isDst)
		{
			return Point.ComputeAngleDeg (this.bounds.Center, pos);
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
			switch (element)
			{
				case ActiveElement.NodeExtend:
					return this.isExtended ? "Réduit la boîte" : "Etend la boîte";

				case ActiveElement.NodeClose:
					return "<b>Supprime</b> le nœud";

				case ActiveElement.NodeComment:
					return (this.comment == null) ? "Ajoute un commentaire au nœud" : "Ferme le commentaire";

				case ActiveElement.NodeInfo:
					return (this.info == null) ? "Monte les informations du nœud" : "Ferme les informations du nœud";

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
			base.MouseMove (message, pos);

			if (this.isMouseDownForDrag && this.draggingMode == DraggingMode.None && this.HilitedElement == ActiveElement.NodeHeader)
			{
				this.draggingMode = DraggingMode.MoveObject;
				this.UpdateButtonsState ();
				this.draggingOffset = this.initialPos-this.bounds.Center;
				this.editor.Invalidate ();
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				this.DraggingMouseMove (pos);
				return true;
			}

			return false;
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if (this.HilitedElement == ActiveElement.NodeHeader && this.draggingMode == DraggingMode.None && !this.Entity.IsForeign)
			{
				this.StartEdition (this.HilitedElement);
				return;
			}

			if (this.draggingMode == DraggingMode.MoveObject)
			{
				this.editor.UpdateAfterGeometryChanged (this);
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.HilitedElement == ActiveElement.NodeExtend)
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

			if (this.HilitedElement == ActiveElement.NodeOpenLink)
			{
				this.OpenLink ();
			}

			if (this.HilitedElement == ActiveElement.NodeComment)
			{
				this.SwapComment();
			}

			if (this.HilitedElement == ActiveElement.NodeInfo)
			{
				this.SwapInfo ();
			}

			if (this.HilitedElement == ActiveElement.NodeClose)
			{
				this.DeleteNode ();
			}

			if (this.HilitedElement == ActiveElement.NodeColor1)
			{
				this.BackgroundColorItem = ColorItem.Yellow;
			}

			if (this.HilitedElement == ActiveElement.NodeColor2)
			{
				this.BackgroundColorItem = ColorItem.Orange;
			}

			if (this.HilitedElement == ActiveElement.NodeColor3)
			{
				this.BackgroundColorItem = ColorItem.Red;
			}

			if (this.HilitedElement == ActiveElement.NodeColor4)
			{
				this.BackgroundColorItem = ColorItem.Lilac;
			}

			if (this.HilitedElement == ActiveElement.NodeColor5)
			{
				this.BackgroundColorItem = ColorItem.Purple;
			}

			if (this.HilitedElement == ActiveElement.NodeColor6)
			{
				this.BackgroundColorItem = ColorItem.Blue;
			}

			if (this.HilitedElement == ActiveElement.NodeColor7)
			{
				this.BackgroundColorItem = ColorItem.Green;
			}

			if (this.HilitedElement == ActiveElement.NodeColor8)
			{
				this.BackgroundColorItem = ColorItem.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (AbstractObject.DetectRoundRectangle (this.ExtendedBounds, ObjectNode.frameRadius, pos))
			{
				return ActiveElement.NodeHeader;
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


		public override MouseCursorType MouseCursor
		{
			get
			{
				if (this.HilitedElement == ActiveElement.NodeHeader)
				{
					if (this.draggingMode == DraggingMode.MoveObject || this.Entity.IsForeign)
					{
						return MouseCursorType.Move;
					}
					else
					{
						return MouseCursorType.MoveOrEdit;
					}
				}

				return MouseCursorType.Finger;
			}
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

	
		private void SwapComment()
		{
			//	Ajoute un commentaire à la boîte.
			if (this.comment == null)
			{
				this.comment = new ObjectComment (this.editor, this.Entity);
				this.comment.AttachObject = this;

				if (!this.Entity.Description.IsNullOrWhiteSpace)
				{
					this.comment.Text = this.Entity.Description.ToString ();
				}

				Rectangle rect = new Rectangle (this.bounds.Left, this.bounds.Top+40, 200, 20);
				this.comment.Bounds = rect;
				this.comment.UpdateHeight ();  // adapte la hauteur en fonction du contenu

				this.editor.AddBalloon (this.comment);
				this.editor.UpdateAfterCommentChanged ();
			}
			else
			{
				this.comment.AttachedNodeDescription = null;
				this.editor.RemoveBalloon (this.comment);
				this.comment = null;
			}

			this.editor.SetLocalDirty ();
		}

		public void SwapInfo()
		{
			//	Ajoute une information à la boîte.
			if (this.info == null)
			{
				this.info = new ObjectInfo (this.editor, this.Entity);
				this.info.AttachObject = this;

				double width  = this.Entity.IsForeign ? 180 : 140;
				double height = this.Entity.IsForeign ? ObjectInfo.foreignHeight : ObjectInfo.lineHeight;
				Rectangle rect = new Rectangle (this.bounds.Right+40, this.bounds.Center.Y-height/2, width, height);
				this.info.Bounds = rect;
				this.info.UpdateAfterAttachChanged ();

				this.editor.AddBalloon (this.info);
				this.editor.UpdateAfterCommentChanged ();
			}
			else
			{
				this.editor.RemoveBalloon (this.info);
				this.info = null;
			}

			this.editor.SetLocalDirty ();
		}

		private void OpenLink()
		{
			//	Crée un moignon de lien o--->
			var link = new ObjectLink (this.editor, this.entity);
			link.SrcObject = this;
			link.SetStumpAngle (this.ComputeBestStumpAngle ());
			link.UpdateLink ();

			this.objectLinks.Add (link);
			this.editor.UpdateAfterGeometryChanged (null);

			this.editor.SetLocalDirty ();
		}

		private void DeleteNode()
		{
			if (!this.isRoot)
			{
				if (!this.Entity.IsPublic || this.editor.IsUnusedPublicNode (this.Entity))
				{
					string message = string.Format ("Voulez-vous supprimer le nœud \"{0}\" ?", this.Entity.Name);
					var result = Common.Dialogs.MessageDialog.ShowQuestion (message, this.editor.Window);
					if (result != Common.Dialogs.DialogResult.Yes)
					{
						return;
					}

					if (this.comment != null)
					{
						this.SwapComment ();  // ferme le commentaire
					}

					if (this.info != null)
					{
						this.SwapInfo ();  // ferme le noeud
					}

					this.editor.CloseObject (this);
					this.editor.UpdateAfterGeometryChanged (null);

					this.editor.SetLocalDirty ();
				}
				else
				{
					Common.Dialogs.MessageDialog.ShowMessage ("Il n'est pas possible de supprimer ce nœud public,<br/>car il est utilisé dans d'autres workflows !", this.editor.Window);
				}
			}
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
			base.DrawBackground (graphics);

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
				rect.Offset (AbstractObject.shadowOffset, -(AbstractObject.shadowOffset));
				this.DrawRoundShadow (graphics, rect, ObjectNode.frameRadius, (int) AbstractObject.shadowOffset, 0.2);

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
			rect.Offset (AbstractObject.shadowOffset, -(AbstractObject.shadowOffset));
			this.DrawNodeShadow (graphics, rect, (int) AbstractObject.shadowOffset, 0.2);

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
			rect.Deflate (3);
			this.title.LayoutSize = rect.Size;
			double length = this.title.GetSingleLineSize ().Width;
			double zoom = System.Math.Max (System.Math.Min (rect.Width/length/1.5, 1), 0.5);
			var zoomRect = new Rectangle (rect.Center.X-rect.Width/2/zoom, rect.Center.Y-rect.Height/2/zoom+2, rect.Width/zoom, rect.Height/zoom);
			this.title.LayoutSize = zoomRect.Size;

			var t = graphics.Transform;
			graphics.Transform = graphics.Transform.MultiplyByPostfix (Transform.CreateScaleTransform (zoom, zoom, rect.Center.X, rect.Center.Y));
			this.title.Paint (zoomRect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);
			graphics.Transform = t;

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, this.isRoot ? 6 : 2);
			graphics.RenderSolid (colorFrame);

			if (this.Entity.IsPublic)
			{
				double offset = 5.0;
				double width = 4;

				if (this.isRoot)
				{
					offset += 2.0;
				}

				var circlePath = new Path ();
				circlePath.AppendCircle (this.bounds.Center, ObjectNode.frameRadius + offset);

				graphics.PaintDashedOutline (circlePath, width, 5, 8.4, CapStyle.Square, colorFrame);
			}

			if (this.Entity.IsForeign)
			{
				double offset = 2.5;
				double width = 1;

				if (this.isRoot)
				{
					offset += 2.0;
				}

				var circlePath = new Path ();
				circlePath.AppendCircle (this.bounds.Center, ObjectNode.frameRadius + offset);

				graphics.PaintDashedOutline (circlePath, width, 5, 5, CapStyle.Square, colorFrame);
			}
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine tous les boutons.
			base.DrawForeground (graphics);
		}

		protected override void DrawAsOriginForMagnetConstrain(Graphics graphics)
		{
			//	Dessine l'objet comme étant l'origine d'une contrainte.
			var rect = this.bounds;
			rect.Deflate (1);
			Path path = this.PathNodeRectangle (rect);

			graphics.Rasterizer.AddOutline (path, 3);
			graphics.RenderSolid (Color.FromRgb (1, 0, 0));  // rouge
		}


		private bool HasStump
		{
			get
			{
				foreach (var link in this.objectLinks)
				{
					if (link.DstObject == null)
					{
						return true;
					}
				}

				return false;
			}
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
				rect.Deflate (-10, 15);

				return rect;
			}
		}

		private Point PositionExtendButton
		{
			//	Retourne la position du bouton.
			get
			{
				Point c = this.bounds.Center;
				double a = this.ComputeBestStumpAngle ();

				return Transform.RotatePointDeg (c, a+52, new Point (c.X+ObjectNode.frameRadius, c.Y));
			}
		}

		private Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point (this.bounds.Center.X-ActiveButton.buttonRadius-1, this.bounds.Bottom-ObjectNode.extendedHeight+ActiveButton.buttonRadius*3+4);
			}
		}

		private Point PositionInfoButton
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

		private void UpdateButtonGeometryColor(ActiveButton button)
		{
			int rank = button.Element - ActiveElement.NodeColor1;

			button.Center = this.PositionColorButton (rank);
		}

		private void UpdateButtonStateOpenLink(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Enable = this.IsHeaderHilite && !this.IsDragging && !this.Entity.IsForeign && !this.HasStump;
			button.State.Visible = button.State.Enable;
		}

		private void UpdateButtonStateExtend(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.Glyph = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
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


		protected override void UpdateMagnetConstrains()
		{
			this.magnetConstrains[0].Position = this.bounds.Center.X;
			this.magnetConstrains[1].Position = this.bounds.Center.Y;
		}

	
		#region Serialize
		public override void Serialize(XElement xml)
		{
			base.Serialize (xml);

			xml.Add (new XAttribute ("IsRoot", this.isRoot));
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);

			this.isRoot = (bool) xml.Attribute ("IsRoot");
		}
		#endregion


		private static readonly double			frameRadius = 25;
		private static readonly double			extendedHeight = 70;

		private bool							isRoot;
		private TextLayout						title;

		private AbstractTextField				editingTextField;
	}
}
