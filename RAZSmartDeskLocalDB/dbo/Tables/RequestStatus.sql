CREATE TABLE [dbo].[RequestStatus] (
    [RequestStatusId]   INT           NOT NULL,
    [RequestStatusName] NVARCHAR (20) NULL,
    CONSTRAINT [PK_RequestStatus] PRIMARY KEY CLUSTERED ([RequestStatusId] ASC)
);

