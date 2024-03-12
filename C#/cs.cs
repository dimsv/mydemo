SELECT SUM(tTotalParses) as totalParses,
SUM(tUsedParses) AS usedParses 

FROM transactions WITH (NOLOCK) 

JOIN products ON (tProductId=pId) 

WHERE tCustomerId=@0 AND tStatus=3 AND pType=1", currentCustomerId);
