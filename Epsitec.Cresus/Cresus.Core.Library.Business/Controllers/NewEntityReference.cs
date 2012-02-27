//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>NewEntityReference</c> class defines which entity should be edited and
	/// which entity should be used as the reference entity (kind of specialized tuple).
	/// </summary>
	public class NewEntityReference
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NewEntityReference"/> class.
		/// </summary>
		/// <param name="referenceEntity">The reference entity.</param>
		public NewEntityReference(AbstractEntity referenceEntity)
		{
			this.referenceEntity = referenceEntity;
			this.editionEntity   = referenceEntity;
			
			this.creationControllerMode = ViewControllerMode.Creation;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NewEntityReference"/> class.
		/// </summary>
		/// <param name="referenceEntity">The reference entity.</param>
		/// <param name="editionEntity">The edition entity (the one which will finally appear in the UI).</param>
		public NewEntityReference(AbstractEntity referenceEntity, AbstractEntity editionEntity)
		{
			this.referenceEntity = referenceEntity;
			this.editionEntity   = editionEntity;
			
			this.creationControllerMode = ViewControllerMode.Creation;
		}


		/// <summary>
		/// Gets the controller mode to be used for the UI, in order to edit a
		/// freshly created (edition) entity.
		/// </summary>
		/// <value>The creation controller mode.</value>
		public ViewControllerMode				CreationControllerMode
		{
			get
			{
				return this.creationControllerMode;
			}
		}


		/// <summary>
		/// Gets the reference entity (the one which will be referred to).
		/// </summary>
		/// <returns>The reference entity.</returns>
		public AbstractEntity GetReferenceEntity()
		{
			return this.referenceEntity;
		}

		/// <summary>
		/// Gets the edition entity (the one which will be edited in the UI).
		/// </summary>
		/// <returns>The edition entity.</returns>
		public AbstractEntity GetEditionEntity()
		{
			return this.editionEntity;
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="AbstractEntity"/> to <see cref="NewEntityReference"/>.
		/// </summary>
		/// <param name="value">The entity which has to be wrapped into a <see cref="NewEntityReference"/>.</param>
		/// <returns>The <see cref="NewEntityReference"/> for the entity.</returns>
		public static implicit operator NewEntityReference(AbstractEntity value)
		{
			return new NewEntityReference (value);
		}


		private readonly AbstractEntity			referenceEntity;
		private readonly AbstractEntity			editionEntity;
		private readonly ViewControllerMode		creationControllerMode;
	}
}
