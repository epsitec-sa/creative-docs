//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public class Bridge<T>
		where T : AbstractEntity, new ()
	{
		public Bridge(EntityViewController<T> controller)
		{
			this.controller = controller;
		}

		public TileDataItem CreateTileDataItem(Brick brick)
		{
			var item = new TileDataItem ();

			item.Name = Bricks.Brick.GetProperty (brick, Bricks.BrickPropertyKey.Name).StringValue;
			item.IconUri = Bricks.Brick.GetProperty (brick, Bricks.BrickPropertyKey.Icon).StringValue;
			item.Title = Bricks.Brick.GetProperty (brick, Bricks.BrickPropertyKey.Title).StringValue;
			item.CompactTitle = Bricks.Brick.GetProperty (brick, Bricks.BrickPropertyKey.TitleCompact).StringValue;
			item.TextAccessor = this.ToAccessor (Brick.GetProperty (brick, BrickPropertyKey.Text));
			item.CompactTextAccessor = this.ToAccessor (Brick.GetProperty (brick, BrickPropertyKey.TextCompact));
			item.EntityMarshaler = this.controller.CreateEntityMarshaler ();

			return item;
		}

		private Accessor<FormattedText> ToAccessor(BrickProperty property)
		{
			System.Func<T, FormattedText> formatter = property.GetFormatter<T> ();
			
			if (formatter == null)
			{
				return null;
			}
			else
			{
				return this.controller.CreateAccessor (formatter);
			}
		}


		private readonly EntityViewController<T> controller;
	}
}
