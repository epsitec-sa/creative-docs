//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe BreakProperty décrit une tabulation.
	/// </summary>
	public class BreakProperty : Property
	{
		public BreakProperty() : this (ParagraphStartMode.Anywhere)
		{
		}
		
		public BreakProperty(ParagraphStartMode start_mode)
		{
			this.start_mode = start_mode;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Break;
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

		
		public ParagraphStartMode				ParagraphStartMode
		{
			get
			{
				return this.start_mode;
			}
		}
		
		
		public static BreakProperty				NewFrame
		{
			get
			{
				return new BreakProperty (ParagraphStartMode.NewFrame);
			}
		}
		
		public static BreakProperty				NewPage
		{
			get
			{
				return new BreakProperty (ParagraphStartMode.NewPage);
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new BreakProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeInt ((int) this.start_mode));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			int start_mode = SerializerSupport.DeserializeInt (args[0]);
			
			this.start_mode = (ParagraphStartMode) start_mode;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ();
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue ((int) this.start_mode);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return BreakProperty.CompareEqualContents (this, value as BreakProperty);
		}
		
		
		private static bool CompareEqualContents(BreakProperty a, BreakProperty b)
		{
			return a.start_mode == b.start_mode;
		}
		
		
		private ParagraphStartMode				start_mode;
	}
}
