//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe MetaProperty permet de stocker des méta-informations et des
	/// commentaires à des fragments de texte.
	/// </summary>
	public class MetaProperty : Property
	{
		public MetaProperty()
		{
		}
		
		public MetaProperty(string meta_type, string meta_data)
		{
			this.meta_type = meta_type;
			this.meta_data = meta_data;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Meta;
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
		
		
		public string							MetaType
		{
			get
			{
				return this.meta_type;
			}
		}
		
		public string							MetaData
		{
			get
			{
				return this.meta_data;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new MetaComparer ();
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new MetaProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.meta_type),
				/**/				SerializerSupport.SerializeString (this.meta_data));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 2);
			
			string meta_type = SerializerSupport.DeserializeString (args[0]);
			string meta_data = SerializerSupport.DeserializeString (args[1]);
			
			this.meta_type = meta_type;
			this.meta_data = meta_data;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.meta_type);
			checksum.UpdateValue (this.meta_data);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return MetaProperty.CompareEqualContents (this, value as MetaProperty);
		}
		
		
		private static bool CompareEqualContents(MetaProperty a, MetaProperty b)
		{
			return a.meta_type == b.meta_type
				&& a.meta_data == b.meta_data;
		}
		
		
		#region MetaComparer Class
		private class MetaComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.MetaProperty px = x as Properties.MetaProperty;
				Properties.MetaProperty py = y as Properties.MetaProperty;
				
				int result = string.Compare (px.meta_type, py.meta_type);
				
				if (result == 0)
				{
					result = string.Compare (px.meta_data, py.meta_data);
				}
				
				return result;
			}
			#endregion
		}
		#endregion
		
		
		private string							meta_type;
		private string							meta_data;
	}
}
