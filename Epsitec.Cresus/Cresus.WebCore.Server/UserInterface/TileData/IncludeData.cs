using Epsitec.Common.Support.EntityEngine;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
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
