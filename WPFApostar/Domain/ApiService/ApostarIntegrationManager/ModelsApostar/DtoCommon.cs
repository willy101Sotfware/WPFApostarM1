namespace WPFApostar.Domain.ApiService.ApostarIntegrationManager.ModelsApostar;

public class DtoCommon
{
    public int Id { get; set; }
    public int IdUserCreated { get; set; }
    public string UserCreated { get; set; }
    public DateTime? DateCreated { get; set; }
    public int IdUserUpdated { get; set; }
    public string UserUpdated { get; set; }
    public DateTime? DateUpdated { get; set; }

}
