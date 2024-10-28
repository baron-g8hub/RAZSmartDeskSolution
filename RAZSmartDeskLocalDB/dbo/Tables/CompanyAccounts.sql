CREATE TABLE [dbo].[CompanyAccounts] (
    [CompanyAccountId]          INT           IDENTITY (1, 1) NOT NULL,
    [CompanyId]                 INT           NOT NULL,
    [AccountCodeId]             INT           NOT NULL,
    [CompanyAccountDescription] NVARCHAR (50) NOT NULL,
    [IsActive]                  BIT           DEFAULT ((1)) NULL,
    [CreatedBy]                 NVARCHAR (20) DEFAULT (getutcdate()) NULL,
    [CreatedDate]               DATETIME      NULL,
    [UpdatedBy]                 NVARCHAR (20) NULL,
    [UpdatedDate]               DATETIME      DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_CompanyAccounts] PRIMARY KEY CLUSTERED ([CompanyAccountId] ASC),
    CONSTRAINT [FK_CompanyAccounts_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);

