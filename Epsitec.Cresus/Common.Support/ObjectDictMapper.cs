//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe ObjectDictMapper permet de faire correspondre les propriétés
	/// publiques d'un objet avec celles d'un dictionnaire de paires clef/valeur
	/// de type string.
	/// </summary>
	public sealed class ObjectDictMapper
	{
		public ObjectDictMapper()
		{
		}
		
		
		public static void CopyToDict(object data, Types.IStringDict dict)
		{
			System.Type type = data.GetType ();
			System.Reflection.PropertyInfo[] props = type.GetProperties (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			
			for (int i = 0; i < props.Length; i++)
			{
				System.Reflection.PropertyInfo prop = props[i];
				
				if ((prop.CanRead) &&
					(prop.CanWrite) &&
					(prop.GetIndexParameters ().Length == 0))
				{
					string key = prop.Name;
					string value = Types.InvariantConverter.ToString (prop.GetValue (data, null));
					
					if (dict.ContainsKey (key))
					{
						dict[key] = value;
					}
					else
					{
						dict.Add (key, value);
					}
				}
			}
		}
		
		public static void CopyFromDict(object data, Types.IStringDict dict)
		{
			System.Type type = data.GetType ();
			System.Reflection.PropertyInfo[] props = type.GetProperties (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			
			for (int i = 0; i < props.Length; i++)
			{
				System.Reflection.PropertyInfo prop = props[i];
				
				string key = prop.Name;

				if ((dict.ContainsKey (key)) &&
					(prop.CanRead) &&
					(prop.CanWrite) &&
					(prop.GetIndexParameters ().Length == 0))
				{
					object value;
					
					if (Types.InvariantConverter.Convert (dict[key], prop.PropertyType, out value))
					{
						prop.SetValue (data, value, null);
					}
				}
			}
		}
		
		public static void UpdateToDict(object data, Types.IStringDict dict)
		{
			//	Comme CopyToDict, mais ne crée pas de nouvelles entrées dans le
			//	dictionnaire (on met uniquement à jour des valeurs qui existaient
			//	déjà).
			
			System.Type type = data.GetType ();
			System.Reflection.PropertyInfo[] props = type.GetProperties (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			
			for (int i = 0; i < props.Length; i++)
			{
				System.Reflection.PropertyInfo prop = props[i];
				
				string key = prop.Name;

				if ((dict.ContainsKey (key)) &&
					(prop.CanRead) &&
					(prop.CanWrite) &&
					(prop.GetIndexParameters ().Length == 0))
				{
					string value = Types.InvariantConverter.ToString (prop.GetValue (data, null));
					
					dict[key] = value;
				}
			}
		}
	}
}
