using System.Threading.Tasks;

namespace Recipe.Formatter.Infrastructure
{
    public interface IQrCodeGenerator
    {
        Task<string> GenerateAsync(string value);
    }
}