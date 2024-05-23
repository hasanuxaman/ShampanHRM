
declare @MonthTo as varchar(100)='1159'
declare @TransType as varchar(100)='PF' -- closing


declare @FirstStart as varchar(100)
declare @FirstEnd as varchar(10)
declare @LastStart as varchar(100)
declare @LastEnd as varchar(100)

declare @FirstYear as varchar(100)
declare @LastYear as varchar(100)
declare @FirstRetainedEarning as decimal(18,4)
declare @LastRetainedEarning as decimal(18,4)
declare @FirstNetProfit  as decimal(18,4)
declare @LastNetProfit as decimal(18,4)
declare @NetProfitCOAId as varchar(100)
declare @RetainedEarningCOAId as varchar(100)


select @LastEnd=PeriodEnd,@LastYear=[Year] from EGCB_HRM.dbo.FiscalYearDetail where id=@MonthTo

select @FirstStart=YearStart,@FirstYear=[Year]   from EGCB_HRM.dbo.FiscalYear where [Year]=cast( @LastYear as int)-1
SELECT  @FirstEnd=CONVERT(VARCHAR(8),DATEADD(MONTH, -12, CONVERT(DATE, @LastEnd, 112)), 112)  
select @LastStart=YearStart  from EGCB_HRM.dbo.FiscalYear where [Year]= @LastYear  

select @LastRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@LastYear
select @FirstRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@FirstYear

select @RetainedEarningCOAId=id from COAs where IsRetainedEarning=1
select @NetProfitCOAId=id from COAs where IsNetProfit=1
create table #TempNetChangeNew(id int identity(1,1),TransType varchar(100),OperationType varchar(100),COAId varchar(100),TransactionAmount decimal(18,2))

if	(@FirstStart is NULL) begin set @FirstStart='19000101'; end
if	(@LastStart is NULL) begin set @LastStart='19000101'; end
if	(@FirstEnd is NULL) begin set @FirstEnd='29001231'; end
if	(@LastEnd is NULL) begin set @LastEnd='29001231'; end
if	(@LastYear is NULL) begin set @LastYear='2900'; end
if	(@FirstYear is NULL) begin set @LastYear='1900'; end

select  @LastNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@LastStart and transactionDate <=@LastEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')

select  @FirstNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@FirstStart and transactionDate <=@FirstEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
 

TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstStart 
and transactionDate <=@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Revenue','Expense') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @NetProfitCOAId ,isnull(Sum(@FirstNetProfit),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @RetainedEarningCOAId ,isnull(Sum(@FirstRetainedEarning),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@LastStart 
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Revenue','Expense') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @NetProfitCOAId ,isnull(Sum(@LastNetProfit),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @RetainedEarningCOAId ,isnull(Sum(@LastRetainedEarning),0) 

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount,OpeningAmount,NetChange,ClosingAmount)
select @TransType,''OperationType ,c.COAId, isnull(t.TransactionAmount,0)TransactionAmount
,isnull(case when c.Nature in('Cr') and isnull(IsDepreciation,0)=0 then -1*o.TransactionAmount else o.TransactionAmount end,0) OpeningAmount
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) NetChange
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) ClosingAmount

--,case when c.IsNetProfit =1 then isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0)
--else isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*(isnull(o.TransactionAmount,0)+isnull(n.TransactionAmount,0)) else isnull(o.TransactionAmount,0)+isnull(n.TransactionAmount,0) end,0) end ClosingAmount

from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='Opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
 
select 
case when c.COAType in('Asset') then '1' 
when c.COAType in('Liability') then '2' 
when c.COAType in('Revenue') then '3' 
when c.COAType in('Expense') then '4' 
when c.COAType in('OwnersEquity') then '5' 
when c.COAType in('RetainedEarnings') then '6' 
when c.COAType in('NetProfit') then '7' 
when c.COAType in('NetProfit') then '8' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,
c.Nature , c.COAType ,c.COACode,c.COAName ,
isnull(case when c.Nature in('Dr') and  t.TransactionAmount >=0 then  t.TransactionAmount 
when c.Nature in('Cr') and  t.TransactionAmount >0 then  t.TransactionAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.TransactionAmount <=0 then  -1*t.TransactionAmount 
when c.Nature in('Dr') and  t.TransactionAmount <0 then  -1* t.TransactionAmount 
else 0 end,0) Cr
,t.TransactionAmount
,t.OpeningAmount
,t.NetChange
,t.ClosingAmount

from View_COA_New c 
left outer join    TempNetChangeNew  t on c.COAId=t.COAId
where 1=1 
and (t.TransactionAmount<>0 or t.OpeningAmount<>0 or t.NetChange<>0 or t.ClosingAmount<>0)
and c.COAType in ('Revenue','Expense','OwnersEquity') 
 order by sl,GroupSL,COASL,COACode

 select @FirstStart FirstStart,@FirstEnd FirstEnd,@FirstYear,cast( @LastYear as int)-1 FirstYear,@LastStart LastStart,@LastEnd LastEnd,@LastYear LastYear,
@FirstRetainedEarning FirstRetainedEarning,@FirstNetProfit FirstNetProfit
,@LastRetainedEarning LastRetainedEarning,@LastNetProfit LastNetProfit

drop table #TempNetChangeNew



