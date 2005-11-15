//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	using EventHandler = Epsitec.Common.Support.EventHandler;
	
	/// <summary>
	/// La classe AbstractState sert de base � chaque �tat utilis� dans un
	/// AbstractWrapper.
	/// </summary>
	public abstract class AbstractState
	{
		protected AbstractState(AbstractWrapper wrapper, AccessMode access)
		{
			this.wrapper = wrapper;
			this.access  = access;
			
			this.wrapper.Register (this);
		}
		
		
		public AbstractWrapper					Wrapper
		{
			get
			{
				return this.wrapper;
			}
		}
		
		public AccessMode						AccessMode
		{
			get
			{
				return this.access;
			}
		}
		
		public bool								IsDirty
		{
			get
			{
				return this.is_dirty;
			}
		}
		
		
		public void DefineClean()
		{
			this.is_dirty = false;
		}
		
		
		internal bool IsValueDefined(StateProperty property)
		{
			return this.state.Contains (property);
		}
		
		internal void ClearValue(StateProperty property)
		{
			if (this.IsValueDefined (property))
			{
				this.state.Remove (property);
				this.is_dirty = true;
			}
		}
		
		internal object GetValue(StateProperty property)
		{
			if (this.IsValueDefined (property))
			{
				return this.state[property];
			}
			else
			{
				return property.DefaultValue;
			}
		}
		
		internal void SetValue(StateProperty property, object value)
		{
			if ((this.IsValueDefined (property) == false) ||
				(this.GetValue (property) != value))
			{
				if (this.access == AccessMode.ReadOnly)
				{
					throw new System.InvalidOperationException (string.Format ("Property {0} in {1} is read only", property.Name, this.GetType ().Name));
				}
				
				this.state[property] = value;
				this.wrapper.Synchronise (this, property);
				this.NotifyChanged (property);
			}
		}
		
		internal void DefineValue(StateProperty property, object value)
		{
			if ((this.IsValueDefined (property) == false) ||
				(this.GetValue (property) != value))
			{
				this.state[property] = value;
				this.is_dirty = true;
			}
		}
		
		internal void CopyFrom(AbstractState model)
		{
			this.state = model.state.Clone () as System.Collections.Hashtable;
		}
		
		
		internal void NotifyIfDirty()
		{
			if (this.IsDirty)
			{
				this.OnChanged ();
				this.DefineClean ();
			}
		}
		
		internal void NotifyChanged(StateProperty property)
		{
			this.OnChanged ();
		}
		
		internal void NotifyWrapperChanged()
		{
			this.OnChanged ();
		}
		
		
		private void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		public event EventHandler				Changed;
		
		private readonly AbstractWrapper		wrapper;
		private readonly AccessMode				access;
		private System.Collections.Hashtable	state = new System.Collections.Hashtable ();
		private bool							is_dirty;
	}
}
