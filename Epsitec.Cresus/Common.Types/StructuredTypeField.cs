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
	public struct StructuredTypeField
	{
		public StructuredTypeField(string id, INamedType type)
		{
			this.id = id;
			this.type = type;
			this.captionId = Support.Druid.Empty;
		}
		
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId)
		{
			this.id = id;
			this.type = type;
			this.captionId = captionId;
		}

		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		public INamedType						Type
		{
			get
			{
				return this.type;
			}
		}

		public Support.Druid					CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		public static readonly StructuredTypeField Empty = new StructuredTypeField ();

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				StructuredTypeField field = (StructuredTypeField) value;

				if (field.captionId.IsValid)
				{
					return string.Concat (field.id, ";", field.type.CaptionId.ToString (), ";", field.captionId.ToString ());
				}
				else
				{
					return string.Concat (field.id, ";", field.type.CaptionId.ToString ());
				}
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				string[] args = value.Split (';');
				
				string        name      = args[0];
				Support.Druid druid     = Support.Druid.Parse (args[1]);
				INamedType    type      = TypeRosetta.GetTypeObject (druid);
				Support.Druid captionId = args.Length < 3 ? Support.Druid.Empty : Support.Druid.Parse (args[2]);
				
				return new StructuredTypeField (name, type, captionId);
			}

			#endregion
		}

		#endregion
		
		private string							id;
		private INamedType						type;
		private Support.Druid					captionId;
	}
}
