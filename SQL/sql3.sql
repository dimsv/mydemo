
@functions {

  //apl.aplPrevPipelineStageId 

  public static string eventsCountSQL = @"
SELECT COUNT(distinct eid) 
        FROM events e WITH(NOLOCK)
        LEFT JOIN applications a ON a.aId = e.eApplicationId
        LEFT JOIN candidates cd on cd.cdId = e.eCandidateId
        LEFT JOIN jobs j ON a.aJobId = j.jId
        WHERE (a.aJobId IN (@1) OR '@1' = '0')
        AND cd.cdCustomerId = @0
        AND (e.eCreatedDateTime >= '@2' OR '@2'='')
        AND (e.eCreatedDateTime <= '@3' OR '@3'='')
        --AND (a.aCreatedDateTime >= '@2' OR '@2'='')
        --AND (a.aCreatedDateTime <= '@3' OR '@3'='')
        AND (e.eCreatedDateTime <= '@4' OR '@4'='' )
        AND (e.eCreatedDateTime >= '@5' OR '@5'='' )
        AND eCreatedBy >0 
        --AND eCandidateId='@6'
		  AND eType NOT IN  (1,2, 3, 12)
";

  //public static string stageChangesSQL =
  //  @"SELECT eCandidateId, eCreatedDateTime,eNumericMetadata1
  //      FROM events e WITH(NOLOCK)
  //      LEFT JOIN applications a ON a.aId = e.eApplicationId
  //      LEFT JOIN candidates cd on cd.cdId = e.eCandidateId
  //      LEFT JOIN jobs j ON a.aJobId = j.jId
  //   WHERE (a.aJobId IN (@1) OR '@1' = '0')
  //      AND cd.cdCustomerId = @0
  //      AND (e.eCreatedDateTime >= '@2' OR '@2'='')
  //      AND (e.eCreatedDateTime <= '@3' OR '@3'='')
  //      --AND (a.aCreatedDateTime >= '@2' OR '@2'='')
  //      --AND (a.aCreatedDateTime <= '@3' OR '@3'='')
  //      AND eCreatedBy >=0
  //  AND eType IN  (1, 3, 12)
  //  ORDER BY eCreatedDateTime";


  public static string stageChangesSQL =
  @"SELECT eNumericMetadata1, MIN(e.eCreatedDateTime) as  eCreatedDateTime, count(DISTINCT e.eCandidateId) as candidates
        FROM events e WITH (NOLOCK)
        LEFT JOIN applications a ON a.aId = e.eApplicationId
        LEFT JOIN candidates cd on cd.cdId = e.eCandidateId
        LEFT JOIN jobs j ON a.aJobId = j.jId
        LEFT JOIN pipeline_stages ps ON ps.psId = eNumericMetadata1 AND a.aPipelineStageId = ps.psId
     WHERE (a.aJobId IN (@1) OR '@1' = '0')
        AND cd.cdCustomerId = @0
        AND (e.eCreatedDateTime >= '@2' OR '@2'='')
        AND (e.eCreatedDateTime <= '@3' OR '@3'='')
        AND eCreatedBy >=0
		    AND eType IN  (1, 3, 12) AND eNumericMetadata1=@4
   GROUP BY eNumericMetadata1, ps.psRank
		  ORDER BY ps.psRank";

  //@"SELECT COUNT(DISTINCT (e.eId))
  //    FROM events e WITH(NOLOCK)
  //    LEFT JOIN applications a ON a.aId = e.eApplicationId
  //    LEFT JOIN candidates cd on cd.cdId = e.eCandidateId
  //    LEFT JOIN jobs j ON a.aJobId = j.jId
  //    WHERE (a.aJobId IN (@1) OR '@1' = '0')
  //      AND cd.cdCustomerId = @0
  //      AND (e.eCreatedDateTime >= '@2' OR '@2'='')
  //      AND (e.eCreatedDateTime <= '@3' OR '@3'='')
  //      AND (a.aCreatedDateTime >= '@2' OR '@2'='')
  //      AND (a.aCreatedDateTime <= '@3' OR '@3'='')
  //      --AND (e.eNumericMetadata1 = @4)
  //      AND eCreatedBy >=0
  //    ";

  public static string HiringPaceSQL = $@"

--HIRING PACE WITH JOINS

SELECT aplPrevPipelineStageId                                                         as aValue,
       CASE apl.aplPrevPipelineStageId WHEN 0 THEN 'Start' ELSE psPrev.psName END     as bValue,
       CASE apl.aplPrevPipelineStageId WHEN 0 THEN 0 ELSE psPrev.psRank END           as cValue,
       ROUND(@7(cast(DATEDIFF(minute, aplPrevDateTime,apl.aplDateTime) as BIGINT)) / (1440), 0)       as value,
       1                                                                              as query,
       0  as eventsCount,
       0  as candidatesCount
FROM {SQLReports.applicationPipelinesLogTableId} apl WITH (NOLOCK)
        LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psId
        LEFT JOIN pipeline_stages_sets pss ON ps.psSetId = pss.pssId OR ps.psSetId = 0
        LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psId
        LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.psSetId = pssPrev.pssId OR psPrev.psSetId = 0
        LEFT OUTER JOIN applications a ON a.aId = apl.aplApplicationId
        LEFT OUTER JOIN jobs j ON j.jId = a.aJobId
        LEFT OUTER JOIN departments jd ON jd.jdId = j.jDeptId
        LEFT OUTER JOIN candidates cd ON cd.cdId = a.aCandidateId
        LEFT OUTER JOIN organizations o ON o.oId = j.jOrganizationId
        LEFT OUTER JOIN customers c ON c.cId = jd.jdCustomerId
WHERE jd.jdCustomerId = @0
  AND (a.aJobId IN (@1) OR '@1' = '0')
  AND (jd.jdId IN (@5) OR '@5'='0')
  AND (o.oId IN (@6) OR '@6'='0')
  AND cast(DATEDIFF(minute, a.aCreatedDateTime,apl.aplDateTime) as BIGINT) > 0
  AND (apl.aplDateTime >= '@2' OR '@2'='')
  AND (apl.aplDateTime <= '@3' OR '@3'='')
  AND (apl.aplPrevDateTime >= '@2' OR '@2'='')
  AND (apl.aplPrevDateTime <= '@3' OR '@3'='')
  AND (j.jPipelineStagesSetId = @4 AND (psPrev.psSetId = @4 OR apl.aplPrevPipelineStageId <= 0))
  AND apl.aplPrevPipelineStageId > -1
GROUP BY apl.aplPrevPipelineStageId, psPrev.psName, psPrev.psRank

UNION ALL

SELECT aplPipelineStageId                                                           as aValue,
       CASE aplPipelineStageId WHEN 0 THEN 'Start' ELSE ps.psName END               as bValue,
       ps.psRank                                                                    as cValue,
       ROUND(@7(cast(DATEDIFF(minute,apl.aplDateTime, j.jLastDateTime) as BIGINT)) / (1440), 0)     as value,
       1                                                                            as query,
       0 as eventsCount,
       0 as candidatesCount
FROM {SQLReports.applicationPipelinesLogTableId} apl WITH (NOLOCK)
        LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psId
        LEFT JOIN pipeline_stages_sets pss ON ps.psSetId = pss.pssId OR ps.psSetId = 0
        LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psId
        LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.psSetId = pssPrev.pssId OR psPrev.psSetId = 0
        LEFT OUTER JOIN applications a ON a.aId = apl.aplApplicationId
        LEFT OUTER JOIN jobs j ON j.jId = a.aJobId
        LEFT OUTER JOIN departments jd ON jd.jdId = j.jDeptId
        LEFT OUTER JOIN candidates cd ON cd.cdId = a.aCandidateId
        LEFT OUTER JOIN organizations o ON o.oId = j.jOrganizationId
        LEFT OUTER JOIN customers c ON c.cId = jd.jdCustomerId
WHERE jd.jdCustomerId = @0
  AND (a.aJobId IN (@1) OR '@1' = '0')
  AND (jd.jdId IN (@5) OR '@5'='0')
  AND (o.oId IN (@6) OR '@6'='0')
  AND aplPipelineStageId NOT IN (
    SELECT DISTINCT(apl.aplPrevPipelineStageId)
    FROM {SQLReports.applicationPipelinesLogTableId} apl WITH (NOLOCK)
        LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psId
        LEFT JOIN pipeline_stages_sets pss ON ps.psSetId = pss.pssId OR ps.psSetId = 0
        LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psId
        LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.psSetId = pssPrev.pssId OR psPrev.psSetId = 0
        LEFT OUTER JOIN applications a ON a.aId = apl.aplApplicationId
        LEFT OUTER JOIN jobs j ON j.jId = a.aJobId
        LEFT OUTER JOIN departments jd ON jd.jdId = j.jDeptId
        LEFT OUTER JOIN candidates cd ON cd.cdId = a.aCandidateId
        LEFT OUTER JOIN organizations o ON o.oId = j.jOrganizationId
        LEFT OUTER JOIN customers c ON c.cId = jd.jdCustomerId
    WHERE jd.jdCustomerId = @0
      AND (a.aJobId IN (@1) OR '@1' = '0')
      AND (jd.jdId IN (@5) OR '@5'='0')
      AND (o.oId IN (@6) OR '@6'='0')
      AND (j.jPipelineStagesSetId = @4 AND (psPrev.psSetId = @4 OR aplPrevPipelineStageId <= 0))
)
  AND cast(DATEDIFF(minute, a.aCreatedDateTime,apl.aplDateTime) as BIGINT) > 0
  AND (apl.aplDateTime >= '@2' OR '@2'='')
  AND (apl.aplDateTime <= '@3' OR '@3'='')
  AND (apl.aplPrevDateTime >= '@2' OR '@2'='')
  AND (apl.aplPrevDateTime <= '@3' OR '@3'='')
  AND (j.jPipelineStagesSetId = @4 AND (psPrev.psSetId = @4 OR aplPrevPipelineStageId <= 0))
  AND (j.jLastDateTime >= '@2' OR '@2'='')
  AND (j.jLastDateTime <= '@3' OR '@3'='')
  AND apl.aplPipelineStageId > -1
GROUP BY apl.aplPipelineStageId, ps.psName, ps.psRank

UNION ALL

SELECT aplPipelineStageId                                                                   as aValue,
       CASE aplPipelineStageId WHEN 0 THEN 'Start' ELSE ps.psName END                       as bValue,
       ps.psRank                                                                            as cValue,
       ROUND(@7(cast(DATEDIFF(minute, a.aCreatedDateTime,apl.aplDateTime) as BIGINT)) / (1440), 0)         as value,
       2                                                                                    as query,
       0 as eventsCount,
       0 as candidatesCount
FROM {SQLReports.applicationPipelinesLogTableId} apl WITH (NOLOCK)
        LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psId
        LEFT JOIN pipeline_stages_sets pss ON ps.psSetId = pss.pssId OR ps.psSetId = 0
        LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psId
        LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.psSetId = pssPrev.pssId OR psPrev.psSetId = 0
        LEFT OUTER JOIN applications a ON a.aId = apl.aplApplicationId
        LEFT OUTER JOIN jobs j ON j.jId = a.aJobId
        LEFT OUTER JOIN departments jd ON jd.jdId = j.jDeptId
        LEFT OUTER JOIN candidates cd ON cd.cdId = a.aCandidateId
        LEFT OUTER JOIN organizations o ON o.oId = j.jOrganizationId
        LEFT OUTER JOIN customers c ON c.cId = jd.jdCustomerId
WHERE jd.jdCustomerId = @0
  AND (a.aJobId IN (@1) OR '@1' = '0')
  AND (jd.jdId IN (@5) OR '@5'='0')
  AND (o.oId IN (@6) OR '@6'='0')
  AND DATEDIFF(minute, a.aCreatedDateTime,apl.aplDateTime) > 0
  AND (apl.aplDateTime >= '@2' OR '@2'='')
  AND (apl.aplDateTime <= '@3' OR '@3'='')
  AND (apl.aplPrevDateTime >= '@2' OR '@2'='')
  AND (apl.aplPrevDateTime <= '@3' OR '@3'='')
  AND (j.jPipelineStagesSetId = @4 AND (psPrev.psSetId = @4 OR apl.aplPrevPipelineStageId <= 0))
  AND (COALESCE(ps.psRank, 0) >= COALESCE(psPrev.psRank,0))
  AND apl.aplPipelineStageId > 0
  AND apl.aplPipelineStageId > -1
GROUP BY apl.aplPipelineStageId, ps.psName, ps.psRank

ORDER BY query, cValue, bValue


";


}