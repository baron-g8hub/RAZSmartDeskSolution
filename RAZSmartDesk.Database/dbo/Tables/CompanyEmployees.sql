CREATE TABLE [dbo].[CompanyEmployees]
(
	CompanyEmployeeId INT NOT NULL  IDENTITY, 
    CompanyId          INT            NULL,
    [Firstname] NCHAR(50) NULL, 
    [Lastname] NCHAR(50) NULL, 
    [Address] NCHAR(10) NULL, 

    [HiredDate] DATETIME NULL, 
    CompanyPosition NCHAR(50) NULL, 
     [IsActive]           BIT            DEFAULT ((1)) NULL,
    [CreatedBy]          NVARCHAR (20)  NULL,
    [CreatedDate]        DATETIME       DEFAULT (getutcdate()) NULL,
    [UpdatedBy]          NVARCHAR (20)  NULL,
    [UpdatedDate]        DATETIME       DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_CompanyEmployees] PRIMARY KEY ([CompanyEmployeeId])
)
