//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for FontProperty.
	/// </summary>
	public class FontProperty : BaseProperty
	{
		public FontProperty()
		{
		}
		
		public FontProperty(string face, string style)
		{
			this.face_name  = face;
			this.style_name = style;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Font;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}

		
		public string							FaceName
		{
			get
			{
				return this.face_name;
			}
			set
			{
				if (this.face_name != value)
				{
					this.face_name = value;
					this.Invalidate ();
				}
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.style_name;
			}
			set
			{
				if (this.style_name != value)
				{
					this.style_name = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeString (this.face_name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.style_name));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = text.Substring (pos, length).Split ('/');
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			this.face_name  = SerializerSupport.DeserializeString (args[0]);
			this.style_name = SerializerSupport.DeserializeString (args[0]);
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.FontProperty);
			
			FontProperty a = this;
			FontProperty b = property as FontProperty;
			FontProperty c = new FontProperty (b.FaceName == null ? a.FaceName : b.FaceName, b.StyleName == null ? a.StyleName : b.StyleName);
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.face_name);
			checksum.UpdateValue (this.style_name);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontProperty.CompareEqualContents (this, value as FontProperty);
		}
		
		
		private static bool CompareEqualContents(FontProperty a, FontProperty b)
		{
			return a.face_name == b.face_name
				&& a.style_name == b.style_name;
		}
		
		
		private string							face_name;
		private string							style_name;
	}
	
	public enum FontStyle : byte
	{
		Normal,
		Italic,
		Oblique,
		
		Other,
	}
	
	public enum FontWeight : byte
	{
		Normal,
		Light,
		Bold,
		
		Other,
	}
}
