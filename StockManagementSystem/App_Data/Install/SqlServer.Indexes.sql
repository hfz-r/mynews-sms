CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [User] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_UserName] ON [User] ([UserName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_UserGuid] ON [User] ([UserGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_CreatedOnUtc] ON [User] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_Role_SystemName] ON [Role] ([SystemName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UserRole_UserId] ON [UserRole] ([User_Id] ASC)
GO
