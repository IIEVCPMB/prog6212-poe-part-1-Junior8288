Here is the link to the MicroApi for the project
GitLink: https://github.com/Junior8288/MicroAPI.git

This is the sql database needed for the project
--------------------------------------
            SQL Database
--------------------------------------

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ProgPart1')
BEGIN
	ALTER DATABASE ProgPart1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE ProgPart1;
END
GO

CREATE DATABASE ProgPart1;
GO

USE ProgPart1;
GO

CREATE TABLE HR (
    HrId INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    StaffNumber VARCHAR(50) UNIQUE NOT NULL,
	Description VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE ClaimStatuses (
    StatusId INT PRIMARY KEY IDENTITY(1,1),
    StatusName VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO ClaimStatuses (StatusName) VALUES
('Pending'), ('Approved'), ('Rejected'), ('In Review');

CREATE TABLE Claims (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Month VARCHAR(50) NOT NULL,
    Taught VARCHAR(255) NOT NULL,
    LecturerId INT NOT NULL,
    Amount INT NOT NULL,
    HourlyRate DECIMAL(10, 2) NOT NULL,
    Description VARCHAR(MAX) NULL,
    SubmittedDate DATETIME DEFAULT GETDATE(),
    SubmittedBy VARCHAR(100) NOT NULL,
    StatusId INT NOT NULL,
    ReviewedBy VARCHAR(100) NULL,
    ReviewedDate DATETIME NULL,
    FileName VARCHAR(255) NULL,
    CoordinatorStatus VARCHAR(50) DEFAULT 'Pending',
    ManagerStatus VARCHAR(50) DEFAULT 'Pending',

    FOREIGN KEY (LecturerId) REFERENCES HR(HrId),
    FOREIGN KEY (StatusId) REFERENCES ClaimStatuses(StatusId)
);

CREATE TABLE UploadedDocuments (
    DocumentId INT PRIMARY KEY IDENTITY(1,1),
    ClaimId INT NOT NULL,
    FileName VARCHAR(255) NOT NULL,
    FilePath VARCHAR(500) NOT NULL,
    FileSize BIGINT DEFAULT 1,
    UploadedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ClaimId) REFERENCES Claims(Id) ON DELETE CASCADE
);

CREATE TABLE ClaimReviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    ClaimId INT NOT NULL,
    ReviewerName VARCHAR(100) NOT NULL,
    ReviewDate DATETIME DEFAULT GETDATE(),
    Decision VARCHAR(50) NOT NULL,
    Comments VARCHAR(MAX) NULL,
    FOREIGN KEY (ClaimId) REFERENCES Claims(Id) ON DELETE CASCADE
);


INSERT INTO HR (FullName, Email, StaffNumber, Description) VALUES
('Alice Lecturer', 'alice.l@uni.com', 'Lec001', 'Teaching Staff'),
('Bob Manager', 'bob.m@uni.com', 'Mgr005', 'Department Manager'),
('Charlie HR', 'charlie.h@uni.com', 'HR101', 'HR Coordinator');

PRINT 'Sample HR data (Lecturer, Manager, HR) inserted successfully.';
GO

----------------------------------------
                END
----------------------------------------
This a powerpoint on the updated part 2 and 3 of the poe. This will give you a idea of how i 
done it.           
[Claims System.pptx](https://github.com/user-attachments/files/23673288/Claims.System.pptx)
