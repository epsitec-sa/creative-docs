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


		public AbstractPersonEntity GetAbstractPersonByExample(AbstractPersonEntity example)
		{
			return this.GetGenericPersonByExample<AbstractPersonEntity> (example);
		}


		public IEnumerable<AbstractPersonEntity> GetAllAbstractPersons()
		{
			return this.GetAllGenericPersons<AbstractPersonEntity> ();
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonByPreferredLanguage<AbstractPersonEntity> (preferredLanguage);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonByContacts<AbstractPersonEntity> (contacts);
		}


		public AbstractPersonEntity CreateAbstractPersonExample()
		{
			return this.CreateGenericPersonExample<AbstractPersonEntity> ();
		}


		protected IEnumerable<EntityType> GetGenericPersonsByExample<EntityType>(EntityType example) where EntityType : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<EntityType> (example);
		}


		protected EntityType GetGenericPersonByExample<EntityType>(EntityType example) where EntityType : AbstractPersonEntity
		{
			return this.GetEntityByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetAllGenericPersons<EntityType>() where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExample<EntityType> ();

			return this.GetGenericPersonsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericPersonByPreferredLanguage<EntityType>(LanguageEntity preferredLanguage) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByPreferredLanguage<EntityType> (preferredLanguage);

			return this.GetGenericPersonsByExample<EntityType> (example);
		}


		protected IEnumerable<EntityType> GetGenericPersonByContacts<EntityType>(params AbstractContactEntity[] contacts) where EntityType : AbstractPersonEntity, new ()
		{
			EntityType example = this.CreateGenericPersonExampleByContacts<EntityType> (contacts);

			return this.GetGenericPersonsByExample<EntityType> (example);
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
