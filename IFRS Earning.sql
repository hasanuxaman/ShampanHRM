
declare @StartDate as varchar(100)='20230701'
declare @EndDate as varchar(100)='20240630' -- closing
declare @TransType as varchar(100)='PF' -- closing

declare @NetProfitCOAId as varchar(100)
select @NetProfitCOAId=id from COAs where IsNetProfit=1


TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate <@StartDate and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
group by TransType, CoaId

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate >=@StartDate and transactionDate <=@EndDate
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
group by TransType, CoaId

 
select case
when c.COAType in('Asset') then '1' 
when c.COAType in('Liability') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('RetainedEarnings') then '4' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('NetProfit') then '6' 
when c.COAType in('NetProfit') then '7' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,c.Nature , c.COAType ,c.COACode,c.COAName ,
isnull(case when c.Nature in('Dr') and  t.TransactionAmount >=0 then  t.TransactionAmount 
when c.Nature in('Cr') and  t.TransactionAmount >0 then  t.TransactionAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.TransactionAmount <=0 then  -1*t.TransactionAmount 
when c.Nature in('Dr') and  t.TransactionAmount <0 then  -1* t.TransactionAmount 
else 0 end,0) Cr
,isnull(o.TransactionAmount,0) OpeningAmount
,isnull(n.TransactionAmount,0) NetChange
,isnull(o.TransactionAmount,0)+isnull(n.TransactionAmount,0) ClosingAmount
from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from TempNetChangeNew where operationType='opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
where (t.TransactionAmount<>0 or n.TransactionAmount<>0 or o.TransactionAmount<>0)
 and c.COAType in ('Revenue','Expense') 


select 'NetProfitOpening' NetProfitOpening,  isnull(sum( transactionAmount),0) TransactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='opening' and c.COAType in ('Revenue','Expense')
 
select 'NetProfitNetchange' NetProfitNetchange , isnull(sum( transactionAmount),0) TransactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='netchange' and c.COAType in ('Revenue','Expense')
