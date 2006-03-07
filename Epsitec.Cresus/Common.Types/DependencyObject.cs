//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;

	/// <summary>
	/// La classe DependencyObject représente un objet dont les propriétés sont
	/// stockées dans un dictionnaire interne plutôt que dans des variables, ce
	/// qui permet une plus grande souplesse (valeurs par défaut, introspection,
	/// sérialisation, styles, génération automatique d'événements, etc.)
	/// </summary>
	public abstract class DependencyObject : System.IDisposable
	{
		protected DependencyObject()
		{
			this.cachedType = DependencyObjectType.SetupFromSystemType (this.GetType ());
		}
		
		public DependencyObjectType						ObjectType
		{
			get
			{
				return this.cachedType;
			}
		}
		public IEnumerable<LocalValueEntry>		LocalValueEntries
		{
			get
			{
				return new LocalValueEnumerator (this);
			}
		}
		
		public static int						RegisteredPropertyCount
		{
			get
			{
				return DependencyObject.registeredPropertyCount;
			}
		}
		
		public object GetValue(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);
			
			if (metadata.GetValueOverride != null)
			{
				return metadata.GetValueOverride (this);
			}
			else
			{
				return this.GetValueBase (property);
			}
		}
		public object GetValueBase(DependencyProperty property)
		{
			object value = this.GetLocalValue (property);
			
			//	Si la valeur n'est pas définie localement, il faut déterminer la
			//	valeur réelle (par défaut, héritée, etc.)
			
			if (value == UndefinedValue.Instance)
			{
				DependencyPropertyMetadata metadata = property.GetMetadata (this);

				if (metadata.InheritsValue)
				{
					//	TODO: trouver la valeur héritée
				}
				else
				{
					//	TODO: faire mieux...
				}
				
				value = metadata.CreateDefaultValue ();
			}
			
			return value;
		}
		
		public void SetValue(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			if (metadata.CoerceValue != null)
			{
				value = metadata.CoerceValue (this, property, value);
			}
			
			if (metadata.SetValueOverride != null)
			{
				metadata.SetValueOverride (this, value);
			}
			else
			{
				this.SetValueBase (property, value);
			}
		}
		public void SetValueBase(DependencyProperty property, object value)
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
		public void ClearValueBase(DependencyProperty property)
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
		
		public object CoerceValue(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			if (metadata.CoerceValue != null)
			{
				return metadata.CoerceValue (this, property, value);
			}
			else
			{
				return value;
			}
		}
		
		public object GetLocalValue(DependencyProperty property)
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
		public void SetLocalValue(DependencyProperty property, object value)
		{
			this.properties[property] = value;
		}
		public void ClearLocalValue(DependencyProperty property)
		{
			if (this.properties.ContainsKey (property))
			{
				this.properties.Remove (property);
			}
		}
		public bool ContainsLocalValue(DependencyProperty property)
		{
			return this.properties.ContainsKey (property);
		}

		public void InvalidateProperty(DependencyProperty property, object old_value, object new_value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			metadata.NotifyPropertyInvalidated (this, old_value, new_value);

			if (this.HasEventHandlerForProperty (property))
			{
				PropertyChangedEventHandler handler = this.propertyEvents[property];
				DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs (property, old_value, new_value);

				handler (this, args);
			}
		}
		
		public void AddEventHandler(DependencyProperty property, PropertyChangedEventHandler handler)
		{
			if (this.propertyEvents == null)
			{
				if (this.propertyEvents == null)
				{
					this.propertyEvents = new Dictionary<DependencyProperty, PropertyChangedEventHandler> ();
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
		public void RemoveEventHandler(DependencyProperty property, PropertyChangedEventHandler handler)
		{
			if ((this.propertyEvents != null) &&
				(this.propertyEvents.ContainsKey (property)))
			{
				this.propertyEvents[property] = this.propertyEvents[property] - handler;
			}
		}
		
		public bool HasEventHandlerForProperty(DependencyProperty property)
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

		public Binding GetBinding(DependencyProperty property)
		{
			if ((this.bindings != null) &&
				(this.bindings.ContainsKey (property)))
			{
				return this.bindings[property].ParentBinding;
			}
			else
			{
				return null;
			}
		}
		public void SetBinding(DependencyProperty property, Binding binding)
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
				this.bindings = new Dictionary<DependencyProperty, BindingExpression> ();
			}
			
			this.bindings[property] = BindingExpression.BindToTarget (this, property, binding);
		}
		public void ClearAllBindings()
		{
			DependencyProperty[] properties = Copier.CopyArray (this.bindings.Keys);

			for (int i = 0; i < properties.Length; i++)
			{
				this.ClearBinding (properties[i]);
			}
		}
		public void ClearBinding(DependencyProperty property)
		{
			if ((this.bindings != null) &&
				(this.bindings.ContainsKey (property)))
			{
				this.bindings[property].Dispose ();
				this.bindings.Remove (property);
			}
		}
		public bool IsDataBound(DependencyProperty property)
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

		internal static void Register(DependencyProperty property, System.Type ownerType)
		{
			System.Diagnostics.Debug.Assert (property != null);
			System.Diagnostics.Debug.Assert (ownerType != null);

			lock (DependencyObject.declarations)
			{
				TypeDeclaration typeDeclaration;

				if (DependencyObject.declarations.ContainsKey (ownerType) == false)
				{
					typeDeclaration = new TypeDeclaration ();
					typeDeclaration[property.Name] = property;
					DependencyObject.declarations[ownerType] = typeDeclaration;
				}
				else
				{
					typeDeclaration = DependencyObject.declarations[ownerType];
					
					if (typeDeclaration.ContainsKey (property.Name))
					{
						throw new System.ArgumentException (string.Format ("DependencyProperty named {0} already exists for type {1}", property.Name, ownerType));
					}
					else
					{
						typeDeclaration[property.Name] = property;
					}
				}

				DependencyObjectType type = DependencyObjectType.FromSystemType (ownerType);
				type.Register (property);

				System.Threading.Interlocked.Increment (ref DependencyObject.registeredPropertyCount);
			}
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		#region Private LocalValueEnumerator Structure
		private struct LocalValueEnumerator : IEnumerator<LocalValueEntry>, IEnumerable<LocalValueEntry>
		{
			public LocalValueEnumerator(DependencyObject o)
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

			IEnumerator<KeyValuePair<DependencyProperty, object>> property_enumerator;
		}
		#endregion

		#region Private TypeDeclaration Class
		private class TypeDeclaration : Dictionary<string, DependencyProperty>
		{
		}
		#endregion
		
		

		Dictionary<DependencyProperty, object>						properties = new Dictionary<DependencyProperty, object> ();
		Dictionary<DependencyProperty, BindingExpression>				bindings;
		Dictionary<DependencyProperty, PropertyChangedEventHandler>	propertyEvents;
		Dictionary<string, System.Delegate>					userEvents;
		DependencyObjectType											cachedType;

		static Dictionary<System.Type, TypeDeclaration>		declarations = new Dictionary<System.Type, TypeDeclaration> ();
		static int											registeredPropertyCount;
	}
}
