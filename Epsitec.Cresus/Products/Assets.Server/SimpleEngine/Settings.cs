//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class Settings
	{
		public Settings()
		{
			this.assetsFields  = new List<UserField> ();
			this.personsFields = new List<UserField> ();

			this.objectFieldDict = new Dictionary<ObjectField, UserField> ();
			this.guidDict = new Dictionary<Guid, UserField> ();
		}


		public IEnumerable<UserField> GetUserFields(BaseType baseType)
		{
			return this.GetUserFieldsList (baseType);
		}

		public void AddUserField(BaseType baseType, UserField userField)
		{
			var list = this.GetUserFieldsList (baseType);
			list.Add (userField);

			this.Update ();
		}

		public void InsertUserField(BaseType baseType, int index, UserField userField)
		{
			var list = this.GetUserFieldsList (baseType);
			list.Insert (index, userField);

			this.Update ();
		}

		public void RemoveUserField(BaseType baseType, Guid guid)
		{
			var list = this.GetUserFieldsList (baseType);
			var index = list.FindIndex (x => x.Guid == guid);
			list.RemoveAt (index);

			this.Update ();
		}


		public FieldType GetUserFieldType(ObjectField field)
		{
			return this.GetUserField (field).Type;
		}

		public string GetUserFieldName(ObjectField field)
		{
			return this.GetUserField (field).Name;
		}

		public UserField GetUserField(ObjectField field)
		{
			UserField userField;

			if (this.objectFieldDict.TryGetValue (field, out userField))
			{
				return userField;
			}

			return UserField.Empty;
		}

		public UserField GetUserField(Guid guid)
		{
			UserField userField;

			if (this.guidDict.TryGetValue (guid, out userField))
			{
				return userField;
			}

			return UserField.Empty;
		}


		public ObjectField GetNewUserObjectField()
		{
			//	Retourne un nouveau champ utilisateur libre.
			if (this.objectFieldDict.Any ())
			{
				ObjectField max = this.objectFieldDict.Max (x => x.Key);

				if (max == ObjectField.UserFieldLast)
				{
					return ObjectField.Unknown;
				}
				else
				{
					return max+1;
				}
			}
			else
			{
				return ObjectField.UserFieldFirst;
			}
		}


		private void Update()
		{
			//	Doit �tre appel� apr�s avoir modifi� le contenu de AssetsField et/ou PersonsFields.
			this.objectFieldDict.Clear ();
			this.guidDict.Clear ();

			foreach (var field in this.assetsFields)
			{
				this.objectFieldDict.Add (field.Field, field);
				this.guidDict.Add (field.Guid, field);
			}

			foreach (var field in this.personsFields)
			{
				this.objectFieldDict.Add (field.Field, field);
				this.guidDict.Add (field.Guid, field);
			}
		}


		private List<UserField> GetUserFieldsList(BaseType baseType)
		{
			switch (baseType)
			{
				case BaseType.Assets:
					return this.assetsFields;

				case BaseType.Persons:
					return this.personsFields;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", baseType.ToString ()));
			}
		}


		private readonly List<UserField>		assetsFields;
		private readonly List<UserField>		personsFields;
		private readonly Dictionary<ObjectField, UserField> objectFieldDict;
		private readonly Dictionary<Guid, UserField> guidDict;
	}
}
