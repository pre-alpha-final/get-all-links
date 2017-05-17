namespace GetAllLinks.Core.Infrastructure.POs
{
	public interface IDownloadable
	{
		string Url { get; set; }
		string Name { get; set; }
		void UpdateProgress(double completion, int speed);
	}
}
