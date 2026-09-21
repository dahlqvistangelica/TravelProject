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

CREATE OR ALTER VIEW gstusr.vwInfoAttractions AS 
    SELECT co.Name AS CountryName, ci.Name AS CityName, c.CategoryName, COUNT(*) as NrAttractions FROM supusr.Attractions AS att
    INNER JOIN supusr.Addresses a ON att.AddressId = a.AddressId
    INNER JOIN supusr.Cities ci ON ci.CityId = a.CityId
    INNER JOIN supusr.Countries co ON co.CountryId = ci.CountryId
    INNER JOIN supusr.Categories c on att.CategoryId = c.CategoryId
    GROUP BY co.Name, ci.Name, c.CategoryName;
GO

CREATE OR ALTER VIEW gstusr.vwInfoUsers AS
    SELECT u.FirstName, u.LastName, COUNT(*) AS NrUsers FROM supusr.Users as u
    GROUP BY u.FirstName, u.LastName WITH ROLLUP;
GO

CREATE OR ALTER VIEW gstusr.vwInfoCities AS
    SELECT co.Name as CountryName, ci.Name AS CityName, COUNT(*) AS NrCities FROM supusr.Cities AS ci
    INNER JOIN supusr.Countries co ON co.CountryId = ci.CountryId
    GROUP BY co.Name, ci.Name;
GO

-- create deleteAll procedure
CREATE OR ALTER PROC supusr.spDeleteAll
    @seededParam BIT = 1,

    @nrAttractionsAffected INT OUTPUT,
    @nrAddressesAffected INT OUTPUT,
    @nrUsersAffected INT OUTPUT,
    @nrCategoriesAffected INT OUTPUT,
    @nrReviewsAffected INT OUTPUT,
    @nrCitiesAffected INT OUTPUT,
    @nrCountriesAffected INT OUTPUT

    AS 

    SET NOCOUNT ON;

    SELECT @nrAttractionsAffected = COUNT(*) FROM supusr.Attractions WHERE Seeded = @seededParam;
    SELECT @nrAddressesAffected = COUNT(*) FROM supusr.Addresses WHERE Seeded = @seededParam;
    SELECT @nrUsersAffected = COUNT(*) FROM supusr.Users WHERE Seeded = @seededParam;
    SELECT @nrCategoriesAffected = COUNT(*) FROM supusr.Categories WHERE Seeded = @seededParam;
    SELECT @nrReviewsAffected = COUNT(*) FROM supusr.Reviews WHERE Seeded = @seededParam;
    SELECT @nrCitiesAffected = COUNT(*) FROM supusr.Cities WHERE Seeded = @seededParam;
    SELECT @nrCountriesAffected = COUNT(*) FROM supusr.Countries WHERE Seeded = @seededParam;

    DELETE FROM supusr.Reviews WHERE Seeded = @seededParam;
    DELETE FROM supusr.Users WHERE Seeded = @seededParam;
    DELETE FROM supusr.Attractions WHERE Seeded = @seededParam;
    DELETE FROM supusr.Addresses WHERE Seeded = @seededParam;
    DELETE FROM supusr.Categories WHERE Seeded = @seededParam;
    DELETE FROM supusr.Cities WHERE Seeded = @seededParam;
    DELETE FROM supusr.Countries WHERE Seeded = @seededParam;
    

    SELECT * FROM gstusr.vwInfoDb;
GO
