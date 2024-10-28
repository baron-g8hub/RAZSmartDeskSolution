CREATE TABLE [dbo].[CompanyEmployees] (
    [CompanyEmployeeId] INT           IDENTITY (1, 1) NOT NULL,
    [CompanyId]         INT           NULL,
    [Firstname]         NVARCHAR (20) NULL,
    [Lastname]          NVARCHAR (20) NULL,
    [Address]           NVARCHAR (30) NULL,
    [HiredDate]         DATETIME      NULL,
    [CompanyPosition]   NCHAR (15)    NULL,
    [IsActive]          BIT           DEFAULT ((1)) NULL,
    [CreatedBy]         NVARCHAR (20) NULL,
    [CreatedDate]       DATETIME      DEFAULT (getutcdate()) NULL,
    [UpdatedBy]         NVARCHAR (20) NULL,
    [UpdatedDate]       DATETIME      DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_CompanyEmployees] PRIMARY KEY CLUSTERED ([CompanyEmployeeId] ASC)
);

