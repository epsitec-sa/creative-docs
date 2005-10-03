//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Object repr�sente un objet dont les propri�t�s sont
	/// stock�es dans un dictionnaire interne plut�t que dans des variables, ce
	/// qui permet une plus grande souplesse (valeurs par d�faut, introspection,
	/// s�rialisation, styles, g�n�ration automatique d'�v�nements, etc.)
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
		
		
		public object GetValue(Property property)
		{
			PropertyMetadata metadata = property.DefaultMetadata;
			
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
			
			//	Si la valeur n'est pas d�finie localement, il faut d�terminer la
			//	valeur r�elle (par d�faut, h�rit�e, etc.)
			
			if (value == UndefinedValue.Instance)
			{
				PropertyMetadata metadata = property.DefaultMetadata;
				
				//	TODO: faire mieux...
				
				value = metadata.DefaultValue;
			}
			
			return value;
		}
		
		
		public void SetValue(Property property, object value)
		{
			PropertyMetadata metadata = property.DefaultMetadata;
			
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
				//	C'est exactement la m�me valeur -- on ne signale donc rien ici.
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
		
		
		public void AddEvent(Property property, PropertyChangedEventHandler handler)
		{
			if (this.events == null)
			{
				this.events = new System.Collections.Hashtable ();
			}
			
			this.events[property] = (PropertyChangedEventHandler) this.events[property] + handler;
		}
		
		public void RemoveEvent(Property property, PropertyChangedEventHandler handler)
		{
			if ((this.events != null) &&
				(this.events.Contains (property)))
			{
				this.events[property] = (PropertyChangedEventHandler) this.events[property] - handler;
			}
		}
		
		
		private void InvalidateProperty(Property property, object old_value, object new_value)
		{
			PropertyMetadata metadata = property.DefaultMetadata;
				
			if (metadata.PropertyInvalidated != null)
			{
				metadata.PropertyInvalidated (this, old_value, new_value);
			}
			
			if (this.events != null)
			{
				PropertyChangedEventHandler handler = (PropertyChangedEventHandler) this.events[property];
				
				if (handler != null)
				{
					handler (this, new PropertyChangedEventArgs (property, old_value, new_value));
				}
			}
		}
		
		
		internal static void Register(Property dp)
		{
			lock (Object.declarations)
			{
				System.Collections.Hashtable type_declaration = Object.declarations[dp.OwnerType] as System.Collections.Hashtable;
				
				if (type_declaration == null)
				{
					type_declaration = new System.Collections.Hashtable ();
					type_declaration[dp.Name] = dp;
					Object.declarations[dp.OwnerType] = type_declaration;
				}
				else if (type_declaration.Contains (dp.Name))
				{
					throw new System.ArgumentException (string.Concat ("Property with name ", dp.Name, " already exists for type ", dp.OwnerType));
				}
				else
				{
					type_declaration[dp.Name] = dp;
				}
			}
		}
		
		
		private System.Collections.Hashtable	properties = new System.Collections.Hashtable ();
		private System.Collections.Hashtable	events;
		private ObjectType						cached_type;
		
		static System.Collections.Hashtable		declarations = new System.Collections.Hashtable ();
	}
}
