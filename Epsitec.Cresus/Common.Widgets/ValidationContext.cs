//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class ValidationContext : DependencyObject
	{
		public ValidationContext()
		{
			this.uniqueId = System.Threading.Interlocked.Increment (ref ValidationContext.nextUniqueId);
		}

		public long UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		/// <summary>
		/// Updates the command enable based on the visual validity.
		/// </summary>
		/// <param name="visual">The visual.</param>
		public void UpdateCommandEnableBasedOnVisualValidity(Visual visual)
		{
			CommandState state;
			CommandContext context;
			
			//	Either find a CommandContext quickly by going through the CommandCache
			//	and CommandState, or find the nearest CommandContext in the chain :
			
			if (visual.HasCommand)
			{
				state   = visual.CommandState;
				context = state.CommandContext;
			}
			else
			{
				state   = null;
				context = Helpers.VisualTree.GetCommandContext (visual);
			}
			
			bool enable = ValidationContext.GetVisualValidity (visual);
			
			long serialId = visual.VisualSerialId;
			string groups = visual.ValidationGroups;
			
			//	Find the record for the specified visual. This should be fast, as
			//	we work with a sorted record list :
			
			Record record = new Record (serialId, groups, enable);
			
			int index = this.records.BinarySearch (record);

			if (index < 0)
			{
				//	There was no record for this visual. If the visual causes some
				//	commands to be disabled, then they get disabled here :
				
				if (enable == false)
				{
					this.DisableGroups (context, record.Groups);
				}

				//	Insert the record at its position :
				
				this.records.Insert (~index, record);
			}
			else
			{
				//	We have found an existing record for this visual. If there is
				//	a change in the record definition, update the command group
				//	enables :
				
				Record oldRecord = this.records[index];
				Record newRecord = record;

				if ((oldRecord.Groups != newRecord.Groups) ||
					(oldRecord.Enable != newRecord.Enable))
				{
					if (oldRecord.Enable == false)
					{
						this.EnableGroups (context, oldRecord.Groups);
					}
					if (newRecord.Enable == false)
					{
						this.DisableGroups (context, newRecord.Groups);
					}
					
					this.records[index] = newRecord;
				}
			}
		}

		public void Refresh(Visual root)
		{
			List<Record> records = new List<Record> ();
			CommandContext context = Helpers.VisualTree.GetCommandContext (root);
			
			foreach (Visual child in root.GetAllChildren ())
			{
				if ((child.HasValidator) &&
					(child.HasValidationGroups))
				{
					bool enable;

					enable = ValidationContext.GetVisualValidity (child);

					records.Add (new Record (child.VisualSerialId, child.ValidationGroups, enable));
				}
			}

			records.Sort ();

			List<Record> oldRecords = this.records;
			List<Record> newRecords = records;
			
			int i = 0;
			int j = 0;

			while ((i < newRecords.Count) && (j < oldRecords.Count))
			{
				Record newRecord = newRecords[i];
				Record oldRecord = oldRecords[j];

				if (newRecord.Visual == oldRecord.Visual)
				{
					//	Really the same records ?

					if ((newRecord.Enable == oldRecord.Enable) &&
						(newRecord.Groups == oldRecord.Groups))
					{
						//	Nothing to do -- up to date for this record.
					}
					else
					{
						if (oldRecord.Enable == false)
						{
							this.EnableGroups (context, oldRecord.Groups);
						}
						if (newRecord.Enable == false)
						{
							this.DisableGroups (context, newRecord.Groups);
						}
					}
					
					i++;
					j++;
				}
				else if (newRecord.Visual < oldRecord.Visual)
				{
					//	There is a new record, not known to the command context.

					if (newRecord.Enable == false)
					{
						this.DisableGroups (context, newRecord.Groups);
					}

					i++;
				}
				else
				{
					//	There is an old record which effect must be undone.

					if (oldRecord.Enable == false)
					{
						this.EnableGroups (context, oldRecord.Groups);
					}

					j++;
				}
			}
			
			while (i < newRecords.Count)
			{
				Record newRecord = newRecords[i];

				if (newRecord.Enable == false)
				{
					this.DisableGroups (context, newRecord.Groups);
				}

				i++;
			}
			
			while (j < oldRecords.Count)
			{
				Record oldRecord = oldRecords[i];

				if (oldRecord.Enable == false)
				{
					this.EnableGroups (context, oldRecord.Groups);
				}

				i++;
			}

			this.records = records;
		}

		private static bool GetVisualValidity(Visual visual)
		{
			//	Return true if the visual is valid or disabled (a disabled widget
			//	cannot break the validity, as the user wouldn't have any means of
			//	fixing the problem).
			
			return visual.IsEnabled ? visual.IsValid : true;
		}

		private void EnableGroups(CommandContext context, string groups)
		{
			int disables;

			foreach (string group in Command.SplitGroupNames (groups))
			{
				if (this.groupDisables.TryGetValue (group, out disables))
				{
					if (disables > 1)
					{
						this.groupDisables[group] = disables-1;
					}
					else
					{
						this.groupDisables.Remove (group);
						context.SetGroupEnable (this, group, true);
					}
				}
			}
		}

		private void DisableGroups(CommandContext context, string groups)
		{
			int disables;

			foreach (string group in Command.SplitGroupNames (groups))
			{
				if (this.groupDisables.TryGetValue (group, out disables))
				{
					this.groupDisables[group] = disables+1;
				}
				else
				{
					this.groupDisables[group] = 1;
					context.SetGroupEnable (this, group, false);
				}
			}
		}

		#region Private Record Structure

		private struct Record : System.IComparable<Record>, System.IEquatable<Record>
		{
			public Record(long visual, string groups, bool enable)
			{
				this.visual = visual;
				this.groups = groups;
				this.enable = enable;
			}

			public long Visual
			{
				get
				{
					return this.visual;
				}
			}
			
			public bool Enable
			{
				get
				{
					return this.enable;
				}
			}
			
			public string Groups
			{
				get
				{
					return this.groups;
				}
			}

			#region IComparable<Record> Members

			public int CompareTo(Record other)
			{
				if (this.visual < other.visual)
				{
					return -1;
				}
				else if (this.visual > other.visual)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

			#endregion

			#region IEquatable<Record> Members

			public bool Equals(Record other)
			{
				return this.visual == other.visual;
			}

			#endregion

			private long visual;
			private string groups;
			private bool enable;
		}
		
		#endregion

		public static ValidationContext GetContext(DependencyObject obj)
		{
			return (ValidationContext) obj.GetValue (ValidationContext.ContextProperty);
		}

		public static void SetContext(DependencyObject obj, ValidationContext context)
		{
			obj.SetValue (ValidationContext.ContextProperty, context);
		}

		public static void ClearContext(DependencyObject obj)
		{
			obj.ClearValue (ValidationContext.ContextProperty);
		}


		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (ValidationContext), typeof (ValidationContext));

		private static long nextUniqueId;
		
		private long uniqueId;
		private List<Record> records = new List<Record> ();
		private Dictionary<string, int> groupDisables = new Dictionary<string, int> ();
	}
}
