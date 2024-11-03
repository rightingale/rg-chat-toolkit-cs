use [RG-Toolkit]
go

-- --- --- ---


create table Tenant
(
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    Name NVARCHAR(100) not null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table Tenant
    add constraint PK_Tenant_ID
    primary key clustered (ID)

create unique index IX_Tenant on Tenant(
    Name
)

-- --- --- ---

create table ContentType
(
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    Name NVARCHAR(100) not null,
    AllowStreamResponse bit not null default 0,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table ContentType
    add constraint PK_ContentType_ID
    primary key clustered (Name)

create unique index IX_ContentType on ContentType(
    Name
)

-- System data:
insert into ContentType
    (Name, AllowStreamResponse)
values
    ('text/plain', 1),
    ('text/html', 1),
    ('application/json', 0),
    ('application/xml', 0),
    ('application/pdf', 1),
    ('image/png', 1),
    ('image/jpeg', 1),
    ('image/gif', 1),
    ('image/svg+xml', 0),
    ('audio/mpeg', 1),
    ('audio/ogg', 1),
    ('audio/wav', 1),
    ('video/mp4', 1),
    ('video/ogg', 1),
    ('video/webm', 1),
    ('application/octet-stream', 1)



-- --- --- ---


create table Prompt
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newID(),

    Name NVARCHAR(100) not null,
    SystemPrompt NVARCHAR(Max) not null,

    ReponseContentTypeName NVARCHAR(100) not null default 'text/plain',
    DoStreamResponse bit null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table Prompt
    add constraint PK_Prompt_ID
    primary key clustered (TenantID, ID)

create unique index IX_Prompt on Prompt(
    TenantID, Name
)
create unique index UQ_Prompt_ID on Prompt(
    ID
) include (IsActive)

-- Relationships
alter table Prompt
    add constraint FK_Prompt_TenantID
    foreign key (TenantID)
    references Tenant(ID)

alter table Prompt
    add constraint FK_Prompt_ContentTypeName
    foreign key (ReponseContentTypeName)
    references ContentType(Name)

-- ---

create table Object
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newID(),

    Name NVARCHAR(100) not null,
    ContentTypeName NVARCHAR(100) not null default 'text/plain',
    ObjectSchema NVARCHAR(Max) null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table Object
    add constraint PK_Object_ID
    primary key clustered (TenantID, ID);

create unique index IX_Object on Object(
    TenantID, Name
)
create unique index UQ_Object_ID on Object (
    ID
) include (IsActive)

-- Relationships
alter table Object
    add constraint FK_Object_TenantID
    foreign key (TenantID)
    references Tenant(ID)

alter table Object
    add constraint FK_Object_ContentTypeName
    foreign key (ContentTypeName)
    references ContentType(Name)

-- ---

create table PromptObjects
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newID(),

    PromptID UNIQUEIDENTIFIER not null,
    ObjectID UNIQUEIDENTIFIER not null,

    -- An object can either be an input or an output
    IsInput bit not null default 0,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table PromptObjects
    add constraint PK_PromptObjects_ID
    primary key clustered (TenantID, ID);

create unique index IX_PromptObjects on PromptObjects(
    TenantID, PromptID, ObjectID
)
create unique index UQ_PromptObjects_ID on PromptObjects (
    ID
) include (IsActive)

-- Relationships include TenantID in each relationship
alter table PromptObjects
    add constraint FK_PromptObjects_PromptID_TenantID
    foreign key (TenantID, PromptID)
    references Prompt(TenantID, ID);

alter table PromptObjects
    add constraint FK_PromptObjects_ObjectID_TenantID
    foreign key (TenantID, ObjectID)
    references Object(TenantID, ID);


-- ---


create table AccessKey
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newID(),

    KeyValue uniqueidentifier not null default newID(),

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table AccessKey
    add constraint PK_AccessKey_ID
    primary key clustered (TenantID, ID);

create unique index IX_AccessKey on AccessKey(
    TenantID, KeyValue
)
create unique index UQ_AccessKey_ID on AccessKey (
    ID
) include (IsActive)

-- Relationships
alter table AccessKey
    add constraint FK_AccessKey_TenantID
    foreign key (TenantID)
    references Tenant(ID)


-- --- --- ---

create table Tool
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    Name NVARCHAR(100) not null,
    Description NVARCHAR(200) not null,
    Parameters NVARCHAR(Max) null,

    Assembly nvarchar(500) null,
    Type nvarchar(500) not null,
    Method nvarchar(500) not null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table Tool
    add constraint PK_Tool_ID
    primary key clustered (TenantID, ID)

create unique index IX_Tool on Tool(
    TenantID, Name
)

-- Relationships
alter table Tool
    add constraint FK_Tool_TenantID
    foreign key (TenantID)
    references Tenant(ID)

-- ---

create table PromptTools
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    PromptID UNIQUEIDENTIFIER not null,
    ToolID UNIQUEIDENTIFIER not null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table PromptTools
    add constraint PK_PromptTools_ID
    primary key clustered (TenantID, ID)

create unique index IX_PromptTools on PromptTools (
    TenantID, PromptID, ToolID
)

-- --- --- ---
-- Filters

create table Filter
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    Name NVARCHAR(100) not null,
    Description NVARCHAR(Max) not null,

    Assembly nvarchar(500) null,
    Type nvarchar(500) null,
    Method nvarchar(500) null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table Filter
    add constraint PK_Filter_ID
    primary key clustered (TenantID, ID)

create unique index IX_Filter on Filter(
    TenantID, Name
)

-- Relationships
alter table Filter
    add constraint FK_Filter_TenantID
    foreign key (TenantID)
    references Tenant(ID)

-- --- --- ---
-- PromptFilters

create table PromptFilters
(
    TenantID UNIQUEIDENTIFIER not null,
    ID UNIQUEIDENTIFIER not null default newSequentialID(),

    PromptID UNIQUEIDENTIFIER not null,
    FilterID UNIQUEIDENTIFIER not null,

    IsActive bit not null default 1,
    CreateDate DATETIME not null default getdate(),
    LastUpdate DATETIME not null default getdate()
)

alter table PromptFilters
    add constraint PK_PromptFilters_ID
    primary key clustered (TenantID, ID)

create unique index IX_PromptFilters on PromptFilters (
    TenantID, PromptID, FilterID
)

-- Relationships
alter table PromptFilters
    add constraint FK_PromptFilters_TenantID
    foreign key (TenantID)
    references Tenant(ID)

-- PromptID
alter table PromptFilters
    add constraint FK_PromptFilters_PromptID
    foreign key (TenantID, PromptID)
    references Prompt(TenantID, ID)

-- FilterID
alter table PromptFilters
    add constraint FK_PromptFilters_FilterID
    foreign key (TenantID, FilterID)
    references Filter(TenantID, ID)

-- --- --- ---
-- Sample data:

declare @tenantID uniqueidentifier = '00000000-0000-0000-0000-000000000000'
insert into Tenant
    (ID, Name)
values
    (@tenantID, 'System')

declare @promptID uniqueidentifier = newID()
insert into Prompt
    (TenantID, ID, Name, SystemPrompt, ReponseContentTypeName, DoStreamResponse)
values
    (@tenantID, @promptID, 'demo_greeter', 'You are a helpful greeter.', 'text/plain', null)

insert into Prompt
    (TenantID, ID, Name, SystemPrompt, ReponseContentTypeName, DoStreamResponse)
values
    (@tenantID, newID(), 'demo_greeter_weather', 'You are a helpful greeter. Lookup current weather conditions for a random major city and return it with your greeting.', 'text/plain', 1)


update Prompt set SystemPrompt='You are a helpful greeter. Lookup current weather conditions for a random major city and return it with your greeting.
Please re-format the weather as plain text. Include the name of the nearby city.' where Name='demo_greeter_weather'


declare @objectID uniqueidentifier = newID()
insert into Object
    (TenantID, ID, Name, ContentTypeName, ObjectSchema)
values
    (@tenantID, @objectID, 'input_message', 'text/plain', null)

insert into PromptObjects
    (TenantID, PromptID, ObjectID, IsInput)
values
    (@tenantID, @promptID, @objectID, 1)

declare @toolID uniqueidentifier = newID()
declare @toolParameters nvarchar(max) = N'{"Type": "object", "Properties": {"message": {"Type": "string"}}, "Required": ["message"]}'
insert into Tool
    (TenantID, ID, Name, Description, Parameters, Assembly, Type, Method)
values
    (@tenantID, @toolID, 'get_greeting', 'Gets the next greeting', @toolParameters, null/*Assembly*/, 'DefaultToolHelper', 'GetGreeting')

insert into PromptTools
    (TenantID, PromptID, ToolID)
values
    (@tenantID, @promptID, @toolID)

insert into AccessKey
    (TenantID, KeyValue)
values
    (@tenantID, newID())


-- HAP speech quality filter
declare @filterID uniqueidentifier = newID()
insert into Filter
    (TenantID, ID, Name, Description, Assembly, Type, Method)
values
    (@tenantID, @filterID, 'hap_default', 'Lightweight default HAP filter.', null, null, null)


-- demo_greeter_weather Prompt ID
declare @filter_PromptID uniqueidentifier = (select ID
from Prompt
where Name='demo_greeter_weather')

insert into PromptFilters
    (TenantID, PromptID, FilterID)
values
    (@tenantID, @filter_PromptID, @filterID)


-- --- --- ---
-- Sample data:

select top 100
    *
from Tenant

select top 100
    *
from ContentType

select top 100
    *
from Prompt

select top 100
    *
from Object

select top 100
    *
from PromptObjects

select top 100
    *
from Tool

select top 100
    *
from PromptTools

select top 100
    *
from Filter

select top 100
    *
from PromptFilters

select top 100
    *
from AccessKey



/*


-- --- --- ---
-- Cleanup:


drop table PromptObjects
drop table AccessKey
drop table Tool
drop table PromptTools
drop table PromptFilters
drop table Filter
drop table Prompt
drop table Object
drop table ContentType
drop table Tenant





*/
