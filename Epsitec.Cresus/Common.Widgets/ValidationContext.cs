//	Copyright © 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ValidationContext</c> class defines a validation context, which
	/// maintains information about user interface value validity, on a group
	/// basis. This is thightly bound to the <see cref="CommandContext"/>.
	/// </summary>
	public sealed class ValidationContext : DependencyObject, System.IEquatable<ValidationContext>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationContext"/> class.
		/// </summary>
		public ValidationContext()
		{
			this.uniqueId = System.Threading.Interlocked.Increment (ref ValidationContext.nextUniqueId);
			this.records = new List<Record> ();
			this.groupDisables = new Dictionary<string, int> ();
		}


		/// <summary>
		/// Gets the unique id associated with this validation context.
		/// </summary>
		/// <value>The unique id.</value>
		public long UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		/// <summary>
		/// Updates the command enable based on the visual validity. This will
		/// also store a tracking record about the visual state, if the visual
		/// is not yet known by the validation context.
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
			
			long serialId = visual.GetVisualSerialId ();
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
					this.DisableGroups (context, groups);
				}

				//	Insert the record at its position (note -1 => 0, -2 => 1, etc.,
				//	hence the bitwise not operator "~" below) :

				if (!string.IsNullOrEmpty (groups))
				{
					this.records.Insert (~index, record);
				}
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

					//	If the visual has no validation groups associated with it,
					//	we can safely remove the tracking record :
					
					if (string.IsNullOrEmpty (groups))
					{
						this.records.RemoveAt (index);
					}
					else
					{
						this.records[index] = newRecord;
					}
				}
			}
		}

		/// <summary>
		/// Refreshes the validation information starting at the specified root
		/// in the visual tree.
		/// </summary>
		/// <param name="root">The root of the visual tree.</param>
		public void Refresh(Visual root)
		{
			List<Record>   records = new List<Record> ();
			CommandContext context = Helpers.VisualTree.GetCommandContext (root);
			
			foreach (Visual child in root.GetAllChildren ())
			{
				System.Diagnostics.Debug.Assert (ValidationContext.GetContext (child) == null);

				if ((child.HasValidator) &&
					(child.HasValidationGroups))
				{
					bool enable;

					enable = ValidationContext.GetVisualValidity (child);

					records.Add (new Record (child.GetVisualSerialId (), child.ValidationGroups, enable));
				}
			}

			records.Sort ();

			//	Walk the record list and check for outdated and new tracking records,
			//	using the information to update the group disables in an efficient
			//	way :
			
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
					//	There is a new record, not yet known to the command context.
					//	Synchronize the disable information.

					if (newRecord.Enable == false)
					{
						this.DisableGroups (context, newRecord.Groups);
					}

					i++;
				}
				else
				{
					//	There is an old record which effect must be undone.
					//	Synchronize the disable information.

					if (oldRecord.Enable == false)
					{
						this.EnableGroups (context, oldRecord.Groups);
					}

					j++;
				}
			}

			for (; i < newRecords.Count; i++)
			{
				Record newRecord = newRecords[i];

				if (newRecord.Enable == false)
				{
					this.DisableGroups (context, newRecord.Groups);
				}
			}

			for (; j < oldRecords.Count; j++)
			{
				Record oldRecord = oldRecords[j];

				if (oldRecord.Enable == false)
				{
					this.EnableGroups (context, oldRecord.Groups);
				}
			}

			this.records.Clear ();
			this.records.AddRange (records);
		}

		/// <summary>
		/// Gets the visual validity. A visual is also considered to be valid
		/// if it is disabled; a disabled widget cannot break the validity, as
		/// the user wouldn't have any means of fixing the problem
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns><c>true</c> if the visual is valid; otherwise, <c>false</c>.</returns>
		public static bool GetVisualValidity(Visual visual)
		{
			return visual.IsEnabled ? visual.IsValid : true;
		}

		#region IEquatable<ValidationContext> Members

		public bool Equals(ValidationContext other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.uniqueId == other.uniqueId;
			}
		}

		#endregion

		public override int GetHashCode()
		{
			return this.uniqueId.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as ValidationContext);
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.uniqueId);
		}


		/// <summary>
		/// Enables the specified command groups.
		/// </summary>
		/// <param name="context">The command context.</param>
		/// <param name="groups">The command groups.</param>
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

		/// <summary>
		/// Disables the specified command groups.
		/// </summary>
		/// <param name="context">The command context.</param>
		/// <param name="groups">The command groups.</param>
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

			public long							Visual
			{
				get
				{
					return this.visual;
				}
			}
			
			public bool							Enable
			{
				get
				{
					return this.enable;
				}
			}
			
			public string						Groups
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

			private readonly long				visual;
			private readonly string				groups;
			private readonly bool				enable;
		}
		
		#endregion

		/// <summary>
		/// Gets the validation context associated with an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The <see cref="ValidationContext"/> or <c>null</c>.</returns>
		public static ValidationContext GetContext(DependencyObject obj)
		{
			return (ValidationContext) obj.GetValue (ValidationContext.ContextProperty);
		}

		/// <summary>
		/// Sets (or clears) the validation context associated with an object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="context">The validation context.</param>
		public static void SetContext(DependencyObject obj, ValidationContext context)
		{
			if (context == null)
			{
				obj.ClearValue (ValidationContext.ContextProperty);
			}
			else
			{
				obj.SetValue (ValidationContext.ContextProperty, context);
			}
		}


		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (ValidationContext), typeof (ValidationContext));

		static long								nextUniqueId;
		
		readonly long							uniqueId;
		readonly List<Record>					records;
		readonly Dictionary<string, int>		groupDisables;
	}
}
