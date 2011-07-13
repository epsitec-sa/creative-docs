//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public static class EntityDataConverter
	{
		public static string ConvertToText(decimal? value)
		{
			if (value.HasValue)
			{
				return value.Value.ToString ();
			}
			else
			{
				return "";
			}
		}

		public static long ConvertToNumeric(decimal? value)
		{
			if (value.HasValue)
			{
				return (long) (value.Value * 1000M);
			}
			else
			{
				return long.MinValue;
			}
		}

		public static long ConvertToNumeric(int? value)
		{
			if (value.HasValue)
			{
				return value.Value * 1000L;
			}
			else
			{
				return long.MinValue;
			}
		}

		public static long ConvertToNumeric(System.DateTime? value)
		{
			if (value.HasValue)
			{
				return value.Value.Ticks;
			}
			else
			{
				return long.MinValue;
			}
		}

		public static long ConvertToNumeric(Date? value)
		{
			if (value.HasValue)
			{
				return value.Value.Ticks;
			}
			else
			{
				return long.MinValue;
			}
		}

		public static long ConvertToNumeric(Time? value)
		{
			if (value.HasValue)
			{
				return value.Value.Ticks;
			}
			else
			{
				return long.MinValue;
			}
		}

		public static EntityDataColumnConverter GetFieldConverter<T1, T2>(System.Func<T1, T2> func)
			where T1 : AbstractEntity
		{
			System.Type type = typeof (T2);

			EntityDataType dataType = EntityDataType.None;
			System.Type creatorType = typeof (GetterCreator<,>).MakeGenericType (typeof (T1), type);
			object[]    creatorArgs = new object[3];

			if (type == typeof (string))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<string, string>) (x => x);
				creatorArgs[2] = null;
			}
			else if (type == typeof (FormattedText))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<FormattedText, string>) (x => x.ToSimpleText () ?? "");
				creatorArgs[2] = null;
			}
			else if (type == typeof (FormattedText?))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<FormattedText?, string>) (x => x.HasValue ? x.Value.ToSimpleText () ?? "" : "");
				creatorArgs[2] = null;
			}
			else if (type == typeof (decimal?))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<decimal?, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<decimal?, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}
			else if (type == typeof (decimal))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<decimal, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<decimal, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}

			if (dataType == EntityDataType.None)
			{
				return null;
			}

			creatorArgs[0] = func;

			var creator = System.Activator.CreateInstance (creatorType, creatorArgs) as GetterCreator;

			return new EntityDataColumnConverter (dataType, creator.CreateTextGetter (), creator.CreateNumericGetter ());
		}


		abstract class GetterCreator
		{
			public abstract System.Func<AbstractEntity, string> CreateTextGetter();
			public abstract System.Func<AbstractEntity, long> CreateNumericGetter();
		}

		sealed class GetterCreator<TEntity, TField> : GetterCreator
			where TEntity : AbstractEntity
		{
			public GetterCreator(System.Func<TEntity, TField> func, System.Func<TField, string> textConverter, System.Func<TField, long> numericConverter)
			{
				this.func = func;
				this.textConverter = textConverter;
				this.numericConverter = numericConverter;
			}

			public override System.Func<AbstractEntity, string> CreateTextGetter()
			{
				if (this.textConverter == null)
				{
					return null;
				}
				else
				{
					return this.TextGetter;
				}
			}

			public override System.Func<AbstractEntity, long> CreateNumericGetter()
			{
				if (this.numericConverter == null)
				{
					return null;
				}
				else
				{
					return this.NumericGetter;
				}
			}

			private string TextGetter(AbstractEntity entity)
			{
				var source = entity as TEntity;

				if (source == null)
				{
					return "";
				}
				else
				{
					return this.textConverter (this.func (source));
				}
			}

			private long NumericGetter(AbstractEntity entity)
			{
				var source = entity as TEntity;

				if (source == null)
				{
					return long.MinValue;
				}
				else
				{
					return this.numericConverter (this.func (source));
				}
			}

			private readonly System.Func<TEntity, TField> func;
			private readonly System.Func<TField, string> textConverter;
			private readonly System.Func<TField, long> numericConverter;
		}
	}
}