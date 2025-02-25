use [RG-Toolkit]
go

-- Tenant ID 902544DA-67E6-4FA8-A346-D1FAA8B27A08

/*


Authentication

ID
TenantID


JWTAuthorization

ID
TenantID
AuthenticationID
RoleAttributeName
SuperUserAttributeValue
ValidIssuer
IsActive bit not null default 1




*/


drop table JWTAuthorization
drop table Authentication
GO

create table Authentication (
    [TenantID] [uniqueidentifier] NOT NULL,
    [ID] [uniqueidentifier] NOT NULL default newID(),
    Name nvarchar(100) not null,

    CreateDate datetime not null default getdate(),
    LastUpdate datetime not null default getdate(),

    -- Primary key
    constraint PK_Authentication primary key clustered (TenantID, ID)
)

-- Indexes

create unique index IX_Authentication_ID on Authentication (TenantID, ID)
GO

-- Foreign keys

alter table Authentication
    add constraint FK_Authentication_TenantID foreign key (TenantID) references Tenant (ID)
GO

-- --- ---

create table JWTAuthorization (
    [TenantID] [uniqueidentifier] NOT NULL,
    [ID] [uniqueidentifier] NOT NULL default newID(),
    AuthenticationID uniqueidentifier not null,

    ValidIssuer nvarchar(255) not null,
    JwksUri nvarchar(255) not null,

    UserIDAttributeName nvarchar(100) not null,
    RoleAttributeName nvarchar(100) not null,
    SuperUserAttributeValue nvarchar(100) not null,
    IsActive bit not null default 1,

    CreateDate datetime not null default getdate(),
    LastUpdate datetime not null default getdate(),

    -- Primary key
    constraint PK_JWTAuthorization primary key clustered (TenantID, ID)
)

-- Indexes

create unique index IX_JWTAuthorization_ID on JWTAuthorization (TenantID, ID)

create unique index IX_JWTAuthorization_AuthenticationID on JWTAuthorization (TenantID, AuthenticationID)
GO

-- Foreign keys

alter table JWTAuthorization
    add constraint FK_JWTAuthorization_AuthenticationID foreign key (TenantID, AuthenticationID) references Authentication (TenantID, ID)
GO

alter table JWTAuthorization
    add constraint FK_JWTAuthorization_TenantID foreign key (TenantID) references Tenant (ID)
go


-- --- --- ---

-- Authentication: "Datanac JWT" b0046219-a679-4e84-b96f-b0030f11ffbc
insert into Authentication (TenantID, ID, Name)
values ('902544DA-67E6-4FA8-A346-D1FAA8B27A08', 'b0046219-a679-4e84-b96f-b0030f11ffbc', 'Datanac JWT')

-- JWTAuthorization
insert into JWTAuthorization (TenantID, ID, AuthenticationID, UserIDAttributeName, RoleAttributeName, SuperUserAttributeValue, ValidIssuer, JwksUri)
values ('902544DA-67E6-4FA8-A346-D1FAA8B27A08', newid(), 'b0046219-a679-4e84-b96f-b0030f11ffbc', 'custom:producer_token', 'custom:datanac_user_role', 'admin', 'https://cognito-idp.us-east-2.amazonaws.com/us-east-2_CrGQj2ghJ', 'https://cognito-idp.us-east-2.amazonaws.com/us-east-2_CrGQj2ghJ/.well-known/jwks.json')

