use [RG-Toolkit]
go

-- Create Tenant Tilley 902544da-67e6-4fa8-a346-d1faa8b27a08

insert into Tenant (ID, Name, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', 'Tilley', getdate(), getdate())

-- Create prompt tilley_chat 31dc0b6b-2b7a-499f-976a-71c9eaca7bc5

declare @systemPrompt nvarchar(max) = 'You are Tilley, a Farm Financial Management expert.
You can only help me with the following:
- Navigating the site ("Go to..." or "Take me to...")
- Asking questions about the site ("What can I do here?" or "Help")
Stay on topic. Do not allow off-topic questions. If you do not know the answer, you can say "I do not know" or "I am not sure".
Return results in JSON format using this schema:
{
    "section": "tilley",
    "module": "module name",
    "object": "object name",
    "container": "container",
    "item": "item name"
}
Off-topic requests shall return this value:
    {section: null, module: null, object: null, container: null, item: null}
JSON Rules:
- "section" is always "tilley".
- "module" is a required field (except NULL for off-topic requests).
- "container" is nullable.
- "item" is nullable.
- Do not wrap the json codes in JSON markers.
- Do not substitute conjecture for missing props.
- Use only the props provided in the MEMORY DATA.

Use the following MEMORY DATA to answer questions:
{section: "tilley", module: "vault", object: "Farm", container: null, item: null}
{section: "tilley", module: "arc_plc", object: "base_acreage", container: null, item: null}
{section: "tilley", module: "arc_plc", object: "projections", container: null, item: null}
{section: "tilley", module: "insurance", object: "coverage", container: null, item: null}
{section: "tilley", module: "insurance", object: "sco_arc_comparison", container: null, item: null}
{section: "tilley", module: "reports", object: "FarmTractFieldReport", container: null, item: null}
{section: "tilley", module: "reports", object: "AcresMapReport", container: null, item: null}

'

-- insert into Prompt (TenantID, ID, Name, SystemPrompt, CreateDate, LastUpdate)
-- values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'tilley_navigation', @systemPrompt, getdate(), getdate())

update Prompt
set SystemPrompt = @systemPrompt, ReponseContentTypeName = 'application/json'
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' and ID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'


delete from PromptUtterances where PromptID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'

-- PromptUtterances (5)
insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'go to the page')

insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'take me to a certain page')

insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'navigate to a certain page')

insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'show me a certain page')

insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'go to a certain page')

insert into PromptUtterances (TenantID, PromptID, Utterance)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'go to a certain page where I can perform a certain action')


-- ---

select * from Tenant where Name = 'Tilley'

select * from Prompt where Name = 'tilley_navigation'

select * from PromptUtterances where PromptID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'

-- --- ---

-- ---
-- Create memory d6566df7-d61b-42dd-abe7-e121af01c2e6

insert into Memory (TenantID, ID, Name, Description, MemoryType, Is_Active, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', 'd6566df7-d61b-42dd-abe7-e121af01c2e6', 'tilley_navigation', 'Tilley navigation items', 'vector/in-memory', 1, getdate(), getdate())

insert into PromptMemories (TenantID, ID, PromptID, MemoryID, Ordinal, Is_Active, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', newID(), '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5', 'd6566df7-d61b-42dd-abe7-e121af01c2e6', 0, 1, getdate(), getdate())

-- ---
-- Create Memory "ProducerFarms" = 2e42961e-c271-4570-818d-5ff2d76bb461

insert into Memory (TenantID, ID, Name, Description, MemoryType, Is_Active, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', '2e42961e-c271-4570-818d-5ff2d76bb461', 'ProducerFarms', 'Producer Farms', 'vector', 1, getdate(), getdate())



-- --- ---

select * from Memory where Name = 'tilley_navigation'

select * from PromptMemories where PromptID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'

select SystemPrompt
from Prompt where ID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'

update Prompt set SystemPrompt = 'You are Tilley, a Farm Financial Management expert. You can only help me with the following: 
- Navigating the site ("Go to..." or "Take me to...") 
- Asking questions about the site ("What can I do here?" or "Help")

Stay on topic. Do not allow off-topic questions. If you do not know the answer, you can say "I do not know" or "I am not sure". 

Return results in JSON format using this schema: 
{"section": "tilley", "module": "module name", "object": "object name", "container": "container", "item": "item name"}
Off-topic requests shall return this value:
{section: null, module: null, object: null, container: null, item: null} 

JSON Rules: 
- "section" is always "tilley". 
- "module" is a required field (except NULL for off-topic requests). 
- "container" is nullable. 
- "item" is nullable. 
- Do not wrap the json codes in JSON markers. 
- Do not substitute conjecture for missing props. 
- Use only the props provided in the MEMORY DATA.  

Use the following MEMORY DATA to answer questions: 
{section: "tilley", module: "vault", object: "Farm", container: null, item: null} 
{section: "tilley", module: "arc_plc", object: "base_acreage", container: null, item: null} 
{section: "tilley", module: "arc_plc", object: "projections", container: null, item: null} 
{section: "tilley", module: "insurance", object: "coverage", container: null, item: null} 
{section: "tilley", module: "insurance", object: "sco_arc_comparison", container: null, item: null} 
{section: "tilley", module: "reports", object: "FarmTractFieldReport", container: null, item: null} 
{section: "tilley", module: "reports", object: "AcresMapReport", container: null, item: null}
' 
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' and ID = '31dc0b6b-2b7a-499f-976a-71c9eaca7bc5'


-- --- --- ---
-- Create Prompt "financials_question" dc17ef0a-559f-4307-b750-564ffac3e648

insert into Prompt (TenantID, ID, Name, SystemPrompt, CreateDate, LastUpdate)
values ('902544da-67e6-4fa8-a346-d1faa8b27a08', 'dc17ef0a-559f-4307-b750-564ffac3e648', 'financials_question', 'Ask questions, analyze, or report on Financial Documents including Balance Sheet, Income Statement, Schedule F tax form.', getdate(), getdate())

-- Create prompt "tilley"

update Prompt set SystemPrompt = 'You are Tilley, a Farm Financial Management expert.  
You can only help me with the following:  
- Ask questions, analyze, or report on Financial Documents including:
    - Balance Sheet
    - Income Statement
    - Schedule F tax forms
Stay on topic. Do not allow off-topic questions. If you do not know the answer, you can say "I do not know" or "I am not sure".  
Return with plain text. Do NOT format as markdown.' 
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' and ID = 'dc17ef0a-559f-4307-b750-564ffac3e648'


-- --- ---

update Prompt set Description = 'Navigate, go to, or show me modules including Farm Vault, Budget, ARC/PLC, Insurance, Financials, Marketing' 
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' and Name = 'tilley_navigation'

update Prompt set Description = 'Ask questions, analyze, or report on Financial Documents including Balance Sheet, Income Statement, Schedule F tax form.' 
where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' and Name = 'financials_question'




select * from Prompt where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' 



select * from PromptUtterances where TenantID = '902544da-67e6-4fa8-a346-d1faa8b27a08' 