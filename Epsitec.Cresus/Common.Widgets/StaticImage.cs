//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StaticImage dessine une image. Cette image est spécifiée
	/// par son nom.
	/// </summary>
	public class StaticImage : StaticText
	{
		public StaticImage()
		{
		}
		
		public StaticImage(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public StaticImage(string name) : this ()
		{
			this.ImageName = name;
		}
		
		public StaticImage(Widget embedder, string name) : this (embedder)
		{
			this.ImageName = name;
		}
		
		
		public string					ImageName
		{
			get
			{
				return this.image_name;
			}
			
			set
			{
				if (this.image_name != value)
				{
					this.image_name = value;
					
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					buffer.Append ("<img src=\"");
					buffer.Append (TextLayout.ConvertToTaggedText (value));
					buffer.Append ("\"/>");
					
					this.Text = buffer.ToString ();
				}
			}
		}
		
		
		protected string				image_name;
	}
}
