using System;
using SautinSoft.Document;

class Program
{
    static void Main()
    {
        // Specify the path to your template file
        string templatePath = "path/to/template.docx";

        // Specify the path where you want to save the generated document
        string outputPath = "path/to/generated_document.docx";

        // Replace placeholders with actual data
        GenerateDocument(templatePath, outputPath, "John Doe", "2023-12-11", "Sample content...");

        Console.WriteLine("Document generated successfully.");
    }

    static void GenerateDocument(string templatePath, string outputPath, string name, string date, string content)
    {
        // Load the template document
        DocumentCore document = DocumentCore.Load(templatePath);

        // Replace placeholders with actual data
        document.ReplaceText("[[Name]]", name);
        document.ReplaceText("[[Date]]", date);
        document.ReplaceText("[[Content]]", content);

        // Save the generated document
        document.Save(outputPath);
    }
}



using System.Linq;
using SautinSoft.Document;
namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ReplaceText();
        }
        /// <summary>
        /// Replace a specific text in an existing DOCX document.
        /// </summary>
        /// </remarks>
        /// Details: https://sautinsoft.com/products/document/help/net/developer-guide/replace-text-in-docx-document-net-csharp-vb.php
        /// </remarks>
        static void ReplaceText()
        {
            string filePath = @"..\..\example.docx";
            string fileResult = @"Result.docx";
            string searchText = "document";
            string ReplaceText = "book";
            DocumentCore dc = DocumentCore.Load(filePath);
            foreach (ContentRange cr in dc.Content.Find(searchText).Reverse())
            {
                // Replace "document" to "book";
                // Mark "book" by yellow.
                cr.Replace(ReplaceText, new CharacterFormat() { BackgroundColor = Color.Yellow });
            }
            dc.Save(fileResult);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fileResult) { UseShellExecute = true });
        }
    }
}