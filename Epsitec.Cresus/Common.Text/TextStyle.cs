//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStyle définit un style de texte de haut niveau. Il s'agit
	/// d'une collection de propriétés.
	/// </summary>
	public sealed class TextStyle : Styles.PropertyContainer, IContentsComparer
	{
		internal TextStyle(string name, TextStyleClass text_style_class)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties) : base (properties)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
		}
		
		internal TextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			this.name             = name;
			this.text_style_class = text_style_class;
			
			if ((parent_styles == null) ||
				(parent_styles.Count == 0))
			{
				base.Initialise (properties);
			}
			else
			{
				this.Initialise (properties, parent_styles);
			}
		}
		
		
		public string							MetaId
		{
			get
			{
				return this.meta_id;
			}
		}
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public TextStyleClass					TextStyleClass
		{
			get
			{
				return this.text_style_class;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new TextStyleComparer ();
			}
		}
		
		
		internal void SetMetaId(string meta_id)
		{
			this.meta_id = meta_id;
		}
		
		internal void Clear()
		{
			//	Oublie les dépendances d'un style avec d'éventuels styles parents
			//	pour redevenir un style "plat".
			
			this.parent_styles = null;
			this.style_properties = null;
			this.Invalidate ();
		}
		
		internal void Initialise(System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			//	Initialise un style dérivé d'autres styles. Il faut d'une part
			//	prendre note des styles parents et d'autre part se souvenir des
			//	propriétés additionnelles. En effet, PropertyContainer stockant
			//	toutes les propriétés au même niveau, il ne serait pas possible
			//	d'en regénérer la liste si ces infos n'étaient pas conservées.
			
			this.parent_styles    = new TextStyle[parent_styles.Count];
			this.style_properties = new Property[properties.Count];
				
			parent_styles.CopyTo (this.parent_styles, 0);
			properties.CopyTo (this.style_properties, 0);
				
			this.GenerateStyleProperties ();
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			//	Ignore le nom dans le calcul de la signature. C'est voulu !
			
			checksum.Update ((int) this.text_style_class);
			
			if ((this.parent_styles != null) &&
				(this.parent_styles.Length > 0))
			{
				foreach (TextStyle style in this.parent_styles)
				{
					checksum.Update (style.GetContentsSignature ());
				}
			}
			
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool Update()
		{
			if ((this.parent_styles != null) &&
				(this.parent_styles.Length > 0))
			{
				long version = 0;
				
				foreach (TextStyle style in this.parent_styles)
				{
					version = System.Math.Max (version, style.Version);
				}
				
				if (this.GetInternalVersion () != version)
				{
					this.SetInternalVersion (version);
					this.ClearContentsSignature ();
					this.GenerateStyleProperties ();
					
					return true;
				}
			}
			
			return base.Update ();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (Property property in this)
			{
				buffer.Append (property.GetType ().Name);
				buffer.Append (":");
				property.SerializeToText (buffer);
				buffer.Append ("\n");
			}
			
			return buffer.ToString ();
		}
		
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		#region TextStyleComparer Class
		private class TextStyleComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				TextStyle px = x as TextStyle;
				TextStyle py = y as TextStyle;
				
				if (px.text_style_class < py.text_style_class)
				{
					return -1;
				}
				if (px.text_style_class > py.text_style_class)
				{
					return 1;
				}
				
				return string.Compare (px.Name, py.Name);
			}
			#endregion
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			if ((a.text_style_class == b.text_style_class) &&
				(Styles.PropertyContainer.CompareEqualContents (a, b)))
			{
				//	Ignore le nom dans la comparaison du contenu. C'est voulu !
				
				//	TODO: compléter
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public static TextStyle[] FilterStyles(System.Collections.ICollection styles, params TextStyleClass[] text_style_classes)
		{
			int count = 0;
			
			foreach (TextStyle style in styles)
			{
				for (int i = 0; i < text_style_classes.Length; i++)
				{
					if (style.TextStyleClass == text_style_classes[i])
					{
						count++;
						break;
					}
				}
			}
			
			TextStyle[] filtered = new TextStyle[count];
			
			int index = 0;
			
			foreach (TextStyle style in styles)
			{
				for (int i = 0; i < text_style_classes.Length; i++)
				{
					if (style.TextStyleClass == text_style_classes[i])
					{
						filtered[index++] = style;
						break;
					}
				}
			}
			
			return filtered;
		}
		
		
		private void GenerateStyleProperties()
		{
			System.Diagnostics.Debug.Assert (this.style_properties != null);
			System.Diagnostics.Debug.Assert (this.parent_styles != null);
			
			Styles.PropertyContainer.Accumulator accumulator = new Accumulator ();
			
			foreach (TextStyle style in this.parent_styles)
			{
				accumulator.Accumulate (style);
			}
			
			accumulator.Accumulate (this.style_properties);
			
			base.Initialise (accumulator.AccumulatedProperties);
		}
		
		
		private string							name;
		private string							meta_id;
		private TextStyleClass					text_style_class;
		private TextStyle[]						parent_styles;
		private Property[]						style_properties;
	}
}
