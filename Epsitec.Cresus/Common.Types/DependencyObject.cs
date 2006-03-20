//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;

	/// <summary>
	/// La classe DependencyObject repr�sente un objet dont les propri�t�s sont
	/// stock�es dans un dictionnaire interne plut�t que dans des variables, ce
	/// qui permet une plus grande souplesse (valeurs par d�faut, introspection,
	/// s�rialisation, styles, g�n�ration automatique d'�v�nements, etc.)
	/// </summary>
	public abstract class DependencyObject : System.IDisposable
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
				
				//	Passe encore en revue les propri�t�s qui ne sont pas d�finies
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
			
			//	Si la valeur n'est pas d�finie localement, il faut d�terminer la
			//	valeur r�elle (par d�faut, h�rit�e, etc.)
			
			if (value == UndefinedValue.Instance)
			{
				DependencyPropertyMetadata metadata = property.GetMetadata (this);

				if (metadata.InheritsValue)
				{
					value = metadata.FindInheritedValue (this, property);
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
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			if (metadata.InheritsValue)
			{
				DependencyObjectTreeSnapshot snapshot = DependencyObjectTree.CreatePropertyTreeSnapshot (this, property);
				
				this.SetLocalValue (property, value);
				
				snapshot.InvalidateDifferentProperties ();
			}
			else
			{
				object old_value = this.GetValue (property);

				this.SetLocalValue (property, value);

				object new_value = this.GetValue (property);

				if (old_value == new_value)
				{
					//	C'est exactement la m�me valeur -- on ne signale donc rien ici.
				}
				else if ((old_value == null) || (!old_value.Equals (new_value)))
				{
					this.InvalidateProperty (property, old_value, new_value);
				}
			}
		}
		public void ClearValueBase(DependencyProperty property)
		{
			DependencyPropertyMetadata metadata = property.GetMetadata (this);

			if (metadata.InheritsValue)
			{
				DependencyObjectTreeSnapshot snapshot = DependencyObjectTree.CreatePropertyTreeSnapshot (this, property);

				this.ClearLocalValue (property);

				snapshot.InvalidateDifferentProperties ();
			}
			else
			{
				object old_value = this.GetValue (property);

				this.ClearLocalValue (property);

				object new_value = this.GetValue (property);

				if (old_value == new_value)
				{
					//	C'est exactement la m�me valeur -- on ne signale donc rien ici.
				}
				else if ((old_value == null) || (!old_value.Equals (new_value)))
				{
					this.InvalidateProperty (property, old_value, new_value);
				}
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
		
		DependencyObjectType								cachedType;

		static Dictionary<System.Type, TypeDeclaration>		declarations = new Dictionary<System.Type, TypeDeclaration> ();
		static int											registeredPropertyCount;
	}
}
