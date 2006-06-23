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
			this.obj = obj;
			this.objectModifier = objectModifier;

			this.bounds = this.objectModifier.GetBounds(this.obj);
		}

		public void AdaptBounds(ObjectModifier.ChildrenPlacement cp)
		{
			if (cp == ObjectModifier.ChildrenPlacement.Anchored)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.DockedHorizontalAttachment.None, ObjectModifier.DockedVerticalAttachment.None);
				this.objectModifier.SetBounds(this.obj, this.bounds);
			}

			if (cp == ObjectModifier.ChildrenPlacement.HorizontalDocked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.DockedHorizontalAttachment.Left, ObjectModifier.DockedVerticalAttachment.Fill);
			}

			if (cp == ObjectModifier.ChildrenPlacement.VerticalDocked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.DockedHorizontalAttachment.Fill, ObjectModifier.DockedVerticalAttachment.Bottom);
			}
		}


		static public void FixBounds(IEnumerable<Widget> list, ObjectModifier objectModifier)
		{
			//	Mémorise la position actuelle des objets sélectionnés.
			foreach (Widget obj in list)
			{
				GeometryCache gc = obj.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
				if (gc == null)
				{
					gc = new GeometryCache(obj, objectModifier);
					obj.SetValue(GeometryCache.GeometryCacheProperty, gc);
				}
			}
		}

		static public void AdaptBounds(IEnumerable<Widget> list, ObjectModifier.ChildrenPlacement cp)
		{
			//	Adapte les objets sélectionnés après un changement de ChildrenPlacement.
			foreach (Widget obj in list)
			{
				GeometryCache gc = obj.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
				gc.AdaptBounds(cp);
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


		protected Widget						obj;
		protected ObjectModifier				objectModifier;
		protected Rectangle						bounds = Rectangle.Empty;
	}
}
