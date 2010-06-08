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


		public AbstractContactEntity GetAbstractContactByExample(AbstractContactEntity example)
		{
			return this.GetGenericContactByExample<AbstractContactEntity> (example);
		}


		public IEnumerable<AbstractContactEntity> GetAllAbstractContacts()
		{
			return this.GetAllGenericContacts<AbstractContactEntity> ();
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactByNaturalPerson<AbstractContactEntity> (naturalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactByLegalPerson<AbstractContactEntity> (legalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactByRoles<AbstractContactEntity> (roles);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactByComments<AbstractContactEntity> (comments);
		}


		public AbstractContactEntity CreateAbstractContactExample()
		{
			return this.CreateGenericContactExample<AbstractContactEntity> ();
		}


		protected IEnumerable<EntityType> GetGenericContactsByExample<EntityType>(EntityType example) where EntityType : AbstractContactEntity
		{
			return this.GetEntitiesByExample<EntityType> (example);
		}


		protected EntityType GetGenericContactByExample<EntityType>(EntityType example) where EntityType : AbstractContactEntity
		{
			return this.GetEntityByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetAllGenericContacts<EntityType>() where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExample<EntityType> ();

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactByNaturalPerson<EntityType>(NaturalPersonEntity naturalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByNaturalPerson<EntityType> (naturalPerson);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactByLegalPerson<EntityType>(LegalPersonEntity legalPerson) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByLegalPerson<EntityType> (legalPerson);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactByRoles<EntityType>(params ContactRoleEntity[] roles) where EntityType : AbstractContactEntity, new ()
		{
			EntityType example = this.CreateGenericContactExampleByRoles<EntityType> (roles);

			return this.GetGenericContactsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericContactByComments<EntityType>(params CommentEntity[] comments) where EntityType : AbstractContactEntity, new ()
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
