//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
					this.RebuildTextLayout ();
				}
			}
		}
		
		public Drawing.Size				ImageSize
		{
			get
			{
				return this.image_size;
			}
			set
			{
				if (this.image_size != value)
				{
					this.image_size = value;
					this.RebuildTextLayout ();
				}
			}
		}
		
		public double					VerticalOffset
		{
			get
			{
				return this.vertical_offset;
			}
			set
			{
				if (this.vertical_offset != value)
				{
					this.vertical_offset = value;
					this.RebuildTextLayout ();
				}
			}
		}

		
		private void RebuildTextLayout()
		{
			if ((this.ImageName == null) ||
				(this.ImageName.Length == 0))
			{
				this.Text = "";
			}
			else
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append ("<img src=\"");
				buffer.Append (TextLayout.ConvertToTaggedText (this.ImageName));
				buffer.Append ("\"");
				
				int vOffset = (int) (this.VerticalOffset * 100 + 0.5);
				
				int imageDx = (int) (this.image_size.Width + 0.5);
				int imageDy = (int) (this.image_size.Height + 0.5);
				
				if (vOffset != 0)
				{
					buffer.Append (" voff=\"");
					buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}", vOffset / 100.0);
					buffer.Append ("\"");
				}
				
				if (imageDx > 0)
				{
					buffer.Append (" dx=\"");
					buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}", imageDx);
					buffer.Append ("\"");
				}
				if (imageDy > 0)
				{
					buffer.Append (" dy=\"");
					buffer.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, "{0}", imageDy);
					buffer.Append ("\"");
				}
				
				buffer.Append ("/>");
					
				this.Text = buffer.ToString ();
			}
		}
		
		
		protected string				image_name;
		protected Drawing.Size			image_size;
		protected double				vertical_offset;
	}
}
