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
			this.pendingFields = new List<StructuredTypeField> ();
		}

		public StructuredTypeFieldCollection(StructuredType owner)
		{
			this.Owner = owner;
		}

		#region ICollection<DependencyObject> Members

		void ICollection<DependencyObject>.Add(DependencyObject item)
		{
			StructuredTypeField field = item as StructuredTypeField;

			if (this.Owner == null)
			{
				this.pendingFields.Add (field);
			}
			else
			{
				this.Owner.AddField (field.Name, field.Type);
			}
		}

		void ICollection<DependencyObject>.Clear()
		{
			if (this.Owner != null)
			{
				this.Owner.Fields.Clear ();
			}
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
				if (this.Owner == null)
				{
					return 0;
				}
				else
				{
					return this.Owner.Fields.Count;
				}
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
			if (this.Owner != null)
			{
				foreach (KeyValuePair<string, INamedType> item in this.Owner.Fields)
				{
					yield return new StructuredTypeField (item.Key, item.Value);
				}
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

		private List<StructuredTypeField> pendingFields;

		public static void HandleOwnerChanged(DependencyObject obj, object oldValue, object newValue)
		{
			StructuredTypeFieldCollection that = obj as StructuredTypeFieldCollection;

			if (that.Owner == null)
			{
				that.pendingFields = new List<StructuredTypeField> ();
			}
			else
			{
				if (that.pendingFields != null)
				{
					foreach (StructuredTypeField field in that.pendingFields)
					{
						that.Owner.AddField (field.Name, field.Type);
					}

					that.pendingFields = null;
				}
			}
		}

		public static DependencyProperty OwnerProperty = DependencyProperty.Register ("Owner", typeof (StructuredType), typeof (StructuredTypeFieldCollection), new DependencyPropertyMetadata (StructuredTypeFieldCollection.HandleOwnerChanged));
	}
}
