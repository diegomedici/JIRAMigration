namespace JIRAMigration.Classes.Export
{
    public class CommentCgm
    {
        public CommentCgm(CommentDetail commentDetail)
        {
            Body = commentDetail.body;
            Author = commentDetail.author.emailAddress;
            Created = commentDetail.created;
        }

        public string Created { get; set; }

        public string Author { get; set; };

        public string Body { get; set; }
    }
}