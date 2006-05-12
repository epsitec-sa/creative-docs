//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	public class AbstractStructuredData : IStructuredData
	{
		public AbstractStructuredData()
		{
		}

		#region IStructuredData Members

		public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			string key = path;
			PropertyChangedEventHandler value;

			if (this.listeners.TryGetValue (key, out value))
			{
				value += handler;
			}
			else
			{
				value = handler;
			}
			
			this.listeners[key] = value;
		}

		public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			string key = path;
			PropertyChangedEventHandler value;

			if (this.listeners.TryGetValue (key, out value))
			{
				value -= handler;
			}
			else
			{
				value = null;
			}

			if (value == null)
			{
				this.listeners.Remove (key);
			}
			else
			{
				this.listeners[key] = value;
			}
		}

		public object GetValue(string path)
		{
			object value = this.GetValueBase (path);
			this.UpdateCachedValue (path, value);
			return value;
		}

		public object GetValueType(string path)
		{
			//	TODO: ...

			throw new System.NotImplementedException ();
		}

		public void SetValue(string path, object value)
		{
			this.SetValueBase (path, value);
			this.UpdateCachedValue (path, value);
		}

		public bool HasImmutableRoots
		{
			get
			{
				return false;
			}
		}
		#endregion

		protected virtual object GetValueBase(string path)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}
		protected virtual void SetValueBase(string path, object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}
		
		protected void UpdateCachedValue(string path, object newValue)
		{
			object oldValue;

			if (this.cachedValues.TryGetValue (path, out oldValue))
			{

			}
			else
			{
				oldValue = UndefinedValue.Instance;
			}
			
			if (oldValue == newValue)
			{
				//	Same value, don't notify anything
			}
			else if ((oldValue == null) || (!oldValue.Equals (newValue)))
			{
				if (newValue == UndefinedValue.Instance)
				{
					this.cachedValues.Remove (path);
				}
				else
				{
					//	TODO: we should clone IClonable values to avoid sharing references between the cached value and the live value.
					
					this.cachedValues[path] = newValue;
				}
				
				this.NotifyChange (path, oldValue, newValue);
			}
			
		}
		
		protected void NotifyChange(string path, object oldValue, object newValue)
		{
			PropertyChangedEventHandler handler;
			
			if (this.listeners.TryGetValue (path, out handler))
			{
				if (handler != null)
				{
					DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs (path, oldValue, newValue);
					
					handler (this, args);
				}
			}
		}

		private Dictionary<string, PropertyChangedEventHandler> listeners = new Dictionary<string, PropertyChangedEventHandler> ();
		private Dictionary<string, object> cachedValues = new Dictionary<string, object> ();
	}
}
