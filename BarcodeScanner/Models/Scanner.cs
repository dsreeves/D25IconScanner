using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CoreScanner;

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


		/// <summary>
		/// Captures Sync context to marshall barcode events back to UI thread
		/// </summary>
		private SynchronizationContext uiSyncContext;
		private Barcode _lastScan;

		#region Barcode Scanner API Config

		private CCoreScanner cCoreScannerClass = new CCoreScanner();
		private string outXML;
		private int status = 1;
		private bool opReady = false;
		private bool ScanDisabled = false;

		public bool sConnected = false;
		public string _Operator { get; set; }
		public string _Fixture { get; set; }
		public string _DateTime { get; set; }
		public string _RawIN { get; set; }
		public enum ExecCodes
		{
			//Scanner SDK Commands
			GET_VERSION = 1000,
			REGISTER_FOR_EVENTS = 1001,
			UNREGISTER_FOR_EVENTS = 1002,

			//General Commands
			AIM_OFF = 2002,
			AIM_ON = 2003,
			DEVICE_PULL_TRIGGER = 2011,
			DEVICE_RELEASE_TRIGGER = 2012,
			SCAN_DISABLE = 2013,
			SCAN_ENABLE = 2014,
			SET_PARAMETER_DEFAULTS = 2015,
			DEVICE_SET_PARAMETERS = 2016,
			SET_PARAMETER_PERSISTANCE = 2017,
			REBOOT_SCANNER = 2019,

			//Operational modes
			DEVICE_CAPTURE_IMAGE = 3000,
			DEVICE_CAPTURE_BARCODE = 3500,
			DEVICE_CAPTURE_VIDEO = 4000,

			//Attributes
			ATTR_GETALL = 5000,
			ATTR_GET = 5001,
			ATTR_SET = 5004,
			ATTR_STORE = 5005,

			//Used to invoke beeps, leds, ect..
			SET_ACTION = 6000,
			_1HighS = 0,
			_2HighS = 1,
			_3HighS = 2,
			_4HighS = 3,
			_5HighS = 4,
			_1LowS = 5,
			_2LowS = 6,
			_3LowS = 7,
			_4LowS = 8,
			_5LowS = 9,
			_1HighL = 10,
			_2HighL = 11,
			_3HighL = 12,
			_4HighL = 13,
			_5HighL = 14,
			_1LowL = 15,
			_2LowL = 16,
			_3LowL = 17,
			_4LowL = 18,
			_5LowL = 19,
			_HighLow = 22,
			_LowHigh = 23,
			_HLH = 24,
			_LHL = 25,
			_HHLL = 26,
			_GreenLEDOff = 42,
			_GreenLEDOn = 43,
			_RedLEDOff = 48,
			_RedLEDOn = 47,

			//Start(0), Stop(1)
			HostTriggerSession = 6005,

			//Event ID's
			SUBSCRIBE_BARCODE = 1,
			SUBSCRIBE_IMAGE = 2,
			SUBSCRIBE_VIDEO = 4,
			SUBSCRIBE_RMD = 8,
			SUBSCRIBE_PNP = 16,
			SUBSCRIBE_OTHER = 32,
		};
		#endregion


		public Scanner()
		{
			Log($"Scanner Constructor run on thread ");
			iniScanner();
			ScannerDebug();
			uiSyncContext = SynchronizationContext.Current;
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

		#region Ini scanner test
		private void iniScanner()
		{
			short[] scannerTypes = new short[1];                        // Scanner Types you are interested in
			scannerTypes[0] = 2;                                        // 1 for all scanner types, 2 for SNAPI mode only
			short numberOfScannerTypes = 1;                             // Size of the scannerTypes array
			cCoreScannerClass.Open(0, scannerTypes, numberOfScannerTypes, out status);
			Log("Created scanner class on thread ");
			ScannerDisable();
			CreateNewBarcodeEvent();
			ScannerEnable();
			WaitForOpScan();
		}

		private async void WaitForOpScan()
		{
			await Task.Run(() => {
				Log("Waiting for Operator scan on thread ");
				while (!opReady)
				{
					ChangeLed(ExecCodes._RedLEDOn);
					Thread.Sleep(350);
					ChangeLed(ExecCodes._GreenLEDOn);
					Thread.Sleep(350);
				} });
			Log("Finished waiting for Operator scan on thread ");
			ChangeLed(ExecCodes._GreenLEDOn);
		}

		private void CreateNewBarcodeEvent()
		{
			//Barcode scan event
			cCoreScannerClass.BarcodeEvent += new _ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);
			// Method for Subscribe events
			string inXML = "<inArgs>" +
								"<cmdArgs>" +
									"<arg-int>1</arg-int>" +            // Number of events you want to subscribe
									"<arg-int>1</arg-int>" +            // Comma separated event IDs
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand(1001, ref inXML, out outXML, out status);
			Log("Created event on thread ");
			Thread.Sleep(200);
		}

		private void CheckForConn()
		{
			short numberOfScanners;
			int[] connectedScannerIDList = new int[255];
			cCoreScannerClass.GetScanners(out numberOfScanners, connectedScannerIDList,
			out outXML, out status);

			if (outXML.ToString().Length < 100)
			{
				Thread.Sleep(500);
			}
			else
			{
				sConnected = true;
			}
		}

		private void OnBarcodeEvent(short eventType, ref string pscanData)
		{
			Log("Barcode Event Fired on thread ");
			_DateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
			string dat = pscanData;
			if (dat != "")
			{
				DecodeXml(dat);
				Debug.WriteLine($"Barcode => {_RawIN}");

				//Update UI Thread
				uiSyncContext.Post((s) =>
				{
					DataGrid.Add(new Barcode("BarcodeEvent", "BarcodeEventRaised", 1));
					Log($"New Barcode class added on thread ");
				}, null);

				//if (_RawIN.StartsWith("#"))
				//{
				//	if (!opReady)
				//	{
				//		InvokeBeep(ExecCodes._HHLL);
				//		ChangeLed(ExecCodes._RedLEDOff);
				//		opReady = true;
				//	}
				//	_Operator = _RawIN.Substring(1);
				//	return;
				//}
				//else
				//{
				//	//Console.WriteLine(_RawIN);
				//	//_Operator = "Test";
				//	//_Fixture = "3";

				//	//Failed Duplicate Barcode Check
				//	//if (!WriteBarcode(this))
				//	//{
				//	//    FailFlag();
				//	//    return;
				//	//}

				//	//IniScanner();
				//	//ScannerDisable();
				//}
			}
		}

		private void ScannerDisable()
		{
			ChangeLed(ExecCodes._RedLEDOn);
			ExecMiscCmd(ExecCodes.SCAN_DISABLE);
		}

		/// <summary>
		/// Output a message with the current thread ID appended
		/// </summary>
		/// <param name="message"></param>
		private static void Log(string message)
		{
			// Write line
			Debug.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
		}

		private void ScannerEnable()
		{
			ChangeLed(ExecCodes._GreenLEDOn);
			ExecMiscCmd(ExecCodes.SCAN_ENABLE);
		}

		private void FailFlag()
		{
			ExecMiscCmd(ExecCodes.SCAN_ENABLE);
			InvokeBeep(ExecCodes._LHL);
			ChangeLed(ExecCodes._RedLEDOn);
			Thread.Sleep(250);
			ChangeLed(ExecCodes._GreenLEDOn);
			Thread.Sleep(250);
			ChangeLed(ExecCodes._RedLEDOn);
			Thread.Sleep(250);
			ChangeLed(ExecCodes._GreenLEDOn);

		}

		private void InvokeBeep(ExecCodes value, int id = 1)
		{
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" + //Scanner ID
								"<cmdArgs>" +
									$"<arg-int>{(int)value}</arg-int>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.SET_ACTION, ref inXML, out outXML, out status);
		}

		/// <summary>
		/// Change LED state/color
		/// </summary>
		/// <param name="value"></param>
		/// <param name="id">Scanner ID</param>
		private void ChangeLed(ExecCodes value, int id = 1)
		{
			string outXML;
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" + //Scanner ID
								"<cmdArgs>" +
									$"<arg-int>{(int)value}</arg-int>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.SET_ACTION, ref inXML, out outXML, out status);
		}


		/// <summary>
		/// Misc. scanner function call.
		/// </summary>
		/// <param name="value">opcode</param>
		/// <param name="id">scanner id</param>
		private void ExecMiscCmd(ExecCodes opcode, int id = 1)
		{
			ScanDisabled = ((int)opcode == 2013) ? true : false;
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" +
								"</inArgs>"; ;

			cCoreScannerClass.ExecCommand((int)opcode, ref inXML, out outXML, out status);

		}

		private string GetAssets()
		{
			string inXML = "<inArgs>" +
							"<scannerID>1</scannerID>" +
								// The scanner you need to get the information
								"<cmdArgs>" +
									"<arg-xml>" +
										"<attrib_list>20004,533,20007,1</attrib_list>" +
									// attribute numbers you need
									"</arg-xml>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.ATTR_GET, ref inXML, out outXML, out status);
			return outXML;
		}

		private void DecodeXml(string strXml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(strXml);

			string strData = String.Empty;
			string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
			string symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
			string[] numbers = barcode.Split(' ');

			foreach (string number in numbers)
			{
				if (String.IsNullOrEmpty(number))
				{
					break;
				}

				strData += ((char)Convert.ToInt32(number, 16)).ToString();
			}

			_RawIN = strData;
		}

		public void Disconnect()
		{
			cCoreScannerClass.Close(0, out status);
		}

		#endregion
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
			Debug.WriteLine($"Creating Barcode class instance on thread [{Thread.CurrentThread.ManagedThreadId}]");
			Operator = op;
			Serial = ser;
			Fixture = i;
			DateTime = DateTime.Now;
		}
	}
}
