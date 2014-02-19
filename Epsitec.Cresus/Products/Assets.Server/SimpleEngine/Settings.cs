//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public DataObject GetTempDataObject(Guid guid)
		{
			//	Retourne un objet temporaire, pour permettre d'accèder à une rubrique
			//	utilisateur dans la fausse "base de données" BaseType.UserFields.
			if (guid.IsEmpty)
			{
				return null;
			}

			var userField = this.GetUserField (guid);
			if (userField.IsEmpty)
			{
				throw new System.InvalidOperationException ("GetTempDataObject: Guid does not exist");
			}

			var obj = new DataObject ();
			var e = new DataEvent (Timestamp.MaxValue, EventType.Input);
			obj.AddEvent (e);

			var p1 = new DataStringProperty (ObjectField.Name,           userField.Name);
			var p2 = new DataIntProperty    (ObjectField.UserFieldType,  (int) userField.Type);
			var p3 = new DataIntProperty    (ObjectField.UserFieldMax,   userField.MaxLength);
			var p4 = new DataIntProperty    (ObjectField.UserFieldField, (int) userField.Field);
			var p5 = new DataGuidProperty   (ObjectField.UserFieldGuid,  guid);

			e.AddProperty (p1);
			e.AddProperty (p2);
			e.AddProperty (p3);
			e.AddProperty (p4);
			e.AddProperty (p5);

			return obj;
		}

		public void SetTempDataObject(DataObject obj)
		{
			var e = obj.GetEvent (0);
			System.Diagnostics.Debug.Assert (e != null);

			var p1 = e.GetProperty (ObjectField.Name          ) as DataStringProperty;
			var p2 = e.GetProperty (ObjectField.UserFieldType ) as DataIntProperty;
			var p3 = e.GetProperty (ObjectField.UserFieldMax  ) as DataIntProperty;
			var p4 = e.GetProperty (ObjectField.UserFieldField) as DataIntProperty;
			var p5 = e.GetProperty (ObjectField.UserFieldGuid ) as DataGuidProperty;

			System.Diagnostics.Debug.Assert (p1 != null);
			System.Diagnostics.Debug.Assert (p2 != null);
			System.Diagnostics.Debug.Assert (p3 != null);
			System.Diagnostics.Debug.Assert (p4 != null);
			System.Diagnostics.Debug.Assert (p5 != null);

			var name  =               p1.Value;
			var type  = (FieldType)   p2.Value;
			var max   =               p3.Value;
			var field = (ObjectField) p4.Value;
			var guid  =               p5.Value;

			var baseType = this.GetBaseType (guid);
			var index = this.GetIndex (baseType, guid);

			this.RemoveUserField (guid);

			var userField = new UserField (guid, name, field, type, max);
			this.InsertUserField (baseType, index, userField);

			this.Update ();
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

		public int GetIndex(BaseType baseType, Guid guid)
		{
			var list = this.GetUserFieldsList (baseType);
			int index = list.FindIndex (x => x.Guid == guid);
			System.Diagnostics.Debug.Assert (index != -1);
			return index;
		}

		public BaseType GetBaseType(Guid guid)
		{
			foreach (var baseType in Settings.BaseTypes)
			{
				var list = this.GetUserFieldsList (baseType);
				var index = list.FindIndex (x => x.Guid == guid);

				if (index != -1)
				{
					return baseType;
				}
			}

			throw new System.InvalidOperationException ("GetBaseType: Unknown Guid");
		}

		public BaseType RemoveUserField(Guid guid)
		{
			foreach (var baseType in Settings.BaseTypes)
			{
				var list = this.GetUserFieldsList (baseType);
				var index = list.FindIndex (x => x.Guid == guid);

				if (index != -1)
				{
					list.RemoveAt (index);
					this.Update ();

					return baseType;
				}
			}

			throw new System.InvalidOperationException ("RemoveUserField: Unknown Guid");
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
			//	Doit être appelé après avoir modifié le contenu de AssetsField et/ou PersonsFields.
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

		private static IEnumerable<BaseType> BaseTypes
		{
			get
			{
				yield return BaseType.Assets;
				yield return BaseType.Persons;
			}
		}


		private readonly List<UserField>		assetsFields;
		private readonly List<UserField>		personsFields;
		private readonly Dictionary<ObjectField, UserField> objectFieldDict;
		private readonly Dictionary<Guid, UserField> guidDict;
	}
}
