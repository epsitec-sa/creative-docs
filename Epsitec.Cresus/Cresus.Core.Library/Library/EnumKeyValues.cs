//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
