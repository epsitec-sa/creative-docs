//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Support.ResourceAccessors
{
	/// <summary>
	/// The <c>FieldResourceAccessor</c> is used to access entity
	/// resources, stored in the <c>Captions</c> resource bundle and which
	/// have a field name prefixed with <c>"Fld."</c>.
	/// </summary>
	internal class FieldResourceAccessor : CaptionResourceAccessor
	{
		public FieldResourceAccessor()
		{
		}

		/// <summary>
		/// Creates the item; this overload may not be used.
		/// Use <see cref="CreateFieldItem"/> instead.
		/// </summary>
		/// <returns>never</returns>
		/// <exception cref="System.InvalidOperationException">Always throws an exception.</exception>
		public override CultureMap CreateItem()
		{
			throw new System.InvalidOperationException ("CreateItem may not be called directly; use CreateFieldItem instead");
		}

		/// <summary>
		/// Creates the field item, using the specified prefix. The prefix should
		/// match the structured type to which this field belongs to.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <returns>An empty item.</returns>
		public PrefixedCultureMap CreateFieldItem(string prefix)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (prefix));
			System.Diagnostics.Debug.Assert (!prefix.EndsWith ("."));
			System.Diagnostics.Debug.Assert (!prefix.StartsWith ("."));
			
			ResourceBundle.Field field = new ResourceBundle.Field (string.Concat (this.Prefix, prefix, "."));
			
			return this.CreateItem (field, this.CreateId ()) as PrefixedCultureMap;
		}

		protected override string Prefix
		{
			get
			{
				return "Fld.";
			}
		}

		protected override string GetNameFromFieldName(CultureMap item, string fieldName)
		{
			PrefixedCultureMap fieldItem = item as PrefixedCultureMap;

			string prefix = string.Concat (this.Prefix, fieldItem.Prefix);

			return fieldName.Substring (prefix.Length + 1);
		}

		protected override string GetFieldNameFromName(CultureMap item, StructuredData data)
		{
			PrefixedCultureMap fieldItem = item as PrefixedCultureMap;

			return string.Concat (this.Prefix, fieldItem.Prefix, ".", fieldItem.Name);
		}

		protected override CultureMap CreateItem(ResourceBundle.Field field, Druid id)
		{
			string name = field.Name;

			System.Diagnostics.Debug.Assert (name.StartsWith (this.Prefix));

			string prefix = name.Substring (this.Prefix.Length);
			int    pos    = prefix.LastIndexOf ('.');

			if (pos < 1)
			{
				throw new System.ArgumentException (string.Format ("Invalid field name '{0}'", name));
			}

			prefix = prefix.Substring (0, pos);

			return new PrefixedCultureMap (this, id, this.GetCultureMapSource (field), prefix);
		}
	}
}
