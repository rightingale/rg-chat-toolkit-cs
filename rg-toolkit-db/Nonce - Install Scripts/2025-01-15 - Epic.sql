use [RG-Toolkit]
go

-- Tenant BS = "787923ab-0d9f-ef11-aced-021fe1d77a3b"
-- Prompt "instore_experience_helper" = "3EFDDFBC-0ED2-43D7-9232-5461B8709762"
-- Memory "find_grocery_items" = "c55525f4-c624-41e1-8320-a77fe326d88c"


insert into Memory (TenantID, ID, Name, Description, DescriptionEmbedding, MemoryType, Is_Active)
values ('787923ab-0d9f-ef11-aced-021fe1d77a3b', 'c55525f4-c624-41e1-8320-a77fe326d88c', 'find_grocery_items', 'Find grocery items', 'rg.integrations.epic.Tools.Memory.FindGroceryItemVectorStoreMemory', 1, 1)


insert into PromptMemories (TenantID, ID, PromptID, MemoryID, Ordinal, Is_Active)
values ('787923ab-0d9f-ef11-aced-021fe1d77a3b', newid(), '3EFDDFBC-0ED2-43D7-9232-5461B8709762', 'c55525f4-c624-41e1-8320-a77fe326d88c', 0, 1)


--.
