//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public static void RemoveFromMeasureQueue(Visual visual)
		{
			if (visual != null)
			{
				int depth;
				LayoutContext context = Helpers.VisualTree.FindLayoutContext (visual, out depth);
				
				if (context != null)
				{
					context.RemoveFromMeasureQueue (visual, depth);
				}
			}
		}
		
		private void AddToMeasureQueue(Visual visual, int depth)
		{
			System.Diagnostics.Debug.Assert (visual != null);
			
			int rank = ++this.nodeRank;
			VisualNode node = new VisualNode (visual, 0, SortMode.Measure, depth);

			if (this.measureQueue.ContainsKey (node))
			{
				this.measureQueue.Remove (node);
			}
			else
			{
				this.measureChanges++;
			}
			
			node.DefineRank (rank);
			this.measureQueue.Add (node, visual);
		}
		private void AddToArrangeQueue(Visual visual, int depth)
		{
			System.Diagnostics.Debug.Assert (visual != null);
			
			int rank = ++this.nodeRank;
			VisualNode node = new VisualNode (visual, 0, SortMode.Arrange, depth);
			
			if (this.arrangeQueue.ContainsKey (node))
			{
				this.arrangeQueue.Remove (node);
			}
			else
			{
				this.arrangeChanges++;
			}
			
			node.DefineRank (rank);
			this.arrangeQueue[node] = visual;
		}
		
		private void RemoveFromMeasureQueue(Visual visual, int depth)
		{
			System.Diagnostics.Debug.Assert (visual != null);

			VisualNode node = new VisualNode (visual, 0, SortMode.Measure, depth);
			this.measureQueue.Remove (node);
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
					//	The visual has specified other measures. Its contents
					//	will have to be arranged.
					
					this.AddToArrangeQueue (node.Visual, node.Depth);
					
					//	If the visual is either docked or anchored, then the
					//	contents of the parent needs to be re-arranged too.

					if (node.Depth > 1)
					{
						LayoutMode mode = LayoutEngine.GetLayoutMode (node.Visual);

						if (mode != LayoutMode.None)
						{
							Visual parent = node.Visual.Parent;
							int depth = node.Depth - 1;

							System.Diagnostics.Debug.Assert (Helpers.VisualTree.FindLayoutContext (node.Visual) == this);
							System.Diagnostics.Debug.Assert (parent != null);
							
							this.AddToArrangeQueue (parent, depth);
						}
					}
				}

				System.Diagnostics.Debug.Assert (this.cacheVisual == node.Visual);

				this.cacheVisual = null;
				this.cacheWidthMeasure = null;
				this.cacheHeightMeasure = null;
			}
		}
		
		public void ExecuteArrange()
		{
			if (this.measureQueue.Count > 0)
			{
				this.ExecuteMeasure ();
			}
			
			//	Arrange all widgets which have been queued to be arranged.
			//	Start near the root and walk down through the children.
			
			//	Abort as soon as there are elements in the measure queue.

			while (this.arrangeQueue.Count > 0)
			{
				VisualNode node = this.arrangeQueue.Keys[0];

				node.Visual.Arrange (this);

				if (this.measureQueue.Count > 0)
				{
					break;
				}
				
				this.arrangeQueue.RemoveAt (0);
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

		internal static Drawing.Size GetResultingMeasuredSize(Visual visual)
		{
			LayoutMeasure measureWidth = LayoutMeasure.GetWidth (visual);
			LayoutMeasure measureHeight = LayoutMeasure.GetHeight (visual);

			if ((measureWidth == null) ||
				(measureHeight == null))
			{
				LayoutContext.AddToMeasureQueue (visual);

				return Drawing.Size.NegativeInfinity;
			}
			
			return new Drawing.Size (measureWidth.Desired, measureHeight.Desired);
		}
	}
}
