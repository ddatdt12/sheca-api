using System.ComponentModel.DataAnnotations;

namespace Sheca.Attributes
{
    public class MinValue : ValidationAttribute
    {
        private readonly int _minValue;

        public MinValue(int maxValue)
        {
            _minValue = maxValue;
        }

        public override bool IsValid(object? value)
        {
            return value != null && (int)value <= _minValue;
        }
    }
}
