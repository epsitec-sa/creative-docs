using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class AbstractContactRepository : Repository
	{


		public AbstractContactRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByExample(AbstractContactEntity example)
		{
			return this.GetGenericContactsByExample<AbstractContactEntity> (example);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByExample(AbstractContactEntity example, int index, int count)
		{
			return this.GetGenericContactsByExample<AbstractContactEntity> (example, index, count);
		}


		public IEnumerable<AbstractContactEntity> GetAllAbstractContacts()
		{
			return this.GetAllGenericContacts<AbstractContactEntity> ();
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<AbstractContactEntity> (naturalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<AbstractContactEntity> (legalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<AbstractContactEntity> (roles);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<AbstractContactEntity> (comments);
		}


		public AbstractContactEntity CreateAbstractContactExample()
		{
			return this.CreateGenericContactExample<AbstractContactEntity> ();
		}


		protected IEnumerable<EntityType> GetGenericContactsByExample<EntityType>(EntityType example) where EntityType : AbstractContactEntity
		{
			return this.GetEntitiesByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactsByExample<EntityType>(EntityType example, int index, int count) where EntityType : AbstractContactEntity
		{
			return this.GetEntitiesByExample<EntityType> (example, index, count);
		}


		protected IEnumerable<EntityType> GetAllGenericContacts<EntityType>() where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactsByNaturalPerson<EntityType>(NaturalPersonEntity naturalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByNaturalPerson<EntityType> (naturalPerson);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactsByLegalPerson<EntityType>(LegalPersonEntity legalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByLegalPerson<EntityType> (legalPerson);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactsByRoles<EntityType>(params ContactRoleEntity[] roles) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByRoles<EntityType> (roles);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactsByComments<EntityType>(params CommentEntity[] comments) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByComments<EntityType> (comments);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected EntityType CreateGenericContactExample<EntityType>() where EntityType : AbstractContactEntity, new ()
		{
			return this.CreateExample<EntityType> ();
		}


		protected EntityType CreateGenericContactExampleByNaturalPerson<EntityType>(NaturalPersonEntity naturalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();
			example.NaturalPerson = naturalPerson;

			return example;
		}


		protected EntityType CreateGenericContactExampleByLegalPerson<EntityType>(LegalPersonEntity legalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();
			example.LegalPerson = legalPerson;

			return example;
		}


		protected EntityType CreateGenericContactExampleByRoles<EntityType>(IEnumerable<ContactRoleEntity> roles) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();

			foreach (ContactRoleEntity role in roles)
			{
				example.Roles.Add (role);
			}

			return example;
		}


		protected EntityType CreateGenericContactExampleByComments<EntityType>(IEnumerable<CommentEntity> comments) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();

			foreach (CommentEntity comment in comments)
			{
				example.Comments.Add (comment);
			}

			return example;
		}


	}


}
