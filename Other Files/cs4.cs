@using System.Text;

@using System.IO;
@using RestSharp;
@using ClosedXML.Excel;

@{

    var jobId = Request.QueryString["jobId"].AsInt(0);
    var organizationId = Request.QueryString["orgId"].AsInt(0);
    var customQuery = Request.QueryString["cq"].AsInt(0) == 1;
    dynamic currentCustomerId = null;
    #region security
    if (!WebSecurity.IsAuthenticated) { SmartCV.ReturnUnauthenticated(Response); }
    using (var dbs = Database.Open("smartcv"))
    {
        var job = dbs.QuerySingle(@"SELECT c.cId FROM jobs j WITH (NOLOCK)
JOIN organizations o ON (j.jOrganizationId=o.oId)
JOIN customers c ON (o.oCustomerId=c.cId)
WHERE (j.jId=@0 OR j.jId=0) AND (o.oId=@1 OR @1=0)", jobId, organizationId);

        var org = dbs.QuerySingle(@"SELECT o.oId FROM organizations o WITH (NOLOCK)
JOIN customers c ON (o.oCustomerId=c.cId)
WHERE (o.oId=@0 OR @0=0)", organizationId);

        currentCustomerId = dbs.QueryValue("SELECT lCustomerId FROM logins WITH (NOLOCK) JOIN roles ON (lRoleId=rlId) WHERE lId=@0", WebSecurity.CurrentUserId);
        #region customer cross-check
        if ((jobId > 0 && job == null) || (organizationId > 0 && org == null) || currentCustomerId == null) { SmartCV.ReturnUnauthenticated(Response); }
        #endregion customer cross-check
    }
    #endregion security

    var db = Database.Open("smartcv");
    //var currentCustomerId = db.QueryValue("SELECT lCustomerId FROM logins WHERE lId=@0", WebSecurity.CurrentUserId);
    var userPrefs = SmartCV.GetUserPrefs(WebSecurity.CurrentUserId);
    var uiTerms = Utils.GetTerms(userPrefs.uiLanguage);
    var projects = db.QuerySingle(@"SELECT TOP 1 prj.* FROM projects prj
                      JOIN customers c ON c.cId = prj.prjCustomerId
                      WHERE prjCustomerId=@0 AND prjIsActive=1
                      ORDER BY prjName", currentCustomerId);

    var projectsEnabled = projects != null;

    if (jobId > 0)
    {
        var job = db.QuerySingle("SELECT * FROM jobs JOIN organizations ON (jOrganizationId=oId) WHERE jId=@0", jobId);
        if (job == null)
        {
            Response.End();
        }
        else if (job.oCustomerId != currentCustomerId)
        {
            Response.End();
        }
    }


    var mainQuery = "customerId:" + currentCustomerId.ToString();

    if (jobId > 0)
    {
        mainQuery += " AND jobId:" + jobId.ToString();
    }

    if (organizationId > 0)
    {
        mainQuery += " AND organizationId:" + organizationId.ToString();
    }

    if (customQuery)
    {

        var q = Request["q"];
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


        Dictionary<string, string> sortOptions = new Dictionary<string, string>();
        sortOptions.Add("createdDateTime:desc", uiTerms["lblSortNewest"]);
        sortOptions.Add("matchScore:desc", uiTerms["lblSortMatchScore"]);
        sortOptions.Add("averageRating:desc", uiTerms["lblSortAverageRating"]);
        sortOptions.Add("geoDistance:asc", uiTerms["lblSortGeoDistance"]);
        sortOptions.Add("createdDateTime:asc", uiTerms["lblSortOldest"]);
        sortOptions.Add("monthsOfWorkExperience:desc", uiTerms["lblSortLongestExperience"]);
        sortOptions.Add("monthsOfWorkExperience:asc", uiTerms["lblSortShortestExperience"]);
        sortOptions.Add("maxDegreeLevel:desc", uiTerms["lblSortHighestEducationLevel"]);
        sortOptions.Add("averageMonthsPerEmployer:desc", uiTerms["lblSortLongestPerEmployerExperience"]);
        sortOptions.Add("averageMonthsPerEmployer:asc", uiTerms["lblSortShortestPerEmployerExperience"]);
        sortOptions.Add("monthsOfManagementExperience:desc", uiTerms["lblSortLongestMgmtExperience"]);
        sortOptions.Add("monthsOfManagementExperience:asc", uiTerms["lblSortShortestMgmtExperience"]);
        sortOptions.Add("highestManagementScore:desc", uiTerms["lblSortHighestMgmtLevel"]);
        sortOptions.Add("highestManagementScore:asc", uiTerms["lblSortLowestMgmtLevel"]);
        sortOptions.Add("isEmployed:desc", uiTerms["lblSortIsEmployed"]);
        sortOptions.Add("isEmployed:asc", uiTerms["lblSortIsUnemployed"]);
        sortOptions.Add("numOfLanguages:desc", uiTerms["lblSortNumOfLanguages"]);


        Func<string, string, string> filterMatch = (member, defaultValue) =>
        {
            var value = Utils.ValueOrEmptyString((dynamic)userPrefs, member);
            var result = string.Empty;
            switch (value.ToString().ToLower())
            {
                case "any":
                    result = " OR ";
                    break;
                case "all":
                    result = " AND ";
                    break;
                default:
                    result = defaultValue;
                    break;
            }
            return result;
        };

        IDictionary<string, string> filterTerms = new Dictionary<string, string>();
        filterTerms.Add("[0 TO 0.9]", "lblNoWorkExperience");
        filterTerms.Add("[1 TO 36]", "lblUpTo3Years");
        filterTerms.Add("[36 TO 60]", "lbl3To5Years");
        filterTerms.Add("[60 TO 120]", "lbl5To10Years");
        filterTerms.Add("[120 TO 180]", "lbl10To15Years");
        filterTerms.Add("[180 TO *]", "lblOver15Years");


        //var sortField = sortOrder.Split(':')[0];
        var sortLine = "";



        if (!String.IsNullOrEmpty(matchScore))
        {
            mainQuery += " AND (";
            foreach (var averageMatchScoreKey in matchScore.Split(','))
            {
                mainQuery += @"averageMatchScore:""" + averageMatchScoreKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!string.IsNullOrEmpty(internalSourceName))
        {
            mainQuery += " AND (";
            foreach (var internalSourceNameKey in internalSourceName.Split(','))
            {
                mainQuery += @"internalSourceName:""" + internalSourceNameKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(externalSource))
        {
            mainQuery += " AND (";
            foreach (var externalSourceKey in externalSource.Split(','))
            {
                mainQuery += @"externalSource:""" + externalSourceKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!string.IsNullOrEmpty(pipelineStageName))
        {
            mainQuery += " AND (";
            foreach (var psn in pipelineStageName.Split(','))
            {
                mainQuery += @"pipelineStageName:""" + psn + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        #region projectName
        if (projectsEnabled && !string.IsNullOrEmpty(projectName))
        {
            mainQuery += " AND (";
            foreach (var psn in projectName.Split(','))
            {
                mainQuery += @"projectName:""" + psn + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }
        #endregion projectName

        if (!String.IsNullOrEmpty(administrativeArea))
        {
            mainQuery += " AND (";
            foreach (var administrativeAreaKey in administrativeArea.Split(','))
            {
                mainQuery += @"administrativeArea:""" + administrativeAreaKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(employers))
        {
            mainQuery += " AND (";
            foreach (var employer in employers.Split(','))
            {
                mainQuery += @"employers:""" + employer + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(employersTxt))
        {
            mainQuery += @" AND (employersTxt: """ + employersTxt + @""") ";
        }

        if (!String.IsNullOrEmpty(positionTitles))
        {
            mainQuery += " AND (";
            foreach (var positionTitle in positionTitles.Split(','))
            {
                mainQuery += @"positionTitles:""" + positionTitle + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(positionTitlesTxt))
        {
            mainQuery += @" AND (positionTitlesTxt: """ + positionTitlesTxt.Replace("&", " ") + @""") ";
        }

        if (!String.IsNullOrEmpty(isEmployed))
        {
            mainQuery += " AND (";
            foreach (var isEmployedKey in isEmployed.Split(','))
            {
                mainQuery += @"isEmployed:""" + isEmployedKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!string.IsNullOrEmpty(tags))
        {
            mainQuery += " AND (";
            foreach (var tag in tags.Split(','))
            {
                mainQuery += @"tags:""" + tag + "\"" + filterMatch("filterMatchTags", " AND ");
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(answerTags))
        {
            mainQuery += " AND (";
            foreach (var answerTag in answerTags.Split(','))
            {
                mainQuery += @"answerTags:""" + Utils.EscapeElasticQuery(answerTag) + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(jobExperienceType))
        {
            mainQuery += " AND (";
            foreach (var jobExperienceTypeKey in jobExperienceType.Split(','))
            {
                mainQuery += @"jobExperienceType:""" + (jobExperienceTypeKey.Replace("_comma_", ",")) + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(currentManagementLevel))
        {
            mainQuery += " AND (";
            foreach (var currentManagementLevelKey in currentManagementLevel.Split(','))
            {
                mainQuery += @"currentManagementLevel:""" + currentManagementLevelKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(executiveType))
        {
            mainQuery += " AND (";
            foreach (var executiveTypeKey in executiveType.Split(','))
            {
                mainQuery += @"executiveType:""" + executiveTypeKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(schoolTypes))
        {
            mainQuery += " AND (";
            foreach (var schoolType in schoolTypes.Split(','))
            {
                mainQuery += @"schoolTypes:""" + schoolType + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(degreeTypes))
        {
            mainQuery += " AND (";
            foreach (var degreeType in degreeTypes.Split(','))
            {
                mainQuery += @"degreeTypes:""" + degreeType + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(languages))
        {
            mainQuery += " AND (";
            foreach (var language in languages.Split(','))
            {
                mainQuery += @"languages:""" + language + @"""" + filterMatch("filterMatchLanguages", " AND ");
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(monthsOfWorkExperience))
        {
            mainQuery += " AND (";
            foreach (var monthsOfWorkExperienceKey in monthsOfWorkExperience.Split(','))
            {
                mainQuery += @"monthsOfWorkExperience:" + monthsOfWorkExperienceKey + " OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(monthsOfManagementExperience))
        {
            mainQuery += " AND (";
            foreach (var monthsOfManagementExperienceKey in monthsOfManagementExperience.Split(','))
            {
                mainQuery += @"monthsOfManagementExperience:" + monthsOfManagementExperienceKey + " OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!String.IsNullOrEmpty(skills))
        {
            mainQuery += " AND (";
            foreach (var skill in skills.Split(','))
            {
                mainQuery += "skills:\"" + skill + "\"" + filterMatch("filterMatchSkills", " AND ");//default value is AND
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        #region not in use
        //if (!String.IsNullOrEmpty(queryText))
        //{
        //  mainQuery += " AND " + queryText;
        //}
        @*
            #region querytext
              if (!string.IsNullOrEmpty(queryText))
              {
                  //and --> caps
                  /*AND OR NOT - + ( ) “ */
                  var isSimpleQuery = queryText.IndexOf("~") == -1 && queryText.IndexOf(" AND ") == -1 && queryText.IndexOf(" OR ") == -1 && queryText.IndexOf(" NOT ") == -1 &&
                                      queryText.IndexOf(" and ") == -1 && queryText.IndexOf(" or ") == -1 && queryText.IndexOf(" not ") == -1 &&
                                      queryText.IndexOf("-") == -1 && queryText.IndexOf("+") == -1 && queryText.IndexOf("(") == -1 &&
                                      queryText.IndexOf(")") == -1 && queryText.IndexOf("“") == -1;

                  var searchFields= new string[]{"fullName","fullNameSrch","currentEmployer","currentRole",
                                            "recentRole","employersTxt","positionTitlesTxt","skills" };

                  //fullName 10  fullNameSrch 10 currentEmployer 8 currentRole 8 recentRole 8 employersTxt 5
                  //positionTitlesTxt 5 skills   2  cvText 1

                  //(fullName:(c# AND asp.net)^10 OR fullNameSrch:(c# AND asp.net)^10 OR ..... OR cvText:("C# asp.net"~5)^1)
                  if (isSimpleQuery) //phrase
                  {
                      //var searchString = searchFields.Except(new string[]{"cvText" }).Aggregate((a, b) => $"{a}:({queryText})^10 OR {b}:({queryText})^10");
                      //searchString = $"({searchString} OR cvText:({queryText})^1)";

                      var queryTextAND = "\"" + string.Join("\" AND \"", Utils.EscapeElasticQuery(queryText).Split(' ')) + "\"";

                      //var searchString = string.Join($":({queryTextAND})^10 OR ", searchFields);
                      //searchString += $":({queryTextAND})^2 ";
                      //searchString = $"({searchString} OR cvText:(\"{Utils.EscapeElasticQuery(queryText)}\"~5)^1)";

                      var phraseSlop = userPrefs.filterPhraseSearchSensitivity == null ? 5 : userPrefs.filterPhraseSearchSensitivity;
                      var searchString = $@"( fullName:({queryTextAND})^10 OR fullNameSrch:({queryTextAND})^10 OR currentEmployer:({queryTextAND})^8 OR
                                    currentRole:({queryTextAND})^8 OR recentRole:({queryTextAND})^8 OR employersTxt:({queryTextAND})^5 OR
                                    positionTitlesTxt:({queryTextAND})^5 OR skills:({queryTextAND})^2 OR
                                    cvText:(""{Utils.EscapeElasticQuery(queryText)}""~{phraseSlop})^1 )";

                      mainQuery += $" AND ({searchString})";
                      //Response.Write($"<script>console.clear();console.log(\"{mainQuery}\")</script>");
                      //mainQuery += $" AND (\"{Utils.EscapeElasticQuery(queryText)}\"~5)";//$" AND ({queryText})";
                  }
                  else
                  {

                      //var searchString = string.Join($":({Utils.EscapeElasticQuery(queryText)})^10 OR ", searchFields);
                      //searchString += $":({Utils.EscapeElasticQuery(queryText)})^2 ";
                      //searchString = $"({searchString} OR cvText:({Utils.EscapeElasticQuery(queryText)})^1)".Replace(" and ", "AND").Replace(" or ", "OR");

                      var escapedQuery = Utils.EscapeElasticQuery(queryText).Replace(" and ", "AND").Replace(" or ", "OR");
                      var searchString = $@"( fullName:({escapedQuery})^10 OR fullNameSrch:({escapedQuery})^10 OR currentEmployer:({escapedQuery})^8 OR
                                  currentRole:({escapedQuery})^8 OR recentRole:({escapedQuery})^8 OR employersTxt:({escapedQuery})^5 OR
                                  positionTitlesTxt:({escapedQuery})^5 OR skills:({escapedQuery})^2  OR
                                  cvText:({escapedQuery})^1 )";
                      mainQuery += $" AND ({searchString})";
                      //Response.Write($"<script>console.clear();console.log(\"{mainQuery}\")</script>");
                      //mainQuery += $" AND ({searchString})";//$" AND ({queryText})";
                  }

              }
            #endregion querytext
        *@
        #endregion not in use


        if (!String.IsNullOrEmpty(hasParsingIssues))
        {
            mainQuery += " AND (";
            foreach (var hasParsingIssuesKey in hasParsingIssues.Split(','))
            {
                mainQuery += @"hasParsingIssues:""" + hasParsingIssuesKey + @""" OR ";
            }
            mainQuery = mainQuery.Substring(0, mainQuery.Length - 4) + ")";
        }

        if (!string.IsNullOrEmpty(queryText))
        {
            var escapedQuery = Utils.EscapeElasticQuery(queryText).Replace(" and ", " AND ").Replace(" or ", " OR ");

            mainQuery += $@" AND ({escapedQuery})";//cvText
        }
    }


    var pageSize = 150;
    var offset = 0;


    var fields = @"candidateId,applicationGuid,jobId,jobTitle,jobCode,organizationId,organizationName,departmentId,departmentName,internalSourceId,internalSourceName,
         externalSource,createdDateTime,hasParsingIssues,gender,lastName,firstName,fullName,phone,email,linkedInUrl,averageRating,pipelineStageId,pipelineStageName,
         countryCode,administrativeArea,geoDistance,matchScore,isEmployed,currentEmployer,currentRole,recentRole,employers,workHistory,monthsOfWorkExperience,
         averageMonthsPerEmployer,monthsOfManagementExperience,currentManagementLevel,schoolTypes,educationHistory,maxDegreeType,languages,expiryDate,tags,answerTags,answersFreeText,comments";

    if (projectsEnabled)
    {
        fields += ",projectId,projectName";
    }

    var query = "size=1&track_total_hits=true&q=" + mainQuery;
    var resp = Utils.Elastic("applications/_search", query + "&_source=" + fields, Method.GET, "");

    var json = Json.Decode(resp);
    
    var totalPages = (int)Math.Ceiling((double)json.hits.total.value / pageSize);



    // File name test txt - vdi test
    var txtFileName = Server.UrlEncode("applications" + "_" + DateTime.Now.ToString("dd_MM_yyyy")) + ".txt";

    string filePath = Server.MapPath("~/app_data/temp/" + txtFileName);
    FileStream streamTest = null;

    try
    {
        streamTest = new FileStream(filePath, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(streamTest, Encoding.UTF8))
        {
            //var wb = new XLWorkbook();
            //var ws = wb.Worksheets.Add("Applications");

            //ws.Range("A1:Z1").Style.Font.SetBold();
            //ws.Range("A1:Z1").Style.Alignment.WrapText = true;

            writer.Write("applicationId" + "\t");
            writer.Write("jobId" + "\t");
            writer.Write("jobTitle" + "\t");
            writer.Write("jobCode" + "\t");
            writer.Write("organizationId" + "\t");
            writer.Write("organizationName" + "\t");
            writer.Write("departmentId" + "\t");
            writer.Write("departmentName" + "\t");
            writer.Write("internalSourceName" + "\t");
            writer.Write("externalSource" + "\t");
            writer.Write("createdDateTime" + "\t");
            writer.Write("hasParsingIssues" + "\t");
            writer.Write("salutation" + "\t");
            writer.Write("lastName" + "\t");
            writer.Write("firstName" + "\t");
            writer.Write("phone" + "\t");
            writer.Write("email" + "\t");
            writer.Write("linkedInUrl" + "\t");
            writer.Write("matchScore" + "\t");
            writer.Write("averageRating" + "\t");
            writer.Write("pipelineStageId" + "\t");
            writer.Write("pipelineStageName" + "\t");
            writer.Write("countryCode" + "\t");
            writer.Write("administrativeArea" + "\t");
            writer.Write("distanceInKm" + "\t");
            writer.Write("isEmployed" + "\t");
            writer.Write("currentEmployer" + "\t");
            writer.Write("currentRole" + "\t");
            writer.Write("recentEmployer" + "\t");
            writer.Write("recentRole" + "\t");
            writer.Write("allEmployers" + "\t");
            writer.Write("allRoles" + "\t");
            writer.Write("yearsOfWorkExperience" + "\t");
            writer.Write("averageYearsPerEmployer" + "\t");
            writer.Write("yearsOfManagementExperience" + "\t");
            writer.Write("currentManagementLevel" + "\t");
            writer.Write("maxDegreeType" + "\t");
            writer.Write("schoolNames" + "\t");
            writer.Write("languages" + "\t");
            writer.Write("tags" + "\t");
            writer.Write("applicationLink" + "\t");
            writer.Write("expiryDate" + "\t");
            writer.Write("answers" + "\t");
            writer.Write("answersFreetext" + "\t");
            writer.Write("comment" + "\t");
            if (projectsEnabled)
            {
                writer.Write("projectId" + "\t");
                writer.Write("projectName" + "\t");
            }

            //change line on txt
            writer.WriteLine();

            int counter = 1;

            var scrollid = string.Empty;
            var method = Method.GET;
            //_scroll_id
            var endpoint = "applications/_search";
            var qt = string.Empty;
            var Empty = "";

            for (var page = 1; page <= totalPages; page++)
            {
                offset = (page - 1) * pageSize;

                query = "scroll=1m&size=" + pageSize.ToString() + "&from=" + offset.ToString() + "&q=" + mainQuery;
                if (page > 1)
                {
                    qt = $"scroll=2m&scroll_id={scrollid}";
                    endpoint = "_search/scroll";
                    //qt = query + $"&scrollid={scrollid}&sort=createdDateTime:asc&_source=" + fields;
                    method = Method.POST;
                }
                else
                {
                    qt = query + "&sort=createdDateTime:asc&_source=" + fields;
                }
                resp = Utils.Elastic(endpoint, qt, method, "");
                json = Json.Decode(resp);
                scrollid = json._scroll_id;
                if (json == null || json.hits == null || json.hits.hits == null)
                {

                    //counter++;


                    //ws.Cell(counter, 1).Value = qt;
                    //ws.Cell(counter, 2).Value = resp;
                    continue;
                }
                foreach (var hit in json.hits.hits)
                {
                    if (DateTime.Now < Convert.ToDateTime(hit._source.expiryDate))
                    {
                        counter++;


                        writer.Write(hit._id + "\t");
                        writer.Write(hit._source.jobId + "\t");
                        writer.Write(hit._source.jobTitle + "\t");

                        if (hit._source.jobCode != null)
                        {
                            writer.Write(hit._source.jobCode + "\t");
                        }
                        else
                        {
                            writer.Write(Empty + "\t");

                        }
                        writer.Write(hit._source.organizationId + "\t");
                        writer.Write(hit._source.organizationName + "\t");
                        writer.Write(hit._source.departmentId + "\t");
                        writer.Write(hit._source.departmentName + "\t");
                        writer.Write(hit._source.internalSourceName + "\t");
                        writer.Write(hit._source.externalSource + "\t");
                        writer.Write(hit._source.createdDateTime + "\t");
                        writer.Write(hit._source.hasParsingIssues == 1 ? "yes" : "no" + "\t");

                        if (hit._source.gender == "male")
                        {
                            writer.Write("Mr." + "\t");
                        }
                        else if (hit._source.gender == "female")
                        {
                            writer.Write("Ms." + "\t");
                        }
                        else
                        {
                            writer.Write("" + "\t");
                        }

                        writer.Write(hit._source.lastName + "\t");
                        writer.Write(hit._source.firstName + "\t");
                        writer.Write(hit._source.phone + "\t");
                        if (!hit._source.email.Contains("candidates.smartcv.co"))
                        {
                            writer.Write(hit._source.email + "\t");
                        }
                        writer.Write(hit._source.linkedInUrl + "\t");
                        if (!String.IsNullOrEmpty(hit._source.linkedInUrl))
                        {
                            writer.Write("https://" + hit._source.linkedInUrl + "\t");
                        }
                        writer.Write(hit._source.matchScore + "\t");
                        writer.Write(hit._source.averageRating + "\t");
                        //ws.Cell(counter, 20).Style.NumberFormat.Format = "0.0";
                        writer.Write(hit._source.pipelineStageId +" \t");
                        writer.Write(hit._source.pipelineStageName +" \t");
                        writer.Write(hit._source.countryCode +" \t");
                        writer.Write(hit._source.administrativeArea +" \t");
                        writer.Write(Math.Round((double)hit._source.geoDistance) +" \t");
                        var emp = "";
                        if (hit._source.isEmployed == 1)
                        {
                            emp = "yes";
                        }
                        else
                        {
                            emp = "no";
                        }
                        //writer.Write(hit._source.isEmployed + " \t" == 1 ? "yes" : "no");
                        writer.Write(emp + " \t");

                        writer.Write(hit._source.currentEmployer +" \t");
                        writer.Write(hit._source.currentRole + " \t");

                        if (!String.IsNullOrEmpty(hit._source.recentRole) & hit._source.employers.Length > 0)
                        {
                            writer.Write(hit._source.employers[0] + " \t");
                        }
                        else
                        {
                            writer.Write(Empty + "\t");
                        }

                        writer.Write(hit._source.recentRole + " \t");




                        var positionLine = "";
                        var positionLine2 = "";

                        if (hit._source.workHistory != null)
                        {
                            foreach (var position in hit._source.workHistory)
                            {
                                if (!String.IsNullOrEmpty(position.employer))
                                {
                                    //ws.Cell(counter, 31).Value
                                    positionLine += position.employer + " (" + position.startDate.Split('-')[0] + "-" + (!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "") + "), ";

                                    //ws.Cell(counter, 31).Value += position.employer + " (" + position.startDate.Split('-')[0] + "-" + (!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "") + "), ";
                                }
                                if (!String.IsNullOrEmpty(position.title))
                                {
                                    //ws.Cell(counter, 32).Value
                                    positionLine2 += position.title + " (" + position.startDate.Split('-')[0] + "-" + (!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "") + "), ";

                                    //ws.Cell(counter, 32).Value += position.title + " (" + position.startDate.Split('-')[0] + "-" + (!String.IsNullOrEmpty(position.EndDate) ? position.EndDate.Split('-')[0] : "") + "), ";
                                }
                            }
                        }



                        if (!String.IsNullOrEmpty(positionLine) & positionLine.Length >= 2)
                        {
                            //ws.Cell(counter, 31).Value = ws.Cell(counter, 31).Value.ToString().Substring(0, ws.Cell(counter, 31).Value.ToString().Length - 2);

                            writer.Write(positionLine.Substring(0, positionLine.Length - 2) + "\t");
                        }

                        if (!String.IsNullOrEmpty(positionLine2) & positionLine2.Length >= 2)
                        {
                            //ws.Cell(counter, 32).Value = ws.Cell(counter, 32).Value.ToString().Substring(0, ws.Cell(counter, 32).Value.ToString().Length - 2);

                            writer.Write(positionLine2.Substring(0, positionLine2.Length -2) + "\t");
                        }



                        if (hit._source.monthsOfWorkExperience != null) { writer.Write(Math.Round((double)hit._source.monthsOfWorkExperience / 12) + "\t"); }
                        else
                        {
                            writer.Write("" + "\t");
                        }

                        if (hit._source.averageMonthsPerEmployer != null) { writer.Write(Math.Round((double)hit._source.averageMonthsPerEmployer / 12) + "\t"); } //?vdi
                        else
                        {
                            writer.Write("" + "\t");
                        }

                        writer.Write(Math.Round((double)hit._source.monthsOfManagementExperience / 12) + "\t");

                        writer.Write(hit._source.currentManagementLevel + "\t");
                        writer.Write(hit._source.maxDegreeType + "\t");


                        var valSchoolName = "";
                        if (hit._source.educationHistory != null)
                        {
                            foreach (var item in hit._source.educationHistory)
                            {

                                valSchoolName += item.schoolName;
                                //ws.Cell(counter, 38).Value += item.schoolName;

                                if (!String.IsNullOrEmpty(item.startDate) | !String.IsNullOrEmpty(item.endDate))
                                {
                                    //ws.Cell(counter, 38).Value += " (" + (!String.IsNullOrEmpty(item.startDate) ? item.startDate.Split('-')[0] : " ") + (!String.IsNullOrEmpty(item.startDate) & !String.IsNullOrEmpty(item.endDate) ? "-" : "") + (!String.IsNullOrEmpty(item.startDate) ? item.endDate.Split('-')[0] : " ") + ")";
                                    valSchoolName += " (" + (!String.IsNullOrEmpty(item.startDate) ? item.startDate.Split('-')[0] : " ") + (!String.IsNullOrEmpty(item.startDate) & !String.IsNullOrEmpty(item.endDate) ? "-" : "") + (!String.IsNullOrEmpty(item.startDate) ? item.endDate.Split('-')[0] : " ") + ")";
                                }
                                //ws.Cell(counter, 38).Value += ", ";
                                valSchoolName += ", ";
                            }
                        }

                        //if (!String.IsNullOrEmpty(ws.Cell(counter, 38).Value.ToString()) & ws.Cell(counter, 38).Value.ToString().Length >= 2)
                        //{
                        //    ws.Cell(counter, 38).Value = ws.Cell(counter, 38).Value.ToString().Substring(0, ws.Cell(counter, 38).Value.ToString().Length - 2);
                        //}


                        if (!String.IsNullOrEmpty(valSchoolName) & valSchoolName.Length >= 2)
                        {
                            valSchoolName = valSchoolName.Substring(0, valSchoolName.Length - 2);
                            writer.Write(valSchoolName + "\t");
                        }
                        else
                        {
                            writer.Write("" + "\t");
                        }





                        var valLanguage = "";
                        foreach (var language in hit._source.languages)
                        {
                            //ws.Cell(counter, 39).Value += language + " ";
                            valLanguage += language + " ";
                        }
                        writer.Write(valLanguage + "\t");


                        var valSourceTags = "";
                        if (hit._source.tags != null)
                        {
                            foreach (var tag in hit._source.tags)
                            {
                                //ws.Cell(counter, 40).Value += tag + " ";
                                valSourceTags += tag + " ";
                            }
                            writer.Write(valSourceTags + "\t");

                        }
                        else
                        {
                            writer.Write("" + "\t");

                        }


                        //ws.Cell(counter, 41).Value = "link";
                        writer.Write("https://app.smartcv.co/application?id=" + hit._id + "\t");

                        writer.Write(hit._source.expiryDate + "\t");

                        if (hit._source.answerTags != null)
                        {
                            writer.Write(string.Join(", ", hit._source.answerTags) + "\t");
                            //foreach (var answerTag in hit._source.answerTags)
                            //{
                            //  ws.Cell(counter, 43).Value += answerTag + " ";
                            //}
                        }
                        else
                        {
                            writer.Write("" + "\t");
                        }


                        var valAnswersFreeText = "";
                        if (hit._source.answersFreeText != null)
                        {
                            for (int i = 0; i < hit._source.answersFreeText.Length; i++)
                            {
                                var item = hit._source.answersFreeText[i];
                                if (!string.IsNullOrEmpty(item.answer) && !string.IsNullOrEmpty(item.question))
                                {
                                    //ws.Cell(counter, 44).Value += $"{item.question}: {item.answer}";
                                    valAnswersFreeText += $"{item.question}: {item.answer}";

                                }

                                if (i < hit._source.answersFreeText.Length - 1)
                                {
                                    valAnswersFreeText += "\n";
                                }
                            }
                            writer.Write(valAnswersFreeText + "\t");
                            //foreach (var item in hit._source.answersFreeText)
                            //{
                            //  if (!string.IsNullOrEmpty(item.answer) && !string.IsNullOrEmpty(item.question))
                            //  {
                            //    ws.Cell(counter, 44).Value += $"[{item.question}: {item.answer}]";
                            //  }
                            //  ws.Cell(counter, 44).Value += "\n, ";
                            //}
                        }
                        else
                        {
                            writer.Write("" + "\t");
                        }
                        // CellType’s wordwrap and Multiline property to true
                        //ws.Cell(counter, 44).Style.Alignment.WrapText = true;
                        //ws.Cell(counter, 44).Style.Font.FontSize = 9;


                        var valComments = "";
                        if (hit._source.comments != null)
                        {
                            for (int i = 0; i < hit._source.comments.Length; i++)
                            {
                                var item = hit._source.comments[i];
                                if (item != null)
                                {

                                    if (string.IsNullOrEmpty(item.comment) && item.ratingValue > 0)
                                    {
                                        //ws.Cell(counter, 45).Value += $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName} - {item.ratingValue.ToString()}/5";
                                        valComments += $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName} - {item.ratingValue.ToString()}/5";

                                    }
                                    else
                                    {
                                        //ws.Cell(counter, 45).Value += $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName}" + (item.ratingValue > 0 ? $"- {item.ratingValue.ToString()}/5" : "") + $" - {item.comment}";
                                        valComments += $"{Utils.LocalDateTimePattern(DateTime.MinValue, userPrefs.uiLanguage, userPrefs.timeZoneId, item.commentDateTime)} - {item.loginFullName}" + (item.ratingValue > 0 ? $"- {item.ratingValue.ToString()}/5" : "") + $" - {item.comment}";
                                    }
                                    valComments = valComments.Replace("\r", ", ").Replace("\n", " ");


                                }
                                if (i < hit._source.comments.Length - 1) { valComments += "\n"; }
                            }
                            writer.Write(valComments + "\t");
                        }
                        else
                        {
                            writer.Write("" + "\t");

                        }
                        // CellType’s wordwrap and Multiline property to true
                        ///ws.Cell(counter, 45).Style.Alignment.WrapText = true;
                        //ws.Cell(counter, 45).Style.Font.FontSize = 9;
                        //ws.Cell(counter, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                        if (projectsEnabled)
                        {
                            if (hit._source.projectId != null)
                            {
                                writer.Write(hit._source.projectId + "\t");
                            }
                            else
                            {
                                writer.Write(Empty + "\t");

                            }

                            if (hit._source.projectName != null)
                            {
                                writer.Write(hit._source.projectName + "\t");
                            }
                            else
                            {
                                writer.Write(Empty + "\t");
                            }
                        }

                    }
                    writer.WriteLine();
                    //change line?
                }
                //change line?

            }


            //change line on txt
            writer.WriteLine();

            //ws.Columns("A", "AP").AdjustToContents();


            //ws.Column("A").Width = 10;
            //ws.Column("E").Width = 12;
            //ws.Column("G").Width = 12;
            //ws.Column("L").Width = 7;
            //ws.Column("M").Width = 6;
            //ws.Column("W").Width = 5;
            //ws.Column("Y").Width = 6;
            //ws.Column("AG").Width = 9;
            //ws.Column("AH").Width = 9;
            //ws.Column("AI").Width = 9;

            //ws.Column("AE").Width = 50;
            //ws.Column("AF").Width = 50;
            //ws.Column("AL").Width = 50;
            //ws.Column("AR").Width = 120;
            //ws.Column("AS").Width = 120;
            ////ws.Column("AR").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Row(1).Style.Font.Bold = true;
            //ws.Row(1).Style.Font.FontSize = 10;
            //ws.Row(1).Style.Alignment.WrapText = true;

            //ws.SheetView.Freeze(1, 0);

            //ws.Column("S").AdjustToContents();
            //ws.Column("AR").AdjustToContents();
            //ws.Column("AS").AdjustToContents();

            //string myName = Server.UrlEncode("applications" + "_" + DateTime.Now.ToShortDateString());
            //MemoryStream stream = GetStream(wb);

            //////////////////////////////////////
            string dynamicContent = writer.ToString();
            Response.Write("<p>" + dynamicContent + "</p>");
            //////////////////////////////////////


            writer.Close();
            streamTest.Close();

            //Response.Clear();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=" + myName + ".xlsx");
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.BinaryWrite(stream.ToArray());
            //Response.End();



        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception Message: {ex.Message}");
        throw;
    }

}

@functions {
    public MemoryStream GetStream(XLWorkbook excelWorkbook)
    {
        MemoryStream fs = new MemoryStream();
        excelWorkbook.SaveAs(fs);
        fs.Position = 0;
        return fs;
    }
}


