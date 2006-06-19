using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		public enum ChildrenPlacement
		{
			[Types.Hide] None,
			Anchored,
			VerticalDocked,
			HorizontalDocked,
			[Types.Hide] VerticalStacked,
			[Types.Hide] HorizontalStacked,
		}

		public enum AnchoredHorizontalAttachment
		{
			[Types.Hide] None,
			Left,
			Right,
			Fill,
		}

		public enum AnchoredVerticalAttachment
		{
			[Types.Hide] None,
			Bottom,
			Top,
			Fill,
		}

		public enum DockedHorizontalAttachment
		{
			[Types.Hide] None,
			Left,
			Right,
			Fill,
		}

		public enum DockedVerticalAttachment
		{
			[Types.Hide] None,
			Bottom,
			Top,
			Fill,
		}

		public enum DockedHorizontalAlignment
		{
			[Types.Hide] None,
			Stretch,
			Center,
			Left,
			Right,
		}

		public enum DockedVerticalAlignment
		{
			[Types.Hide] None,
			Stretch,
			Center,
			Bottom,
			Top,
		}


		public ObjectModifier(MyWidgets.PanelEditor panelEditor)
		{
			//	Constructeur unique.
			this.panelEditor = panelEditor;
			this.container = this.panelEditor.Panel;
		}


		#region ChildrenPlacement
		public bool IsChildrenAnchored(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Anchored);
			}
			return false;
		}

		public bool IsChildrenDocked(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalDocked || p == ChildrenPlacement.VerticalDocked);
			}
			return false;
		}

		public bool IsChildrenStacked(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked || p == ChildrenPlacement.VerticalStacked);
			}
			return false;
		}

		public bool IsChildrenHorizontal(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalDocked || p == ChildrenPlacement.HorizontalStacked);
			}
			return false;
		}

		public bool IsChildrenPlacement(Widget obj)
		{
			//	Indique s'il existe un mode de placement des enfants de l'objet.
			AbstractGroup group = obj as AbstractGroup;
			return (group != null);
		}

		public ChildrenPlacement GetChildrenPlacement(Widget obj)
		{
			//	Retourne le mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Anchored)
				{
					return ChildrenPlacement.Anchored;
				}

				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Docked)
				{
					if (group.ContainerLayoutMode == ContainerLayoutMode.HorizontalFlow)
					{
						return ChildrenPlacement.HorizontalDocked;
					}
					else
					{
						return ChildrenPlacement.VerticalDocked;
					}
				}
			}

			return ChildrenPlacement.None;
		}

		public void SetChildrenPlacement(Widget obj, ChildrenPlacement mode)
		{
			//	Choix du mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			System.Diagnostics.Debug.Assert(group != null);

			switch (mode)
			{
				case ChildrenPlacement.Anchored:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
					break;

				case ChildrenPlacement.HorizontalDocked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
					group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
					break;

				case ChildrenPlacement.VerticalDocked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
					group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
					break;
			}
		}
		#endregion


		public bool IsBounds(Widget obj)
		{
			//	Indique si l'objet a une position et des dimensions modifiables.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.Anchored);
		}

		public Rectangle GetBounds(Widget obj)
		{
			//	Retourne la position et les dimensions de l'objet.
			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != this.container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public void SetBounds(Widget obj, Rectangle bounds)
		{
			//	Choix de la position et des dimensions de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

			bounds.Normalise();

			if (bounds.Width < obj.MinWidth)
			{
				bounds.Width = obj.MinWidth;
			}

			if (bounds.Height < obj.MinHeight)
			{
				bounds.Height = obj.MinHeight;
			}

			obj.Window.ForceLayout();
			Widget parent = obj.Parent;
			while (parent != this.container)
			{
				bounds = parent.MapParentToClient(bounds);
				parent = parent.Parent;
			}

			parent = obj.Parent;
			Rectangle box = parent.ActualBounds;
			Margins margins = obj.Margins;
			Margins padding = parent.Padding + parent.GetInternalPadding();
			AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
			AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);

			if (ha == AnchoredHorizontalAttachment.Left || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = bounds.Left;
				px -= padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if (ha == AnchoredHorizontalAttachment.Right || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = box.Width - bounds.Right;
				px -= padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if (va == AnchoredVerticalAttachment.Bottom || va == AnchoredVerticalAttachment.Fill)
			{
				double py = bounds.Bottom;
				py -= padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if (va == AnchoredVerticalAttachment.Top || va == AnchoredVerticalAttachment.Fill)
			{
				double py = box.Height - bounds.Top;
				py -= padding.Top;
				py = System.Math.Max(py, 0);
				margins.Top = py;
			}

			obj.Margins = margins;
			obj.PreferredSize = bounds.Size;

			this.Invalidate();
		}


		public bool IsMargins(Widget obj)
		{
			//	Indique si l'objet a des marges.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked)
			{
				return true;
			}

			return false;
		}

		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les objets Docked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked)
			{
				return obj.Margins;
			}

			return Margins.Zero;
		}

		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les objets Docked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked);

			if (obj.Margins != margins)
			{
				obj.Margins = margins;
				this.Invalidate();
			}
		}


		public double GetWidth(Widget obj)
		{
			//	Retourne la largeur de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked)
			{
				return obj.PreferredWidth;
			}

			return 0;
		}

		public void SetWidth(Widget obj, double width)
		{
			//	Choix de la largeur de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.HorizontalDocked);

			if (obj.PreferredWidth != width)
			{
				obj.PreferredWidth = width;
				this.Invalidate();
			}
		}


		public double GetHeight(Widget obj)
		{
			//	Retourne la hauteur de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.VerticalDocked)
			{
				return obj.PreferredHeight;
			}

			return 0;
		}

		public void SetHeight(Widget obj, double height)
		{
			//	Choix de la hauteur de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.VerticalDocked);

			if (obj.PreferredHeight != height)
			{
				obj.PreferredHeight = height;
				this.Invalidate();
			}
		}


		public int GetZOrder(Widget obj)
		{
			//	Retourne l'ordre de l'objet.
			return obj.ZOrder;
		}

		public void SetZOrder(Widget obj, int order)
		{
			//	Choix de l'ordre de l'objet.
			if (obj.ZOrder != order)
			{
				obj.ZOrder = order;
				this.Invalidate();
			}
		}


		public bool IsAttachmentLeft(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Right);
			}

			if (placement == ChildrenPlacement.HorizontalDocked)
			{
				DockedHorizontalAttachment ha = this.GetDockedHorizontalAttachment(obj);
				return (ha == DockedHorizontalAttachment.Left);
			}

			return false;
		}

		public bool IsAttachmentRight(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Left);
			}

			if (placement == ChildrenPlacement.HorizontalDocked)
			{
				DockedHorizontalAttachment ha = this.GetDockedHorizontalAttachment(obj);
				return (ha == DockedHorizontalAttachment.Right);
			}

			return false;
		}

		public bool IsAttachmentBottom(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Top);
			}

			if (placement == ChildrenPlacement.VerticalDocked)
			{
				DockedVerticalAttachment va = this.GetDockedVerticalAttachment(obj);
				return (va == DockedVerticalAttachment.Bottom);
			}

			return false;
		}

		public bool IsAttachmentTop(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Bottom);
			}

			if (placement == ChildrenPlacement.VerticalDocked)
			{
				DockedVerticalAttachment va = this.GetDockedVerticalAttachment(obj);
				return (va == DockedVerticalAttachment.Top);
			}

			return false;
		}


		#region Anchored
		public AnchoredVerticalAttachment GetAnchoredVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool bottom = ((style & AnchorStyles.Bottom) != 0);
				bool top    = ((style & AnchorStyles.Top   ) != 0);

				if (bottom && top)  return AnchoredVerticalAttachment.Fill;
				if (bottom       )  return AnchoredVerticalAttachment.Bottom;
				if (top          )  return AnchoredVerticalAttachment.Top;
			}

			return AnchoredVerticalAttachment.None;
		}

		public void SetAnchoredVerticalAttachment(Widget obj, AnchoredVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredVerticalAttachment.Bottom:
					style |=  AnchorStyles.Bottom;
					style &= ~AnchorStyles.Top;
					break;

				case AnchoredVerticalAttachment.Top:
					style |=  AnchorStyles.Top;
					style &= ~AnchorStyles.Bottom;
					break;

				case AnchoredVerticalAttachment.Fill:
					style |=  AnchorStyles.Bottom;
					style |=  AnchorStyles.Top;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				this.Invalidate();
			}
		}


		public AnchoredHorizontalAttachment GetAnchoredHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool left  = ((style & AnchorStyles.Left ) != 0);
				bool right = ((style & AnchorStyles.Right) != 0);

				if (left && right)  return AnchoredHorizontalAttachment.Fill;
				if (left         )  return AnchoredHorizontalAttachment.Left;
				if (right        )  return AnchoredHorizontalAttachment.Right;
			}

			return AnchoredHorizontalAttachment.None;
		}

		public void SetAnchoredHorizontalAttachment(Widget obj, AnchoredHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredHorizontalAttachment.Left:
					style |=  AnchorStyles.Left;
					style &= ~AnchorStyles.Right;
					break;

				case AnchoredHorizontalAttachment.Right:
					style |=  AnchorStyles.Right;
					style &= ~AnchorStyles.Left;
					break;

				case AnchoredHorizontalAttachment.Fill:
					style |=  AnchorStyles.Left;
					style |=  AnchorStyles.Right;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				this.Invalidate();
			}
		}
		#endregion


		#region Docked
		public bool IsDockedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.VerticalDocked);
		}

		public DockedVerticalAttachment GetDockedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.VerticalDocked)
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.Fill  )  return DockedVerticalAttachment.Fill;
				if (style == DockStyle.Bottom)  return DockedVerticalAttachment.Bottom;
				if (style == DockStyle.Top   )  return DockedVerticalAttachment.Top;
			}

			return DockedVerticalAttachment.None;
		}

		public void SetDockedVerticalAttachment(Widget obj, DockedVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.VerticalDocked);

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case DockedVerticalAttachment.Bottom:
					style = DockStyle.Bottom;
					break;

				case DockedVerticalAttachment.Top:
					style = DockStyle.Top;
					break;

				case DockedVerticalAttachment.Fill:
					style = DockStyle.Fill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				this.Invalidate();
			}
		}


		public bool IsDockedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalDocked);
		}

		public DockedHorizontalAttachment GetDockedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked)
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.Fill )  return DockedHorizontalAttachment.Fill;
				if (style == DockStyle.Left )  return DockedHorizontalAttachment.Left;
				if (style == DockStyle.Right)  return DockedHorizontalAttachment.Right;
			}

			return DockedHorizontalAttachment.None;
		}

		public void SetDockedHorizontalAttachment(Widget obj, DockedHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.HorizontalDocked);

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case DockedHorizontalAttachment.Left:
					style = DockStyle.Left;
					break;

				case DockedHorizontalAttachment.Right:
					style = DockStyle.Right;
					break;

				case DockedHorizontalAttachment.Fill:
					style = DockStyle.Fill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				this.Invalidate();
			}
		}


		public bool IsDockedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked);
		}

		public DockedHorizontalAlignment GetDockedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked)
			{
				HorizontalAlignment ha = obj.HorizontalAlignment;
				if (ha == HorizontalAlignment.Stretch)  return DockedHorizontalAlignment.Stretch;
				if (ha == HorizontalAlignment.Center )  return DockedHorizontalAlignment.Center;
				if (ha == HorizontalAlignment.Left   )  return DockedHorizontalAlignment.Left;
				if (ha == HorizontalAlignment.Right  )  return DockedHorizontalAlignment.Right;
			}

			return DockedHorizontalAlignment.None;
		}

		public void SetDockedHorizontalAlignment(Widget obj, DockedHorizontalAlignment alignment)
		{
			//	Choix de l'alignement horizontal de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked);

			HorizontalAlignment ha = obj.HorizontalAlignment;

			switch (alignment)
			{
				case DockedHorizontalAlignment.Stretch:
					ha = HorizontalAlignment.Stretch;
					break;

				case DockedHorizontalAlignment.Center:
					ha = HorizontalAlignment.Center;
					break;

				case DockedHorizontalAlignment.Left:
					ha = HorizontalAlignment.Left;
					break;

				case DockedHorizontalAlignment.Right:
					ha = HorizontalAlignment.Right;
					break;
			}

			if (obj.HorizontalAlignment != ha)
			{
				obj.HorizontalAlignment = ha;
				this.Invalidate();
			}
		}


		public bool IsDockedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked);
		}

		public DockedVerticalAlignment GetDockedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked)
			{
				VerticalAlignment va = obj.VerticalAlignment;
				if (va == VerticalAlignment.Stretch)  return DockedVerticalAlignment.Stretch;
				if (va == VerticalAlignment.Center )  return DockedVerticalAlignment.Center;
				if (va == VerticalAlignment.Bottom )  return DockedVerticalAlignment.Bottom;
				if (va == VerticalAlignment.Top    )  return DockedVerticalAlignment.Top;
			}

			return DockedVerticalAlignment.None;
		}

		public void SetDockedVerticalAlignment(Widget obj, DockedVerticalAlignment alignment)
		{
			//	Choix de l'alignement vertical de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.HorizontalDocked || placement == ChildrenPlacement.VerticalDocked);

			VerticalAlignment va = obj.VerticalAlignment;

			switch (alignment)
			{
				case DockedVerticalAlignment.Stretch:
					va = VerticalAlignment.Stretch;
					break;

				case DockedVerticalAlignment.Center:
					va = VerticalAlignment.Center;
					break;

				case DockedVerticalAlignment.Bottom:
					va = VerticalAlignment.Bottom;
					break;

				case DockedVerticalAlignment.Top:
					va = VerticalAlignment.Top;
					break;
			}

			if (obj.VerticalAlignment != va)
			{
				obj.VerticalAlignment = va;
				this.Invalidate();
			}
		}
		#endregion


		protected ChildrenPlacement GetParentPlacement(Widget obj)
		{
			//	Retourne le mode de placement du parent d'un objet.
			if (obj == this.container)
			{
				return ChildrenPlacement.None;
			}
			else
			{
				return this.GetChildrenPlacement(obj.Parent);
			}
		}

		protected void Invalidate()
		{
			//	Invalide le PanelEditor.
			this.panelEditor.Invalidate();
			this.panelEditor.UpdateGeometry();
		}


		protected MyWidgets.PanelEditor				panelEditor;
		protected UI.Panel							container;
	}
}
