//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (StructuredType))]

namespace Epsitec.Common.Types
{

	/// <summary>
	/// The <c>StructuredType</c> class describes the type of the data stored in
	/// a <see cref="StructuredData"/> class.
	/// </summary>
	public class StructuredType : AbstractType, IStructuredType, Serialization.ISerialization, Serialization.IDeserialization
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredType"/> class,
		/// defaulting to <c>StructuredTypeClass.None</c>.
		/// </summary>
		public StructuredType()
			: base ("Structure")
		{
			this.fields = new Collections.HostedStructuredTypeFieldDictionary (this.NotifyFieldInsertion, this.NotifyFieldRemoval);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredType"/> class.
		/// </summary>
		/// <param name="class">The structured type class.</param>
		public StructuredType(StructuredTypeClass @class)
			: this ()
		{
			this.DefineName (null);
			
			if (@class != this.Class)
			{
				this.SetValue (StructuredType.ClassProperty, @class);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StructuredType"/> class.
		/// </summary>
		/// <param name="class">The structured type class.</param>
		/// <param name="baseTypeId">The structured type this instance extends.</param>
		public StructuredType(StructuredTypeClass @class, Druid baseTypeId)
			: this (@class)
		{
			if (baseTypeId.IsValid)
			{
				this.SetValue (StructuredType.BaseTypeIdProperty, baseTypeId);
			}

			//	An interface may not inherit from a base type; just
			//	make sure :

			if ((this.Class == StructuredTypeClass.Interface) &&
				(baseTypeId.IsValid))
			{
				throw new System.ArgumentException ("An interface may not inherit from a base type");
			}
		}

		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Structured;
			}
		}


		/// <summary>
		/// Gets the field definition dictionary. This instance is writable.
		/// </summary>
		/// <value>The fields.</value>
		public Collections.HostedStructuredTypeFieldDictionary Fields
		{
			get
			{
				this.IncludeInheritedFields ();
				return this.fields;
			}
		}

		/// <summary>
		/// Gets the structured type class (e.g. <c>Entity</c> or <c>View</c>).
		/// </summary>
		/// <value>The class for this structured type.</value>
		public StructuredTypeClass Class
		{
			get
			{
				return (StructuredTypeClass) this.GetValue (StructuredType.ClassProperty);
			}
		}

		/// <summary>
		/// Gets the structured type id extended by this instance (also known as
		/// its base type).
		/// </summary>
		/// <value>The structured type id this instance extends or <c>Druid.Empty</c>.</value>
		public Druid BaseTypeId
		{
			get
			{
				object value = this.GetValue (StructuredType.BaseTypeIdProperty);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					return Druid.Empty;
				}
				else
				{
					return (Druid) value;
				}
			}
		}

		public StructuredTypeFlags Flags
		{
			get
			{
				object value = this.GetValue (StructuredType.FlagsProperty);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(value == null))
				{
					return StructuredTypeFlags.None;
				}
				else
				{
					return (StructuredTypeFlags) value;
				}
			}
			set
			{
				if (value == StructuredTypeFlags.None)
				{
					this.ClearValue (StructuredType.FlagsProperty);
				}
				else
				{
					this.SetValue (StructuredType.FlagsProperty, value);
				}
			}
		}

		public DataLifetimeExpectancy DefaultLifetimeExpectancy
		{
			get
			{
				object value = this.GetValue (StructuredType.DefaultLifetimeExpectancyProperty);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(value == null))
				{
					return DataLifetimeExpectancy.Unknown;
				}
				else
				{
					return (DataLifetimeExpectancy) value;
				}
			}
			set
			{
				if (value == DataLifetimeExpectancy.Unknown)
				{
					this.ClearValue (StructuredType.DefaultLifetimeExpectancyProperty);
				}
				else
				{
					this.SetValue (StructuredType.DefaultLifetimeExpectancyProperty, value);
				}
			}
		}

		/// <summary>
		/// Gets the list of interface ids defined locally for this structured
		/// type. See <see cref="GetInterfaceIds(bool)"/> if you need to retrieve the
		/// complete list of interfaces associated with this structured type.
		/// </summary>
		/// <value>The interface ids.</value>
		public IList<Druid> InterfaceIds
		{
			get
			{
				if (this.interfaces == null)
				{
					this.interfaces = new Collections.HostedList<Druid> (this.HandleInterfaceInsertion, this.HandleInterfaceRemoval);
				}

				return this.interfaces;
			}
		}

		/// <summary>
		/// Gets the structured type extended by this instance (also known as
		/// its base type). This uses the <c>BaseTypeId</c> and an access through
		/// the resource manager to resolve the structured type.
		/// </summary>
		/// <value>The structured type this instance extends or <c>null</c>.</value>
		public StructuredType BaseType
		{
			get
			{
				return this.GetType (this.BaseTypeId, true);
			}
		}

		/// <summary>
		/// Gets or sets the serialized designer layouts.
		/// </summary>
		/// <value>The serialized designer layouts.</value>
		public string SerializedDesignerLayouts
		{
			get
			{
				return (string) this.GetValue (StructuredType.SerializedDesignerLayoutsProperty);
			}
			set
			{
				this.SetValue (StructuredType.SerializedDesignerLayoutsProperty, value);
			}
		}

		/// <summary>
		/// Gets a comparer which can be used to sort <see cref="StructuredTypeField"/>
		/// entries by their rank.
		/// </summary>
		/// <value>The rank comparer.</value>
		public static IComparer<StructuredTypeField> RankComparer
		{
			get
			{
				return new RankComparerImplementation ();
			}
		}

		/// <summary>
		/// Finds a field which has matching rank. If there are several fields
		/// with the same rank, which field will be returned is not defined.
		/// </summary>
		/// <param name="rank">The rank.</param>
		/// <param name="field">The field.</param>
		/// <returns><c>true</c> if a matching field was found; otherwise, <c>false</c>.</returns>
		public bool FindFieldByRank(int rank, out StructuredTypeField field)
		{
			foreach (StructuredTypeField item in this.fields.Values)
			{
				if (item.Rank == rank)
				{
					field = item;
					return true;
				}
			}

			field = null;
			return false;
		}

		/// <summary>
		/// Gets the list of interface ids. Every interface is represented by
		/// a <see cref="Druid"/> which maps to a <see cref="StructuredType"/>.
		/// An interface will appear at most once. Interfaces which are themselves
		/// based on interfaces will be handled as a collection of interfaces,
		/// meaning that they will all show up in the list, independently of
		/// the <c>inherit</c> parameter.
		/// </summary>
		/// <param name="inherit">
		/// If set to <c>true</c>, lists also the interfaces inherited through
		/// the base type.</param>
		/// <returns>
		/// The list of interfaces (or an empty list).
		/// </returns>
		public IList<Druid> GetInterfaceIds(bool inherit)
		{
			List<Druid> interfaces = new List<Druid> ();
			this.GetInterfaceIds (interfaces, inherit);
			return new Collections.ReadOnlyList<Druid> (interfaces);
		}

		public void FreezeInheritance()
		{
			this.fieldInheritance = InheritanceMode.Frozen;
			this.interfaceInheritance = InheritanceMode.Frozen;
		}

		#region IStructuredType Members

		/// <summary>
		/// Gets the field descriptor for the specified field identifier.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <returns>
		/// The matching field descriptor; otherwise, <c>null</c>.
		/// </returns>
		public StructuredTypeField GetField(string fieldId)
		{
			this.IncludeInheritedFields ();

			StructuredTypeField field;

			if (this.fields.TryGetValue (fieldId, out field))
			{
				return field;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a collection of field identifiers, sorted by rank and identifier.
		/// </summary>
		/// <returns>A collection of field identifiers.</returns>
		public IEnumerable<string> GetFieldIds()
		{
			this.IncludeInheritedFields ();

			StructuredTypeField[] fields = this.GetSortedFields ();
			
			foreach (StructuredTypeField field in fields)
			{
				yield return field.Id;
			}
		}

		/// <summary>
		/// Executes the specified action for each field. The fields are sorted
		/// before they are processed.
		/// </summary>
		/// <param name="action">The action for the <see cref="StructuredTypeField"/>.</param>
		public void ForEachField(System.Action<StructuredTypeField> action)
		{
			this.IncludeInheritedFields ();

			foreach (StructuredTypeField field in this.GetSortedFields ())
			{
				action (field);
			}
		}

		/// <summary>
		/// Finds the field which matches the predicate. The fields are sorted
		/// before they are processed.
		/// </summary>
		/// <param name="predicate">The predicate used to find the matching <see cref="StructuredTypeField"/>.</param>
		/// <returns>The first matching <see cref="StructuredTypeField"/> or <c>null</c>.</returns>
		public StructuredTypeField FindField(System.Predicate<StructuredTypeField> predicate)
		{
			this.IncludeInheritedFields ();
			
			foreach (StructuredTypeField field in this.GetSortedFields ())
			{
				if (predicate (field))
				{
					return field;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets an array containing the sorted fields.
		/// </summary>
		/// <returns></returns>
		private StructuredTypeField[] GetSortedFields()
		{
			StructuredTypeField[] fields = new StructuredTypeField[this.fields.Values.Count];
			this.fields.Values.CopyTo (fields, 0);
			System.Array.Sort (fields, StructuredType.RankComparer);
			return fields;
		}

		/// <summary>
		/// Gets the structured type class for this instance. The default is
		/// simply <c>Node</c>.
		/// </summary>
		/// <returns>The structured type class to which this instance belongs.</returns>
		StructuredTypeClass IStructuredType.GetClass()
		{
			return this.Class;
		}
		
		#endregion
		
		#region ISystemType Members

		/// <summary>
		/// Gets the system type described by this object. This is <c>null</c> for
		/// structured type objects which are not mapped directly to a native class.
		/// </summary>
		/// <value>The system type described by this object.</value>
		public override System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region ISerialization Members

		bool Serialization.ISerialization.NotifySerializationStarted(Serialization.Context context)
		{
			//	Remove any inherited fields from the fields collection, so we don't serialize
			//	inherited fields.

			System.Diagnostics.Debug.Assert (this.fieldInheritance != InheritanceMode.Disabled);
			System.Diagnostics.Debug.Assert (this.interfaceInheritance != InheritanceMode.Disabled);
			
			this.RemoveInheritedFields ();

			if (this.fieldInheritance != InheritanceMode.Frozen)
			{
				this.fieldInheritance = InheritanceMode.Disabled;
			}
			if (this.interfaceInheritance != InheritanceMode.Frozen)
			{
				this.interfaceInheritance = InheritanceMode.Disabled;
			}
			
			return true;
		}

		void Serialization.ISerialization.NotifySerializationCompleted(Serialization.Context context)
		{
			if (this.fieldInheritance != InheritanceMode.Frozen)
			{
				System.Diagnostics.Debug.Assert (this.fieldInheritance == InheritanceMode.Disabled);
				this.fieldInheritance = InheritanceMode.Undefined;
			}
			if (this.interfaceInheritance != InheritanceMode.Frozen)
			{
				System.Diagnostics.Debug.Assert (this.interfaceInheritance == InheritanceMode.Disabled);
				this.interfaceInheritance = InheritanceMode.Undefined;
			}
		}

		#endregion

		#region IDeserialization Members

		bool Serialization.IDeserialization.NotifyDeserializationStarted(Serialization.Context context)
		{
			System.Diagnostics.Debug.Assert (this.fieldInheritance == InheritanceMode.Undefined);
			System.Diagnostics.Debug.Assert (this.interfaceInheritance == InheritanceMode.Undefined);

			this.fieldInheritance = InheritanceMode.Disabled;
			this.interfaceInheritance = InheritanceMode.Disabled;
			
			return true;
		}

		void Serialization.IDeserialization.NotifyDeserializationCompleted(Serialization.Context context)
		{
			System.Diagnostics.Debug.Assert (this.fieldInheritance == InheritanceMode.Disabled);
			System.Diagnostics.Debug.Assert (this.interfaceInheritance == InheritanceMode.Disabled);

			this.fieldInheritance = InheritanceMode.Undefined;
			this.interfaceInheritance = InheritanceMode.Undefined;
		}

		#endregion
		
		#region RankComparerImplementation Class
		
		private class RankComparerImplementation : IComparer<StructuredTypeField>
		{
			#region IComparer Members

			public int Compare(StructuredTypeField valX, StructuredTypeField valY)
			{
				if (valX.Membership != valY.Membership)
				{
					if ((valX.Membership == FieldMembership.Inherited) &&
						(valY.Membership != FieldMembership.Inherited))
					{
						return -1;
					}
					else
					{
						return 1;
					}
				}
				if (valX.DefiningTypeId.IsEmpty != valY.DefiningTypeId.IsEmpty)
				{
					if (valX.DefiningTypeId.IsEmpty)
					{
						return 1;
					}
					else
					{
						return -1;
					}
				}

				int rx = valX.Rank;
				int ry = valY.Rank;

				if (rx < ry)
				{
					return -1;
				}
				if (rx > ry)
				{
					return 1;
				}

				return string.CompareOrdinal (valX.Id, valY.Id);
			}
			
			#endregion
		}
		
		#endregion

		/// <summary>
		/// Determines whether the specified field name is valid or not.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns>
		/// 	<c>true</c> if the specified field name is valid; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsValidFieldName(string fieldName)
		{
			if ((string.IsNullOrEmpty (fieldName)) ||
				(!RegexFactory.PascalCaseSymbol.IsMatch (fieldName)) ||
				((fieldName.Length > 1) && (fieldName == fieldName.ToUpper ())))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Checks if the two types are equal.
		/// </summary>
		/// <param name="a">The first structured type.</param>
		/// <param name="b">The second structured type.</param>
		/// <returns><c>true</c> if both types are equal; otherwise, <c>false</c>.</returns>
		public static bool EqualTypes(IStructuredType a, IStructuredType b)
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			StructuredType typeA = a as StructuredType;
			StructuredType typeB = b as StructuredType;

			if ((typeA != null) &&
				(typeB != null))
			{
				if ((typeA.BaseTypeId != typeB.BaseTypeId) ||
					(typeA.Class != typeB.Class))
				{
					return false;
				}

				if (!Collection.CompareEqual (typeA.InterfaceIds, typeB.InterfaceIds))
				{
					return false;
				}
			}

			if (!Collection.CompareEqual (a.GetFieldIds (), b.GetFieldIds ()))
			{
				return false;
			}

			//	TODO: should really make sure by comparing the field types ?
			
			return true;
		}

		/// <summary>
		/// Merges two structured types and creates a new one.
		/// </summary>
		/// <param name="a">The first structured type.</param>
		/// <param name="b">The second structured type.</param>
		/// <returns>The merged structured type.</returns>
		public static StructuredType Merge(StructuredType a, StructuredType b)
		{
			if (a.Module.Layer > b.Module.Layer)
			{
				StructuredType temp = a;
				
				a = b;
				b = temp;
			}

			System.Diagnostics.Debug.Assert (a.Module.Layer <= b.Module.Layer);

			if (a.Class != b.Class)
			{
				throw new System.ArgumentException (string.Format ("Cannot merge StructuredType of Class {0} and {1}", a.Class, b.Class));
			}
			
			if (a.BaseTypeId != b.BaseTypeId)
			{
				throw new System.ArgumentException ("Cannot merge StructuredType with different base types");
			}

			StructuredType merge = new StructuredType (b.Class, b.BaseTypeId);
			
			if (((bool)a.GetValue (StructuredType.DebugDisableChecksProperty)) ||
				((bool)b.GetValue (StructuredType.DebugDisableChecksProperty)))
			{
				merge.SetValue (StructuredType.DebugDisableChecksProperty, true);
			}

			Caption caption = Caption.Merge (a.Caption, b.Caption);

			caption.ClearValue (AbstractType.ComplexTypeProperty);
			merge.DefineCaption (caption);

			int rank = 0;
			
			foreach (string id in a.GetFieldIds ())
			{
				StructuredTypeField field = a.GetField (id);

				switch (field.Membership)
				{
					case FieldMembership.Local:
					case FieldMembership.LocalOverride:
						merge.fields.Add (new StructuredTypeField (id, field.Type, field.CaptionId, rank++, field.Relation));
						break;
				}
			}

			foreach (string id in b.GetFieldIds ())
			{
				StructuredTypeField field = b.GetField (id);

				switch (field.Membership)
				{
					case FieldMembership.Local:
					case FieldMembership.LocalOverride:
						if (merge.fields.ContainsKey (id))
						{
							//	There is a collision between two fields; the second structured type (b)
							//	wins over (a); this can be used for a user layer to override something
							//	defined in the application layer.

							int fieldRank = merge.fields[id].Rank;

							merge.fields[id] = new StructuredTypeField (id, field.Type, field.CaptionId, fieldRank, field.Relation, FieldMembership.Local, field.Source, field.Options, field.Expression);
						}
						else
						{
							merge.fields.Add (new StructuredTypeField (id, field.Type, field.CaptionId, rank++, field.Relation, FieldMembership.Local, field.Source, field.Options, field.Expression));
						}
						break;
				}
			}

			System.Diagnostics.Debug.Assert (merge.fieldInheritance == InheritanceMode.Undefined);
			System.Diagnostics.Debug.Assert (merge.interfaceInheritance == InheritanceMode.Undefined);

			//	Make the merged structure type belong to the same bundle/module
			//	as the lower layer source structured type :
			
			ResourceManager.SetSourceBundle (merge.Caption, ResourceManager.GetSourceBundle (a.Caption));

			return merge;
		}

		/// <summary>
		/// Check if two structured types share compatible fields, i.e. if all
		/// fields from the source can be safely copied to the target.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <returns><c>true</c> if source and target have compatible fields;
		/// otherwise, <c>false</c>.</returns>
		public static bool HaveCompatibleFields(IStructuredType source, IStructuredType target)
		{
			if (source == target)
			{
				return true;
			}

			if ((source == null) ||
				(target == null))
			{
				return false;
			}

			foreach (string fieldId in source.GetFieldIds ())
			{
				StructuredTypeField sourceField = source.GetField (fieldId);
				StructuredTypeField targetField = target.GetField (fieldId);

				if ((targetField == null) ||
					(sourceField.Relation != targetField.Relation))
				{
					return false;
				}

				//	TODO: compare field types...
			}

			return true;
		}

		/// <summary>
		/// Determines whether the specified value is valid according to the
		/// constraint.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			IStructuredTypeProvider provider = value as IStructuredTypeProvider;

			if (provider == null)
			{
				return false;
			}
			else
			{
				IStructuredType otherType = provider.GetStructuredType ();
				
				if (StructuredType.EqualTypes (this, otherType))
				{
					return true;
				}

				StructuredType type = otherType as StructuredType;

				if (type != null)
				{
					type = type.BaseType;

					while (type != null)
					{
						if (StructuredType.EqualTypes (this, type))
						{
							return true;
						}

						type = type.BaseType;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Refreshes the list of inherited fields. Call this method if there
		/// have been changes to one of the base types.
		/// </summary>
		public void RefreshInheritedFields()
		{
			this.RemoveInheritedFields ();
			this.IncludeInheritedFields ();
		}

		#region Private and Protected Methods
		
		protected override void OnCaptionDefined()
		{
			base.OnCaptionDefined ();

			Caption caption = this.Caption;

			if (caption != null)
			{
				AbstractType.SetComplexType (caption, this);
			}
		}

		/// <summary>
		/// Includes the inherited fields; this is done automatically whenever
		/// fields are accessed through the public methods and properties.
		/// </summary>
		private void IncludeInheritedFields()
		{
			if (this.fieldInheritance == InheritanceMode.Undefined)
			{
				this.IncludeInheritedFields (this.BaseTypeId, FieldMembership.Inherited);

				foreach (Druid interfaceId in this.GetInterfaceIds (false))
				{
					this.IncludeInheritedFields (interfaceId, FieldMembership.Local);
				}

				StructuredTypeField[] fields = this.GetSortedFields ();

				for (int i = 0; i < fields.Length; i++)
				{
					if (fields[i].Rank != -1)
					{
						fields[i].ResetRank (i);
					}
				}
				
				this.fieldInheritance = InheritanceMode.Defined;
			}
		}

		private void IncludeInheritedFields(Druid typeId, FieldMembership membership)
		{
			System.Diagnostics.Debug.Assert (membership != FieldMembership.LocalOverride);

			StructuredType type = this.GetType (typeId, false);

			if (type != null)
			{
				foreach (string id in type.GetFieldIds ())
				{
					if (this.fields.ContainsKey (id))
					{
						StructuredTypeField local = this.fields[id];
						StructuredTypeField model = type.Fields[id];
						StructuredTypeField clone = null;

						if (membership == FieldMembership.Local)
						{
							FieldMembership localMembership;
							FieldSource source;
							string expression;

							if (local.Expression != null)
							{
								source          = local.Source;
								expression      = local.Expression;
								localMembership = FieldMembership.LocalOverride;
							}
							else
							{
								source          = model.Source;
								expression      = model.Expression;
								localMembership = FieldMembership.Local;
							}

							clone = new StructuredTypeField (model.Id, model.Type, model.CaptionId, model.Rank, model.Relation, localMembership, source, model.Options, expression);
						}
						else if ((membership == FieldMembership.Inherited) &&
								 (local.Expression != null) &&
								 (model.Expression != null))
						{
							clone = new StructuredTypeField (model.Id, model.Type, model.CaptionId, model.Rank, model.Relation, FieldMembership.LocalOverride, local.Source, model.Options, local.Expression);
						}

						if (clone == null)
						{
							throw new System.NotImplementedException (string.Format ("Unhandled field inheritance override for field {0} in type {1}", id, typeId));
						}
						else
						{
							clone.DefineDefiningTypeId (typeId);
							this.fields[id] = clone;
						}
					}
					else
					{
						StructuredTypeField clone = type.Fields[id].Clone (membership, typeId);
						this.fields.Add (id, clone);
					}
				}
			}
		}

		/// <summary>
		/// Removes the inherited fields; this is used to clean up the fields
		/// collection to keep only the locally defined fields.
		/// </summary>
		private void RemoveInheritedFields()
		{
			if (this.fieldInheritance == InheritanceMode.Defined)
			{
				List<string> ids = new List<string> ();

				foreach (KeyValuePair<string, StructuredTypeField> fieldEntry in this.fields)
				{
					if (fieldEntry.Value.Membership == FieldMembership.Inherited)
					{
						ids.Add (fieldEntry.Key);
					}
				}

				foreach (string id in ids)
				{
					this.fields.Remove (id);
				}

				this.fieldInheritance = InheritanceMode.Undefined;
			}
		}

		private void HandleInterfaceInsertion(Druid interfaceId)
		{
			this.RemoveInheritedFields ();
		}

		private void HandleInterfaceRemoval(Druid interfaceId)
		{
			this.RemoveInheritedFields ();
		}
		
		private void GetInterfaceIds(IList<Druid> interfaces, bool inherit)
		{
			System.Diagnostics.Debug.Assert (interfaces != null);

			//	If the caller is interested in the inherited interfaces, we will
			//	first walk through the base type's interface id list :
			
			if (inherit)
			{
				StructuredType baseType = this.GetType (this.BaseTypeId, false);

				if (baseType != null)
				{
					System.Diagnostics.Debug.Assert (baseType.Class == StructuredTypeClass.Entity);
					
					baseType.GetInterfaceIds (interfaces, inherit);
				}
			}

			//	Now, for every interface not yet in our list, recursively add
			//	it and its own interfaces :

			foreach (Druid interfaceId in this.InterfaceIds)
			{
				if (interfaces.Contains (interfaceId))
				{
					continue;
				}

				StructuredType interfaceType = this.GetType (interfaceId, false);

				System.Diagnostics.Debug.Assert (interfaceType != null);
				System.Diagnostics.Debug.Assert (interfaceType.Class == StructuredTypeClass.Interface);
				System.Diagnostics.Debug.Assert (interfaceType.BaseTypeId.IsEmpty);

				interfaces.Add (interfaceId);
				interfaceType.GetInterfaceIds (interfaces, inherit);
			}
		}

		/// <summary>
		/// Gets the structured type associated with a type id. This will try to
		/// locate the type based on the same resource manager as the one which
		/// was used to instanciate this instance.
		/// </summary>
		/// <param name="typeId">The type DRUID.</param>
		/// <param name="useTypeCache">If set to <c>true</c>, uses a cache.</param>
		/// <returns>The structured type or <c>null</c>.</returns>
		private StructuredType GetType(Druid typeId, bool useTypeCache)
		{
			if (typeId.IsValid)
			{
				ResourceManager manager = this.FindAssociatedResourceManager ();
				Caption caption = manager.GetCaption (typeId);

				System.Diagnostics.Debug.Assert (manager.BasedOnPatchModule == false);

				if (caption != null)
				{
					if (useTypeCache)
					{
						return TypeRosetta.GetTypeObject (caption) as StructuredType;
					}
					else
					{
						return TypeRosetta.CreateTypeObject (caption, false) as StructuredType;
					}
				}
			}

			return null;
		}

		private void NotifyFieldInsertion(string name, StructuredTypeField field)
		{
			StructuredTypeClass typeClass = this.Class;

#if false
			if (typeClass == StructuredTypeClass.View)
			{
				//	A structured type used to represent a view may only contain fields
				//	using an inclusion relation with the same target (structured) type.

				if (field.Relation != FieldRelation.Inclusion)
				{
					throw new System.ArgumentException (string.Format ("View may not use field with {0} relation", field.Relation));
				}

				if (field.TypeId.IsValid)
				{
					foreach (StructuredTypeField item in this.fields.Values)
					{
						if (item.TypeId.IsValid)
						{
							if (field.TypeId != item.TypeId)
							{
								throw new System.ArgumentException (string.Format ("View may not use mismatched fields ({0} and {1})", item.Id, field.Id));
							}
						}
					}
				}
			}
#endif

			if ((typeClass == StructuredTypeClass.Entity) ||
				(typeClass == StructuredTypeClass.Interface))
			{
				if ((this.IsCaptionDefined) &&
					(this.fieldInheritance != InheritanceMode.Frozen) &&
					(! (bool) this.GetValue (StructuredType.DebugDisableChecksProperty)))
				{
					//	Ensure that the field ID matches the field's caption ID. This is
					//	a strict requirement for entities and views.

					ResourceManager manager = this.FindAssociatedResourceManager ();
					Caption         caption = manager.GetCaption (field.CaptionId);

					if ((caption == null) ||
						(string.IsNullOrEmpty (caption.Name)))
					{
						throw new System.ArgumentException (string.Format ("Invalid caption specified for field in {0} {1}", typeClass.ToString ().ToLower (), this.Name));
					}

					if (!StructuredType.IsValidFieldName (caption.Name))
					{
						throw new System.ArgumentException (string.Format ("Field name {0} invalid in {1} {2}", caption.Name, typeClass.ToString ().ToLower (), this.Name));
					}

					if (field.Id != field.CaptionId.ToString ())
					{
						throw new System.ArgumentException (string.Format ("Field {0} must specify {1} as ID in {2} {3}", caption.Name, field.CaptionId.ToString (), typeClass.ToString ().ToLower (), this.Name));
					}
				}
				
#if false
				if ((field.TypeId.IsValid) &&
					(field.Relation == FieldRelation.Inclusion))
				{
					if (field.Type != null)
					{
						StructuredType sourceType = field.Type as StructuredType;

						if (sourceType == null)
						{
							throw new System.ArgumentException (string.Format ("Field inclusion not possible ({0}); invalid source type", field.Id));
						}
						if (sourceType.Class != StructuredTypeClass.Interface)
						{
							throw new System.ArgumentException (string.Format ("Field inclusion not possible ({0}); invalid source type {1}", field.Id, sourceType.Class));
						}
					}
				}
#endif
			}
		}

		private ResourceManager FindAssociatedResourceManager()
		{
			ResourceBundle  bundle  = ResourceManager.GetSourceBundle (this.Caption);
			ResourceManager manager = bundle == null
						? Serialization.Context.GetResourceManager (Storage.CurrentDeserializationContext)
						: bundle.ResourceManager;

			if (manager == null)
			{
				manager = ResourceManager.GetResourceManager (this) ?? Support.Resources.DefaultManager;

				System.Diagnostics.Debug.Assert (manager != null);
				System.Diagnostics.Debug.Assert (manager.BasedOnPatchModule == false);
			}
			
			return manager;
		}

		private void NotifyFieldRemoval(string name, StructuredTypeField field)
		{
		}

		#endregion

		#region Static Dependency Property Support Methods

		private static object GetFieldsValue(DependencyObject obj)
		{
			//	The fields value is not serializable in its native HostedDictionary
			//	form, so we wrap it into a synthetic (and temporary) collection which
			//	is only used when serializing and deserializing.
			
			StructuredType that = obj as StructuredType;
			Serialization.Context context = Serialization.Context.GetActiveContext ();

			System.Diagnostics.Debug.Assert (context != null);

			Collections.StructuredTypeFieldCollection data = context.GetEntry (that) as Collections.StructuredTypeFieldCollection;
			
			if (data == null)
			{
				data = new Collections.StructuredTypeFieldCollection (that);
				context.SetEntry (that, data);
			}
			
			return data;
		}
		
		private static object GetInterfaceIdsValue(DependencyObject o)
		{
			StructuredType that = (StructuredType) o;
			return that.InterfaceIds;
		}

		#endregion

		#region InheritanceMode Enumeration

		private enum InheritanceMode : byte
		{
			Undefined,
			Defined,
			Disabled,
			Frozen
		}

		#endregion

		public static readonly DependencyProperty DebugDisableChecksProperty		= DependencyProperty.Register ("DebugDisableChecks", typeof (bool), typeof (StructuredType), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty FieldsProperty					= DependencyProperty.RegisterReadOnly ("Fields", typeof (Collections.StructuredTypeFieldCollection), typeof (StructuredType), new DependencyPropertyMetadata (StructuredType.GetFieldsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty ClassProperty						= DependencyProperty.RegisterReadOnly ("Class", typeof (StructuredTypeClass), typeof (StructuredType), new DependencyPropertyMetadata (StructuredTypeClass.None).MakeReadOnlySerializable ());
		public static readonly DependencyProperty BaseTypeIdProperty				= DependencyProperty.Register ("BaseTypeId", typeof (Druid), typeof (StructuredType), new DependencyPropertyMetadata (Druid.Empty).MakeReadOnlySerializable ());
		public static readonly DependencyProperty InterfaceIdsProperty				= DependencyProperty.RegisterReadOnly ("InterfaceIds", typeof (ICollection<Druid>), typeof (StructuredType), new DependencyPropertyMetadata (StructuredType.GetInterfaceIdsValue).MakeReadOnlySerializable ());

		public static readonly DependencyProperty FlagsProperty						= DependencyProperty<StructuredType>.Register<StructuredTypeFlags> (x => x.Flags);
		public static readonly DependencyProperty DefaultLifetimeExpectancyProperty = DependencyProperty<StructuredType>.Register<DataLifetimeExpectancy> (x => x.DefaultLifetimeExpectancy);
		public static readonly DependencyProperty SerializedDesignerLayoutsProperty = DependencyProperty<StructuredType>.Register<string> (x => x.SerializedDesignerLayouts);

		private readonly Collections.HostedStructuredTypeFieldDictionary fields;
		
		private InheritanceMode fieldInheritance;
		private InheritanceMode interfaceInheritance;
		private Collections.HostedList<Druid> interfaces;
	}
}
