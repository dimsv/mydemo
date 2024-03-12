
SELECT        TOP (200) tId,
 tGuid,
 tStatus,
 tElorusInvoiceId,
 tVivaOrderCode,
 tVivaTransactionId,
 tVivaStatusId,
 tCustomerId,
 tLoginId,
 tProductId,
 tNetAmount,
 tTotalAmount,
 tUsedParses,
 tTotalParses,
 tCreatedDate,
 tCreatedDateTime
FROM            dbo.transactions
WHERE        (tCustomerId = 250)