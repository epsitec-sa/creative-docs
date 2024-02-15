//	Copyright © 2010-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	/// <summary>
	/// Ce contrôleur permet de voir un document déjà imprimé.
	/// </summary>
	public class SerializedDocumentBlobController : IEntitySpecialController
	{
		public SerializedDocumentBlobController(TileContainer tileContainer, SerializedDocumentBlobEntity serializedDocumentBlobEntity)
		{
			this.tileContainer = tileContainer;
			this.serializedDocumentBlobEntity = serializedDocumentBlobEntity;
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var controller = this.tileContainer.EntityViewController;
			var businessContext = controller.BusinessContext;
			var orchestrator = controller.Orchestrator;

			var c = new ComplexControllers.SerializedDocumentBlobController (businessContext, this.serializedDocumentBlobEntity);
			c.CreateUI (box);
		}


		private class Factory : DefaultEntitySpecialControllerFactory<SerializedDocumentBlobEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, SerializedDocumentBlobEntity entity, ViewId mode)
			{
				return new SerializedDocumentBlobController (container, entity);
			}
		}

	
		private readonly TileContainer tileContainer;
		private readonly SerializedDocumentBlobEntity serializedDocumentBlobEntity;

		private bool isReadOnly;
	}
}
