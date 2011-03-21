//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>EnumKeyValues</c> class is used to store an <c>enum</c> key
	/// and associated texts which represent its value. See the generic
	/// type <c>EnumKeyValues{T}</c> for a concrete implementation.
	/// </summary>
	public abstract class EnumKeyValues : ITextFormatter
	{
		public static EnumKeyValues<T> Create<T>(T key, params string[] values)
		{
			return new EnumKeyValues<T> (key, values);
		}
		
		public static EnumKeyValues<T> Create<T>(T key, params FormattedText[] values)
		{
			return new EnumKeyValues<T> (key, values);
		}

		public static IEnumerable<EnumKeyValues<T>> FromEnum<T>()
			where T : struct
		{
			var typeObject = TypeRosetta.GetTypeObject (typeof (T));
			var enumType   = typeObject as EnumType;

			if (enumType == null)
			{
				yield break;
			}

			foreach (var item in enumType.Values)
			{
				var enumKey = (T) (object) item.Value;
				var caption = item.Caption;
				var labels  = caption.Labels.ToArray ();

				if (labels.Length == 0)
				{
					var red = Epsitec.Common.Drawing.Color.FromName ("Red");
					labels = new string[] { FormattedText.FromSimpleText (enumKey.ToString ()).ApplyFontColor (red).ToString () };
				}

				yield return EnumKeyValues.Create<T> (enumKey, labels);
			}
		}

		public static IEnumerable<EnumKeyValues<Druid>> FromEntityIds(IEnumerable<Druid> entityIds)
		{
			foreach (var entityId in entityIds)
			{
				var info    = EntityInfo.GetStructuredType (entityId);
				var caption = info == null ? null : info.Caption;
				var label   = new FormattedText (info.Caption.DefaultLabel ?? info.Caption.Description ?? "");
				var name    = FormattedText.FromSimpleText (info.Caption.Name);

				yield return EnumKeyValues.Create (caption.Id, name, label);
			}
		}

		public abstract FormattedText[] Values
		{
			get;
		}

		#region ITextFormatter Members

		public FormattedText ToFormattedText(System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			return FormattedText.Join (" - ", this.Values);
		}

		#endregion
	}
}
