using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using N2e.MarkDown.Abstractions;
using N2e.MarkDown;

namespace C2sc.MarkDown
{
    [TestClass]
    public class MarkDownReaderTests
    {
        [DataTestMethod]
        [DataRow("Any simple text.", 0, MdType.Document, "Any simple text.")]
        [DataRow("Any simple text.\n\rStarting on a new line.", 0, MdType.Document, "Any simple text.\n\rStarting on a new line.")]
        [DataRow("Any simple text.## Header2\n\rStarting on a new line.", 0, MdType.Document, "Any simple text.## Header2\n\rStarting on a new line.")]
        [DataRow("Any simple text.\n\r## Valid Header2\n\rStarting on a new line.", 1, MdType.Header, "Any simple text.\n\r{0}\n\rStarting on a new line.")]
        [DataRow("Any simple text.\n\r##Invalid Header2\n\rStarting on a new line.", 0, MdType.Document, "Any simple text.\n\r##Invalid Header2\n\rStarting on a new line.")]
        [DataRow("## Valid Header2", 1, MdType.Header, "{0}")]
        [DataRow("List items\n\r- First\n\r- Second", 2, MdType.ListItem, "List items\n\r{0}\n\r{1}")]
        [DataRow("List items\n\r* First\n\r* Second", 2, MdType.ListItem, "List items\n\r{0}\n\r{1}")]
        [DataRow("List items\n\r+ First\n\r+ Second", 2, MdType.ListItem, "List items\n\r{0}\n\r{1}")]
        [DataRow("List items\n\r  - First\n\r  - Second", 2, MdType.ListItem, "List items\n\r  {0}\n\r  {1}")]
        [DataRow("Numbered items\n\r  1. First\n\r  2. Second", 2, MdType.NumberedItem, "Numbered items\n\r  {0}\n\r  {1}")]
        [DataRow("[ ] Checkbox", 1, MdType.CheckBox, "{0} Checkbox")]
        [DataRow("[x]  Checkbox", 1, MdType.CheckBox, "{0}  Checkbox")]
        [DataRow("[v] Checkbox", 1, MdType.CheckBox, "{0} Checkbox")]
        [DataRow("```\n\rCode block\n\r```\n\r", 1, MdType.Code, "{0}\n\r")]
        [DataRow("````\n\rCode block\n\r````\n\r", 1, MdType.Code, "{0}\n\r")]
        [DataRow("```C#\n\rCode block\n\r```\n\r", 1, MdType.Code, "{0}\n\r")]
        // > Quote end with double crlf
        // (text)[httplink] hyperlink
        // [anchor] some text
        // ~~~~ \n\rFixed text on newline ends with \n\r~~~~\n\r
        [DataRow("Some text _emphasis_ with emphasis", 1, MdType.Emphasis, "Some text {0} with emphasis")]
        [DataRow("Some text *emphasis* with emphasis", 1, MdType.Emphasis, "Some text {0} with emphasis")]
        [DataRow("Some text __emphasis__ with emphasis", 1, MdType.Bold, "Some text {0} with emphasis")]
        [DataRow("Some text __emphasis with nested *emphasis*__ with emphasis", 1, MdType.Bold, "Some text {0} with emphasis")]
        [DataRow("Some text ~~strikethrough~~ with strikethrough", 1, MdType.Strikethrough, "Some text {0} with strikethrough")]
        [DataRow("Some text `Inline code block` with inline code ignores other elements??", 1, MdType.InlineCode, "Some text {0} with inline code ignores other elements??")]
        // --- *** ___ Horizontal line
        public void ParseDocument(string value, int expectedElementCount, MdType expectType, string expectedContent)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            Assert.IsNotNull(contentBlocks);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentBlocks, Newtonsoft.Json.Formatting.Indented));

            Assert.AreEqual(expectedElementCount, contentBlocks.SubElements.Count);
            Assert.AreEqual(expectedContent, contentBlocks.Content);
            if (expectedElementCount > 0)
            {
                var item = contentBlocks.SubElements[0];
                Assert.IsNotNull(item);
                Assert.AreEqual(expectType, item.Type);
            }            
        }


        [DataTestMethod]
        [DataRow("Some text `Inline code block` with inline code ignores other elements??")]
        [DataRow("Some text `Inline _code_ block` with inline code ignores other elements??")]
        public void ParseCodeElements(string value)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            Assert.IsNotNull(contentBlocks);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentBlocks, Newtonsoft.Json.Formatting.Indented));

            Assert.AreEqual(1, contentBlocks.SubElements.Count);
            Assert.AreEqual(0, contentBlocks.SubElements[0].SubElements.Count);
        }

        [DataTestMethod]
        [DataRow("| Table | Table | Column3 |\n\r| :--- | :---: | ---: \n\r| col1 | col2 |\n\r")]
        [DataRow("| Table | Table | Column3\n\r| --- | :---: | ---: \n\r| col1 | col2 | col3 \n\r")]
        [DataRow("| Table | Table\n\r| --- | :---: | ---: \n\r| col1 | col2 \n\r")]
        public void ParseTables(string value)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentBlocks, Newtonsoft.Json.Formatting.Indented));

            // three rows
            Assert.IsNotNull(contentBlocks);
            Assert.AreEqual(3, contentBlocks.SubElements.Count);
            Assert.AreEqual("{0}\n\r{1}\n\r{2}\n\r", contentBlocks.Content);

            var firstRow = contentBlocks.SubElements[0];
            Assert.IsNotNull(firstRow);
            Assert.AreEqual(MdType.TableRow, firstRow.Type);

            Assert.IsTrue(firstRow.SubElements.Count>=2);
            Assert.AreEqual(MdType.TableCell, firstRow.SubElements[0].Type);
            Assert.AreEqual("Table", firstRow.SubElements[0].Content);

            var secondRow = contentBlocks.SubElements[1];
            Assert.IsNotNull(secondRow);
            Assert.AreEqual(MdType.SplitRow, secondRow.Type);

            Assert.AreEqual(3, secondRow.SubElements.Count);
            Assert.AreEqual(MdType.TableCell, secondRow.SubElements[0].Type);
            Assert.AreEqual(MdType.TableCellCenter, secondRow.SubElements[1].Type);
            Assert.AreEqual(MdType.TableCellRight, secondRow.SubElements[2].Type);
        }

        [DataTestMethod]
        [DataRow("List items\n\r- First\n\r- Second", new int[] { 0, 0 })]
        [DataRow("List items\n\r  - First\n\r  - Second", new int[] { 2, 2 })]
        [DataRow("List items\n\r- First\n\r  - Second", new int[] { 0, 2 })]
        public void ParseListItemWithDepthCount(string value, int[] expectValue)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            Assert.AreEqual(MdType.ListItem, contentBlocks.SubElements[0].Type);
            Assert.AreEqual(expectValue[0], contentBlocks.SubElements[0].Depth);
            Assert.AreEqual("First", contentBlocks.SubElements[0].Content.Trim());

            Assert.AreEqual(MdType.ListItem, contentBlocks.SubElements[1].Type);
            Assert.AreEqual(expectValue[1], contentBlocks.SubElements[1].Depth);
            Assert.AreEqual("Second", contentBlocks.SubElements[1].Content.Trim());
        }
    }

   
}
