//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe PropertiesProperty contient une liste de propriétés sérialisées
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class PropertiesProperty : Property
	{
		public PropertiesProperty()
		{
			this.serializedProperties = Epsitec.Common.Types.Collections.EmptyArray<string>.Instance;
		}
		
		public PropertiesProperty(string[] serializedProperties)
		{
			this.serializedProperties = serializedProperties.Clone () as string[];
		}
		
		public PropertiesProperty(System.Collections.ICollection properties)
		{
			this.serializedProperties = new string[properties.Count];
			
			Property[] props  = new Property[properties.Count];
			
			properties.CopyTo (props, 0);
			
			this.serializedProperties = PropertiesProperty.SerializePropertiesToStringArray (props);
		}
		
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.CoreSetting;
			}
		}
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Properties;
			}
		}
		
		
		public string[]							SerializedProperties
		{
			get
			{
				return this.serializedProperties.Clone () as string[];
			}
		}
		
		public string[]							SerializedUniformParagraphProperties
		{
			get
			{
				int count = 0;
				int index = 0;
				
				for (int i = 0; i < this.serializedProperties.Length; i++)
				{
					if (this.serializedProperties[i][0] == 'P')
					{
						count++;
					}
				}
				
				string[] copy = new string[count];
				
				for (int i = 0; i < this.serializedProperties.Length; i++)
				{
					if (this.serializedProperties[i][0] == 'P')
					{
						copy[index++] = this.serializedProperties[i];
						
						if (index == count)
						{
							break;
						}
					}
				}
				
				return copy;
			}
		}
		
		public string[]							SerializedOtherProperties
		{
			get
			{
				int count = 0;
				int index = 0;
				
				for (int i = 0; i < this.serializedProperties.Length; i++)
				{
					if (this.serializedProperties[i][0] != 'P')
					{
						count++;
					}
				}
				
				string[] copy = new string[count];
				
				for (int i = 0; i < this.serializedProperties.Length; i++)
				{
					if (this.serializedProperties[i][0] != 'P')
					{
						copy[index++] = this.serializedProperties[i];
						
						if (index == count)
						{
							break;
						}
					}
				}
				
				return copy;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new PropertiesProperty ();
		}
		
		public static string[] SerializePropertiesToStringArray(Property[] properties)
		{
			if ((properties == null) ||
				(properties.Length == 0))
			{
				return Epsitec.Common.Types.Collections.EmptyArray<string>.Instance;
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string[]   serializedProperties  = new string[properties.Length];
			
			for (int i = 0; i < properties.Length; i++)
			{
				char flag = properties[i].RequiresUniformParagraph ? 'P' : 'X';
				
				buffer.Length = 0;
				buffer.Append (flag);
				buffer.Append ('.');
				
				Property.SerializeToText (buffer, properties[i]);
				
				serializedProperties[i] = buffer.ToString ();
			}
			
			return serializedProperties;
		}
		
		public static Property[] DeserializePropertiesFromStringArray(TextContext context, string[] serializedProperties)
		{
			if ((serializedProperties == null) ||
				(serializedProperties.Length == 0))
			{
				return new Property[0];
			}
			
			Property[] properties = new Property[serializedProperties.Length];
			
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i] = PropertiesProperty.DeserializeProperty (context, serializedProperties[i]);
			}
			
			return properties;
		}
		
		public static Property DeserializeProperty(TextContext context, string serializedProperty)
		{
			Property property;
			Property.DeserializeFromText (context, serializedProperty, 2, serializedProperty.Length-2, out property);
			return property;
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (this.serializedProperties));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] serializedProperties = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.serializedProperties = serializedProperties;
		}

		public override Property GetCombination(Property property)
		{
			//	Produit une propriété qui est le résultat de la combinaison de
			//	la propriété actuelle avec celle passée en entrée (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.PropertiesProperty);
			
			PropertiesProperty a = this;
			PropertiesProperty b = property as PropertiesProperty;
			PropertiesProperty c = new PropertiesProperty ();
			
			//	TODO: gérer les doublets
			
			c.serializedProperties = new string[a.serializedProperties.Length + b.serializedProperties.Length];
			
			a.serializedProperties.CopyTo (c.serializedProperties, 0);
			b.serializedProperties.CopyTo (c.serializedProperties, a.serializedProperties.Length);
			
			return c;
		}
		
		public override bool CompareEqualContents(object value)
		{
			return PropertiesProperty.CompareEqualContents (this, value as PropertiesProperty);
		}
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			foreach (string property in this.serializedProperties)
			{
				checksum.UpdateValue (property);
			}
		}
		
		
		public static bool ContainsPropertiesProperties(System.Collections.ICollection properties)
		{
			System.Diagnostics.Debug.Assert (properties != null);
			foreach (Property property in properties)
			{
				if (property is PropertiesProperty)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public static Property[] RemovePropertiesProperties(System.Collections.ICollection properties)
		{
			//	Supprime les propriétés PropertiesProprty de la liste.
			
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (! (property is PropertiesProperty))
				{
					count++;
				}
			}
			
			Property[] filtered = new Property[count];
			
			int index = 0;
			
			foreach (Property property in properties)
			{
				if (! (property is PropertiesProperty))
				{
					filtered[index++] = property;
				}
			}
			
			System.Diagnostics.Debug.Assert (index == count);
			
			return filtered;
		}
		
		
		private static bool CompareEqualContents(PropertiesProperty a, PropertiesProperty b)
		{
			return Types.Comparer.Equal (a.serializedProperties, b.serializedProperties);
		}
		
		
		private string[]						serializedProperties;
	}
}
