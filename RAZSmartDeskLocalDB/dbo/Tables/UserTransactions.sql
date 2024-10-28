CREATE TABLE [dbo].[UserTransactions] (
    [UserTransactionId]  INT           IDENTITY (1, 1) NOT NULL,
    [UserId]             INT           NOT NULL,
    [CompanyId]          INT           NOT NULL,
    [TransactionName]    NVARCHAR (10) NOT NULL,
    [TransactionTypeId]  INT           NOT NULL,
    [TransactionDetails] NVARCHAR (50) NULL,
    [TransactionDate]    DATETIME      DEFAULT (getutcdate()) NULL,
    [StatusRemarks]      NVARCHAR (20) NULL,
    CONSTRAINT [PK_UserTransactions] PRIMARY KEY CLUSTERED ([UserTransactionId] ASC),
    CONSTRAINT [FK_UserTransactions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

