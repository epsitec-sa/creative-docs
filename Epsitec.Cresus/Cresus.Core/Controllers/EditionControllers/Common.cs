//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	internal static class Common
	{
		public static void ChangeEditedLineEntity(TileContainer tileContainer, DataContext dataContext, AbstractDocumentItemEntity entity, string tabPageName)
		{
			EntityViewController parentController = Common.GetParentController (tileContainer);
			InvoiceDocumentEntity invoiceDocument = parentController.GetEntity () as InvoiceDocumentEntity;

			//	Cherche l'index de la ligne dans la collection.
			int index = invoiceDocument.Lines.IndexOf (entity);
			if (index == -1)
			{
				return;
			}

			//	Ferme la tuile.
			parentController.Orchestrator.CloseSubViews (parentController);

			//	Supprime l'entité dans la db.
			invoiceDocument.Lines.RemoveAt (index);              // supprime dans la liste de la facture
			dataContext.DeleteEntity (entity);                   // supprime dans le DataContext de la ligne
			parentController.DataContext.DeleteEntity (entity);  // supprime dans le DataContext de la facture

			//	Crée la nouvelle entité.
			AbstractDocumentItemEntity newEntity = null;

			if (tabPageName == "Text")
			{
				newEntity = parentController.DataContext.CreateEmptyEntity<TextDocumentItemEntity> ();
			}
			else if (tabPageName == "Article")
			{
				newEntity = parentController.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();

				var article = newEntity as ArticleDocumentItemEntity;
				article.BeginDate = invoiceDocument.CreationDate;
				article.EndDate   = invoiceDocument.CreationDate;
			}
			else if (tabPageName == "Price")
			{
				newEntity = parentController.DataContext.CreateEmptyEntity<PriceDocumentItemEntity> ();
			}

			System.Diagnostics.Debug.Assert (newEntity != null);
			newEntity.Visibility = true;

			invoiceDocument.Lines.Insert (index, newEntity);

			//	Crée et montre la nouvelle tuile.
			parentController.TileContainerController.ShowSubView (index, "DocumentItem");
		}


		/// <summary>
		/// Cette méthode "magique" est capable de retrouver l'entité parent à partir du container servant
		/// à éditer une entité fille.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static AbstractEntity GetParentEntity(TileContainer container)
		{
			var parentController = Common.GetParentController (container);

			if (parentController != null)
			{
				return parentController.GetEntity ();
			}

			return null;
		}

		/// <summary>
		/// Cette méthode "magique" est capable de retrouver le contrôleur parent à partir du container servant
		/// à éditer une entité fille.
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public static EntityViewController GetParentController(TileContainer container)
		{
			// TODO: Il faudra supprimer cette méthode. Un *ViewController devrait connaître les entités parent !
			var controllers = container.Controller.Orchestrator.Controller.GetAllSubControllers ();

			if (controllers.Count () > 1)
			{
				return controllers.ElementAt (1) as EntityViewController;
			}

			return null;
		}
	}
}
