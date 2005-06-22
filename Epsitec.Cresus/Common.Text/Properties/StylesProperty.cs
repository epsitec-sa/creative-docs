//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe StylesProperty contient une liste de styles (cascadés)
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class StylesProperty : Property
	{
		public StylesProperty()
		{
			this.styles = new TextStyle[0];
		}
		
		public StylesProperty(TextStyle style)
		{
			this.styles = new TextStyle[1];
			this.styles[0] = style;
		}
		
		public StylesProperty(System.Collections.ICollection styles)
		{
			this.styles = new TextStyle[styles.Count];
			styles.CopyTo (this.styles, 0);
		}

		
		public override long					Version
		{
			get
			{
				long version = base.Version;
				
				for (int i = 0; i < this.styles.Length; i++)
				{
					long style_version = this.styles[i].Version;
					
					if (style_version > version)
					{
						version = style_version;
					}
				}
				
				return version;
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
				return WellKnownType.Styles;
			}
		}
		
		
		public TextStyle[]						Styles
		{
			get
			{
				return this.styles.Clone () as TextStyle[];
			}
		}
		
		
		public int								CountStyles
		{
			get
			{
				return this.styles.Length;
			}
		}
		
		public int								CountParagraphStyles
		{
			get
			{
				int count = 0;
				
				for (int i = 0; i < this.styles.Length; i++)
				{
					if (this.styles[i].TextStyleClass == TextStyleClass.Paragraph)
					{
						count++;
					}
				}
				
				return count;
			}
		}
		
		public int								CountOtherStyles
		{
			get
			{
				int count = 0;
				
				for (int i = 0; i < this.styles.Length; i++)
				{
					if (this.styles[i].TextStyleClass != TextStyleClass.Paragraph)
					{
						count++;
					}
				}
				
				return count;
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			string[] names = new string[this.styles.Length];
			
			//	Les styles sont définis par un nom et une classe d'appartenance
			//	(par exemple "Default" et TextStyleClass.Paragraph). Il faut
			//	conserver les deux en cas de sérialisation.
			
			for (int i = 0; i < this.styles.Length; i++)
			{
				names[i] = StyleList.GetFullName (this.styles[i].Name, this.styles[i].TextStyleClass);
			}
			
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeStringArray (names));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string[] names = SerializerSupport.DeserializeStringArray (args[0]);
			
			this.styles = new TextStyle[names.Length];
			
			for (int i = 0; i < names.Length; i++)
			{
				TextStyleClass text_style_class;
				string         name;
				
				StyleList.SplitFullName (names[i], out name, out text_style_class);
				
				this.styles[i] = context.StyleList.GetTextStyle (name, text_style_class);
			}
		}

		public override Property GetCombination(Property property)
		{
			//	Produit une propriété qui est le résultat de la combinaison de
			//	la propriété actuelle avec celle passée en entrée (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.StylesProperty);
			
			StylesProperty a = this;
			StylesProperty b = property as StylesProperty;
			StylesProperty c = new StylesProperty ();
			
			//	TODO: gérer les doublets
			
			c.styles = new TextStyle[a.styles.Length + b.styles.Length];
			
			a.styles.CopyTo (c.styles, 0);
			b.styles.CopyTo (c.styles, a.styles.Length);
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}
		
		public override bool CompareEqualContents(object value)
		{
			return StylesProperty.CompareEqualContents (this, value as StylesProperty);
		}
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			foreach (TextStyle style in this.styles)
			{
				checksum.UpdateValue (style.GetContentsSignature ());
			}
		}
		
		
		public static Property[] RemoveStylesProperties(System.Collections.ICollection properties)
		{
			//	Supprime les propriétés StylesProprty de la liste.
			
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
			return StylesProperty.CompareEqualContents (a.styles, b.styles);
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
		
		
		private TextStyle[]						styles;
	}
}
