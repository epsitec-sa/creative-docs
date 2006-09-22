//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredTypeField</c> class is used to represent a name/type
	/// pair, when serializing a <see cref="StructuredType"/>.
	/// </summary>
	[SerializationConverter (typeof (StructuredTypeField.SerializationConverter))]
	public class StructuredTypeField
	{
		private StructuredTypeField()
		{
		}

		public StructuredTypeField(KeyValuePair<string, INamedType> field)
		{
			this.name = field.Key;
			this.type = field.Value;
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public INamedType						Type
		{
			get
			{
				return this.type;
			}
		}

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				StructuredTypeField field = (StructuredTypeField) value;
				return string.Format ("{0};{1}", field.name, field.type.CaptionId);
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				string[] args = value.Split (';');
				
				string        name  = args[0];
				Support.Druid druid = Support.Druid.Parse (args[1]);
				INamedType    type  = null;

				//	TODO: re-create type from DRUID
				
				KeyValuePair<string, INamedType> keyValuePair = new KeyValuePair<string, INamedType> (name, type);
				
				return new StructuredTypeField (keyValuePair);
			}

			#endregion
		}

		#endregion
		
		private string							name;
		private INamedType						type;
	}
}
