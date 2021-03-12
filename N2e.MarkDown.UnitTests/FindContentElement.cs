using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using N2e.MarkDown.Abstractions;
using N2e.MarkDown;
using System.IO;
using System.Linq;

namespace C2sc.MarkDown
{
    [TestClass]
    public class HtmlWriterTests
    {
        [DataTestMethod]
        [DataRow("Any simple text.", "Any simple text.")]
        [DataRow("Any simple text with {{0}} ignored placeholder.", "Any simple text with {{0}} ignored placeholder.")]
        [DataRow("Any simple text with edge case {", "Any simple text with edge case {")]
        [DataRow("} Any simple text with edge case.", "} Any simple text with edge case.")]
        [DataRow("Any simple text.\r\nStarting on a new line.", "Any simple text.\nStarting on a new line.")]
        [DataRow("Double CR will result in line break. \r\n\r\nStarting on a new line.", "Double CR will result in line break. \n<br />\nStarting on a new line.")]
        [DataRow("Any simple text.\r\n# Header1\r\nStarting on a new line.", "Any simple text.\n<h1> Header1</h1>\nStarting on a new line.")]
        [DataRow("Any simple text.\r\n## Header2\r\nStarting on a new line.", "Any simple text.\n<h2> Header2</h2>\nStarting on a new line.")]
        public void CreateHtmlDocument(string value, string expectedContent)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            string result;
            using(var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                md.WriteHtmlContent(sw, contentBlocks);
                sw.Flush();

                result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }

            Assert.AreEqual(expectedContent, result);
        }

        [TestMethod]
        public void CreateHTMLDocument()
        {
            var file = File.OpenText("C:\\_github\\n2e-Markdown\\DeveloperNotes.md");
            var html = File.OpenWrite("C:\\_github\\n2e-Markdown\\DeveloperNotes.html");
            var md = new MarkDownReader();
            var result = md.ParseStream(file, html, (s) => Console.WriteLine(s), true);
            Assert.IsTrue(result);
        }
    }

    [TestClass]
    public class MarkDownReaderTests
    {
        [DataTestMethod]
        [DataRow("Any simple text.", 0, MdType.Document, "Any simple text.")]
        [DataRow("Any simple text.\r\nStarting on a new line.", 0, MdType.Document, "Any simple text.\r\nStarting on a new line.")]
        [DataRow("Any simple text.## Header2\r\nStarting on a new line.", 0, MdType.Document, "Any simple text.## Header2\r\nStarting on a new line.")]
        [DataRow("Any simple text.\r\n## Valid Header2\r\nStarting on a new line.", 1, MdType.Header, "Any simple text.\r\n{0}\r\nStarting on a new line.")]
        [DataRow("Any simple text.\r\n##Invalid Header2\r\nStarting on a new line.", 0, MdType.Document, "Any simple text.\r\n##Invalid Header2\r\nStarting on a new line.")]
        [DataRow("Ignore placeholders in content {{0}}.", 0, MdType.Document, "Ignore placeholders in content {{0}}.")]
        [DataRow("## Valid Header2", 1, MdType.Header, "{0}")]
        [DataRow("List items\r\n- First\r\n- Second", 1, MdType.List, "List items\r\n{0}\r\n")]
        [DataRow("List items\r\n* First\r\n* Second", 1, MdType.List, "List items\r\n{0}\r\n")]
        [DataRow("List items\r\n+ First\r\n+ Second", 1, MdType.List, "List items\r\n{0}\r\n")]
        [DataRow("List items\r\n  - First\r\n  - Second", 1, MdType.List, "List items\r\n  {0}  \r\n")]
        [DataRow("Numbered items\r\n  1. First\r\n  2. Second", 1, MdType.OrderedList, "Numbered items\r\n  {0}  \r\n")]
        [DataRow("Numbered items\r\n  1. First\r\n  2. Second\r\n# Header1\r\n", 2, MdType.OrderedList, "Numbered items\r\n  {0}\r\n  {2}\r\n")]
        [DataRow("[ ] Checkbox", 1, MdType.CheckBox, "{0} Checkbox")]
        [DataRow("[x]  Checkbox", 1, MdType.CheckBox, "{0}  Checkbox")]
        [DataRow("[v] Checkbox", 1, MdType.CheckBox, "{0} Checkbox")]
        [DataRow("> BlockQuote", 1, MdType.BlockQuote, "{0}")]
        [DataRow("> BlockQuote\r\nNext line", 1, MdType.BlockQuote, "{0}\r\nNext line")]
        [DataRow(" > BlockQuote\r\nNext line", 1, MdType.BlockQuote, " {0}\r\nNext line")]
        [DataRow("---", 1, MdType.HorizontalRule, "{0}")]
        [DataRow("___\r\nNext line", 1, MdType.HorizontalRule, "{0}\r\nNext line")]
        [DataRow(" ***\r\nNext line", 1, MdType.HorizontalRule, " {0}\r\nNext line")]
        [DataRow("Some text\r\n---\r\nNext line", 1, MdType.HorizontalRule, "Some text\r\n{0}\r\nNext line")]
        [DataRow("```\r\nCode block\r\n```\r\n", 1, MdType.Code, "{0}\r\n")]
        [DataRow("````\r\nCode block\r\n````\r\n", 1, MdType.Code, "{0}\r\n")]
        [DataRow("```C#\r\nCode block\r\n```\r\n", 1, MdType.Code, "{0}\r\n")]
        [DataRow("~~~~\r\nPreformatted \r\ncode\r\n block\r\n~~~~\r\n", 1, MdType.Code, "{0}\r\n")]
        [DataRow("Some text (Nice2experience) with label", 1, MdType.Label, "Some text {0} with label")]
        [DataRow("Some text (Nice2experience)[http://www/nice2experience.com] with label and anchor", 2, MdType.Label, "Some text {0}{1} with label and anchor")]
        [DataRow("Some text [http://www/nice2experience.com] with anchor", 1, MdType.Anchor, "Some text {0} with anchor")]
        [DataRow("Some text _emphasis_ with emphasis", 1, MdType.Emphasis, "Some text {0} with emphasis")]
        [DataRow("Some text *emphasis* with emphasis", 1, MdType.Emphasis, "Some text {0} with emphasis")]
        [DataRow("Some text __emphasis__ with emphasis", 1, MdType.Bold, "Some text {0} with emphasis")]
        [DataRow("Some text __emphasis with nested *emphasis*__ with emphasis", 1, MdType.Bold, "Some text {0} with emphasis")]
        [DataRow("Some text ~~strikethrough~~ with strikethrough", 1, MdType.Strikethrough, "Some text {0} with strikethrough")]
        [DataRow("~~strikethrough~~ with strikethrough", 1, MdType.Strikethrough, "{0} with strikethrough")]
        [DataRow("Some text `Inline code block` with inline code ignores other elements??", 1, MdType.InlineCode, "Some text {0} with inline code ignores other elements??")]
        public void ParseDocument(string value, int expectedElementCount, MdType expectType, string expectedContent)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            Assert.IsNotNull(contentBlocks);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentBlocks, Newtonsoft.Json.Formatting.Indented));

            Assert.AreEqual(expectedElementCount, contentBlocks.SubElements.Count(m => m!=null));
            Assert.AreEqual(expectedContent.Replace(" ", string.Empty), contentBlocks.Content.Replace(" ", string.Empty));
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
        [DataRow("```C#\r\nCode _ignore inner code_ block\r\n```\r\n")]
        [DataRow("~~~~\r\nPreformatted \r\n# Header\r\n code block\r\n~~~~\r\n")]
        public void ParseElementsThatIgnoreInnerMarkdown(string value)
        {
            var md = new MarkDownReader();
            IMarkdownContent contentBlocks = md.Parse(value);
            Assert.IsNotNull(contentBlocks);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentBlocks, Newtonsoft.Json.Formatting.Indented));

            Assert.AreEqual(1, contentBlocks.SubElements.Count);
            Assert.AreEqual(0, contentBlocks.SubElements[0].SubElements.Count);
        }

        [DataTestMethod]
        [DataRow("| Table | Table | Column3 |\r\n| :--- | :---: | ---: \r\n| col1 | col2 |\r\n")]
        [DataRow("| Table | Table | Column3\r\n| --- | :---: | ---: \r\n| col1 | col2 | col3 \r\n")]
        [DataRow("| Table | Table\r\n| --- | :---: | ---: \r\n| col1 | col2 \r\n")]
        public void ParseTables(string value)
        {
            var md = new MarkDownReader();
            IMarkdownContent tableContents = md.Parse(value);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tableContents, Newtonsoft.Json.Formatting.Indented));

            // one table
            Assert.IsNotNull(tableContents);
            Assert.AreEqual(1, tableContents.SubElements.Count);
            Assert.AreEqual("{0}\r\n", tableContents.Content);

            // two rows
            var contentBlocks = tableContents.SubElements[1];
            Assert.IsNotNull(contentBlocks);
            Assert.AreEqual(2, contentBlocks.SubElements.Count);
            Assert.AreEqual("{0}\r\n{1}\r\n{2}\r\n", contentBlocks.Content);

            var firstRow = contentBlocks.SubElements[0];
            Assert.IsNotNull(firstRow);
            Assert.AreEqual(MdType.TableRow, firstRow.Type);

            Assert.IsTrue(firstRow.SubElements.Count>=2);
            Assert.AreEqual(MdType.TableHeader, firstRow.SubElements[0].Type);
            Assert.AreEqual("Table", firstRow.SubElements[0].Content);

            var secondRow = contentBlocks.SubElements[1];
            Assert.IsNotNull(secondRow);
            Assert.AreEqual(MdType.TableRow, secondRow.Type);

            Assert.AreEqual(3, secondRow.SubElements.Count);
            Assert.AreEqual(MdType.TableCell, secondRow.SubElements[0].Type);
            Assert.AreEqual(MdType.TableCellCenter, secondRow.SubElements[1].Type);
            Assert.AreEqual(MdType.TableCellRight, secondRow.SubElements[2].Type);
        }

        [DataTestMethod]
        [DataRow("List items\r\n- First\r\n- Second", new int[] { 0, 0 })]
        [DataRow("List items\r\n  - First\r\n  - Second", new int[] { 2, 2 })]
        [DataRow("List items\r\n- First\r\n  - Second", new int[] { 0, 2 })]
        public void ParseListItemWithDepthCount(string value, int[] expectValue)
        {
            var md = new MarkDownReader();
            IMarkdownContent listContent = md.Parse(value);

            var contentBlocks = listContent.SubElements[0];
            Assert.AreEqual(MdType.List, contentBlocks.Type);

            Assert.AreEqual(MdType.ListItem, contentBlocks.SubElements[0].Type);
            Assert.AreEqual(expectValue[0], contentBlocks.SubElements[0].Depth);
            Assert.AreEqual("First", contentBlocks.SubElements[0].Content.Trim());

            Assert.AreEqual(MdType.ListItem, contentBlocks.SubElements[1].Type);
            Assert.AreEqual(expectValue[1], contentBlocks.SubElements[1].Depth);
            Assert.AreEqual("Second", contentBlocks.SubElements[1].Content.Trim());
        }
    }

   
}
