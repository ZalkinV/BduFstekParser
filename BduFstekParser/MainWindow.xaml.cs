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

namespace BduFstekParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal List<ThreatEntry> threatEntries { get; set; }
		private string threatFileName = "thrlist.xlsx";

		public MainWindow()
		{
			InitializeComponent();

			listViewThreatEntries.ItemsSource = threatEntries;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (File.Exists(threatFileName))
			{
				MessageBox.Show("Файл с УБИ найден!");
			}
			else
			{
				MessageBox.Show("Файл с УБИ не найден.");
			}
		}
	}
}
