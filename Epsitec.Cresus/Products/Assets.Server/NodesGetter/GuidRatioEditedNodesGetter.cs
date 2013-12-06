//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Donne accès en lecture aux champs guid-ratio de l'objet en édition.
	/// </summary>
	public class GuidRatioEditedNodesGetter : AbstractNodesGetter<ObjectField>  // outputNodes
	{
		public GuidRatioEditedNodesGetter(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.fields = new List<ObjectField> ();
		}


		public void SetParams()
		{
			this.fields.Clear ();
			this.fields.AddRange (this.SortedFields);
			this.fields.Add (this.FreeField);
		}


		public override int Count
		{
			get
			{
				return this.fields.Count;
			}
		}

		public override ObjectField this[int index]
		{
			get
			{
				if (index >= 0 && index < this.fields.Count)
				{
					return this.fields[index];
				}
				else
				{
					return ObjectField.Unknown;
				}
			}
		}


		private IEnumerable<ObjectField> SortedFields
		{
			get
			{
				return this.UsedFields.OrderBy (x => this.GetName (x));
			}
		}

		private string GetName(ObjectField field)
		{
			var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
			return GroupsLogic.GetFullName (this.accessor, gr.Guid);
		}

		private IEnumerable<ObjectField> UsedFields
		{
			get
			{
				for (int i=0; i<10; i++)
				{
					var field = ObjectField.GroupGuidRatio+i;

					var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					if (!gr.IsEmpty)
					{
						yield return field;
					}
				}
			}
		}

		private ObjectField FreeField
		{
			get
			{
				for (int i=0; i<10; i++)
				{
					var field = ObjectField.GroupGuidRatio+i;

					var gr = this.accessor.EditionAccessor.GetFieldGuidRatio (field);
					if (gr.IsEmpty)
					{
						return field;
					}
				}

				return ObjectField.Unknown;
			}
		}


		private readonly DataAccessor			accessor;
		private readonly List<ObjectField>		fields;
	}
}
