//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La propriété ImageProperty décrit quelle image associer avec un
	/// caractère Unicode ObjectReplacement (uFFFC).
	/// </summary>
	public class ImageProperty : Property, IGlyphRenderer
	{
		public ImageProperty()
		{
		}
		
		public ImageProperty(string image_tag)
		{
			this.image_tag = image_tag;
		}
		
		public ImageProperty(string image_tag, TextContext context) : this (image_tag)
		{
			this.SetupImageRenderer (context);
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Image;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.LocalSetting;
			}
		}
		
		public override PropertyAffinity		PropertyAffinity
		{
			get
			{
				return PropertyAffinity.Symbol;
			}
		}

		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Invalid;
			}
		}
		
		
		public string							ImageTag
		{
			get
			{
				return this.image_tag;
			}
		}
		
		public IGlyphRenderer					ImageRenderer
		{
			get
			{
				return this.image_renderer;
			}
		}
		
		
		public void SetupImageRenderer(TextContext context)
		{
			this.image_renderer = context.FindResource (this.image_tag);
		}

		
		public override Property EmptyClone()
		{
			return new ImageProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.image_tag));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string image_tag = SerializerSupport.DeserializeString (args[0]);
			
			this.image_tag = image_tag;
			
			this.SetupImageRenderer (context);
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.image_tag);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ImageProperty.CompareEqualContents (this, value as ImageProperty);
		}
		
		
		#region IGlyphRenderer Members
		public bool GetGeometry(out double ascender, out double descender, out double advance, out double x1, out double x2)
		{
			if (this.image_renderer == null)
			{
				ascender  = 0;
				descender = 0;
				advance   = 0;
				x1        = 0;
				x2        = 0;
				
				return false;
			}
			
			return this.image_renderer.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
		}
		
		public void RenderGlyph(ITextFrame frame, double x, double y)
		{
			if (this.image_renderer != null)
			{
				this.image_renderer.RenderGlyph (frame, x, y);
			}
		}
		#endregion
		
		private static bool CompareEqualContents(ImageProperty a, ImageProperty b)
		{
			return a.image_tag == b.image_tag;
		}
		
		
		private string							image_tag;
		private IGlyphRenderer					image_renderer;
	}
}
