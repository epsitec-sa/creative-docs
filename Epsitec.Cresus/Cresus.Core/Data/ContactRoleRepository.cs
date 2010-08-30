using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class ContactRoleRepository : Repository<ContactRoleEntity>
	{
		public ContactRoleRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<ContactRoleEntity> (request);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByExample(ContactRoleEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<ContactRoleEntity> (example, index, count);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<ContactRoleEntity> (request, index, count);
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


		public IEnumerable<ContactRoleEntity> GetContactRolesByName(FormattedText name)
		{
			ContactRoleEntity example = this.CreateContactRoleExampleByName (name);

			return this.GetContactRolesByExample (example);
		}


		public IEnumerable<ContactRoleEntity> GetContactRolesByName(FormattedText name, int index, int count)
		{
			ContactRoleEntity example = this.CreateContactRoleExampleByName (name);

			return this.GetContactRolesByExample (example, index, count);
		}


		public ContactRoleEntity CreateContactRoleExample()
		{
			return this.CreateExample<ContactRoleEntity> ();
		}


		private ContactRoleEntity CreateContactRoleExampleByName(FormattedText name)
		{
			ContactRoleEntity example = this.CreateContactRoleExample ();
			example.Name = name;

			return example;
		}


	}


}
