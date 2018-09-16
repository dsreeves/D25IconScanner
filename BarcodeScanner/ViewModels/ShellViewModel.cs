using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using BarcodeScanner.Views;
using BarcodeScanner.Models;
using System.Windows.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace BarcodeScanner.ViewModels
{
	public class ShellViewModel : Screen, INotifyPropertyChanged
	{
		#region VM Properties

		//INI Properties
		public string FormName { get; set; } = "DefaultName";

		//Misc Properties
		public string CurrentTime     { get; set; } = DateTime.Now.ToString("HH:mm");
		public string CurrentDate     { get; set; } = DateTime.Now.ToShortDateString();

		//Model for all scanner data
		public Scanner Scanner { get; set; } = new Scanner();

		#endregion

		public ShellViewModel()
		{
			
		}
	}


}
