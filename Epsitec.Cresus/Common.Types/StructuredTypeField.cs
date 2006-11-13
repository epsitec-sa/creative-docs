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
		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		public StructuredTypeField(string id, INamedType type)
		{
			this.id = id;
			this.type = type;
			this.captionId = Support.Druid.Empty;
			this.rank = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId)
		{
			this.id = id;
			this.type = type;
			this.captionId = captionId;
			this.rank = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank)
		{
			this.id = id;
			this.type = type;
			this.captionId = captionId;
			this.rank = rank;
		}

		/// <summary>
		/// Gets the field id.
		/// </summary>
		/// <value>The field id.</value>
		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the field type.
		/// </summary>
		/// <value>The field type.</value>
		public INamedType						Type
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the field caption DRUID.
		/// </summary>
		/// <value>The field caption DRUID.</value>
		public Support.Druid					CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		/// <summary>
		/// Gets the field rank; this is used to sort fields in the
		/// user interface.
		/// </summary>
		/// <value>The field rank or <c>-1</c> if no rank has been defined.</value>
		public int								Rank
		{
			get
			{
				return this.rank;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this field is empty.
		/// </summary>
		/// <value><c>true</c> if this field is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return (this.id == null) && (this.type == null) && (this.rank == 0) && (this.captionId.IsEmpty);
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

				string captionId = field.type == null ? Support.Druid.Empty.ToString () : field.type.CaptionId.ToString ();
				string rank = field.rank == -1 ? "" : field.rank.ToString (System.Globalization.CultureInfo.InvariantCulture);

				if (field.captionId.IsValid)
				{
					return string.Concat (field.id, ";", captionId, ";", rank, ";", field.captionId.ToString ());
				}
				else if (string.IsNullOrEmpty (rank))
				{
					return string.Concat (field.id, ";", captionId);
				}
				else
				{
					return string.Concat (field.id, ";", captionId, ";", rank);
				}
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				Support.ResourceManager manager = Serialization.Context.GetResourceManager (context);
				
				string[] args = value.Split (';');
				
				string        name      = args[0];
				Support.Druid druid     = Support.Druid.Parse (args[1]);
				INamedType    type      = druid.IsEmpty ? null : (manager == null ? TypeRosetta.GetTypeObject (druid) : TypeRosetta.GetTypeObject (manager.GetCaption (druid)));
				string        rank      = args.Length < 3 ? "-1" : string.IsNullOrEmpty (args[2]) ? "-1" : args[2];
				Support.Druid captionId = args.Length < 4 ? Support.Druid.Empty : Support.Druid.Parse (args[3]);
				
				return new StructuredTypeField (name, type, captionId, System.Int32.Parse (rank, System.Globalization.CultureInfo.InvariantCulture));
			}

			#endregion
		}

		#endregion
		
		private string							id;
		private INamedType						type;
		private Support.Druid					captionId;
		private int								rank;
	}
}
