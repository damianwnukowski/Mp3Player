using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Mp3Player
{
	public class Mp3File
	{
		public string Path { get; }
		public string Duration { get; }
		public string Title { get; }
		public string FullTitle //Title that has author
			=> Author + " - " +  Title;

		public string Author { get; }

		public Mp3File(string path)
		{
			Path = path;
			File f = File.Create(path);
			Duration = String.Format("{0}:{1:00}", f.Properties.Duration.Minutes, f.Properties.Duration.Seconds);
			Title = f.Tag.Title;
			Author = f.Tag.FirstPerformer;
		}
	}
}
