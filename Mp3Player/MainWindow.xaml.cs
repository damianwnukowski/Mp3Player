using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WMPLib;

namespace Mp3Player
{
	
	
	/// <summary>
	/// Logika interakcji dla klasy MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private BackgroundWorker backgroundWorker;
		public ObservableCollection<Mp3File> Mp3Files { get; set; }
		public BitmapImage DefaultArtwork { get; }
		public Mp3File CurrentPlaying { get; set; }

		public WindowsMediaPlayer Player = new WindowsMediaPlayer();


		public MainWindow()
		{
			InitializeComponent();
			Mp3Files = new ObservableCollection<Mp3File>();
			DefaultArtwork = new BitmapImage();
			DefaultArtwork.BeginInit();
			DefaultArtwork.UriSource = new Uri("Resource Images/DefaultArtwork.png", UriKind.Relative);
			DefaultArtwork.EndInit();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (;;)
			{
				Thread.Sleep(5);
				backgroundWorker.ReportProgress(DateTime.Now.Millisecond);
			}
		}

		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressBar.Value = Player.controls.currentPosition/CurrentPlaying.MaxPosition * 100;
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
				Play(firstMp3);
			}
		}

		private void ResumeButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (CurrentPlaying == null)
				return;
			Resume();
		}

		private void PauseButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (CurrentPlaying == null)
				return;
			Pause();
		}

		public void Play(Mp3File mp3File)
		{
			CurrentPlaying = mp3File;
			AlbumCoverImage.Source = mp3File.AlbumCover != null ? CurrentPlaying.AlbumCover : DefaultArtwork;
			Player.URL = mp3File.FilePath;
			Player.controls.play();
			TitleTextBox.Text = CurrentPlaying.FullTitle;
			ResumeButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Visible;
			//Progress bar and mp3 position updating
			backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
			backgroundWorker.WorkerReportsProgress = true;
			backgroundWorker.RunWorkerAsync();
		}

		public void Pause()
		{
			ResumeButton.Visibility = Visibility.Visible;
			PauseButton.Visibility = Visibility.Collapsed;
			Player.controls.pause();
		}

		public void Resume()
		{
			ResumeButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Visible;
			Player.controls.play();
		}
	}
}
