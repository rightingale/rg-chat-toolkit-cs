USE [RG-Toolkit]
GO
/****** Object:  Table [dbo].[AccessKey]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccessKey]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AccessKey](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[KeyValue] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccessKey_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ContentType]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContentType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ContentType](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[AllowStreamResponse] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ContentType_ID] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Filter]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Filter]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Filter](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Assembly] [nvarchar](500) NULL,
	[Type] [nvarchar](500) NULL,
	[Method] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Filter_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Memory]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Memory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Memory](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DescriptionEmbedding] [nvarchar](max) NULL,
	[MemoryType] [nvarchar](500) NOT NULL,
	[Is_Active] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Memory] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Object]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Object]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Object](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ContentTypeName] [nvarchar](100) NOT NULL,
	[ObjectSchema] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Object_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Persona]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Persona]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Persona](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Gender] [char](1) NOT NULL,
	[LanguageCode] [varchar](20) NULL,
	[SystemPrompt] [nvarchar](max) NULL,
	[Is_Active] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Prompt]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Prompt]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Prompt](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemPrompt] [nvarchar](max) NOT NULL,
	[ReponseContentTypeName] [nvarchar](100) NOT NULL,
	[DoStreamResponse] [bit] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Prompt_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptFilters]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptFilters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptFilters](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[FilterID] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptFilters_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptMemories]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptMemories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptMemories](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[MemoryID] [uniqueidentifier] NOT NULL,
	[Ordinal] [int] NOT NULL,
	[Is_Active] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptMemories] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptObjects]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptObjects]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptObjects](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[IsInput] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptObjects_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptPersonas]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptPersonas]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptPersonas](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[PersonaID] [uniqueidentifier] NOT NULL,
	[Ordinal] [int] NOT NULL,
	[Is_Active] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptPersonas] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptTools]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptTools]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptTools](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[ToolID] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptTools_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PromptUtterances]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromptUtterances]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PromptUtterances](
	[TenantID] [uniqueidentifier] NOT NULL,
	[PromptID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Utterance] [nvarchar](200) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PromptUtterances] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Tenant]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tenant]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tenant](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Tenant_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Tool]    Script Date: 1/15/2025 6:36:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tool]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tool](
	[TenantID] [uniqueidentifier] NOT NULL,
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[Assembly] [nvarchar](500) NULL,
	[Type] [nvarchar](500) NOT NULL,
	[Method] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Tool_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Index [IX_AccessKey]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AccessKey]') AND name = N'IX_AccessKey')
CREATE UNIQUE NONCLUSTERED INDEX [IX_AccessKey] ON [dbo].[AccessKey]
(
	[TenantID] ASC,
	[KeyValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_AccessKey_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AccessKey]') AND name = N'UQ_AccessKey_ID')
CREATE UNIQUE NONCLUSTERED INDEX [UQ_AccessKey_ID] ON [dbo].[AccessKey]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ContentType]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ContentType]') AND name = N'IX_ContentType')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ContentType] ON [dbo].[ContentType]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Filter]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Filter]') AND name = N'IX_Filter')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Filter] ON [dbo].[Filter]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Memory_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Memory]') AND name = N'IX_Memory_ID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Memory_ID] ON [dbo].[Memory]
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Memory_Name]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Memory]') AND name = N'IX_Memory_Name')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Memory_Name] ON [dbo].[Memory]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Object]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Object]') AND name = N'IX_Object')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Object] ON [dbo].[Object]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Object_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Object]') AND name = N'UQ_Object_ID')
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Object_ID] ON [dbo].[Object]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Persona_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Persona]') AND name = N'IX_Persona_ID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Persona_ID] ON [dbo].[Persona]
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Persona_Name]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Persona]') AND name = N'IX_Persona_Name')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Persona_Name] ON [dbo].[Persona]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Prompt]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Prompt]') AND name = N'IX_Prompt')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Prompt] ON [dbo].[Prompt]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Prompt_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Prompt]') AND name = N'UQ_Prompt_ID')
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Prompt_ID] ON [dbo].[Prompt]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptFilters]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptFilters]') AND name = N'IX_PromptFilters')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptFilters] ON [dbo].[PromptFilters]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[FilterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptMemories_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptMemories]') AND name = N'IX_PromptMemories_ID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptMemories_ID] ON [dbo].[PromptMemories]
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptMemories_Ordinal]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptMemories]') AND name = N'IX_PromptMemories_Ordinal')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptMemories_Ordinal] ON [dbo].[PromptMemories]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[Ordinal] ASC
)
WHERE ([Is_Active]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptMemories_PromptID_MemoryID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptMemories]') AND name = N'IX_PromptMemories_PromptID_MemoryID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptMemories_PromptID_MemoryID] ON [dbo].[PromptMemories]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[MemoryID] ASC
)
WHERE ([Is_Active]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptObjects]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptObjects]') AND name = N'IX_PromptObjects')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptObjects] ON [dbo].[PromptObjects]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_PromptObjects_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptObjects]') AND name = N'UQ_PromptObjects_ID')
CREATE UNIQUE NONCLUSTERED INDEX [UQ_PromptObjects_ID] ON [dbo].[PromptObjects]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptPersonas_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptPersonas]') AND name = N'IX_PromptPersonas_ID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptPersonas_ID] ON [dbo].[PromptPersonas]
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptPersonas_Ordinal]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptPersonas]') AND name = N'IX_PromptPersonas_Ordinal')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptPersonas_Ordinal] ON [dbo].[PromptPersonas]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[Ordinal] ASC
)
WHERE ([Is_Active]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptPersonas_PromptID_PersonaID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptPersonas]') AND name = N'IX_PromptPersonas_PromptID_PersonaID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptPersonas_PromptID_PersonaID] ON [dbo].[PromptPersonas]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[PersonaID] ASC
)
WHERE ([Is_Active]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptTools]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptTools]') AND name = N'IX_PromptTools')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptTools] ON [dbo].[PromptTools]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ToolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptUtterances_ID]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptUtterances]') AND name = N'IX_PromptUtterances_ID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptUtterances_ID] ON [dbo].[PromptUtterances]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PromptUtterances_Utterance]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PromptUtterances]') AND name = N'IX_PromptUtterances_Utterance')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptUtterances_Utterance] ON [dbo].[PromptUtterances]
(
	[TenantID] ASC,
	[Utterance] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenant]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Tenant]') AND name = N'IX_Tenant')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenant] ON [dbo].[Tenant]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tool]    Script Date: 1/15/2025 6:36:06 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Tool]') AND name = N'IX_Tool')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tool] ON [dbo].[Tool]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__AccessKey__ID__46D27B73]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__AccessKey__KeyVa__47C69FAC]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (newid()) FOR [KeyValue]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__AccessKey__IsAct__48BAC3E5]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__AccessKey__Creat__49AEE81E]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__AccessKey__LastU__4AA30C57]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentType__ID__2665ABE1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentTy__Allow__2759D01A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT ((0)) FOR [AllowStreamResponse]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentTy__IsAct__284DF453]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentTy__Creat__2942188C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__ContentTy__LastU__2A363CC5]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Filter__ID__5AD97420]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Filter__IsActive__5BCD9859]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Filter] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Filter__CreateDa__5CC1BC92]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Filter__LastUpda__5DB5E0CB]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Memory__ID__795DFB40]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Memory] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Memory__Is_Activ__7A521F79]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Memory] ADD  DEFAULT ((1)) FOR [Is_Active]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Memory__CreateDa__7B4643B2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Memory] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Memory__LastUpda__7C3A67EB]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Memory] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Object__ID__35A7EF71]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Object] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Object__ContentT__369C13AA]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Object] ADD  DEFAULT ('text/plain') FOR [ContentTypeName]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Object__IsActive__379037E3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Object] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Object__CreateDa__38845C1C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Object] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Object__LastUpda__39788055]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Object] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Persona__ID__6B0FDBE9]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Persona] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Persona__Is_Acti__6C040022]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Persona] ADD  DEFAULT ((1)) FOR [Is_Active]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Persona__CreateD__6CF8245B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Persona] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Persona__LastUpd__6DEC4894]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Persona] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Prompt__ID__2D12A970]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Prompt__ReponseC__2E06CDA9]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT ('text/plain') FOR [ReponseContentTypeName]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Prompt__IsActive__2EFAF1E2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Prompt__CreateDa__2FEF161B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Prompt__LastUpda__30E33A54]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptFilter__ID__618671AF]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptFil__IsAct__627A95E8]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptFil__Creat__636EBA21]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptFil__LastU__6462DE5A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptMemori__ID__000AF8CF]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptMemories] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptMem__Ordin__00FF1D08]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptMemories] ADD  DEFAULT ((0)) FOR [Ordinal]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptMem__Is_Ac__01F34141]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptMemories] ADD  DEFAULT ((1)) FOR [Is_Active]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptMem__Creat__02E7657A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptMemories] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptMem__LastU__03DB89B3]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptMemories] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptObject__ID__3E3D3572]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptObj__IsInp__3F3159AB]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT ((0)) FOR [IsInput]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptObj__IsAct__40257DE4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptObj__Creat__4119A21D]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptObj__LastU__420DC656]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptPerson__ID__70C8B53F]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptPersonas] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptPer__Ordin__71BCD978]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptPersonas] ADD  DEFAULT ((0)) FOR [Ordinal]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptPer__Is_Ac__72B0FDB1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptPersonas] ADD  DEFAULT ((1)) FOR [Is_Active]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptPer__Creat__73A521EA]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptPersonas] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptPer__LastU__74994623]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptPersonas] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptTools__ID__55209ACA]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptToo__IsAct__5614BF03]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptToo__Creat__5708E33C]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptToo__LastU__57FD0775]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptUttera__ID__1F83A428]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptUtterances] ADD  DEFAULT (newid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptUtt__LastU__2077C861]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptUtterances] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__PromptUtt__Creat__216BEC9A]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PromptUtterances] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tenant__ID__20ACD28B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tenant__IsActive__21A0F6C4]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tenant__CreateDa__22951AFD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tenant__LastUpda__23893F36]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tool__ID__4E739D3B]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (newsequentialid()) FOR [ID]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tool__IsActive__4F67C174]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tool] ADD  DEFAULT ((1)) FOR [IsActive]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tool__CreateDate__505BE5AD]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__Tool__LastUpdate__515009E6]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (getdate()) FOR [LastUpdate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AccessKey_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[AccessKey]'))
ALTER TABLE [dbo].[AccessKey]  WITH CHECK ADD  CONSTRAINT [FK_AccessKey_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AccessKey_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[AccessKey]'))
ALTER TABLE [dbo].[AccessKey] CHECK CONSTRAINT [FK_AccessKey_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Filter_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Filter]'))
ALTER TABLE [dbo].[Filter]  WITH CHECK ADD  CONSTRAINT [FK_Filter_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Filter_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Filter]'))
ALTER TABLE [dbo].[Filter] CHECK CONSTRAINT [FK_Filter_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Memory_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Memory]'))
ALTER TABLE [dbo].[Memory]  WITH CHECK ADD  CONSTRAINT [FK_Memory_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Memory_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Memory]'))
ALTER TABLE [dbo].[Memory] CHECK CONSTRAINT [FK_Memory_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Object_ContentTypeName]') AND parent_object_id = OBJECT_ID(N'[dbo].[Object]'))
ALTER TABLE [dbo].[Object]  WITH CHECK ADD  CONSTRAINT [FK_Object_ContentTypeName] FOREIGN KEY([ContentTypeName])
REFERENCES [dbo].[ContentType] ([Name])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Object_ContentTypeName]') AND parent_object_id = OBJECT_ID(N'[dbo].[Object]'))
ALTER TABLE [dbo].[Object] CHECK CONSTRAINT [FK_Object_ContentTypeName]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Object_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Object]'))
ALTER TABLE [dbo].[Object]  WITH CHECK ADD  CONSTRAINT [FK_Object_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Object_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Object]'))
ALTER TABLE [dbo].[Object] CHECK CONSTRAINT [FK_Object_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Prompt_ContentTypeName]') AND parent_object_id = OBJECT_ID(N'[dbo].[Prompt]'))
ALTER TABLE [dbo].[Prompt]  WITH CHECK ADD  CONSTRAINT [FK_Prompt_ContentTypeName] FOREIGN KEY([ReponseContentTypeName])
REFERENCES [dbo].[ContentType] ([Name])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Prompt_ContentTypeName]') AND parent_object_id = OBJECT_ID(N'[dbo].[Prompt]'))
ALTER TABLE [dbo].[Prompt] CHECK CONSTRAINT [FK_Prompt_ContentTypeName]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Prompt_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Prompt]'))
ALTER TABLE [dbo].[Prompt]  WITH CHECK ADD  CONSTRAINT [FK_Prompt_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Prompt_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Prompt]'))
ALTER TABLE [dbo].[Prompt] CHECK CONSTRAINT [FK_Prompt_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_FilterID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_FilterID] FOREIGN KEY([TenantID], [FilterID])
REFERENCES [dbo].[Filter] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_FilterID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_FilterID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_PromptID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_PromptID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptFilters_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptFilters]'))
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptMemories_MemoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptMemories]'))
ALTER TABLE [dbo].[PromptMemories]  WITH CHECK ADD  CONSTRAINT [FK_PromptMemories_MemoryID] FOREIGN KEY([TenantID], [MemoryID])
REFERENCES [dbo].[Memory] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptMemories_MemoryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptMemories]'))
ALTER TABLE [dbo].[PromptMemories] CHECK CONSTRAINT [FK_PromptMemories_MemoryID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptMemories_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptMemories]'))
ALTER TABLE [dbo].[PromptMemories]  WITH CHECK ADD  CONSTRAINT [FK_PromptMemories_PromptID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptMemories_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptMemories]'))
ALTER TABLE [dbo].[PromptMemories] CHECK CONSTRAINT [FK_PromptMemories_PromptID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptObjects_ObjectID_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptObjects]'))
ALTER TABLE [dbo].[PromptObjects]  WITH CHECK ADD  CONSTRAINT [FK_PromptObjects_ObjectID_TenantID] FOREIGN KEY([TenantID], [ObjectID])
REFERENCES [dbo].[Object] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptObjects_ObjectID_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptObjects]'))
ALTER TABLE [dbo].[PromptObjects] CHECK CONSTRAINT [FK_PromptObjects_ObjectID_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptObjects_PromptID_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptObjects]'))
ALTER TABLE [dbo].[PromptObjects]  WITH CHECK ADD  CONSTRAINT [FK_PromptObjects_PromptID_TenantID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptObjects_PromptID_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptObjects]'))
ALTER TABLE [dbo].[PromptObjects] CHECK CONSTRAINT [FK_PromptObjects_PromptID_TenantID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptPersonas_Persona]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptPersonas]'))
ALTER TABLE [dbo].[PromptPersonas]  WITH CHECK ADD  CONSTRAINT [FK_PromptPersonas_Persona] FOREIGN KEY([TenantID], [PersonaID])
REFERENCES [dbo].[Persona] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptPersonas_Persona]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptPersonas]'))
ALTER TABLE [dbo].[PromptPersonas] CHECK CONSTRAINT [FK_PromptPersonas_Persona]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptPersonas_Prompt]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptPersonas]'))
ALTER TABLE [dbo].[PromptPersonas]  WITH CHECK ADD  CONSTRAINT [FK_PromptPersonas_Prompt] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptPersonas_Prompt]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptPersonas]'))
ALTER TABLE [dbo].[PromptPersonas] CHECK CONSTRAINT [FK_PromptPersonas_Prompt]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptUtterances_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptUtterances]'))
ALTER TABLE [dbo].[PromptUtterances]  WITH CHECK ADD  CONSTRAINT [FK_PromptUtterances_PromptID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PromptUtterances_PromptID]') AND parent_object_id = OBJECT_ID(N'[dbo].[PromptUtterances]'))
ALTER TABLE [dbo].[PromptUtterances] CHECK CONSTRAINT [FK_PromptUtterances_PromptID]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Tool_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Tool]'))
ALTER TABLE [dbo].[Tool]  WITH CHECK ADD  CONSTRAINT [FK_Tool_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Tool_TenantID]') AND parent_object_id = OBJECT_ID(N'[dbo].[Tool]'))
ALTER TABLE [dbo].[Tool] CHECK CONSTRAINT [FK_Tool_TenantID]
GO
