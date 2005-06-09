//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe PropertiesProperty contient une liste de propriétés sérialisées
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class PropertiesProperty : BaseProperty
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
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			BaseProperty[]            props  = new BaseProperty[properties.Count];
			
			properties.CopyTo (props, 0);
			
			for (int i = 0; i < properties.Count; i++)
			{
				buffer.Length = 0;
				props[i].SerializeToText (buffer);
				
				this.serialized_properties[i] = buffer.ToString ();
			}
		}
		
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (this.serialized_properties));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] serialized_properties = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.serialized_properties = serialized_properties;
		}

		public override BaseProperty GetCombination(BaseProperty property)
		{
			//	Produit une propriété qui est le résultat de la combinaison de
			//	la propriété actuelle avec celle passée en entrée (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.PropertiesProperty);
			
			PropertiesProperty a = this;
			PropertiesProperty b = property as PropertiesProperty;
			PropertiesProperty c = new PropertiesProperty ();
			
			//	TODO: gérer les doublets
			
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
		
		
		private static bool CompareEqualContents(PropertiesProperty a, PropertiesProperty b)
		{
			return Types.Comparer.Equal (a.serialized_properties, b.serialized_properties);
		}
		
		
		private string[]						serialized_properties;
	}
}
