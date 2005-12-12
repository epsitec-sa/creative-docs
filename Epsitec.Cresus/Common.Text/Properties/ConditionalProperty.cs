//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe ConditionalProperty d�finit la condition qui doit �tre remplie
	/// pour que le texte soit consid�r� comme visible.
	/// </summary>
	public class ConditionalProperty : Property
	{
		public ConditionalProperty()
		{
		}
		
		public ConditionalProperty(string condition) : this (condition, true)
		{
		}
		
		public ConditionalProperty(string condition, bool show_if_true)
		{
			this.condition    = condition;
			this.show_if_true = show_if_true;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Conditional;
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
				return CombinationMode.Accumulate;
			}
		}

		
		public string							Condition
		{
			get
			{
				return this.condition;
			}
		}
		
		public bool								ShowIfTrue
		{
			get
			{
				return this.show_if_true;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new ConditionalProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.condition),
				/**/				SerializerSupport.SerializeBoolean (this.show_if_true));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string condition    = SerializerSupport.DeserializeString (args[0]);
			bool   show_if_true = SerializerSupport.DeserializeBoolean (args[1]);
			
			this.condition    = condition;
			this.show_if_true = show_if_true;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.condition);
			checksum.UpdateValue (this.show_if_true);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ConditionalProperty.CompareEqualContents (this, value as ConditionalProperty);
		}
		
		
		private static bool CompareEqualContents(ConditionalProperty a, ConditionalProperty b)
		{
			return a.condition == b.condition
				&& a.show_if_true == b.show_if_true;
		}
		
		
		
		private string							condition;
		private bool							show_if_true;
	}
}
