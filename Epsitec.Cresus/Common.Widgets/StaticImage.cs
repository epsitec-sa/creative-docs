//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
				return this.imageName;
			}
			
			set
			{
				if (this.imageName != value)
				{
					this.imageName = value;
					this.RebuildTextLayout ();
				}
			}
		}
		
		public Drawing.Size				ImageSize
		{
			get
			{
				return this.imageSize;
			}
			set
			{
				if (this.imageSize != value)
				{
					this.imageSize = value;
					this.RebuildTextLayout ();
				}
			}
		}
		
		public double					VerticalOffset
		{
			get
			{
				return this.verticalOffset;
			}
			set
			{
				if (this.verticalOffset != value)
				{
					this.verticalOffset = value;
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
				
				int imageDx = (int) (this.imageSize.Width + 0.5);
				int imageDy = (int) (this.imageSize.Height + 0.5);
				
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
		
		
		protected string				imageName;
		protected Drawing.Size			imageSize;
		protected double				verticalOffset;
	}
}
