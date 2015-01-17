//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Donne accès aux rubriques de l'utilisateur définies pour les objets d'immobilisations
	/// (Assets) et pour les contacts (Persons) sous la forme de UserFields. Comme ces
	/// rubriques sont stockées dans des bases standards (BaseType.AssetsUserFields et
	/// BaseType.PersonsUserFields), il est bien plus aisé de les voir ainsi.
	/// </summary>
	public class UserFieldsAccessor
	{
		public UserFieldsAccessor(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public ObjectField GetMainStringField(BaseType baseType)
		{
			//	On considère comme rubrique "principale" la première rubrique textuelle.
			return this.GetUserFields (baseType)
				.Where (x => x.Type == FieldType.String)
				.Select (x => x.Field)
				.FirstOrDefault ();
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

		private UserField GetUserField(ObjectField field)
		{
			//	Retourne une rubrique utilisateur.
			var userField = this.GetUnsortedUserFields ().Where (x => x.Field == field).FirstOrDefault ();

			if (userField == null)
			{
				return UserField.Empty;
			}
			else
			{
				return userField;
			}
		}

		public UserField GetUserField(Guid guid)
		{
			//	Retourne une rubrique utilisateur.
			var userField = this.GetUnsortedUserFields ().Where (x => x.Guid == guid).FirstOrDefault ();

			if (userField == null)
			{
				return UserField.Empty;
			}
			else
			{
				return userField;
			}
		}


		public ObjectField GetNewUserField()
		{
			//	Retourne un nouveau champ utilisateur libre.
			if (this.accessor.Mandat.GetData (BaseType.AssetsUserFields ).Count ==  0 &&
				this.accessor.Mandat.GetData (BaseType.PersonsUserFields).Count ==  0)
			{
				return ObjectField.UserFieldFirst;
			}
			else
			{
				ObjectField max = this.GetUnsortedUserFields ().Max (x => x.Field);

				if (max == ObjectField.UserFieldLast)
				{
					return ObjectField.Unknown;
				}
				else
				{
					return max+1;
				}
			}
		}


		private IEnumerable<UserField> GetUnsortedUserFields()
		{
			//	Retourne la liste de toutes les rubriques utilisateur (des 2 bases).
			foreach (var obj in this.accessor.Mandat.GetData (BaseType.AssetsUserFields))
			{
				yield return this.GetUserField (obj);
			}

			foreach (var obj in this.accessor.Mandat.GetData (BaseType.PersonsUserFields))
			{
				yield return this.GetUserField (obj);
			}
		}

		public IEnumerable<UserField> GetUserFields(BaseType baseType)
		{
			//	Retourne la liste des rubriques utilisateur d'une base triée selon
			//	les numéros d'ordre (UserFieldOrder).
			return this.GetUnsortedUserFields (baseType).OrderBy (x => x.Order);
		}

		private IEnumerable<UserField> GetUnsortedUserFields(BaseType baseType)
		{
			//	Retourne la liste des rubriques utilisateur d'une base.
			switch (baseType.Kind)
			{
				case BaseTypeKind.AssetsUserFields:
					foreach (var obj in this.accessor.Mandat.GetData (BaseType.AssetsUserFields))
					{
						yield return this.GetUserField (obj);
					}
					break;

				case BaseTypeKind.PersonsUserFields:
					foreach (var obj in this.accessor.Mandat.GetData (BaseType.PersonsUserFields))
					{
						yield return this.GetUserField (obj);
					}
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", baseType));
			}
		}


		public void ChangeOrder(BaseType baseType, UserField userField = null, int order = -1)
		{
			//	Modifie l'ordre d'une rubrique utilisateur, ce qui implique de modifier
			//	l'ordre de toutes les rubriques.
			//	Si userField = null, on renumérote simplement tout.
			var list = this.GetUserFields (baseType)
				.Where (x => userField == null || x.Field != userField.Field)
				.ToArray ();  // tous les UserFields, sauf celui dont on change l'ordre

			int newOrder = 0;
			bool placed = false;

			foreach (var existingUserField in list)
			{
				if (userField != null && newOrder == order)  // est-ce que le UserField à modifier vient ici ?
				{
					this.RemoveUserField (baseType, userField.Guid);
					this.AddUserField (baseType, new UserField (userField, newOrder++));
					placed = true;
				}

				if (existingUserField.Order != newOrder)  // changement d'ordre ?
				{
					this.RemoveUserField (baseType, existingUserField.Guid);
					this.AddUserField (baseType, new UserField (existingUserField, newOrder));
				}

				newOrder++;
			}

			if (userField != null && !placed)
			{
				this.RemoveUserField (baseType, userField.Guid);
				this.AddUserField (baseType, new UserField (userField, newOrder++));
			}
		}

		public void AddUserField(BaseType baseType, UserField userField)
		{
			//	Ajoute une rubrique utilisateur.
			var obj = this.GetDataObject (userField);
			this.accessor.Mandat.GetData (baseType).Add (obj);
		}

		public void RemoveUserField(BaseType baseType, Guid guid)
		{
			//	Supprime une rubrique utilisateur.
			var obj = this.accessor.Mandat.GetData (baseType)[guid];
			this.accessor.Mandat.GetData (baseType).Remove (obj);
		}


		private DataObject GetDataObject(UserField userField)
		{
			//	Retourne l'objet à partir du UserField, sans changer son Guid.
			var obj = new DataObject (this.accessor.UndoManager, userField.Guid);
			var e = new DataEvent (this.accessor.UndoManager, Timestamp.MaxValue, EventType.Input);
			obj.AddEvent (e);

			var p0 = new DataIntProperty (ObjectField.UserFieldOrder, userField.Order);
			e.AddProperty (p0);

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

			var p9 = new DataIntProperty (ObjectField.UserFieldRequired, userField.Required ? 1:0);
			e.AddProperty (p9);

			return obj;
		}

		private UserField GetUserField(DataObject obj)
		{
			//	Retourne le UserField à partir d'un objet, sans changer son Guid.
			var e = obj.GetInputEvent ();
			System.Diagnostics.Debug.Assert (e != null);

			var p0 = e.GetProperty (ObjectField.UserFieldOrder       ) as DataIntProperty;
			var p1 = e.GetProperty (ObjectField.Name                 ) as DataStringProperty;
			var p2 = e.GetProperty (ObjectField.UserFieldType        ) as DataIntProperty;
			var p3 = e.GetProperty (ObjectField.UserFieldColumnWidth ) as DataIntProperty;
			var p4 = e.GetProperty (ObjectField.UserFieldLineWidth   ) as DataIntProperty;
			var p5 = e.GetProperty (ObjectField.UserFieldLineCount   ) as DataIntProperty;
			var p6 = e.GetProperty (ObjectField.UserFieldSummaryOrder) as DataIntProperty;
			var p7 = e.GetProperty (ObjectField.UserFieldTopMargin   ) as DataIntProperty;
			var p8 = e.GetProperty (ObjectField.UserFieldField       ) as DataIntProperty;
			var p9 = e.GetProperty (ObjectField.UserFieldRequired    ) as DataIntProperty;

			System.Diagnostics.Debug.Assert (p0 != null);
			System.Diagnostics.Debug.Assert (p1 != null);
			System.Diagnostics.Debug.Assert (p2 != null);
			System.Diagnostics.Debug.Assert (p3 != null);
			System.Diagnostics.Debug.Assert (p7 != null);
			System.Diagnostics.Debug.Assert (p8 != null);
			System.Diagnostics.Debug.Assert (p9 != null);

			var order       =               p0.Value;
			var name        =               p1.Value;
			var type        = (FieldType)   p2.Value;
			var columnWidth =               p3.Value;
			var topMargin   =               p7.Value;
			var field       = (ObjectField) p8.Value;
			var required    =               p9.Value != 0;

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

			return new UserField (obj.Guid, order, name, field, type, required, columnWidth, lineWidth, lineCount, summaryOrder, topMargin);
		}


		private readonly DataAccessor			accessor;
	}
}
