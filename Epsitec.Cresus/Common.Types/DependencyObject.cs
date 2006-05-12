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
	public abstract class DependencyObject : System.IDisposable, IInheritedPropertyCache, IStructuredData, IStructuredTree
	{
		protected DependencyObject()
		{
			this.cachedType = DependencyObjectType.SetupFromSystemType (this.GetType ());
		}
		
		public DependencyObjectType				ObjectType
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
				foreach (DependencyProperty property in this.properties.Keys)
				{
					yield return new LocalValueEntry (property, this.properties[property]);
				}
				
				//	Passe encore en revue les propriétés qui ne sont pas définies
				//	dans la variable 'properties' mais directement au moyen de
				//	callbacks GetValueOverrideCallback :

				//	TODO: gestion des GetValueOverrideCallback

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
		
		public IInheritedPropertyCache			InheritedPropertyCache
		{
			get
			{
				return this;
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
				return this.GetValueBase (property, metadata);
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
		public void SetValueBase(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.SetValueBase (property, value, metadata);
		}
		public void ClearValueBase(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.ClearValueBase (property, metadata);
		}

		public object CoerceValue(DependencyProperty property, object value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);
			return this.CoerceValue (property, metadata, value);
		}

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
		public void SetLocalValue(DependencyProperty property, object value)
		{
			this.properties[property] = value;
		}
		public void ClearLocalValue(DependencyProperty property)
		{
			this.properties.Remove (property);
		}
		public bool ContainsLocalValue(DependencyProperty property)
		{
			return this.properties.ContainsKey (property);
		}

		public void InvalidateProperty(DependencyProperty property, object old_value, object new_value)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			this.InvalidateProperty (property, old_value, new_value, metadata);
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
		private void ClearValueBase(DependencyProperty property, DependencyPropertyMetadata metadata)
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
			BindingExpression bindingExpression;
			
			if ((this.bindings != null) &&
				(this.bindings.TryGetValue (property, out bindingExpression)))
			{
				return bindingExpression.ParentBinding;
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
				this.bindings = new BindingExpressionDictionary ();
			}
			else
			{
				this.ClearBinding (property);
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

			if (this.bindings.Count == 0)
			{
				this.bindings = null;
			}
		}
		public void ClearBinding(DependencyProperty property)
		{
			BindingExpression bindingExpression;
			if ((this.bindings != null) &&
				(this.bindings.TryGetValue (property, out bindingExpression)))
			{
				bindingExpression.Dispose ();
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

		void IStructuredData.AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (path);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot map to DependencyProperty", path));
			}

			this.AddEventHandler (property, handler);
		}

		void IStructuredData.DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			DependencyProperty property = this.ObjectType.GetProperty (path);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot map to DependencyProperty", path));
			}

			this.RemoveEventHandler (property, handler);
		}

		object IStructuredData.GetValue(string path)
		{
			DependencyProperty property = this.ObjectType.GetProperty (path);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot map to DependencyProperty", path));
			}

			return this.GetValue (property);
		}

		object IStructuredData.GetValueTypeObject(string path)
		{
			DependencyProperty property = this.ObjectType.GetProperty (path);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot map to DependencyProperty", path));
			}

			return property;
		}

		void IStructuredData.SetValue(string path, object value)
		{
			DependencyProperty property = this.ObjectType.GetProperty (path);

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot map to DependencyProperty", path));
			}

			this.SetValue (property, value);
		}

		bool IStructuredData.HasImmutableRoots
		{
			get
			{
				return false;
			}
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

		Dictionary<DependencyProperty, object>				properties = new Dictionary<DependencyProperty, object> ();
		BindingExpressionDictionary							bindings;
		PropertyChangedEventDictionary						propertyEvents;
		UserEventDictionary									userEvents;
		InheritedPropertyCache								inheritedPropertyCache;
		
		DependencyObjectType								cachedType;

		static Dictionary<System.Type, TypeDeclaration>		declarations = new Dictionary<System.Type, TypeDeclaration> ();
		static int											registeredPropertyCount;

		#region IStructuredTree Members

		string[] IStructuredTree.GetFieldNames()
		{
			ReadOnlyArray<DependencyProperty> properties = this.ObjectType.GetProperties ();

			string[] names = new string[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				names[i] = properties[i].Name;
			}
			
			return names;
		}

		string[] IStructuredTree.GetFieldPaths(string path)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion
	}
}
