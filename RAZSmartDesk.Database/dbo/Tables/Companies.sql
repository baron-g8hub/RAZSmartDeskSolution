CREATE TABLE [dbo].[Companies] (
    [CompanyId]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyName]        NVARCHAR (50)  NOT NULL,
    [CompanyCode]        NVARCHAR (20)  NOT NULL,
    [CompanyDescription] NVARCHAR (100) NULL,
    [ExtraString1]       NVARCHAR (50)  NULL,
    [ExtraString2]       NVARCHAR (50)  NULL,
    [ExtraInt1]          INT            NULL,
    [ExtraInt2]          INT            NULL,
    [IsActive]           BIT            DEFAULT ((1)) NULL,
    [CreatedBy]          NVARCHAR (20)  NULL,
    [CreatedDate]        DATETIME       DEFAULT (getutcdate()) NULL,
    [UpdatedBy]          NVARCHAR (20)  NULL,
    [UpdatedDate]        DATETIME       DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([CompanyId] ASC)
);

