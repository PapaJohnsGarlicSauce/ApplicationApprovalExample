namespace ApplicationApproval.Models
{
    public interface IRegexClass<T>
    {
        T? GetFromFile(string fileContent);
    }
}
