//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// The <c>EntityFieldDialogOptions</c> structure is used by the <see cref="EntityFieldDialog"/>
	/// class to store information about the field options.
	/// </summary>
	public struct EntityFieldDialogOptions
	{
		public EntityFieldDialogOptions(FieldOptions options, FieldRelation rel)
			: this ()
		{
			this.IsNullable = options.HasFlag (FieldOptions.Nullable);
			this.IsPrivate = options.HasFlag (FieldOptions.PrivateRelation);
			this.IsVirtual = options.HasFlag (FieldOptions.Virtual);
			this.IsCollection = (rel == FieldRelation.Collection);
			this.IsIndexAscending = options.HasFlag (FieldOptions.IndexAscending);
			this.IsIndexDescending = options.HasFlag (FieldOptions.IndexDescending);
			this.IsCaseInsensitive = options.HasFlag (FieldOptions.CollationCaseInsensitive);
			this.IsAccentInsensitive = options.HasFlag (FieldOptions.CollationAccentInsensitive);
			this.IsDisablePrefetch = options.HasFlag (FieldOptions.DisablePrefetch);
		}

		public bool								IsNullable
		{
			get;
			set;
		}
		public bool								IsCollection
		{
			get;
			set;
		}
		public bool								IsPrivate
		{
			get;
			set;
		}
		public bool								IsVirtual
		{
			get;
			set;
		}
		public bool								IsIndexAscending
		{
			get;
			set;
		}
		public bool								IsIndexDescending
		{
			get;
			set;
		}
		public bool								IsCaseInsensitive
		{
			get;
			set;
		}
		public bool								IsAccentInsensitive
		{
			get;
			set;
		}
		public bool								IsDisablePrefetch
		{
			get;
			set;
		}

		public FieldOptions GetFieldOptions(FieldOptions initialValue)
		{
			if (this.IsNullable)
			{
				initialValue |= FieldOptions.Nullable;
			}
			else
			{
				initialValue &= ~FieldOptions.Nullable;
			}

			if (this.IsPrivate)
			{
				initialValue |= FieldOptions.PrivateRelation;
			}
			else
			{
				initialValue &= ~FieldOptions.PrivateRelation;
			}

			if (this.IsVirtual)
			{
				initialValue |= FieldOptions.Virtual;
			}
			else
			{
				initialValue &= ~FieldOptions.Virtual;
			}

			if (this.IsIndexAscending)
			{
				initialValue |= FieldOptions.IndexAscending;
			}
			else
			{
				initialValue &= ~FieldOptions.IndexAscending;
			}

			if (this.IsIndexDescending)
			{
				initialValue |= FieldOptions.IndexDescending;
			}
			else
			{
				initialValue &= ~FieldOptions.IndexDescending;
			}

			if (this.IsAccentInsensitive)
			{
				initialValue |= FieldOptions.CollationAccentInsensitive;
			}
			else
			{
				initialValue &= ~FieldOptions.CollationAccentInsensitive;
			}

			if (this.IsCaseInsensitive)
			{
				initialValue |= FieldOptions.CollationCaseInsensitive;
			}
			else
			{
				initialValue &= ~FieldOptions.CollationCaseInsensitive;
			}

			if (this.IsDisablePrefetch)
			{
				initialValue |= FieldOptions.DisablePrefetch;
			}
			else
			{
				initialValue &= ~FieldOptions.DisablePrefetch;
			}

			return initialValue;
		}
	}
}
