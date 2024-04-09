namespace WebCV.Helpers
{
    public class RandomUniqueName
    {
        public static string GenerateFileName()
        {
            string randomString = GenerateRandomString();
            string currentTimeString = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return $"{randomString}_{currentTimeString}";
        }

        public static string GenerateRandomString()
        {
            int length = 6;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string randomString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
