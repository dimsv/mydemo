Sub ConsolidateData()
    Dim wsDest As Worksheet
    Dim wsSource As Worksheet
    Dim sourceFile As Variant

    ' Set the destination worksheet
    Set wsDest = ThisWorkbook.Sheets("Sheet1") ' Change "Sheet1" to your destination sheet name

    ' Loop through each source file
    For Each sourceFile In Application.GetOpenFilename(FileFilter:="Excel Files (*.xls; *.xlsx), *.xls; *.xlsx", Title:="Select a File", MultiSelect:=True)
        ' Set the source worksheet
        Set wsSource = Workbooks.Open(sourceFile).Sheets("Sheet1") ' Change "Sheet1" to your source sheet name

        ' Copy data from source to destination
        wsSource.UsedRange.Copy wsDest.Cells(wsDest.Rows.Count, "A").End(xlUp).Offset(1, 0)

        ' Close the source workbook
        Workbooks.Open(sourceFile).Close SaveChanges:=False
    Next sourceFile
End Sub


using System;
using System.IO;
using OfficeOpenXml;

class Program
{
    static void Main()
    {
        // Specify the path to the folder containing your Excel files
        string folderPath = @"path\to\your\excel\files";

        // Specify the path for the new combined Excel file
        string outputPath = @"path\to\your\output\file\combined_data.xlsx";

        // Create a new Excel package for the combined data
        using (var combinedPackage = new ExcelPackage())
        {
            // Create a worksheet in the combined Excel file
            var combinedWorksheet = combinedPackage.Workbook.Worksheets.Add("CombinedSheet");

            // Get a list of all Excel files in the folder
            string[] excelFiles = Directory.GetFiles(folderPath, "*.xlsx");

            foreach (var file in excelFiles)
            {
                using (var package = new ExcelPackage(new FileInfo(file)))
                {
                    // Assuming data is in the first worksheet, adjust as needed
                    var worksheet = package.Workbook.Worksheets[0];

                    // Get the dimension of the data in the worksheet
                    var dimension = worksheet.Dimension;

                    // Copy the data from the source worksheet to the combined worksheet
                    combinedWorksheet.Cells[combinedWorksheet.Dimension.End.Row + 1, 1]
                        .LoadFromCollection(worksheet.Cells[1, 1, dimension.End.Row, dimension.End.Column]
                        .Select(cell => cell.Text), true);
                }
            }

            // Save the combined Excel file
            combinedPackage.SaveAs(new FileInfo(outputPath));
        }

        Console.WriteLine($"Combined data saved to: {outputPath}");
    }
}
