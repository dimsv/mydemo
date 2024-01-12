static void ReplaceFields(DocumentCore document, string firstName, string lastName, string email, string company, string job, DateTime startDate, DateTime endDate)
{
    ReplaceField(document, "[FirstName]", firstName);
    ReplaceField(document, "[LastName]", lastName);
    ReplaceField(document, "[Email]", email);
    ReplaceField(document, "[Company]", company);
    ReplaceField(document, "[Job]", job);
    ReplaceField(document, "[StartDate]", startDate.ToString("MMMM yyyy")); // Format the start date as needed
    ReplaceField(document, "[EndDate]", endDate.ToString("MMMM yyyy"));     // Format the end date as needed
    // Add more fields as necessary
}




static List<(DateTime StartDate, DateTime EndDate)> GenerateRandomDates(int count)
{
    Random random = new Random();
    List<(DateTime StartDate, DateTime EndDate)> dateRanges = new List<(DateTime StartDate, DateTime EndDate)>();

    for (int i = 0; i < count; i++)
    {
        // Generate a random start date within the last 10 years (adjust as needed)
        DateTime startDate = DateTime.Now.AddYears(-random.Next(1, 11));

        // Generate a random end date after the start date (within the last 5 years, adjust as needed)
        DateTime endDate = startDate.AddYears(random.Next(1, 6));

        dateRanges.Add((startDate, endDate));
    }

    return dateRanges;
}




// Generate start and end dates for work experiences
List<(DateTime StartDate, DateTime EndDate)> dateRanges = GenerateRandomDates(enteredNumber);

// Sort date ranges based on start date in ascending order
dateRanges.Sort((a, b) => a.StartDate.CompareTo(b.StartDate));

// Loop through and generate documents
for (int i = 0; i < enteredNumber; i++)
{
    // ... existing code ...

    // Replace fields in the document with the current values and date range
    ReplaceFields(document, firstName, lastName, email, company, job, dateRanges[i].StartDate, dateRanges[i].EndDate);

    // ... existing code ...
}
















using System;
using System.Collections.Generic;
using System.IO;
using SautinSoft.Document;

namespace SampleCVGenerator
{
    internal class Program
    {
        static void Main()
        {
            // ... (existing code)

            // Generate start and end dates for work experiences outside the loop
            List<(DateTime StartDate, DateTime EndDate)> dateRanges = GenerateRandomDates(enteredNumber);

            // Sort date ranges based on start date in ascending order
            dateRanges.Sort((a, b) => a.StartDate.CompareTo(b.StartDate));

            // Loop through and generate documents
            for (int i = 0; i < enteredNumber; i++)
            {
                // ... existing code ...

                // Get the date range for the current iteration
                DateTime startDate = dateRanges[i].StartDate;
                DateTime endDate = dateRanges[i].EndDate;

                // Replace fields in the document with the current values and date range
                ReplaceFields(document, firstName, lastName, email, company, job, startDate, endDate);

                // Save the modified document with a unique name
                string outputFileName = Path.Combine(outputDirectory, $"Demo_CV_{i + 1} - {firstName}_{lastName}.docx"); // Fixed index and file name
                document.Save(outputFileName);

                Console.WriteLine($"Document generated: {outputFileName}");
            }

            Console.WriteLine("Documents successfully generated.");
            Console.ReadLine();
        }

        // ... (existing code)
    }
}




using System;
using System.Collections.Generic;
using System.IO;
using SautinSoft.Document;

namespace SampleCVGenerator
{
    internal class Program
    {
        static void Main()
        {
            // ... (existing code)

            // Generate start and end dates for work experiences outside the loop
            List<(DateTime StartDate, DateTime EndDate)> dateRanges = GenerateRandomDates(enteredNumber);

            // Sort date ranges based on start date in ascending order
            dateRanges.Sort((a, b) => a.StartDate.CompareTo(b.StartDate));

            // Loop through and generate documents
            for (int i = 1; i <= enteredNumber; i++)
            {
                // ... existing code ...

                // Get the date range for the current iteration
                DateTime startDate = dateRanges[i - 1].StartDate;
                DateTime endDate = dateRanges[i - 1].EndDate;

                // Replace fields in the document with the current values and date range
                ReplaceFields(document, firstName, lastName, email, company, job, startDate, endDate);

                // Save the modified document with a unique name
                string outputFileName = Path.Combine(outputDirectory, $"Demo_CV_{i} - {firstName}_{lastName}.docx");
                document.Save(outputFileName);

                Console.WriteLine($"Document generated: {outputFileName}");
            }

            Console.WriteLine("Documents successfully generated.");
            Console.ReadLine();
        }

        // ... (existing code)
    }
}




static void ReplaceFields(DocumentCore document, string firstName, string lastName, string email, string company, string job, DateTime startDate, DateTime endDate)
{
    ReplaceField(document, "[FirstName]", firstName);
    ReplaceField(document, "[LastName]", lastName);
    ReplaceField(document, "[Email]", email);

    // Replace [Company], [Job], [StartDate], and [EndDate] with distinct values
    ReplaceField(document, "[Company]", GetRandomValue(ReadNames(companiesFilePath)));
    ReplaceField(document, "[Job]", GetRandomValue(ReadNames(JobTitleFilePath)));
    ReplaceField(document, "[StartDate]", startDate.ToString("MMMM yyyy"));
    ReplaceField(document, "[EndDate]", endDate.ToString("MMMM yyyy"));
    // Add more fields as necessary
}

// Helper method to get a random value from a list
static string GetRandomValue(List<string> values)
{
    Random random = new Random();
    return values[random.Next(values.Count)];
}


static void ReplaceFields(DocumentCore document, string firstName, string lastName, string email, string company, string job, DateTime startDate, DateTime endDate)
{
    ReplaceField(document, "[FirstName]", firstName);
    ReplaceField(document, "[LastName]", lastName);
    ReplaceField(document, "[Email]", email);
    ReplaceField(document, "[Company]", GetRandomValue(ReadNames(companiesFilePath)));
    ReplaceField(document, "[Job]", GetRandomValue(ReadNames(JobTitleFilePath)));
    ReplaceField(document, "[StartDate]", startDate.ToString("MMMM yyyy"));
    ReplaceField(document, "[EndDate]", endDate.ToString("MMMM yyyy"));
    // Add more fields as necessary
}



static void ReplaceFields(DocumentCore document, string firstName, string lastName, string email, string company, string job, DateTime startDate, DateTime endDate)
{
    foreach (ContentRange field in document.Content.Find("[FirstName]"))
    {
        field.Replace(firstName);
    }

    foreach (ContentRange field in document.Content.Find("[LastName]"))
    {
        field.Replace(lastName);
    }

    foreach (ContentRange field in document.Content.Find("[Email]"))
    {
        field.Replace(email);
    }

    ReplaceField(document, "[Company]", company);
    ReplaceField(document, "[Job]", job);
    ReplaceField(document, "[StartDate]", startDate.ToString("MMMM yyyy"));
    ReplaceField(document, "[EndDate]", endDate.ToString("MMMM yyyy"));
    // Add more fields as necessary
}



static void ReplaceFields(DocumentCore document, string firstName, string lastName, string email, string company, string job, DateTime startDate, DateTime endDate)
{
    ReplaceField(document, "[FirstName]", firstName);
    ReplaceField(document, "[LastName]", lastName);
    ReplaceField(document, "[Email]", email);

    // Replace [Company], [Job], [StartDate], and [EndDate] with distinct values
    ReplaceFields(document, "[Company]", company);
    ReplaceFields(document, "[Job]", job);
    ReplaceFields(document, "[StartDate]", startDate.ToString("MMMM yyyy")); // Format the start date as needed
    ReplaceFields(document, "[EndDate]", endDate.ToString("MMMM yyyy"));     // Format the end date as needed

    // Add more fields as necessary
}

static void ReplaceFields(DocumentCore document, string fieldName, string replacement)
{
    foreach (ContentRange field in document.Content.Find(fieldName))
    {
        // Replace each occurrence of the field with a distinct value
        field.Replace(replacement);
    }
}












// ... (your existing code)

for (int i = 1; i <= enteredNumber; i++)
{
    // ... (your existing code)

    // Load the selected document template
    DocumentCore templateDocument = DocumentCore.Load(templatePath);

    // Count occurrences of [Company] in the template
    int companyPlaceholderCount = CountPlaceholders(templateDocument, "[Company]");

    Console.WriteLine($"Number of [Company] placeholders in the template: {companyPlaceholderCount}");

    // Clone the template for each document
    DocumentCore document = templateDocument.Clone(true);

    // ... (your existing code)
}

// ... (your existing code)

static int CountPlaceholders(DocumentCore document, string placeholderName)
{
    // Find all occurrences of the placeholder in the document
    ContentRangeCollection occurrences = document.Content.Find(placeholderName);

    // Return the count of occurrences
    return occurrences.Count;
}












using System;
using System.Collections.Generic;
using System.IO;
using SautinSoft.Document;

namespace SampleCVGenerator
{
    internal class Program
    {
        public static void Main()
        {
            // ... (your existing code)

            for (int i = 1; i <= enteredNumber; i++)
            {
                // ... (your existing code)

                // Load the selected document template
                DocumentCore templateDocument = DocumentCore.Load(templatePath);

                // Count occurrences of [Company] in the template
                int companyPlaceholderCount = CountPlaceholdersInDocument(templateDocument, "[Company]");

                Console.WriteLine($"Number of [Company] placeholders in the template: {companyPlaceholderCount}");

                // Clone the template for each document
                DocumentCore document = templateDocument.Clone(true);

                // ... (your existing code)
            }

            // ... (your existing code)
        }

        static int CountPlaceholdersInDocument(DocumentCore document, string placeholderName)
        {
            int count = 0;

            foreach (ContentRange contentRange in document.Content.Find(placeholderName))
            {
                // Increment the count when a placeholder is found
                count++;
            }

            return count;
        }

        // ... (your existing code)
    }
}

























using System;
using System.Collections.Generic;
using System.IO;
using SautinSoft.Document;

namespace SampleCVGenerator
{
    internal class Program
    {
        private static Dictionary<string, List<string>> placeholderValueMapping;

        public static void Main()
        {
            // Initialize the mapping for each placeholder and its corresponding text file
            InitializePlaceholderMapping();

            // ... (your existing code)

            for (int i = 1; i <= enteredNumber; i++)
            {
                // ... (your existing code)

                // Load the selected document template
                DocumentCore templateDocument = DocumentCore.Load(templatePath);

                // Map placeholders with distinct values
                MapPlaceholders(document, placeholderValueMapping);

                // ... (your existing code)
            }

            // ... (your existing code)
        }

        static void InitializePlaceholderMapping()
        {
            // Initialize the mapping for each placeholder and its corresponding text file
            placeholderValueMapping = new Dictionary<string, List<string>>
            {
                { "[Company]", ReadNamesFromFile("SampleCVData\\Companies.txt") },
                { "[Job]", ReadNamesFromFile("SampleCVData\\JobTitle.txt") },
                // Add more placeholders as needed
            };
        }

        static List<string> ReadNamesFromFile(string filePath)
        {
            List<string> names = new List<string>();

            try
            {
                // Read lines from the text file
                string[] lines = File.ReadAllLines(filePath);

                // Add each line (name) to the list
                names.AddRange(lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading names from {filePath}: {ex.Message}");
            }

            return names;
        }

        static void MapPlaceholders(DocumentCore document, Dictionary<string, List<string>> placeholderMapping)
        {
            foreach (var kvp in placeholderMapping)
            {
                string placeholder = kvp.Key;
                List<string> values = kvp.Value;

                // Get the next value from the list
                string replacement = GetNextValueFromList(values);

                // Map the placeholder to the value in the document
                MapPlaceholder(document, placeholder, replacement);
            }
        }

        static void MapPlaceholder(DocumentCore document, string placeholder, string replacement)
        {
            foreach (ContentRange contentRange in document.Content.Find(placeholder))
            {
                // Replace each occurrence of the placeholder with the mapped value
                contentRange.Replace(replacement);
            }
        }

        static string GetNextValueFromList(List<string> values)
        {
            // Implement logic to get the next value from the list
            // For simplicity, a basic example is shown here (you might want to customize this based on your requirements)
            string nextValue = values[0];
            values.RemoveAt(0); // Remove the used value to ensure uniqueness
            return nextValue;
        }

        // ... (your existing code)
    }
}


