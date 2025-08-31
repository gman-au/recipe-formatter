using System.Collections.Generic;

namespace Recipe.Formatter.Infrastructure
{
    public interface ITodoistActionGenerator
    {
        string Generate(string recipeName, IEnumerable<string> ingredients);
    }
}