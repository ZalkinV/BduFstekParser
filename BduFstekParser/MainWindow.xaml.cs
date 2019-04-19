using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace BduFstekParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal List<ThreatEntry> threatEntries { get; set; }
		private string threatFileName;
		private string threatFileUrl;

		public MainWindow()
		{
			InitializeComponent();

			threatEntries = new List<ThreatEntry>();
			listViewThreatEntries.ItemsSource = threatEntries;

			threatFileName = "thrlist.xlsx";
			threatFileUrl = "https://bdu.fstec.ru/documents/files/thrlist.xlsx";
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Thread fillingListView = new Thread(FillThreatsListView);
			fillingListView.Start();
			fillingListView.Join();
		}

		private void FillThreatsListView()
		{
			PrepareFile();
			GetFileData();
		}

		private void PrepareFile()
		{
			if (!File.Exists(threatFileName))
			{
				MessageBoxResult userChoice = MessageBox.Show("Файл с УБИ не найден.\nСкачать его с сайта ФСТЭК?", "Проверка наличия файла", MessageBoxButton.YesNo);
				if (userChoice == MessageBoxResult.Yes)
				{
					using (WebClient webClient = new WebClient())
					{
						webClient.DownloadFile(new Uri(threatFileUrl), threatFileName);
					}
				}
			}
		}

		private void GetFileData()
		{
			Excel.Application excel = new Excel.Application();
			Excel.Workbook excelWorkbook = excel.Workbooks.Open(Directory.GetCurrentDirectory() + "/" + threatFileName, 0, true);
			Excel.Worksheet excelSheet = excelWorkbook.Sheets[1];
			Excel.Range excelRange = excelSheet.UsedRange;
		}
	}
}
