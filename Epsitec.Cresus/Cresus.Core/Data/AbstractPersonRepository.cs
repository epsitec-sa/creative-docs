using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{

	
	public class AbstractPersonRepository : Repository
	{


		public AbstractPersonRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example, index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAllAbstractPersons()
		{
			return this.GetAllGenericPersons<AbstractPersonEntity> ();
		}


		public IEnumerable<AbstractPersonEntity> GetAllAbstractPersons(int index, int count)
		{
			return this.GetAllGenericPersons<AbstractPersonEntity> (index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<AbstractPersonEntity> (preferredLanguage);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByPreferredLanguage(LanguageEntity preferredLanguage, int index, int count)
		{
			return this.GetGenericPersonsByPreferredLanguage<AbstractPersonEntity> (preferredLanguage, index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<AbstractPersonEntity> (contacts);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByContacts(int index, int count, params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<AbstractPersonEntity> (index, count, contacts);
		}


		public AbstractPersonEntity CreateAbstractPersonExample()
		{
			return this.CreateGenericPersonExample<AbstractPersonEntity> ();
		}


		protected IEnumerable<EntityType> GetGenericPersonsByExample<EntityType>(EntityType example) where EntityType : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericPersonsByExample<EntityType>(EntityType example, int index, int count) where EntityType : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<EntityType> (example, index, count);
		}


		protected IEnumerable<EntityType> GetAllGenericPersons<EntityType>() where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExample<EntityType> ();

			return this.GetGenericPersonsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetAllGenericPersons<EntityType>(int index, int count) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExample<EntityType> ();

			return this.GetGenericPersonsByExample<EntityType> (example, index, count);
		}


		protected IEnumerable<EntityType> GetGenericPersonsByPreferredLanguage<EntityType>(LanguageEntity preferredLanguage) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByPreferredLanguage<EntityType> (preferredLanguage);

			return this.GetGenericPersonsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericPersonsByPreferredLanguage<EntityType>(LanguageEntity preferredLanguage, int index, int count) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByPreferredLanguage<EntityType> (preferredLanguage);

			return this.GetGenericPersonsByExample<EntityType> (example, index, count);
		}


		protected IEnumerable<EntityType> GetGenericPersonsByContacts<EntityType>(params AbstractContactEntity[] contacts) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByContacts<EntityType> (contacts);

			return this.GetGenericPersonsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericPersonsByContacts<EntityType>(int index, int count, params AbstractContactEntity[] contacts) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByContacts<EntityType> (contacts);

			return this.GetGenericPersonsByExample<EntityType> (example, index, count);
		}


		protected EntityType CreateGenericPersonExample<EntityType>() where EntityType : AbstractPersonEntity, new ()
		{
			return this.CreateExample<EntityType> ();
		}


		protected EntityType CreateGenericPersonExampleByPreferredLanguage<EntityType>(LanguageEntity preferredLanguage) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExample<EntityType> ();
			example.PreferredLanguage = preferredLanguage;

			return example;
		}


		protected EntityType CreateGenericPersonExampleByContacts<EntityType>(IEnumerable<AbstractContactEntity> contacts) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExample<EntityType> ();

			foreach (AbstractContactEntity contact in contacts)
			{
				example.Contacts.Add (contact);
			}

			return example;
		}


	}


}
