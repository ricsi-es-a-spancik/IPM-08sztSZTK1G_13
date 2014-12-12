IF DB_ID('ELTE') IS NOT NULL
	DROP DATABASE [ELTE];
GO

CREATE DATABASE [ELTE];
GO
USE [ELTE];
GO

CREATE TABLE [User](
	[Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[UserName] VARCHAR(30) NOT NULL,
	[Password] BINARY(32) NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	[Email] VARCHAR(80) NOT NULL,
	[RegisterDate] DATETIME NOT NULL,
	[LastLogin] DATETIME
);
GO

CREATE TABLE [Message](
	[Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[FromId] INTEGER NOT NULL,
	[ToId] INTEGER NOT NULL,
	[Subject] VARCHAR(50) NOT NULL,
	[Content] VARCHAR(max) NOT NULL,
	[IsRead] BIT NOT NULL DEFAULT 0,
	[HideFromSender] BIT NOT NULL DEFAULT 0,
	[HideFromTarget] BIT NOT NULL DEFAULT 0
	CONSTRAINT FK_Message_From
        FOREIGN KEY ([FromId]) 
        REFERENCES [User] ([Id])
);
GO

CREATE TABLE [UserImage] (
	[Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[UserId] INTEGER NOT NULL,
	[Image] VARBINARY(max) NOT NULL
	CONSTRAINT FK_Image_User
        FOREIGN KEY ([UserId]) 
        REFERENCES [User] ([Id])
);
GO

CREATE TABLE [Organization] (
    [Id] INTEGER PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(50) NOT NULL,
	[FoundationYear] INTEGER,
	[Country] VARCHAR(50),
	[City] VARCHAR(50),
	[Activity] VARCHAR(100),
	[Description] VARCHAR(max)
);
GO

CREATE TABLE [CoverImage] (
	[Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[OrganizationId] INTEGER NOT NULL,
	[Image] VARBINARY(max) NOT NULL
	CONSTRAINT FK_CoverImage_Organization
		FOREIGN KEY ([OrganizationId])
		REFERENCES [Organization] ([Id]),
);
GO

CREATE TABLE [Employee] (
	[UserId] INTEGER NOT NULL,
	[OrganizationId] INTEGER NOT NULL,
	[Status] SMALLINT NOT NULL
	PRIMARY KEY ([UserId],[OrganizationId])
	CONSTRAINT FK_Employee_User
		FOREIGN KEY ([UserId])
		REFERENCES [User] ([Id]),
	CONSTRAINT FK_Employee_Organization
		FOREIGN KEY ([OrganizationId])
		REFERENCES [Organization] ([Id])
);
GO

CREATE TABLE [Project] (
    [Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[OrganizationId] INTEGER NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
	[Description] VARCHAR(max),
	[Deadline] DATE
	CONSTRAINT FK_Project_Organization
		FOREIGN KEY ([OrganizationId])
		REFERENCES [Organization] ([Id])
);
GO

CREATE TABLE [Document] (
    [Id] INTEGER PRIMARY KEY IDENTITY(1,1),
	[ProjectId] INTEGER NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
	[Content] VARCHAR(max) NOT NULL,
	[Modified] DATE NOT NULL
	CONSTRAINT FK_Document_Project
		FOREIGN KEY ([ProjectId])
		REFERENCES [Project] ([Id])
);
GO

CREATE TABLE [ProjectMember] (
	[UserId] INTEGER NOT NULL,
	[ProjectId] INTEGER NOT NULL,
	[Status] SMALLINT NOT NULL
	PRIMARY KEY ([UserId],[ProjectId])
	CONSTRAINT FK_ProjectMember_User
		FOREIGN KEY ([UserId])
		REFERENCES [User] ([Id]),
	CONSTRAINT FK_ProjectMember_Project
		FOREIGN KEY ([ProjectId])
		REFERENCES [Project] ([Id])
);
GO

CREATE TABLE [Issue] (
    [Id] INTEGER PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(max) NOT NULL,
	[Type] SMALLINT NOT NULL,
	[Status] SMALLINT NOT NULL DEFAULT 1,
	[Deadline] DATE,
	[UserId] INTEGER NOT NULL,
	[ProjectId] INTEGER NOT NULL
	CONSTRAINT FK_Issue_User
		FOREIGN KEY ([UserId])
		REFERENCES [User] ([Id]),
	CONSTRAINT FK_Issue_Project
		FOREIGN KEY ([ProjectId])
		REFERENCES [Project] ([Id])
);
GO

