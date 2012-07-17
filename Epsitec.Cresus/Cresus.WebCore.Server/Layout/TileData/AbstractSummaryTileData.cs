using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal abstract class AbstractSummaryTileData
	{

		// TODO Add the compact title, the compact text ?


		public string Icon
		{
			get;
			set;
		}


		public Type EntityType
		{
			get;
			set;
		}


		public Func<AbstractEntity, FormattedText> TitleGetter
		{
			get;
			set;
		}


		public Func<AbstractEntity, FormattedText> TextGetter
		{
			get;
			set;
		}


	}


}

