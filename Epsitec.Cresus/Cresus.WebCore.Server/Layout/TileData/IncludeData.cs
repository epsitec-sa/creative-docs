using Epsitec.Common.Support.EntityEngine;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class IncludeData
	{


		public Func<AbstractEntity, AbstractEntity> EntityGetter
		{
			get;
			set;
		}


	}


}
