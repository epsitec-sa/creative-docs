using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor
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

			this.bounds = this.objectModifier.GetActualBounds(this.obj);
			this.margins = this.objectModifier.GetMargins(this.obj);

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
				this.objectModifier.SetMargins(this.obj, this.margins);
			}

			if (cp == ObjectModifier.ChildrenPlacement.VerticalStacked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.Fill, ObjectModifier.StackedVerticalAttachment.Bottom);
				this.objectModifier.SetMargins(this.obj, this.margins);
			}

			if (cp == ObjectModifier.ChildrenPlacement.Grid)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.StackedHorizontalAttachment.None, ObjectModifier.StackedVerticalAttachment.None);
				this.objectModifier.SetMargins(this.obj, this.margins);
			}
		}


		public static void FixBounds(Widget parent, ObjectModifier objectModifier)
		{
			//	Mémorise la position actuelle de tous les fils d'un objet sélectionné.
			if (parent.HasChildren)
			{
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
		}

		public static void AdaptBounds(Widget parent, ObjectModifier objectModifier, ObjectModifier.ChildrenPlacement cp)
		{
			//	Adapte les fils d'un objet sélectionné après un changement de ChildrenPlacement.
			if (cp == ObjectModifier.ChildrenPlacement.Grid)
			{
				double n;
				n = System.Math.Sqrt(parent.Children.Count);
				n = System.Math.Ceiling(n);
				n = System.Math.Max(n, 2);
				int columns = (int) n;
				int rows = System.Math.Max((parent.Children.Count+columns-1)/columns, 1);

				objectModifier.SetGridColumnsCount(parent, columns);
				objectModifier.SetGridRowsCount(parent, rows);

				int column = 0;
				int row = 0;
				if (parent.HasChildren)
				{
					foreach (Widget children in parent.Children)
					{
						GeometryCache gc = children.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
						gc.AdaptBounds(cp);

						objectModifier.SetGridParentColumnRow(children, children.Parent, column, row);

						column++;
						if (column >= columns)
						{
							column = 0;
							row++;
						}
					}
				}
			}
			else
			{
				if (parent.HasChildren)
				{
					foreach (Widget children in parent.Children)
					{
						GeometryCache gc = children.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
						gc.AdaptBounds(cp);

						objectModifier.SetGridClear(children);
					}
				}
			}

			objectModifier.Invalidate();
		}

		public static void Clear(Widget parent)
		{
			//	Oublie toutes les informations de géométrie mémorisées.
			if (parent.HasChildren)
			{
				foreach (Widget obj in parent.Children)
				{
					obj.ClearValue(GeometryCache.GeometryCacheProperty);

					if (obj.HasChildren)
					{
						GeometryCache.Clear(obj);
					}
				}
			}
		}


		protected static readonly DependencyProperty GeometryCacheProperty = DependencyProperty.RegisterAttached("GeometryCache", typeof(GeometryCache), typeof(GeometryCache), new DependencyPropertyMetadata().MakeNotSerializable());

		protected Widget										obj;
		protected ObjectModifier								objectModifier;
		protected Rectangle										bounds = Rectangle.Empty;
		protected Margins										margins = Margins.Zero;
		protected ObjectModifier.AnchoredHorizontalAttachment	aha;
		protected ObjectModifier.AnchoredVerticalAttachment		ava;
	}
}
