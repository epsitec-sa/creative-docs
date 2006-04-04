//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutContext : Types.DependencyObject
	{
		public LayoutContext()
		{
			this.GenerateNewPassId ();
		}

		public int								MeasureQueueLength
		{
			get
			{
				return this.measureQueue.Count;
			}
		}
		public int								ArrangeQueueLength
		{
			get
			{
				return this.arrangeQueue.Count;
			}
		}

		public void StartNewLayoutPass()
		{
			this.measureQueue.Clear ();
			this.arrangeQueue.Clear ();
			
			this.nodeRank = 0;

			this.measureChanges = 0;
			this.arrangeChanges = 0;

			this.GenerateNewPassId ();
		}

		public IEnumerable<Visual> GetMeasureQueue()
		{
			foreach (VisualNode node in this.measureQueue.Keys)
			{
				yield return node.Visual;
			}
		}
		public IEnumerable<Visual> GetArrangeQueue()
		{
			foreach (VisualNode node in this.arrangeQueue.Keys)
			{
				yield return node.Visual;
			}
		}

		public static void AddToMeasureQueue(Visual visual)
		{
			if (visual != null)
			{
				int depth;
				LayoutContext context = Helpers.VisualTree.GetLayoutContext (visual, out depth);
				context.AddToMeasureQueue (visual, depth);
			}
		}
		public static void AddToArrangeQueue(Visual visual)
		{
			if (visual != null)
			{
				int depth;
				LayoutContext context = Helpers.VisualTree.GetLayoutContext (visual, out depth);
				context.AddToArrangeQueue (visual, depth);
			}
		}
		
		private void AddToMeasureQueue(Visual visual, int depth)
		{
			int rank = ++this.nodeRank;
			VisualNode node = new VisualNode (visual, 0, SortMode.Measure, depth);

			if (this.measureQueue.ContainsKey (node))
			{
				node.DefineRank (rank);
				this.measureQueue.Remove (node);
			}
			else
			{
				node.DefineRank (rank);
				this.measureQueue.Add (node, visual);
				this.measureChanges++;
			}
		}
		private void AddToArrangeQueue(Visual visual, int depth)
		{
			int rank = ++this.nodeRank;
			VisualNode node = new VisualNode (visual, 0, SortMode.Arrange, depth);
			
			if (this.arrangeQueue.ContainsKey (node))
			{
				node.DefineRank (rank);
				this.arrangeQueue[node] = visual;
			}
			else
			{
				node.DefineRank (rank);
				this.arrangeQueue.Add (node, visual);
				this.arrangeChanges++;
			}
		}

		public void ExecuteMeasure()
		{
			//	Measure all widgets which have been queued to be measured. Start
			//	with the children farthest down the tree, finish with the root.
			
			while (this.measureQueue.Count > 0)
			{
				VisualNode node = this.measureQueue.Keys[0];
				this.measureQueue.RemoveAt (0);

				System.Diagnostics.Debug.Assert (this.cacheVisual == null);
				
				this.cacheVisual = node.Visual;
				this.cacheWidthMeasure = this.GetOrCreateCleanMeasure (node.Visual, LayoutMeasure.WidthProperty);
				this.cacheHeightMeasure = this.GetOrCreateCleanMeasure (node.Visual, LayoutMeasure.HeightProperty);

				node.Visual.Measure (this);

				this.cacheWidthMeasure.UpdatePassId (this.passId);
				this.cacheHeightMeasure.UpdatePassId (this.passId);
				
				//	Did the visual update in any way the measures ?

				if ((this.cacheWidthMeasure.HasChanged) ||
					(this.cacheHeightMeasure.HasChanged))
				{
					//	The visual has specified other measures.
					
					//	If the visual has children, then its contents need to
					//	be arranged.

					if (node.Visual.HasChildren)
					{
						this.AddToArrangeQueue (node.Visual, node.Depth);
					}
					
					//	If the visual is either docked or anchored, then the
					//	contents of the parent need to be re-arranged.

					if ((node.Depth > 1) &&
						((node.Visual.Dock != DockStyle.None) || (node.Visual.Anchor != AnchorStyles.None)))
					{
						Visual parent = node.Visual.Parent;
						int depth = node.Depth - 1;
						
						this.AddToArrangeQueue (parent, depth);
					}
				}

				System.Diagnostics.Debug.Assert (this.cacheVisual == node.Visual);

				this.cacheVisual = null;
				this.cacheWidthMeasure = null;
				this.cacheHeightMeasure = null;
			}
		}
		
		public void DefineMinWidth(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedWidthMeasure (visual);
			measure.UpdateMin (this.passId, value);
		}
		public void DefineMaxWidth(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedWidthMeasure (visual);
			measure.UpdateMax (this.passId, value);
		}
		
		public void DefineMinHeight(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedHeightMeasure (visual);
			measure.UpdateMin (this.passId, value);
		}
		public void DefineMaxHeight(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedHeightMeasure (visual);
			measure.UpdateMax (this.passId, value);
		}
		
		public void DefineDesiredWidth(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedWidthMeasure (visual);
			measure.UpdateDesired (this.passId, value);
		}
		public void DefineDesiredHeight(Visual visual, double value)
		{
			LayoutMeasure measure = this.GetCachedHeightMeasure (visual);
			measure.UpdateDesired (this.passId, value);
		}

		public void ClearMeasures(Visual visual)
		{
			visual.ClearLocalValue (LayoutMeasure.WidthProperty);
			visual.ClearLocalValue (LayoutMeasure.HeightProperty);
		}

		private void GenerateNewPassId()
		{
			this.passId = System.Threading.Interlocked.Increment (ref LayoutContext.nextPassId);
		}
		
		private LayoutMeasure GetCachedWidthMeasure(Visual visual)
		{
			if (object.ReferenceEquals (this.cacheVisual, visual))
			{
				return this.cacheWidthMeasure;
			}

			throw new System.InvalidOperationException ("Wrong visual accessing width measure");
		}
		private LayoutMeasure GetCachedHeightMeasure(Visual visual)
		{
			if (object.ReferenceEquals (this.cacheVisual, visual))
			{
				return this.cacheHeightMeasure;
			}

			throw new System.InvalidOperationException ("Wrong visual accessing height measure");
		}
		
		private LayoutMeasure GetOrCreateCleanMeasure(Visual visual, Types.DependencyProperty property)
		{
			object value;

			if (visual.TryGetLocalValue (property, out value))
			{
				LayoutMeasure measure = value as LayoutMeasure;
				measure.ClearHasChanged ();
				return measure;
			}
			else
			{
				LayoutMeasure measure;
				measure = new LayoutMeasure (0);
				visual.SetLocalValue (property, measure);
				return measure;
			}
		}

		private enum SortMode
		{
			Measure,
			Arrange
		}
		
		private struct VisualNode : System.IEquatable<VisualNode>, System.IComparable<VisualNode>
		{
			public VisualNode(Visual visual, int rank, SortMode mode, int depth)
			{
				if (depth == 0)
				{
					depth = Helpers.VisualTree.GetDepth (visual);
				}
				
				//	Depending on the SortMode, we either use a positive or negative
				//	depth; "arrange" needs to walk through the visuals from the root
				//	down to the children whereas "measure" needs to start with the
				//	children, moving up to the root.

				switch (mode)
				{
					case SortMode.Arrange: this.depth =  depth; break;
					case SortMode.Measure: this.depth = -depth; break;

					default:
						throw new System.ArgumentException (string.Format ("SortMode.{0} not accepted"));
				}

				this.visual = visual;
				this.rank   = rank;
			}

			public Visual						Visual
			{
				get
				{
					return this.visual;
				}
			}
			public int							Depth
			{
				get
				{
					return this.depth < 0 ? -this.depth : this.depth;
				}
			}

			public void DefineRank(int rank)
			{
				this.rank = rank;
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
				else if ((object.ReferenceEquals (this.visual, other.visual)) &&
					/**/ ((this.rank == 0) || (other.rank == 0)))
				{
					return 0;
				}
				else
				{
					return this.rank.CompareTo (other.rank);
				}
			}

			#endregion
			
			private Visual						visual;
			private int							depth;
			private int							rank;
		}

		public static void SetLayoutContext(Visual visual, LayoutContext context)
		{
			visual.SetValue (LayoutContext.LayoutContextProperty, context);
		}
		public static LayoutContext GetLayoutContext(Visual visual)
		{
			return visual.GetValue (LayoutContext.LayoutContextProperty) as LayoutContext;
		}

		public static Types.DependencyProperty	LayoutContextProperty = Types.DependencyProperty.RegisterAttached ("LayoutContext", typeof (LayoutContext), typeof (LayoutContext));
		
		private static int						nextPassId = 0;
		
		private int passId;
		private int nodeRank;

		private SortedList<VisualNode, Visual> measureQueue = new SortedList<VisualNode, Visual> ();
		private SortedList<VisualNode, Visual> arrangeQueue = new SortedList<VisualNode, Visual> ();
		private int measureChanges;
		private int arrangeChanges;
		private Visual cacheVisual;
		private LayoutMeasure cacheWidthMeasure;
		private LayoutMeasure cacheHeightMeasure;
	}
}
