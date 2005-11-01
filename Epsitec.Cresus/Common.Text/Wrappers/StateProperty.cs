//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// ... StateProperty.
	/// </summary>
	public sealed class StateProperty
	{
		public StateProperty(System.Type state_type, string name, object default_value)
		{
			this.name          = name;
			this.default_value = default_value;
			
			StateProperty.Register (state_type, this);
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public object							DefaultValue
		{
			get
			{
				return this.default_value;
			}
		}
		
		
		public override int GetHashCode()
		{
			return this.name.GetHashCode ();
		}
		
		public override bool Equals(object obj)
		{
			StateProperty that = obj as StateProperty;
			
			if (that == null)
			{
				return false;
			}
			
			return (that.name == this.name);
		}

		
		static private void Register(System.Type state_type, StateProperty property)
		{
			if (StateProperty.types.Contains (state_type) == false)
			{
				StateProperty.types[state_type] = new System.Collections.ArrayList ();
			}
			
			System.Collections.ArrayList list = StateProperty.types[state_type] as System.Collections.ArrayList;
			
			foreach (StateProperty find in list)
			{
				if (find.Name == property.Name)
				{
					throw new System.InvalidOperationException (string.Format ("Property {0} already defined for type {1}", property.Name, state_type.Name));
				}
			}
			
			list.Add (property);
		}
		
		
		static System.Collections.Hashtable		types = new System.Collections.Hashtable ();
		
		private readonly string					name;
		private readonly object					default_value;
	}
}
