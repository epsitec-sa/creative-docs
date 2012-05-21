//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

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

		public object SaveInternalState()
		{
			return new SavedState (this);
		}

		public void RestoreInternalState(object state)
		{
			SavedState savedState = state as SavedState;
			
			if (savedState != null)
			{
				savedState.RestoreToState (this);
				this.NotifyIfDirty ();
			}
		}
		
		public void AddPendingProperty(StateProperty property)
		{
			if (this.pending_properties.Contains (property) == false)
			{
				this.pending_properties.Add (property);
			}
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
			return this.pending_properties.ToArray ();
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
			
			foreach (KeyValuePair<StateProperty, object> pair in this.state)
			{
				this.flags[pair.Key] = true;
			}
		}
		
		internal bool IsValueDefined(StateProperty property)
		{
			return this.state.ContainsKey (property);
		}
		
		internal bool IsValueFlagged(StateProperty property)
		{
			return this.flags.ContainsKey (property);
		}
		
		internal bool ReadValueFlag(StateProperty property)
		{
			return this.flags[property];
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

		private class SavedState
		{
			public SavedState(AbstractState state)
			{
				SavedState.Copy (state.state, this.state);
				SavedState.Copy (state.flags, this.flags);
				SavedState.Copy (state.pending_properties, this.pending);
			}

			public void RestoreToState(AbstractState state)
			{
				List<StateProperty> add = new List<StateProperty> ();
				List<StateProperty> remove = new List<StateProperty> ();
				List<StateProperty> update = new List<StateProperty> ();
				
				foreach (StateProperty property in this.state.Keys)
				{
					if (state.state.ContainsKey (property))
					{
						update.Add (property);
					}
					else
					{
						//	The property does no longer exist in the current state; we
						//	will have to re-create it :

						add.Add (property);
					}
				}
				
				foreach (StateProperty property in state.state.Keys)
				{
					if (this.state.ContainsKey (property))
					{
						System.Diagnostics.Debug.Assert (update.Contains (property));
					}
					else
					{
						//	The current state contains a excess property; we will have
						//	to remove it :

						remove.Add (property);
					}
				}

				state.wrapper.SuspendSynchronizations ();
				
				foreach (StateProperty property in remove)
				{
					state.ClearValue (property);
				}
				foreach (StateProperty property in add)
				{
					state.DefineValue (property, this.state[property]);
				}
				foreach (StateProperty property in update)
				{
					state.DefineValue (property, this.state[property]);
				}
				
				state.wrapper.ResumeSynchronizations ();
			}
			
			private static void Copy(Dictionary<StateProperty, object> a, Dictionary<StateProperty, object> b)
			{
				b.Clear ();

				foreach (KeyValuePair<StateProperty, object> pair in a)
				{
					b.Add (pair.Key, pair.Value);
				}
			}

			private static void Copy(Dictionary<StateProperty, bool> a, Dictionary<StateProperty, bool> b)
			{
				b.Clear ();

				foreach (KeyValuePair<StateProperty, bool> pair in a)
				{
					b.Add (pair.Key, pair.Value);
				}
			}

			private static void Copy(List<StateProperty> a, List<StateProperty> b)
			{
				b.Clear ();

				foreach (StateProperty value in a)
				{
					b.Add (value);
				}
			}

			Dictionary<StateProperty, object> state = new Dictionary<StateProperty, object> ();
			Dictionary<StateProperty, bool> flags = new Dictionary<StateProperty, bool> ();
			List<StateProperty> pending = new List<StateProperty> ();
		}
		
		public event EventHandler				Changed;
		
		private readonly AbstractWrapper		wrapper;
		private readonly AccessMode				access;
		private Dictionary<StateProperty, object> state = new Dictionary<StateProperty, object> ();
		private Dictionary<StateProperty, bool> flags = new Dictionary<StateProperty, bool> ();
		private List<StateProperty>				pending_properties = new List<StateProperty> ();
		private int								change_id;
		private bool							is_dirty;
	}
}
