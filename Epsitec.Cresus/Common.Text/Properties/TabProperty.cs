//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TabProperty décrit une tabulation.
	/// </summary>
	public class TabProperty : Property
	{
		public TabProperty()
		{
		}
		
		public TabProperty(string tag)
		{
			this.tab_tag = tag;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Tab;
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

		
		public string							TabTag
		{
			get
			{
				return this.tab_tag;
			}
		}
		
		
		
		public override Property EmptyClone()
		{
			return new TabProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.tab_tag));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string tab_tag = SerializerSupport.DeserializeString (args[0]);
			
			this.tab_tag = tab_tag;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ();
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.tab_tag);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return TabProperty.CompareEqualContents (this, value as TabProperty);
		}
		
		
		private static bool CompareEqualContents(TabProperty a, TabProperty b)
		{
			return a.tab_tag == b.tab_tag;
		}
		
		
		private string							tab_tag;
	}
}
