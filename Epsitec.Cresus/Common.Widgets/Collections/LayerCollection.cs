//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Common.Widgets.Collections
{
	public sealed class LayerCollection : System.Collections.IList
	{
		public LayerCollection(Visual visual)
		{
			this.visual = visual;
			this.list   = new System.Collections.ArrayList ();
		}
		
		
		public Layer							this[int index]
		{
			get
			{
				return this.list[index] as Layer;
			}
		}
		
		public Layer							this[string name]
		{
			get
			{
				foreach (Layer layer in this.list)
				{
					if (layer.Name == name)
					{
						return layer;
					}
				}
				
				return null;
			}
		}
		
		
		public Layer AddLayer()
		{
			return this.AddLayer ("");
		}
		
		public Layer AddLayer(string name)
		{
			Layer layer = new Layer (this.visual);
			
			layer.Name = name;
			
			this.list.Add (layer);
			this.NotifyLayerInsertion (layer);
			
			return layer;
		}
		
		public void RemoveLayer(Layer layer)
		{
			if (this.list.Contains (layer))
			{
				this.list.Remove (layer);
				this.NotifyLayerRemoval (layer);
			}
		}
		
		
		public bool Contains(Layer layer)
		{
			return this.list.Contains (layer);
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
				return this.list.IsFixedSize;
			}
		}
		
		
		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				throw new System.InvalidOperationException ("Indexed set prohibited");
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
			this.RemoveLayer (value as Layer);
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value as Layer);
		}

		public void Clear()
		{
			if (this.list.Count > 0)
			{
				Layer[] layers = (Layer[]) this.list.ToArray (typeof (Layer));
				
				this.list.Clear ();
				
				for (int i = 0; i < layers.Length; i++)
				{
					this.NotifyLayerRemoval (layers[i]);
				}
			}
		}

		int System.Collections.IList.IndexOf(object value)
		{
			throw new System.InvalidOperationException ();
		}
		
		int System.Collections.IList.Add(object value)
		{
			throw new System.InvalidOperationException ();
		}
		#endregion
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public object							SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		private void NotifyLayerInsertion(Layer item)
		{
		}
		
		private void NotifyLayerRemoval(Layer item)
		{
		}
		
		
		private Visual							visual;
		private System.Collections.ArrayList	list;
	}
}
