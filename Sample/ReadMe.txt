DB scripts:

Databasename Common:


CREATE TABLE [dbo].[Contacts](    
       [ContactId] [int] IDENTITY(1,1) NOT NULL,      
       [FirstName] [nvarchar](50) NOT NULL,    
       [LastName] [nvarchar](50) NOT NULL,    
       [MobileNumber] [nvarchar](12) NOT NULL,      
    
 CONSTRAINT [PK_CallDetail] PRIMARY KEY CLUSTERED    
(    
       [ContactId] ASC    
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]    
) ON [PRIMARY]   
GO 


CREATE TABLE [dbo].[ContactGroups](    
    [ContactGroupId] [int] IDENTITY(1,1) NOT NULL,    
    [ContactId] [int] NOT NULL,       
    [GroupName] [nvarchar](max) NULL,    
	[ContactMapId] [int] NOT NULL,  
 CONSTRAINT [PK_CallTransactionDetail] PRIMARY KEY CLUSTERED     
(    
    [ContactGroupId] ASC    
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]    
) ON [PRIMARY] 
GO 

ALTER TABLE [dbo].[ContactGroups]  WITH CHECK ADD  CONSTRAINT [FK_Contgroup_Contact] FOREIGN KEY([ContactId])    
REFERENCES [dbo].[Contacts] ([ContactId])    ON DELETE CASCADE
GO    
    
ALTER TABLE [dbo].[ContactGroups] CHECK CONSTRAINT [FK_Contgroup_Contact]    
GO 


----------------------------------------------------------------------------------------------------------------------------------------------


I have created webapi using .NET Core -Visula studio 2019

configure cinnection in startupclass:

 services.AddDbContextPool<CommonContext>(options => options.UseSqlServer("Server=your server;Database=Common;Trusted_Connection=True;"));


Below are api methods:

1)https://localhost:44304/weatherforecast/GetContactDetails

2)https://localhost:44304/weatherforecast/PostContactGroupDetail
(This method created using primary and foreign key reltion,if contact has been added automatically contactgrp will be created)
Payload:

{
       
        "FirstName": "Pradeep",
        "LastName": "Venkat",
        "MobileNumber": "80504568"
    
}

3)https://localhost:44304/weatherforecast/UpdateContacts

Payload:

{
       "ContactId":4,
        "FirstName": "Pradeep",
        "LastName": "Venkat",
        "MobileNumber": "800004568"
    
}

4)https://localhost:44304/weatherforecast/DeleteContacts(id=4)
(This method created using cascade delete functionality,if contact has been deleted automatically contactgrp will be deleted)

5)https://localhost:44304/weatherforecast/SearchContact

Payload:

{
       "pageNumber":1,
        "pageSize": 7,
        "QuerySearch": "Venkat"
    
}








  
