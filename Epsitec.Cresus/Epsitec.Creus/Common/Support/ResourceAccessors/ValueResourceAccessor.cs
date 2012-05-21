//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>ValueResourceAccessor</c> is used to access enumeration values,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Val."</c>.
	/// </summary>
	internal class ValueResourceAccessor : CaptionResourceAccessor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueResourceAccessor"/> class.
		/// </summary>
		public ValueResourceAccessor()
		{
		}

		/// <summary>
		/// Creates the item; this overload may not be used.
		/// Use <see cref="CreateValueItem"/> instead.
		/// </summary>
		/// <returns>never</returns>
		/// <exception cref="System.InvalidOperationException">Always throws an exception.</exception>
		public override CultureMap CreateItem()
		{
			throw new System.InvalidOperationException ("CreateItem may not be called directly; use CreateValueItem instead");
		}

		/// <summary>
		/// Creates the value item, using the specified prefix. The prefix should
		/// match the enumeration to which this value belongs to.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <returns>An empty item.</returns>
		public PrefixedCultureMap CreateValueItem(string prefix)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (prefix));
			System.Diagnostics.Debug.Assert (!prefix.EndsWith ("."));
			System.Diagnostics.Debug.Assert (!prefix.StartsWith ("."));
			
			ResourceBundle.Field field = new ResourceBundle.Field (string.Concat (this.Prefix, prefix, "."));
			
			return this.CreateItem (field, this.CreateId ()) as PrefixedCultureMap;
		}

		/// <summary>
		/// Gets the caption prefix for this accessor.
		/// Note: several resource types are stored as captions; the prefix of
		/// the field name is used to differentiate them.
		/// </summary>
		/// <value>The caption <c>"Val."</c> prefix.</value>
		protected override string Prefix
		{
			get
			{
				return "Val.";
			}
		}

		/// <summary>
		/// Gets the pure caption name from a field name. This simply strips of
		/// the field name prefix and the item prefix.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="fieldName">The field name.</param>
		/// <returns>The pure caption name.</returns>
		protected override string GetNameFromFieldName(CultureMap item, string fieldName)
		{
			PrefixedCultureMap fieldItem = item as PrefixedCultureMap;

			System.Diagnostics.Debug.Assert (fieldItem != null);

			string prefix = string.Concat (this.Prefix, fieldItem.Prefix, ".");

			System.Diagnostics.Debug.Assert (fieldName.StartsWith (prefix));

			return fieldName.Substring (prefix.Length);
		}

		/// <summary>
		/// Gets the resource field name of the resource based on the caption
		/// name and the item prefix.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="data">The data record.</param>
		/// <returns>The resource field name.</returns>
		protected override string GetFieldNameFromName(CultureMap item, StructuredData data)
		{
			PrefixedCultureMap fieldItem = item as PrefixedCultureMap;

			return string.Concat (this.Prefix, fieldItem.Prefix, ".", fieldItem.Name);
		}

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <param name="field">The resource field.</param>
		/// <param name="id">The id for the new item.</param>
		/// <param name="source">The source for the data.</param>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
		protected override CultureMap CreateItem(ResourceBundle.Field field, Druid id, CultureMapSource source)
		{
			string name = field.Name;

			System.Diagnostics.Debug.Assert (name.StartsWith (this.Prefix));
			
			string prefix = name.Substring (this.Prefix.Length);
			int    pos    = prefix.LastIndexOf ('.');

			if (pos < 1)
			{
				throw new System.ArgumentException (string.Format ("Invalid value name '{0}'", name));
			}

			prefix = prefix.Substring (0, pos);

			CultureMap item = new PrefixedCultureMap (this, id, source, prefix);
			item.IsNewItem = true;
			return item;
		}
	}
}
