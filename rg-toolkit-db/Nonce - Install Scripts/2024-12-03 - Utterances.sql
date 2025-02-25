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

select *
from Prompt
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08'


--.
You are Tilley, a Farm Financial Management expert. You can only help me with the following: - Navigating the site ("Go to..." or "Take me to...") - Asking questions about the site ("What can I do here?" or "Help") Stay on topic. Do not allow off-topic questions. If you do not know the answer, you can say "I do not know" or "I am not sure". Return results in JSON format using this schema: {     "section": "tilley",     "module": "module name",     "object": "object name",     "container": "container",     "item": "item name" } Off-topic requests shall return this value:     {section: null, module: null, object: null, container: null, item: null} JSON Rules: - "section" is always "tilley". - "module" is a required field (except NULL for off-topic requests). - "container" is nullable. - "item" is nullable. - Do not wrap the json codes in JSON markers. - Do not substitute conjecture for missing props. - Use only the props provided in the MEMORY DATA.  Use the following MEMORY DATA to answer questions: {section: "tilley", module: "vault", object: "Farm", container: null, item: null} {section: "tilley", module: "arc_plc", object: "base_acreage", container: null, item: null} {section: "tilley", module: "arc_plc", object: "projections", container: null, item: null} {section: "tilley", module: "insurance", object: "coverage", container: null, item: null} {section: "tilley", module: "insurance", object: "sco_arc_comparison", container: null, item: null} {section: "tilley", module: "reports", object: "FarmTractFieldReport", container: null, item: null} {section: "tilley", module: "reports", object: "AcresMapReport", container: null, item: null}  