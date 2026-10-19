namespace API.Shared
{
  public class RoleWithPermissionsDto : LookupDetailsDto
  {
    public List<int> PermissionsIds { get; set; } = new List<int>();

  }
}
