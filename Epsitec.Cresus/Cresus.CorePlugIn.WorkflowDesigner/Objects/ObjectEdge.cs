//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	/// <summary>
	/// Objet ayant la forme d'un rectangle allongé avec les extrémités gauche et droite arrondies.
	/// </summary>
	public class ObjectEdge : LinkableObject
	{
		public ObjectEdge(Editor editor, AbstractEntity entity)
			: base (editor, entity)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);

			this.title = new TextLayout ();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.subtitles = new List<TextLayout> ();

			this.description = new TextLayout ();
			this.description.DefaultFontSize = 9;
			this.description.Alignment = ContentAlignment.MiddleLeft;
			this.description.BreakMode = TextBreakMode.Hyphenate;

			this.editingTextFieldCombos = new List<TextFieldCombo> ();

			this.UpdateTitle ();
			this.UpdateSubtitle ();
			this.UpdateDescription ();

			this.magnetConstrains.Add (new MagnetConstrain (isVertical: true));
			this.magnetConstrains.Add (new MagnetConstrain (isVertical: false));

			this.Bounds = new Rectangle (Point.Zero, new Size (ObjectEdge.frameSize.Width, this.RequiredHeight));

			//	Crée la liaison unique.
			this.CreateInitialLinks ();
		}


		private string Title
		{
			//	Titre au sommet de la boîte (nom du noeud).
			get
			{
				return this.titleString;
			}
			set
			{
				if (this.titleString != value)
				{
					this.titleString = value;
					this.title.Text = Misc.Bold (this.titleString);
				}
			}
		}

		private string Subtitle
		{
			//	Sous-titre au sommet de la boîte (nom de l'action).
			get
			{
				return this.subtitleString;
			}
			set
			{
				if (this.subtitleString != value)
				{
					this.subtitleString = value;

					this.subtitles.Clear ();

					if (!string.IsNullOrEmpty (this.subtitleString))
					{
						var list = this.subtitleString.Split ('\n');

						foreach (var part in list)
						{
							var subtitle = new TextLayout ();
							subtitle.DefaultFontSize = 9;
							subtitle.Alignment = ContentAlignment.MiddleCenter;
							subtitle.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
							subtitle.Text = part;

							this.subtitles.Add (subtitle);
						}
					}
				}
			}
		}

		private string Description
		{
			//	Titre au sommet de la boîte (nom du noeud).
			get
			{
				return this.descriptionString;
			}
			set
			{
				if (this.descriptionString != value)
				{
					this.descriptionString = value;
					this.description.Text = this.descriptionString;
				}
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
					box = new Rectangle (box.Left, box.Bottom-ObjectEdge.extendedHeight, box.Width, box.Height+ObjectEdge.extendedHeight);
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
	
			var link = new ObjectLink (this.editor, this.Entity);
			link.SrcObject = this;
			link.DstObject = this.editor.SearchInitialObject (this.Entity.NextNode);  // null si n'existe pas (et donc moignon o--->)

			if (link.DstObject == null)
			{
				link.SetStumpAngle (0);
			}

			this.objectLinks.Add (link);
		}

		private void CreateContinuationLink()
		{
			if (this.Entity.TransitionType == WorkflowTransitionType.Call)
			{
				if (this.objectLinks.Count == 1)
				{
					var link = new ObjectLink (this.editor, this.Entity);
					link.IsContinuation = true;
					link.SrcObject = this;
					link.DstObject = this.editor.SearchInitialObject (this.Entity.Continuation);  // null si n'existe pas (et donc moignon o--->)

					if (link.DstObject == null)
					{
						link.SetStumpAngle (270);
					}

					this.objectLinks.Add (link);
				}
			}
			else
			{
				this.Entity.Continuation = null;

				if (this.objectLinks.Count == 2)
				{
					this.objectLinks.RemoveAt (1);
				}
			}
		}


		public override void ContextMenu()
		{
			this.editor.CreateMenuItem (this.Entity.TransitionType == WorkflowTransitionType.Default, "Normal", "Edge.Normal");
			this.editor.CreateMenuItem (this.Entity.TransitionType == WorkflowTransitionType.Call,    "Call",   "Edge.Call");
			this.editor.CreateMenuItem (this.Entity.TransitionType == WorkflowTransitionType.Fork,    "Fork",   "Edge.Fork");

			this.editor.CreateMenuSeparator ();
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Yellow, "Jaune",  "Edge.ColorYellow");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Orange, "Orange", "Edge.ColorOrange");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Red,    "Rouge",  "Edge.ColorRed");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Lilac,  "Lilas",  "Edge.ColorLilac");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Purple, "Violet", "Edge.ColorPurple");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Blue,   "Bleu",   "Edge.ColorBlue");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Green,  "Vert",   "Edge.ColorGreen");
			this.editor.CreateMenuItem (this.colorFactory.ColorItem == ColorItem.Grey,   "Gris",   "Edge.ColorGrey");

			if (this.IsButtonEnable (ActiveElement.EdgeComment))
			{
				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (null, this.comment == null ? "Ajoute un commentaire" : "Ferme le commentaire", "Edge.Comment");
			}

			if (this.IsButtonEnable (ActiveElement.EdgeClose))
			{
				this.editor.CreateMenuSeparator ();
				this.editor.CreateMenuItem (null, "Supprime la transition...", "Edge.Delete");
			}
		}

		public override void MenuAction(string name)
		{
			switch (name)
			{
				case "Edge.Normal":
					this.Entity.TransitionType = WorkflowTransitionType.Default;
					this.CreateContinuationLink ();
					this.editor.SetLocalDirty ();
					break;

				case "Edge.Call":
					this.Entity.TransitionType = WorkflowTransitionType.Call;
					this.CreateContinuationLink ();
					this.editor.SetLocalDirty ();
					break;

				case "Edge.Fork":
					this.Entity.TransitionType = WorkflowTransitionType.Fork;
					this.CreateContinuationLink ();
					this.editor.SetLocalDirty ();
					break;


				case "Edge.ColorYellow":
					this.BackgroundColorItem = ColorItem.Yellow;
					break;

				case "Edge.ColorOrange":
					this.BackgroundColorItem = ColorItem.Orange;
					break;

				case "Edge.ColorRed":
					this.BackgroundColorItem = ColorItem.Red;
					break;

				case "Edge.ColorLilac":
					this.BackgroundColorItem = ColorItem.Lilac;
					break;

				case "Edge.ColorPurple":
					this.BackgroundColorItem = ColorItem.Purple;
					break;

				case "Edge.ColorBlue":
					this.BackgroundColorItem = ColorItem.Blue;
					break;

				case "Edge.ColorGreen":
					this.BackgroundColorItem = ColorItem.Green;
					break;

				case "Edge.ColorGrey":
					this.BackgroundColorItem = ColorItem.Grey;
					break;


				case "Edge.Comment":
					this.SwapComment ();
					break;

				case "Edge.Delete":
					this.DeleteEdge ();
					break;
			}
		}


		public override void SetBoundsAtEnd(Point start, Point end)
		{
			double h = this.RequiredHeight;
			double a = Point.ComputeAngleRad (start, end);
			double d = System.Math.Abs (ObjectEdge.frameSize.Width*System.Math.Cos (a) + h*System.Math.Sin (a)) * 0.5;

			Point center = Point.Move (end, start, -d);
			Rectangle rect = new Rectangle (center.X-ObjectEdge.frameSize.Width/2, center.Y-h/2, ObjectEdge.frameSize.Width, h);

			this.Bounds = rect;
		}


		public override void RemoveEntityLink(LinkableObject dst, bool isContinuation)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowNodeEntity);

			if (isContinuation)
			{
				System.Diagnostics.Debug.Assert (this.Entity.Continuation == dst.AbstractEntity as WorkflowNodeEntity);

				this.Entity.Continuation = null;
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.Entity.NextNode == dst.AbstractEntity as WorkflowNodeEntity);

				this.Entity.NextNode = null;
			}
		}

		public override void AddEntityLink(LinkableObject dst, bool isContinuation)
		{
			System.Diagnostics.Debug.Assert (dst.AbstractEntity is WorkflowNodeEntity);

			if (isContinuation)
			{
				this.Entity.Continuation = dst.AbstractEntity as WorkflowNodeEntity;
			}
			else
			{
				this.Entity.NextNode = dst.AbstractEntity as WorkflowNodeEntity;
			}
		}


		public override Vector GetLinkVector(double angle, bool isDst)
		{
			double xMargin = System.Math.Min (this.bounds.Height/2 * (isDst ? 2.5 : 1.5), this.bounds.Width/2);
			double yMargin = this.bounds.Height/2;

			Point c = this.bounds.Center;
			Point p = Transform.RotatePointDeg (c, angle, new Point (c.X+this.bounds.Width+this.bounds.Height, c.Y));

			Segment s = this.GetIntersect (c, p);
			if (s != null)
			{
				if (s.anchor == LinkAnchor.Left || s.anchor == LinkAnchor.Right)
				{
					s.intersection.Y = System.Math.Max (s.intersection.Y, s.p1.Y+yMargin);
					s.intersection.Y = System.Math.Min (s.intersection.Y, s.p2.Y-yMargin);
				}

				if (s.anchor == LinkAnchor.Bottom || s.anchor == LinkAnchor.Top)
				{
					s.intersection.X = System.Math.Max (s.intersection.X, s.p1.X+xMargin);
					s.intersection.X = System.Math.Min (s.intersection.X, s.p2.X-xMargin);
				}

				Point i = new Point (System.Math.Floor (s.intersection.X), System.Math.Floor (s.intersection.Y));

				switch (s.anchor)
				{
					case LinkAnchor.Left:
						return new Vector (i, new Size (-1, 0));

					case LinkAnchor.Right:
						return new Vector (i, new Size (1, 0));

					case LinkAnchor.Bottom:
						return new Vector (i, new Size (0, -1));

					case LinkAnchor.Top:
						return new Vector (i, new Size (0, 1));
				}
			}

			return Vector.Zero;
		}

		public override Point GetLinkStumpPos(double angle)
		{
			Point c = this.bounds.Center;
			Point p = Transform.RotatePointDeg (c, angle, new Point (c.X+this.bounds.Width+this.bounds.Height, c.Y));

			Segment s = this.GetIntersect (c, p);
			if (s != null)
			{
				return Point.Move (s.intersection, c, -AbstractObject.lengthStumpLink);
			}

			return this.bounds.Center;
		}

		public override double GetLinkAngle(Point pos, bool isDst)
		{
			double xMargin = this.bounds.Height/2 * (isDst ? 2.5 : 1.5);
			double yMargin = this.bounds.Height/2;

			if (pos.Y >= this.bounds.Bottom && pos.Y <= this.bounds.Top)
			{
				pos.Y = System.Math.Max (pos.Y, this.bounds.Bottom+yMargin);
				pos.Y = System.Math.Min (pos.Y, this.bounds.Top   -yMargin);

				if (pos.X < this.bounds.Left)
				{
					pos.X = this.bounds.Left;
				}

				if (pos.X > this.bounds.Right)
				{
					pos.X = this.bounds.Right;
				}
			}
			else
			{
				pos.X = System.Math.Max (pos.X, this.bounds.Left +xMargin);
				pos.X = System.Math.Min (pos.X, this.bounds.Right-xMargin);

				if (pos.Y < this.bounds.Bottom)
				{
					pos.Y = this.bounds.Bottom;
				}

				if (pos.Y > this.bounds.Top)
				{
					pos.Y = this.bounds.Top;
				}
			}

			return Point.ComputeAngleDeg (this.bounds.Center, pos);
		}

		private Segment GetIntersect(Point c, Point p)
		{
			Point i;

			i = Geometry.IsIntersect (c, p, this.bounds.BottomRight, this.bounds.TopRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomRight, this.bounds.TopRight, LinkAnchor.Right);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.BottomLeft, this.bounds.BottomRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomLeft, this.bounds.BottomRight, LinkAnchor.Bottom);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.BottomLeft,  this.bounds.TopLeft);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.BottomLeft, this.bounds.TopLeft, LinkAnchor.Left);
			}

			i = Geometry.IsIntersect (c, p, this.bounds.TopLeft, this.bounds.TopRight);
			if (!i.IsZero)
			{
				return new Segment (i, this.bounds.TopLeft, this.bounds.TopRight, LinkAnchor.Top);
			}

			return null;
		}

		private class Segment
		{
			public Segment(Point intersection, Point p1, Point p2, LinkAnchor anchor)
			{
				this.intersection = intersection;
				this.p1           = p1;
				this.p2           = p2;
				this.anchor       = anchor;
			}

			public Point		intersection;
			public Point		p1;
			public Point		p2;
			public LinkAnchor	anchor;
		}


		public override void AcceptEdition()
		{
			if (this.editingElement == ActiveElement.EdgeHeader)
			{
				this.Entity.Name              = this.editingTextFieldPrimary.Text;
				this.Entity.Labels            = this.editingTextFieldSecondary.Text;
				this.Entity.TransitionActions = ObjectEdge.GetTransitionActions (this.editingTextFieldCombos);
				this.UpdateTitle ();
				this.UpdateSubtitle ();

				double h = this.RequiredHeight;

				if (this.bounds.Height != h)
				{
					this.Bounds = new Rectangle (this.bounds.Left, this.bounds.Top-h, this.bounds.Width, h);
					this.editor.UpdateAfterGeometryChanged (this);
				}
			}

			if (this.editingElement == ActiveElement.EdgeEditDescription)
			{
				this.Entity.Description = this.editingTextFieldPrimary.Text;
				this.UpdateDescription ();
			}

			this.editor.UpdateObjects ();
			this.StopEdition ();
		}

		public override void CancelEdition()
		{
			this.StopEdition ();
		}

		public override void StartEdition()
		{
			this.StartEdition (ActiveElement.EdgeHeader);
		}

		private void StartEdition(ActiveElement element)
		{
			Rectangle rect = Rectangle.Empty;
			double height = 20;
			string text = null;
			var comboTexts = new List<string> ();

			if (element == ActiveElement.EdgeHeader)
			{
				rect = this.RectangleTitle;
				rect.Deflate (-4, 4);

				text = this.Entity.Name.ToString ();
				comboTexts = ObjectEdge.GetTransitionActions (this.Entity.TransitionActions);

				this.editingTextFieldPrimary = new TextField ();
				ToolTip.Default.SetToolTip (this.editingTextFieldPrimary, "Nom logique de la transition<br/><i>Champ \"Name\"</i>");

				this.editingTextFieldSecondary = new TextFieldMulti ();
				(this.editingTextFieldSecondary as TextFieldMulti).ScrollerVisibility = false;
				ToolTip.Default.SetToolTip (this.editingTextFieldSecondary, "Diverses variantes de textes pour l'interface grahpique (une par ligne)<br/><i>Champ \"Labels\"</i>");

				for (int i = 0; i < comboTexts.Count; i++)
				{
					this.editingTextFieldCombos.Add (new TextFieldCombo ());
				}
			}

			if (element == ActiveElement.EdgeEditDescription)
			{
				rect = this.RectangleDescription;
				rect.Inflate (6);
				height = 5+15*4;  // place pour 4 lignes

				text = this.Entity.Description.ToString ();

				this.editingTextFieldPrimary = new TextFieldMulti ();
				(this.editingTextFieldPrimary as TextFieldMulti).ScrollerVisibility = false;
				ToolTip.Default.SetToolTip (this.editingTextFieldPrimary, "Description de la transition<br/><i>Champ \"Description\"</i>");
			}

			if (rect.IsEmpty)
			{
				return;
			}

			this.editingElement = element;

			Point p1 = this.editor.ConvEditorToWidget (rect.TopLeft);
			Point p2 = this.editor.ConvEditorToWidget (rect.BottomRight);
			double width  = System.Math.Max (p2.X-p1.X, 150);

			rect = new Rectangle (new Point (p1.X, p1.Y-height), new Size (width, height));

			this.editingTextFieldPrimary.Parent = this.editor;
			this.editingTextFieldPrimary.SetManualBounds (rect);
			this.editingTextFieldPrimary.Text = text;
			this.editingTextFieldPrimary.TabIndex = 1;
			this.editingTextFieldPrimary.SelectAll ();
			this.editingTextFieldPrimary.Focus ();

			if (this.editingTextFieldSecondary != null)
			{
				height = 5+15*3;  // place pour 3 lignes
				rect = new Rectangle (rect.Left, rect.Bottom+1-height, rect.Width, height);

				this.editingTextFieldSecondary.Parent = this.editor;
				this.editingTextFieldSecondary.SetManualBounds (rect);
				this.editingTextFieldSecondary.Text = this.Entity.Labels.ToString ();
				this.editingTextFieldSecondary.TabIndex = 2;
			}

			height = 20;
			rect = new Rectangle (rect.Left, rect.Bottom-2-height, rect.Width, height);

			for (int i = 0; i < comboTexts.Count; i++)
			{
				this.editingTextFieldCombos[i].Parent = this.editor;
				this.editingTextFieldCombos[i].SetManualBounds (rect);
				this.editingTextFieldCombos[i].IsReadOnly = true;
				this.editingTextFieldCombos[i].TabIndex = 3+i;

				this.UpdateTransitionActionCombo (this.editingTextFieldCombos[i], comboTexts[i], i);

				rect.Offset (0, -rect.Height+1);
			}

			this.editor.EditingObject = this;
			this.editor.ClearHilited ();
			this.UpdateButtonsState ();
		}

		private void StopEdition()
		{
			this.editor.Children.Remove (this.editingTextFieldPrimary);
			this.editingTextFieldPrimary = null;

			if (this.editingTextFieldSecondary != null)
			{
				this.editor.Children.Remove (this.editingTextFieldSecondary);
				this.editingTextFieldSecondary = null;
			}

			foreach (var combo in this.editingTextFieldCombos)
			{
				this.editor.Children.Remove (combo);
			}

			this.editingTextFieldCombos.Clear ();

			this.editor.EditingObject = null;
			this.UpdateButtonsState ();
		}


		private void UpdateTransitionActionCombo(TextFieldCombo combo, string text, int rank)
		{
			//	Initialise le widget combo permettant de choisir une action.
			combo.Items.Clear ();
			combo.Items.Add (ObjectEdge.emptyTransitionAction);

			var actionClasses = WorkflowActionResolver.GetActionClasses ();
			foreach (var actionClasse in actionClasses)
			{
				var verbs = WorkflowActionResolver.GetActionVerbs (actionClasse).Select (x => x.Name);
				foreach (var verb in verbs)
				{
					combo.Items.Add (string.Concat (actionClasse, ".", verb));
				}
			}

			if (string.IsNullOrWhiteSpace (text))
			{
				text = ObjectEdge.emptyTransitionAction;
			}

			combo.Text = string.Concat (ObjectEdge.subtitlePrefix, (rank+1).ToString (), ObjectEdge.subtitlePostfix, text);

			combo.ComboClosed += delegate
			{
				this.AcceptEdition ();
				this.StartEdition ();
			};
		}

		private static List<string> GetTransitionActions(string actions)
		{
			var list = new List<string> ();

			if (!string.IsNullOrEmpty (actions))
			{
				list.AddRange (actions.Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
			}

			list.Add("");  // pour avoir une ligne "<vide>"

			return list;
		}

		private static string GetTransitionActions(List<TextFieldCombo> combos)
		{
			//	Retourne l'action choisie par les widgets combo.
			var list = new List<string> ();

			foreach (var combo in combos)
			{
				string text = combo.Text;

				if (!string.IsNullOrEmpty (text))
				{
					int index = text.IndexOf (ObjectEdge.subtitlePostfix);
					if (index != -1)
					{
						text = text.Substring (index+ObjectEdge.subtitlePostfix.Length);  // supprime le "n) " au début
					}
				}

				if (!string.IsNullOrWhiteSpace (text) && text != ObjectEdge.emptyTransitionAction)
				{
					list.Add (text);
				}
			}

			return string.Join ("\n", list);
		}


		private double RequiredHeight
		{
			get
			{
				return ObjectEdge.frameSize.Height + (this.RequiredTransitionLines-1)*ObjectEdge.subtitleHeight;
			}
		}

		private int RequiredTransitionLines
		{
			get
			{
				int linesCount = 1;

				if (!string.IsNullOrEmpty (this.Entity.TransitionActions))
				{
					linesCount = this.Entity.TransitionActions.Count (x => x == '\n') + 1;
				}

				return linesCount;
			}
		}


		protected override string GetToolTipText(ActiveElement element)
		{
			//	Retourne le texte pour le tooltip.
			switch (element)
			{
				case ActiveElement.EdgeChangeWidth:
					return "Change la largeur de la boîte";

				case ActiveElement.EdgeClose:
					return "<b>Supprime</b> la transition";

				case ActiveElement.EdgeComment:
					return (this.comment == null) ? "Ajoute un commentaire à la transition" : "Ferme le commentaire";

				case ActiveElement.EdgeExtend:
					return this.isExtended ? "Réduit la boîte" : "Etend la boîte";

				case ActiveElement.EdgeColor1:
					return "Jaune";

				case ActiveElement.EdgeColor2:
					return "Orange";

				case ActiveElement.EdgeColor3:
					return "Rouge";

				case ActiveElement.EdgeColor4:
					return "Lilas";

				case ActiveElement.EdgeColor5:
					return "Violet";

				case ActiveElement.EdgeColor6:
					return "Bleu";

				case ActiveElement.EdgeColor7:
					return "Vert";

				case ActiveElement.EdgeColor8:
					return "Gris";
			}

			return base.GetToolTipText (element);
		}

		public override bool MouseMove(Message message, Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			base.MouseMove (message, pos);

			if (this.isMouseDownForDrag && this.draggingMode == DraggingMode.None && this.HilitedElement == ActiveElement.EdgeHeader)
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

			if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.ChangeBoundsWidth (pos.X, 120);
				this.editor.UpdateLinks ();
				return true;
			}

			return false;
		}

		public override void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			base.MouseDown (message, pos);

			if (this.HilitedElement == ActiveElement.EdgeChangeWidth)
			{
				this.draggingMode = DraggingMode.ChangeWidth;
				this.UpdateButtonsState ();
			}
		}

		public override void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			base.MouseUp (message, pos);

			if ((this.HilitedElement == ActiveElement.EdgeHeader ||
				 this.HilitedElement == ActiveElement.EdgeEditDescription) &&
				this.draggingMode == DraggingMode.None)
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

			if (this.draggingMode == DraggingMode.ChangeWidth)
			{
				this.editor.UpdateAfterGeometryChanged (this);
				this.draggingMode = DraggingMode.None;
				this.UpdateButtonsState ();
				this.editor.SetLocalDirty ();
				return;
			}

			if (this.HilitedElement == ActiveElement.EdgeExtend)
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

			if (this.HilitedElement == ActiveElement.EdgeClose)
			{
				this.DeleteEdge ();
			}

			if (this.HilitedElement == ActiveElement.EdgeComment)
			{
				this.SwapComment ();
			}

			if (this.HilitedElement == ActiveElement.EdgeColor1)
			{
				this.BackgroundColorItem = ColorItem.Yellow;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor2)
			{
				this.BackgroundColorItem = ColorItem.Orange;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor3)
			{
				this.BackgroundColorItem = ColorItem.Red;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor4)
			{
				this.BackgroundColorItem = ColorItem.Lilac;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor5)
			{
				this.BackgroundColorItem = ColorItem.Purple;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor6)
			{
				this.BackgroundColorItem = ColorItem.Blue;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor7)
			{
				this.BackgroundColorItem = ColorItem.Green;
			}

			if (this.HilitedElement == ActiveElement.EdgeColor8)
			{
				this.BackgroundColorItem = ColorItem.Grey;
			}
		}

		public override ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			if (AbstractObject.DetectRoundRectangle (this.bounds, this.bounds.Height/2, pos))
			{
				return ActiveElement.EdgeHeader;
			}

			if (this.isExtended && this.RectangleDescription.Contains (pos))
			{
				return ActiveElement.EdgeEditDescription;
			}

			if (AbstractObject.DetectRoundRectangle (this.ExtendedBounds, this.bounds.Height/2, pos))
			{
				return ActiveElement.EdgeInside;
			}

			return ActiveElement.None;
		}

		public override ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			var result = ActiveElement.None;

			if (this.editor.CurrentModifyMode != Editor.ModifyMode.Locked)
			{
				result = this.DetectButtons (pos);

				if (result == ActiveElement.None && this.isExtended && this.RectangleDescription.Contains (pos))
				{
					result = ActiveElement.EdgeEditDescription;
				}
			}

			return result;
		}


		public override MouseCursorType MouseCursor
		{
			get
			{
				if (this.HilitedElement == ActiveElement.EdgeHeader)
				{
					if (this.draggingMode == DraggingMode.MoveObject)
					{
						return MouseCursorType.Move;
					}
					else
					{
						return MouseCursorType.MoveOrEdit;
					}
				}

				if (this.HilitedElement == ActiveElement.EdgeInside ||
					this.HilitedElement == ActiveElement.EdgeHilited)
				{
					return MouseCursorType.Arrow;
				}

				if (this.HilitedElement == ActiveElement.EdgeEditDescription)
				{
					return MouseCursorType.IBeam;
				}

				return MouseCursorType.Finger;
			}
		}


		public override string DebugInformations
		{
			get
			{
				return string.Format ("Edge: {0} {1}", this.Entity.Name.ToString (), this.DebugInformationsObjectLinks);
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

				Rectangle rect = new Rectangle (this.bounds.Left, this.bounds.Top+40, System.Math.Max (this.bounds.Width, AbstractObject.infoMinWidth), 20);
				this.comment.Bounds = rect;
				this.comment.UpdateHeight ();  // adapte la hauteur en fonction du contenu

				this.editor.AddBalloon (this.comment);
				this.editor.UpdateAfterCommentChanged ();
			}
			else
			{
				this.editor.RemoveBalloon (this.comment);
				this.comment = null;
			}

			this.editor.SetLocalDirty ();
		}

		private void DeleteEdge()
		{
			string message = string.Format ("Voulez-vous supprimer la transition \"{0}\" ?", this.Entity.Name);
			var result = Common.Dialogs.MessageDialog.ShowQuestion (message, this.editor.Window);
			if (result != Common.Dialogs.DialogResult.Yes)
			{
				return;
			}

			if (this.comment != null)
			{
				this.SwapComment ();  // ferme le commentaire
			}

			this.editor.CloseObject (this);
			this.editor.UpdateAfterGeometryChanged (null);
		}


		public override void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
			//	Héritage	->	Traitillé
			//	Interface	->	Trait plein avec o---
			base.DrawBackground (graphics);

			Rectangle rect;
			Color c1, c2;

			bool dragging = (this.hilitedElement == ActiveElement.EdgeHeader || this.hilitedElement == ActiveElement.EdgeEditDescription || this.isHilitedForLinkChanging);
			Color colorFrame = dragging ? this.colorFactory.GetColorMain () : this.colorFactory.GetColor (0);

			var extendedRect = new Rectangle (this.bounds.Left, this.bounds.Bottom-ObjectEdge.extendedHeight, this.bounds.Width, this.bounds.Height+ObjectEdge.extendedHeight);
			extendedRect.Deflate (2, 0);

			//	Dessine le boîte étendue.
			if (this.isExtended)
			{
				//	Dessine l'ombre.
				rect = extendedRect;
				rect.Offset (AbstractObject.shadowOffset, -(AbstractObject.shadowOffset));
				this.DrawRoundShadow (graphics, rect, this.bounds.Height/2, (int) AbstractObject.shadowOffset, 0.2);

				rect = extendedRect;
				rect.Deflate (0.5);
				Path extendedPath = this.PathRoundRectangle (rect, this.bounds.Height/2);

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
			rect.Offset (AbstractObject.shadowOffset, -(AbstractObject.shadowOffset));
			this.DrawEdgeShadow (graphics, rect, (int) AbstractObject.shadowOffset, 0.2);

			//	Construit le chemin du cadre.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathEdgeRectangle (rect);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid (this.colorFactory.GetColor (1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			c1 = this.colorFactory.GetColorMain (dragging ? 0.8 : 0.4);
			c2 = this.colorFactory.GetColorMain (dragging ? 0.4 : 0.1);
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			//	Dessine le titre.
			Color titleColor = dragging ? this.colorFactory.GetColor (1) : this.colorFactory.GetColor (0);

			rect = this.RectangleTitle;
			rect.Deflate (0, 2);

			if (string.IsNullOrEmpty (this.subtitleString))
			{
				var r = rect;
				r.Deflate (0, 6);
				this.title.LayoutSize = r.Size;
				this.title.Paint (r.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);
			}
			else
			{
				var r = new Rectangle (rect.Left, rect.Bottom+12, rect.Width, rect.Height-12);
				this.title.LayoutSize = r.Size;
				this.title.Paint (r.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

				r = new Rectangle (rect.Left, rect.Bottom, rect.Width, 20);

				foreach (var subtitle in this.subtitles)
				{
					subtitle.LayoutSize = r.Size;
					subtitle.Paint (r.BottomLeft, graphics, Rectangle.MaxValue, titleColor, GlyphPaintStyle.Normal);

					r.Offset (0, -ObjectEdge.subtitleHeight);
				}
			}

			//	Dessine la description.
			if (this.isExtended)
			{
				Color descriptionColor = this.colorFactory.GetColor (0);

				rect = this.RectangleDescription;
				this.description.LayoutSize = rect.Size;
				this.description.Paint (rect.BottomLeft, graphics, Rectangle.MaxValue, descriptionColor, GlyphPaintStyle.Normal);
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline (path, 2);
			graphics.RenderSolid (colorFrame);

			if (this.Entity.TransitionType == WorkflowTransitionType.Call)
			{
				rect = this.bounds;
				rect.Deflate (0, 3.5);
				double r = rect.Height/2;
				graphics.AddLine (rect.Left+r,  rect.Bottom-2, rect.Left+r,  rect.Top+2);
				graphics.AddLine (rect.Right-r, rect.Bottom-2, rect.Right-r, rect.Top+2);

				graphics.RenderSolid (colorFrame);
			}

			if (this.Entity.TransitionType == WorkflowTransitionType.Fork)
			{
				rect = this.bounds;
				rect.Inflate (2.5);
				path = this.PathEdgeRectangle (rect);

				graphics.Rasterizer.AddOutline (path, 1);
				graphics.RenderSolid (colorFrame);
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
			Path path = this.PathEdgeRectangle (rect);

			graphics.Rasterizer.AddOutline (path, 3);
			graphics.RenderSolid (Color.FromRgb (1, 0, 0));  // rouge
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

				return (this.hilitedElement == ActiveElement.EdgeHeader ||
						this.hilitedElement == ActiveElement.EdgeEditDescription ||
						this.hilitedElement == ActiveElement.EdgeInside ||
						this.hilitedElement == ActiveElement.EdgeComment ||
						this.hilitedElement == ActiveElement.EdgeColor1 ||
						this.hilitedElement == ActiveElement.EdgeColor2 ||
						this.hilitedElement == ActiveElement.EdgeColor3 ||
						this.hilitedElement == ActiveElement.EdgeColor4 ||
						this.hilitedElement == ActiveElement.EdgeColor5 ||
						this.hilitedElement == ActiveElement.EdgeColor6 ||
						this.hilitedElement == ActiveElement.EdgeColor7 ||
						this.hilitedElement == ActiveElement.EdgeColor8 ||
						this.hilitedElement == ActiveElement.EdgeExtend ||
						this.hilitedElement == ActiveElement.EdgeClose ||
						this.hilitedElement == ActiveElement.EdgeChangeWidth);
			}
		}

		private Rectangle RectangleTitle
		{
			get
			{
				return new Rectangle (this.bounds.Left+ObjectEdge.frameSize.Height/2, this.bounds.Top-ObjectEdge.frameSize.Height, this.bounds.Width-ObjectEdge.frameSize.Height, ObjectEdge.frameSize.Height);
			}
		}

		private Rectangle RectangleDescription
		{
			get
			{
				var rect = new Rectangle (this.bounds.Left, this.bounds.Bottom-ObjectEdge.extendedHeight, this.bounds.Width, ObjectEdge.extendedHeight);
				rect.Deflate (11, 8);

				return rect;
			}
		}

		private Point PositionExtendButton
		{
			//	Retourne la position du bouton pour étendre.
			get
			{
				return new Point (this.bounds.Left+ActiveButton.buttonRadius+6, this.bounds.Center.Y);
			}
		}

		private Point PositionChangeWidthButton
		{
			//	Retourne la position du bouton pour changer la largeur.
			get
			{
				return new Point (this.bounds.Right-1, this.bounds.Bottom-ActiveButton.buttonRadius-4);
			}
		}

		private Point PositionCommentButton
		{
			//	Retourne la position du bouton pour montrer le commentaire.
			get
			{
				return new Point (this.bounds.Left+ActiveButton.buttonRadius+6, this.bounds.Bottom-ObjectEdge.extendedHeight+ActiveButton.buttonRadius+4);
			}
		}

		private Point PositionCloseButton
		{
			//	Retourne la position du bouton pour fermer.
			get
			{
				return new Point (this.bounds.Right-ActiveButton.buttonRadius-6, this.bounds.Bottom-ObjectEdge.extendedHeight+ActiveButton.buttonRadius+4);
			}
		}

		private Point PositionColorButton(int rank)
		{
			//	Retourne la position du bouton pour choisir la couleur.
			return new Point (this.bounds.Left-1+(ActiveButton.buttonSquare+0.5)*(rank+1)*2, this.bounds.Bottom-7);
		}

		private string GetGroupTooltip(int rank)
		{
			//	Retourne le tooltip à afficher pour un groupe.
			return null;  // pas de tooltip
		}


		private void UpdateTitle()
		{
			//	Met à jour le titre du noeud.
			this.Title = this.Entity.Name.ToString ();
		}

		private void UpdateSubtitle()
		{
			//	Met à jour le sous-titre du noeud.
			this.Subtitle = this.Entity.TransitionActions;
		}

		private void UpdateDescription()
		{
			//	Met à jour le sous-titre du noeud.
			this.Description = this.Entity.Description.ToString ();
		}


		public WorkflowEdgeEntity Entity
		{
			get
			{
				return this.entity as WorkflowEdgeEntity;
			}
		}


		protected override void CreateButtons()
		{
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeExtend,      this.colorFactory, GlyphShape.ArrowUp,        this.UpdateButtonGeometryExtend,      this.UpdateButtonStateExtend));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeClose,       this.colorFactory, GlyphShape.Close,          this.UpdateButtonGeometryClose,       this.UpdateButtonStateClose));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeComment,     this.colorFactory, "C",                       this.UpdateButtonGeometryComment,     this.UpdateButtonStateComment));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeChangeWidth, this.colorFactory, GlyphShape.HorizontalMove, this.UpdateButtonGeometryChangeWidth, this.UpdateButtonStateChangeWidth));

			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor1, this.colorFactory, ColorItem.Yellow, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor2, this.colorFactory, ColorItem.Orange, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor3, this.colorFactory, ColorItem.Red,    this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor4, this.colorFactory, ColorItem.Lilac,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor5, this.colorFactory, ColorItem.Purple, this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor6, this.colorFactory, ColorItem.Blue,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor7, this.colorFactory, ColorItem.Green,  this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
			this.buttons.Add (new ActiveButton (ActiveElement.EdgeColor8, this.colorFactory, ColorItem.Grey,   this.UpdateButtonGeometryColor, this.UpdateButtonStateColor));
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

		private void UpdateButtonGeometryChangeWidth(ActiveButton button)
		{
			button.Center = this.PositionChangeWidthButton;
		}

		private void UpdateButtonGeometryColor(ActiveButton button)
		{
			int rank = button.Element - ActiveElement.EdgeColor1;

			button.Center = this.PositionColorButton (rank);
		}

		private void UpdateButtonStateExtend(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.Glyph = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			button.State.Visible = this.IsHeaderHilite && !this.IsDragging;
		}

		private void UpdateButtonStateClose(ActiveButton button)
		{
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

		private void UpdateButtonStateChangeWidth(ActiveButton button)
		{
			button.State.Hilited = this.hilitedElement == button.Element;
			button.State.Visible = button.State.Hilited || (this.IsHeaderHilite && !this.IsDragging && this.isExtended);
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
		}

		public override void Deserialize(XElement xml)
		{
			base.Deserialize (xml);
		}
		#endregion


		public static readonly Size				frameSize = new Size (200, 36);
		private static readonly double			extendedHeight = 80;
		private static readonly double			subtitleHeight = 10;
		private static readonly string			emptyTransitionAction = "<i>vide</i>";
		private static readonly string			subtitlePrefix = "<b>";
		private static readonly string			subtitlePostfix = ")</b> ";

		private string							titleString;
		private TextLayout						title;
		private string							subtitleString;
		private List<TextLayout>				subtitles;
		private string							descriptionString;
		private TextLayout						description;

		private ActiveElement					editingElement;
		private AbstractTextField				editingTextFieldPrimary;
		private AbstractTextField				editingTextFieldSecondary;
		private List<TextFieldCombo>			editingTextFieldCombos;
	}
}
