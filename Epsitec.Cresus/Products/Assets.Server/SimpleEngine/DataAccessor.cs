//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public partial class DataAccessor
	{
		public DataAccessor(DataClipboard clipboard)
		{
			this.clipboard = clipboard;

			this.userFieldsAccessor = new UserFieldsAccessor (this);
			this.editionAccessor    = new EditionAccessor (this);

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

		public INodeGetter<GuidNode> GetNodeGetter(BaseType baseType)
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
			//	Déplace un événement à une autre date, mais sans jamais modifier l'ordre des événements.
			//	Retourne le nouveau timestamp de l'événement.
			//	Exemple:
			//	01.01.2011.0
			//	01.01.2012.0
			//	01.01.2013.0 -> événement à déplacer
			//	01.01.2014.0
			//	01.01.2015.0
			//	L'événement à déplacer doit être compris entre le 01.01.2012 et le 01.01.2014. En
			//	d'autres termes, il doit rester le troisième dans la liste !
			int i = obj.GetIndex (e);
			System.Diagnostics.Debug.Assert (i != -1);

			int position = 0;

			//	Si on recule l'événement à une date contenant déjà d'autres événements,
			//	il devra venir après le dernier.
			//	Exemple:
			//	01.01.2014.0
			//	01.01.2014.1
			//	01.03.2014.0 -> à déplacer le 01.01.2014
			//	Résultat:
			//	01.01.2014.0
			//	01.01.2014.1
			//	01.03.2014.2 <- déplacé après le dernier événement du 01.01.2014
			if (i > 0)
			{
				var prev = obj.GetEvent (i-1);  // événement précédent
				if (prev.Timestamp.Date == date)
				{
					position = prev.Timestamp.Position + 1;
				}
			}

			//	Si on avance l'événement à une date contenant déjà d'autres événements,
			//	il faut "pousser" la position de ceux-ci.
			//	Exemple:
			//	01.01.2014.0 -> à déplacer le 01.03.2014
			//	01.03.2014.0
			//	01.03.2014.1
			//	01.04.2014.0
			//	Résultat:
			//	01.03.2014.0 <- déplacé avant le premier événement du 01.03.2014
			//	01.03.2014.1 <- position passée de 0 à 1
			//	01.03.2014.2 <- position passée de 1 à 2
			//	01.04.2014.0 <- inchangé
			int j = i + 1;
			int p = 1;
			while (j < obj.EventsCount)
			{
				var next = obj.GetEvent (j++);  // événement suivant
				if (next.Timestamp.Date == date)
				{
					//	Pendant le processus de changement, il peut y avoir le même timestamp pour 2
					//	événements. C'est normal et temporaire.
					var t = new Timestamp (date, p++);
					obj.ChangeEventTimestamp (next, t);
				}
				else
				{
					break;
				}
			}

			var timestamp = new Timestamp (date, position);
			obj.ChangeEventTimestamp (e, timestamp);
			obj.CheckEvents ();

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

				var position = obj.GetNewPosition (date);
				var ts = new Timestamp (date, position);
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
			//	Ajoute la date du jour comme date valeur.
			var p = new DataDateProperty (ObjectField.OneShotDateOperation, Timestamp.Now.Date);
			e.AddProperty (p);
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

			var amortizationType = AmortizationType.Unknown;
			var entryScenario    = EntryScenario.None;

			switch (e.Type)
			{
				case EventType.Input:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Purchase;
					break;

				case EventType.Increase:
				case EventType.MainValue:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Increase;
					break;

				case EventType.Decrease:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Decrease;
					break;

				case EventType.AmortizationAuto:
				case EventType.AmortizationPreview:
					amortizationType = AmortizationType.Linear;
					entryScenario    = EntryScenario.AmortizationAuto;
					break;

				case EventType.AmortizationExtra:
					amortizationType = AmortizationType.Linear;
					entryScenario    = EntryScenario.AmortizationExtra;
					break;

				case EventType.Output:
					amortizationType = AmortizationType.Unknown;  // montant fixe
					entryScenario    = EntryScenario.Sale;
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown EventType {0}", e.Type.ToString ()));
			}

			var aa = Amortizations.InitialiseAmortizedAmount (obj, e, timestamp, amortizationType, entryScenario);

			if (e.Type == EventType.Output)
			{
				//	Il est bien pratique de mettre tout de suite une valeur comptable nulle
				//	lors de la création d'un événement de sortie.
				aa = AmortizedAmount.SetInitialAmount (aa, 0.0m);
			}

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
					while (obj.EventsCount > 0)
					{
						this.RemoveObjectEvent (obj, obj.GetEvent (0));
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
				var aa = Entries.RemoveEntry(this, p.Value);
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

			var dstEvent =   obj.GetEvent (0);  // événement unique de l'objet
			var srcEvent = model.GetEvent (0);  // événement unique de l'objet

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
				yield return ObjectField.AmortizationRate;
				yield return ObjectField.AmortizationType;
				yield return ObjectField.Periodicity;
				yield return ObjectField.Prorata;
				yield return ObjectField.Round;
				yield return ObjectField.ResidualValue;

				yield return ObjectField.Account1;
				yield return ObjectField.Account2;
				yield return ObjectField.Account3;
				yield return ObjectField.Account4;
				yield return ObjectField.Account5;
				yield return ObjectField.Account6;
				yield return ObjectField.Account7;
				yield return ObjectField.Account8;
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


		public string GetFieldName(ObjectField objectField)
		{
			if (objectField >= ObjectField.UserFieldFirst &&
				objectField <= ObjectField.UserFieldLast)
			{
				return this.UserFieldsAccessor.GetUserFieldName (objectField);
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

			switch (objectField)
			{
				case ObjectField.MainValue:
					return FieldType.AmortizedAmount;

				case ObjectField.AmortizationRate:
				case ObjectField.ResidualValue:
				case ObjectField.Round:
				case ObjectField.EntryAmount:
					return FieldType.Decimal;

				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
					return FieldType.Int;

				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotDateEvent:
				case ObjectField.EntryDate:
					return FieldType.Date;

				case ObjectField.GroupParent:
					return FieldType.GuidGroup;

				case ObjectField.Account1:
				case ObjectField.Account2:
				case ObjectField.Account3:
				case ObjectField.Account4:
				case ObjectField.Account5:
				case ObjectField.Account6:
				case ObjectField.Account7:
				case ObjectField.Account8:
				case ObjectField.EntryDebitAccount:
				case ObjectField.EntryCreditAccount:
					return FieldType.Account;

				default:
					return FieldType.String;
			}
		}


		private readonly DataClipboard			clipboard;
		private readonly UserFieldsAccessor		userFieldsAccessor;
		private readonly EditionAccessor		editionAccessor;
		private readonly List<AbstractCleanerAgent> cleanerAgents;

		private DataMandat						mandat;
	}
}
