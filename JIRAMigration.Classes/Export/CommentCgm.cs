namespace JIRAMigration.Classes.Export
{
    public class CommentCgm
    {
        public CommentCgm(CommentDetail commentDetail, string projectWithDash, string destProjectWithDash)
        {
            Body = commentDetail.body.Replace(projectWithDash, destProjectWithDash);
            Author = commentDetail.author.emailAddress.Replace("@studiofarma.it", "@cgm.com");
            Created = commentDetail.created;
        }

        public CommentCgm()
        {
            
        }

        public string Created { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }
    }
}