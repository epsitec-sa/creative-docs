//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe UserTagProperty permet de stocker des informations supplémentaires,
	/// ainsi que des commentaires, à des fragments de texte.
	/// </summary>
	public class UserTagProperty : Property
	{
		public UserTagProperty()
		{
		}
		
		public UserTagProperty(string tag_type, string tag_data)
		{
			this.tag_type = tag_type;
			this.tag_data = tag_data;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.UserTag;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Accumulate;
			}
		}
		
		
		public string							TagType
		{
			get
			{
				return this.tag_type;
			}
		}
		
		public string							TagData
		{
			get
			{
				return this.tag_data;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new UserTagComparer ();
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new UserTagProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.tag_type),
				/**/				SerializerSupport.SerializeString (this.tag_data));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 2);
			
			string tag_type = SerializerSupport.DeserializeString (args[0]);
			string tag_data = SerializerSupport.DeserializeString (args[1]);
			
			this.tag_type = tag_type;
			this.tag_data = tag_data;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.tag_type);
			checksum.UpdateValue (this.tag_data);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return UserTagProperty.CompareEqualContents (this, value as UserTagProperty);
		}
		
		
		private static bool CompareEqualContents(UserTagProperty a, UserTagProperty b)
		{
			return a.tag_type == b.tag_type
				&& a.tag_data == b.tag_data;
		}
		
		
		#region UserTagComparer Class
		private class UserTagComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.UserTagProperty px = x as Properties.UserTagProperty;
				Properties.UserTagProperty py = y as Properties.UserTagProperty;
				
				int result = string.Compare (px.tag_type, py.tag_type);
				
				if (result == 0)
				{
					result = string.Compare (px.tag_data, py.tag_data);
				}
				
				return result;
			}
			#endregion
		}
		#endregion
		
		
		private string							tag_type;
		private string							tag_data;
	}
}
