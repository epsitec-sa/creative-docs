//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès aux objets d'immobilisations.
	/// C'est un ObjectsNodeGetter trié (parfois).
	/// Provisoire, cela demande encore de la réflexion !!!
	/// </summary>
	public class MCH2SummaryNodeGetter : INodeGetter<CumulNode>, ITreeFunctions, IObjectsNodeGetter  // outputNodes
	{
		public MCH2SummaryNodeGetter(DataAccessor accessor, INodeGetter<GuidNode> groupNodes, INodeGetter<GuidNode> objectNodes)
		{
			this.accessor   = accessor;
			this.inputNodes = new ObjectsNodeGetter (this.accessor, groupNodes, objectNodes);

			this.outputSortedNodes = new List<CumulNode> ();
		}


		public void SetParams(Timestamp? timestamp, Guid rootGuid, SortingInstructions instructions, List<ExtractionInstructions> extractionInstructions = null)
		{
			this.timestamp              = timestamp;
			this.rootGuid               = rootGuid;
			this.sortingInstructions    = instructions;
			this.extractionInstructions = extractionInstructions;
	
			this.inputNodes.SetParams (this.timestamp, this.rootGuid, this.sortingInstructions, this.extractionInstructions);
			this.Sort ();
		}


		public int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public CumulNode this[int index]
		{
			get
			{
				if (this.SortingRequired)
				{
					return this.outputSortedNodes[index];
				}
				else
				{
					return this.inputNodes[index];
				}
			}
		}


		public decimal? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			return this.inputNodes.GetValue (obj, node, field);
		}


		#region ITreeFonctions
		public bool IsAllCompacted
		{
			get
			{
				return this.inputNodes.IsAllCompacted;
			}
		}

		public bool IsAllExpanded
		{
			get
			{
				return this.inputNodes.IsAllExpanded;
			}
		}

		public void CompactOrExpand(int index)
		{
			this.inputNodes.CompactOrExpand (index);
		}

		public void CompactAll()
		{
			this.inputNodes.CompactAll ();
		}

		public void CompactOne()
		{
			this.inputNodes.CompactOne ();
		}

		public void ExpandOne()
		{
			this.inputNodes.ExpandOne ();
		}

		public void ExpandAll()
		{
			this.inputNodes.ExpandAll ();
		}

		public void SetLevel(int level)
		{
			this.inputNodes.SetLevel (level);
		}

		public int GetLevel()
		{
			return this.inputNodes.GetLevel ();
		}

		public int SearchBestIndex(Guid value)
		{
			return this.inputNodes.SearchBestIndex (value);
		}

		public int VisibleToAll(int index)
		{
			return this.inputNodes.VisibleToAll (index);
		}

		public int AllToVisible(int index)
		{
			return this.inputNodes.AllToVisible (index);
		}
		#endregion


		private void Sort()
		{
			this.outputSortedNodes.Clear ();

			if (this.SortingRequired)
			{
				this.mainField = this.MainField;
				this.outputSortedNodes.AddRange (this.SortNodes (this.InputNodes));
			}
		}

		private IEnumerable<CumulNode> InputNodes
		{
			get
			{
				for (int i=0; i<this.inputNodes.Count;i++)
				{
					yield return this.inputNodes[i];
				}
			}
		}

		private IEnumerable<CumulNode> SortNodes(IEnumerable<CumulNode> nodes)
		{
			if (this.sortingInstructions.PrimaryField   != ObjectField.Unknown &&
				this.sortingInstructions.SecondaryField == ObjectField.Unknown)
			{
				//	Seulement un critère de tri principal.

				if (this.sortingInstructions.PrimaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => this.GetPrimaryData (x));
				}
				else if (this.sortingInstructions.PrimaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => this.GetPrimaryData (x));
				}
			}
			else if (this.sortingInstructions.PrimaryField   != ObjectField.Unknown &&
					 this.sortingInstructions.SecondaryField != ObjectField.Unknown)
			{
				//	Un critère de tri principal et un secondaire.

				if (this.sortingInstructions.PrimaryType   == SortedType.Ascending &&
					this.sortingInstructions.SecondaryType == SortedType.Ascending)
				{
					return nodes.OrderBy (x => this.GetSecondaryData (x))
								.OrderBy (x => this.GetPrimaryData (x));
				}
				else if (this.sortingInstructions.PrimaryType   == SortedType.Ascending &&
						 this.sortingInstructions.SecondaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => this.GetSecondaryData (x))
								.OrderBy           (x => this.GetPrimaryData (x));
				}
				else if (this.sortingInstructions.PrimaryType   == SortedType.Descending &&
						 this.sortingInstructions.SecondaryType == SortedType.Ascending)
				{
					return nodes.OrderBy           (x => this.GetSecondaryData (x))
								.OrderByDescending (x => this.GetPrimaryData (x));
				}
				else if (this.sortingInstructions.PrimaryType   == SortedType.Descending &&
						 this.sortingInstructions.SecondaryType == SortedType.Descending)
				{
					return nodes.OrderByDescending (x => this.GetSecondaryData (x))
								.OrderByDescending (x => this.GetPrimaryData (x));
				}
			}

			return nodes;
		}

		private ComparableData GetPrimaryData(CumulNode node)
		{
			return this.GetComparableData (node, this.sortingInstructions.PrimaryField);
		}

		private ComparableData GetSecondaryData(CumulNode node)
		{
			return this.GetComparableData (node, this.sortingInstructions.SecondaryField);
		}

		private ComparableData GetComparableData(CumulNode node, ObjectField field)
		{
			var obj = this.accessor.GetObject (node.BaseType, node.Guid);

			if (field == ObjectField.MCH2Report)
			{
				var text = ObjectProperties.GetObjectPropertyString (obj, this.timestamp, this.mainField);
				return new ComparableData (text);
			}
			else
			{
				var value = this.inputNodes.GetValue (obj, node, field);
				return new ComparableData (value);
			}
		}


		private ObjectField MainField
		{
			get
			{
				if (this.UserFields.Any ())
				{
					return this.UserFields.First ().Field;
				}
				else
				{
					return ObjectField.Unknown;
				}
			}
		}

		private IEnumerable<UserField> UserFields
		{
			get
			{
				return AssetsLogic.GetUserFields (this.accessor);
			}
		}


		private bool SortingRequired
		{
			get
			{
				if (this.rootGuid.IsEmpty)  // pas de regroupements ?
				{
					return this.extractionInstructions != null
						&& this.extractionInstructions.Any ();
				}
				else
				{
					return false;
				}
			}
		}


		private readonly DataAccessor			accessor;
		private readonly ObjectsNodeGetter		inputNodes;
		private readonly List<CumulNode>		outputSortedNodes;

		private Timestamp?						timestamp;
		private Guid							rootGuid;
		private SortingInstructions				sortingInstructions;
		private List<ExtractionInstructions>	extractionInstructions;

		private ObjectField						mainField;
	}
}
