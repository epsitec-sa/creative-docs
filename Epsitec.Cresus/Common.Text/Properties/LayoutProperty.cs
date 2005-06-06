//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for LayoutProperty.
	/// </summary>
	public class LayoutProperty : BaseProperty
	{
		public LayoutProperty()
		{
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Layout;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Invalid;
			}
		}
		
		
		public string							EngineName
		{
			get
			{
				return this.engine_name;
			}
			set
			{
				if (this.engine_name != value)
				{
					this.engine_name = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeString (this.engine_name));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			this.engine_name = SerializerSupport.DeserializeString (text.Substring (pos, length));
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			throw new System.InvalidOperationException ("Cannot combine layouts.");
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.engine_name);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LayoutProperty.CompareEqualContents (this, value as LayoutProperty);
		}
		
		
		private static bool CompareEqualContents(LayoutProperty a, LayoutProperty b)
		{
			if (a.engine_name == b.engine_name)
			{
				return true;
			}
			
			return false;
		}
		
		
		private string							engine_name;
	}
}
