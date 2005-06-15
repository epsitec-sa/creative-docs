//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe ManagedParagraphProperty ...
	/// </summary>
	public class ManagedParagraphProperty : Property
	{
		public ManagedParagraphProperty()
		{
		}
		
		public ManagedParagraphProperty(string manager_name, string[] manager_parameters)
		{
			this.manager_name       = manager_name;
			this.manager_parameters = manager_parameters == null ? new string[0] : (manager_parameters.Clone () as string[]);
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.ManagedParagraph;
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
				return CombinationMode.Accumulate;
			}
		}
		
		
		public string							ManagerName
		{
			get
			{
				return this.manager_name;
			}
		}
		
		public string[]							ManagerParameters
		{
			get
			{
				return this.manager_parameters.Clone () as string[];
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.manager_name),
				/**/				SerializerSupport.SerializeStringArray (this.manager_parameters));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string   manager_name       = SerializerSupport.DeserializeString (args[0]);
			string[] manager_parameters = SerializerSupport.DeserializeStringArray (args[1]);
			
			this.manager_name       = manager_name;
			this.manager_parameters = manager_parameters;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.manager_name);
			checksum.UpdateValue (this.manager_parameters);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ManagedParagraphProperty.CompareEqualContents (this, value as ManagedParagraphProperty);
		}
		
		
		private static bool CompareEqualContents(ManagedParagraphProperty a, ManagedParagraphProperty b)
		{
			return a.manager_name == b.manager_name
				&& Types.Comparer.Equal (a.manager_parameters, b.manager_parameters);
		}
		
		
		
		private string							manager_name;
		private string[]						manager_parameters;
	}
}
