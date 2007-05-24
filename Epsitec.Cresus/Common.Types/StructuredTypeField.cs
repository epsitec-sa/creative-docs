//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		public StructuredTypeField()
			: this (null, null, Support.Druid.Empty)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		public StructuredTypeField(string id, INamedType type)
			: this (id, type, Support.Druid.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId)
			: this (id, type, captionId, -1, FieldRelation.None, null, FieldMembership.Local)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank)
			: this (id, type, captionId, rank, FieldRelation.None, null, FieldMembership.Local)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The relation.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation)
			: this (id, type, captionId, rank, relation, null, FieldMembership.Local)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The relation.</param>
		/// <param name="sourceFieldId">The source field id.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation, string sourceFieldId)
			: this (id, type, captionId, rank, relation, sourceFieldId, FieldMembership.Local)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The relation.</param>
		/// <param name="sourceFieldId">The source field id.</param>
		/// <param name="membership">The field membership.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation, string sourceFieldId, FieldMembership membership)
		{
			this.id = id ?? (captionId.IsValid ? captionId.ToString () : null);
			this.captionId = captionId;
			this.rank = rank;
			this.relation = relation;
			this.membership = membership;
			this.sourceFieldId = string.IsNullOrEmpty (sourceFieldId) ? null : sourceFieldId;
			this.DefineType (type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="flags">The flags.</param>
		/// <param name="sourceFieldId">The source field id.</param>
		private StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, int flags, string sourceFieldId)
		{
			this.id = id ?? (captionId.IsValid ? captionId.ToString () : null);
			this.captionId = captionId;
			this.rank = rank;
			this.relation = (FieldRelation) ((flags >> StructuredTypeField.RelationShift) & StructuredTypeField.RelationMask);
			this.membership = (FieldMembership) ((flags >> StructuredTypeField.MembershipShift) & StructuredTypeField.MembershipMask);
			this.sourceFieldId = string.IsNullOrEmpty (sourceFieldId) ? null : sourceFieldId;
			this.DefineType (type);
		}

		internal StructuredTypeField(StructuredTypeField model, FieldMembership membership)
			: this (model.id, model.type, model.captionId, model.rank, model.relation, model.sourceFieldId, membership)
		{
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
		/// Gets the relation defined by this field. This is useful when the
		/// field is a reference.
		/// </summary>
		/// <value>The relation.</value>
		public FieldRelation							Relation
		{
			get
			{
				return this.relation;
			}
		}

		/// <summary>
		/// Gets the field membership (either <c>Local</c> or <c>Inherited</c>).
		/// </summary>
		/// <value>The field membership.</value>
		public FieldMembership					Membership
		{
			get
			{
				return this.membership;
			}
		}

		/// <summary>
		/// Gets the ID of the source field if this field defines an inclusion
		/// relation (see <see cref="T:Relation.Inclusion"/>).
		/// </summary>
		/// <value>The source field id or <c>null</c>.</value>
		public string							SourceFieldId
		{
			get
			{
				return this.sourceFieldId;
			}
		}

		/// <summary>
		/// Resets the rank; the use of this method is reserved for <see cref="StructuredType"/>.
		/// </summary>
		/// <param name="rank">The rank.</param>
		internal void ResetRank(int rank)
		{
			this.rank = rank;
		}

		#region Private Fields

		/// <summary>
		/// Gets a bitwise representation of the relation property and other
		/// settings such as the field membership.
		/// </summary>
		/// <value>The flags.</value>
		private int								Flags
		{
			get
			{
				int flags = 0;
				
				flags |= (StructuredTypeField.RelationMask & (int) this.relation) << StructuredTypeField.RelationShift;
				flags |= (StructuredTypeField.MembershipMask & (int) this.membership) << StructuredTypeField.MembershipShift;

				return flags;
			}
		}

		#endregion

		private void DefineType(INamedType abstractType)
		{
			if ((abstractType != null) &&
				(abstractType != this.type))
			{
				if (this.type != null)
				{
					throw new System.InvalidOperationException ("Field type is immutable");
				}

				bool isStructuredType = abstractType is IStructuredType;

				switch (this.relation)
				{
					case FieldRelation.Collection:
					case FieldRelation.Reference:
						if (!isStructuredType)
						{
							throw new System.ArgumentException (string.Format ("Invalid type {0} in relation {1} for field {2}", abstractType.Name, this.relation, this.id));
						}
						break;

					case FieldRelation.Inclusion:
						if (!isStructuredType)
						{
							throw new System.ArgumentException (string.Format ("Invalid type {0} in relation {1} for field {2}", abstractType.Name, this.relation, this.id));
						}
						if (this.sourceFieldId == null)
						{
							throw new System.ArgumentException (string.Format ("Invalid relation {0} for field {1}, no source field id specified", this.relation, this.id));
						}
						break;
				}

				this.type = abstractType;
			}
		}
		
		#region SerializationConverter Class

		public sealed class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				StructuredTypeField field = (StructuredTypeField) value;

				string captionId = field.type == null ? Support.Druid.Empty.ToString () : field.type.CaptionId.ToString ();
				string rank  = field.rank == -1 ? "" : field.rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string flags = field.Flags == 0 ? "" : field.Flags.ToString (System.Globalization.CultureInfo.InvariantCulture);

				if (field.sourceFieldId != null)
				{
					return string.Concat (field.id, ";", captionId, ";", rank, ";", field.captionId.ToString (), ";", flags, ";", field.sourceFieldId);
				}
				else if (!string.IsNullOrEmpty (flags))
				{
					return string.Concat (field.id, ";", captionId, ";", rank, ";", field.captionId.ToString (), ";", flags);
				}
				else if (field.captionId.IsValid)
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
				string        rank      = args.Length < 3 ? "-1" : string.IsNullOrEmpty (args[2]) ? "-1" : args[2];
				Support.Druid captionId = args.Length < 4 ? Support.Druid.Empty : Support.Druid.Parse (args[3]);
				string        flags     = args.Length < 5 ? "0" : args[4];
				string        sourceId  = args.Length < 6 ? null : args[5];

				int rankValue  = System.Int32.Parse (rank, System.Globalization.CultureInfo.InvariantCulture);
				int flagsValue = System.Int32.Parse (flags, System.Globalization.CultureInfo.InvariantCulture);
				
				StructuredTypeField field = new StructuredTypeField (name, null, captionId, rankValue, flagsValue, sourceId);

				if (druid.IsValid)
				{
					INamedType type = TypeRosetta.GetTypeObject (druid);

					if (type == null)
					{
						//	If the type is not yet known to TypeRosetta, queue a request for
						//	a later fix-up instead of trying to resolve it here and now, as
						//	this could lead to endless loops :
						
						TypeRosetta.QueueFixUp (
							delegate ()
							{
								Caption caption = (manager ?? Support.Resources.DefaultManager).GetCaption (druid);
								field.DefineType (TypeRosetta.GetTypeObject (caption));
							});
					}
					else
					{
						field.type = type;
					}
				}
				
				return field;
			}

			#endregion
		}

		#endregion


		const int RelationShift = 0;
		const int RelationMask = 0x0f;
		const int MembershipShift = 4;
		const int MembershipMask = 0x03;
		
		private readonly string					id;
		private INamedType						type;
		private readonly Support.Druid			captionId;
		private int								rank;
		private readonly FieldRelation				relation;
		private readonly FieldMembership		membership;
		private readonly string					sourceFieldId;
	}
}
