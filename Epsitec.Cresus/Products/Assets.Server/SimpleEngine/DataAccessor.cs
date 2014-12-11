//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Expression;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public partial class DataAccessor
	{
		public DataAccessor(ComputerSettings computerSettings, DataClipboard clipboard)
		{
			this.computerSettings = computerSettings;
			this.clipboard        = clipboard;

			this.userFieldsAccessor      = new UserFieldsAccessor (this);
			this.editionAccessor         = new EditionAccessor (this);
			this.amortizationExpressions = new AmortizationExpressions ();

			this.cleanerAgents = new List<AbstractCleanerAgent> ();
		}

		public DataMandat						Mandat
		{
			get
			{
				return this.mandat;
			}
			set
			{
				this.mandat = value;

				this.WarningsDirty = true;

				//	Recalcule tout.
				foreach (var obj in this.mandat.GetData (BaseType.Assets))
				{
					Amortizations.UpdateAmounts (this, obj);
				}
			}
		}

		public static int						Simulation;

		public DataClipboard					Clipboard
		{
			get
			{
				return this.clipboard;
			}
		}

		public UserFieldsAccessor				UserFieldsAccessor
		{
			get
			{
				return this.userFieldsAccessor;
			}
		}

		public ComputerSettings					ComputerSettings
		{
			get
			{
				return this.computerSettings;
			}
		}

		public GlobalSettings					GlobalSettings
		{
			get
			{
				return this.mandat.GlobalSettings;
			}
		}

		public System.DateTime					StartDate
		{
			get
			{
				return this.mandat.StartDate;
			}
		}

		public EditionAccessor					EditionAccessor
		{
			get
			{
				return this.editionAccessor;
			}
		}

		public bool								WarningsDirty;

		public List<AbstractCleanerAgent>		CleanerAgents
		{
			get
			{
				return this.cleanerAgents;
			}
		}

		public UndoManager						UndoManager
		{
			get
			{
				return this.mandat.UndoManager;
			}
		}

		public AmortizationExpressions			AmortizationExpressions
		{
			get
			{
				return this.amortizationExpressions;
			}
		}

		public GuidNodeGetter GetNodeGetter(BaseType baseType)
		{
			//	Retourne un moyen standardisé d'accès en lecture aux données d'une base.
			return new GuidNodeGetter (this.mandat, baseType);
		}


		#region Objects
		public DataObject GetObject(BaseType baseType, Guid objectGuid)
		{
			return this.mandat.GetData (baseType)[objectGuid];
		}

		public Guid CreateObject(BaseType baseType, System.DateTime date, string name, Guid parent)
		{
			//	Ajoute le nom de l'objet.
			var field = this.GetMainStringField (baseType);
			var p = new DataStringProperty (field, name);

			return this.CreateObject (baseType, date, parent, p);
		}

		public Guid CreateObject(BaseType baseType, System.DateTime date, Guid parent, params AbstractDataProperty[] requiredProperties)
		{
			var obj = new DataObject (this.UndoManager);
			this.mandat.GetData (baseType).Add (obj);

			var timestamp = new Timestamp (date, 0);
			var e = new DataEvent (this.UndoManager, timestamp, EventType.Input);
			obj.AddEvent (e);

			//	Ajoute la date de l'opération.
			this.AddDateOperation (e);

			//	Ajoute l'objet parent.
			if (!parent.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, parent));
			}

			//	Ajoute les propriétés requises.
			foreach (var property in requiredProperties)
			{
				e.AddProperty (property);
			}

			//	Ajoute la valeur comptable.
			if (baseType == BaseType.Assets)
			{
				this.AddMainValue (obj, timestamp, e);
			}

			this.WarningsDirty = true;

			return obj.Guid;
		}

		public Timestamp ChangeAssetEventTimestamp(DataObject obj, DataEvent e, System.DateTime date)
		{
			int position = 0;
			var sameDates = obj.Events.Where (x => x.Timestamp.Date == date).ToArray ();

			if (sameDates.Length != 0)  // y a-t-il d'autres événements à la même date ?
			{
				if (date < e.Timestamp.Date)  // recule l'événement ?
				{
					//	Si on recule l'événement à une date contenant déjà d'autres événements,
					//	il devra venir après le dernier.
					var last = sameDates.Max (x => x.Timestamp);
					position = last.Position + 1;
				}
				else  // avance l'événement ?
				{
					//	Si on avance l'événement à une date contenant déjà d'autres événements,
					//	il faut "pousser" la position de ceux-ci.
					foreach (var s in sameDates)
					{
						var t = new Timestamp (date, s.Timestamp.Position + 1);
						obj.ChangeEventTimestamp (s, t);
					}
				}
			}

			var timestamp = new Timestamp (date, position);
			obj.ChangeEventTimestamp (e, timestamp);

			return timestamp;
		}

		public DataEvent CreateAssetEvent(DataObject obj, System.DateTime date, EventType type)
		{
			if (obj != null)
			{
				if (type == EventType.Locked)
				{
					//	Il ne peut y avoir qu'un seul verrou par objet.
					AssetCalculator.RemoveLockedEvent (obj);
				}

				var ts = obj.GetNewTimestamp (date);
				var e = new DataEvent (this.UndoManager, ts, type);
				obj.AddEvent (e);

				this.AddDateOperation (e);
				this.AddMainValue (obj, ts, e);

				Amortizations.UpdateAmounts (this, obj);
				return e;
			}

			return null;
		}

		private void AddDateOperation(DataEvent e)
		{
			//	Ajoute la date du jour comme date valeur, ainsi que l'utilisateur courant.
			var dp = new DataDateProperty (ObjectField.OneShotDateOperation, Timestamp.Now.Date);
			e.AddProperty (dp);

			var up = new DataStringProperty (ObjectField.OneShotUser, DataIO.CurrentUser);
			e.AddProperty (up);
		}

		private void AddMainValue(DataObject obj, Timestamp timestamp, DataEvent e)
		{
			//	La valeur comptable est créée une bonne fois pour toutes.
			//	On l'initialise avec les paramètres d'amortissement.
			if (e.Type == EventType.Modification ||
				e.Type == EventType.Locked)
			{
				//	Pour ces seuls types, il n'y a pas et n'y aura jamais de valeur comptable.
				return;
			}

			//	Cherche la valeur imposée à utiliser.
			decimal? forcedAmount = null;

			if (e.Type == EventType.Output)
			{
				//	Il est bien pratique de mettre tout de suite une valeur comptable nulle
				//	lors de la création d'un événement de sortie.
				forcedAmount = 0.0m;
			}
			else if (e.Type == EventType.Increase ||
					 e.Type == EventType.Decrease ||
					 e.Type == EventType.Adjust)
			{
				//	Il est bien pratique de mettre tout de suite la valeur actuelle lors de
				//	la création d'un événement qui modifie la valeur.
				var currentAmount = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, ObjectField.MainValue);
				if (currentAmount.HasValue)
				{
					forcedAmount = currentAmount.Value.FinalAmount;
				}
			}

			//	Cherche le scénario d'écriture à utiliser.
			var entryScenario = EntryScenario.None;

			switch (e.Type)
			{
				case EventType.Input:
					entryScenario = EntryScenario.Purchase;
					break;

				case EventType.Increase:
					entryScenario = EntryScenario.Increase;
					break;

				case EventType.Decrease:
					entryScenario = EntryScenario.Decrease;
					break;

				case EventType.Adjust:
					entryScenario = EntryScenario.Adjust;
					break;

				case EventType.AmortizationAuto:
				case EventType.AmortizationPreview:
					entryScenario = EntryScenario.AmortizationAuto;
					break;

				case EventType.AmortizationExtra:
					entryScenario = EntryScenario.AmortizationExtra;
					break;

				case EventType.Output:
					entryScenario = EntryScenario.Sale;
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown EventType {0}", e.Type.ToString ()));
			}

			//	Crée la propriété.
			var aa = new AmortizedAmount (null, null, null, entryScenario, Guid.Empty, 0);
			Amortizations.SetAmortizedAmount (e, aa);
		}


		public void RemoveObject(BaseType baseType, Guid objectGuid)
		{
			var obj = this.GetObject (baseType, objectGuid);
			this.RemoveObject (baseType, obj);
		}

		public void RemoveObject(BaseType baseType, DataObject obj)
		{
			if (obj != null)
			{
				//	Appelle tous les agents de nettoyage enregistrés.
				this.cleanerAgents.ForEach (x => x.Clean (baseType, obj.Guid));

				if (baseType == BaseType.Assets)
				{
					//	Supprime tous les événements, ce qui est nécessaire pour
					//	supprimer proprement toutes les écritures liées.
					while (obj.EventsAny)
					{
						var e = obj.Events.First ();
						this.RemoveObjectEvent (obj, e);
					}
				}

				var list = this.mandat.GetData (baseType);
				list.Remove (obj);

				this.WarningsDirty = true;
			}
		}

		public void RemoveObjectEvent(DataObject obj, Timestamp? timestamp)
		{
			if (obj != null && timestamp.HasValue)
			{
				var e = obj.GetEvent (timestamp.Value);
				if (e != null)
				{
					this.RemoveObjectEvent (obj, e);
				}
			}
		}

		public void RemoveObjectEvent(DataObject obj, DataEvent e)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			if (p != null)
			{
				var aa = Entries.RemoveEntry(this, obj, e, p.Value);
				Amortizations.SetAmortizedAmount (e, aa);
			}

			obj.RemoveEvent (e);
			Amortizations.UpdateAmounts (this, obj);
		}


		public void CopyObject(DataObject obj, DataObject model)
		{
			//	Copie dans 'obj' toutes les propriétés de 'model' que 'obj' n'a pas déjà.
			//	Ne fonctionne que pour les objets sans timeline (donc tous sauf les Assets),
			//	c'est-à-dire les objets qui n'ont qu'un seul événement.
			System.Diagnostics.Debug.Assert (  obj.EventsCount == 1);
			System.Diagnostics.Debug.Assert (model.EventsCount == 1);

			var dstEvent =   obj.GetInputEvent ();  // événement unique de l'objet
			var srcEvent = model.GetInputEvent ();  // événement unique de l'objet

			dstEvent.SetUndefinedProperties (srcEvent);
		}
		#endregion


		public ObjectField GetMainStringField(BaseType baseType)
		{
			if (baseType == BaseType.Assets)
			{
				return this.userFieldsAccessor.GetMainStringField (BaseType.AssetsUserFields);
			}
			else if (baseType == BaseType.Persons)
			{
				return this.userFieldsAccessor.GetMainStringField (BaseType.PersonsUserFields);
			}
			else
			{
				return ObjectField.Name;
			}
		}


		public static IEnumerable<ObjectField> GroupGuidRatioFields
		{
			get
			{
				for (int i=0; i<=ObjectField.GroupGuidRatioLast-ObjectField.GroupGuidRatioFirst; i++)
				{
					yield return ObjectField.GroupGuidRatioFirst+i;
				}
			}
		}

		public static IEnumerable<ObjectField> ArgumentFields
		{
			get
			{
				for (int i=0; i<=ObjectField.ArgumentLast-ObjectField.ArgumentFirst; i++)
				{
					yield return ObjectField.ArgumentFirst+i;
				}
			}
		}

		public IEnumerable<ObjectField> AssetFields
		{
			//	Retourne tous les champs pouvant potentiellement faire partie d'un objet d'immobilisation.
			get
			{
				yield return ObjectField.MainValue;

				foreach (var field in this.userFieldsAccessor.GetUserFields (BaseType.AssetsUserFields).Select (x => x.Field))
				{
					yield return field;
				}

				for (var field = ObjectField.GroupGuidRatioFirst; field <= ObjectField.GroupGuidRatioLast; field++)
				{
					yield return field;
				}

				yield return ObjectField.CategoryName;
				yield return ObjectField.MethodGuid;
				yield return ObjectField.Periodicity;

				foreach (var field in DataAccessor.AccountFields)
				{
					yield return field;
				}
			}
		}

		public IEnumerable<ObjectField> AssetValueFields
		{
			//	Retourne tous les champs d'un objet d'immobilisation contenant un montant.
			get
			{
				yield return ObjectField.MainValue;

				foreach (var field in this.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields)
					.Where (x => x.Type == FieldType.ComputedAmount)
					.Select (x => x.Field))
				{
					yield return field;
				}
			}
		}

		public static IEnumerable<ObjectField> AccountFields
		{
			get
			{
				yield return ObjectField.AccountPurchaseDebit;
				yield return ObjectField.AccountPurchaseCredit;
				yield return ObjectField.AccountSaleDebit;
				yield return ObjectField.AccountSaleCredit;
				yield return ObjectField.AccountAmortizationAutoDebit;
				yield return ObjectField.AccountAmortizationAutoCredit;
				yield return ObjectField.AccountAmortizationExtraDebit;
				yield return ObjectField.AccountAmortizationExtraCredit;
				yield return ObjectField.AccountIncreaseDebit;
				yield return ObjectField.AccountIncreaseCredit;
				yield return ObjectField.AccountDecreaseDebit;
				yield return ObjectField.AccountDecreaseCredit;
				yield return ObjectField.AccountAdjustDebit;
				yield return ObjectField.AccountAdjustCredit;
			}
		}

		public static IEnumerable<ObjectField> MethodFields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Expression;
			}
		}

		public string GetFieldName(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.UserFieldsAccessor.GetUserFieldName (objectField);
			}

			if (objectField >= ObjectField.ArgumentFirst &&
				objectField <= ObjectField.ArgumentLast)
			{
				var argument = ArgumentsLogic.GetArgument (this, objectField);
				return ArgumentsLogic.GetShortName (argument);
			}

			return DataDescriptions.GetObjectFieldDescription (objectField);
		}

		public FieldType GetFieldType(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.UserFieldsAccessor.GetUserFieldType (objectField);
			}

			if (objectField >= ObjectField.GroupGuidRatioFirst &&
				objectField <= ObjectField.GroupGuidRatioLast)
			{
				return FieldType.GuidRatio;
			}

			if (objectField >= ObjectField.ArgumentFirst &&
				objectField <= ObjectField.ArgumentLast)
			{
				var type = ArgumentsLogic.GetArgumentType (this, objectField);
				switch (type)
				{
					case ArgumentType.Decimal:
					case ArgumentType.Amount:
					case ArgumentType.Rate:
						return FieldType.Decimal;

					case ArgumentType.Int:
					case ArgumentType.Bool:
						return FieldType.Int;

					case ArgumentType.Date:
						return FieldType.Date;

					case ArgumentType.String:
						return FieldType.String;

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown ArgumentType {0}", type.ToString ()));
				}
			}

			if (DataAccessor.AccountFields.Where (x => x == objectField).Any ())
			{
				return FieldType.Account;
			}

			switch (objectField)
			{
				case ObjectField.MainValue:
					return FieldType.AmortizedAmount;

				case ObjectField.EntryAmount:
					return FieldType.Decimal;

				case ObjectField.Periodicity:
				case ObjectField.ArgumentType:
				case ObjectField.ArgumentNullable:
					return FieldType.Int;

				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotDateEvent:
				case ObjectField.EntryDate:
					return FieldType.Date;

				case ObjectField.GroupParent:
					return FieldType.GuidGroup;

				case ObjectField.EntryDebitAccount:
				case ObjectField.EntryCreditAccount:
					return FieldType.Account;

				case ObjectField.MethodGuid:
					return FieldType.GuidMethod;

				default:
					return FieldType.String;
			}
		}

		public DecimalFormat GetFieldFormat(ObjectField objectField)
		{
			if (objectField >= ObjectField.ArgumentFirst &&
				objectField <= ObjectField.ArgumentLast)
			{
				var type = ArgumentsLogic.GetArgumentType (this, objectField);
				switch (type)
				{
					case ArgumentType.Decimal:
						return DecimalFormat.Real;

					case ArgumentType.Amount:
						return DecimalFormat.Amount;

					case ArgumentType.Rate:
						return DecimalFormat.Rate;

					default:
						return DecimalFormat.Real;
				}
			}

			switch (objectField)
			{
				case ObjectField.MainValue:
				case ObjectField.EntryAmount:
					return DecimalFormat.Amount;

				default:
					return DecimalFormat.Real;
			}
		}


		private readonly ComputerSettings		computerSettings;
		private readonly DataClipboard			clipboard;
		private readonly UserFieldsAccessor		userFieldsAccessor;
		private readonly EditionAccessor		editionAccessor;
		private readonly AmortizationExpressions amortizationExpressions;
		private readonly List<AbstractCleanerAgent> cleanerAgents;

		private DataMandat						mandat;
	}
}
