use [RG-Toolkit]
go

create table Memory (
    [TenantID] [uniqueidentifier] NOT NULL,
    [ID] [uniqueidentifier] NOT NULL default newID(),

    Name nvarchar(100) not null,
    Description nvarchar(max) null,
    DescriptionEmbedding nvarchar(max) null,

    MemoryType nvarchar(500) not null,

    [Is_Active] bit not null default 1,
    CreateDate datetime not null default getdate(),
    LastUpdate datetime not null default getdate()

    -- Primary key
    constraint PK_Memory primary key clustered (TenantID, ID)
)

-- Indexes

create unique index IX_Memory_ID on Memory (TenantID, ID)
GO
create unique index IX_Memory_Name on Memory (TenantID, Name)
GO

-- Foreign keys

alter table Memory
    add constraint FK_Memory_TenantID foreign key (TenantID) references Tenant (ID)



-- --- --- ---
-- Relationships

-- PromptMemories
if not exists (select * from sysobjects where name='PromptMemories' and xtype='U') begin
    create table PromptMemories (
        [TenantID] [uniqueidentifier] NOT NULL,
        [ID] [uniqueidentifier] NOT NULL default newID(),

        PromptID uniqueidentifier not null,
        MemoryID uniqueidentifier not null,

        Ordinal Int not null default 0,

        [Is_Active] bit not null default 1,
        CreateDate datetime not null default getdate(),
        LastUpdate datetime not null default getdate()

        -- Primary key
        constraint PK_PromptMemories primary key clustered (TenantID, ID)
    )
end

create unique index IX_PromptMemories_ID on PromptMemories (TenantID, ID)
GO
create unique index IX_PromptMemories_PromptID_MemoryID on PromptMemories (TenantID, PromptID, MemoryID) where Is_Active = 1
GO
create unique index IX_PromptMemories_Ordinal on PromptMemories (TenantID, PromptID, Ordinal) where Is_Active = 1
go

-- --- --- ---

-- Foreign keys

alter table PromptMemories
    add constraint FK_PromptMemories_PromptID foreign key (TenantID, PromptID) references Prompt (TenantID, ID)

alter table PromptMemories
    add constraint FK_PromptMemories_MemoryID foreign key (TenantID, MemoryID) references Memory (TenantID, ID)

-- --- --- ---

-- drop table MemoryItem

create table MemoryItem (
    [TenantID] [uniqueidentifier] NOT NULL,
    [ID] [uniqueidentifier] NOT NULL default newID(),

    MemoryID uniqueidentifier not null,
    Section nvarchar(100) not null,
    Module nvarchar(100) not null,
    Object nvarchar(100) null,
    Category nvarchar(100) null,
    Item nvarchar(max) not null,

    Description nvarchar(max) null,

    [Is_Active] bit not null default 1,
    CreateDate datetime not null default getdate(),
    LastUpdate datetime not null default getdate()

    -- Primary key
    constraint PK_MemoryItem primary key clustered (TenantID, ID)
)

-- Indexes

create unique index IX_MemoryItem_ID on MemoryItem (TenantID, ID)
GO

-- Foreign keys

alter table MemoryItem
    add constraint FK_MemoryItem_MemoryID foreign key (TenantID, MemoryID) references Memory (TenantID, ID)

alter table MemoryItem
    add constraint FK_MemoryItem_TenantID foreign key (TenantID) references Tenant (ID)




-- --- --- ---

select * from Memory

select * from PromptMemories

select * from MemoryItem

-- 

-- Create 

-- /vault
insert into MemoryItem (TenantID, ID, MemoryID, Section, Module, Object, Category, Item, Description, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', newID(), 'd6566df7-d61b-42dd-abe7-e121af01c2e6', 
        'tilley', 'vault', null, null, null, 
        'Farm Vault: Create and Edit Farm, Tract, Field records including APH, Production History, Yields, Acreage, Base Acres.', getdate(), getdate())

-- /budget
insert into MemoryItem (TenantID, ID, MemoryID, Section, Module, Object, Category, Item, Description, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', newID(), 'd6566df7-d61b-42dd-abe7-e121af01c2e6', 
        'tilley', 'budget', null, null, null, 
        'Farm Budget: Create and Edit Farm Budgets, including Revenue, Expenses.', getdate(), getdate())

-- /arc_plc
insert into MemoryItem (TenantID, ID, MemoryID, Section, Module, Object, Category, Item, Description, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', newID(), 'd6566df7-d61b-42dd-abe7-e121af01c2e6', 
        'tilley', 'arc_plc', null, null, null, 
        'Farm ARC/PLC: View ARC-County (ARC or ARC-CO) and PLC (Price Loss Coverage) information, including Base Acres, projected payments.', getdate(), getdate())


-- Ooops MemoryItems should be json object here

/*
``` json
{"section": "tilley", "module": "vault", "object": null, "category": null, "item": null, "description": "Farm Vault: Create and Edit Farm, Tract, Field records including APH, Production History, Yields, Acreage, Base Acres."}
{"section": "tilley", "module": "budget", "object": null, "category": null, "item": null, "description": "Farm Budget: Create and Edit Farm Budgets, including Revenue, Expenses."}
{"section": "tilley", "module": "arc_plc", "object": null, "category": null, "item": null, "description": "Farm ARC/PLC: View ARC-County (ARC or ARC-CO) and PLC (Price Loss Coverage) information, including Base Acres, projected payments."}

```
*/