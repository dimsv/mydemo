using System;
using System.IO;

// Assuming 'ws' is an object representing your worksheet
// Assuming 'hit._source.workHistory' is a collection of work history objects

if (hit._source.workHistory != null)
{
    using (StreamWriter writer = new StreamWriter("output.txt"))
    {
        foreach (var position in hit._source.workHistory)
        {
            if (!String.IsNullOrEmpty(position.employer))
            {
                writer.WriteLine($"Employer: {position.employer} ({position.startDate.Split('-')[0]}-{(!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "")})");
            }

            if (!String.IsNullOrEmpty(position.title))
            {
                writer.WriteLine($"Title: {position.title} ({position.startDate.Split('-')[0]}-{(!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "")})");
            }
        }
    }
}


using System;
using System.IO;

// Assuming 'ws' is an object representing your worksheet

// ... (previous code)

if (!String.IsNullOrEmpty(ws.Cell(counter, 31).Value.ToString()) && ws.Cell(counter, 31).Value.ToString().Length >= 2)
{
    string cellValue = ws.Cell(counter, 31).Value.ToString().Substring(0, ws.Cell(counter, 31).Value.ToString().Length - 2);
    using (StreamWriter writer = new StreamWriter("output.txt", true)) // 'true' appends to the file if it exists
    {
        writer.WriteLine($"Cell 31: {cellValue}");
    }
}

if (!String.IsNullOrEmpty(ws.Cell(counter, 32).Value.ToString()) && ws.Cell(counter, 32).Value.ToString().Length >= 2)
{
    string cellValue = ws.Cell(counter, 32).Value.ToString().Substring(0, ws.Cell(counter, 32).Value.ToString().Length - 2);
    using (StreamWriter writer = new StreamWriter("output.txt", true)) // 'true' appends to the file if it exists
    {
        writer.WriteLine($"Cell 32: {cellValue}");
    }
}

// ... (remaining code)







using System;
using System.IO;

// Assuming the values are already available or fetched from somewhere

// ... (previous code)

// Replace the following lines using ws.Cell with your actual data source
string cell31Value = "Replace with your data"; // Replace with your data source
string cell32Value = "Replace with your data"; // Replace with your data source

if (!String.IsNullOrEmpty(cell31Value) && cell31Value.Length >= 2)
{
    using (StreamWriter writer = new StreamWriter("output.txt", true)) // 'true' appends to the file if it exists
    {
        writer.WriteLine($"Cell 31: {cell31Value}");
    }
}

if (!String.IsNullOrEmpty(cell32Value) && cell32Value.Length >= 2)
{
    using (StreamWriter writer = new StreamWriter("output.txt", true)) // 'true' appends to the file if it exists
    {
        writer.WriteLine($"Cell 32: {cell32Value}");
    }
}

// ... (remaining code)




valComments += $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName}" + (item.ratingValue > 0 ? $"- {item.ratingValue.ToString()}/5" : "") + $" - {item.comment}";
var commentLine = $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName}" + (item.ratingValue > 0 ? $"- {item.ratingValue.ToString()}/5" : "") + $" - {item.comment}";
valComments += commentLine.Replace("\r", "").Replace("\n", "");


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

@using System
@using System.IO.Compression

@{
    if (!WebSecurity.IsAuthenticated) { Response.Redirect("~/account/login?ReturnUrl=" + Request.Url.ToString()); }


    var applicationIds = Request.QueryString["ids"];

    if (String.IsNullOrEmpty(applicationIds))
    {
        Response.End();
    }

    foreach (var applicationId in applicationIds.Split(','))
    {
        int i;
        if (!int.TryParse(applicationId, out i))
        {
            Response.End();
        }
    }

    var db = Database.Open("smartcv");
    var currentLogin = db.QuerySingle("SELECT * FROM logins JOIN customers ON (lCustomerId=cId) WHERE lId=@0",
    WebSecurity.CurrentUserId);
    var currentCustomerId = currentLogin.lCustomerId;

    int maxFiles = 50;
    int c = 0;

    var finalApplications = new List<string>();
    foreach (var applicationId in applicationIds.Split(','))
    {
        finalApplications.Add(applicationId);
        c++;
        if (c==maxFiles)
        {
            break;
        }
    }


    Guid g = Guid.NewGuid();

    var archiveFileName = "bulkcv_" + currentCustomerId.ToString() + "_" + g.ToString() + ".zip";


    var blobPath = "";
    var blobExtension = "";
    var blobMimeType = "";

    string format = "";

    var applications = db.Query("SELECT cId, cvFileName, cvMimeType, cvGuid, aLastName, aFirstName, aId FROM applications WITH (NOLOCK)" +
        " JOIN jobs ON (aJobId=jId) JOIN organizations ON (jOrganizationId=oId) JOIN customers ON (oCustomerId=cId) JOIN parsed_cvs pcvs ON (aParsedCvId=cvId) " +
        "WHERE cId=@0 AND aId in (" + String.Join(",",finalApplications.ToArray()) + ")", currentCustomerId);

    using (FileStream zipToOpen = new FileStream(Server.MapPath(@"~/app_data/temp/" + archiveFileName), FileMode.Create))
    {
        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
        {
            foreach (var application in applications)
            {
                if (application.cvMimeType== "application/pdf")
                {
                    format = "original";
                }
                else
                {
                    format = "pdf";
                }

                if (format == "original")
                {
                    blobPath = "cv-bin";
                    blobExtension = Path.GetExtension(application.cvFileName);
                    blobMimeType = application.cvMimeType;
                }
                else
                {
                    blobPath = "cv-pdf";
                    blobExtension = ".pdf";
                    blobMimeType = "application/pdf";
                }

                var cvBinary = Azure.DownloadFile(blobPath, application.cvGuid + blobExtension);

                var fileName = $"resume_{application.aId.ToString()}.pdf";
                if (!string.IsNullOrEmpty(Utils.GreeklishDisplay(application.aLastName) + Utils.GreeklishDisplay(application.aFirstName)))
                {
                    fileName = Utils.GreeklishDisplay(application.aLastName) + "." + Utils.GreeklishDisplay(application.aFirstName) + blobExtension;
                }

                ZipArchiveEntry cvEntry = archive.CreateEntry(fileName); //create a file with this name
                using (BinaryWriter writer = new BinaryWriter(cvEntry.Open()))
                {
                    writer.Write(cvBinary); //write the binary data
                }

            }



        }


    }


    var archiveFile = File.ReadAllBytes(Server.MapPath(@"~/app_data/temp/" + archiveFileName));
    Response.AddHeader("content-disposition", "attachment; filename=" + "resumes.zip");
    Response.ContentType = "application/zip";
    Response.BinaryWrite(archiveFile);
    File.Delete(Server.MapPath(@"~/app_data/temp/" + archiveFileName));

}