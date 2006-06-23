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
	public class GeometryCache
	{
		public GeometryCache(Widget obj, ObjectModifier objectModifier)
		{
			this.obj = obj;
			this.objectModifier = objectModifier;
		}

		public void FixBounds()
		{
			if (this.bounds.IsEmpty)
			{
				this.bounds = this.objectModifier.GetBounds(this.obj);
			}
		}

		public void AdaptBounds(ObjectModifier.ChildrenPlacement cp)
		{
			if (cp == ObjectModifier.ChildrenPlacement.Anchored)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.DockedHorizontalAttachment.None, ObjectModifier.DockedVerticalAttachment.None);
				this.objectModifier.SetBounds(this.obj, this.bounds);
			}

			if (cp == ObjectModifier.ChildrenPlacement.HorizontalDocked || cp == ObjectModifier.ChildrenPlacement.VerticalDocked)
			{
				this.objectModifier.AdaptFromParent(obj, ObjectModifier.DockedHorizontalAttachment.Fill, ObjectModifier.DockedVerticalAttachment.Fill);
			}
		}


		static public void FixBounds(IEnumerable<Widget> list, ObjectModifier objectModifier)
		{
			foreach (Widget obj in list)
			{
				GeometryCache gc = new GeometryCache(obj, objectModifier);
				obj.SetValue(GeometryCache.GeometryCacheProperty, gc);
				gc.FixBounds();
			}
		}

		static public void AdaptBounds(IEnumerable<Widget> list, ObjectModifier.ChildrenPlacement cp)
		{
			foreach (Widget obj in list)
			{
				GeometryCache gc = obj.GetValue(GeometryCache.GeometryCacheProperty) as GeometryCache;
				gc.AdaptBounds(cp);
			}
		}

		static public void Clear(List<Widget> list)
		{
			foreach (Widget obj in list)
			{
				obj.ClearValue(GeometryCache.GeometryCacheProperty);
			}
		}


		protected static readonly DependencyProperty GeometryCacheProperty = DependencyProperty.RegisterAttached("GeometryCache", typeof(GeometryCache), typeof(GeometryCache));


		protected Widget						obj;
		protected ObjectModifier				objectModifier;
		protected Rectangle						bounds = Rectangle.Empty;
	}
}
