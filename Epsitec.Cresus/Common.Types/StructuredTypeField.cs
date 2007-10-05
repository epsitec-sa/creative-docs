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
	public sealed class StructuredTypeField
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
			: this (id, type, captionId, -1, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)
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
			: this (id, type, captionId, rank, FieldRelation.None, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)
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
			: this (id, type, captionId, rank, relation, FieldMembership.Local, FieldSource.Value, FieldOptions.None, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The field relation.</param>
		/// <param name="membership">The field membership.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation, FieldMembership membership)
			: this (id, type, captionId, rank, relation, membership, FieldSource.Value, FieldOptions.None, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The field relation.</param>
		/// <param name="membership">The field membership.</param>
		/// <param name="source">The field source.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation, FieldMembership membership, FieldSource source)
			: this (id, type, captionId, rank, relation, membership, source, FieldOptions.None, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredTypeField"/> class.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="type">The field type.</param>
		/// <param name="captionId">The field caption DRUID.</param>
		/// <param name="rank">The field rank when listed in a user interface.</param>
		/// <param name="relation">The field relation.</param>
		/// <param name="membership">The field membership.</param>
		/// <param name="source">The field source.</param>
		/// <param name="options">The field options.</param>
		/// <param name="expression">The expression.</param>
		public StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, FieldRelation relation, FieldMembership membership, FieldSource source, FieldOptions options, string expression)
		{
			this.id = id ?? (captionId.IsValid ? captionId.ToString () : null);
			this.captionId = captionId;
			this.rank = rank;
			this.relation = relation;
			this.membership = membership;
			this.source = source;
			this.options = options;
			this.expression = expression;
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
		/// <param name="expression">The expression.</param>
		private StructuredTypeField(string id, INamedType type, Support.Druid captionId, int rank, int flags, string expression)
			: this (id, type, captionId, rank,
					(FieldRelation) ((flags >> StructuredTypeField.RelationShift) & StructuredTypeField.RelationMask),
					(FieldMembership) ((flags >> StructuredTypeField.MembershipShift) & StructuredTypeField.MembershipMask),
					(FieldSource) ((flags >> StructuredTypeField.SourceShift) & StructuredTypeField.SourceMask),
					(FieldOptions) ((flags >> StructuredTypeField.OptionsShift) & StructuredTypeField.OptionsMask), expression)
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
		/// Gets the field type DRUID.
		/// </summary>
		/// <value>The field type DRUID.</value>
		public Support.Druid					TypeId
		{
			get
			{
				return this.typeId;
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
		public FieldRelation					Relation
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
		/// Gets the field source.
		/// </summary>
		/// <value>The field source.</value>
		public FieldSource						Source
		{
			get
			{
				return this.source;
			}
		}

		/// <summary>
		/// Gets the field options.
		/// </summary>
		/// <value>The field options.</value>
		public FieldOptions						Options
		{
			get
			{
				return this.options;
			}
		}

		/// <summary>
		/// Gets the expression.
		/// </summary>
		/// <value>The expression.</value>
		public string							Expression
		{
			get
			{
				return this.expression;
			}
		}


		/// <summary>
		/// Gets the defining type DRUID. This information is produced
		/// and maintained automatically by the <see cref="StructuredType"/>
		/// class.
		/// </summary>
		/// <value>
		/// The defining type DRUID or <c>Druid.Empty</c> if this field is
		/// locally defined in its containing structured type.
		/// </value>
		public Support.Druid					DefiningTypeId
		{
			get
			{
				return this.definingTypeId;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this field is nullable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this field is nullable; otherwise, <c>false</c>.
		/// </value>
		public bool								IsNullable
		{
			get
			{
				return (this.Options & FieldOptions.Nullable) != 0;
			}
		}


		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>A copy of this instance.</returns>
		public StructuredTypeField Clone()
		{
			return this.Clone (this.membership);
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <param name="membership">The field membership to use.</param>
		/// <returns>A copy of this instance.</returns>
		public StructuredTypeField Clone(FieldMembership membership)
		{
			StructuredTypeField copy = new StructuredTypeField (this.id, null, this.captionId, this.rank, this.relation, membership, this.source, this.options, this.expression);

			copy.type           = this.type;
			copy.typeId         = this.typeId;
			copy.definingTypeId = this.definingTypeId;

			return copy;
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <param name="membership">The field membership to use.</param>
		/// <param name="definingTypeId">The defining type DRUID if this field
		/// does not yet define one.</param>
		/// <returns>A copy of this instance.</returns>
		public StructuredTypeField Clone(FieldMembership membership, Support.Druid definingTypeId)
		{
			StructuredTypeField copy = this.Clone (membership);
			
			copy.DefineDefiningTypeId (definingTypeId);

			return copy;
		}

		internal void DefineDefiningTypeId(Support.Druid definingTypeId)
		{
			if (this.definingTypeId.IsEmpty)
			{
				this.definingTypeId = definingTypeId;
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
				flags |= (StructuredTypeField.SourceMask & (int) this.source) << StructuredTypeField.SourceShift;
				flags |= (StructuredTypeField.OptionsMask & (int) this.options) << StructuredTypeField.OptionsShift;

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

				IStructuredType structType = abstractType as IStructuredType;
				
				switch (this.relation)
				{
					case FieldRelation.None:
					case FieldRelation.Collection:
						break;

					case FieldRelation.Reference:
						if (structType == null)
						{
							throw new System.ArgumentException (string.Format ("Invalid type {0} in relation {1} for field {2}", abstractType.Name, this.relation, this.id));
						}
						break;

					default:
						throw new System.ArgumentException (string.Format ("Invalid relation {0} for field {1}", this.relation, this.id));
				}

				this.type   = abstractType;
				this.typeId = abstractType.CaptionId;
			}
		}

		/// <summary>
		/// Defines the type id (only use this internally).
		/// </summary>
		/// <param name="typeId">The type id.</param>
		public void DefineTypeId(Support.Druid typeId)
		{
			System.Diagnostics.Debug.Assert (this.type == null);
			System.Diagnostics.Debug.Assert (this.typeId.IsEmpty);

			this.typeId = typeId;
		}
		
		#region SerializationConverter Class

		public sealed class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				StructuredTypeField field = (StructuredTypeField) value;

				string typeId = field.typeId.ToString ();
				string rank   = field.rank == -1 ? "" : field.rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
				string flags  = field.Flags == 0 ? "" : field.Flags.ToString (System.Globalization.CultureInfo.InvariantCulture);

				if (!string.IsNullOrEmpty (field.Expression))
				{
					return string.Concat (field.id, ";", typeId, ";", rank, ";", field.captionId.ToString (), ";", flags, ";", "Exp", ";", field.Expression);
				}
				else if (!string.IsNullOrEmpty (flags))
				{
					return string.Concat (field.id, ";", typeId, ";", rank, ";", field.captionId.ToString (), ";", flags);
				}
				else if (field.captionId.IsValid)
				{
					return string.Concat (field.id, ";", typeId, ";", rank, ";", field.captionId.ToString ());
				}
				else if (string.IsNullOrEmpty (rank))
				{
					return string.Concat (field.id, ";", typeId);
				}
				else
				{
					return string.Concat (field.id, ";", typeId, ";", rank);
				}
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				Support.ResourceManager manager = Serialization.Context.GetResourceManager (context);
				
				string[] args = value.Split (';');
				
				string        name      = args[0];
				Support.Druid typeId    = Support.Druid.Parse (args[1]);
				string        rank      = args.Length < 3 ? "-1" : string.IsNullOrEmpty (args[2]) ? "-1" : args[2];
				Support.Druid captionId = args.Length < 4 ? Support.Druid.Empty : Support.Druid.Parse (args[3]);
				string        flags     = args.Length < 5 ? "0" : args[4];
				string        expId     = args.Length < 6 ? "" : args[5];

				string expression = null;

				if (expId == "Exp")
				{
					expression = string.Join (";", args, 6, args.Length-6);
				}

				int rankValue  = System.Int32.Parse (rank, System.Globalization.CultureInfo.InvariantCulture);
				int flagsValue = System.Int32.Parse (flags, System.Globalization.CultureInfo.InvariantCulture);
				
				StructuredTypeField field = new StructuredTypeField (name, null, captionId, rankValue, flagsValue, expression);

				field.typeId = typeId;

				if (typeId.IsValid)
				{
					INamedType type = TypeRosetta.GetTypeObject (typeId);

					if (type == null)
					{
						//	If the type is not yet known to TypeRosetta, queue a request for
						//	a later fix-up instead of trying to resolve it here and now, as
						//	this could lead to endless loops :
						
						TypeRosetta.QueueFixUp (
							delegate ()
							{
								Caption caption = (manager ?? Support.Resources.DefaultManager).GetCaption (typeId);
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


		const int								RelationShift	= 0;
		const int								RelationMask	= 0x0f;
		const int								MembershipShift	= 4;
		const int								MembershipMask	= 0x03;
		const int								SourceShift		= 6;
		const int								SourceMask		= 0x0f;
		const int								OptionsShift	= 10;
		const int								OptionsMask		= 0x3f;
		
		private readonly string					id;
		private INamedType						type;
		private Support.Druid					typeId;
		private readonly Support.Druid			captionId;
		private int								rank;
		private Support.Druid					definingTypeId;		//	not serialized
		private readonly FieldRelation			relation;
		private readonly FieldMembership		membership;
		private readonly FieldSource			source;
		private readonly FieldOptions			options;
		private string							expression;
	}
}
