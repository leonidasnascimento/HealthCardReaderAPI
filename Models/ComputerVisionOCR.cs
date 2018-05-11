using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Word
    {
        public List<int> BoundingBox { get; set; }
        public string Text { get; set; }
    }

    public class Line
    {
        public List<int> BoundingBox { get; set; }
        public string Text { get; set; }
        public List<Word> Words { get; set; }
    }

    public class RecognitionResult
    {
        public List<Line> Lines { get; set; }
    }

    public class ComputerVisionOCR
    {
        public string Status { get; set; }
        public RecognitionResult RecognitionResult { get; set; }
    }
}