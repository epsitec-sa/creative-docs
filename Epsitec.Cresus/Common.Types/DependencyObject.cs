//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	using BindingChangedEventHandler = Epsitec.Common.Support.EventHandler<BindingChangedEventArgs>;

	/// <summary>
	/// The <c>DependencyObject</c> class represents an object which stores its
	/// properties and events in internal dictionnaries, instead of variables.
	/// This is useful for easy run-time analysis, handling zero-storage default
	/// values, automatic change event generation, etc.
	/// </summary>
	public abstract class DependencyObject : System.IDisposable, IInheritedPropertyCache, IStructuredData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DependencyObject"/>
		/// class. Since the class is <c>abstract</c>, the constructor is not
		/// publicly visible.
		/// </summary>
		protected DependencyObject()
		{
			this.cachedType = DependencyObjectType.SetupFromSystemType (this.GetType ());
		}

		/// <summary>
		/// Gets the type of the object (as a <see cref="T:DependencyObjectType"/>
		/// instance).
		/// </summary>
		/// <value>The type of the object.</value>
		public DependencyObjectType				ObjectType
		{
			get
			{
				return this.cachedType;
			}
		}

		/// <summary>
		/// Gets the enumeration of the local value entries, i.e. the values
		/// defined in the internal dictionary.
		/// </summary>
		/// <value>The enumeration of all local value entries.</value>
		public IEnumerable<LocalValueEntry>		LocalValueEntries
		{
			get
			{
				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return new LocalValueEntry (property, this.properties[property]);
				}
				
				//	Passe encore en revue les propriétés qui ne sont pas définies
				//	dans la variable 'properties' mais directement au moyen de
				//	callbacks GetValueOverrideCallback :

				DependencyObjectType type = this.ObjectType;
				System.Type sysType = this.GetType ();

				foreach (DependencyProperty property in type.GetProperties ())
				{
					if (this.properties.ContainsKey (property) == false)
					{
						DependencyPropertyMetadata metadata = property.GetMetadata (sysType);
						
						if (metadata.GetValueOverride != null)
						{
							object value = metadata.GetValueOverride (this);
							
							if (UndefinedValue.IsValueUndefined (value) == false)
							{
								yield return new LocalValueEntry (property, value);
							}
						}
					}
				}
			}
		}

		internal IEnumerable<DependencyProperty> LocalProperties
		{
			get
			{
				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return property;
				}

				//	Passe encore en revue les propriétés qui ne sont pas définies
				//	dans la variable 'properties' mais directement au moyen de
				//	callbacks GetValueOverrideCallback :

				DependencyObjectType type = this.ObjectType;
				System.Type sysType = this.GetType ();

				foreach (DependencyProperty property in type.GetProperties ())
				{
					if (this.properties.ContainsKey (property) == false)
					{
						DependencyPropertyMetadata metadata = property.GetMetadata (sysType);

						if (metadata.GetValueOverride != null)
						{
							yield return property;
						}
					}
				}
			}
		}

		internal IEnumerable<LocalValueEntry> SerializableLocalValueEntries
		{
			get
			{
				DependencyObjectType type = this.ObjectType;
				System.Type sysType = this.GetType ();

				DependencyPropertyMetadata metadata;
				
				foreach (DependencyProperty property in this.properties.Keys)
				{
					metadata = property.GetMetadata (sysType);

					if ((property.IsReadWrite && metadata.CanSerializeReadWrite) ||
						(property.IsReadOnly && metadata.CanSerializeReadOnly))
					{
						yield return new LocalValueEntry (property, this.properties[property]);
					}
				}
				
				//	Passe encore en revue les propriétés qui ne sont pas définies
				//	dans la variable 'properties' mais directement au moyen de
				//	callbacks GetValueOverrideCallback :

				foreach (DependencyProperty property in type.GetProperties ())
				{
					if (this.properties.ContainsKey (property) == false)
					{
						metadata = property.GetMetadata (sysType);

						if ((property.IsReadWrite && metadata.CanSerializeReadWrite) ||
							(property.IsReadOnly && metadata.CanSerializeReadOnly))
						{
							if (metadata.GetValueOverride != null)
							{
								object value = metadata.GetValueOverride (this);

								if (UndefinedValue.IsValueUndefined (value) == false)
								{
									yield return new LocalValueEntry (property, value);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets access to the cache storing the inherited properties.
		/// </summary>
		/// <value>The inherited property cache.</value>
		public IInheritedPropertyCache			InheritedPropertyCache
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Gets the value for the specified property. This internally calls the
		/// <c>GetValueBase</c> method to get the value. If the property metadata
		/// defines a <c>GetValueOverride</c>, it will be called instead of
		/// <c>GetValueBase</c>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The value of the property, or its default value if it is
		/// not defined for this object.</returns>
		public object GetValue(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);
			
			if (metadata.GetValueOverride != null)
			{
				return metadata.GetValueOverride (this);
			}
			else
			{
				return this.GetValueBase (property, metadata);
			}
		}

		/// <summary>
		/// Gets the base value for the specified property. This is called by
		/// <c>GetValue</c> or its override.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The value of the property, or its default value if it is
		/// not defined for this object.</returns>
		public object GetValueBase(DependencyProperty property)
		{
			object value = this.GetLocalValue (property);
			
			//	Si la valeur n'est pas définie localement, il faut déterminer la
			//	valeur réelle (par défaut, héritée, etc.)
			
			if (value == UndefinedValue.Instance)
			{
				DependencyPropertyMetadata metadata = property.GetMetadata (this);

				if ((metadata.InheritsValue) &&
					(this.inheritedPropertyCache.TryGetValue (this, property, out value)))
				{
					//	Re-use cached value.
				//-	System.Diagnostics.Debug.WriteLine ("Reuse cached value " + (value == null ? "<null>" : value.ToString ()));
				}
				else
				{
					value = metadata.CreateDefaultValue ();
				}
			}
			
			return value;
		}

		/// <summary>
		/// Sets the value for the specified property. This internally calls the
		/// <c>CoerceValue</c> method, followed by the <c>SetValueBase</c> method.
		/// If the property metadata defines a <c>SetValueOverride</c>, it will
		/// be called instead of <c>SetValueBase</c>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value of the property.</param>
		public void SetValue(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			value = this.CoerceValue (property, metadata, value);
			
			if (metadata.SetValueOverride != null)
			{
				metadata.SetValueOverride (this, value);
			}
			else
			{
				this.SetValueBase (property, value, metadata);
			}
		}

		/// <summary>
		/// Sets the base value for the specified property. This is called
		/// by <c>SetValue</c> or its override.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value of the property.</param>
		public void SetValueBase(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.SetValueBase (property, value, metadata);
		}
		
		/// <summary>
		/// Clears the value of the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		public void ClearValue(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.ClearValue (property, metadata);
		}

		/// <summary>
		/// Coerces the value so that it respects the constraints defined by the
		/// specified property's metadata.
		/// </summary>
		/// <param name="property">The property to use for coercion.</param>
		/// <param name="value">The value to coerce.</param>
		/// <returns>The coerced value.</returns>
		public object CoerceValue(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);
			return this.CoerceValue (property, metadata, value);
		}

		/// <summary>
		/// Gets the value directly from the internal dictionary, using
		/// the specified property as the access key. This shortcuts the default
		/// value management.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The value, or <c>UndefinedValue.Instance</c> if the value
		/// is not defined.</returns>
		public object GetLocalValue(DependencyProperty property)
		{
			object value;
			
			if (this.properties.TryGetValue (property, out value))
			{
				return value;
			}
			else
			{
				return UndefinedValue.Instance;
			}
		}

		/// <summary>
		/// Tries to get the value directly from the internal dictionary.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value could be found; otherwise <c>false</c>.</returns>
		public bool TryGetLocalValue(DependencyProperty property, out object value)
		{
			if (this.properties.TryGetValue (property, out value))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Sets the value into the internal dictionary. There is no checking and
		/// no coercion whatsoever.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		public void SetLocalValue(DependencyProperty property, object value)
		{
			this.properties[property] = value;
		}

		/// <summary>
		/// Clears the value from the internal dictionary.
		/// </summary>
		/// <param name="property">The property.</param>
		public void ClearLocalValue(DependencyProperty property)
		{
			this.properties.Remove (property);
		}

		/// <summary>
		/// Determines whether the value is contained within the internal dictionary.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the value is defined in the internal dictionary;
		/// otherwise, <c>false</c>.</returns>
		public bool ContainsLocalValue(DependencyProperty property)
		{
			return this.properties.ContainsKey (property);
		}

		/// <summary>
		/// Invalidates the property. This will fire a <c>DependencyPropertyChanged</c>
		/// event.
		/// </summary>
		/// <param name="property">The property which changes.</param>
		/// <param name="oldValue">The old value (before change).</param>
		/// <param name="newValue">The new value (after change).</param>
		public void InvalidateProperty(DependencyProperty property, object oldValue, object newValue)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.InvalidateProperty (property, oldValue, newValue, metadata);
		}

		#region Private Methods with DependencyPropertyMetadata
		
		private object GetValueBase(DependencyProperty property, DependencyPropertyMetadata metadata)
		{
			object value = this.GetLocalValue (property);

			//	Si la valeur n'est pas définie localement, il faut déterminer la
			//	valeur réelle (par défaut, héritée, etc.)

			if (value == UndefinedValue.Instance)
			{
				if ((metadata.InheritsValue) &&
					(this.inheritedPropertyCache.TryGetValue (this, property, out value)))
				{
					//	Re-use cached value.
					//-	System.Diagnostics.Debug.WriteLine ("Reuse cached value " + (value == null ? "<null>" : value.ToString ()));
				}
				else
				{
					value = metadata.CreateDefaultValue ();
				}
			}

			return value;
		}
		private void SetValueBase(DependencyProperty property, object value, DependencyPropertyMetadata metadata)
		{
			if (metadata.InheritsValue)
			{
				this.SetLocalValue (property, value);
				this.inheritedPropertyCache.SetValue (this, property, value);
				this.inheritedPropertyCache.NotifyChanges (this);
			}
			else
			{
				object old_value = this.GetValue (property);

				this.SetLocalValue (property, value);

				object new_value = this.GetValue (property);

				if (old_value == new_value)
				{
					//	C'est exactement la même valeur -- on ne signale donc rien ici.
				}
				else if ((old_value == null) || (!old_value.Equals (new_value)))
				{
					this.InvalidateProperty (property, old_value, new_value, metadata);
				}
			}
		}
		private void ClearValue(DependencyProperty property, DependencyPropertyMetadata metadata)
		{
			if (metadata.InheritsValue)
			{
				this.ClearLocalValue (property);
				this.InheritPropertyFromParent (property);
				this.inheritedPropertyCache.NotifyChanges (this);
			}
			else
			{
				object old_value = this.GetValue (property);

				this.ClearLocalValue (property);

				object new_value = this.GetValue (property);

				if (old_value == new_value)
				{
					//	C'est exactement la même valeur -- on ne signale donc rien ici.
				}
				else if ((old_value == null) || (!old_value.Equals (new_value)))
				{
					this.InvalidateProperty (property, old_value, new_value, metadata);
				}
			}
		}
		private void InvalidateProperty(DependencyProperty property, object old_value, object new_value, DependencyPropertyMetadata metadata)
		{
			if (metadata.NotifyPropertyInvalidated (this, old_value, new_value))
			{
				if (this.HasEventHandlerForProperty (property))
				{
					PropertyChangedEventHandler handler = this.propertyEvents[property];
					DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs (property, old_value, new_value);

					handler (this, args);
				}
			}
		}
		private object CoerceValue(DependencyProperty property, DependencyPropertyMetadata metadata, object value)
		{
			if (metadata.CoerceValue != null)
			{
				return metadata.CoerceValue (this, property, value);
			}
			else
			{
				return value;
			}
		}
		
		#endregion

		/// <summary>
		/// Adds the event handler for a <c>DependencyPropertyChanged</c> event
		/// on the specified proeprty.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="handler">The handler.</param>
		public void AddEventHandler(DependencyProperty property, PropertyChangedEventHandler handler)
		{
			if (this.propertyEvents == null)
			{
				if (this.propertyEvents == null)
				{
					this.propertyEvents = new PropertyChangedEventDictionary ();
				}
			}
			
			//	TODO: ajouter le support pour des "weak delegates" qui ne maintiennent qu'une WeakReference sur le handler

			if (this.propertyEvents.ContainsKey (property))
			{
				this.propertyEvents[property] = this.propertyEvents[property] + handler;
			}
			else
			{
				this.propertyEvents[property] = handler;
			}
		}

		/// <summary>
		/// Removes the event handler for a <c>DependencyPropertyChanged</c> event
		/// on the specified proeprty.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="handler">The handler.</param>
		public void RemoveEventHandler(DependencyProperty property, PropertyChangedEventHandler handler)
		{
			if ((this.propertyEvents != null) &&
				(this.propertyEvents.ContainsKey (property)))
			{
				this.propertyEvents[property] = this.propertyEvents[property] - handler;
			}
		}

		/// <summary>
		/// Determines whether the specified property has at least one event handler
		/// registered.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property has an event handler;
		/// otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Gets the binding for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The binding object for the property, or <c>null</c> if
		/// there is no binding.</returns>
		public Binding GetBinding(DependencyProperty property)
		{
			BindingExpression bindingExpression = this.GetBindingExpression (property);
			
			if (bindingExpression != null)
			{
				return bindingExpression.ParentBinding;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the binding expression for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The binding expression for the property, or <c>null</c> if
		/// there is no binding.</returns>
		public BindingExpression GetBindingExpression(DependencyProperty property)
		{
			BindingExpression bindingExpression;

			if ((this.bindings != null) &&
				(this.bindings.TryGetValue (property, out bindingExpression)))
			{
				return bindingExpression;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Sets the binding for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="binding">The binding.</param>
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
				this.bindings = new BindingExpressionDictionary ();
			}
			else
			{
				this.ClearBinding (property);
			}
			
			this.bindings[property] = BindingExpression.BindToTarget (this, property, binding);

			this.OnBindingChanged (property);
		}

		/// <summary>
		/// Clears all bindings for all properties.
		/// </summary>
		public void ClearAllBindings()
		{
			DependencyProperty[] properties = Copier.CopyArray (this.bindings.Keys);

			for (int i = 0; i < properties.Length; i++)
			{
				this.ClearBinding (properties[i]);
			}

			if (this.bindings.Count == 0)
			{
				this.bindings = null;
			}
		}

		/// <summary>
		/// Clears the binding for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		public void ClearBinding(DependencyProperty property)
		{
			BindingExpression bindingExpression;
			
			if ((this.bindings != null) &&
				(this.bindings.TryGetValue (property, out bindingExpression)))
			{
				bindingExpression.Dispose ();
				this.bindings.Remove (property);
				this.OnBindingChanged (property);
			}
		}

		/// <summary>
		/// Determines whether the specified property is bound.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property is bound; otherwise,
		/// <c>false</c>.</returns>
		public bool IsBound(DependencyProperty property)
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

		/// <summary>
		/// Enumerates all bindings for all properties.
		/// </summary>
		/// <returns>The enumeration of all bindings.</returns>
		public IEnumerable<KeyValuePair<DependencyProperty, Binding>> GetAllBindings()
		{
			if (this.bindings != null)
			{
				foreach (KeyValuePair<DependencyProperty, BindingExpression> entry in this.bindings)
				{
					yield return new KeyValuePair<DependencyProperty, Binding> (entry.Key, entry.Value.ParentBinding);
				}
			}
		}


		/// <summary>
		/// Copies the attached properties from the source to the destination.
		/// This is a shallow copy (it copies just the references, not the
		/// objects themselves for reference types).
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="destination">The destination object.</param>
		public static void CopyAttachedProperties(DependencyObject source, DependencyObject destination)
		{
			foreach (DependencyProperty property in source.properties.Keys)
			{
				if (property.IsAttached)
				{
					destination.SetValue (property, source.GetValue (property));
				}
			}
		}
		
		protected void AddUserEventHandler(string name, System.Delegate handler)
		{
			if (this.userEvents == null)
			{
				if (this.userEvents == null)
				{
					this.userEvents = new UserEventDictionary ();
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
			System.Delegate value;
			
			if ((this.userEvents != null) &&
				(this.userEvents.TryGetValue (name, out value)))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		protected virtual void OnBindingChanged(DependencyProperty property)
		{
			BindingChangedEventHandler handler = (BindingChangedEventHandler) this.GetUserEventHandler (DependencyObject.BindingChangedString);

			if (handler != null)
			{
				handler (this, new BindingChangedEventArgs (property));
			}
		}

		private void InheritPropertyFromParent(DependencyProperty property)
		{
			//	Update the cached value for this inherited property, based
			//	on what is currently defined by the parent (if any).

			DependencyObject parent = DependencyObjectTree.GetParent (this);

			if (parent == null)
			{
				//	No parent means that we can clear the cache and forget
				//	about it; clearing the cache will return the default
				//	value from now on.

				this.inheritedPropertyCache.ClearValue (this, property);
			}
			else
			{
				//	There is a parent; ask it to provide its value and use
				//	that as the locally cached value.

				this.inheritedPropertyCache.SetValue (this, property, parent.GetValue (property));
			}
		}

		internal static void Register(DependencyProperty property, System.Type ownerType)
		{
			System.Diagnostics.Debug.Assert (property != null);
			System.Diagnostics.Debug.Assert (ownerType != null);

			lock (DependencyObject.declarations)
			{
				DependencyObjectType type = DependencyObjectType.FromSystemType (ownerType);
				TypeDeclaration typeDeclaration;
				
				string name = property.Name;

				//	Verify that neither the owner type, nor any of its ancestors,
				//	already defines the specified property :

				if (property.IsAttached == false)
				{
					System.Type t = ownerType;
					
					while (t != typeof (object))
					{
						if (DependencyObject.declarations.TryGetValue (t, out typeDeclaration))
						{
							if ((typeDeclaration.ContainsKey (name)) &&
								(typeDeclaration[name].IsAttached == false))
							{
								throw new System.ArgumentException (string.Format ("DependencyProperty named '{0}' already exists for type {1} (defined by {2})", name, ownerType, t));
							}
						}

						t = t.BaseType;
					}
				}

				if (DependencyObject.declarations.TryGetValue (ownerType, out typeDeclaration))
				{
					typeDeclaration[name] = property;
				}
				else
				{
					typeDeclaration = new TypeDeclaration ();
					typeDeclaration[name] = property;
					DependencyObject.declarations[ownerType] = typeDeclaration;
				}

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

		#region IInheritedPropertyCache Members

		void IInheritedPropertyCache.ClearAllValues(DependencyObject node)
		{
			this.inheritedPropertyCache.ClearAllValues (node);
		}
		void IInheritedPropertyCache.ClearValues(DependencyObject node, IEnumerable<DependencyProperty> properties)
		{
			this.inheritedPropertyCache.ClearValues (node, properties);
		}
		void IInheritedPropertyCache.SetValues(DependencyObject node, IEnumerable<LocalValueEntry> propertyValues)
		{
			this.inheritedPropertyCache.SetValues (node, propertyValues);
		}
		bool IInheritedPropertyCache.IsDefined(DependencyObject node, DependencyProperty property)
		{
			return this.inheritedPropertyCache.IsDefined (node, property);
		}
		bool IInheritedPropertyCache.TryGetValue(DependencyObject node, DependencyProperty property, out object value)
		{
			return this.inheritedPropertyCache.TryGetValue (node, property, out value);
		}
		void IInheritedPropertyCache.NotifyChanges(DependencyObject node)
		{
			this.inheritedPropertyCache.NotifyChanges (node);
		}
		IEnumerable<LocalValueEntry> IInheritedPropertyCache.GetValues(DependencyObject node)
		{
			return this.inheritedPropertyCache.GetValues (node);
		}
		void IInheritedPropertyCache.InheritValuesFromParent(DependencyObject node, DependencyObject parent)
		{
			this.inheritedPropertyCache.InheritValuesFromParent (node, parent);
		}

		#endregion

		#region IStructuredData Members

		void IStructuredData.AttachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (name);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot map to DependencyProperty", name));
			}

			this.AddEventHandler (property, handler);
		}

		void IStructuredData.DetachListener(string name, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (name);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot map to DependencyProperty", name));
			}

			this.RemoveEventHandler (property, handler);
		}

		string[] IStructuredData.GetValueNames()
		{
			List<string> names = new List<string> ();
			
			foreach (DependencyProperty property in this.LocalProperties)
			{
				names.Add (property.Name);
			}

			return names.ToArray ();
		}

		object IStructuredData.GetValue(string name)
		{
			DependencyProperty property = this.ObjectType.GetProperty (name);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot be mapped to a DependencyProperty", name));
			}

			return this.GetValue (property);
		}

		void IStructuredData.SetValue(string name, object value)
		{
			DependencyProperty property = this.ObjectType.GetProperty (name);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot be mapped to a DependencyProperty", name));
			}

			this.SetValue (property, value);
		}

		#endregion

		#region Private TypeDeclaration Class
		private class TypeDeclaration : Dictionary<string, DependencyProperty>
		{
		}
		#endregion

		#region Private PropertyChangedEventDictionary Class
		private class PropertyChangedEventDictionary : Dictionary<DependencyProperty, PropertyChangedEventHandler>
		{
		}
		#endregion

		#region Private UserEventDictionary Class
		private class UserEventDictionary : Dictionary<string, System.Delegate>
		{
		}
		#endregion

		#region Private BindingExpressionDictionary Class
		private class BindingExpressionDictionary : Dictionary<DependencyProperty, BindingExpression>
		{
		}
		#endregion
		
		public event BindingChangedEventHandler				BindingChanged
		{
			add
			{
				this.AddUserEventHandler (DependencyObject.BindingChangedString, value);
			}
			remove
			{
				this.RemoveUserEventHandler (DependencyObject.BindingChangedString, value);
			}
		}

		private const string								BindingChangedString = "BindingChanged";

		Dictionary<DependencyProperty, object>				properties = new Dictionary<DependencyProperty, object> ();
		BindingExpressionDictionary							bindings;
		PropertyChangedEventDictionary						propertyEvents;
		UserEventDictionary									userEvents;
		InheritedPropertyCache								inheritedPropertyCache;
		
		DependencyObjectType								cachedType;

		static Dictionary<System.Type, TypeDeclaration>		declarations = new Dictionary<System.Type, TypeDeclaration> ();
		static int											registeredPropertyCount;
	}
}
