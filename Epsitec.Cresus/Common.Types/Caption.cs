//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Caption : DependencyObject
	{
		public Caption()
		{
		}

		public ICollection<string> Labels
		{
			get
			{
				if (this.labels == null)
				{
					this.labels = new Collections.HostedList<string> (this.HandleLabelInsertion, this.HandleLabelRemoval);
				}
				
				return this.labels;
			}
		}
		
		public IEnumerable<string> SortedLabels
		{
			get
			{
				if (this.sortedLabels == null)
				{
					this.RefreshSortedLabels ();
				}
				
				return this.sortedLabels;
			}
		}

		public string Description
		{
			get
			{
				return "";
			}
		}

		public string HelpReference
		{
			get
			{
				return null;
			}
		}


		private void RefreshSortedLabels()
		{
			string[] labels = this.labels.ToArray ();

			System.Array.Sort (labels, new StringLengthComparer ());
			
			this.sortedLabels = labels;
		}
		
		private void HandleLabelInsertion(string value)
		{
			this.sortedLabels = null;
		}

		private void HandleLabelRemoval(string value)
		{
			this.sortedLabels = null;
		}

		#region StringLengthComparer Class

		private class StringLengthComparer : IComparer<string>
		{
			#region IComparer<string> Members

			public int Compare(string x, string y)
			{
				int lengthX = string.IsNullOrEmpty (x) ? 0 : x.Length;
				int lengthY = string.IsNullOrEmpty (y) ? 0 : y.Length;

				if (lengthX < lengthY)
				{
					return -1;
				}
				else if (lengthX > lengthY)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

			#endregion
		}

		#endregion

		private static object GetLabelsValue(DependencyObject o)
		{
			Caption that = (Caption) o;
			return that.Labels;
		}
		
		public static readonly DependencyProperty LabelsProperty = DependencyProperty.RegisterReadOnly ("Labels", typeof (ICollection<string>), typeof (Caption), new DependencyPropertyMetadata (Caption.GetLabelsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty DecriptionProperty = DependencyProperty.Register ("Description", typeof (string), typeof (Caption));

		private Collections.HostedList<string> labels;
		private string[] sortedLabels;
	}
}
