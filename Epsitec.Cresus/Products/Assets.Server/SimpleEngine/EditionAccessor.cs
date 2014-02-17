﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
			if (this.dirty)
			{
				var e = this.obj.GetEvent (this.timestamp.Value);
				e.SetProperties (this.dataEvent);

				if (this.computedAmountDirty)
				{
					ObjectCalculator.UpdateComputedAmounts (this.obj);
				}

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
			this.dirty               = false;
			this.computedAmountDirty = false;
			this.objectGuid          = Guid.Empty;
			this.timestamp           = null;
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
					property = ObjectCalculator.GetObjectProperty (this.obj, before, field, true) as DataComputedAmountProperty;

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

		public System.DateTime? GetFieldDate(ObjectField field)
		{
			var p = this.GetProperty (field) as DataDateProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public decimal? GetFieldDecimal(ObjectField field)
		{
			var p = this.GetProperty (field) as DataDecimalProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public Guid GetFieldGuid(ObjectField field)
		{
			var p = this.GetProperty (field) as DataGuidProperty;

			if (p == null)
			{
				return Guid.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public GuidRatio GetFieldGuidRatio(ObjectField field)
		{
			var p = this.GetProperty (field) as DataGuidRatioProperty;

			if (p == null)
			{
				return GuidRatio.Empty;
			}
			else
			{
				return p.Value;
			}
		}

		public int? GetFieldInt(ObjectField field)
		{
			var p = this.GetProperty (field) as DataIntProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public string GetFieldString(ObjectField field)
		{
			var p = this.GetProperty (field) as DataStringProperty;

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


		private AbstractDataProperty GetProperty(ObjectField field)
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

				if (this.obj != null && this.timestamp.HasValue)
				{
					var before = this.timestamp.Value.JustBefore;
					return ObjectCalculator.GetObjectProperty (this.obj, before, field, true);
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
	}
}
