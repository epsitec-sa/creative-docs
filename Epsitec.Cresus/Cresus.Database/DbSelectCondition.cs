//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{
	
	
	/// <summary>
	/// The <c>DbSelectCondition</c> class represents a condition for the
	/// WHERE clause.
	/// </summary>
	public sealed class DbSelectCondition : AbstractConditionContainer
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		public DbSelectCondition(): this (DbSelectRevision.All)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		/// <param name="revision">The expected revision.</param>
		public DbSelectCondition(DbSelectRevision revision) : base()
		{
			this.Revision = revision;
		}


		/// <summary>
		/// Gets or sets the expected revision.
		/// </summary>
		/// <value>The revision.</value>
		public DbSelectRevision Revision
		{
			get;
			set;
		}


		/// <summary>
		/// Creates the conditions based on the previous <c>AddCondition</c>
		/// calls and using the expected revision.
		/// </summary>
		/// <param name="fields">The collection to which the conditions will be added.</param>
		internal void CreateConditions(Collections.SqlFieldList fields)
		{
			this.CreateConditions (null, null, fields);
		}


		internal void CreateConditions(DbTable mainTable, string mainTableAlias, Collections.SqlFieldList fields)
		{
			if (!this.IsEmpty)
			{
				fields.Add (this.CreateSqlField ());
			}

			DbCondition revisionCondition = null;

			if (mainTable != null)
			{
				switch (this.Revision)
				{
					case DbSelectRevision.LiveAll:
						//	Select all live revisions of the rows: live (0), copied (1)
						//	and archive copy (2) are all < deleted (3).

						revisionCondition = new DbCondition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
														   DbCompare.LessThan,
														   DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbRawType.Int16);
						break;

					case DbSelectRevision.LiveActive:
						//	Select only the active revisions of the rows: live (0) and
						//	copied (1) both describe active rows and are < archive copy (2).

						revisionCondition = new DbCondition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
														   DbCompare.LessThan,
														   DbKey.ConvertToIntStatus (DbRowStatus.ArchiveCopy), DbRawType.Int16);
						break;

					case DbSelectRevision.All:
						//	Select all revisions of the rows; there is no need to add an
						//	additional condition for this !

						break;

					default:
						throw new System.NotSupportedException (string.Format ("DbSelectRevision.{0} not supported", this.Revision));
				}
			}
			
			if (revisionCondition != null)
			{
				fields.Add (revisionCondition.CreateSqlField ());
			}	
		}


	}


}
