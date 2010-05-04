//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Classe de base de SummaryTile et EditionTile, qui représente une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public abstract class AbstractTile : TileContainer
	{
		public AbstractTile()
		{
		}

		public AbstractTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public EntitiesAccessors.AbstractAccessor EntitiesAccessor
		{
			get;
			set;
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}

		public Controllers.ViewControllerMode Mode
		{
			get;
			set;
		}

		public Controllers.ViewControllerMode ChildrenMode
		{
			get;
			set;
		}

		public bool EnableCreateAndRemoveButton
		{
			get;
			set;
		}
	}
}
