using System.Text.RegularExpressions;

namespace LoginApp.Maui.Helpers
{ 
    public static class ValidationHelper
    {
        public static bool IsValidUntarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Format: nama.nim@stu.untar.ac.id
            var regex = new Regex(@"^[a-z0-9]+\.\d{9}@stu\.untar\.ac\.id$", RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
    }
}