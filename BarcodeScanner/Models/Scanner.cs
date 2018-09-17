using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScanner.Models
{
	public class Scanner : INotifyPropertyChanged
	{
		/// <summary>
		/// VM binding for datagrid
		/// </summary>
		public ObservableCollection<Barcode> DataGrid { get; set; } = new ObservableCollection<Barcode>();
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };


		/// <summary>
		/// VM binding for last scanned barcode
		/// </summary>
		public string LastScanned { get; set; } = "test";
		public string Operator { get; set; } = "test";


		private Barcode _lastScan;

 
		public Scanner()
		{
			ScannerDebug();

		}

		/// <summary>
		/// Debug Constructor
		/// </summary>
		[Conditional("DEBUG")]
		private void ScannerDebug()
		{
			DataGrid.Add(new Barcode("test", "123123123123123", 1));
			DataGrid.Add(new Barcode("test", "123123123123123", 2));
			DataGrid.Add(new Barcode("test", "123123123123123", 3));
			DataGrid = new ObservableCollection<Barcode>(DataGrid.Reverse());
		}




	}


	/// <summary>
	/// Unique Obj class for every part scanned
	/// </summary>
	public class Barcode 
	{
		public string Operator   { get;  set; } = "null";
		public string Serial     { get;  set; } = "123456789Test";
		public int Fixture       { get;  set; } = 1;
		public DateTime DateTime { get;  set; }

		public Barcode(string op, string ser, int i)
		{
			Operator = op;
			Serial = ser;
			Fixture = i;
			DateTime = DateTime.Now;
		}
	}
}
