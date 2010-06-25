using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class ContactRoleRepository : Repository
	{


		public ContactRoleRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example, EntityConstrainer constrainer)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example, constrainer);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example, index, count);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example, EntityConstrainer constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example, constrainer, index, count);
		}


		public IEnumerable<ContactRoleEntity> GetAllContactRoles()
		{
			ContactRoleEntity example = this.CreateContactRoleExample ();

			return this.GetContactRolesByExample (example);
		}


		public IEnumerable<ContactRoleEntity> GetAllContactRoles(int index, int count)
		{
			ContactRoleEntity example = this.CreateContactRoleExample ();

			return this.GetContactRolesByExample (example, index, count);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByName(string name)
		{
			ContactRoleEntity example = this.CreateContactRoleExampleByName (name);

			return this.GetContactRolesByExample (example);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByName(string name, int index, int count)
		{
			ContactRoleEntity example = this.CreateContactRoleExampleByName (name);

			return this.GetContactRolesByExample (example, index, count);
		}


		public ContactRoleEntity CreateContactRoleExample()
		{
			return this.CreateExample<ContactRoleEntity> ();
		}


		private ContactRoleEntity CreateContactRoleExampleByName(string name)
		{
			ContactRoleEntity example = this.CreateContactRoleExample ();
			example.Name = name;

			return example;
		}


	}


}
