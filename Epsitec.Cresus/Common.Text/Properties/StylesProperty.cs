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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			for (int i = 0; i < this.styles.Length; i++)
			{
				if (i > 0)
				{
					buffer.Append (';');
				}
				
				buffer.Append (this.styles[i].Name);
			}
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = text.Substring (pos, length).Split (';');
			
			if (args[0].Length > 0)
			{
				this.styles = new TextStyle[args.Length];
				
				for (int i = 0; i < args.Length; i++)
				{
					this.styles[i] = context.StyleList.GetTextStyle (args[i]);
				}
			}
			else
			{
				this.styles = new TextStyle[0];
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
			
			c.styles = new TextStyle[a.styles.Length + b.styles.Length];
			
			a.styles.CopyTo (c.styles, 0);
			b.styles.CopyTo (c.styles, a.styles.Length);
			
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
			if (a.styles.Length != b.styles.Length)
			{
				return false;
			}
			
			int n = a.styles.Length;
			
			for (int i = 0; i < n; i++)
			{
				if (a.GetContentsSignature () != b.GetContentsSignature ())
				{
					return false;
				}
			}
			
			for (int i = 0; i < n; i++)
			{
				if (TextStyle.CompareEqualContents (a.styles[i], b.styles[i]) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		
		private TextStyle[]						styles;
	}
}
