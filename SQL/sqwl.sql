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
    o.oCustomerId, c.oName;ks