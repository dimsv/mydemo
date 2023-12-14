using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        // Example file path
        string filePath = "example.txt";

        // Write to a file using FileMode.Create
        WriteToFile(filePath, "Hello, FileStream! This file will be created.", FileMode.Create);

        // Read from a file using FileMode.Open
        ReadFromFile(filePath, FileMode.Open);

        // Append to a file using FileMode.Append
        WriteToFile(filePath, "\nThis text will be appended.", FileMode.Append);

        // Read from the file again after appending
        ReadFromFile(filePath, FileMode.Open);

        // Truncate the file using FileMode.Truncate
        WriteToFile(filePath, "\nThis will overwrite the content.", FileMode.Truncate);

        // Read from the file again after truncating
        ReadFromFile(filePath, FileMode.Open);
    }

    static void WriteToFile(string filePath, string content, FileMode fileMode)
    {
        // Open or create the file with the specified FileMode
        using (FileStream fileStream = new FileStream(filePath, fileMode, FileAccess.Write))
        {
            // Convert the content to bytes
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);

            // Write the content to the file
            fileStream.Write(contentBytes, 0, contentBytes.Length);

            // Flush the stream to ensure that the data is written to the file
            fileStream.Flush();
        }

        Console.WriteLine($"File written using FileMode.{fileMode}");
    }

    static void ReadFromFile(string filePath, FileMode fileMode)
    {
        // Open the file with the specified FileMode
        using (FileStream fileStream = new FileStream(filePath, fileMode, FileAccess.Read))
        {
            // Read the content from the file
            byte[] buffer = new byte[1024];
            int bytesRead = fileStream.Read(buffer, 0, buffer.Length);

            // Convert the bytes to a string and display the content
            string content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"File content using FileMode.{fileMode}:\n{content}");
        }

        Console.WriteLine();
    }
}
