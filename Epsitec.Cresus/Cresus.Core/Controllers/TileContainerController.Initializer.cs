//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed partial class TileContainerController
	{
		/// <summary>
		/// The <c>Initializer</c> class is used to set up the <see cref="TileContainerController"/>.
		/// </summary>
		public class Initializer : System.IDisposable
		{
			public Initializer(TileContainerController controller)
			{
				this.controller = controller;
			}

			public void Add(TileDataItem item)
			{
				this.controller.DataItems.Add (item);
			}

			public void Add(CollectionAccessor item)
			{
				this.controller.DataItems.Add (item);
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