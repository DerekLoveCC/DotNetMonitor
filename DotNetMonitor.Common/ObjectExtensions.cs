namespace DotNetMonitor.Common
{
    public static class ObjectExtensions
    {
        public static int? ToNullableInt(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (int.TryParse(obj.ToString(), out int result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}