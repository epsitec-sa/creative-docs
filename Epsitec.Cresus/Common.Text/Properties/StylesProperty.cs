//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		
		public TextStyle[] GetTextStyles(TextContext context)
		{
			if (this.style_cache == null)
			{
				this.RefreshStyleCache (context.StyleList);
			}
			
			return (TextStyle[]) this.style_cache.Clone ();
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
			this.style_cache = null;
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
			
			c.style_names = new string[a.style_names.Length + b.style_names.Length];
			
			a.style_names.CopyTo (c.style_names, 0);
			b.style_names.CopyTo (c.style_names, a.style_names.Length);
			
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
		
		
		private void RefreshStyleCache(StyleList list)
		{
			//	La propriété stocke de manière interne une table des TextStyle
			//	correspondant à la table des noms de styles. Celle-ci ne peut
			//	pas être construite à la désérialisation (car les styles ne sont
			//	pas encore disponibles dans le StyleList).
			
			//	Profite de l'occasion pour remplacer d'éventuelles références à
			//	des styles supprimés (qui ne se trouvent plus dans le StyleList)
			//	par le style par défaut correspondant.
			
			int n = (this.style_names == null) ? 0 : this.style_names.Length;
			
			bool refresh_names = false;
			bool replace_done  = false;
			bool filter_null   = false;
			
			this.style_cache = new TextStyle[n];
			
			for (int i = 0; i < n; i++)
			{
				TextStyle style = list.GetTextStyle (this.style_names[i]);
				
				if (style == null)
				{
					if (!replace_done)
					{
						//	Le style a été supprimé et nous n'avons pas encore généré
						//	de référence au style de paragraphe par défaut :
						
						string         name;
						TextStyleClass text_style_class;
						
						StyleList.SplitFullName(this.style_names[i], out name, out text_style_class);
						
						if (text_style_class == TextStyleClass.Paragraph)
						{
							//	Ne remplace que le style qui faisait référence à un
							//	style de paragraphe; les autres styles peuvent simple-
							//	ment être "oubliés".
							
							style        = list.TextContext.DefaultParagraphStyle;
							replace_done = true;
						}
						
						refresh_names = true;
					}
				}
				
				if (style == null)
				{
					filter_null = true;
				}
				
				this.style_cache[i] = style;
			}
			
			if (filter_null)
			{
				this.style_cache = TextStyle.FilterNullStyles (this.style_cache);
			}
			
			if (refresh_names)
			{
				//	Regénère la table des noms de styles; ceci est nécessaire, car
				//	nous avons fait le ménage plus haut :
				
				this.style_names = new string[this.style_cache.Length];
				
				for (int i = 0; i < this.style_cache.Length; i++)
				{
					TextStyle style = this.style_cache[i];
					string    name  = StyleList.GetFullName (style.Name, style.TextStyleClass);
					
					this.style_names[i] = name;
				}
			}
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
		private TextStyle[]						style_cache;
	}
}
