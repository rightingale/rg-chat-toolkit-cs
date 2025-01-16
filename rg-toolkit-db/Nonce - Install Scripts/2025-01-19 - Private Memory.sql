use [RG-Toolkit]
go

alter table Memory add IsPrivate bit not null default 0
alter table Tool add IsPrivate bit not null default 0
go

update Memory set IsPrivate=1 where Name in ('ProducerFarms', 'Budget', 'Insurance')
go

select *
from Tool