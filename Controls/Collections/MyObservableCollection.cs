using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Controls.Collections
{
	/// <summary>
	/// Custom ObservableCollection class which takes FrameworkElements. 
	/// Hook into collection at runtime and set LoadedSotryBoard. 
	/// 
	/// This example binds to a resource on the XAML page: 
	/// "Controls.LoadedSotryBoard = (Storyboard)FindResource("OnLoaded1");"
	/// 
	/// </summary>
	public class MyObservableCollection<T> : ObservableCollection<FrameworkElement>
	{
		private Storyboard unloadedStoryboard;

		private HashSet<int> indexesToRemove = new HashSet<int>();

		public Storyboard LoadedSotryBoard { get; set; }

		public Storyboard UnloadedSotryBoard
		{
			get { return unloadedStoryboard; }
			set
			{
				unloadedStoryboard = value;
				unloadedStoryboard.Completed += UnloadedStoryboardOnCompleted;
			}
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (FrameworkElement item in e.NewItems)
					item.BeginStoryboard(LoadedSotryBoard);
			}
			base.OnCollectionChanged(e);
		}

		protected override void RemoveItem(int index)
		{
			indexesToRemove.Add(index);
			var item = Items[index];
			UnloadedSotryBoard.Begin(item);
		}

		private void UnloadedStoryboardOnCompleted(object sender, EventArgs eventArgs)
		{
			foreach (var i in new HashSet<int>(indexesToRemove))
			{
				base.RemoveItem(i);
				indexesToRemove.Remove(i);
			}
		}

	}
}
