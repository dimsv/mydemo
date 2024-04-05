-- Optimized eventsCountSQL query
SELECT COUNT(DISTINCT e.eid) 
FROM events e
LEFT JOIN applications a ON a.aid = e.eApplicationId
LEFT JOIN candidates cd ON cd.cdid = e.eCandidateId
WHERE (a.aJobId = @1 OR @1 = '0')
  AND cd.cdCustomerId = @0
  AND e.eCreatedBy > 0 
  AND e.eType NOT IN (1, 2, 3, 12)
  AND e.eCreatedDateTime >= COALESCE(NULLIF('@2', ''), e.eCreatedDateTime)
  AND e.eCreatedDateTime <= COALESCE(NULLIF('@3', ''), e.eCreatedDateTime)
  AND e.eCreatedDateTime <= COALESCE(NULLIF('@4', ''), e.eCreatedDateTime)
  AND e.eCreatedDateTime >= COALESCE(NULLIF('@5', ''), e.eCreatedDateTime);

-- Optimized stageChangesSQL query
SELECT e.eNumericMetadata1, MIN(e.eCreatedDateTime) as eCreatedDateTime, COUNT(DISTINCT e.eCandidateId) as candidates
FROM events e
LEFT JOIN applications a ON a.aid = e.eApplicationId
LEFT JOIN candidates cd ON cd.cdid = e.eCandidateId
LEFT JOIN jobs j ON a.aid = j.jid
LEFT JOIN pipeline_stages ps ON ps.psid = e.eNumericMetadata1 AND a.aPipelineStageId = ps.psid
WHERE (a.aJobId = @1 OR @1 = '0')
  AND cd.cdCustomerId = @0
  AND e.eCreatedBy > 0
  AND e.eType IN (1, 3, 12)
  AND e.eNumericMetadata1 = @4
  AND e.eCreatedDateTime >= COALESCE(NULLIF('@2', ''), e.eCreatedDateTime)
  AND e.eCreatedDateTime <= COALESCE(NULLIF('@3', ''), e.eCreatedDateTime)
GROUP BY e.eNumericMetadata1, ps.psRank
ORDER BY ps.psRank;

-- Optimized HiringPaceSQL query
SELECT aplPrevPipelineStageId as aValue,
       CASE aplPrevPipelineStageId WHEN 0 THEN 'Start' ELSE psPrev.psName END as bValue,
       CASE aplPrevPipelineStageId WHEN 0 THEN 0 ELSE psPrev.psRank END as cValue,
       ROUND(DATEDIFF(minute, aplPrevDateTime, apl.aplDateTime) / (1440.0), 0) as value,
       1 as query,
       0 as eventsCount,
       0 as candidatesCount
FROM SQLReports.applicationPipelinesLogTableId apl
LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psid
LEFT JOIN pipeline_stages_sets pss ON ps.pssid = pss.pssid OR ps.pssid = 0
LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psid
LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.pssid = pssPrev.pssid OR psPrev.pssid = 0
LEFT JOIN applications a ON a.aid = apl.aplApplicationId
LEFT JOIN jobs j ON j.jid = a.aid
LEFT JOIN departments jd ON jd.jdid = j.jdid
LEFT JOIN candidates cd ON cd.cdid = a.aid
LEFT JOIN organizations o ON o.oid = j.joid
LEFT JOIN customers c ON c.cid = jd.jdcustomerid
WHERE jd.jdcustomerid = @0
  AND (a.ajobid = @1 OR @1 = '0')
  AND (jd.jdid = @5 OR @5 = '0')
  AND (o.oid = @6 OR @6 = '0')
  AND DATEDIFF(minute, a.acreateddatetime, apl.apldatetime) > 0
  AND apl.apldatetime >= COALESCE(NULLIF('@2', ''), apl.apldatetime)
  AND apl.apldatetime <= COALESCE(NULLIF('@3', ''), apl.apldatetime)
  AND apl.aplprevdatetime >= COALESCE(NULLIF('@2', ''), apl.aplprevdatetime)
  AND apl.aplprevdatetime <= COALESCE(NULLIF('@3', ''), apl.aplprevdatetime)
  AND (j.jpipelinestagesetid = @4 AND (psPrev.pssid = @4 OR apl.aplprevpipelinestageid <= 0))
  AND apl.aplprevpipelinestageid > -1
GROUP BY apl.aplprevpipelinestageid, psPrev.psname, psPrev.psrank

UNION ALL

SELECT aplPipelineStageId as aValue,
       CASE aplPipelineStageId WHEN 0 THEN 'Start' ELSE ps.psname END as bValue,
       ps.psrank as cValue,
       ROUND(DATEDIFF(minute, apl.apldatetime, j.jlastdatetime) / (1440.0), 0) as value,
       1 as query,
       0 as eventsCount,
       0 as candidatesCount
FROM SQLReports.applicationPipelinesLogTableId apl
LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psid
LEFT JOIN pipeline_stages_sets pss ON ps.pssid = pss.pssid OR ps.pssid = 0
LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psid
LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.pssid = pssPrev.pssid OR psPrev.pssid = 0
LEFT JOIN applications a ON a.aid = apl.aplApplicationId
LEFT JOIN jobs j ON j.jid = a.aid
LEFT JOIN departments jd ON jd.jdid = j.jdid
LEFT JOIN candidates cd ON cd.cdid = a.aid
LEFT JOIN organizations o ON o.oid = j.joid
LEFT JOIN customers c ON c.cid = jd.jdcustomerid
WHERE jd.jdcustomerid = @0
  AND (a.ajobid = @1 OR @1 = '0')
  AND (jd.jdid = @5 OR @5 = '0')
  AND (o.oid = @6 OR @6 = '0')
  AND aplPipelineStageId NOT IN (
    SELECT DISTINCT apl.aplprevpipelinestageid
    FROM SQLReports.applicationPipelinesLogTableId apl
    LEFT JOIN pipeline_stages ps ON apl.aplPipelineStageId = ps.psid
    LEFT JOIN pipeline_stages_sets pss ON ps.pssid = pss.pssid OR ps.pssid = 0
    LEFT JOIN pipeline_stages psPrev ON apl.aplPrevPipelineStageId = psPrev.psid
    LEFT JOIN pipeline_stages_sets pssPrev ON psPrev.pssid = pssPrev.pssid OR psPrev.pssid = 0
    LEFT JOIN applications a ON a.aid = apl.aplApplicationId
    LEFT JOIN jobs j ON j.jid = a.aid
    LEFT JOIN departments jd ON jd.jdid = j.jdid
    LEFT JOIN candidates cd ON cd.cdid = a.aid
    LEFT JOIN organizations o ON o.oid = j.joid
    LEFT JOIN customers c ON c.cid = jd.jdcustomerid
