
declare @StartDate as varchar(100)='20220701'
declare @EndDate as varchar(100)='20230630' -- closing
--declare @ReportType as varchar(100)='IS'


declare @NetProfitCOAId as varchar(100)

select @NetProfitCOAId=id from COAs where COAType in ('NetProfit')

create table #TempNetProfit(operationType nvarchar(100),COAType nvarchar(100),TransactionAmount decimal(18,4))

TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate <@StartDate  
group by TransType, CoaId
--select c.COACode,c.COAName,t.TransactionAmount from View_COA_New c left outer join TempNetChangeNew t on c.COAId=t.COAId


insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate >=@StartDate and transactionDate <=@EndDate and isnull(IsYearClosing,0)=0 
group by TransType, CoaId


select  c.COAType,c.Nature,c.COACode,c.COAName
--,case when c.Nature in('Dr') and  opening.TransactionAmount >=0 then  opening.TransactionAmount 
--when c.Nature in('Cr') and opening.TransactionAmount >0 then   opening.TransactionAmount  
--when c.Nature in('Cr') and opening.TransactionAmount <=0 then  -1*opening.TransactionAmount 
--when c.Nature in('Dr') and opening.TransactionAmount <0 then  -1* opening.TransactionAmount 
--else 0 end OpeningAmount
,case when  c.Nature in('Cr') and c.COAType not in('Asset') then -1*opening.TransactionAmount else   opening.TransactionAmount end  OTransactionAmount

--,case when c.Nature in('Dr') and  NetChange.TransactionAmount >=0 then  NetChange.TransactionAmount 
--when c.Nature in('Cr') and NetChange.TransactionAmount >0 then   NetChange.TransactionAmount  
--when c.Nature in('Cr') and NetChange.TransactionAmount <=0 then  -1*NetChange.TransactionAmount 
--when c.Nature in('Dr') and NetChange.TransactionAmount <0 then  -1* NetChange.TransactionAmount 
--else 0 end NetChangeAmount
,case when  c.Nature in('Cr')  and c.COAType not in('Asset') then -1*NetChange.TransactionAmount else  NetChange.TransactionAmount end  NTransactionAmount
 
from View_COA_New c left outer join (
select * from TempNetChangeNew where OperationType in('opening')
) opening on c.COAId=opening.COAId
left outer join (
select * from TempNetChangeNew where OperationType in('NetChange')
) NetChange on c.COAId=NetChange.COAId

where 1=1
--and NetChange.TransactionAmount<>0 and opening.TransactionAmount<>0
--and c.COAType in('Asset','Liability','OwnersEquity')
order by COAType,COACode
 
----TB
select distinct c.COAType,c.Nature,c.COACode,c.COAName ,
sum(case when c.Nature in('Dr') and  t.TransactionAmount >=0 then  t.TransactionAmount 
when c.Nature in('Cr') and  t.TransactionAmount >0 then  t.TransactionAmount  
else 0 end) Dr
,sum(case when c.Nature in('Cr') and  t.TransactionAmount <=0 then  -1*t.TransactionAmount 
when c.Nature in('Dr') and  t.TransactionAmount <0 then  -1* t.TransactionAmount 
else 0 end) Cr
from View_COA_New c left outer join TempNetChangeNew t on c.COAId=t.COAId
where t.TransactionAmount<>0
group by c.COAType,c.Nature,c.COACode,c.COAName




insert into #TempNetProfit(operationType,COAType,TransactionAmount)
select 'NetProfitOpening', c.COAType,sum( transactionAmount) transactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='opening' and c.COAType in ('Revenue','Expense')
group by  c.COAType
insert into #TempNetProfit(operationType,COAType,TransactionAmount)
select 'NetProfitNetchange', c.COAType,sum( transactionAmount) transactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='netchange' and c.COAType in ('Revenue','Expense')
group by  c.COAType

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select 'PF','NetProfitOpening',@NetProfitCOAId,sum(TransactionAmount)TransactionAmount from (
select -1* TransactionAmount TransactionAmount from #TempNetProfit
where COAType in('Revenue') and operationType in ('NetProfitOpening')
union all
select -1*TransactionAmount from #TempNetProfit
where COAType in('Expense')  and operationType in ('NetProfitOpening')
) as a

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select 'PF','NetProfitNetchange',@NetProfitCOAId,sum(TransactionAmount)TransactionAmount from (
select -1* TransactionAmount TransactionAmount from #TempNetProfit
where COAType in('Revenue') and operationType in ('NetProfitNetchange')
union all
select -1*TransactionAmount from #TempNetProfit
where COAType in('Expense')  and operationType in ('NetProfitNetchange')
) as a
/*
select 
case
when c.COAType in('Asset') then '1' 
when c.COAType in('Liability') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('RetainedEarnings') then '4' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('NetProfit') then '6' 
when c.COAType in('NetProfit') then '7' 
else 0 
end as SL 
,c.reportType, case when  c.COAType in('RetainedEarnings') then 'OwnersEquity' else c.COAType end  COAType, c.GroupSL, c.COAGroupName, c.COAId AS COAId, c.TransactionType, c.TransType, c.COASL,c.Nature, c.COACode, c.COAName
,case when c.COAType in('Revenue','Liability','OwnersEquity','RetainedEarnings') then -1*isnull(op.TransactionAmount,0) else isnull(op.TransactionAmount,0) end OpeningAmount 
,case when c.COAType in('Revenue','LiabilityX') then -1*isnull(t.TransactionAmount,0) else isnull(t.TransactionAmount,0) end NetChange 
,case when c.COAType in('Revenue','Liability','OwnersEquity','RetainedEarnings') then -1*(isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0)) else isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) end ClosingAmount 

--,isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) ClosingAmount
,isnull(n.TransactionAmount,0) NetProfitOpening,isnull(nn.TransactionAmount,0) NetProfitNetChange

from View_COA_New c
left outer join (select distinct CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('opening')  
group by CoaId
)Op on c.COAId=op.CoaId
left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetChange')  
group by CoaId
)t on c.COAId=t.CoaId

left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetProfitOpening')  
group by CoaId
)n on c.COAId=n.CoaId

left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetProfitNetchange')  
group by CoaId
)nn on c.COAId=nn.CoaId

where isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0)+isnull(n.TransactionAmount,0)+isnull(nn.TransactionAmount,0)<>0 
and c.ReportType  in('IS')
order by SL,c.GroupSL,c.COASL
 */
--select * from TempNetChangeNew
drop table #TempNetProfit
