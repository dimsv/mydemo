SmartCV - Sample CV Generator
=================

This C# console application generates multiple sample CVs (resumes) using predefined templates and random data for names, companies, and job titles. The generated CVs are saved in a specified output directory.

Prerequisites
-------------

*   [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

Usage
-----

1.  Clone this repository to your local machine.
2.  Open a terminal or command prompt and navigate to the project directory.

bashCopy code

`cd path/to/SampleCVGenerator`

3.  Compile and run the application.

bashCopy code

`dotnet run`

4.  Follow the on-screen instructions to enter the number of CVs you want to generate.

Configuration
-------------

The application uses text files for storing first names, last names, companies, and job titles. Ensure the following files are available in the specified directory:

*   `SampleCVData\FirstNames.txt`

*   `SampleCVData\LastNames.txt`

*   `SampleCVData\Companies.txt`

*   `SampleCVData\JobTitle.txt`

The CV templates are stored in the `SampleCVTemplate` directory with filenames:

*   `Resume Sample.docx`
*   `Resume Sample 2.docx`
*   `Resume Sample 3.docx`

Add more templates as needed, and update the `templatePaths` array in the code accordingly.

Output
------

Generated CVs are saved in the user's Downloads folder under the "SmartCV.SampleCVs.OutputFolder" directory.

Notes
-----

*   Ensure the application has the necessary permissions to create directories and files in the specified output directory.
*   Make sure to customize the template fields in the CV templates with corresponding placeholders like `[FirstName]`, `[LastName]`, `[Email]`, `[Company]`, `[Job]`, etc. The application replaces these placeholders with actual data during the generation process.

License
-------

This project is licensed under the [MIT License](LICENSE).
