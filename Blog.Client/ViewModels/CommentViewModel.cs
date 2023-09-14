namespace Blog.Client.ViewModels
{
	public class CommentViewModel
	{
		public string User { get; set; } = string.Empty;
		public DateTime DateOfCreation { get; set; }
		public string Content { get; set; } = string.Empty;
	}
}
