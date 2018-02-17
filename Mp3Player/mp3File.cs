using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mp3Player.Annotations;
using TagLib;
using WMPLib;

namespace Mp3Player
{
	public class Mp3File
	{
		public static Mp3File CurrentPlaying { get; set; }
		public static WindowsMediaPlayer _player = new WindowsMediaPlayer();

		public string Path { get; }
		public string Duration { get; }
		public string Title { get; }
		public string FullTitle //Title that has author
			=> Author + " - " +  Title;

		public string Author { get; }

		static Mp3File()
		{

		}

		public Mp3File(string path)
		{
			Path = path;
			File f = File.Create(path);
			Duration = String.Format("{0}:{1:00}", f.Properties.Duration.Minutes, f.Properties.Duration.Seconds);
			Title = f.Tag.Title;
			Author = f.Tag.FirstPerformer;
		}

		public void Play(MainWindow resource)
		{
			CurrentPlaying = this;
			_player.URL = Path;
			_player.controls.play();
			resource.TitleTextBox.Text = CurrentPlaying.FullTitle;
			resource.PlayButton.Visibility = Visibility.Collapsed;
			resource.PauseButton.Visibility = Visibility.Visible;

		}

		public void Pause(MainWindow resource)
		{
			resource.PlayButton.Visibility = Visibility.Visible;
			resource.PauseButton.Visibility = Visibility.Collapsed;
			_player.controls.pause();
		}
	}
}
