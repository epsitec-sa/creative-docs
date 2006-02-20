//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<PropertyChangedEventArgs>;

	/// <summary>
	/// La classe Object représente un objet dont les propriétés sont
	/// stockées dans un dictionnaire interne plutôt que dans des variables, ce
	/// qui permet une plus grande souplesse (valeurs par défaut, introspection,
	/// sérialisation, styles, génération automatique d'événements, etc.)
	/// </summary>
	public abstract class Object : System.IDisposable
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
		public IEnumerable<LocalValueEntry>		LocalValueEntries
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
			if (this.properties.ContainsKey (property))
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
			if (this.properties.ContainsKey (property))
			{
				this.properties.Remove (property);
			}
		}
		public bool ContainsLocalValue(Property property)
		{
			return this.properties.ContainsKey (property);
		}

		public void InvalidateProperty(Property property, object old_value, object new_value)
		{
			PropertyMetadata metadata = property.GetMetadata (this);

			metadata.NotifyPropertyInvalidated (this, old_value, new_value);

			if (this.HasEventHandlerForProperty (property))
			{
				PropertyChangedEventHandler handler = this.propertyEvents[property];
				PropertyChangedEventArgs args = new PropertyChangedEventArgs (property, old_value, new_value);

				handler (this, args);
			}
		}
		
		public void AddEventHandler(Property property, PropertyChangedEventHandler handler)
		{
			if (this.propertyEvents == null)
			{
				if (this.propertyEvents == null)
				{
					this.propertyEvents = new Dictionary<Property, PropertyChangedEventHandler> ();
				}
			}

			if (this.propertyEvents.ContainsKey (property))
			{
				this.propertyEvents[property] = this.propertyEvents[property] + handler;
			}
			else
			{
				this.propertyEvents[property] = handler;
			}
		}
		public void RemoveEventHandler(Property property, PropertyChangedEventHandler handler)
		{
			if ((this.propertyEvents != null) &&
				(this.propertyEvents.ContainsKey (property)))
			{
				this.propertyEvents[property] = this.propertyEvents[property] - handler;
			}
		}
		
		public bool HasEventHandlerForProperty(Property property)
		{
			if ((this.propertyEvents != null) &&
				(this.propertyEvents.ContainsKey (property)) &&
				(this.propertyEvents[property] != null))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public Binding GetBinding(Property property)
		{
			if ((this.bindings != null) &&
				(this.bindings.ContainsKey (property)))
			{
				return this.bindings[property].Binding;
			}
			else
			{
				return null;
			}
		}
		public void SetBinding(Property property, Binding binding)
		{
			if (property == null)
			{
				throw new System.ArgumentNullException ("property");
			}
			if (binding == null)
			{
				throw new System.ArgumentNullException ("binding");
			}
			
			if (this.bindings == null)
			{
				this.bindings = new Dictionary<Property, BindingExpression> ();
			}
			
			this.bindings[property] = BindingExpression.BindToTarget (this, property, binding);
		}
		public void ClearAllBindings()
		{
			Property[] properties = Copier.CopyArray (this.bindings.Keys);

			for (int i = 0; i < properties.Length; i++)
			{
				this.ClearBinding (properties[i]);
			}
		}
		public void ClearBinding(Property property)
		{
			if ((this.bindings != null) &&
				(this.bindings.ContainsKey (property)))
			{
				this.bindings[property].Dispose ();
				this.bindings.Remove (property);
			}
		}
		public bool IsDataBound(Property property)
		{
			if ((this.bindings != null) &&
				(this.bindings.ContainsKey (property)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		protected void AddUserEventHandler(string name, System.Delegate handler)
		{
			if (this.userEvents == null)
			{
				if (this.userEvents == null)
				{
					this.userEvents = new Dictionary<string, System.Delegate> ();
				}
			}

			if (this.userEvents.ContainsKey (name))
			{
				this.userEvents[name] = System.Delegate.Combine (this.userEvents[name], handler);
			}
			else
			{
				this.userEvents[name] = handler;
			}
		}
		protected void RemoveUserEventHandler(string name, System.Delegate handler)
		{
			if ((this.userEvents != null) &&
				(this.userEvents.ContainsKey (name)))
			{
				this.userEvents[name] = System.Delegate.Remove (this.userEvents[name], handler);
			}
		}

		protected System.Delegate GetUserEventHandler(string name)
		{
			if ((this.userEvents != null) &&
				(this.userEvents.ContainsKey (name)))
			{
				return this.userEvents[name];
			}
			else
			{
				return null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		internal static void Register(Property property)
		{
			System.Diagnostics.Debug.Assert (property != null);
			
			lock (Object.declarations)
			{
				TypeDeclaration type_declaration;

				if (Object.declarations.ContainsKey (property.OwnerType) == false)
				{
					type_declaration = new TypeDeclaration ();
					type_declaration[property.Name] = property;
					Object.declarations[property.OwnerType] = type_declaration;
				}
				else
				{
					type_declaration = Object.declarations[property.OwnerType];
					
					if (type_declaration.ContainsKey (property.Name))
					{
						throw new System.ArgumentException (string.Format ("Property named {0} already exists for type {1}", property.Name, property.OwnerType));
					}
					else
					{
						type_declaration[property.Name] = property;
					}
				}
				
				ObjectType.FromSystemType (property.OwnerType).Register (property);
			}
		}


		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		#region LocalValueEnumerator Class
		private struct LocalValueEnumerator : IEnumerator<LocalValueEntry>, IEnumerable<LocalValueEntry>
		{
			public LocalValueEnumerator(Object o)
			{
				this.property_enumerator = o.properties.GetEnumerator ();
			}
			
			public LocalValueEntry Current
			{
				get
				{
					return new LocalValueEntry (this.property_enumerator.Current);
				}
			}
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return this.Current;
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
			public void Dispose()
			{
			}
			
			public IEnumerator<LocalValueEntry> GetEnumerator()
			{
				return this;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this;
			}

			IEnumerator<KeyValuePair<Property, object>> property_enumerator;
		}
		#endregion

		#region TypeDeclaration Class
		private class TypeDeclaration : Dictionary<string, Property>
		{
		}
		#endregion


		Dictionary<Property, object>						properties = new Dictionary<Property, object> ();
		Dictionary<Property, BindingExpression>				bindings;
		Dictionary<Property, PropertyChangedEventHandler>	propertyEvents;
		Dictionary<string, System.Delegate>					userEvents;
		ObjectType											cached_type;

		static Dictionary<System.Type, TypeDeclaration>		declarations = new Dictionary<System.Type, TypeDeclaration> ();
	}
}
