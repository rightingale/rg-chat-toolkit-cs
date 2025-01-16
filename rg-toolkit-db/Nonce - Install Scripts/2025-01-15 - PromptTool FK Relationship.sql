use [RG-Toolkit]
go

-- Define Relationship Prompt -> PromptTools -> Tool

alter table PromptTools
    add constraint FK_PromptTools_PromptID foreign key (TenantID, PromptID) references Prompt (TenantID, ID)

alter table PromptTools
    add constraint FK_PromptTools_ToolID foreign key (TenantID, ToolID) references Tool (TenantID, ID)
