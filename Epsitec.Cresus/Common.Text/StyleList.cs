//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleList gère la liste des styles associés à un ou plusieurs
	/// textes.
	/// Note: "StyleList" se prononce comme "stylist" :-)
	/// </summary>
	public sealed class StyleList
	{
		public StyleList()
		{
			this.internal_styles = new Internal.StyleTable ();
			
			this.text_style_list = new System.Collections.ArrayList ();
			this.text_style_hash = new System.Collections.Hashtable ();
		}
		
		
		public long								Version
		{
			get
			{
				return this.StyleVersion.Current;
			}
		}
		
		public StyleVersion						StyleVersion
		{
			get
			{
				return Text.StyleVersion.Default;
			}
		}
		
		
		public TextStyle						this[string name, TextStyleClass text_style_class]
		{
			get
			{
				return this.GetTextStyle (name, text_style_class);
			}
		}
		
		
		#region Internal Properties
		internal Internal.StyleTable			InternalStyleTable
		{
			get
			{
				return this.internal_styles;
			}
		}
		
		internal int							InternalStyleCount
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.text_style_list.Count == this.text_style_hash.Count);
				
				return this.text_style_list.Count;
			}
		}
		
		internal Styles.SimpleStyle				this[ulong code]
		{
			get
			{
				return this.internal_styles.GetStyle (code);
			}
		}
		#endregion
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class)
		{
			TextStyle style = new TextStyle (name, text_style_class);
			
			this.Attach (style);
			
			return style;
		}
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties)
		{
			TextStyle style = new TextStyle (name, text_style_class, properties);
			
			this.Attach (style);
			
			return style;
		}
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			TextStyle style = new TextStyle (name, text_style_class, properties, parent_styles);
			
			this.Attach (style);
			
			return style;
		}
		
		
		public void RedefineTextStyle(TextStyle style, System.Collections.ICollection properties)
		{
			//	Change les propriétés d'un style. Le style devient un style "plat"
			//	indépendant d'autres styles.
			
			System.Diagnostics.Debug.Assert (this.text_style_list.Contains (style));
			
			style.Clear ();
			style.Initialise (properties);
		}
		
		public void RedefineTextStyle(TextStyle style, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			//	Change les propriétés et les parents d'un style. Le style devient
			//	un style dérivé.
			
			System.Diagnostics.Debug.Assert (this.text_style_list.Contains (style));
			System.Diagnostics.Debug.Assert (parent_styles != null);
			System.Diagnostics.Debug.Assert (parent_styles.Count > 0);
			
			style.Clear ();
			style.Initialise (properties, parent_styles);
		}
		
		public void RecycleTextStyle(TextStyle style)
		{
			this.Detach (style);
		}
		
		
		public TextStyle GetTextStyle(string name, TextStyleClass text_style_class)
		{
			string full_name = StyleList.GetFullName (name, text_style_class);
			
			if (this.text_style_hash.Contains (full_name))
			{
				return this.text_style_hash[full_name] as TextStyle;
			}
			else
			{
				return null;
			}
		}
		
		
		#region Internal Methods
		internal Styles.SimpleStyle GetStyleFromIndex(int index)
		{
			return this.internal_styles.GetStyleFromIndex (index);
		}
		
		
		internal Property[] Flatten(ulong code)
		{
			Styles.SimpleStyle style = this[code];
			
			if (style != null)
			{
				return style.Flatten (code);
			}
			else
			{
				return new Property[0];
			}
		}
		
		
		internal static string GetFullName(string name, TextStyleClass text_style_class)
		{
			switch (text_style_class)
			{
				case TextStyleClass.Abstract:	return string.Concat ("A.", name);
				case TextStyleClass.Paragraph:	return string.Concat ("P.", name);
				case TextStyleClass.Text:		return string.Concat ("T.", name);
				case TextStyleClass.Character:	return string.Concat ("C.", name);
				
				default:
					throw new System.ArgumentException ();
			}
		}
		
		internal static void SplitFullName(string full_name, out string name, out TextStyleClass text_style_class)
		{
			if ((full_name.Length < 2) ||
				(full_name[1] != '.'))
			{
				throw new System.ArgumentException ();
			}
			
			char prefix = full_name[0];
			
			switch (prefix)
			{
				case 'A': text_style_class = TextStyleClass.Abstract;	break;
				case 'P': text_style_class = TextStyleClass.Paragraph;	break;
				case 'T': text_style_class = TextStyleClass.Text;		break;
				case 'C': text_style_class = TextStyleClass.Character;	break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			name = full_name.Substring (2);
		}
		#endregion
		
		private void Attach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass text_style_class = style.TextStyleClass;
			string         full_name        = StyleList.GetFullName (name, text_style_class);
			
			if (this.text_style_hash.Contains (full_name))
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) already exists", name, text_style_class), "style");
			}
			
			this.text_style_list.Add (style);
			this.text_style_hash[full_name] = style;
		}
		
		private void Detach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass text_style_class = style.TextStyleClass;
			string         full_name        = StyleList.GetFullName (name, text_style_class);
			
			if (this.text_style_hash.Contains (full_name))
			{
				this.text_style_list.Remove (style);
				this.text_style_hash.Remove (full_name);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) does not exist", name, text_style_class), "style");
			}
		}
		
		
		private Internal.StyleTable				internal_styles;
		private System.Collections.ArrayList	text_style_list;
		private System.Collections.Hashtable	text_style_hash;
	}
}
