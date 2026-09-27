namespace Models.DTO;
public class GstUsrInfoDbDto
{
    public int NrSeededAttractions { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;
    public int NrAttractionsWithAddress { get; set; } = 0;

    public int NrSeededAddresses { get; set; } = 0;
    public int NrUnseededAddresses { get; set; } = 0;

    public int NrSeededUsers { get; set; } = 0;
    public int NrUnseededUsers { get; set; } = 0;

    public int NrSeededCategories {get; set;} = 0;
    public int NrUnSeededCategories {get; set;} = 0;

    public int NrSeededReviews { get; set; } = 0;
    public int NrUnseededReviews { get; set; } = 0;

    public int NrCities { get; set; } = 0;
    public int NrUsers {get; set;} = 0;
    public int NrAttractions {get; set;} = 0;
}

public class GstUsrInfoAttractionsDto
{
    public string Country { get; set; } = null;
    public string City { get; set; } = null;
    public string Category {get; set;} = null;
    public int NrAttractions { get; set; } = 0;
}

public class GstUsrInfoUsersDto
{
    public string FirstName { get; set; } = null;
    public string LastName { get; set; } = null;
    public int NrUsers { get; set; } = 0;
}

public class GstUsrInfoCitiesDto
{
    public string Country { get; set; } = null;
    public int NrCities { get; set; } = 0;
}

public class GstUsrInfoAllDto
{
    public GstUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoAttractionsDto> Attractions { get; set; } = null;
    public List<GstUsrInfoUsersDto> Users { get; set; } = null;
    public List<GstUsrInfoCitiesDto> Cities { get; set; } = null;
}

