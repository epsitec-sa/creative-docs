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
			return this.GetEntitiesByExample<AbstractContactEntity> (example);
		}


		public AbstractContactEntity GetAbstractContactByExample(AbstractContactEntity example)
		{
			return this.GetEntityByExample<AbstractContactEntity> (example);
		}


		public IEnumerable<AbstractContactEntity> GetAllAbstractContacts()
		{
			AbstractContactEntity example = this.CreateAbstractContactExample ();

			return this.GetAbstractContactsByExample (example);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			AbstractContactEntity example = this.CreateAbstractContactExampleByNaturalPerson (naturalPerson);

			return this.GetAbstractContactsByExample (example);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByLegalPerson(LegalPersonEntity legalPerson)
		{
			AbstractContactEntity example = this.CreateAbstractContactExampleByLegalPerson (legalPerson);

			return this.GetAbstractContactsByExample (example);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByRoles(params ContactRoleEntity[] roles)
		{
			AbstractContactEntity example = this.CreateAbstractContactExampleByRoles (roles);

			return this.GetAbstractContactsByExample (example);
		}


		public IEnumerable<AbstractContactEntity> GetAbstractContactByComments(params CommentEntity[] comments)
		{
			AbstractContactEntity example = this.CreateAbstractContactExampleByComments (comments);

			return this.GetAbstractContactsByExample (example);
		}


		protected AbstractContactEntity CreateAbstractContactExample ()
		{
			return this.CreateExample<AbstractContactEntity> ();
		}


		private AbstractContactEntity CreateAbstractContactExampleByNaturalPerson(NaturalPersonEntity naturalPerson)
		{
			AbstractContactEntity example = this.CreateAbstractContactExample ();
			example.NaturalPerson = naturalPerson;

			return example;
		}


		private AbstractContactEntity CreateAbstractContactExampleByLegalPerson(LegalPersonEntity legalPerson)
		{
			AbstractContactEntity example = this.CreateAbstractContactExample ();
			example.LegalPerson = legalPerson;

			return example;
		}


		private AbstractContactEntity CreateAbstractContactExampleByRoles(IEnumerable<ContactRoleEntity> roles)
		{
			AbstractContactEntity example = this.CreateAbstractContactExample ();

			foreach (ContactRoleEntity role in roles)
			{
				example.Roles.Add (role);
			}
			
			return example;
		}


		private AbstractContactEntity CreateAbstractContactExampleByComments(IEnumerable<CommentEntity> comments)
		{
			AbstractContactEntity example = this.CreateAbstractContactExample ();

			foreach (CommentEntity comment in comments)
			{
				example.Comments.Add (comment);
			}

			return example;
		}


	}


}
