CREATE PROCEDURE [dbo].[DeleteGuests]
(
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (UserId int)
		
	INSERT #tmp_guests (UserId)
	SELECT [Id] FROM [User] u with (NOLOCK)
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (u.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (u.[CreatedOnUtc] < @CreatedToUtc))
	AND
	--guests only
	(EXISTS(SELECT 1 FROM [UserRole] ur with (NOLOCK) inner join [User] with (NOLOCK) on ur.[User_Id]=u.[Id] inner join [Role] r with (NOLOCK) on r.[Id]=ur.[Role_Id] WHERE r.[SystemName] = N'Cashier'))
	AND
	--no system accounts
	(u.IsSystemAccount = 0)
	
	--delete guests
	DELETE [User]
	WHERE [Id] IN (SELECT [UserId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [UserId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'User')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO