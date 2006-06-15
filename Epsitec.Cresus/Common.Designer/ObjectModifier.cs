using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		public enum Placement
		{
			Anchored,
			VerticalDocked,
			HorizontalDocked,
		}

		public enum HorizontalAttachment
		{
			Left,
			Right,
			Fill,
		}

		public enum VerticalAttachment
		{
			Bottom,
			Top,
			Fill,
		}


		public ObjectModifier(MyWidgets.PanelEditor panelEditor)
		{
			//	Constructeur unique.
			this.panelEditor = panelEditor;
			this.container = this.panelEditor.Panel;
		}


		public void SetPlacement(Widget obj, Placement mode)
		{
			//	Choix du mode de placement de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			System.Diagnostics.Debug.Assert(group != null);

			switch (mode)
			{
				case Placement.Anchored:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
					break;

				case Placement.HorizontalDocked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
					group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
					break;

				case Placement.VerticalDocked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
					group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
					break;
			}
		}

		public Placement GetPlacement(Widget obj)
		{
			//	Retourne le mode de placement de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			System.Diagnostics.Debug.Assert(group != null);

			if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Anchored)
			{
				return Placement.Anchored;
			}

			if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Docked)
			{
				if (group.ContainerLayoutMode == ContainerLayoutMode.HorizontalFlow)
				{
					return Placement.HorizontalDocked;
				}
				else
				{
					return Placement.VerticalDocked;
				}
			}

			throw new System.Exception("Not supported.");
		}


		public void SetBounds(Widget obj, Rectangle bounds)
		{
			//	Choix de la position et des dimensions de l'objet.
			//	Uniquement pour les objets Anchored.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.Anchored);

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
			HorizontalAttachment ha = this.GetHorizontalAttachment(obj);
			VerticalAttachment va = this.GetVerticalAttachment(obj);

			if (ha == HorizontalAttachment.Left || ha == HorizontalAttachment.Fill)
			{
				double px = bounds.Left;
				px -= padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if (ha == HorizontalAttachment.Right || ha == HorizontalAttachment.Fill)
			{
				double px = box.Width - bounds.Right;
				px -= padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if (va == VerticalAttachment.Bottom || va == VerticalAttachment.Fill)
			{
				double py = bounds.Bottom;
				py -= padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if (va == VerticalAttachment.Top || va == VerticalAttachment.Fill)
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

		public Rectangle GetBounds(Widget obj)
		{
			//	Retourne la position et les dimensions de l'objet.
			//	Uniquement pour les objets Anchored.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.Anchored);

			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != this.container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}


		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les objets Docked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked || placement == Placement.VerticalDocked);

			if (obj.Margins != margins)
			{
				obj.Margins = margins;
				this.Invalidate();
			}
		}

		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les objets Docked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked || placement == Placement.VerticalDocked);

			return obj.Margins;
		}


		public void SetWidth(Widget obj, double width)
		{
			//	Choix de la largeur de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.VerticalDocked);

			if (obj.PreferredWidth != width)
			{
				obj.PreferredWidth = width;
				this.Invalidate();
			}
		}

		public double GetWidth(Widget obj)
		{
			//	Retourne la largeur de l'objet.
			//	Uniquement pour les objets VerticalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.VerticalDocked);

			return obj.PreferredWidth;
		}


		public void SetHeight(Widget obj, double height)
		{
			//	Choix de la hauteur de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked);

			if (obj.PreferredHeight != height)
			{
				obj.PreferredHeight = height;
				this.Invalidate();
			}
		}

		public double GetHeight(Widget obj)
		{
			//	Retourne la hauteur de l'objet.
			//	Uniquement pour les objets HorizontalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked);

			return obj.PreferredHeight;
		}


		public void SetZOrder(Widget obj, int order)
		{
			//	Choix de l'ordre de l'objet.
			//	Uniquement pour les objets Docked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked || placement == Placement.VerticalDocked);

			if (obj.ZOrder != order)
			{
				obj.ZOrder = order;
				this.Invalidate();
			}
		}

		public int GetZOrder(Widget obj)
		{
			//	Retourne l'ordre de l'objet.
			//	Uniquement pour les objets Docked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == Placement.HorizontalDocked || placement == Placement.VerticalDocked);

			return obj.ZOrder;
		}


		public void SetVerticalAttachment(Widget obj, VerticalAttachment attachment)
		{
			//	Choix de l'alignement vertical de l'objet.
			//	Uniquement pour les objets Anchored ou VerticalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement != Placement.HorizontalDocked);

			if (placement == Placement.Anchored)
			{
				AnchorStyles style = obj.Anchor;

				switch (attachment)
				{
					case VerticalAttachment.Bottom:
						style |=  AnchorStyles.Bottom;
						style &= ~AnchorStyles.Top;
						break;

					case VerticalAttachment.Top:
						style |=  AnchorStyles.Top;
						style &= ~AnchorStyles.Bottom;
						break;

					case VerticalAttachment.Fill:
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

			if (placement == Placement.VerticalDocked)
			{
				DockStyle style = obj.Dock;

				switch (attachment)
				{
					case VerticalAttachment.Bottom:
						style = DockStyle.Bottom;
						break;

					case VerticalAttachment.Top:
						style = DockStyle.Top;
						break;

					case VerticalAttachment.Fill:
						style = DockStyle.Fill;
						break;
				}

				if (obj.Dock != style)
				{
					obj.Dock = style;
					this.Invalidate();
				}
			}
		}

		public VerticalAttachment GetVerticalAttachment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			//	Uniquement pour les objets Anchored ou VerticalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement != Placement.HorizontalDocked);

			if (placement == Placement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool bottom = ((style & AnchorStyles.Bottom) != 0);
				bool top    = ((style & AnchorStyles.Top   ) != 0);

				if (bottom && top)  return VerticalAttachment.Fill;
				if (bottom       )  return VerticalAttachment.Bottom;
				if (top          )  return VerticalAttachment.Top;
			}

			if (placement == Placement.VerticalDocked)
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.Fill  )  return VerticalAttachment.Fill;
				if (style == DockStyle.Bottom)  return VerticalAttachment.Bottom;
				if (style == DockStyle.Top   )  return VerticalAttachment.Top;
			}

			throw new System.Exception("Not supported.");
		}


		public void SetHorizontalAttachment(Widget obj, HorizontalAttachment attachment)
		{
			//	Choix de l'alignement horizontal de l'objet.
			//	Uniquement pour les objets Anchored ou HorizontalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement != Placement.VerticalDocked);

			if (placement == Placement.Anchored)
			{
				AnchorStyles style = obj.Anchor;

				switch (attachment)
				{
					case HorizontalAttachment.Left:
						style |=  AnchorStyles.Left;
						style &= ~AnchorStyles.Right;
						break;

					case HorizontalAttachment.Right:
						style |=  AnchorStyles.Right;
						style &= ~AnchorStyles.Left;
						break;

					case HorizontalAttachment.Fill:
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

			if (placement == Placement.HorizontalDocked)
			{
				DockStyle style = obj.Dock;

				switch (attachment)
				{
					case HorizontalAttachment.Left:
						style = DockStyle.Left;
						break;

					case HorizontalAttachment.Right:
						style = DockStyle.Right;
						break;

					case HorizontalAttachment.Fill:
						style = DockStyle.Fill;
						break;
				}

				if (obj.Dock != style)
				{
					obj.Dock = style;
					this.Invalidate();
				}
			}
		}

		public HorizontalAttachment GetHorizontalAttachment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			//	Uniquement pour les objets Anchored ou HorizontalDocked.
			Placement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement != Placement.VerticalDocked);

			if (placement == Placement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool left  = ((style & AnchorStyles.Left ) != 0);
				bool right = ((style & AnchorStyles.Right) != 0);

				if (left && right)  return HorizontalAttachment.Fill;
				if (left         )  return HorizontalAttachment.Left;
				if (right        )  return HorizontalAttachment.Right;
			}

			if (placement == Placement.HorizontalDocked)
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.Fill )  return HorizontalAttachment.Fill;
				if (style == DockStyle.Left )  return HorizontalAttachment.Left;
				if (style == DockStyle.Right)  return HorizontalAttachment.Right;
			}

			throw new System.Exception("Not supported.");
		}


		protected Placement GetParentPlacement(Widget obj)
		{
			//	Retourne le mode de placement du parent d'un objet.
			System.Diagnostics.Debug.Assert(obj != this.container);
			return this.GetPlacement(obj.Parent);
		}

		protected void Invalidate()
		{
			//	Invalide le PanelEditor.
			this.panelEditor.Invalidate();
		}


		protected MyWidgets.PanelEditor				panelEditor;
		protected UI.Panel							container;
	}
}
