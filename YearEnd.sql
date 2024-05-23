declare @YearStart as varchar(100);
declare @YearEnd as varchar(100);
declare @Year as varchar(100);
declare @TransType as varchar(100)='PF' -- closing

declare @RetainedEarningsCOAId as varchar(100)
select @RetainedEarningsCOAId=id from COAs  where isnull(IsRetainedEarning,0)=1  
--select @RetainedEarningsCOAId RetainedEarningsCOAId

create table #FiscalYear(Id int identity(1,1),[Year] varchar(100),YearStart varchar(100),YearEnd varchar(100))

insert into #FiscalYear([Year],YearStart,YearEnd)
select [Year],YearStart,YearEnd from EGCB_HRM.dbo.FiscalYear



TRUNCATE TABLE NetProfitYearEnds; 
DBCC CHECKIDENT ('NetProfitYearEnds', RESEED, 1);

DECLARE @Counter INT 
SET @Counter=1
WHILE ( @Counter <= (select count(id) from #FiscalYear) )
BEGIN
select @Year=[Year],@YearStart=YearStart ,@YearEnd=YearEnd from #FiscalYear where id=@Counter
 
 insert into NetProfitYearEnds(TransType,[Year],YearStart,YearEnd,COAId,CoaType,TransactionAmount,NetProfit,RetainedEarning)
 select @TransType,@Year,@YearStart,@YearEnd,0,'',0 TransactionAmount,sum(NetProfit)NetProfit,sum(RetainedEarning)RetainedEarning from 
 (
select  isnull(Sum(TransactionAmount),0)NetProfit, 0 RetainedEarning
from View_GLJournalDetailNew 
where transactionDate >=@YearStart and transactionDate <=@YearEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
union all
select 0 NetProfit,isnull(Sum(TransactionAmount),0)TransactionAmount 
from View_GLJournalDetailNew 
where transactionDate >=@YearStart and transactionDate <= @YearEnd  and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAId in (@RetainedEarningsCOAId)
) as a

    SET @Counter  = @Counter  + 1
END

SET @Counter=1
WHILE ( @Counter <= (select count(id) from NetProfitYearEnds) )
BEGIN
declare @NetProfit as decimal(18,4)=0;
declare @RetainedEarning  as decimal(18,4)=0;

select @NetProfit=NetProfit,@RetainedEarning=RetainedEarning from NetProfitYearEnds where id=@Counter-1
update NetProfitYearEnds set RetainedEarning=RetainedEarning+@RetainedEarning+@NetProfit where id=@Counter

    SET @Counter  = @Counter  + 1
END

select * from NetProfitYearEnds

drop table #FiscalYear

