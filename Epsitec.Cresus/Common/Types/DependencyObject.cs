//	Copyright © 2005-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

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
	public abstract class DependencyObject : System.IDisposable, IInheritedPropertyCache, IStructuredData, IStructuredTypeProvider
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
		/// Gets the defined entries, i.e. the properties and values defined by
		/// this object, either locally or through a <c>GetValueOverride</c>
		/// callback.
		/// </summary>
		/// <value>The defined entries.</value>
		public IEnumerable<PropertyValuePair>	DefinedEntries
		{
			get
			{
				DependencyProperty[] keys = new DependencyProperty[this.properties.Count];
				this.properties.Keys.CopyTo (keys, 0);
				
				foreach (DependencyProperty property in keys)
				{
					yield return new PropertyValuePair (property, this.properties[property]);
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
							yield return new PropertyValuePair (property, metadata.GetValueOverride (this));
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the defined properties, i.e. the properties defined by this
		/// object, either locally or through a <c>GetValueOverride</c> callback.
		/// </summary>
		/// <value>The defined properties.</value>
		public IEnumerable<DependencyProperty>	DefinedProperties
		{
			get
			{
				DependencyObjectType type = this.ObjectType;
				System.Type sysType = this.GetType ();

				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return property;
				}

				//	Passe encore en revue les propriétés qui ne sont pas définies
				//	dans la variable 'properties' mais directement au moyen de
				//	callbacks GetValueOverrideCallback :

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

		public IEnumerable<PropertyValuePair>	LocalEntries
		{
			get
			{
				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return new PropertyValuePair (property, this.properties[property]);
				}
			}
		}

		public IEnumerable<DependencyProperty>	LocalProperties
		{
			get
			{
				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return property;
				}
			}
		}

		public IEnumerable<DependencyProperty>	AttachedProperties
		{
			get
			{
				foreach (DependencyProperty property in this.properties.Keys)
				{
					if (property.IsAttached)
					{
						yield return property;
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

		public T GetValue<T>(DependencyProperty property)
		{
			return (T) this.GetValue (property);
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
			
			if (value == UndefinedValue.Value)
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
		/// Clears all values.
		/// </summary>
		public void ClearAllValues()
		{
			try
			{
				this.BeginMultiplePropertyChange ();

				List<DependencyProperty> properties = new List<DependencyProperty> (this.DefinedProperties);
				
				foreach (DependencyProperty property in properties)
				{
					this.ClearValue (property);
				}
			}
			finally
			{
				this.EndMultiplePropertyChange ();
			}
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
		/// <returns>The value, or <c>UndefinedValue.Value</c> if the value
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
				return UndefinedValue.Value;
			}
		}

		/// <summary>
		/// Tries to get the value directly from the internal dictionary.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value could be found; otherwise, <c>false</c>.</returns>
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

		public bool TryGetLocalValue<T>(DependencyProperty property, out T value)
		{
			object objectValue;

			if (this.properties.TryGetValue (property, out objectValue))
			{
				value = (T) objectValue;
				return true;
			}
			else
			{
				value = default (T);
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
			System.Diagnostics.Debug.Assert (!UndefinedValue.IsUndefinedValue (value));
			System.Diagnostics.Debug.Assert (property.IsValidType (value));

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

		public bool ContainsValue(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			return this.ContainsValue (property, metadata);
		}

		private bool ContainsValue(DependencyProperty property, DependencyPropertyMetadata metadata)
		{
			if ((metadata.GetValueOverride != null) ||
				(this.ContainsLocalValue (property)))
			{
				return true;
			}
			else
			{
				return false;
			}
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

			if (value == UndefinedValue.Value)
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
			System.Diagnostics.Debug.Assert (! UndefinedValue.IsUndefinedValue (value));
			
			if (metadata.InheritsValue)
			{
				this.SetLocalValue (property, value);
				this.inheritedPropertyCache.SetValue (this, property, value);
				this.inheritedPropertyCache.NotifyChanges (this);
			}
			else
			{
				object oldValue = this.GetValue (property);

				this.SetLocalValue (property, value);

				object newValue = this.GetValue (property);

				if (DependencyObject.EqualObjectValues (oldValue, newValue))
				{
					//	C'est exactement la même valeur -- on ne signale donc rien ici.
				}
				else
				{
					this.InvalidateProperty (property, oldValue, newValue, metadata);
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
				object oldValue = this.GetValue (property);

				this.ClearLocalValue (property);

				object newValue = this.GetValue (property);

				if (DependencyObject.EqualObjectValues (oldValue, newValue))
				{
					//	C'est exactement la même valeur -- on ne signale donc rien ici.
				}
				else
				{
					this.InvalidateProperty (property, oldValue, newValue, metadata);
				}
			}
		}
		private void InvalidateProperty(DependencyProperty property, object oldValue, object newValue, DependencyPropertyMetadata metadata)
		{
			if (metadata.NotifyPropertyInvalidated (this, oldValue, newValue))
			{
				if (this.HasEventHandlerForProperty (property))
				{
					PropertyChangedEventHandler handler = this.propertyEvents[property];
					DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs (property, oldValue, newValue);

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
		/// on the specified property.
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

		public void ClearEventHandlers(DependencyProperty property)
		{
			if ((this.propertyEvents != null) &&
				(this.propertyEvents.ContainsKey (property)))
			{
				this.propertyEvents[property] = null;
			}
		}
		
		public void ClearUserEventHandlers(string name)
		{
			if ((this.userEvents != null) &&
				(this.userEvents.ContainsKey (name)))
			{
				this.userEvents[name] = null;
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
		public void SetBinding(DependencyProperty property, AbstractBinding binding)
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
			if (this.bindings == null)
			{
				return;
			}
			
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
		/// Compares two dependency objects based on their values.
		/// </summary>
		/// <param name="a">Dependency object.</param>
		/// <param name="b">Dependency object.</param>
		/// <returns><c>true</c> if both objects contain the same values; otherwise, <c>false</c>.</returns>
		public static bool EqualValues(DependencyObject a, DependencyObject b)
		{
			if (a == b)
			{
				return true;
			}

			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			List<PropertyValuePair> aValues = new List<PropertyValuePair> (a.DefinedEntries);
			List<PropertyValuePair> bValues = new List<PropertyValuePair> (b.DefinedEntries);

			if (aValues.Count != bValues.Count)
			{
				return false;
			}

			foreach (PropertyValuePair entry in aValues)
			{
				object va = entry.Value;
				object vb = b.GetValue (entry.Property);

				if (DependencyObject.EqualObjectValues (va, vb))
				{
					continue;
				}

				if ((va == null) ||
					(vb == null))
				{
					return false;
				}

				if ((va is System.Collections.ICollection) &&
					(vb is System.Collections.ICollection))
				{
					System.Collections.ICollection ca = va as System.Collections.ICollection;
					System.Collections.ICollection cb = vb as System.Collections.ICollection;

					if ((ca.Count == 0) &&
						(cb.Count == 0))
					{
						continue;
					}
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Compares two objects for equality.
		/// </summary>
		/// <param name="valueA">Object value.</param>
		/// <param name="valueB">Object value.</param>
		/// <returns><c>true</c> if both objects are equal; otherwise, <c>false</c>.</returns>
		public static bool EqualObjectValues(object valueA, object valueB)
		{
			if (valueA == valueB)
			{
				return true;
			}
			else if ((valueA == null) || (valueB == null))
			{
				return false;
			}
			else if (valueA.Equals (valueB))
			{
				return true;
			}
			else
			{
				return false;
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
			try
			{
				destination.BeginMultiplePropertyChange ();

				foreach (DependencyProperty property in source.AttachedProperties)
				{
					destination.SetValue (property, source.GetValue (property));
				}
			}
			finally
			{
				destination.EndMultiplePropertyChange ();
			}
		}

		/// <summary>
		/// Copies the value of the property from the source to the destination.
		/// If the source does not define this property, it will be cleared in
		/// the destination.
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="destination">The destination object.</param>
		/// <param name="property">The property which must be copied.</param>
		public static void CopyProperty(DependencyObject source, DependencyObject destination, DependencyProperty property)
		{
			if (! DependencyObject.CopyDefinedProperty (source, destination, property, false))
			{
				destination.ClearLocalValue (property);
			}
		}

		/// <summary>
		/// Copies the value of the property from the source to the destination.
		/// If the source does not define this property, nothing happens.
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="destination">The destination object.</param>
		/// <param name="property">The property which must be copied.</param>
		public static void CopyDefinedProperty(DependencyObject source, DependencyObject destination, DependencyProperty property)
		{
			DependencyObject.CopyDefinedProperty (source, destination, property, true);
		}

		/// <summary>
		/// Copies the values of the defined properties from the source to the
		/// destination.
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="destination">The destination object.</param>
		public static void CopyDefinedProperties(DependencyObject source, DependencyObject destination)
		{
			try
			{
				destination.BeginMultiplePropertyChange ();
				
				foreach (DependencyProperty property in source.DefinedProperties)
				{
					DependencyObject.CopyDefinedProperty (source, destination, property, true);
				}
			}
			finally
			{
				destination.EndMultiplePropertyChange ();
			}
		}
		
		private static bool CopyDefinedProperty(DependencyObject source, DependencyObject destination, DependencyProperty property, bool ignoreEmptyCollection)
		{
			if (source.ContainsValue (property))
			{
				object value = source.GetValue (property);

				System.Collections.ICollection collection = value as System.Collections.ICollection;

				if (collection != null)
				{
					if (ignoreEmptyCollection)
					{
						if (collection.Count == 0)
						{
							return false;
						}
					}

					if (DependencyObject.CopyCollection<DependencyObject> (source, destination, property))
					{
						//	Copied a DependencyObject collection.
					}
					else if (DependencyObject.CopyCollection<string> (source, destination, property))
					{
						//	Copied a string collection.
					}
					else if (DependencyObject.CopyValueTypeOrCloneableCollection (source, destination, property))
					{
						//	Copied a ValueType collection.
					}
					else
					{
						throw new System.InvalidOperationException ("Cannot copy unsupported collection");
					}
				}
				else
				{
					destination.SetValue (property, source.GetValue (property));
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool CopyCollection<T>(DependencyObject src, DependencyObject dst, DependencyProperty property)
		{
			if (TypeRosetta.DoesTypeImplementInterface (property.PropertyType, typeof (ICollection<T>)))
			{
				ICollection<T> srcList = src.GetValue (property) as ICollection<T>;

				if (srcList.Count > 0)
				{
					ICollection<T> dstList = dst.GetValue (property) as ICollection<T>;

					if (dstList == null)
					{
						throw new System.InvalidOperationException ("Cannot copy to null destination collection");
					}

					dstList.Clear ();

					foreach (T item in srcList)
					{
						dstList.Add (item);
					}
				}
				
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool CopyValueTypeOrCloneableCollection(DependencyObject src, DependencyObject dst, DependencyProperty property)
		{
			System.Type interfaceType;
			
			if ((TypeRosetta.DoesTypeImplementInterface (property.PropertyType, typeof (System.Collections.IList))) &&
				(TypeRosetta.DoesTypeImplementGenericInterface (property.PropertyType, typeof (ICollection<>), out interfaceType)))
			{
				//	We must copy a ICollection<T> by using the IList implementation. This is
				//	possible if T is a ValueType or if T implements ICloneable.
				
				System.Type[] genericArguments = interfaceType.GetGenericArguments ();

				if (genericArguments.Length != 1)
				{
					return false;
				}
				
				System.Collections.IList srcList = src.GetValue (property) as System.Collections.IList;

				if (srcList.Count > 0)
				{
					System.Collections.IList dstList = dst.GetValue (property) as System.Collections.IList;
					IReadOnlyLock readOnlyLock = null;

					if (dstList == null)
					{
						throw new System.InvalidOperationException ("Cannot copy to null destination collection");
					}
					
					//	If the destination collection is locked down, we must try to unlock
					//	it in order to be able to overwrite it with the source values :
					
					if (dstList.IsReadOnly)
					{
						readOnlyLock = dstList as IReadOnlyLock;

						if (readOnlyLock == null)
						{
							throw new System.InvalidOperationException ("Cannot copy to read only destination collection");
						}

						readOnlyLock.Unlock ();

						System.Diagnostics.Debug.Assert (dstList.IsReadOnly == false);
					}

					if (genericArguments[0].IsValueType)
					{
						dstList.Clear ();

						foreach (object item in srcList)
						{
							dstList.Add (item);
						}
					}
					else if (TypeRosetta.DoesTypeImplementInterface (genericArguments[0], typeof (System.ICloneable)))
					{
						dstList.Clear ();

						foreach (object item in srcList)
						{
							System.ICloneable cloneable = item as System.ICloneable;
							dstList.Add (cloneable.Clone ());
						}
					}
					else
					{
						return false;
					}

					//	In case we unlocked the collection, relock it.

					if (readOnlyLock != null)
					{
						readOnlyLock.Lock ();
					}
				}

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

		protected void RaiseUserEvent(string name)
		{
			this.GetUserEventHandler (name).Raise (this);
		}

		protected void RaiseUserEvent<TEventArgs>(string name, TEventArgs e)
			where TEventArgs : System.EventArgs
		{
			this.GetUserEventHandler<TEventArgs> (name).Raise (this, e);
		}

		protected Support.EventHandler GetUserEventHandler(string name)
		{
			System.Delegate value;
			
			if ((this.userEvents != null) &&
				(this.userEvents.TryGetValue (name, out value)))
			{
				return value as Support.EventHandler;
			}
			else
			{
				return null;
			}
		}

		protected Support.EventHandler<TEventArgs> GetUserEventHandler<TEventArgs>(string name)
			where TEventArgs : System.EventArgs
		{
			System.Delegate value;

			if ((this.userEvents != null) &&
				(this.userEvents.TryGetValue (name, out value)))
			{
				return value as Support.EventHandler<TEventArgs>;
			}
			else
			{
				return null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			this.propertyEvents = null;
			this.userEvents = null;
		}

		protected virtual void BeginMultiplePropertyChange()
		{
		}

		protected virtual void EndMultiplePropertyChange()
		{
		}

		protected virtual void OnBindingChanged(DependencyProperty property)
		{
			BindingChangedEventHandler handler = this.GetUserEventHandler<BindingChangedEventArgs> (DependencyObject.BindingChangedString);

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

		public IEnumerable<PropertyValuePair> GetSerializableDefinedValues()
		{
			System.Type sysType = this.GetType ();
			Serialization.BlackList blackList = Serialization.BlackList.GetSerializationBlackList (this);

			foreach (PropertyValuePair pair in this.DefinedEntries)
			{
				DependencyProperty property = pair.Property;

				//	Don't return properties which have been black-listed for
				//	serialization.
				
				if ((blackList != null) &&
					(blackList.Contains (property)))
				{
					continue;
				}
				
				DependencyPropertyMetadata metadata = property.GetMetadata (sysType);

				if ((property.IsReadWrite && metadata.CanSerializeReadWrite) ||
					(property.IsReadOnly && metadata.CanSerializeReadOnly))
				{
					yield return pair;
				}
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
		void IInheritedPropertyCache.SetValues(DependencyObject node, IEnumerable<PropertyValuePair> propertyValues)
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
		IEnumerable<PropertyValuePair> IInheritedPropertyCache.GetValues(DependencyObject node)
		{
			return this.inheritedPropertyCache.GetValues (node);
		}
		void IInheritedPropertyCache.InheritValuesFromParent(DependencyObject node, DependencyObject parent)
		{
			this.inheritedPropertyCache.InheritValuesFromParent (node, parent);
		}

		#endregion

		#region IStructuredData Members

		void IStructuredData.AttachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (id);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Identifier '{0}' cannot map to DependencyProperty", id));
			}

			this.AddEventHandler (property, handler);
		}

		void IStructuredData.DetachListener(string id, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (id);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Identifier '{0}' cannot map to DependencyProperty", id));
			}

			this.RemoveEventHandler (property, handler);
		}

		IEnumerable<string> IStructuredData.GetValueIds()
		{
			List<string> ids = new List<string> ();
			
			foreach (DependencyProperty property in this.DefinedProperties)
			{
				ids.Add (property.Name);
			}

			return ids;
		}

		void IStructuredData.SetValue(string id, object value)
		{
			IValueStore store = this;
			store.SetValue (id, value, ValueStoreSetMode.Default);
		}

		object IValueStore.GetValue(string id)
		{
			DependencyProperty property = this.ObjectType.GetProperty (id);

			if (property == null)
			{
				return UnknownValue.Value;
			}
			else
			{
				return this.GetValue (property);
			}
		}

		void IValueStore.SetValue(string id, object value, ValueStoreSetMode mode)
		{
			DependencyProperty property = this.ObjectType.GetProperty (id);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Identifier '{0}' cannot be mapped to a DependencyProperty", id));
			}

			this.SetValue (property, value);
		}

		#endregion
		
		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.ObjectType;
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
