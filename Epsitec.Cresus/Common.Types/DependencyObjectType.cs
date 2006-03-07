//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DependencyObjectType représente une version de plus haut niveau de
	/// System.Type.
	/// </summary>
	public sealed class DependencyObjectType
	{
		private DependencyObjectType(System.Type system_type, DependencyObjectType base_type)
		{
			this.systemType = system_type;
			this.baseType   = base_type;

			DependencyObjectType type = this.baseType;
			
			//	Register this type as a derived type on all standard properties
			//	defined by the base classes :
			
			while (type != null)
			{
				if (type.localStandardProperties != null)
				{
					foreach (DependencyProperty property in type.localStandardProperties)
					{
						property.AddDerivedType (this.systemType);
					}
				}
				else if (type.standardPropertiesArray != null)
				{
					foreach (DependencyProperty property in type.standardPropertiesArray)
					{
						property.AddDerivedType (this.systemType);
					}
					
					break;
				}
				
				type = type.baseType;
			}
		}
		
		public DependencyObjectType						BaseType
		{
			get
			{
				return this.baseType;
			}
		}
		public System.Type						SystemType
		{
			get
			{
				return this.systemType;
			}
		}
		public string							Name
		{
			get
			{
				return this.systemType.Name;
			}
		}
		
		public bool IsObjectInstanceOfType(DependencyObject o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			if (o.ObjectType == this)
			{
				return true;
			}
			if (o.ObjectType.IsSubclassOf (this))
			{
				return true;
			}
			
			return false;
		}
		public bool IsSubclassOf(DependencyObjectType type)
		{
			DependencyObjectType base_type = this.BaseType;
			
			if (base_type == type)
			{
				return true;
			}
			
			if ((base_type != null) &&
				(base_type.IsSubclassOf (type)))
			{
				return true;
			}
			
			return false;
		}
		
		public void Register(DependencyProperty property)
		{
			System.Diagnostics.Debug.Assert (property.IsOwnedBy (this.SystemType));
			System.Diagnostics.Debug.Assert (this.localStandardProperties != null);
			System.Diagnostics.Debug.Assert (this.localAttachedProperties != null);

			if (property.IsAttached)
			{
				this.localAttachedProperties.Add (property);
			}
			else
			{
				this.localStandardProperties.Add (property);
			}
		}

		public ReadOnlyArray<DependencyProperty> GetProperties()
		{
			if (this.standardPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<DependencyProperty> (this.standardPropertiesArray);
		}
		public ReadOnlyArray<DependencyProperty> GetAttachedProperties()
		{
			if (this.attachedPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<DependencyProperty> (this.attachedPropertiesArray);
		}

		public DependencyProperty GetProperty(string name)
		{
			if (this.lookup == null)
			{
				this.BuildPropertyList ();
			}

			if (this.lookup.ContainsKey (name))
			{
				return this.lookup[name];
			}
			
			//	TODO: trouver une propriété attachée qui conviendrait
			
			return null;
		}
		public DependencyProperty GetProperty(DependencyProperty property)
		{
			if (this.lookup == null)
			{
				this.BuildPropertyList ();
			}

			for (int i = 0; i < this.standardPropertiesArray.Length; i++)
			{
				if (this.standardPropertiesArray[i] == property)
				{
					return property;
				}
			}
			
			return null;
		}

		public static DependencyObjectType FromSystemType(System.Type type)
		{
			lock (DependencyObjectType.types)
			{
				return DependencyObjectType.FromSystemTypeLocked (type);
			}
		}

		internal static DependencyObjectType SetupFromSystemType(System.Type type)
		{
			DependencyObjectType objectType = DependencyObjectType.FromSystemType (type);

			if (objectType.initialized == false)
			{
				lock (objectType)
				{
					if (objectType.initialized == false)
					{
						objectType.InitializeLocked ();
						objectType.initialized = true;
					}
				}
			}
			
			return objectType;
		}

		#region Private Methods
		private void BuildPropertyList()
		{
			lock (this)
			{
				if (this.standardPropertiesArray == null)
				{
					ICollection<DependencyProperty> baseProperties;

					if (this.BaseType == null)
					{
						baseProperties = new DependencyProperty[0];
					}
					else
					{
						baseProperties = this.BaseType.GetProperties ();
					}

					DependencyProperty[] allProperties  = new DependencyProperty[baseProperties.Count + this.localStandardProperties.Count];

					baseProperties.CopyTo (allProperties, 0);
					this.localStandardProperties.CopyTo (allProperties, baseProperties.Count);

					this.standardPropertiesArray = allProperties;
					this.attachedPropertiesArray = this.localAttachedProperties.ToArray ();
					
					this.localStandardProperties = null;
					this.localAttachedProperties = null;

					this.lookup = new Dictionary<string, DependencyProperty> (this.standardPropertiesArray.Length);

					foreach (DependencyProperty property in this.standardPropertiesArray)
					{
						this.lookup[property.Name] = property;
					}
					foreach (DependencyProperty property in this.attachedPropertiesArray)
					{
						this.lookup[property.Name] = property;
					}
				}
			}
		}
		private void ExecuteTypeStaticConstructor()
		{
			System.Reflection.FieldInfo[] infos = this.systemType.GetFields (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			if (infos.Length > 0)
			{
				for (int i = 0; i < infos.Length; i++)
				{
					if (infos[i].FieldType == typeof (DependencyProperty))
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Initialized type {0}", this.Name));
						infos[i].GetValue (null);
						break;
					}
				}
			}
		}

		private static DependencyObjectType FromSystemTypeLocked(System.Type system_type)
		{
			if (system_type == null)
			{
				return null;
			}
			else if (DependencyObjectType.types.ContainsKey (system_type))
			{
				return DependencyObjectType.types[system_type];
			}
			else if (system_type == typeof (DependencyObject))
			{
				DependencyObjectType this_type = new DependencyObjectType (system_type, null);
				
				DependencyObjectType.types[system_type] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
			else
			{
				DependencyObjectType base_type = DependencyObjectType.FromSystemTypeLocked (system_type.BaseType);
				
				if (base_type == null)
				{
					return null;
				}
				
				DependencyObjectType this_type = new DependencyObjectType (system_type, base_type);
				
				DependencyObjectType.types[system_type] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
		}

		private void InitializeLocked()
		{
			
		}
		#endregion


		private DependencyObjectType						baseType;
		private System.Type						systemType;
		private List<DependencyProperty>					localStandardProperties = new List<DependencyProperty> ();
		private List<DependencyProperty>					localAttachedProperties = new List<DependencyProperty> ();
		private DependencyProperty[]						standardPropertiesArray;
		private DependencyProperty[]						attachedPropertiesArray;
		private Dictionary<string, DependencyProperty>	lookup;
		private bool							initialized;

		static Dictionary<System.Type, DependencyObjectType> types = new Dictionary<System.Type, DependencyObjectType> ();
	}
}
