CREATE TABLE [dbo].[Users] (
    [UserId]        INT           IDENTITY (1, 1) NOT NULL,
    [UserCompanyId] INT           NOT NULL,
    [UserTypeId]    INT           NOT NULL,
    [Username]      NVARCHAR (30) NOT NULL,
    [Password]      NVARCHAR (20) NOT NULL,
    [IsActive]      BIT           DEFAULT ((1)) NULL,
    [CreatedBy]     NVARCHAR (20) NULL,
    [CreatedDate]   DATETIME      DEFAULT (getutcdate()) NULL,
    [UpdatedBy]     NVARCHAR (20) NULL,
    [UpdatedDate]   DATETIME      DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Users_Companies] FOREIGN KEY ([UserCompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Users_UserTypes] FOREIGN KEY ([UserTypeId]) REFERENCES [dbo].[UserTypes] ([UserTypeId])
);

