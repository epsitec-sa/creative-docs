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
		
		
		internal Internal.StyleTable			InternalStyleTable
		{
			get
			{
				return this.internal_styles;
			}
		}
		
		internal Styles.SimpleStyle				this[ulong code]
		{
			get
			{
				return this.internal_styles.GetStyle (code);
			}
		}
		
		
		public TextStyle NewTextStyle(string name)
		{
			TextStyle style = new TextStyle (name);
			
			this.Attach (style);
			
			return style;
		}
		
		public TextStyle NewTextStyle(string name, System.Collections.ICollection properties)
		{
			TextStyle style = new TextStyle (name, properties);
			
			this.Attach (style);
			
			return style;
		}
		
		
		public void RecycleTextStyle(TextStyle style)
		{
			this.Detach (style);
		}
		
		
		public TextStyle GetTextStyle(string name)
		{
			if (this.text_style_hash.Contains (name))
			{
				return this.text_style_hash[name] as TextStyle;
			}
			else
			{
				return null;
			}
		}
		
		
		private void Attach(TextStyle style)
		{
			string name = style.Name;

			if (this.text_style_hash.Contains (name))
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} already exists", name), "style");
			}
			
			this.text_style_list.Add (style);
			this.text_style_hash[name] = style;
		}
		
		private void Detach(TextStyle style)
		{
			string name = style.Name;

			if (this.text_style_hash.Contains (name))
			{
				this.text_style_list.Remove (style);
				this.text_style_hash.Remove (name);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} does not exist", name), "style");
			}
		}
		
		
		private Internal.StyleTable				internal_styles;
		private System.Collections.ArrayList	text_style_list;
		private System.Collections.Hashtable	text_style_hash;
	}
}
