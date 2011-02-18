//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>StructuredTypeFieldCollection</c> is used only when serializing the
	/// <c>StructuredType</c> contents.
	/// </summary>
	internal class StructuredTypeFieldCollection : ICollection<StructuredTypeField>
	{
		public StructuredTypeFieldCollection(StructuredType owner)
		{
			this.owner = owner;
		}

		#region ICollection<StructuredTypeField> Members

		void ICollection<StructuredTypeField>.Add(StructuredTypeField field)
		{
			this.owner.Fields.Add (field);
		}

		void ICollection<StructuredTypeField>.Clear()
		{
			this.owner.Fields.Clear ();
		}

		bool ICollection<StructuredTypeField>.Contains(StructuredTypeField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void ICollection<StructuredTypeField>.CopyTo(StructuredTypeField[] array, int arrayIndex)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		int ICollection<StructuredTypeField>.Count
		{
			get
			{
				return this.owner.Fields.Count;
			}
		}

		bool ICollection<StructuredTypeField>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<StructuredTypeField>.Remove(StructuredTypeField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<StructuredTypeField> Members

		IEnumerator<StructuredTypeField> IEnumerable<StructuredTypeField>.GetEnumerator()
		{
			return this.owner.Fields.Values.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.owner.Fields.Values.GetEnumerator ();
		}

		#endregion

		private StructuredType					owner;
	}
}
