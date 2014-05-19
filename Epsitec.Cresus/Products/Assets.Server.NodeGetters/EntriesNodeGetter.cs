//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class EntriesNodeGetter : INodeGetter<EntryNode>, ITreeFunctions  // outputNodes
	{
		public EntriesNodeGetter(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.nodes       = new List<EntryNode> ();
			this.nodeIndexes = new List<int> ();
		}


		public void SetParams(SortingInstructions instructions)
		{
			bool groupedByAssets = instructions.PrimaryField != ObjectField.EntryDate;
			this.UpdateData (groupedByAssets);
		}


		public int Count
		{
			get
			{
				return this.nodeIndexes.Count;
			}
		}

		public EntryNode this[int index]
		{
			get
			{
				if (index < this.nodeIndexes.Count)
				{
					int i = this.nodeIndexes[index];
					if (i < this.nodes.Count)
					{
						return this.nodes[i];
					}
				}

				return EntryNode.Empty;
			}
		}

		private void UpdateData(bool groupedByAssets)
		{
			this.nodes.Clear ();

			if (groupedByAssets)
			{
				this.UpdateDataByAssets ();
			}
			else
			{
				this.UpdateDataByDates ();
			}

			this.UpdateNodeIndexes ();
		}

		private void UpdateDataByAssets()
		{
			var nodes = this.GetNodes ().OrderBy (x => x.Date).OrderBy (x => x.AssetName);
			string lastName = null;

			foreach (var node in nodes)
			{
				var name = AssetsLogic.GetSummary (this.accessor, node.AssetGuid);

				if (lastName != name)
				{
					//	Ajoute une ligne de "titre".
					var n = new EntryNode (Guid.Empty, Guid.Empty, null, null, null, null, null, name, null, 0, NodeType.Expanded, EventType.Unknown);
					this.nodes.Add (n);

					lastName = name;
				}

				this.nodes.Add (node);
			}
		}

		private void UpdateDataByDates()
		{
			var nodes = this.GetNodes ().OrderBy (x => x.AssetName).OrderBy (x => x.Date);
			var lastDate = System.DateTime.MinValue;

			foreach (var node in nodes)
			{
				if (lastDate != node.Date)
				{
					//	Ajoute une ligne de "titre".
					var n = new EntryNode (Guid.Empty, Guid.Empty, null, node.Date, null, null, null, null, null, 0, NodeType.Expanded, EventType.Unknown);
					this.nodes.Add (n);

					lastDate = node.Date.Value;
				}

				this.nodes.Add (node);
			}
		}

		private List<EntryNode> GetNodes()
		{
			var nodes = new List<EntryNode> ();
			var entries = this.accessor.Mandat.GetData (BaseType.Entries);

			for (int i=0; i<entries.Count; i++)
			{
				var entry = entries[i];

				var assetGuid = ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryAssetGuid);
				var eventGuid = ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryEventGuid);

				var obj = this.accessor.GetObject (BaseType.Assets, assetGuid);
				System.Diagnostics.Debug.Assert (obj != null);
				var e = obj.GetEvent (eventGuid);
				System.Diagnostics.Debug.Assert (e != null);
				var name = AssetsLogic.GetSummary (this.accessor, assetGuid);

				var date   = ObjectProperties.GetObjectPropertyDate    (entry, null, ObjectField.EntryDate);
				var debit  = ObjectProperties.GetObjectPropertyGuid    (entry, null, ObjectField.EntryDebitAccount);
				var credit = ObjectProperties.GetObjectPropertyGuid    (entry, null, ObjectField.EntryCreditAccount);
				var stamp  = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryStamp);
				var title  = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryTitle);
				var value  = ObjectProperties.GetObjectPropertyDecimal (entry, null, ObjectField.EntryAmount);

				var d = AccountsLogic.GetNumber (this.accessor, debit);
				var c = AccountsLogic.GetNumber (this.accessor, credit);

				var node = new EntryNode (entry.Guid, assetGuid, name, date, d, c, stamp, title, value, 1, NodeType.Final, e.Type);
				nodes.Add (node);
			}

			return nodes;
		}


		#region ITreeFonctions
		public bool IsAllCompacted
		{
			get
			{
				return !this.nodes.Where (x => x.NodeType == NodeType.Expanded).Any ();
			}
		}

		public bool IsAllExpanded
		{
			get
			{
				return !this.nodes.Where (x => x.NodeType == NodeType.Compacted).Any ();
			}
		}

		public void CompactOrExpand(int index)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			int i = this.nodeIndexes[index];
			var node = this.nodes[i];

			if (node.NodeType == NodeType.Compacted)
			{
				this.nodes[i] = new EntryNode (node.EntryGuid, node.AssetGuid, node.AssetName, node.Date, node.Debit, node.Credit, node.Stamp, node.Title, node.Value, node.Level, NodeType.Expanded, node.EventType);
			}
			else if (node.NodeType == NodeType.Expanded)
			{
				this.nodes[i] = new EntryNode (node.EntryGuid, node.AssetGuid, node.AssetName, node.Date, node.Debit, node.Credit, node.Stamp, node.Title, node.Value, node.Level, NodeType.Compacted, node.EventType);
			}

			this.UpdateNodeIndexes ();
		}

		public void CompactAll()
		{
			//	Compacte toutes les lignes.
			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.NodeType == NodeType.Expanded)
				{
					this.nodes[i] = new EntryNode (node.EntryGuid, node.AssetGuid, node.AssetName, node.Date, node.Debit, node.Credit, node.Stamp, node.Title, node.Value, node.Level, NodeType.Compacted, node.EventType);
				}
			}

			this.UpdateNodeIndexes ();
		}

		public void CompactOne()
		{
			//	Compacte d'un niveau.
		}

		public void ExpandOne()
		{
			//	Etend d'un niveau.
		}

		public void ExpandAll()
		{
			//	Etend toutes les lignes.
			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.NodeType == NodeType.Compacted)
				{
					this.nodes[i] = new EntryNode (node.EntryGuid, node.AssetGuid, node.AssetName, node.Date, node.Debit, node.Credit, node.Stamp, node.Title, node.Value, node.Level, NodeType.Expanded, node.EventType);
				}
			}

			this.UpdateNodeIndexes ();
		}

		public void SetLevel(int level)
		{
		}

		public int GetLevel()
		{
			return 0;
		}

		public int SearchBestIndex(Guid value)
		{
			//	Retourne l'index ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour retourner la prochaine ligne
			//	visible, vers le haut.
			int index = -1;

			if (!value.IsEmpty)
			{
				var i = this.nodes.FindIndex (x => x.EntryGuid == value);
				if (i != -1)
				{
					index = this.nodeIndexes.Where (x => x <= i).Count () - 1;
				}
			}

			return index;
		}

		public int VisibleToAll(int index)
		{
			if (index >= 0 && index < this.nodeIndexes.Count)
			{
				return this.nodeIndexes[index];
			}
			else
			{
				return -1;
			}
		}

		public int AllToVisible(int index)
		{
			return this.nodeIndexes.IndexOf (index);
		}
		#endregion



		private void UpdateNodeIndexes()
		{
			//	Met à jour l'accès aux noeuds (nodeIndexes) en sautant les
			//	noeuds cachés.
			this.nodeIndexes.Clear ();

			bool skip = false;
			int skipLevel = 0;

			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (skip)
				{
					if (node.Level <= skipLevel)
					{
						skip = false;
					}
					else
					{
						continue;
					}
				}

				if (node.NodeType == NodeType.Compacted)
				{
					skip = true;
					skipLevel = node.Level;
				}

				this.nodeIndexes.Add (i);
			}
		}


		private readonly DataAccessor			accessor;
		private readonly List<EntryNode>		nodes;
		private readonly List<int>				nodeIndexes;
	}
}
