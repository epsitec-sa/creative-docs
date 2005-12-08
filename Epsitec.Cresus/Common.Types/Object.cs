//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Object représente un objet dont les propriétés sont
	/// stockées dans un dictionnaire interne plutôt que dans des variables, ce
	/// qui permet une plus grande souplesse (valeurs par défaut, introspection,
	/// sérialisation, styles, génération automatique d'événements, etc.)
	/// </summary>
	public abstract class Object
	{
		protected Object()
		{
		}
		
		
		public ObjectType						ObjectType
		{
			get
			{
				if (this.cached_type == null)
				{
					lock (this.properties)
					{
						if (this.cached_type == null)
						{
							this.cached_type = ObjectType.FromSystemType (this.GetType ());
						}
					}
				}
				
				return this.cached_type;
			}
		}
		
		public System.Collections.IEnumerable	LocalValueEntries
		{
			get
			{
				return new LocalValueEnumerator (this);
			}
		}
		
		
		public object GetValue(Property property)
		{
			PropertyMetadata metadata = property.GetMetadata (this);
			
			if (metadata.GetValueOverride != null)
			{
				return metadata.GetValueOverride (this);
			}
			else
			{
				return this.GetValueBase (property);
			}
		}
		
		public object GetValueBase(Property property)
		{
			object value = this.GetLocalValue (property);
			
			//	Si la valeur n'est pas définie localement, il faut déterminer la
			//	valeur réelle (par défaut, héritée, etc.)
			
			if (value == UndefinedValue.Instance)
			{
				PropertyMetadata metadata = property.GetMetadata (this);
				
				//	TODO: faire mieux...
				
				value = metadata.CreateDefaultValue ();
			}
			
			return value;
		}
		
		
		public void SetValue(Property property, object value)
		{
			PropertyMetadata metadata = property.GetMetadata (this);
			
			if (metadata.SetValueOverride != null)
			{
				metadata.SetValueOverride (this, value);
			}
			else
			{
				this.SetValueBase (property, value);
			}
		}
		
		public void SetValueBase(Property property, object value)
		{
			object old_value = this.GetValue (property);
			
			this.SetLocalValue (property, value);
			
			object new_value = this.GetValue (property);
			
			if (old_value == new_value)
			{
				//	C'est exactement la même valeur -- on ne signale donc rien ici.
			}
			else if ((old_value == null) || (! old_value.Equals (new_value)))
			{
				this.InvalidateProperty (property, old_value, new_value);
			}
		}
		
		public void ClearValueBase(Property property)
		{
			object old_value = this.GetValue (property);
			
			this.ClearLocalValue (property);
			
			object new_value = this.GetValue (property);
			
			if (old_value == new_value)
			{
				//	C'est exactement la même valeur -- on ne signale donc rien ici.
			}
			else if ((old_value == null) || (! old_value.Equals (new_value)))
			{
				this.InvalidateProperty (property, old_value, new_value);
			}
		}
		
		
		public object GetLocalValue(Property property)
		{
			if (this.properties.Contains (property))
			{
				return this.properties[property];
			}
			else
			{
				return UndefinedValue.Instance;
			}
		}
		
		public void SetLocalValue(Property property, object value)
		{
			this.properties[property] = value;
		}
		
		public void ClearLocalValue(Property property)
		{
			if (this.properties.Contains (property))
			{
				this.properties.Remove (property);
			}
		}
		
		public bool ContainsLocalValue(Property property)
		{
			return this.properties.Contains (property);
		}
		
		
		public void AddEvent(Property property, PropertyChangedEventHandler handler)
		{
			this.InitialiseEventsHashtable ();
			this.events[property] = (PropertyChangedEventHandler) this.events[property] + handler;
		}
		
		public void AddEvent(string name, Support.EventHandler handler)
		{
			this.InitialiseEventsHashtable ();
			this.events[name] = (Support.EventHandler) this.events[name] + handler;
		}
		
		public void AddEvent(string name, Support.CancelEventHandler handler)
		{
			this.InitialiseEventsHashtable ();
			this.events[name] = (Support.CancelEventHandler) this.events[name] + handler;
		}
		
		public void RemoveEvent(Property property, PropertyChangedEventHandler handler)
		{
			if ((this.events != null) &&
				(this.events.Contains (property)))
			{
				this.events[property] = (PropertyChangedEventHandler) this.events[property] - handler;
			}
		}
		
		public void RemoveEvent(string name, Support.EventHandler handler)
		{
			if ((this.events != null) &&
				(this.events.Contains (name)))
			{
				this.events[name] = (Support.EventHandler) this.events[name] - handler;
			}
		}
		
		public void RemoveEvent(string name, Support.CancelEventHandler handler)
		{
			if ((this.events != null) &&
				(this.events.Contains (name)))
			{
				this.events[name] = (Support.CancelEventHandler) this.events[name] - handler;
			}
		}
		
		
		public bool IsEventUsed(Property property)
		{
			if (this.events != null)
			{
				if (this.events[property] != null)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public bool IsEventUsed(string name)
		{
			if (this.events != null)
			{
				if (this.events[name] != null)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public void InvalidateProperty(Property property, object old_value, object new_value)
		{
			PropertyMetadata metadata = property.GetMetadata (this);
			
			metadata.NotifyPropertyInvalidated (this, old_value, new_value);
			
			if (this.events != null)
			{
				PropertyChangedEventHandler handler = (PropertyChangedEventHandler) this.events[property];
				
				if (handler != null)
				{
					handler (this, new PropertyChangedEventArgs (property, old_value, new_value));
				}
			}
		}
		
		
		protected void InitialiseEventsHashtable()
		{
			if (this.events == null)
			{
				lock (this)
				{
					if (this.events == null)
					{
						this.events = new System.Collections.Hashtable ();
					}
				}
			}
		}
		
		protected void RaiseEvent(string name)
		{
			if (this.events != null)
			{
				Support.EventHandler handler = (Support.EventHandler) this.events[name];
				
				if (handler != null)
				{
					handler (this);
				}
			}
		}
		
		protected void RaiseEvent(string name, Support.CancelEventArgs e)
		{
			if (this.events != null)
			{
				Support.CancelEventHandler handler = (Support.CancelEventHandler) this.events[name];
				
				if (handler != null)
				{
					handler (this, e);
				}
			}
		}
		
		
		internal static void Register(Property property)
		{
			lock (Object.declarations)
			{
				System.Collections.Hashtable type_declaration = Object.declarations[property.OwnerType] as System.Collections.Hashtable;
				
				if (type_declaration == null)
				{
					type_declaration = new System.Collections.Hashtable ();
					type_declaration[property.Name] = property;
					Object.declarations[property.OwnerType] = type_declaration;
				}
				else if (type_declaration.Contains (property.Name))
				{
					throw new System.ArgumentException (string.Concat ("Property with name ", property.Name, " already exists for type ", property.OwnerType));
				}
				else
				{
					type_declaration[property.Name] = property;
				}
				
				ObjectType.FromSystemType (property.OwnerType).Register (property);
			}
		}
		
		
		private class LocalValueEnumerator : System.Collections.IEnumerator, System.Collections.IEnumerable
		{
			public LocalValueEnumerator(Object o)
			{
				this.property_enumerator = o.properties.GetEnumerator ();
			}
			
			
			public object						Current
			{
				get
				{
					System.Collections.DictionaryEntry entry = this.property_enumerator.Entry;
					return new LocalValueEntry (entry.Key as Property, entry.Value);
				}
			}
			
			
			public void Reset()
			{
				this.property_enumerator.Reset ();
			}
			
			public bool MoveNext()
			{
				return this.property_enumerator.MoveNext ();
			}
			
			
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}
			
			
			System.Collections.IDictionaryEnumerator property_enumerator;
		}
		
		
		private System.Collections.Hashtable	properties = new System.Collections.Hashtable ();
		protected System.Collections.Hashtable	events;
		private ObjectType						cached_type;
		
		static System.Collections.Hashtable		declarations = new System.Collections.Hashtable ();
	}
}
