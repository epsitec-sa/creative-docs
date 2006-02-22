//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe ChildrenCollection regroupe dans une collection unique tous les
	/// widgets qui constituent les enfants d'un widget donné.
	/// </summary>
	public struct ChildrenCollection : System.Collections.IList, ICollection<Types.Object>
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
					(this.visual.HasLayers))
				{
					Collections.LayerCollection layers = this.visual.Layers;
				
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
		public Widget[]							Widgets
		{
			get
			{
				Visual[] visuals = this.ToArray ();
				Widget[] widgets = new Widget[visuals.Length];
				
				for (int i = 0; i < widgets.Length; i++)
				{
					widgets[i] = visuals[i] as Widget;
				}
				
				return widgets;
			}
		}
		
		public Visual[] ToArray()
		{
			if (this.visual.HasLayers)
			{
				Collections.LayerCollection layers = this.visual.Layers;
				
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
			if (this.Contains (visual))
			{
				//	Ne fait rien: le widget est déjà contenu dans la liste des
				//	enfants.
			}
			else
			{
				this.visual.GetDefaultLayer ().Children.Add (visual);
			}
		}
		public bool Remove(Visual visual)
		{
			if ((visual != null) &&
				(visual.ParentLayer != null))
			{
				return visual.ParentLayer.Children.Remove (visual);
			}
			else
			{
				return false;
			}
		}
		public void Clear()
		{
			if (this.visual.HasLayers)
			{
				Collections.LayerCollection layers = this.visual.Layers;
				
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
				
				if (this.visual.HasLayers)
				{
					Collections.LayerCollection layers = this.visual.Layers;
					
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
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		#endregion

		#region ICollection<Object> Members
		void ICollection<Types.Object>.Add(Types.Object item)
		{
			this.Add (item as Visual);
		}

		void ICollection<Types.Object>.Clear()
		{
			this.Clear ();
		}

		bool ICollection<Types.Object>.Contains(Types.Object item)
		{
			return this.Contains (item as Visual);
		}

		void ICollection<Types.Object>.CopyTo(Types.Object[] array, int index)
		{
			this.CopyTo (array, index);
		}

		int ICollection<Types.Object>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<Types.Object>.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		bool ICollection<Types.Object>.Remove(Types.Object item)
		{
			return this.Remove (item as Visual);
		}
		#endregion

		#region IEnumerable<Object> Members
		public IEnumerator<Types.Object> GetEnumerator()
		{
			return new ChildrenCollectionEnumerator (this.visual);
		}
		#endregion

		#region ChildrenCollectionEnumerator Class
		private class ChildrenCollectionEnumerator : System.Collections.IEnumerator, IEnumerator<Types.Object>
		{
			public ChildrenCollectionEnumerator(Visual visual)
			{
				this.visual = visual;
				this.Reset ();
			}

			#region IEnumerator Members
			object								System.Collections.IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}
			
			public void Reset()
			{
				this.layer_index = 0;
				this.child_index = -1;
			}
			public bool MoveNext()
			{
				if (this.visual.HasLayers)
				{
					Collections.LayerCollection layers = this.visual.Layers;
					
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
			#endregion

			#region IEnumerator<Object> Members
			public Types.Object Current
			{
				get
				{
					return this.visual.Layers[this.layer_index].Children[this.child_index];
				}
			}
			#endregion

			#region IDisposable Members
			public void Dispose()
			{
			}
			#endregion
			
			private Visual						visual;
			private int							layer_index;
			private int							child_index;
		}
		#endregion

		private Visual							visual;
	}
}
