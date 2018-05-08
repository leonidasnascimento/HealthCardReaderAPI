using System;
using System.Collections.Generic;

namespace API.Models
{
    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
        public int Length { get; internal set; }

        public static implicit operator int(Word v)
        {
            throw new NotImplementedException();
        }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public List<Line> lines { get; set; }
    }

    public class ComputerVisionOCR
    {
        public string language { get; set; }
        public string orientation { get; set; }
        public double textAngle { get; set; }
        public List<Region> regions { get; set; }
    }
}