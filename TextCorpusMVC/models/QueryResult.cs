using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextCorpusMVC.Models
{
    public class QueryResult
    {
        static int _cutOffset = 90;
        public string TextName { get; }
        public string Result { get; set; }

        public List<Tag> Tags { get; private set; }


        public List<int> StartPositions { get; set; }
        public List<int> EndPositions { get; set; }

        public QueryResult()
        { }

        public QueryResult(string textName, string text, List<int> startPositions, List<int> endPositions)
        {
            this.TextName = textName;
            this.Result = text;
            this.StartPositions = startPositions;
            this.EndPositions = endPositions;
            CutText();
        }

        private void CutText()
        {
            int lesserHighlightIndex = StartPositions.Min();
            int greaterHighlightIndex = EndPositions.Max();

            int startCutIndex = lesserHighlightIndex - _cutOffset < 0 ? 0 : lesserHighlightIndex - _cutOffset;
            int cutLenght = greaterHighlightIndex - lesserHighlightIndex + startCutIndex + _cutOffset * 2 > Result.Length ? Result.Length - startCutIndex : greaterHighlightIndex - lesserHighlightIndex + _cutOffset * 2;

            Result = Result.Substring(startCutIndex, cutLenght);
            if (startCutIndex != 0)
            {
                Result = Result.Insert(0, "...");
                startCutIndex -= 3;
            }
            for (int i = 0; i < StartPositions.Count; i++)
            {
                StartPositions[i] -= startCutIndex;
                EndPositions[i] -= startCutIndex;
            }

            if (cutLenght == greaterHighlightIndex - lesserHighlightIndex + _cutOffset * 2)
            {
                Result = Result.Insert(Result.Length, "...");
            }

        }
    }
}