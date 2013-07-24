using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator
{


	/// <summary>
	/// This class is used as a factory to create entities that are referenced by other. For
	/// instance, if we have a Person entity with a Comment field, we can use an AutoCreator that
	/// will be used to instantiates Comments and put them in the appropriate Person property.
	/// </summary>
	/// <remarks>
	/// An AutoCreator is fully defined by the lambda expression that represents the entity
	/// property that it targets. In addition, they have an id, so that this id can be given to
	/// the client and the corresponding AutoCreator can be find back later on, only based on its
	/// id.
	/// An AutoCreator is not bound to an entity. They are general factories that can be used for
	/// all entities of the same type. The entity on which to apply the AutoCreator is given as an
	/// argument to the Execute(...) method.
	/// The Execute(...) method is thread safe.
	/// </remarks>
	internal sealed class AutoCreator
	{


		public AutoCreator(LambdaExpression lambda, string id)
		{
			this.id = id;
			this.propertyAccessor = new EntityReferencePropertyAccessor (lambda, "");
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}


		public AbstractEntity Execute(BusinessContext businessContext, AbstractEntity entity)
		{
			var childType = this.propertyAccessor.Type;
			var childTypeId = EntityInfo.GetTypeId (childType);

			var child = (AbstractEntity) this.propertyAccessor.GetValue (entity);

			if (child.IsNull ())
			{
				child = businessContext.CreateAndRegisterEntity (childTypeId);

				propertyAccessor.SetValue (entity, child);
			}

			return child;
		}


		private readonly string id;


		private readonly EntityReferencePropertyAccessor propertyAccessor;



	}


}
