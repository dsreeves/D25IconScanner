using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using BarcodeScanner.ViewModels;



namespace BarcodeScanner.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window
	{
		public ShellView()
		{
			InitializeComponent();
			Debug.WriteLine($"Finished Initalizing ShellView on thread [{Thread.CurrentThread.ManagedThreadId}]");
		}
	}
}
