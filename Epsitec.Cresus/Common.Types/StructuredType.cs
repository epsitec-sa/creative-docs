//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.StructuredType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredData"/> class.
	/// </summary>
	public class StructuredType : AbstractType, IStructuredType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:StructuredType"/> class.
		/// </summary>
		public StructuredType() : base ("Structure")
		{
			this.fields = new Collections.HostedStructuredTypeFieldDictionary (this.NotifyFieldInserted, this.NotifyFieldRemoved);
		}

		/// <summary>
		/// Gets the field definition dictionary. This instance is writable.
		/// </summary>
		/// <value>The fields.</value>
		public Collections.HostedStructuredTypeFieldDictionary Fields
		{
			get
			{
				return this.fields;
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

			field = StructuredTypeField.Empty;
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
			StructuredTypeField field;

			if (this.fields.TryGetValue (fieldId, out field))
			{
				return field;
			}
			else
			{
				return StructuredTypeField.Empty;
			}
		}

		/// <summary>
		/// Gets a collection of field identifiers, sorted by rank and identifier.
		/// </summary>
		/// <returns>A collection of field identifiers.</returns>
		public IEnumerable<string> GetFieldIds()
		{
			StructuredTypeField[] fields = new StructuredTypeField[this.fields.Values.Count];
			
			this.fields.Values.CopyTo (fields, 0);

			System.Array.Sort (fields, StructuredType.RankComparer);
			
			foreach (StructuredTypeField field in fields)
			{
				yield return field.Id;
			}
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

		#region RankComparerImplementation Class
		
		private class RankComparerImplementation : IComparer<StructuredTypeField>
		{
			#region IComparer Members

			public int Compare(StructuredTypeField valX, StructuredTypeField valY)
			{
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

		protected override void OnCaptionDefined()
		{
			base.OnCaptionDefined ();


			Caption caption = this.Caption;

			if (caption != null)
			{
				AbstractType.SetComplexType (caption, this);
			}
		}
		
		private void NotifyFieldInserted(string name, StructuredTypeField field)
		{
		}

		private void NotifyFieldRemoved(string name, StructuredTypeField field)
		{
		}

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

		public static DependencyProperty FieldsProperty = DependencyProperty.RegisterReadOnly ("Fields", typeof (Collections.StructuredTypeFieldCollection), typeof (StructuredType), new DependencyPropertyMetadata (StructuredType.GetFieldsValue).MakeReadOnlySerializable ());

		private Collections.HostedStructuredTypeFieldDictionary fields;
	}
}
