//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed partial class TileContainerController
	{
		/// <summary>
		/// The <c>Initializer</c> class is used to set up the <see cref="TileContainerController"/>
		/// in a <c>using</c> block.
		/// </summary>
		public sealed class Initializer : System.IDisposable
		{
			public Initializer(EntityViewController entityViewController)
			{
				this.controller = new TileContainerController (entityViewController);
			}

			public static implicit operator TileDataItems(Initializer x)
			{
				return x.controller.DataItems;
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.ValidateCreation ();
			}

			#endregion

			private void ValidateCreation()
			{
				this.controller.GenerateTiles ();

				var viewController = this.controller.controller;
				
				using (var builder = new UIBuilder (viewController))
				{
					builder.CreateFooterEditorTile ();
				}
			}
			
			private readonly TileContainerController	controller;
		}
	}
}