using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using WMPLib;

namespace Mp3Player
{
	
	
	/// <summary>
	/// Logika interakcji dla klasy MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private BackgroundWorker _backgroundWorker;

		public ObservableCollection<Mp3File> Mp3Files { get; set; }
		private BitmapImage _defaultArtwork { get; }
		private Mp3File _currentPlaying { get; set; }
		private WindowsMediaPlayer _player;


		public MainWindow()
		{
			InitializeComponent();
			Mp3Files = new ObservableCollection<Mp3File>();
			_player = new WindowsMediaPlayer();
			_defaultArtwork = new BitmapImage();
			_defaultArtwork.BeginInit();
			_defaultArtwork.UriSource = new Uri("Resource Images/DefaultArtwork.png", UriKind.Relative);
			_defaultArtwork.EndInit();
			_player.PlayStateChange += PlayStateChange;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
			_backgroundWorker.WorkerReportsProgress = true;
			_backgroundWorker.RunWorkerAsync();
		}

		public void Play(Mp3File mp3File)
		{
			_currentPlaying = mp3File;
			AlbumCoverImage.Source = mp3File.AlbumCover != null ? _currentPlaying.AlbumCover : _defaultArtwork;
			_player.URL = mp3File.FilePath;
			TitleTextBox.Text = _currentPlaying.FullTitle;
			var timeSpan = TimeSpan.FromSeconds(_currentPlaying.MaxPosition);
			MaxPositionTextBlock.Text = String.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
		}

		//change buttons from resume to pause
		private void PlayStateChange(int newState)
		{
			switch (newState)
			{
				case (int)WMPPlayState.wmppsPlaying:
					ResumeButton.Visibility = Visibility.Collapsed;
					PauseButton.Visibility = Visibility.Visible;
					break;			
				default:
					ResumeButton.Visibility = Visibility.Visible;
					PauseButton.Visibility = Visibility.Collapsed;
					break;

			}
		}
		
		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (;;)
			{
				Thread.Sleep(5);
				_backgroundWorker.ReportProgress(DateTime.Now.Millisecond);
			}
		}
		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{

			//On app start CurrentPlaying object is null so we must check for it before going further
			if (_currentPlaying == null)
				return;
			var index = Mp3Files.IndexOf(_currentPlaying);
			if (_player.playState==WMPPlayState.wmppsStopped)
			{
				if(index < Mp3Files.Count - 1)
					Play(Mp3Files[index + 1]); //play next song when there is any and player stopped which happens only on media end, but because we can miss media end state we use this, which works fine
				else
					Play(Mp3Files[0]);
			}
			//Progress bar and time progress updating
			var timeSpan = TimeSpan.FromSeconds(_player.controls.currentPosition);
			CurrentPositionTextBlock.Text = String.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
			ProgressBar.Value = _player.controls.currentPosition/_currentPlaying.MaxPosition * 100;
		}
		
		private void OpenButton_OnClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "mp3 files (*.mp3)|*.mp3"
			};
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

		private void OpenDirectory_OnClick(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.InitialDirectory = "C:\\Users";
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				var files = Directory.GetFiles(dialog.FileName, "*.mp3");
				Mp3File firstMp3 = new Mp3File(files[0]);
				Mp3Files.Add(firstMp3);
				for (int i = 1; i < files.Length; i++)
				{
					Mp3Files.Add(new Mp3File(files[i]));
				}
				Play(firstMp3);
			}
		}


		private void ResumeButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentPlaying == null)
				return;
			_player.controls.play();
		}

		private void PauseButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentPlaying == null)
				return;
			_player.controls.pause();
		}	

		//setting position of the mp3 file that is playing by pressing on progress bar
		private void ProgressBar_OnMouseLeftButtonDown(object sender, MouseEventArgs e)
		{
			double percentagePosition = e.GetPosition(ProgressBar).X / ProgressBar.ActualWidth * 100;
			if (_currentPlaying != null && e.LeftButton == MouseButtonState.Pressed)			
				_player.controls.currentPosition = percentagePosition/100*_currentPlaying.MaxPosition;		
		}

		private void NextButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentPlaying == null)
				return;
			var index = Mp3Files.IndexOf(_currentPlaying);
			if (index < Mp3Files.Count - 1)
			{
				Play(Mp3Files[index + 1]);
			}
			else
			{
				Play(Mp3Files[0]);
			}
		}

		private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentPlaying == null)
				return;
			var index = Mp3Files.IndexOf(_currentPlaying);
			if (index > 0)  
			{
				Play(Mp3Files[index - 1]);
			}
			else
			{
				Play(Mp3Files[Mp3Files.Count-1]);
			}
		}

		private void ListViewItemHandleDoubleClick(object sender, RoutedEventArgs e)
		{
			Mp3File mp3File = (Mp3File)((ListViewItem)sender).Content;
			Play(mp3File);
		}

		private void ListViewItemHandleKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key==Key.Enter) //does the same what doubleclick just checks if the key was enter
				ListViewItemHandleDoubleClick(sender, e);
		}
	}
}
