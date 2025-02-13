namespace MoneyControl.Core.Entities;

public class CategoryEntity
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
}