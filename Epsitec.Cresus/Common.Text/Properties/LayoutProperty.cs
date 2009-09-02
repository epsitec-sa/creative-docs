//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe LayoutProperty décrit quel moteur de layout utiliser pour
	/// un fragment de texte.
	/// </summary>
	public class LayoutProperty : Property
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
				return PropertyType.CoreSetting;
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
				return this.engineName;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new LayoutProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeString (this.engineName));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			this.engineName = SerializerSupport.DeserializeString (text.Substring (pos, length));
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ("Cannot combine layouts.");
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.engineName);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LayoutProperty.CompareEqualContents (this, value as LayoutProperty);
		}
		
		
		private static bool CompareEqualContents(LayoutProperty a, LayoutProperty b)
		{
			if (a.engineName == b.engineName)
			{
				return true;
			}
			
			return false;
		}
		
		
		private string							engineName;
	}
}
