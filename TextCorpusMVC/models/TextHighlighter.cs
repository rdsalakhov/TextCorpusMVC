using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Documents;
using TextCorpusMVC.Models;
using System.Windows.Media;
using System.IO;

namespace TextCorpusMVC.Models
{
    public class TextHighlighter
    {
        static Color rangeHighlitingColor = Color.FromArgb(185, 250, 230, 0);
        public static FlowDocument GetHighlightedText(string text, List<Tag> tags)
        {

            var doc = new FlowDocument();
            TextRange initialText = new TextRange(doc.ContentStart, doc.ContentEnd);
            initialText.Text = text;

            foreach (var tag in tags)
            {
                Random rnd = new Random(tag.NameId.GetHashCode());
                var colorBytes = new byte[3];
                rnd.NextBytes(colorBytes);
                Color color = Color.FromArgb(185, colorBytes[0], colorBytes[1], colorBytes[2]);

                TextRange range = GetTextRange(tag.StartPos, tag.EndPos, doc);
                var oldColor = range.GetPropertyValue(TextElement.BackgroundProperty);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
            }
            return doc;
        }



        private static TextRange GetTextRange(int start, int end, FlowDocument doc)
        {
            var startPointer = GetTextPointerAtOffset(doc, start);
            var endPointer = GetTextPointerAtOffset(doc, end);
            return new TextRange(startPointer, endPointer);
        }

        private static TextPointer GetTextPointerAtOffset(FlowDocument doc, int offset)
        {
            var navigator = doc.ContentStart;
            int cnt = 0;

            while (navigator.CompareTo(doc.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }

        public static FlowDocument HighlighAtRange(FlowDocument doc, int startPos, int endPos)
        {

            TextRange tr = new TextRange(GetTextPointerAtOffset(doc, startPos), GetTextPointerAtOffset(doc, endPos));
            tr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(rangeHighlitingColor));
            return doc;
        }
    }
}