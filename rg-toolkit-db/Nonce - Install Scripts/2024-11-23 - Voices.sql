use [RG-Toolkit]
go

if not exists (select * from sysobjects where name='Persona' and xtype='U') begin
    create table Persona (
        [TenantID] [uniqueidentifier] NOT NULL,
        [ID] [uniqueidentifier] NOT NULL default newID(),

        Name nvarchar(100) not null,
        Gender char(1) not null,
        LanguageCode varchar(20) null,
        SystemPrompt nvarchar(max) null,

        [Is_Active] bit not null default 1,
        CreateDate datetime not null default getdate(),
        LastUpdate datetime not null default getdate()

        -- Primary key
        constraint PK_Persona primary key clustered (TenantID, ID)
    )
end
go

create unique index IX_Persona_ID on Persona (TenantID, ID)
GO
create unique index IX_Persona_Name on Persona (TenantID, Name)
GO

-- --- --- ---


if not exists (select * from sysobjects where name='PromptPersonas' and xtype='U') begin
    create table PromptPersonas (
        [TenantID] [uniqueidentifier] NOT NULL,
        [ID] [uniqueidentifier] NOT NULL default newID(),

        PromptID uniqueidentifier not null,
        PersonaID uniqueidentifier not null,

        Ordinal Int not null default 0,

        [Is_Active] bit not null default 1,
        CreateDate datetime not null default getdate(),
        LastUpdate datetime not null default getdate()

        -- Primary key
        constraint PK_PromptPersonas primary key clustered (TenantID, ID)
    )
end
go

create unique index IX_PromptPersonas_ID on PromptPersonas (TenantID, ID)
GO
create unique index IX_PromptPersonas_PromptID_PersonaID on PromptPersonas (TenantID, PromptID, PersonaID) where Is_Active = 1
GO
create unique index IX_PromptPersonas_Ordinal on PromptPersonas (TenantID, PromptID, Ordinal) where Is_Active = 1
go


-- Relationships
alter table PromptPersonas
    add constraint FK_PromptPersonas_Prompt foreign key (TenantID, PromptID) references Prompt(TenantID, ID)
go
alter table PromptPersonas
    add constraint FK_PromptPersonas_Persona foreign key (TenantID, PersonaID) references Persona(TenantID, ID)


-- --- --- ---
-- System data

declare @TenantID_System uniqueidentifier = '787923AB-0D9F-EF11-ACED-021FE1D77A3B'
declare @PromptID uniqueIdentifier = (select ID from Prompt where TenantID = '787923AB-0D9F-EF11-ACED-021FE1D77A3B' and Name = 'instore_experience_helper')

delete from PromptPersonas where TenantID = '787923AB-0D9F-EF11-ACED-021FE1D77A3B'
delete from Persona where TenantID = '787923AB-0D9F-EF11-ACED-021FE1D77A3B'

declare @personaID_female uniqueIdentifier = newID()
insert into Persona (TenantID, ID, Name, Gender, LanguageCode) values (@TenantID_System, @personaID_female, 'chef_female', 'F', 'en')
insert into PromptPersonas (TenantID, PromptID, PersonaID, Ordinal) values (@TenantID_System, @PromptID, @personaID_female, 0)


declare @personaID_male uniqueIdentifier = newID()
insert into Persona (TenantID, ID, Name, Gender, LanguageCode) values (@TenantID_System, @personaID_male, 'chef_male', 'M', 'en')
insert into PromptPersonas (TenantID, PromptID, PersonaID, Ordinal) values (@TenantID_System, @PromptID, @personaID_male, 1)


declare @personaID_female_Spanish uniqueIdentifier = newID()
insert into Persona (TenantID, ID, Name, Gender, LanguageCode) values (@TenantID_System, @personaID_female_Spanish, 'chef_female_spanish', 'F', 'es')
insert into PromptPersonas (TenantID, PromptID, PersonaID, Ordinal) values (@TenantID_System, @PromptID, @personaID_female_Spanish, 2)


declare @personaID_male_Spanish uniqueIdentifier = newID()
insert into Persona (TenantID, ID, Name, Gender, LanguageCode) values (@TenantID_System, @personaID_male_Spanish, 'chef_male_spanish', 'M', 'es')
insert into PromptPersonas (TenantID, PromptID, PersonaID, Ordinal) values (@TenantID_System, @PromptID, @personaID_male_Spanish, 3)

-- ---

select *
from Persona

select *
from PromptPersonas