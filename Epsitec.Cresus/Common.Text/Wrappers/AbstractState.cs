//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	using EventHandler = Epsitec.Common.Support.EventHandler;
	
	/// <summary>
	/// La classe AbstractState sert de base à chaque état utilisé dans un
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
		
		
		public void AddPendingProperty(StateProperty property)
		{
			this.pending_properties[property] = property;
		}
		
		public void RemovePendingProperty(StateProperty property)
		{
			this.pending_properties.Remove (property);
		}
		
		public void ClearPendingProperties()
		{
			this.pending_properties.Clear ();
		}
		
		
		public StateProperty[] GetPendingProperties()
		{
			StateProperty[] properties = new StateProperty[this.pending_properties.Count];
			this.pending_properties.Keys.CopyTo (properties, 0);
			return properties;
		}
		
		public StateProperty[] GetDefinedProperties()
		{
			StateProperty[] properties = new StateProperty[this.state.Count];
			this.state.Keys.CopyTo (properties, 0);
			return properties;
		}

		public void ClearDefinedProperties()
		{
			StateProperty[] properties = this.GetDefinedProperties ();

			if (properties.Length > 0)
			{
				this.wrapper.SuspendSynchronizations ();
				
				foreach (StateProperty property in properties)
				{
					this.state.Remove (property);
					this.flags[property] = true;
					this.wrapper.Synchronize (this, property);
				}
				
				this.wrapper.ResumeSynchronizations ();
			}
		}
		
		internal void FlagAllDefinedProperties()
		{
			this.flags.Clear ();
			
			foreach (System.Collections.DictionaryEntry entry in this.state)
			{
				this.flags[entry.Key] = true;
			}
		}
		
		internal bool IsValueDefined(StateProperty property)
		{
			return this.state.Contains (property);
		}
		
		internal bool IsValueFlagged(StateProperty property)
		{
			return this.flags.Contains (property);
		}
		
		internal bool ReadValueFlag(StateProperty property)
		{
			return (bool) this.flags[property];
		}
		
		internal void ClearValueFlags()
		{
			this.flags.Clear ();
		}
		
		internal int CountFlaggedValues()
		{
			return this.flags.Count;
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
		
		internal void ClearValue(StateProperty property)
		{
			bool changed = (this.IsValueDefined (property));
			
			this.state.Remove (property);
			this.flags[property] = changed;
			
			this.wrapper.Synchronize (this, property);
		}
		
		internal void SetValue(StateProperty property, object value)
		{
			bool changed = (this.IsValueDefined (property) == false) ||
				/**/	   (AbstractState.Equal (this.GetValue (property), value) == false);
			
			this.SetValue (property, value, changed);
		}
		
		internal void SetValue(StateProperty property, object value, bool changed)
		{
			if (this.access == AccessMode.ReadOnly)
			{
				throw new System.InvalidOperationException (string.Format ("Property {0} in {1} is read only", property.Name, this.GetType ().Name));
			}
			
			this.state[property] = value;
			this.flags[property] = changed;
			
			this.wrapper.Synchronize (this, property);
		}
		
		
		internal void DefineValue(StateProperty property, object value)
		{
			bool changed = (this.IsValueDefined (property) == false) ||
				/**/	   (AbstractState.Equal (this.GetValue (property), value) == false);
			
			this.DefineValue (property, value, changed);
		}
		
		static private bool Equal(object a, object b)
		{
			if (a == b)
			{
				return true;
			}
			
			if (a == null)
			{
				return false;
			}
			
			return a.Equals (b);
		}
		
		internal void DefineValue(StateProperty property, object value, bool changed)
		{
			if (changed)
			{
				this.state[property] = value;
				this.is_dirty = true;
			}
		}
		
		internal void DefineValue(StateProperty property)
		{
			if (this.IsValueDefined (property))
			{
				this.state.Remove (property);
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
		
		internal void NotifyChanged(StateProperty property, int change_id)
		{
			if (this.change_id != change_id)
			{
				this.change_id = change_id;
				this.OnChanged ();
			}
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
		private System.Collections.Hashtable	flags = new System.Collections.Hashtable ();
		private System.Collections.Hashtable	pending_properties = new System.Collections.Hashtable ();
		private int								change_id;
		private bool							is_dirty;
	}
}
