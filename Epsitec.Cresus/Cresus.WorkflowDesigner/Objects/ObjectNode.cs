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
			this.title.DefaultFontSize = 24;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.isRoot = false;

			if (this.Entity.Name.IsNullOrWhiteSpace)
			{
				this.TitleNumber = this.editor.GetNodeTitleNumbrer ();
			}

			this.UpdateTitle ();
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

		public override void Move(double dx, double dy)
		{
			//	Déplace l'objet.
			this.bounds.Offset(dx, dy);
		}

		public override void CreateLinks()
		{
			this.objectLinks.Clear ();

			foreach (var entityEdge in this.Entity.Edges)
			{
				var link = new ObjectLink (this.editor, this.Entity);
				link.SrcObject = this;
				link.DstObject = this.editor.SearchObject (entityEdge);

				this.objectLinks.Add (link);
			}
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


		public override Vector GetLinkVector(LinkAnchor anchor, Point dstPos, bool isDst)
		{
			Point p1 = Point.Move (this.bounds.Center, dstPos, ObjectNode.frameRadius);
			Point p2 = Point.Move (this.bounds.Center, dstPos, ObjectNode.frameRadius+1);

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
			this.hilitedElement = ActiveElement.None;
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextField);
			this.editingTextField = null;

			this.editor.EditingObject = null;
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			if (this.isDragging)
			{
				return null;  // pas de tooltip
			}

			return base.GetToolTipText(element);
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

			if (this.hilitedElement == ActiveElement.NodeHeader && this.editor.NodeCount > 1)
			{
				this.isDragging = true;
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
				this.editor.LockObject (null);
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.hilitedElement == ActiveElement.NodeOpenLink)
			{
				//	Crée un moignon de lien o--->
				var link = new ObjectLink (this.editor, this.entity);
				link.SrcObject = this;
				link.StumpAngle = this.ComputeBestStumpAngle ();
				link.UpdateLink ();

				this.objectLinks.Add (link);
				this.editor.UpdateAfterGeometryChanged (null);
			}

			if (this.hilitedElement == ActiveElement.NodeClose)
			{
				if (!this.isRoot)
				{
					this.editor.CloseObject(this);
					this.editor.UpdateAfterGeometryChanged (null);
				}
			}

			if (this.hilitedElement == ActiveElement.NodeComment)
			{
				this.AddComment();
			}

			if (this.hilitedElement == ActiveElement.NodeColor1)
			{
				this.BackgroundMainColor = MainColor.Yellow;
			}

			if (this.hilitedElement == ActiveElement.NodeColor2)
			{
				this.BackgroundMainColor = MainColor.Orange;
			}

			if (this.hilitedElement == ActiveElement.NodeColor3)
			{
				this.BackgroundMainColor = MainColor.Red;
			}

			if (this.hilitedElement == ActiveElement.NodeColor4)
			{
				this.BackgroundMainColor = MainColor.Lilac;
			}

			if (this.hilitedElement == ActiveElement.NodeColor5)
			{
				this.BackgroundMainColor = MainColor.Purple;
			}

			if (this.hilitedElement == ActiveElement.NodeColor6)
			{
				this.BackgroundMainColor = MainColor.Blue;
			}

			if (this.hilitedElement == ActiveElement.NodeColor7)
			{
				this.BackgroundMainColor = MainColor.Green;
			}

			if (this.hilitedElement == ActiveElement.NodeColor8)
			{
				this.BackgroundMainColor = MainColor.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			ActiveElement element = base.MouseDetectBackground (pos);
			if (element != ActiveElement.None)
			{
				return element;
			}

			if (this.bounds.Contains (pos))
			{
				return ActiveElement.NodeHeader;
			}

			if (this.bounds.Contains (pos))
			{
				return ActiveElement.NodeInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			ActiveElement element = base.MouseDetectForeground (pos);
			if (element != ActiveElement.None)
			{
				return element;
			}

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				//	Détection dans l'ordre inverse du dessin !
				foreach (var b in this.ActiveButtons.Reverse ())
				{
					if (b.roundButton)
					{
						if (this.DetectRoundButton (b.pos, pos))
						{
							return b.element;
						}
					}
					else
					{
						if (this.DetectSquareButton (b.pos, pos))
						{
							return b.element;
						}
					}
				}
			}

			return ActiveElement.None;
		}

		public override bool IsMousePossible(ActiveElement element)
		{
			//	Indique si l'opération est possible.
			return true;
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
				rect.Left = rect.Right+30;
				rect.Width = System.Math.Max (this.bounds.Width, AbstractObject.infoMinWidth);
				this.comment.SetBounds (rect);
				this.comment.UpdateHeight ();  // adapte la hauteur en fonction du contenu

				this.editor.AddComment (this.comment);
				this.editor.UpdateAfterCommentChanged ();

				this.comment.EditComment ();  // édite tout de suite le texte du commentaire
			}
			else
			{
				this.comment.IsVisible = !this.comment.IsVisible;
			}

			this.editor.SetLocalDirty ();
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
			Rectangle rect;

			bool dragging = (this.hilitedElement == ActiveElement.NodeHeader || this.isHilitedForLinkChanging);

			//	Dessine l'ombre.
			rect = this.bounds;
			if (this.isRoot)
			{
				rect.Inflate(2);
			}
			rect.Offset(ObjectNode.shadowOffset, -(ObjectNode.shadowOffset));
			this.DrawNode2Shadow (graphics, rect, (int) ObjectNode.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathNode2Rectangle (rect);

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

			//	Dessine le titre.
			Color titleColor = dragging ? this.GetColor(1) : this.GetColor(0);

			rect = this.bounds;
			rect.Offset (0, 2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, this.isRoot ? 6 : 2);
			graphics.RenderSolid (colorFrame);
		}

		public override void DrawForeground(Graphics graphics)
		{
			//	Dessine tous les boutons.
			foreach (var b in this.ActiveButtons)
			{
				if (b.glyph != GlyphShape.None)
				{
					if (this.hilitedElement == b.element)
					{
						this.DrawRoundButton (graphics, b.pos, AbstractObject.buttonRadius, b.glyph, true, false, b.enable);
					}
					else if (this.IsHeaderHilite && !this.isDragging)
					{
						this.DrawRoundButton (graphics, b.pos, AbstractObject.buttonRadius, b.glyph, false, false, b.enable);
					}
				}
				else if (b.text != null)
				{
					if (this.hilitedElement == b.element)
					{
						this.DrawRoundButton (graphics, b.pos, AbstractObject.buttonRadius, b.text, true, false, b.enable);
					}
					else if (this.IsHeaderHilite && !this.isDragging)
					{
						this.DrawRoundButton (graphics, b.pos, AbstractObject.buttonRadius, b.text, false, false, b.enable);
					}
				}
				else if (b.color != MainColor.None)
				{
					if (this.hilitedElement == b.element)
					{
						this.DrawSquareButton (graphics, b.pos, b.color, this.boxColor == b.color, true);
					}
					else if (this.IsHeaderHilite && !this.isDragging)
					{
						this.DrawSquareButton (graphics, b.pos, b.color, this.boxColor == b.color, false);
					}
				}
			}
		}


		private IEnumerable<ActiveButton> ActiveButtons
		{
			get
			{
				yield return new ActiveButton (ActiveElement.NodeOpenLink, this.PositionOpenLinkButton, GlyphShape.Plus);
				yield return new ActiveButton (ActiveElement.NodeClose,    this.PositionCloseButton,    GlyphShape.Close, !this.isRoot);
				yield return new ActiveButton (ActiveElement.LinkComment,  this.PositionCommentButton,  "C");

				yield return new ActiveButton (ActiveElement.NodeColor1, this.PositionColorButton (0), MainColor.Yellow);
				yield return new ActiveButton (ActiveElement.NodeColor2, this.PositionColorButton (1), MainColor.Orange);
				yield return new ActiveButton (ActiveElement.NodeColor3, this.PositionColorButton (2), MainColor.Red);
				yield return new ActiveButton (ActiveElement.NodeColor4, this.PositionColorButton (3), MainColor.Lilac);
				yield return new ActiveButton (ActiveElement.NodeColor5, this.PositionColorButton (4), MainColor.Purple);
				yield return new ActiveButton (ActiveElement.NodeColor6, this.PositionColorButton (5), MainColor.Blue);
				yield return new ActiveButton (ActiveElement.NodeColor7, this.PositionColorButton (6), MainColor.Green);
				yield return new ActiveButton (ActiveElement.NodeColor8, this.PositionColorButton (7), MainColor.Grey);
			}
		}

		private class ActiveButton
		{
			public ActiveButton(ActiveElement element, Point pos, GlyphShape glyph, bool enable = true)
			{
				this.element     = element;
				this.pos         = pos;
				this.glyph       = glyph;
				this.color       = MainColor.None;
				this.enable      = enable;
				this.roundButton = true;
			}

			public ActiveButton(ActiveElement element, Point pos, string text, bool enable = true)
			{
				this.element     = element;
				this.pos         = pos;
				this.glyph       = GlyphShape.None;
				this.text        = text;
				this.color       = MainColor.None;
				this.enable      = enable;
				this.roundButton = true;
			}

			public ActiveButton(ActiveElement element, Point pos, MainColor color, bool enable = true)
			{
				this.element     = element;
				this.pos         = pos;
				this.color       = color;
				this.enable      = enable;
				this.roundButton = false;
			}

			public ActiveElement	element;
			public Point			pos;
			public bool				enable;
			public bool				roundButton;
			public GlyphShape		glyph;
			public string			text;
			public MainColor		color;
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
						this.hilitedElement == ActiveElement.NodeComment ||
						this.hilitedElement == ActiveElement.NodeColor1 ||
						this.hilitedElement == ActiveElement.NodeColor2 ||
						this.hilitedElement == ActiveElement.NodeColor3 ||
						this.hilitedElement == ActiveElement.NodeColor4 ||
						this.hilitedElement == ActiveElement.NodeColor5 ||
						this.hilitedElement == ActiveElement.NodeColor6 ||
						this.hilitedElement == ActiveElement.NodeColor7 ||
						this.hilitedElement == ActiveElement.NodeColor8 ||
						this.hilitedElement == ActiveElement.NodeOpenLink ||
						this.hilitedElement == ActiveElement.NodeClose);
			}
		}

		private Rectangle RectangleEditName
		{
			get
			{
				Rectangle rect = this.bounds;
				rect.Deflate (15, 20);

				return rect;
			}
		}

		private Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point (this.bounds.Center.X-AbstractObject.buttonRadius-1, this.bounds.Top-AbstractObject.buttonRadius-9);
			}
		}

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Center.X+AbstractObject.buttonRadius+1, this.bounds.Top-AbstractObject.buttonRadius-9);
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
			double y = this.bounds.Bottom;

			Point p = this.PositionOpenLinkButton;
			if (!p.IsZero)
			{
				//	Sous le bouton 'OpenLink', avec un petit chevauchement.
				y = System.Math.Min (y, p.Y-AbstractObject.buttonRadius-2);
			}

			return new Point (this.bounds.Center.X + (2*AbstractObject.buttonSquare+1)*(rank-3.5) + 0.5, y);
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


		public static readonly double			frameRadius = 30;
		private static readonly double			shadowOffset = 6;

		private bool							isRoot;
		private bool							isConnectedToRoot;
		private TextLayout						title;

		private bool							isDragging;
		private Point							initialPos;

		private AbstractTextField				editingTextField;
	}
}
