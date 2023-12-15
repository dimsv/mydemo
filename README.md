Sample CV Generator - Console Application
Description
Sample CV Generator is a C# console application that automates the generation of multiple CVs (resumes) based on templates and random data. Use this tool to quickly create sample CVs for testing or demonstration purposes.

Table of Contents
Installation
Usage
Configuration
Contributing
License
Installation
Clone the repository to your local machine:

bashCopy code

git clone https://github.com/yourusername/SampleCVGenerator.git cd SampleCVGenerator

Open the project in your preferred C# development environment.

Build the project to ensure all dependencies are resolved.

Usage
Specify the paths to the text files containing first names, last names, companies, job titles, and CV document templates in the Program.cs file.

csharpCopy code

// Specify the paths to the text files and the CV document templates string firstNamesFilePath = @"C:\path\to\FirstNames.txt"; string lastNamesFilePath = @"C:\path\to\LastNames.txt"; string companiesFilePath = @"C:\path\to\Companies.txt"; string JobTitleFilePath = @"C:\path\to\JobTitle.txt"; string[] templatePaths = { @"C:\path\to\Template1.docx", @"C:\path\to\Template2.docx", // Add more templates as needed };

Open a console window and navigate to the directory where the compiled executable is located.

Run the program by executing the following command:

bashCopy code

SampleCVGenerator.exe

The program will prompt you to enter the number of CVs you want to generate.

The application will create CVs using random data and save them to the specified output directory.

Configuration
Adjust the file paths in Program.cs to point to your specific data files and CV templates.
Customize the template documents by replacing or adding fields inside the templates.
Contributing
If you'd like to contribute to this project, please follow these guidelines:

Fork the repository.
Create a new branch for your feature or bug fix.
Make your changes and test thoroughly.
Submit a pull request to the main repository.