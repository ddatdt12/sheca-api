using System.ComponentModel.DataAnnotations;

namespace Sheca.Attributes
{
    public class MinValue : ValidationAttribute
    {
        private readonly int _minValue;

        public MinValue(int minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object? value)
        {
            return value == null || (int)value >= _minValue;
        }
    }
}
