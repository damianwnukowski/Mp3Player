using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WMPLib;
using File = TagLib.File;


namespace Mp3Player
{
	public class Mp3File
	{
		public static Mp3File CurrentPlaying { get; set; }
		public static WindowsMediaPlayer Player = new WindowsMediaPlayer();

		public double MaxPosition;
		public string FilePath { get; }
		public string Duration { get; }
		public string Title { get; }
		public BitmapImage AlbumCover { get; }
		public string Author { get; }

		public string FullTitle //Title that begins with author
		{
			get
			{
				if (Title == null || Author == null)
				{
					return Path.GetFileName(FilePath);
				}

				return Author + " - " + Title;
			}
		}

		


		public Mp3File(string path)
		{
			FilePath = path;
			File f = File.Create(path);
			Duration = String.Format("{0}:{1:00}", f.Properties.Duration.Minutes, f.Properties.Duration.Seconds);
			Title = f.Tag.Title;
			Author = f.Tag.FirstPerformer;

			//retrieving album cover as byte array and convering it into bitmapimage
			if (f.Tag.Pictures.Length != 0)
			{
				var img = f.Tag.Pictures[0].Data.Data;
				AlbumCover = new BitmapImage();
				AlbumCover.BeginInit();
				AlbumCover.StreamSource = new MemoryStream(img);
				AlbumCover.EndInit();
			}
		}

		public void Play(MainWindow w)
		{
			CurrentPlaying = this;

			if (AlbumCover != null)
				w.AlbumCover.Source = AlbumCover;
			else
				w.AlbumCover.Source = w.DefaultArtwork;

			Player.URL = FilePath;
			Player.controls.play();
			w.TitleTextBox.Text = CurrentPlaying.FullTitle;
			w.ResumeButton.Visibility = Visibility.Collapsed;
			w.PauseButton.Visibility = Visibility.Visible;

		}

		public void Pause(MainWindow w)
		{
			w.ResumeButton.Visibility = Visibility.Visible;
			w.PauseButton.Visibility = Visibility.Collapsed;
			Player.controls.pause();
		}

		public void Resume(MainWindow w)
		{
			w.ResumeButton.Visibility = Visibility.Collapsed;
			w.PauseButton.Visibility = Visibility.Visible;
			Player.controls.play();
		}

	}
}
