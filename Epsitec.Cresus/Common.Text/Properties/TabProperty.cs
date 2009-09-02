//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.tabTag = tag;
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
				return this.tabTag;
			}
		}
		
		
		
		public override Property EmptyClone()
		{
			return new TabProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.tabTag));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string tabTag = SerializerSupport.DeserializeString (args[0]);
			
			this.tabTag = tabTag;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ();
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.tabTag);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return TabProperty.CompareEqualContents (this, value as TabProperty);
		}
		
		
		private static bool CompareEqualContents(TabProperty a, TabProperty b)
		{
			return a.tabTag == b.tabTag;
		}
		
		
		private string							tabTag;
	}
}
