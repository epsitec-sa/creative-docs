//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public abstract class AbstractDocumentBusinessLogic
	{
		public AbstractDocumentBusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
		{
			this.businessContext        = businessContext;
			this.documentMetadataEntity = documentMetadataEntity;
		}


		public virtual bool IsLinesEditionEnabled
		{
			//	Indique si on peut éditer librement toutes les lignes, c'est-à-dire en créer, modifier,
			//	déplacer et supprimer.
			get
			{
				return false;
			}
		}

		public virtual bool IsArticleParametersEditionEnabled
		{
			//	Indique si on peut choisir ou modifier un article.
			get
			{
				return false;
			}
		}

		public virtual bool IsTextEditionEnabled
		{
			//	Indique si on peut éditer les textes.
			get
			{
				return false;
			}
		}

		public virtual bool IsMyEyesOnlyEditionEnabled
		{
			//	Indique si on ne peut éditer que les textes 'MyEyesOnly', c'est-à-dire pour les documents
			//	interne à l'entreprise.
			get
			{
				return false;
			}
		}

		public virtual bool IsPriceEditionEnabled
		{
			//	Indique si on peut éditer les prix.
			get
			{
				return false;
			}
		}

		public virtual bool IsDiscountEditionEnabled
		{
			//	Indique si on peut éditer les rabais.
			get
			{
				return false;
			}
		}


		public virtual ArticleQuantityType MainArticleQuantityType
		{
			//	Retourne le type de quantité principal, c'est-à-dire celui qui est édité avec l'article.
			get
			{
				return ArticleQuantityType.None;
			}
		}

		public abstract IEnumerable<ArticleQuantityType> EnabledArticleQuantityTypes
		{
			//	Retourne la liste des types de quantité éditables.
			get;
		}

		public abstract IEnumerable<ArticleQuantityType> PrintableArticleQuantityTypes
		{
			//	Retourne la liste des types de quantité imprimables.
			//	La première est la quantité principale.
			get;
		}


		protected readonly BusinessContext						businessContext;
		protected readonly DocumentMetadataEntity				documentMetadataEntity;
	}
}
