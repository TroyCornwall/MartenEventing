using System;

namespace PoCCommon.Database.Models
{
    public class Watermark
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "PoCEventHandler";
        public long LastSequenceId { get; set; } = 0;
    }
}