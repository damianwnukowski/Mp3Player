using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Media;
using Microsoft.Win32;
using WMPLib;

namespace Mp3Player
{
	
	
	/// <summary>
	/// Logika interakcji dla klasy MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ObservableCollection<Mp3File> Mp3Files { get; set; }
		public BitmapImage DefaultArtwork { get; }

		public MainWindow()
		{
			InitializeComponent();
			Mp3Files = new ObservableCollection<Mp3File>();
			DefaultArtwork = new BitmapImage();
			DefaultArtwork.BeginInit();
			DefaultArtwork.UriSource = new Uri("Resource Images/DefaultArtwork.png", UriKind.Relative);
			DefaultArtwork.EndInit();
		}

		private void OpenButton_OnClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			openFileDialog.Filter = "mp3 files (*.mp3)|*.mp3";
			openFileDialog.ShowDialog();


			if (openFileDialog.FileNames.Length > 0)
			{
				Mp3File firstMp3 = new Mp3File(openFileDialog.FileNames[0]);
				Mp3Files.Add(firstMp3);
				for (int i = 1; i < openFileDialog.FileNames.Length; i++)
				{
					Mp3Files.Add(new Mp3File(openFileDialog.FileNames[i]));
				}
				firstMp3.Play(this);
			}

		}

		private void ResumeButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Mp3File.CurrentPlaying == null)
				return;
			Mp3File.CurrentPlaying.Resume(this);
		}

		private void PauseButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Mp3File.CurrentPlaying == null)
				return;
			Mp3File.CurrentPlaying.Pause(this);
		}

		
	}
}
