using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal abstract class AbstractSummaryTileData : AbstractTileData
	{

		// TODO Add the compact title, the compact text ?


		public Func<AbstractEntity, FormattedText> TextGetter
		{
			get;
			set;
		}


	}


}

