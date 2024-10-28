CREATE TABLE [dbo].[UserRequests] (
    [UserRequestId]      INT           IDENTITY (1, 1) NOT NULL,
    [RequesterId]        INT           NOT NULL,
    [ApproverId]         INT           NOT NULL,
    [RequestTypeId]      INT           NOT NULL,
    [RequestDescription] NVARCHAR (20) NULL,
    [RequestDate]        DATETIME      DEFAULT (getutcdate()) NULL,
    [ApprovedDate]       DATETIME      NULL,
    [RequestStatusId]    INT           NOT NULL,
    CONSTRAINT [PK_UserRequests] PRIMARY KEY CLUSTERED ([UserRequestId] ASC),
    CONSTRAINT [FK_UserRequests_RequestStatus] FOREIGN KEY ([RequestStatusId]) REFERENCES [dbo].[RequestStatus] ([RequestStatusId]),
    CONSTRAINT [FK_UserRequests_RequestTypes] FOREIGN KEY ([RequestTypeId]) REFERENCES [dbo].[RequestTypes] ([RequestTypeId])
);

