use [RG-Toolkit]
go

create table PromptUtterances (
    TenantID uniqueidentifier not null,
    PromptID uniqueidentifier not null,
    ID uniqueidentifier not null default newID(),
    
    Utterance nvarChar(200) not null,

    LastUpdate datetime not null default getdate(),
    CreateDate datetime not null default getdate(),

    constraint PK_PromptUtterances primary key clustered (TenantID, PromptID, ID)
)

create unique index IX_PromptUtterances_ID on PromptUtterances (TenantID, PromptID, ID)
go

create unique index IX_PromptUtterances_Utterance on PromptUtterances (TenantID, Utterance)
go

alter table PromptUtterances
add constraint FK_PromptUtterances_PromptID foreign key (TenantID, PromptID) references Prompt (TenantID, ID)

-- ---

alter table Prompt add Description nvarchar(500) null
go
update Prompt set Description = name where description is null
Go

alter table Prompt alter column Description nvarchar(500) not null

-- ---



--.
