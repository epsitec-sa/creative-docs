//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Common.Widgets.Collections
{
	public class LayerCollection : System.Collections.IList
	{
		internal LayerCollection(Visual parent)
		{
			this.parent = parent;
		}
		
		public bool								HasLayers
		{
			get
			{
				if ((this.list == null) ||
					(this.list.Count == 0))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		public Visual							ParentVisual
		{
			get
			{
				return this.parent;
			}
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
				if (this.list != null)
				{
					foreach (Layer layer in this.list)
					{
						if (layer.Name == name)
						{
							return layer;
						}
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
			Layer layer = new Layer (this.parent);
			
			layer.Name = name;

			this.EnsureListExists ();
			this.list.Add (layer);
			this.NotifyLayerInsertion (layer);
			
			return layer;
		}
		
		public bool RemoveLayer(Layer layer)
		{
			if ((this.list != null) &&
				(this.list.Contains (layer)))
			{
				this.list.Remove (layer);
				this.NotifyLayerRemoval (layer);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public bool Contains(Layer layer)
		{
			if (this.list == null)
			{
				return false;
			}
			else
			{
				return this.list.Contains (layer);
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
			if ((this.list != null) &&
				(this.list.Count > 0))
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
				return false;
			}
		}

		public int								Count
		{
			get
			{
				if (this.list == null)
				{
					return 0;
				}
				else
				{
					return this.list.Count;
				}
			}
		}

		public object							SyncRoot
		{
			get
			{
				this.EnsureListExists ();
				
				return this.list.SyncRoot;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			if (this.list != null)
			{
				this.list.CopyTo (array, index);
			}
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			this.EnsureListExists ();
			return this.list.GetEnumerator ();
		}
		#endregion

		private void EnsureListExists()
		{
			if (this.list == null)
			{
				lock (this)
				{
					if (this.list == null)
					{
						this.list = new System.Collections.ArrayList ();
					}
				}
			}
		}

		private void NotifyLayerInsertion(Layer item)
		{
			this.parent.NotifyLayersChanged ();
		}
		private void NotifyLayerRemoval(Layer item)
		{
			this.parent.NotifyLayersChanged ();
		}
		
		private System.Collections.ArrayList	list;
		private Visual							parent;
	}
}
