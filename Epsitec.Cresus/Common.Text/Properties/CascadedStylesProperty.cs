//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe CascadedStylesProperty contient une liste de styles cascad�s
	/// qui doivent s'appliquer au texte.
	/// </summary>
	public class CascadedStylesProperty : BaseProperty
	{
		public CascadedStylesProperty()
		{
			this.styles = new TextStyle[0];
		}
		
		public CascadedStylesProperty(TextStyle style)
		{
			this.styles = new TextStyle[1];
			this.styles[0] = style;
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
				return WellKnownType.CascadedStyles;
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
			//	Produit une propri�t� qui est le r�sultat de la combinaison de
			//	la propri�t� actuelle avec celle pass�e en entr�e (qui vient
			//	ajouter ses attributs aux attributs actuels).
			
			Debug.Assert.IsTrue (property is Properties.CascadedStylesProperty);
			
			CascadedStylesProperty a = this;
			CascadedStylesProperty b = property as CascadedStylesProperty;
			CascadedStylesProperty c = new CascadedStylesProperty ();
			
			c.styles = new TextStyle[a.styles.Length + b.styles.Length];
			
			a.styles.CopyTo (c.styles, 0);
			b.styles.CopyTo (c.styles, a.styles.Length);
			
			return c;
		}
		
		public override bool CompareEqualContents(object value)
		{
			return CascadedStylesProperty.CompareEqualContents (this, value as CascadedStylesProperty);
		}
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			foreach (TextStyle style in this.styles)
			{
				checksum.UpdateValue (style.GetContentsSignature ());
			}
		}
		
		
		private static bool CompareEqualContents(CascadedStylesProperty a, CascadedStylesProperty b)
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
