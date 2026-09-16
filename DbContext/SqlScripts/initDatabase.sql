USE [sql-travel];
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'gstusr')
    EXEC('CREATE SCHEMA gstusr');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'usr')
    EXEC('CREATE SCHEMA usr');
GO

-- create a view that gives overview of the database content
CREATE OR ALTER VIEW gstusr.vwInfoDb AS
    SELECT (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 1) as nrSeededAttractions, 
        (SELECT COUNT(*) FROM supusr.Attractions WHERE Seeded = 0) as nrUnseededAttractions,
        (SELECT COUNT(*) FROM supusr.Attractions WHERE AddressId IS NOT NULL) as nrAttractionsWithAddress,
        (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 1) as nrSeededAddresses, 
        (SELECT COUNT(*) FROM supusr.Addresses WHERE Seeded = 0) as nrUnseededAddresses,
        (SELECT COUNT(*) FROM supusr.Users WHERE Seeded = 1) as nrSeededUsers, 
        (SELECT COUNT(*) FROM supusr.Users WHERE Seeded = 0) as nrUnseededUsers,
        (SELECT COUNT(*) FROM supusr.Reviews WHERE Seeded = 1) as nrSeededReviews, 
        (SELECT COUNT(*) FROM supusr.Reviews WHERE Seeded = 0) as nrUnseededReviews,
        (SELECT COUNT(*) FROM supusr.Categories WHERE Seeded = 1) as nrSeededCategories,
        (SELECT COUNT(*) FROM supusr.Categories WHERE Seeded = 0) as nrUnseededCategories;
GO

-- create deleteAll procedure
CREATE OR ALTER PROC supusr.spDeleteAll
    @seededParam BIT = 1,

    @nrAttractionsAffected INT OUTPUT,
    @nrAddressesAffected INT OUTPUT,
    @nrUsersAffected INT OUTPUT,
    @nrCategoriesAffected INT OUTPUT,
    @nrReviewsAffected INT OUTPUT

    AS 

    SET NOCOUNT ON;

    SELECT @nrAttractionsAffected = COUNT(*) FROM supusr.Attractions WHERE Seeded = @seededParam;
    SELECT @nrAddressesAffected = COUNT(*) FROM supusr.Addresses WHERE Seeded = @seededParam;
    SELECT @nrUsersAffected = COUNT(*) FROM supusr.Users WHERE Seeded = @seededParam;
    SELECT @nrCategoriesAffected = COUNT(*) FROM supusr.Categories WHERE Seeded = @seededParam;
    SELECT @nrReviewsAffected = COUNT(*) FROM supusr.Reviews WHERE Seeded = @seededParam;

    ;THROW 999999, 'Error occurred in supusr.spDeleteAll', 1

    SELECT * FROM gstusr.vwInfoDb;
GO
