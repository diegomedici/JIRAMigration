﻿namespace JIRAMigration.Classes
{
    public class Attachment
    {
        public string self { get; set; }
        public string filename { get; set; }
        public Author author { get; set; }
        public string created { get; set; }
        public int size { get; set; }
        public string mimeType { get; set; }
        public Properties properties { get; set; }
        public string content { get; set; }
        public string thumbnail { get; set; } 
    }
}