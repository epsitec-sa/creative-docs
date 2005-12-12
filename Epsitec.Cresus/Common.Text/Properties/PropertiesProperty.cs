//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe PropertiesProperty contient une liste de propri�t�s s�rialis�es
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class PropertiesProperty : Property
	{
		public PropertiesProperty()
		{
			this.serialized_properties = new string[0];
		}
		
		public PropertiesProperty(string[] serialized_properties)
		{
			this.serialized_properties = serialized_properties.Clone () as string[];
		}
		
		public PropertiesProperty(System.Collections.ICollection properties)
		{
			this.serialized_properties = new string[properties.Count];
			
			Property[] props  = new Property[properties.Count];
			
			properties.CopyTo (props, 0);
			
			this.serialized_properties = PropertiesProperty.SerializeProperties (props);
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
				return this.serialized_properties.Clone () as string[];
			}
		}
		
		public string[]							SerializedUniformParagraphProperties
		{
			get
			{
				int count = 0;
				int index = 0;
				
				for (int i = 0; i < this.serialized_properties.Length; i++)
				{
					if (this.serialized_properties[i][0] == 'P')
					{
						count++;
					}
				}
				
				string[] copy = new string[count];
				
				for (int i = 0; i < this.serialized_properties.Length; i++)
				{
					if (this.serialized_properties[i][0] == 'P')
					{
						copy[index++] = this.serialized_properties[i];
						
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
				
				for (int i = 0; i < this.serialized_properties.Length; i++)
				{
					if (this.serialized_properties[i][0] != 'P')
					{
						count++;
					}
				}
				
				string[] copy = new string[count];
				
				for (int i = 0; i < this.serialized_properties.Length; i++)
				{
					if (this.serialized_properties[i][0] != 'P')
					{
						copy[index++] = this.serialized_properties[i];
						
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
		
		public static string[] SerializeProperties(Property[] properties)
		{
			if ((properties == null) ||
				(properties.Length == 0))
			{
				return new string[0];
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string[]   serialized_properties = new string[properties.Length];
			
			for (int i = 0; i < properties.Length; i++)
			{
				char flag = properties[i].RequiresUniformParagraph ? 'P' : 'X';
				
				buffer.Length = 0;
				buffer.Append (flag);
				buffer.Append ('.');
				
				Property.SerializeToText (buffer, properties[i]);
				
				serialized_properties[i] = buffer.ToString ();
			}
			
			return serialized_properties;
		}
		
		public static Property[] DeserializeProperties(TextContext context, string[] serialized_properties)
		{
			if ((serialized_properties == null) ||
				(serialized_properties.Length == 0))
			{
				return new Property[0];
			}
			
			Property[] properties = new Property[serialized_properties.Length];
			
			for (int i = 0; i < properties.Length; i++)
			{
				properties[i] = PropertiesProperty.DeserializeProperty (context, serialized_properties[i]);
			}
			
			return properties;
		}
		
		public static Property DeserializeProperty(TextContext context, string serialized_property)
		{
			Property property;
			Property.DeserializeFromText (context, serialized_property, 2, serialized_property.Length-2, out property);
			return property;
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (this.serialized_properties));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] serialized_properties = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.serialized_properties = serialized_properties;
		}

		public override Property GetCombination(Property property)
		{
			//	Produit une propri�t� qui est le r�sultat de la combinaison de
			//	la propri�t� actuelle avec celle pass�e en entr�e (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.PropertiesProperty);
			
			PropertiesProperty a = this;
			PropertiesProperty b = property as PropertiesProperty;
			PropertiesProperty c = new PropertiesProperty ();
			
			//	TODO: g�rer les doublets
			
			c.serialized_properties = new string[a.serialized_properties.Length + b.serialized_properties.Length];
			
			a.serialized_properties.CopyTo (c.serialized_properties, 0);
			b.serialized_properties.CopyTo (c.serialized_properties, a.serialized_properties.Length);
			
			return c;
		}
		
		public override bool CompareEqualContents(object value)
		{
			return PropertiesProperty.CompareEqualContents (this, value as PropertiesProperty);
		}
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			foreach (string property in this.serialized_properties)
			{
				checksum.UpdateValue (property);
			}
		}
		
		
		public static bool ContainsPropertiesProperties(System.Collections.ICollection properties)
		{
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
			//	Supprime les propri�t�s PropertiesProprty de la liste.
			
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
			return Types.Comparer.Equal (a.serialized_properties, b.serialized_properties);
		}
		
		
		private string[]						serialized_properties;
	}
}
