declare @MonthFrom as varchar(100)='1148'
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

declare @OpeningNetChange  as decimal(18,4)
declare @ClosingNetChange  as decimal(18,4)


create table #TempNetChangeNew(id int identity(1,1),TransType varchar(100),OperationType varchar(100),COAId varchar(100),TransactionAmount decimal(18,2))
select @RetainedEarningCOAId=id from COAs where IsRetainedEarning=1
select @NetProfitCOAId=id from COAs where IsNetProfit=1

select @LastEnd=PeriodEnd,@LastYear=[Year] from EGCB_HRM.dbo.FiscalYearDetail where id=@MonthTo
select @LastStart=YearStart   from EGCB_HRM.dbo.FiscalYear where [Year]=@LastYear
select @FirstEnd=PeriodStart,@FirstYear=[Year] from EGCB_HRM.dbo.FiscalYearDetail where id=@MonthFrom
select @FirstStart=YearStart   from EGCB_HRM.dbo.FiscalYear where [Year]=@FirstYear

if	(@FirstStart is NULL) begin set @FirstStart='19000101'; end
if	(@LastStart is NULL) begin set @LastStart='19000101'; end
if	(@FirstEnd is NULL) begin set @FirstEnd='29001231'; end
if	(@LastEnd is NULL) begin set @LastEnd='29001231'; end
if	(@LastYear is NULL) begin set @LastYear='2900'; end
if	(@FirstYear is NULL) begin set @FirstYear='1900'; end

select @LastRetainedEarning=isnull(RetainedEarning,0) from NetProfitYearEnds where [Year]=@LastYear
select @FirstRetainedEarning=isnull(RetainedEarning,0) from NetProfitYearEnds where [Year]=@FirstYear

--select top 1 @FirstRetainedEarning=RetainedEarning from NetProfitYearEnds where YearEnd<@FirstStart order by id desc
 
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
--and transactionDate >=@FirstStart 
and transactionDate <@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Asset','Liability','OwnersEquity') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstStart 
and transactionDate <@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Revenue','Expense') 
group by TransType, CoaId



insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Asset','Liability','OwnersEquity') 
group by TransType, CoaId

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

 

select @OpeningNetChange=isnull(sum(TransactionAmount),0) from #TempNetChangeNew where OperationType in('opening')
select @ClosingNetChange=isnull(sum(TransactionAmount),0)   from #TempNetChangeNew where OperationType in('NetChange')


insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @RetainedEarningCOAId ,case when @OpeningNetChange>0 then isnull(Sum(@FirstRetainedEarning),0) else 0 end

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @RetainedEarningCOAId , isnull(Sum(@LastRetainedEarning),0) 


insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount,OpeningAmount,NetChange,ClosingAmount)
select @TransType,''OperationType ,c.COAId
,0 TransactionAmount
,o.TransactionAmount OpeningAmount
,0 NetChange
,n.TransactionAmount ClosingAmount

from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='Opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
 
select 
case when c.COAType in('Asset') then '1' 
when c.COAType in('Liability') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('RetainedEarnings') then '6' 
when c.COAType in('NetProfit') then '7' 
when c.COAType in('NetProfit') then '8' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,
c.Nature , c.COAType ,c.COAId,c.COACode,c.COAName 
,t.TransactionAmount 

,isnull(case when c.Nature in('Dr') and  t.ClosingAmount >=0 then  t.ClosingAmount 
when c.Nature in('Cr') and  t.ClosingAmount >0 then  t.ClosingAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.ClosingAmount <=0 then  -1*t.ClosingAmount 
when c.Nature in('Dr') and  t.ClosingAmount <0 then  -1* t.ClosingAmount 
else 0 end,0) Cr

,isnull(case when c.Nature in('Dr') and  t.OpeningAmount >=0 then  t.OpeningAmount 
when c.Nature in('Cr') and  t.OpeningAmount >0 then  t.OpeningAmount  
else 0 end,0) OpenDr
,isnull(case when c.Nature in('Cr') and  t.OpeningAmount <=0 then  -1*t.OpeningAmount 
when c.Nature in('Dr') and  t.OpeningAmount <0 then  -1* t.OpeningAmount 
else 0 end,0) OpenCr

,isnull(t.TransactionAmount,0)TransactionAmount
,isnull(t.OpeningAmount	   ,0)OpeningAmount	   
,isnull(t.NetChange		   ,0)NetChange		   
,isnull(t.ClosingAmount	   ,0)ClosingAmount	   

from View_COA_New c 
left outer join    TempNetChangeNew  t on c.COAId=t.COAId
where 1=1 
and (   t.ClosingAmount<>0)
--and c.COAType in ('Revenue','Expense','OwnersEquity') 
 order by sl,GroupSL,COASL,COACode


   select @FirstStart FirstStart,@FirstEnd FirstEnd,@FirstYear  FirstYear,@LastStart LastStart,@LastEnd LastEnd,@LastYear LastYear,
isnull(@FirstRetainedEarning,0) FirstRetainedEarning,isnull(@FirstNetProfit,0) FirstNetProfit
,isnull(@LastRetainedEarning,0) LastRetainedEarning,isnull(@LastNetProfit,0) LastNetProfit


drop table #TempNetChangeNew



