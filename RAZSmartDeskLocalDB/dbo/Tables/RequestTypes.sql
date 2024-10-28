CREATE TABLE [dbo].[RequestTypes] (
    [RequestTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [RequestTypeName] NVARCHAR (20) NULL,
    CONSTRAINT [PK_RequestTypes] PRIMARY KEY CLUSTERED ([RequestTypeId] ASC)
);

