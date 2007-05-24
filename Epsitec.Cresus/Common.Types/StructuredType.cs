//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
		public StructuredType(StructuredTypeClass @class, Support.Druid baseTypeId)
			: this (@class)
		{
			if (baseTypeId.IsValid)
			{
				this.SetValue (StructuredType.BaseTypeIdProperty, baseTypeId);
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
		public Support.Druid BaseTypeId
		{
			get
			{
				object value = this.GetValue (StructuredType.BaseTypeIdProperty);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					return Support.Druid.Empty;
				}
				else
				{
					return (Support.Druid) value;
				}
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
				Support.Druid baseTypeId = this.BaseTypeId;

				if (baseTypeId.IsValid)
				{
					Support.ResourceManager manager = this.FindAssociatedResourceManager ();
					Caption                 caption = manager.GetCaption (baseTypeId);

					if (caption != null)
					{
						return TypeRosetta.GetTypeObject (caption) as StructuredType;
					}
				}

				return null;
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

			System.Diagnostics.Debug.Assert (this.fieldInheritance != FieldInheritance.Disabled);
			
			this.RemoveInheritedFields ();
			this.fieldInheritance = FieldInheritance.Disabled;
			
			return true;
		}

		void Serialization.ISerialization.NotifySerializationCompleted(Serialization.Context context)
		{
			System.Diagnostics.Debug.Assert (this.fieldInheritance == FieldInheritance.Disabled);
			
			this.fieldInheritance = FieldInheritance.Undefined;
		}

		#endregion


		#region IDeserialization Members

		bool Serialization.IDeserialization.NotifyDeserializationStarted(Serialization.Context context)
		{
			System.Diagnostics.Debug.Assert (this.fieldInheritance == FieldInheritance.Undefined);
			this.fieldInheritance = FieldInheritance.Disabled;
			return true;
		}

		void Serialization.IDeserialization.NotifyDeserializationCompleted(Serialization.Context context)
		{
			System.Diagnostics.Debug.Assert (this.fieldInheritance == FieldInheritance.Disabled);
			this.fieldInheritance = FieldInheritance.Undefined;
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
		/// Merges two structured types and creates a new one.
		/// </summary>
		/// <param name="a">The first structured type.</param>
		/// <param name="b">The second structured type.</param>
		/// <returns>The merged structured type.</returns>
		public static StructuredType Merge(StructuredType a, StructuredType b)
		{
			// modOK001 complété l'implémentation, par moment au pifomètre. Toutes les modifications y relatives
			// désignées par modOK001. Remplacé TO_DO (sans underscore) par OKDONE?
			// throw new System.NotImplementedException ();
			
			//	OKDONE? : implement structured type merge; swap a and b if needed, based on
			//	their layer depth (a should belong to the lower level layer)

			if (a.Module.Layer > b.Module.Layer)
			{
				StructuredType c;
			
				c = a; // il doit y avoir plus élégant, non?
				a = b;
				b = c;
			}

			System.Diagnostics.Debug.Assert (a.Module.Layer <= b.Module.Layer);

			// est-ce qu'ici, par prudence, il faut tester si c'est null? Il y a une valeur par défaut (.None), mais au cas où ?
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

			// J'ai dû mettre ici, si on met après avoir peuplé les fields, il refuse de redéfinir la caption

			Caption caption = Caption.Merge (a.Caption, b.Caption);

			caption.ClearValue (AbstractType.ComplexTypeProperty);
			merge.DefineCaption (caption);

			int rank = 0;
			
			foreach (string id in a.GetFieldIds ())
			{
				StructuredTypeField field = a.GetField (id);

				if (field.Membership == FieldMembership.Local)
				{
					merge.fields.Add (new StructuredTypeField (id, field.Type, field.CaptionId, rank++,
															   field.Relation, field.SourceFieldId));
				}
			}

			foreach (string id in b.GetFieldIds ())
			{
				StructuredTypeField field = b.GetField (id);

				if (field.Membership == FieldMembership.Local)
				{
					if (merge.fields.ContainsKey (id))
					{
						merge.fields[id] = new StructuredTypeField (id, field.Type, field.CaptionId, merge.fields[id].Rank, field.Relation, field.SourceFieldId);
					}
					else
					{
						merge.fields.Add (new StructuredTypeField (id, field.Type, field.CaptionId, rank++,
																   field.Relation, field.SourceFieldId));
					}
				}
			}

			System.Diagnostics.Debug.Assert (merge.fieldInheritance == FieldInheritance.Undefined);

			//	Make the merged structure type belong to the same bundle/module
			//	as the lower layer source structured type :
			
			Support.ResourceManager.SetSourceBundle (merge, Support.ResourceManager.GetSourceBundle (a));

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

			StructuredData data = value as StructuredData;

			return (data != null) && (data.StructuredType == this);
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

		/// <summary>
		/// Includes the inherited fields; this is done automatically whenever
		/// fields are accessed through the public methods and properties.
		/// </summary>
		protected virtual void IncludeInheritedFields()
		{
			if (this.fieldInheritance == FieldInheritance.Undefined)
			{
				StructuredType baseType = this.BaseType;
				
				if (baseType != null)
				{
					foreach (string id in baseType.GetFieldIds ())
					{
						this.fields.Add (id, new StructuredTypeField (baseType.Fields[id], FieldMembership.Inherited));
					}

					StructuredTypeField[] fields = this.GetSortedFields ();

					for (int i = 0; i < fields.Length; i++)
					{
						if (fields[i].Rank != -1)
						{
							fields[i].ResetRank (i);
						}
					}
					
					this.fieldInheritance = FieldInheritance.Defined;
				}
			}
		}

		/// <summary>
		/// Removes the inherited fields; this is used to clean up the fields
		/// collection to keep only the locally defined fields.
		/// </summary>
		protected virtual void RemoveInheritedFields()
		{
			if (this.fieldInheritance == FieldInheritance.Defined)
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
				
				this.fieldInheritance = FieldInheritance.Undefined;
			}
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
		
		private void NotifyFieldInsertion(string name, StructuredTypeField field)
		{
			StructuredTypeClass typeClass = this.Class;

			if (typeClass == StructuredTypeClass.View)
			{
				//	A structured type used to represent a view may only contain fields
				//	using an inclusion relation with the same target (structured) type.

				if (field.Relation != Relation.Inclusion)
				{
					throw new System.ArgumentException (string.Format ("View may not use field with {0} relation", field.Relation));
				}

				if (field.Type != null)
				{
					foreach (StructuredTypeField item in this.fields.Values)
					{
						if (item.Type != null)
						{
							if (field.Type != item.Type)
							{
								throw new System.ArgumentException (string.Format ("View may not use mismatched fields ({0} and {1})", item.Id, field.Id));
							}
						}
					}
				}
			}

			if ((typeClass == StructuredTypeClass.Entity) ||
				(typeClass == StructuredTypeClass.View))
			{
				if ((this.IsCaptionDefined) &&
					(! (bool) this.GetValue (StructuredType.DebugDisableChecksProperty)))
				{
					//	Ensure that the field ID matches the field's caption ID. This is
					//	a strict requirement for entities and views.

					Support.ResourceManager manager = this.FindAssociatedResourceManager ();
					Caption                 caption = manager.GetCaption (field.CaptionId);

					if ((caption == null) ||
						(string.IsNullOrEmpty (caption.Name)))
					{
						throw new System.ArgumentException (string.Format ("Invalid caption specified for field in {0} {1}", typeClass.ToString ().ToLower (), this.Name));
					}

					if ((!Support.RegexFactory.PascalCaseSymbol.IsMatch (caption.Name)) ||
						((caption.Name.Length > 1) && (caption.Name == caption.Name.ToUpper ())))
					{
						throw new System.ArgumentException (string.Format ("Field name {0} invalid in {1} {2}", caption.Name, typeClass.ToString ().ToLower (), this.Name));
					}

					if (field.Id != field.CaptionId.ToString ())
					{
						throw new System.ArgumentException (string.Format ("Field {0} must specify {1} as ID in {2} {3}", caption.Name, field.CaptionId.ToString (), typeClass.ToString ().ToLower (), this.Name));
					}
				}
			}
		}

		private Support.ResourceManager FindAssociatedResourceManager()
		{
			Support.ResourceBundle  bundle  = Support.ResourceManager.GetSourceBundle (this.Caption);
			Support.ResourceManager manager = bundle == null
						? (Serialization.Context.GetResourceManager (Storage.CurrentDeserializationContext) ?? Support.Resources.DefaultManager)
						: bundle.ResourceManager;
			
			return manager;
		}

		private void NotifyFieldRemoval(string name, StructuredTypeField field)
		{
		}

		#endregion

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

		#region FieldInheritance Enumeration

		private enum FieldInheritance
		{
			Undefined,
			Defined,
			Disabled
		}

		#endregion

		public static readonly DependencyProperty DebugDisableChecksProperty = DependencyProperty.Register ("DebugDisableChecks", typeof (bool), typeof (StructuredType), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty FieldsProperty = DependencyProperty.RegisterReadOnly ("Fields", typeof (Collections.StructuredTypeFieldCollection), typeof (StructuredType), new DependencyPropertyMetadata (StructuredType.GetFieldsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty ClassProperty = DependencyProperty.RegisterReadOnly ("Class", typeof (StructuredTypeClass), typeof (StructuredType), new DependencyPropertyMetadata (StructuredTypeClass.None).MakeReadOnlySerializable ());
		public static readonly DependencyProperty BaseTypeIdProperty = DependencyProperty.RegisterReadOnly ("BaseTypeId", typeof (Support.Druid), typeof (StructuredType), new DependencyPropertyMetadata (Support.Druid.Empty).MakeReadOnlySerializable ());

		private Collections.HostedStructuredTypeFieldDictionary fields;
		private FieldInheritance fieldInheritance;
	}
}
