using Epsitec.Common.Support.EntityEngine;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Column
	{


		public Column(string title, string name, ColumnType type, Func<AbstractEntity, object> valueGetter)
		{
			this.title = title;
			this.name = name;
			this.type = type;
			this.valueGetter = valueGetter;
		}


		public string Title
		{
			get
			{
				return this.title;
			}
		}


		public string Name
		{
			get
			{
				return this.name;
			}
		}


		public ColumnType Type
		{
			get
			{
				return this.type;
			}
		}


		public object GetValue(AbstractEntity entity)
		{
			return this.valueGetter (entity);
		}


		private readonly string title;


		private readonly string name;


		private readonly ColumnType type;


		private readonly Func<AbstractEntity, object> valueGetter;


	}


}
