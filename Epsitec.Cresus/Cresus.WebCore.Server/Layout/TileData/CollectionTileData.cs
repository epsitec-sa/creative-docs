using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class CollectionTileData
	{


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
		
		
		public EntityCollectionPropertyAccessor PropertyAccessor
		{
			get;
			set;
		}


		public Func<AbstractEntity, IEnumerable<AbstractEntity>> EntitiesGetter
		{
			get;
			set;
		}


	}


}

