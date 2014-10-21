//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Data
{
	public class GlobalSettings
	{
		public GlobalSettings()
		{
			this.assetsFields  = new List<UserField> ();
			this.personsFields = new List<UserField> ();

			this.objectFieldDict = new Dictionary<ObjectField, UserField> ();
			this.guidDict        = new Dictionary<Guid, UserField> ();

			this.CopyNameStrategy = CopyNameStrategy.NameBracketCopy;
		}


		public CopyNameStrategy					CopyNameStrategy;


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
				throw new System.InvalidOperationException ("Guid does not exist");
			}

			var obj = new DataObject (null);
			var e = new DataEvent (null, Timestamp.MaxValue, EventType.Input);
			obj.AddEvent (e);

			var p1 = new DataStringProperty (ObjectField.Name, userField.Name);
			e.AddProperty (p1);

			var p2 = new DataIntProperty (ObjectField.UserFieldType, (int) userField.Type);
			e.AddProperty (p2);

			var p3 = new DataIntProperty (ObjectField.UserFieldColumnWidth, userField.ColumnWidth);
			e.AddProperty (p3);

			if (userField.LineWidth.HasValue)
			{
				var p4 = new DataIntProperty (ObjectField.UserFieldLineWidth, userField.LineWidth.Value);
				e.AddProperty (p4);
			}

			if (userField.LineCount.HasValue)
			{
				var p5 = new DataIntProperty (ObjectField.UserFieldLineCount, userField.LineCount.Value);
				e.AddProperty (p5);
			}

			if (userField.SummaryOrder.HasValue)
			{
				var p6 = new DataIntProperty (ObjectField.UserFieldSummaryOrder, userField.SummaryOrder.Value);
				e.AddProperty (p6);
			}

			var p7 = new DataIntProperty (ObjectField.UserFieldTopMargin, userField.TopMargin);
			e.AddProperty (p7);

			var p8 = new DataIntProperty (ObjectField.UserFieldField, (int) userField.Field);
			e.AddProperty (p8);

			var p9 = new DataGuidProperty (ObjectField.UserFieldGuid, guid);
			e.AddProperty (p9);

			var p10 = new DataIntProperty (ObjectField.UserFieldRequired, userField.Required ? 1:0);
			e.AddProperty (p10);

			return obj;
		}

		public void SetTempDataObject(DataObject obj)
		{
			//	Réinjecte l'objet temporaire dans les définitions des rubriques utilisateur.
			var e = obj.GetEvent (0);
			System.Diagnostics.Debug.Assert (e != null);

			var p1  = e.GetProperty (ObjectField.Name                 ) as DataStringProperty;
			var p2  = e.GetProperty (ObjectField.UserFieldType        ) as DataIntProperty;
			var p3  = e.GetProperty (ObjectField.UserFieldColumnWidth ) as DataIntProperty;
			var p4  = e.GetProperty (ObjectField.UserFieldLineWidth   ) as DataIntProperty;
			var p5  = e.GetProperty (ObjectField.UserFieldLineCount   ) as DataIntProperty;
			var p6  = e.GetProperty (ObjectField.UserFieldSummaryOrder) as DataIntProperty;
			var p7  = e.GetProperty (ObjectField.UserFieldTopMargin   ) as DataIntProperty;
			var p8  = e.GetProperty (ObjectField.UserFieldField       ) as DataIntProperty;
			var p9  = e.GetProperty (ObjectField.UserFieldGuid        ) as DataGuidProperty;
			var p10 = e.GetProperty (ObjectField.UserFieldRequired    ) as DataIntProperty;

			System.Diagnostics.Debug.Assert (p1  != null);
			System.Diagnostics.Debug.Assert (p2  != null);
			System.Diagnostics.Debug.Assert (p3  != null);
			System.Diagnostics.Debug.Assert (p7  != null);
			System.Diagnostics.Debug.Assert (p8  != null);
			System.Diagnostics.Debug.Assert (p9  != null);
			System.Diagnostics.Debug.Assert (p10 != null);

			var name        =               p1.Value;
			var type        = (FieldType)   p2.Value;
			var columnWidth =               p3.Value;
			var topMargin   =               p7.Value;
			var field       = (ObjectField) p8.Value;
			var guid        =               p9.Value;
			var required    =               p10.Value != 0;

			int? lineWidth = null;
			if (p4 != null && type == FieldType.String)
			{
				lineWidth = p4.Value;
			}

			int? lineCount = null;
			if (p5 != null && type == FieldType.String)
			{
				lineCount = p5.Value;
			}

			int? summaryOrder = null;
			if (p6 != null && type == FieldType.String)
			{
				summaryOrder = p6.Value;
			}

			var baseType = this.GetBaseType (guid);
			var index = this.GetIndex (baseType, guid);

			//	Supprime l'ancienne rubrique utilisateur.
			this.RemoveUserField (guid);

			//	Recrée la nouvelle rubrique utilisateur, au même emplacement et sans
			//	modifier son Guid.
			var userField = new UserField (guid, name, field, type, required, columnWidth, lineWidth, lineCount, summaryOrder, topMargin);
			this.InsertUserField (baseType, index, userField);
		}


		public ObjectField GetMainStringField(BaseType baseType)
		{
			return this.GetUserFields (baseType)
				.Where (x => x.Type == FieldType.String)
				.Select (x => x.Field)
				.FirstOrDefault ();
		}


		public IEnumerable<UserField> GetUserFields(BaseType baseType)
		{
			//	Retourne la liste des rubriques utilisateur.
			return this.GetUserFieldsList (baseType);
		}

		public void AddUserField(BaseType baseType, UserField userField)
		{
			//	Ajoute une rubrique utilisateur à la fin de la liste.
			var list = this.GetUserFieldsList (baseType);
			list.Add (userField);

			this.Update ();
		}

		public void InsertUserField(BaseType baseType, int index, UserField userField)
		{
			//	Ajoute une rubrique utilisateur à l'index choisi.
			var list = this.GetUserFieldsList (baseType);
			list.Insert (index, userField);

			this.Update ();
		}

		private int GetIndex(BaseType baseType, Guid guid)
		{
			//	Retourne l'index d'une rubrique utilisateur.
			var list = this.GetUserFieldsList (baseType);
			int index = list.FindIndex (x => x.Guid == guid);
			System.Diagnostics.Debug.Assert (index != -1);
			return index;
		}

		public BaseType GetBaseType(Guid guid)
		{
			//	Retourne la base d'une rubrique utilisateur.
			foreach (var baseType in GlobalSettings.BaseTypes)
			{
				var list = this.GetUserFieldsList (baseType);
				var index = list.FindIndex (x => x.Guid == guid);

				if (index != -1)
				{
					return baseType;
				}
			}

			throw new System.InvalidOperationException ("Unknown Guid");
		}

		public BaseType RemoveUserField(Guid guid)
		{
			//	Supprime une rubrique utilisateur.
			foreach (var baseType in GlobalSettings.BaseTypes)
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

			throw new System.InvalidOperationException ("Unknown Guid");
		}


		public FieldType GetUserFieldType(ObjectField field)
		{
			//	Retourne le type d'une rubrique utilisateur.
			return this.GetUserField (field).Type;
		}

		public string GetUserFieldName(ObjectField field)
		{
			//	Retourne le nom d'une rubrique utilisateur.
			return this.GetUserField (field).Name;
		}

		public UserField GetUserField(ObjectField field)
		{
			//	Retourne une rubrique utilisateur.
			UserField userField;

			if (this.objectFieldDict.TryGetValue (field, out userField))
			{
				return userField;
			}

			return UserField.Empty;
		}

		public UserField GetUserField(Guid guid)
		{
			//	Retourne une rubrique utilisateur.
			UserField userField;

			if (this.guidDict.TryGetValue (guid, out userField))
			{
				return userField;
			}

			return UserField.Empty;
		}


		public ObjectField GetNewUserField()
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
			//	Met à jour les dictionnaires après une modification d'une rubrique utilisateur.
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
			//	Retourne la liste des rubriques utilisateur d'une base.
			switch (baseType.Kind)
			{
				case BaseTypeKind.AssetsUserFields:
					return this.assetsFields;

				case BaseTypeKind.PersonsUserFields:
					return this.personsFields;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", baseType));
			}
		}

		private static IEnumerable<BaseType> BaseTypes
		{
			//	Retourne les bases contenant des rubriques utilisateur.
			get
			{
				yield return BaseType.AssetsUserFields;
				yield return BaseType.PersonsUserFields;
			}
		}


		private readonly List<UserField>					assetsFields;
		private readonly List<UserField>					personsFields;
		private readonly Dictionary<ObjectField, UserField>	objectFieldDict;
		private readonly Dictionary<Guid, UserField>		guidDict;
	}
}
