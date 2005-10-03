//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe ChildrenCollection regroupe dans une collection unique tous les
	/// widgets qui constituent les enfants d'un widget donné.
	/// </summary>
	public class ChildrenCollection : System.Collections.IList
	{
		public ChildrenCollection(Visual visual)
		{
			this.visual = visual;
		}
		
		
		public Visual							this[int index]
		{
			get
			{
				if ((index > -1) &&
					(this.visual.HasLayerCollection))
				{
					Collections.LayerCollection layers = this.visual.GetLayerCollection ();
				
					for (int i = 0; i < layers.Count; i++)
					{
						int children = layers[i].Children.Count;
						
						if (index < children)
						{
							return layers[i].Children[index];
						}
						
						index -= children;
					}
				}
				
				throw new System.ArgumentOutOfRangeException ("index");
			}
		}
		
		
		public Visual[] ToArray()
		{
			if (this.visual.HasLayerCollection)
			{
				Collections.LayerCollection layers = this.visual.GetLayerCollection ();
				
				switch (layers.Count)
				{
					case 0: return new Visual[0];
					case 1: return layers[0].Children.ToArray ();
				}
				
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (Layouts.Layer layer in layers)
				{
					list.AddRange (layer.Children);
				}
				
				return (Visual[]) list.ToArray (typeof (Visual));
			}
			else
			{
				return new Visual[0];
			}
		}
		
		
		public Visual FindNext(Visual visual)
		{
			Visual[] visuals = this.ToArray ();
			
			for (int i = 0; i < visuals.Length; i++)
			{
				if (visuals[i] == visual)
				{
					return (++i < visuals.Length) ? visuals[i] : null;
				}
			}
			
			return null;
		}
		
		public Visual FindPrevious(Visual visual)
		{
			Visual[] visuals = this.ToArray ();
			
			for (int i = 0; i < visuals.Length; i++)
			{
				if (visuals[i] == visual)
				{
					return (--i >= 0) ? visuals[i] : null;
				}
			}
			
			return null;
		}
		
		
		public int IndexOf(Visual visual)
		{
			Visual[] visuals = this.ToArray ();
			
			for (int i = 0; i < visuals.Length; i++)
			{
				if (visuals[i] == visual)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		public bool Contains(Visual visual)
		{
			return this.IndexOf (visual) < 0 ? false : true;
		}
		
		
		public void Add(Visual visual)
		{
			Collections.LayerCollection layers = this.visual.GetLayerCollection ();
			
			if (layers.Count == 0)
			{
				lock (this.visual)
				{
					if (layers.Count == 0)
					{
						layers.AddLayer ();
					}
				}
			}
			
			if (this.Contains (visual))
			{
				//	Ne fait rien: le widget est déjà contenu dans la liste des
				//	enfants.
			}
			else
			{
				layers[0].Children.Add (visual);
			}
		}
		
		public void Remove(Visual visual)
		{
			if ((visual != null) &&
				(visual.ParentLayer != null))
			{
				visual.ParentLayer.Children.Remove (visual);
			}
		}
		
		public void Clear()
		{
			if (this.visual.HasLayerCollection)
			{
				Collections.LayerCollection layers = this.visual.GetLayerCollection ();
				
				foreach (Layouts.Layer layer in layers)
				{
					layer.Children.Clear ();
				}
			}
		}

		
		#region IList Members
		public bool								IsReadOnly
		{
			get
			{
				return false;
			}
		}
		
		public bool								IsFixedSize
		{
			get
			{
				return false;
			}
		}
		
		
		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}
		
		void System.Collections.IList.RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}
		
		void System.Collections.IList.Insert(int index, object value)
		{
			throw new System.InvalidOperationException ();
		}
		
		void System.Collections.IList.Remove(object value)
		{
			this.Remove (value as Visual);
		}
		
		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value as Visual);
		}
		
		void System.Collections.IList.Clear()
		{
			this.Clear ();
		}
		
		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf (value as Visual);
		}
		
		int System.Collections.IList.Add(object value)
		{
			this.Add (value as Visual);
			return this.IndexOf (value as Visual);
		}
		#endregion
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return this;
			}
		}
		
		public int								Count
		{
			get
			{
				int count = 0;
				
				if (this.visual.HasLayerCollection)
				{
					Collections.LayerCollection layers = this.visual.GetLayerCollection ();
					
					for (int i = 0; i < layers.Count; i++)
					{
						count += layers[i].Children.Count;
					}
				}
				
				return count;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			Visual[] visuals = this.ToArray ();
			visuals.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return new ChildrenCollectionEnumerator (this.visual);
		}
		#endregion
		
		#region ChildrenCollectionEnumerator Class
		private class ChildrenCollectionEnumerator : System.Collections.IEnumerator
		{
			public ChildrenCollectionEnumerator(Visual visual)
			{
				this.visual = visual;
			}
			
			
			public object						Current
			{
				get
				{
					return this.visual.GetLayerCollection ()[this.layer_index].Children[this.child_index];
				}
			}
			
			
			public void Reset()
			{
				this.layer_index = 0;
				this.child_index = -1;
			}
			
			public bool MoveNext()
			{
				if (this.visual.HasLayerCollection)
				{
					Collections.LayerCollection layers = this.visual.GetLayerCollection ();
					
					while (this.layer_index < layers.Count)
					{
						if (++this.child_index < layers[this.layer_index].Children.Count)
						{
							return true;
						}
						else
						{
							this.child_index = -1;
							this.layer_index++;
						}
					}
				}
				
				return false;
			}
			
			
			private Visual						visual;
			private int							layer_index;
			private int							child_index;
		}
		#endregion
		
		Visual							visual;
	}
}
