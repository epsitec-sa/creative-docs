//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataColumnConverter</c> class provides data access to a field of an entity.
	/// The data will be available as a simple string, or as a numeric value and should be used
	/// only for sorting and ordering, as the numeric value is not necessarily meaningful.
	/// </summary>
	public sealed class EntityDataColumnConverter
	{
		internal EntityDataColumnConverter(EntityDataType dataType, System.Func<AbstractEntity, string> f1, System.Func<AbstractEntity, long> f2)
		{
			this.dataType = dataType;
			this.f1 = f1;
			this.f2 = f2;
		}

		
		public EntityDataType					DataType
		{
			get
			{
				return this.dataType;
			}
		}

		public bool								IsNumeric
		{
			get
			{
				return this.f2 != null;
			}
		}


		/// <summary>
		/// Gets the textual representation of this column's field.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The textual value.</returns>
		public string GetText(AbstractEntity entity)
		{
			return this.f1 (entity);
		}

		/// <summary>
		/// Gets the numeric representation of this column's field.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The numeric value.</returns>
		public long GetNumber(AbstractEntity entity)
		{
			return this.f2 (entity);
		}

		
		private readonly EntityDataType							dataType;
		private readonly System.Func<AbstractEntity, string>	f1;
		private readonly System.Func<AbstractEntity, long>		f2;
	}
}
