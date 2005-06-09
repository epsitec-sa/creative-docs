//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe StylesProperty contient une liste de styles (cascadés)
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class StylesProperty : BaseProperty
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
			set
			{
				if (value == null)
				{
					value = new TextStyle[0];
				}
				
				if (StylesProperty.CompareEqualContents (this.styles, value) == false)
				{
					this.styles = new TextStyle[value.Length];
					value.CopyTo (this.styles, 0);
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			string[] names = new string[this.styles.Length];
			
			for (int i = 0; i < this.styles.Length; i++)
			{
				names[i] = this.styles[i].Name;
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
				this.styles[i] = context.StyleList.GetTextStyle (names[i]);
			}
		}

		public override BaseProperty GetCombination(BaseProperty property)
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
				if (a[i].GetContentsSignature () != b[i].GetContentsSignature ())
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
