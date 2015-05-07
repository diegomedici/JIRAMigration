namespace JIRAMigration.Classes
{
    public class CommentDetail
    {
        public string self { get; set; }
        public string id { get; set; }
        public Author author { get; set; }
        public string body { get; set; }
        public UpdateAuthor updateAuthor { get; set; }
        public string created { get; set; }
        public string updated { get; set; } 
    }
}