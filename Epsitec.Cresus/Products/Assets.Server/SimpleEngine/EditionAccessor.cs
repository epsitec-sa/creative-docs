//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class EditionAccessor
	{
		public EditionAccessor(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.objectGuid = Guid.Empty;
		}


		public DataObject						EditedObject
		{
			get
			{
				return this.obj;
			}
		}

		public Timestamp?						EditedTimestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public bool								IsLocked
		{
			get
			{
				return this.isLocked;
			}
		}


		public void StartObjectEdition(BaseType baseType, Guid objectGuid, Timestamp? timestamp)
		{
			//	Marque le début de l'édition de l'événement d'un objet.
			this.baseType = baseType;

			if (objectGuid.IsEmpty || !timestamp.HasValue)
			{
				return;
			}

			this.dirty               = false;
			this.computedAmountDirty = false;
			this.objectGuid          = objectGuid;
			this.timestamp           = timestamp;

			this.obj = this.accessor.GetObject (this.baseType, this.objectGuid);
			System.Diagnostics.Debug.Assert (this.obj != null);

			this.isLocked = AssetCalculator.IsLocked (obj, timestamp.Value);

			var e = this.EditionEvent;
			if (e == null)
			{
				this.dataEvent = null;
			}
			else
			{
				this.dataEvent = new DataEvent (e);  // conserve une copie
			}
		}

		public bool SaveObjectEdition()
		{
			//	Marque la fin de l'édition de l'événement d'un objet.
			//	Retourne true si les données ont été mises à jour.
			Timestamp? newTimestamp;
			return this.SaveObjectEdition (out newTimestamp);
		}

		public bool SaveObjectEdition(out Timestamp? newTimestamp)
		{
			//	Marque la fin de l'édition de l'événement d'un objet.
			//	Si l'événement a été déplacé dans le temps, newTimestamp donne sa nouvelle position.
			//	Retourne true si les données ont été mises à jour.
			newTimestamp = null;

			if (this.dirty)
			{
				var e = this.obj.GetEvent (this.timestamp.Value);
				e.SetProperties (this.dataEvent);

				if (this.baseType == BaseType.Assets)
				{
					//	On regarde si l'utilisateur a changé la date de l'événement. Si oui, il faut
					//	modifier l'événement en conséquence.
					var p = this.dataEvent.GetProperty (ObjectField.OneShotDateEvent) as DataDateProperty;
					if (p != null && p.Value != e.Timestamp.Date)  // date changée ?
					{
						newTimestamp = this.accessor.ChangeAssetEventTimestamp (obj, e, p.Value);
					}
				}

				if (this.baseType == BaseType.UserFields)
				{
					this.accessor.GlobalSettings.SetTempDataObject (this.obj);
				}
				else
				{
					if (this.computedAmountDirty)
					{
						Amortizations.UpdateAmounts (this.accessor, this.obj);
					}
				}

				this.accessor.WarningsDirty = true;

				this.CancelObjectEdition ();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void CancelObjectEdition()
		{
			//	Marque la fin de l'édition de l'événement d'un objet.
			this.baseType            = BaseType.Unknown;
			this.dirty               = false;
			this.computedAmountDirty = false;
			this.objectGuid          = Guid.Empty;
			this.timestamp           = null;
			this.obj                 = null;
			this.dataEvent           = null;
		}


		public PropertyState GetEditionPropertyState(ObjectField field)
		{
			if (this.dataEvent != null)
			{
				var property = this.dataEvent.GetProperty (field);
				if (property != null)
				{
					return PropertyState.Single;
				}
			}

			return PropertyState.Synthetic;
		}


		public System.DateTime EventDate
		{
			//	Retourne la date de l'événement en cours d'édition.
			get
			{
				if (this.dataEvent == null)
				{
					return Timestamp.Now.Date;
				}
				else
				{
					return this.dataEvent.Timestamp.Date;
				}
			}
		}


		#region Getters
		public ComputedAmount? GetFieldComputedAmount(ObjectField field)
		{
			if (this.dataEvent != null)
			{
				var property = this.dataEvent.GetProperty (field) as DataComputedAmountProperty;
				if (property != null)
				{
					return property.Value;
				}

				if (this.obj != null && this.timestamp.HasValue)
				{
					var before = this.timestamp.Value.JustBefore;
					property = ObjectProperties.GetObjectProperty (this.obj, before, field, true) as DataComputedAmountProperty;

					if (property != null)
					{
						//	Cette situation est tordue. On demande le montant calculé à un
						//	instant pour lequel il n'existe pas. On cherche donc le précédent,
						//	mais on ne peut pas le retourner tel quel. On doit retourner un
						//	montant qui a une valeur initiale égale à la valeur finale du
						//	montant précédent trouvé.
						return new ComputedAmount
						(
							property.Value.FinalAmount,
							null,
							property.Value.FinalAmount,
							property.Value.Subtract,
							property.Value.Rate,
							false
						);
					}
				}
			}

			return null;
		}

		public AmortizedAmount? GetFieldAmortizedAmount(ObjectField field)
		{
			if (this.dataEvent != null)
			{
				var property = this.dataEvent.GetProperty (field) as DataAmortizedAmountProperty;
				if (property != null)
				{
					return property.Value;
				}

				if (this.obj != null && this.timestamp.HasValue)
				{
					var before = this.timestamp.Value.JustBefore;
					property = ObjectProperties.GetObjectProperty (this.obj, before, field, true) as DataAmortizedAmountProperty;

					if (property != null)
					{
						//	Cette situation est tordue. On demande le montant calculé à un
						//	instant pour lequel il n'existe pas. On cherche donc le précédent,
						//	mais on ne peut pas le retourner tel quel. On doit retourner un
						//	montant qui a une valeur initiale égale à la valeur finale du
						//	montant précédent trouvé.
						throw new System.InvalidOperationException ("Should never happen!");
					}
				}
			}

			return null;
		}

		public System.DateTime? GetFieldDateMin(ObjectField field)
		{
			if (field == ObjectField.OneShotDateEvent && this.timestamp.HasValue)
			{
				var e = this.obj.GetPrevEvent (this.timestamp.Value);
				if (e != null)
				{
					return e.Timestamp.Date;
				}
			}

			return null;
		}

		public System.DateTime? GetFieldDateMax(ObjectField field)
		{
			if (field == ObjectField.OneShotDateEvent && this.timestamp.HasValue)
			{
				var e = this.obj.GetNextEvent (this.timestamp.Value);
				if (e != null)
				{
					return e.Timestamp.Date;
				}
			}

			return null;
		}

		public System.DateTime? GetFieldDate(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataDateProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public decimal? GetFieldDecimal(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataDecimalProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public Guid GetFieldGuid(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataGuidProperty;

			if (p == null)
			{
				return Guid.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public GuidRatio GetFieldGuidRatio(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataGuidRatioProperty;

			if (p == null)
			{
				return GuidRatio.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public int? GetFieldInt(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataIntProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public string GetFieldString(ObjectField field, bool synthetic = true)
		{
			var p = this.GetProperty (field, synthetic) as DataStringProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}
		#endregion


		#region Setters
		public void SetField(ObjectField field, ComputedAmount? value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
				this.computedAmountDirty = true;
			}
		}

		public void SetField(ObjectField field, AmortizedAmount? value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				Amortizations.SetAmortizedAmount (e, value);

				this.dirty = true;
				this.computedAmountDirty = true;
			}
		}

		public void SetField(ObjectField field, System.DateTime? value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataDateProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
			}
		}

		public void SetField(ObjectField field, decimal? value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataDecimalProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
			}
		}

		public void SetField(ObjectField field, Guid value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (!value.IsEmpty)
				{
					var newProperty = new DataGuidProperty (field, value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
			}
		}

		public void SetField(ObjectField field, GuidRatio value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (!value.IsEmpty)
				{
					var newProperty = new DataGuidRatioProperty (field, value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
			}
		}

		public void SetField(ObjectField field, int? value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (value.HasValue)
				{
					var newProperty = new DataIntProperty (field, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (field);
				}

				this.dirty = true;
			}
		}

		public void SetField(ObjectField field, string value)
		{
			var e = this.dataEvent;
			if (e != null)
			{
				if (value == null)
				{
					e.RemoveProperty (field);
				}
				else
				{
					var newProperty = new DataStringProperty (field, value);
					e.AddProperty (newProperty);
				}

				this.dirty = true;
			}
		}
		#endregion


		private AbstractDataProperty GetProperty(ObjectField field, bool synthetic = true)
		{
			//	Retourne une propriété en édition. L'événement en cours d'édition
			//	a la priorité.
			if (this.dataEvent != null)
			{
				var property = this.dataEvent.GetProperty (field);
				if (property != null)
				{
					return property;
				}

				if (this.obj != null && this.timestamp.HasValue && synthetic)
				{
					var before = this.timestamp.Value.JustBefore;
					return ObjectProperties.GetObjectProperty (this.obj, before, field, true);
				}
			}

			return null;
		}

		private DataEvent EditionEvent
		{
			get
			{
				if (this.obj != null && this.timestamp.HasValue)
				{
					return this.obj.GetEvent (this.timestamp.Value);
				}

				return null;
			}
		}


		private readonly DataAccessor			accessor;

		private BaseType						baseType;
		private Guid							objectGuid;
		private DataObject						obj;
		private Timestamp?						timestamp;
		private DataEvent						dataEvent;
		private bool							dirty;
		private bool							computedAmountDirty;
		private bool							isLocked;
	}
}
