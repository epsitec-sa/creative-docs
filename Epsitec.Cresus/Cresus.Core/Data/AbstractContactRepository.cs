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


		public IEnumerable<AbstractContactEntity> GetAllAbstractContacts(int index, int count)
		{
			return this.GetAllGenericContacts<AbstractContactEntity> (index, count);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			return this.GetGenericContactsByNaturalPerson<AbstractContactEntity> (naturalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByNaturalPerson(NaturalPersonEntity naturalPerson, int index, int count)
		{
			return this.GetGenericContactsByNaturalPerson<AbstractContactEntity> (naturalPerson, index, count);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByLegalPerson(LegalPersonEntity legalPerson)
		{
			return this.GetGenericContactsByLegalPerson<AbstractContactEntity> (legalPerson);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByLegalPerson(LegalPersonEntity legalPerson, int index, int count)
		{
			return this.GetGenericContactsByLegalPerson<AbstractContactEntity> (legalPerson, index, count);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByRoles(params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<AbstractContactEntity> (roles);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByRoles(int index, int count, params ContactRoleEntity[] roles)
		{
			return this.GetGenericContactsByRoles<AbstractContactEntity> (index, count, roles);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByComments(params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<AbstractContactEntity> (comments);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactsByComments(int index, int count, params CommentEntity[] comments)
		{
			return this.GetGenericContactsByComments<AbstractContactEntity> (index, count, comments);
		}


		public AbstractContactEntity CreateAbstractContactExample()
		{
			return this.CreateGenericContactExample<AbstractContactEntity> ();
		}


		protected IEnumerable<T> GetGenericContactsByExample<T>(T example) where T : AbstractContactEntity
		{
			return this.GetEntitiesByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByExample<T>(T example, int index, int count) where T : AbstractContactEntity
		{
			return this.GetEntitiesByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetAllGenericContacts<T>() where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetAllGenericContacts<T>(int index, int count) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();

			return this.GetGenericContactsByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericContactsByNaturalPerson<T>(NaturalPersonEntity naturalPerson) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByNaturalPerson<T> (naturalPerson);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByNaturalPerson<T>(NaturalPersonEntity naturalPerson, int index, int count) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByNaturalPerson<T> (naturalPerson);

			return this.GetGenericContactsByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericContactsByLegalPerson<T>(LegalPersonEntity legalPerson) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByLegalPerson<T> (legalPerson);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByLegalPerson<T>(LegalPersonEntity legalPerson, int index, int count) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByLegalPerson<T> (legalPerson);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByRoles<T>(params ContactRoleEntity[] roles) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByRoles<T> (roles);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByRoles<T>(int index, int count, params ContactRoleEntity[] roles) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByRoles<T> (roles);

			return this.GetGenericContactsByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericContactsByComments<T>(params CommentEntity[] comments) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByComments<T> (comments);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericContactsByComments<T>(int index, int count, params CommentEntity[] comments) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExampleByComments<T> (comments);

			return this.GetGenericContactsByExample<T> (example);
		}


		protected T CreateGenericContactExample<T>() where T : AbstractContactEntity, new ()
		{
			return this.CreateExample<T> ();
		}


		protected T CreateGenericContactExampleByNaturalPerson<T>(NaturalPersonEntity naturalPerson) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();
			example.NaturalPerson = naturalPerson;

			return example;
		}


		protected T CreateGenericContactExampleByLegalPerson<T>(LegalPersonEntity legalPerson) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();
			example.LegalPerson = legalPerson;

			return example;
		}


		protected T CreateGenericContactExampleByRoles<T>(IEnumerable<ContactRoleEntity> roles) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();

			foreach (ContactRoleEntity role in roles)
			{
				example.Roles.Add (role);
			}

			return example;
		}


		protected T CreateGenericContactExampleByComments<T>(IEnumerable<CommentEntity> comments) where T : AbstractContactEntity, new ()
		{
			T example = this.CreateGenericContactExample<T> ();

			foreach (CommentEntity comment in comments)
			{
				example.Comments.Add (comment);
			}

			return example;
		}


	}


}
