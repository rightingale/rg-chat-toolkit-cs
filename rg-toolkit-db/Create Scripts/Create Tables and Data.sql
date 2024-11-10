USE [RG-Toolkit]
GO
/****** Object:  Table [dbo].[AccessKey]    Script Date: 11/10/2024 3:41:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[ContentType]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[Filter]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[Object]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[Prompt]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
 CONSTRAINT [PK_Prompt_ID] PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromptFilters]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[PromptObjects]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[PromptTools]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[Tenant]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  Table [dbo].[Tool]    Script Date: 11/10/2024 3:41:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
INSERT [dbo].[AccessKey] ([TenantID], [ID], [KeyValue], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'f017eca4-bcc4-4e04-abeb-464c8792a467', N'9ca151e3-d544-4b85-b44b-bbe055220808', 1, CAST(N'2024-11-02T23:58:04.907' AS DateTime), CAST(N'2024-11-02T23:58:04.907' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'813d2f35-a099-ef11-aced-021fe1d77a3b', N'application/json', 0, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'8e3d2f35-a099-ef11-aced-021fe1d77a3b', N'application/octet-stream', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'833d2f35-a099-ef11-aced-021fe1d77a3b', N'application/pdf', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'823d2f35-a099-ef11-aced-021fe1d77a3b', N'application/xml', 0, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'883d2f35-a099-ef11-aced-021fe1d77a3b', N'audio/mpeg', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'893d2f35-a099-ef11-aced-021fe1d77a3b', N'audio/ogg', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'8a3d2f35-a099-ef11-aced-021fe1d77a3b', N'audio/wav', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'863d2f35-a099-ef11-aced-021fe1d77a3b', N'image/gif', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'853d2f35-a099-ef11-aced-021fe1d77a3b', N'image/jpeg', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'843d2f35-a099-ef11-aced-021fe1d77a3b', N'image/png', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'873d2f35-a099-ef11-aced-021fe1d77a3b', N'image/svg+xml', 0, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'803d2f35-a099-ef11-aced-021fe1d77a3b', N'text/html', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'7f3d2f35-a099-ef11-aced-021fe1d77a3b', N'text/plain', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'8b3d2f35-a099-ef11-aced-021fe1d77a3b', N'video/mp4', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'8c3d2f35-a099-ef11-aced-021fe1d77a3b', N'video/ogg', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[ContentType] ([ID], [Name], [AllowStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'8d3d2f35-a099-ef11-aced-021fe1d77a3b', N'video/webm', 1, 1, CAST(N'2024-11-02T23:58:04.800' AS DateTime), CAST(N'2024-11-02T23:58:04.800' AS DateTime))
GO
INSERT [dbo].[Filter] ([TenantID], [ID], [Name], [Description], [Assembly], [Type], [Method], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'ce321dab-f000-49e5-84b4-ce66925de3bc', N'hap_default', N'Lightweight default HAP filter.', NULL, NULL, NULL, 1, CAST(N'2024-11-02T23:58:04.910' AS DateTime), CAST(N'2024-11-02T23:58:04.910' AS DateTime))
GO
INSERT [dbo].[Object] ([TenantID], [ID], [Name], [ContentTypeName], [ObjectSchema], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'34ff30ad-f0c2-4a2e-9643-c01191057c93', N'input_message', N'text/plain', NULL, 1, CAST(N'2024-11-02T23:58:04.900' AS DateTime), CAST(N'2024-11-02T23:58:04.900' AS DateTime))
GO
INSERT [dbo].[Prompt] ([TenantID], [ID], [Name], [SystemPrompt], [ReponseContentTypeName], [DoStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'57111519-5332-4315-b49c-4bbe76297148', N'demo_greeter_weather', N'You are a helpful greeter. Lookup current weather conditions for a random major city and return it with your greeting.
Please re-format the weather as plain text. Include the name of the nearby city.', N'text/plain', 1, 1, CAST(N'2024-11-02T23:58:04.890' AS DateTime), CAST(N'2024-11-02T23:58:04.890' AS DateTime))
GO
INSERT [dbo].[Prompt] ([TenantID], [ID], [Name], [SystemPrompt], [ReponseContentTypeName], [DoStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'a2bae879-77ba-40ee-b1c5-5cf21ff16453', N'demo_greeter', N'You are a helpful greeter.', N'text/plain', NULL, 1, CAST(N'2024-11-02T23:58:04.890' AS DateTime), CAST(N'2024-11-02T23:58:04.890' AS DateTime))
GO
INSERT [dbo].[Prompt] ([TenantID], [ID], [Name], [SystemPrompt], [ReponseContentTypeName], [DoStreamResponse], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'787923ab-0d9f-ef11-aced-021fe1d77a3b', N'3efddfbc-0ed2-43d7-9232-5461b8709762', N'instore_experience_helper', N'You are an expert customer service agent for Big Star. You dont know everything, so dont make inferences. Stick to the GROCERY ITEMS information, if provided. You helpfully guide the user ("customer") through the store.  You can help find items, and lookup complementary items.  You have a neighborly, warm tone. Keep it professional. You may include emoji where it would help. Respond only in plain text. Do not use lists; neither markdown nor other formatting. Limit your responses to 1-3 items.', N'text/plain', NULL, 1, CAST(N'2024-11-09T20:54:46.663' AS DateTime), CAST(N'2024-11-09T20:54:46.663' AS DateTime))
GO
INSERT [dbo].[PromptFilters] ([TenantID], [ID], [PromptID], [FilterID], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'903d2f35-a099-ef11-aced-021fe1d77a3b', N'57111519-5332-4315-b49c-4bbe76297148', N'ce321dab-f000-49e5-84b4-ce66925de3bc', 1, CAST(N'2024-11-02T23:58:04.913' AS DateTime), CAST(N'2024-11-02T23:58:04.913' AS DateTime))
GO
INSERT [dbo].[PromptObjects] ([TenantID], [ID], [PromptID], [ObjectID], [IsInput], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'c4d25d31-1220-4860-a159-f81c5f741ade', N'a2bae879-77ba-40ee-b1c5-5cf21ff16453', N'34ff30ad-f0c2-4a2e-9643-c01191057c93', 1, 1, CAST(N'2024-11-02T23:58:04.900' AS DateTime), CAST(N'2024-11-02T23:58:04.900' AS DateTime))
GO
INSERT [dbo].[PromptTools] ([TenantID], [ID], [PromptID], [ToolID], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'8f3d2f35-a099-ef11-aced-021fe1d77a3b', N'a2bae879-77ba-40ee-b1c5-5cf21ff16453', N'9642f94f-d0b0-4cb4-9555-d3d76a77bc4c', 1, CAST(N'2024-11-02T23:58:04.907' AS DateTime), CAST(N'2024-11-02T23:58:04.907' AS DateTime))
GO
INSERT [dbo].[PromptTools] ([TenantID], [ID], [PromptID], [ToolID], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'787923ab-0d9f-ef11-aced-021fe1d77a3b', N'40cbdbcb-109f-ef11-aced-021fe1d77a3b', N'3efddfbc-0ed2-43d7-9232-5461b8709762', N'fa84bbdf-a4b2-4ae3-82f4-6d387c258633', 1, CAST(N'2024-11-09T21:06:37.023' AS DateTime), CAST(N'2024-11-09T21:06:37.023' AS DateTime))
GO
INSERT [dbo].[Tenant] ([ID], [Name], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'System', 1, CAST(N'2024-11-02T23:58:04.887' AS DateTime), CAST(N'2024-11-02T23:58:04.887' AS DateTime))
GO
INSERT [dbo].[Tenant] ([ID], [Name], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'787923ab-0d9f-ef11-aced-021fe1d77a3b', N'Bigstar', 1, CAST(N'2024-11-09T20:44:13.637' AS DateTime), CAST(N'2024-11-09T20:44:13.637' AS DateTime))
GO
INSERT [dbo].[Tool] ([TenantID], [ID], [Name], [Description], [Parameters], [Assembly], [Type], [Method], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'00000000-0000-0000-0000-000000000000', N'9642f94f-d0b0-4cb4-9555-d3d76a77bc4c', N'get_greeting', N'Gets the next greeting', N'{"Type": "object", "Properties": {"message": {"Type": "string"}}, "Required": ["message"]}', NULL, N'DefaultToolHelper', N'GetGreeting', 1, CAST(N'2024-11-02T23:58:04.903' AS DateTime), CAST(N'2024-11-02T23:58:04.903' AS DateTime))
GO
INSERT [dbo].[Tool] ([TenantID], [ID], [Name], [Description], [Parameters], [Assembly], [Type], [Method], [IsActive], [CreateDate], [LastUpdate]) VALUES (N'787923ab-0d9f-ef11-aced-021fe1d77a3b', N'fa84bbdf-a4b2-4ae3-82f4-6d387c258633', N'find_grocery_items', N'', N'', N'rg-integrations-epic', N'rg.integrations.epic.Tools.Memory.FindGroceryItemVectorStoreMemory', N'', 1, CAST(N'2024-11-09T21:06:06.440' AS DateTime), CAST(N'2024-11-09T21:06:06.440' AS DateTime))
GO
/****** Object:  Index [IX_AccessKey]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AccessKey] ON [dbo].[AccessKey]
(
	[TenantID] ASC,
	[KeyValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_AccessKey_ID]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_AccessKey_ID] ON [dbo].[AccessKey]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ContentType]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ContentType] ON [dbo].[ContentType]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Filter]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Filter] ON [dbo].[Filter]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Object]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Object] ON [dbo].[Object]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Object_ID]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Object_ID] ON [dbo].[Object]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Prompt]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Prompt] ON [dbo].[Prompt]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Prompt_ID]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Prompt_ID] ON [dbo].[Prompt]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptFilters]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptFilters] ON [dbo].[PromptFilters]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[FilterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptObjects]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptObjects] ON [dbo].[PromptObjects]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_PromptObjects_ID]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_PromptObjects_ID] ON [dbo].[PromptObjects]
(
	[ID] ASC
)
INCLUDE([IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PromptTools]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PromptTools] ON [dbo].[PromptTools]
(
	[TenantID] ASC,
	[PromptID] ASC,
	[ToolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenant]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenant] ON [dbo].[Tenant]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tool]    Script Date: 11/10/2024 3:41:46 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tool] ON [dbo].[Tool]
(
	[TenantID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (newid()) FOR [KeyValue]
GO
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[AccessKey] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT ((0)) FOR [AllowStreamResponse]
GO
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[ContentType] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[Filter] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Filter] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Object] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Object] ADD  DEFAULT ('text/plain') FOR [ContentTypeName]
GO
ALTER TABLE [dbo].[Object] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Object] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Object] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT ('text/plain') FOR [ReponseContentTypeName]
GO
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Prompt] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[PromptFilters] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT ((0)) FOR [IsInput]
GO
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[PromptObjects] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[PromptTools] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Tenant] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (newsequentialid()) FOR [ID]
GO
ALTER TABLE [dbo].[Tool] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Tool] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[AccessKey]  WITH CHECK ADD  CONSTRAINT [FK_AccessKey_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[AccessKey] CHECK CONSTRAINT [FK_AccessKey_TenantID]
GO
ALTER TABLE [dbo].[Filter]  WITH CHECK ADD  CONSTRAINT [FK_Filter_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[Filter] CHECK CONSTRAINT [FK_Filter_TenantID]
GO
ALTER TABLE [dbo].[Object]  WITH CHECK ADD  CONSTRAINT [FK_Object_ContentTypeName] FOREIGN KEY([ContentTypeName])
REFERENCES [dbo].[ContentType] ([Name])
GO
ALTER TABLE [dbo].[Object] CHECK CONSTRAINT [FK_Object_ContentTypeName]
GO
ALTER TABLE [dbo].[Object]  WITH CHECK ADD  CONSTRAINT [FK_Object_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[Object] CHECK CONSTRAINT [FK_Object_TenantID]
GO
ALTER TABLE [dbo].[Prompt]  WITH CHECK ADD  CONSTRAINT [FK_Prompt_ContentTypeName] FOREIGN KEY([ReponseContentTypeName])
REFERENCES [dbo].[ContentType] ([Name])
GO
ALTER TABLE [dbo].[Prompt] CHECK CONSTRAINT [FK_Prompt_ContentTypeName]
GO
ALTER TABLE [dbo].[Prompt]  WITH CHECK ADD  CONSTRAINT [FK_Prompt_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[Prompt] CHECK CONSTRAINT [FK_Prompt_TenantID]
GO
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_FilterID] FOREIGN KEY([TenantID], [FilterID])
REFERENCES [dbo].[Filter] ([TenantID], [ID])
GO
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_FilterID]
GO
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_PromptID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_PromptID]
GO
ALTER TABLE [dbo].[PromptFilters]  WITH CHECK ADD  CONSTRAINT [FK_PromptFilters_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[PromptFilters] CHECK CONSTRAINT [FK_PromptFilters_TenantID]
GO
ALTER TABLE [dbo].[PromptObjects]  WITH CHECK ADD  CONSTRAINT [FK_PromptObjects_ObjectID_TenantID] FOREIGN KEY([TenantID], [ObjectID])
REFERENCES [dbo].[Object] ([TenantID], [ID])
GO
ALTER TABLE [dbo].[PromptObjects] CHECK CONSTRAINT [FK_PromptObjects_ObjectID_TenantID]
GO
ALTER TABLE [dbo].[PromptObjects]  WITH CHECK ADD  CONSTRAINT [FK_PromptObjects_PromptID_TenantID] FOREIGN KEY([TenantID], [PromptID])
REFERENCES [dbo].[Prompt] ([TenantID], [ID])
GO
ALTER TABLE [dbo].[PromptObjects] CHECK CONSTRAINT [FK_PromptObjects_PromptID_TenantID]
GO
ALTER TABLE [dbo].[Tool]  WITH CHECK ADD  CONSTRAINT [FK_Tool_TenantID] FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([ID])
GO
ALTER TABLE [dbo].[Tool] CHECK CONSTRAINT [FK_Tool_TenantID]
GO
