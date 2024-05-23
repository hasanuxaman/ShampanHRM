select * from View_GLJournalDetailNew
where 1=1
and TransactionDate>'20220630'  and TransactionDate<='20230630'
and COAType in ('Revenue','Expense','OwnersEquity') 

select distinct TransType,'NetChange' tt, CoaId ,isnull(Sum(isnull(TransactionAmount,0)),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and TransactionDate>'20220630'  and TransactionDate<='20230630'
and COAType in ('Revenue','Expense','OwnersEquity') 

--and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
group by TransType, CoaId

--and TransType in(@TransType)