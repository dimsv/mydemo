using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the path to the Excel file:");
        string filePath = Console.ReadLine();

        if (File.Exists(filePath))
        {
            try
            {
                Dictionary<string, Tuple<string, string>> mapping = ReadExcelFile(filePath);

                Console.WriteLine("Mapping values:");
                foreach (var entry in mapping)
                {
                    Console.WriteLine($"Key: {entry.Key}, Value1: {entry.Value.Item1}, Value2: {entry.Value.Item2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("File not found. Exiting...");
        }
    }

    static Dictionary<string, Tuple<string, string>> ReadExcelFile(string filePath)
    {
        var mapping = new Dictionary<string, Tuple<string, string>>();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first worksheet

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 1; row <= rowCount; row++)
            {
                // Assuming the data is in the first three columns (A, B, C)
                string key = worksheet.Cells[row, 1].Value?.ToString();
                string value1 = worksheet.Cells[row, 2].Value?.ToString();
                string value2 = worksheet.Cells[row, 3].Value?.ToString();

                if (!string.IsNullOrEmpty(key))
                {
                    mapping[key] = new Tuple<string, string>(value1, value2);
                }
            }
        }

        return mapping;
    }
}
