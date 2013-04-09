using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using System;

using System.Collections.Generic;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	public abstract class EntityWriter
	{


		public EntityWriter(DataSetMetadata metadata, DataSetAccessor accessor)
		{
			this.metadata = metadata;
			this.accessor = accessor;
		}


		protected DataSetMetadata Metadata
		{
			get
			{
				return this.metadata;
			}
		}


		protected DataSetAccessor Accessor
		{
			get
			{
				return this.accessor;
			}
		}


		public abstract string Filename
		{
			get;
		}


		public abstract Stream GetStream();


		private readonly DataSetMetadata metadata;


		private readonly DataSetAccessor accessor;


	}


}
