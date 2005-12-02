//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe StylesProperty contient une liste de styles (cascad�s)
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class StylesProperty : Property
	{
		public StylesProperty()
		{
			this.Setup (null);
		}
		
		public StylesProperty(TextStyle style)
		{
			this.Setup (new TextStyle[] { style });
		}
		
		public StylesProperty(System.Collections.ICollection styles)
		{
			this.Setup (styles);
		}

		
		private void Setup(System.Collections.ICollection styles)
		{
			int n = styles == null ? 0 : styles.Count;
			int i = 0;
			
			this.style_names = new string[n];
			
			if (n > 0)
			{
				foreach (TextStyle style in styles)
				{
					this.style_names[i++] = StyleList.GetFullName (style.Name, style.TextStyleClass);
				}
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Polymorph;
			}
		}
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Styles;
			}
		}
		
		
		public string[]							StyleNames
		{
			get
			{
				return this.style_names.Clone () as string[];
			}
		}
		
		public int								StyleCount
		{
			get
			{
				return this.style_names.Length;
			}
		}
		
		
		
		public override Property EmptyClone()
		{
			return new StylesProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (this.style_names));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] names = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.style_names = names;
		}

		public override Property GetCombination(Property property)
		{
			//	Produit une propri�t� qui est le r�sultat de la combinaison de
			//	la propri�t� actuelle avec celle pass�e en entr�e (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.StylesProperty);
			
			StylesProperty a = this;
			StylesProperty b = property as StylesProperty;
			StylesProperty c = new StylesProperty ();
			
			//	TODO: g�rer les doublets
			
			c.style_names = new string[a.style_names.Length + b.style_names.Length];
			
			a.style_names.CopyTo (c.style_names, 0);
			b.style_names.CopyTo (c.style_names, a.style_names.Length);
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}
		
		public override bool CompareEqualContents(object value)
		{
			return StylesProperty.CompareEqualContents (this, value as StylesProperty);
		}
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			foreach (string style in this.style_names)
			{
				checksum.UpdateValue (style);
			}
		}
		
		
		public static int CountMatchingStyles(TextStyle[] styles, TextStyleClass style_class)
		{
			int count = 0;
			
			for (int i = 0; i < styles.Length; i++)
			{
				if (styles[i].TextStyleClass == style_class)
				{
					count++;
				}
			}
			
			return count;
		}
		
		
		public static bool ContainsStylesProperties(System.Collections.ICollection properties)
		{
			foreach (Property property in properties)
			{
				if (property is StylesProperty)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public static Property[] RemoveStylesProperties(System.Collections.ICollection properties)
		{
			//	Supprime les propri�t�s StylesProprty de la liste.
			
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (! (property is StylesProperty))
				{
					count++;
				}
			}
			
			Property[] filtered = new Property[count];
			
			int index = 0;
			
			foreach (Property property in properties)
			{
				if (! (property is StylesProperty))
				{
					filtered[index++] = property;
				}
			}
			
			System.Diagnostics.Debug.Assert (index == count);
			
			return filtered;
		}
		
		
		private static bool CompareEqualContents(StylesProperty a, StylesProperty b)
		{
			if (a == b)
			{
				return true;
			}
			
			return Types.Comparer.Equal (a.style_names, b.style_names);
		}
		
		private static bool CompareEqualContents(TextStyle[] a, TextStyle[] b)
		{
			if (a == b)
			{
				return true;
			}
			
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			
			if (a.Length != b.Length)
			{
				return false;
			}
			
			int n = a.Length;
			
			for (int i = 0; i < n; i++)
			{
				if ((a[i].GetContentsSignature () != b[i].GetContentsSignature ()) ||
					(a[i].Name != b[i].Name))
				{
					return false;
				}
			}
			
			for (int i = 0; i < n; i++)
			{
				if (TextStyle.CompareEqualContents (a[i], b[i]) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		private string[]						style_names;
	}
}
