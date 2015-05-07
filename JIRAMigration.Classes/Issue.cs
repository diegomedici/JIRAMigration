namespace JIRAMigration.Classes
{
    public class IssueSTF
    {
        public string expand { get; set; }
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
        public RenderedFields renderedFields { get; set; }
        public Names names { get; set; }
    }
}