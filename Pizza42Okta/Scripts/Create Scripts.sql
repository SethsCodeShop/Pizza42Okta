IF OBJECT_ID('dbo.[Orders]', 'U') IS NOT NULL
begin
    drop table [Orders] 
end
CREATE TABLE [Orders] (
    [PizzaOrderId] int NOT NULL primary key identity(1,1),
    [UserId] varchar(max) Not null,
    [TypeId] INTEGER NOT NULL,
	[Created] Datetime not null default GetUTCDate()
);
GO
IF OBJECT_ID('dbo.[Types]', 'U') IS NOT NULL
begin
    drop table [Types]
end
CREATE TABLE [Types] (
    [Id] INTEGER NOT NULL identity(1,1),
    [Name] varchar(max) not NULL,
    CONSTRAINT [PK_Types] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_Orders_TypeId] ON [Orders] ([TypeId]);
GO

ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Types_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Types] ([Id]) ON DELETE NO ACTION;
GO
