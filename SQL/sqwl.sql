SELECT TOP (1000) [oId]
      ,[oGuid]
      ,[oCustomerId]
      ,[oName]
      ,[oVirtualDir]
      ,[oPreferences]
      ,[oCreatedDateTime]
  FROM [dbo].[organizations]


SELECT 
    oCustomerId,
    COUNT(oId) AS total_organizations,
    MIN(oCreatedDateTime) AS first_created,
    MAX(oCreatedDateTime) AS last_created
FROM 
    [dbo].[organizations]
GROUP BY 
    oCustomerId;


SELECT 
    oCustomerId,
	oName,
    COUNT(oId) AS total_organizations,
    MIN(oCreatedDateTime) AS first_created,
    MAX(oCreatedDateTime) AS last_created,
    STRING_AGG(oName, ', ') AS organization_names,
    STRING_AGG(CONVERT(VARCHAR(36), oGuid), ', ') AS organization_guids
FROM 
    [dbo].[organizations]
GROUP BY 
    oCustomerId;












WITH CustomerNames AS (
    SELECT
        oCustomerId,
        oName,
        ROW_NUMBER() OVER (PARTITION BY oCustomerId ORDER BY oCreatedDateTime) AS rn
    FROM
        [dbo].[organizations]
)
SELECT 
    o.oCustomerId,
    c.oName AS customer_name,
    COUNT(o.oId) AS total_organizations,
    STRING_AGG(o.oName, ', ') AS organization_names,
    STRING_AGG(CONVERT(VARCHAR(36), o.oGuid), ', ') AS organization_guids
FROM 
    [dbo].[organizations] o
JOIN
    CustomerNames c ON o.oCustomerId = c.oCustomerId AND c.rn = 1
GROUP BY 
    o.oCustomerId, c.oName;






WITH CustomerNames AS (
    SELECT
        oCustomerId,
        oName,
        ROW_NUMBER() OVER (PARTITION BY oCustomerId ORDER BY oCreatedDateTime) AS rn
    FROM
        [dbo].[organizations]
),
CustomerTotals AS (
    SELECT 
        oCustomerId,
        SUM(tTotalParses) AS total_parses
    FROM
        [dbo].[transactions]
    GROUP BY
        oCustomerId
)
SELECT 
    o.oCustomerId,
    c.oName AS customer_name,
    COUNT(o.oId) AS total_organizations,
    STRING_AGG(o.oName, ', ') AS organization_names,
    STRING_AGG(CONVERT(VARCHAR(36), o.oGuid), ', ') AS organization_guids
FROM 
    [dbo].[organizations] o
JOIN
    CustomerNames c ON o.oCustomerId = c.oCustomerId AND c.rn = 1
JOIN
    CustomerTotals t ON o.oCustomerId = t.oCustomerId
WHERE
    t.total_parses > 76
GROUP BY 
    o.oCustomerId, c.oName;







SELECT 
    t.tCustomerId,
    SUM(t.tTotalParses) AS total_parses,
    c.cMainOrgName
FROM
    [dbo].[transactions] t
JOIN
    [dbo].[customers] c
ON
    t.tCustomerId = c.cId
WHERE 
    t.tTotalParses > 66
GROUP BY
    t.tCustomerId,
    c.cMainOrgName







WITH CustomerNames AS (
    SELECT
        oCustomerId,
        oName,
        ROW_NUMBER() OVER (PARTITION BY oCustomerId ORDER BY oCreatedDateTime) AS rn
    FROM
        [dbo].[organizations]
),
CustomerTotals AS (
    SELECT 
        tCustomerId,
        SUM(tTotalParses) AS total_parses
    FROM
        [dbo].[transactions]
    GROUP BY
        tCustomerId
)
SELECT 
	o.oCustomerId,
    c.oName AS customer_name,
    COUNT(o.oId) AS total_organizations,
    STRING_AGG(o.oName, ', ') AS organization_names,
    STRING_AGG(CONVERT(VARCHAR(36), o.oGuid), ', ') AS organization_guids
FROM 
    [dbo].[organizations] o
JOIN
    CustomerNames c ON o.oCustomerId = c.oCustomerId AND c.rn = 1
JOIN
    CustomerTotals t ON o.oCustomerId = t.tCustomerId
WHERE
    t.total_parses > 66
GROUP BY 
    o.oCustomerId, c.oName;






SELECT *
  FROM [dbo].[customers]
  where cMainOrgName = 'Pax Hospitality'


SELECT*
  FROM [dbo].[transactions]
where [tCustomerId] = 	223
--or [tCustomerId] = 397


	--SELECT SUM(tTotalParses) as totalParses, SUM(tUsedParses) AS usedParses 
	--FROM transactions WITH (NOLOCK) 
	--JOIN products ON (tProductId=pId)
	--WHERE tCustomerId= 294
	--AND tStatus=3 AND pType=1

SELECT 
    totalParses,
    usedParses,
    totalParses - usedParses AS remainingCount
FROM (
    SELECT 
        SUM(tTotalParses) as totalParses, 
        SUM(tUsedParses) AS usedParses 
    FROM 
        transactions WITH (NOLOCK) 
        JOIN products ON (tProductId = pId)
    WHERE 
        tCustomerId = 223
        AND tStatus = 3 
        AND pType = 1
) AS sums;












WITH ParseSummary AS (
    SELECT 
        psTransactionId,
        COUNT(*) AS RecordCount,
        MAX(psId) AS MaxPsId,
        MIN(psCreatedDateTime) AS FirstCreatedDateTime,
        MAX(psCreatedDateTime) AS LastCreatedDateTime
    FROM 
        dbo.parses
    GROUP BY 
        psTransactionId
)
SELECT 
    ps.psTransactionId,
    ps.RecordCount,
    ps.MaxPsId,
    ps.FirstCreatedDateTime,
    ps.LastCreatedDateTime,
    t.tCustomerId,
    c.cMainOrgName
FROM 
    ParseSummary ps
JOIN 
    dbo.transactions t ON ps.psTransactionId = t.tId
JOIN 
    dbo.customers c ON t.tCustomerId = c.cId
ORDER BY 
    ps.LastCreatedDateTime DESC;










WITH ParseSummary AS (
    SELECT 
        t.tCustomerId,
        SUM(t.tTotalParses) AS total_parses,
        c.cMainOrgName
    FROM 
        dbo.transactions t
    JOIN 
        dbo.customers c ON t.tCustomerId = c.cId
    WHERE 
        t.tTotalParses > 66
    GROUP BY 
        t.tCustomerId,
        c.cMainOrgName
)
SELECT 
    ps.tCustomerId,
    ps.total_parses,
    ps.cMainOrgName
FROM 
    ParseSummary ps
ORDER BY 
    ps.total_parses DESC;














    
WITH CustomerNames AS (
    SELECT
        oCustomerId,
        oName,
        ROW_NUMBER() OVER (PARTITION BY oCustomerId ORDER BY oCreatedDateTime) AS rn
    FROM
        [dbo].[organizations]
)
SELECT 
    o.oCustomerId,
    c.oName AS customer_name,
    COUNT(o.oId) AS total_organizations,
    STRING_AGG(o.oName, ', ') AS organization_names,
    STRING_AGG(CONVERT(VARCHAR(36), o.oGuid), ', ') AS organization_guids
FROM 
    [dbo].[organizations] o
JOIN
    CustomerNames c ON o.oCustomerId = c.oCustomerId AND c.rn = 1
GROUP BY 
    o.oCustomerId, c.oName;



	--SELECT SUM(tTotalParses) as totalParses, SUM(tUsedParses) AS usedParses 
	--FROM transactions WITH (NOLOCK) 
	--JOIN products ON (tProductId=pId)
	--WHERE tCustomerId= 219 AND tStatus=3 AND pType=1