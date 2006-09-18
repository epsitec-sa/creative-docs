//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.StructuredTypeFieldCollection))]

namespace Epsitec.Common.Types
{
	public class StructuredTypeFieldCollection : DependencyObject, ICollection<DependencyObject>
	{
		public StructuredTypeFieldCollection()
		{
		}

		public StructuredTypeFieldCollection(StructuredType owner)
		{
			this.Owner = owner;
		}

		#region ICollection<DependencyObject> Members

		void ICollection<DependencyObject>.Add(DependencyObject item)
		{
			StructuredTypeField field = item as StructuredTypeField;

			this.Owner.AddField (field.Name, field.Type);
		}

		void ICollection<DependencyObject>.Clear()
		{
			this.Owner.Fields.Clear ();
		}

		bool ICollection<DependencyObject>.Contains(DependencyObject item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void ICollection<DependencyObject>.CopyTo(DependencyObject[] array, int arrayIndex)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		int ICollection<DependencyObject>.Count
		{
			get
			{
				return this.Owner.Fields.Count;
			}
		}

		bool ICollection<DependencyObject>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<DependencyObject>.Remove(DependencyObject item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<DependencyObject> Members

		IEnumerator<DependencyObject> IEnumerable<DependencyObject>.GetEnumerator()
		{
			foreach (KeyValuePair<string, INamedType> item in this.Owner.Fields)
			{
				yield return new StructuredTypeField (item.Key, item.Value);
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		private StructuredType Owner
		{
			get
			{
				return (StructuredType) this.GetValue (StructuredTypeFieldCollection.OwnerProperty);
			}
			set
			{
				this.SetValue (StructuredTypeFieldCollection.OwnerProperty, value);
			}
		}

		public static DependencyProperty OwnerProperty = DependencyProperty.Register ("Owner", typeof (StructuredType), typeof (StructuredTypeFieldCollection));
	}
}
