//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public ImageProperty(string imageTag)
		{
			this.imageTag = imageTag;
		}
		
		public ImageProperty(string imageTag, TextContext context) : this (imageTag)
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
				return this.imageTag;
			}
		}
		
		public IGlyphRenderer					ImageRenderer
		{
			get
			{
				return this.imageRenderer;
			}
		}
		
		
		public void SetupImageRenderer(TextContext context)
		{
			this.imageRenderer = context.FindResource (this.imageTag);
		}

		
		public override Property EmptyClone()
		{
			return new ImageProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.imageTag));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string imageTag = SerializerSupport.DeserializeString (args[0]);
			
			this.imageTag = imageTag;
			
			this.SetupImageRenderer (context);
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.imageTag);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ImageProperty.CompareEqualContents (this, value as ImageProperty);
		}
		
		
		#region IGlyphRenderer Members
		public bool GetGeometry(out double ascender, out double descender, out double advance, out double x1, out double x2)
		{
			if (this.imageRenderer == null)
			{
				ascender  = 0;
				descender = 0;
				advance   = 0;
				x1        = 0;
				x2        = 0;
				
				return false;
			}
			
			return this.imageRenderer.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
		}
		
		public void RenderGlyph(ITextFrame frame, double x, double y)
		{
			if (this.imageRenderer != null)
			{
				this.imageRenderer.RenderGlyph (frame, x, y);
			}
		}
		#endregion
		
		private static bool CompareEqualContents(ImageProperty a, ImageProperty b)
		{
			return a.imageTag == b.imageTag;
		}
		
		
		private string							imageTag;
		private IGlyphRenderer					imageRenderer;
	}
}
