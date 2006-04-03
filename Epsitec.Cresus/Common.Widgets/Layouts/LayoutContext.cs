//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutContext
	{
		public LayoutContext()
		{
		}

		public int								MeasureQueueLength
		{
			get
			{
				return this.measureQueue.Count;
			}
		}

		public void StartNewLayoutPass()
		{
			this.passId++;
		}

		public void AddToMeasureQueue(Visual visual)
		{
			VisualNode node = new VisualNode (visual);
			this.measureQueue.Add (node, visual);
		}
		
		public void DefineMinWidth(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetOrCreateWidthMeasure (visual);
			measure.UpdateMin (this.passId, value);
		}
		public void DefineMaxWidth(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetOrCreateWidthMeasure (visual);
			measure.UpdateMax (this.passId, value);
		}
		
		public void DefineMinHeight(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetOrCreateHeightMeasure (visual);
			measure.UpdateMin (this.passId, value);
		}
		public void DefineMaxHeight(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetOrCreateHeightMeasure (visual);
			measure.UpdateMax (this.passId, value);
		}

		public LayoutMeasure GetOrCreateWidthMeasure(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (LayoutMeasure.WidthProperty, out value))
			{
				return value as LayoutMeasure;
			}
			else
			{
				LayoutMeasure measure;
				measure = new LayoutMeasure (this.passId);
				visual.SetLocalValue (LayoutMeasure.WidthProperty, measure);
				return measure;
			}
		}
		public LayoutMeasure GetOrCreateHeightMeasure(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (LayoutMeasure.HeightProperty, out value))
			{
				return value as LayoutMeasure;
			}
			else
			{
				LayoutMeasure measure;
				measure = new LayoutMeasure (this.passId);
				visual.SetLocalValue (LayoutMeasure.HeightProperty, measure);
				return measure;
			}
		}

		public void ClearMeasures(Visual visual)
		{
			visual.ClearLocalValue (LayoutMeasure.WidthProperty);
			visual.ClearLocalValue (LayoutMeasure.HeightProperty);
		}

		private struct VisualNode : System.IEquatable<VisualNode>, System.IComparable<VisualNode>
		{
			public VisualNode(Visual visual)
			{
				this.visual = visual;
				this.depth  = Helpers.VisualTree.GetDepth (visual);
			}

			public Visual						Visual
			{
				get
				{
					return this.visual;
				}
			}
			
			public override int GetHashCode()
			{
				return this.visual.VisualSerialId;
			}
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				else
				{
					return this.Equals ((VisualNode) obj);
				}
			}

			#region IEquatable<VisualNode> Members

			public bool Equals(VisualNode other)
			{
				return object.ReferenceEquals (this.visual, other.visual);
			}

			#endregion

			#region IComparable<VisualNode> Members

			public int CompareTo(VisualNode other)
			{
 				if (this.depth < other.depth)
				{
					return -1;
				}
				else if (this.depth > other.depth)
				{
					return 1;
				}
				else
				{
					return this.visual.VisualSerialId.CompareTo (other.visual.VisualSerialId);
				}
			}

			#endregion
			
			private Visual						visual;
			private int							depth;
		}

		private int passId;
		private SortedDictionary<VisualNode, Visual> measureQueue = new SortedDictionary<VisualNode,Visual> ();
		private SortedDictionary<VisualNode, Visual> arrangeQueue = new SortedDictionary<VisualNode,Visual> ();
	}
}
