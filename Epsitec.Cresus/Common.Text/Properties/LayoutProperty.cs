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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			//	TODO: ...
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			//	TODO: ...
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			throw new System.InvalidOperationException ("Cannot combine layouts.");
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			//	TODO: ...
			
			//checksum.UpdateValue (...);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LayoutProperty.CompareEqualContents (this, value as LayoutProperty);
		}
		
		
		private static bool CompareEqualContents(LayoutProperty a, LayoutProperty b)
		{
			//	TODO: ...
			
			return true;
		}
	}
}
