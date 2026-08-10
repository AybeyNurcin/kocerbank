select * from KB_HESAPBILGILERI where length(IBAN) = 26;
select * from KB_HESAPHAREKETI order by recorddate desc;
select * from KB_PARATRANSFERI order by recorddate desc;

select sifre from kb_personel ORDER BY recorddate desc;