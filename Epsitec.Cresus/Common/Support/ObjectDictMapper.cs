//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
					object value = prop.GetValue (data, null);

					if ((value != null) &&
						(value != System.DBNull.Value))
					{
						string key = prop.Name;
						string text = Types.InvariantConverter.ToString (value);

						if (dict.ContainsKey (key))
						{
							dict[key] = text;
						}
						else
						{
							dict.Add (key, text);
						}
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
