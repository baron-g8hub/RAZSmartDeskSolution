CREATE TABLE [dbo].[UserTypes] (
    [UserTypeId]    INT           IDENTITY (1, 1) NOT NULL,
    [UserTypeName]  NVARCHAR (10) NOT NULL,
    [UserTypeLevel] INT           DEFAULT ((0)) NULL,
    CONSTRAINT [PK_UserTypes] PRIMARY KEY CLUSTERED ([UserTypeId] ASC)
);

