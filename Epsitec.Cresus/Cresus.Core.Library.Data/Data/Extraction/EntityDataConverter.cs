//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataConverter</c> is used to generate <see cref="EntityDataColumnConverter"/>
	/// instances based on a field accessor function.
	/// </summary>
	public static class EntityDataConverter
	{
		/// <summary>
		/// Gets the converter for an entity field.
		/// </summary>
		/// <typeparam name="T1">The type of the entity.</typeparam>
		/// <typeparam name="T2">The type of the field.</typeparam>
		/// <param name="func">The accessor function.</param>
		/// <returns>The converter (see <see cref="EntityDataConverter"/>).</returns>
		public static EntityDataColumnConverter GetFieldConverter<T1, T2>(System.Func<T1, T2> func)
			where T1 : AbstractEntity
		{
			System.Type entityType = typeof (T1);
			System.Type fieldType  = typeof (T2);

			EntityDataType dataType = EntityDataType.None;
			System.Type creatorType = typeof (GetterCreator<,>).MakeGenericType (entityType, fieldType);
			object[]    creatorArgs = new object[3];

			if (fieldType == typeof (string))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<string, string>) (x => x);
				creatorArgs[2] = null;
			}
			else if (fieldType == typeof (FormattedText))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<FormattedText, string>) (x => x.ToSimpleText () ?? "");
				creatorArgs[2] = null;
			}
			else if (fieldType == typeof (FormattedText?))
			{
				dataType = EntityDataType.Text;
				creatorArgs[1] = (System.Func<FormattedText?, string>) (x => x.HasValue ? x.Value.ToSimpleText () ?? "" : "");
				creatorArgs[2] = null;
			}
			else if (fieldType == typeof (decimal?))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<decimal?, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<decimal?, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}
			else if (fieldType == typeof (decimal))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<decimal, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<decimal, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}
			else if (fieldType == typeof (int?))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<int?, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<int?, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}
			else if (fieldType == typeof (int))
			{
				dataType = EntityDataType.Number;
				creatorArgs[1] = (System.Func<int, string>) (x => EntityDataConverter.ConvertToText (x));
				creatorArgs[2] = (System.Func<int, long>) (x => EntityDataConverter.ConvertToNumeric (x));
			}
			else
			{
				//	TODO: add conversion functions as needed
			}

			if (dataType == EntityDataType.None)
			{
				return null;
			}

			creatorArgs[0] = func;

			var creator = System.Activator.CreateInstance (creatorType, creatorArgs) as GetterCreator;

			return new EntityDataColumnConverter (dataType, creator.CreateTextGetter (), creator.CreateNumericGetter ());
		}

		
		private static string ConvertToText(decimal? value)
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


		//	TODO: add ConvertToText implementations for other type


		private static long ConvertToNumeric(decimal? value)
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

		private static long ConvertToNumeric(int? value)
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

		private static long ConvertToNumeric(System.DateTime? value)
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

		private static long ConvertToNumeric(Date? value)
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

		private static long ConvertToNumeric(Time? value)
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


		#region GetterCreator Class

		abstract class GetterCreator
		{
			public abstract System.Func<AbstractEntity, string> CreateTextGetter();
			public abstract System.Func<AbstractEntity, long> CreateNumericGetter();
		}

		#endregion

		#region GetterCreator<TEntity, TField> Class

		sealed class GetterCreator<TEntity, TField> : GetterCreator
			where TEntity : AbstractEntity
		{
			public GetterCreator(System.Func<TEntity, TField> func, System.Func<TField, string> textConverter, System.Func<TField, long> numericConverter)
			{
				this.func = func;
				
				this.textConverter    = textConverter;
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

			private readonly System.Func<TEntity, TField>	func;
			private readonly System.Func<TField, string>	textConverter;
			private readonly System.Func<TField, long>		numericConverter;
		}

		#endregion
	}
}