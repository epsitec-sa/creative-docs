using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Cache pour la géométrie d'un objet (widget).
	/// </summary>
	public class GeometryCache : DependencyObject
	{
		public GeometryCache(Widget obj, ObjectModifier objectModifier)
		{
			//	Constructeur unique qui mémorise la géométrie de l'objet.
			this.obj = obj;
			this.objectModifier = objectModifier;

			this.bounds = this.objectModifier.GetBounds(this.obj);

			if (this.objectModifier.GetChildrenPlacement(obj.Parent) == ObjectModifier.ChildrenPlacement.Anchored)
			{
				this.aha = this.objectModifier.GetAnchoredHorizontalAttachment(obj);
				this.ava = this.objectModifier.GetAnchoredVerticalAttachment(obj);
			}
			else
			{
				this.aha = ObjectModifier.AnchoredHorizontalAttachment.Left;
				this.ava = ObjectModifier.AnchoredVerticalAttachment.Bottom;
			}
		}

		public void AdaptBounds(ObjectModifier.ChildrenPlacement cp)
		{
			//	Adapte un objet selon le mode ChildrenPlacement de son parent.
			if (cp == ObjectModifier.ChildrenPlacement.Anchored)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);
				this.objectModifier.SetAnchoredHorizontalAttachment(obj, this.aha);
				this.objectModifier.SetAnchoredVerticalAttachment(obj, this.ava);
				this.objectModifier.SetBounds(this.obj, this.bounds);
			}

			if (cp == ObjectModifier.ChildrenPlacement.HorizontalStacked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.Left, ObjectModifier.StackedVerticalAttachment.Fill);
			}

			if (cp == ObjectModifier.ChildrenPlacement.VerticalStacked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.Fill, ObjectModifier.StackedVerticalAttachment.Bottom);
			}

			if (cp == ObjectModifier.ChildrenPlacement.Grid)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);
			}
		}


		static public void FixBounds(Widget parent, ObjectModifier objectModifier)
		{
			//	Mémorise la position actuelle de tous les fils d'un objet sélectionné.
			foreach (Widget children in parent.Children)
			{
				GeometryCache gc = children.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
				if (gc == null)
				{
					gc = new GeometryCache(children, objectModifier);
					children.SetValue(GeometryCache.GeometryCacheProperty, gc);
				}
			}
		}

		static public void AdaptBounds(Widget parent, ObjectModifier objectModifier, ObjectModifier.ChildrenPlacement cp)
		{
			//	Adapte les fils d'un objet sélectionné après un changement de ChildrenPlacement.
			if (cp == ObjectModifier.ChildrenPlacement.Grid)
			{
				double n;
				n = System.Math.Sqrt(parent.Children.Count);
				n = System.Math.Ceiling(n);
				n = System.Math.Max(n, 2);
				int count = (int) n;

				objectModifier.SetGridColumnsCount(parent, count);
				objectModifier.SetGridRowsCount(parent, count);

				int column = 0;
				int row = 0;
				foreach (Widget children in parent.Children)
				{
					GeometryCache gc = children.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
					gc.AdaptBounds(cp);

					objectModifier.SetGridColumn(children, column);
					objectModifier.SetGridRow(children, row);

					column ++;
					if (column >= count)
					{
						column = 0;
						row ++;
					}
				}
			}
			else
			{
				foreach (Widget children in parent.Children)
				{
					GeometryCache gc = children.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
					gc.AdaptBounds(cp);
				}
			}
		}

		static public void Clear(Widget parent)
		{
			//	Oublie toutes les informations de géométrie mémorisées.
			foreach (Widget obj in parent.Children)
			{
				obj.ClearValue(GeometryCache.GeometryCacheProperty);

				if (obj.Children.Count > 0)
				{
					GeometryCache.Clear(obj);
				}
			}
		}


		protected static readonly DependencyProperty GeometryCacheProperty = DependencyProperty.RegisterAttached("GeometryCache", typeof(GeometryCache), typeof(GeometryCache));

		protected Widget										obj;
		protected ObjectModifier								objectModifier;
		protected Rectangle										bounds = Rectangle.Empty;
		protected ObjectModifier.AnchoredHorizontalAttachment	aha;
		protected ObjectModifier.AnchoredVerticalAttachment		ava;
	}
}
