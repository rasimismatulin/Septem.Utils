using System.Text;

namespace Septem.Utils.Helpers.CodeGeneration
{
    public class StaticCodeGenerator : IRandomCodeGenerator
    {
        public string Generate(int length)
        {
            var generated = new StringBuilder();

            for (var i = 0; i < length; i++)
                generated.Append(i);

            return generated.ToString();
        }
    }
}
