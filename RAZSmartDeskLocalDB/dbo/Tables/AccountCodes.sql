CREATE TABLE [dbo].[AccountCodes] (
    [AccountCodeId] INT           IDENTITY (1, 1) NOT NULL,
    [AccountCode]   NVARCHAR (50) NOT NULL,
    [RegionId]      INT           NOT NULL,
    CONSTRAINT [PK_AccountCodes] PRIMARY KEY CLUSTERED ([AccountCodeId] ASC)
);

