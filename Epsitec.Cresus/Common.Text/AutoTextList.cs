//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe AutoTextList gère la liste des texte automatiques, accessibles
	/// par leur nom.
	/// </summary>
	public sealed class AutoTextList
	{
		public AutoTextList(Text.Context context)
		{
			this.context    = context;
			this.auto_texts = new System.Collections.Hashtable ();
		}
		
		
		public AutoText							this[string name]
		{
			get
			{
				return this.auto_texts[name] as AutoText;
			}
		}
		
		public AutoText							this[Properties.AutoTextProperty property]
		{
			get
			{
				return property == null ? null : this[property.Source];
			}
		}
		
		
		public AutoText NewAutoText(string name)
		{
			System.Diagnostics.Debug.Assert (this.auto_texts.Contains (name) == false);
			
			AutoText auto_text = new AutoText (name);
			
			this.auto_texts[name] = auto_text;
			
			return auto_text;
		}
		
		public void DisposeAutoText(AutoText auto_text)
		{
			System.Diagnostics.Debug.Assert (this.auto_texts.Contains (auto_text.Name));
			
			this.auto_texts.Remove (auto_text.Name);
		}
		
		
		
		private Text.Context					context;
		private System.Collections.Hashtable	auto_texts;
	}
}
