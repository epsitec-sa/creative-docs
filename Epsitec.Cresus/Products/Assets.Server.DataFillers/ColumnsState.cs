//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Etats de toutes les colonnes. On conserve ici quelles sont les colonnes visibles,
	/// dans quel ordre, et comment elles sont triées.
	/// </summary>
	public struct ColumnsState
	{
		public ColumnsState(int[] mapper, ColumnState[] columns, SortedColumn[] sorted, int dockToLeftCount)
		{
			System.Diagnostics.Debug.Assert (mapper.Length == columns.Length);

			this.Mapper          = mapper;
			this.Columns         = columns;
			this.Sorted          = sorted;
			this.DockToLeftCount = dockToLeftCount;
		}

		public ColumnsState(System.Xml.XmlReader reader)
		{
			var mapper  = new List<int> ();
			var columns = new List<ColumnState> ();
			var sorted  = new List<SortedColumn> ();
			this.DockToLeftCount = 0;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Mappers":
							var mappers = reader.ReadElementContentAsString ().Split (new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);
							foreach (var m in mappers)
							{
								mapper.Add (m.ParseInt ());
							}
							break;

						case "Column":
							var c = new ColumnState (reader);
							columns.Add (c);
							break;

						case "Sorted":
							var s = new SortedColumn (reader);
							sorted.Add (s);
							break;

						case "DockToLeftCount":
							this.DockToLeftCount = reader.ReadElementContentAsString ().ParseInt ();
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}

			this.Mapper  = mapper .ToArray ();
			this.Columns = columns.ToArray ();
			this.Sorted  = sorted .ToArray ();
		}


		public bool IsEmpty
		{
			get
			{
				return this.Mapper == null || this.Mapper.Length == 0;
			}
		}

		public IEnumerable<ColumnState> MappedColumns
		{
			get
			{
				for (int i=0; i<this.Mapper.Length; i++)
				{
					int j = this.Mapper[i];
					yield return this.Columns[j];
				}
			}
		}


		public int AbsoluteToMapped(int absRank)
		{
			return this.Mapper.ToList ().IndexOf (absRank);
		}

		public int MappedToAbsolute(int mappedRank)
		{
			return this.Mapper[mappedRank];
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			var mappers = string.Join (";", this.Mapper.Select (x => x.ToStringIO ()));
			writer.WriteElementString ("Mappers", mappers);

			foreach (var column in this.Columns)
			{
				column.Serialize (writer, "Column");
			}

			foreach (var sorted in this.Sorted)
			{
				sorted.Serialize (writer, "Sorted");
			}

			writer.WriteElementString ("DockToLeftCount", this.DockToLeftCount.ToStringIO ());

			writer.WriteEndElement ();
		}


		public static ColumnsState Empty = new ColumnsState (new int[0], new ColumnState[0], new SortedColumn[0], 0);


		public readonly int[]					Mapper;
		public readonly ColumnState[]			Columns;
		public readonly SortedColumn[]			Sorted;
		public readonly int						DockToLeftCount;
	}
}
