        var queryText = string.IsNullOrEmpty(q) ? string.Empty : Server.UrlDecode(Server.UrlEncode(q)).ToString().Trim();
        var matchScore = Request["averageMatchScore"];
        var jobTitles = Request["jobTitles"];
        var internalSourceName = Request["internalSourceName"];
        var externalSource = Request["externalSource"];
        var pipelineStageName = Request["pipelineStageName"];
        var projectName = Request["projectName"];
        var administrativeArea = Request["administrativeArea"];
        var isEmployed = Request["isEmployed"];
        var employers = Request["employers"];
        var employersTxt = Request["employersTxt"];
        var positionTitles = Request["positionTitles"];
        var positionTitlesTxt = Request["positionTitlesTxt"];
        var jobExperienceType = Request["jobExperienceType"];
        var currentManagementLevel = Request["currentManagementLevel"];
        var executiveType = Request["executiveType"];
        var schoolTypes = Request["schoolTypes"];
        var degreeTypes = Request["degreeTypes"];
        var languages = Request["languages"];
        var monthsOfWorkExperience = Request["monthsOfWorkExperience"];
        var monthsOfManagementExperience = Request["monthsOfManagementExperience"];
        var tags = Request["tags"];
        var hasParsingIssues = Request["hasParsingIssues"];
        var answerTags = Request["answerTags"];
        var answersFreeText = Request["answersFreeText"];
        var comments = Request["comments"];

        var skills = Request["skills"];
        var sortOrder = Request["sort"];


        06/12/2023 - Dim Vouts - Αλλο ενα σχολιο
        με αλλασγη γραμμης

        06/12/2023 - Dim Vouts - and this is one more test comment 2
        06/12/2023 - Dim Vouts - this a test comment  1


.Replace("\r\n", "\n")



//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var txtFileName = Server.UrlEncode("applications" + "_" + DateTime.Now.ToShortDateString()) + ".txt";

// File name test txt - vdi test
string filePath = Server.MapPath("~/app_data/temp/" + txtFileName);
using (FileStream streamTest = new FileStream(filePath, FileMode.Create))
{
    // Your code to work with the FileStream goes here
    // For example, you can write data to the stream or perform other operations.
}
