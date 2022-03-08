namespace Ada.FirstCatering.API;

public static class RandomHelper
{
    public static string GenerateRandomString(int length, bool includeUpper = true, bool includeLower = true,
        bool includeNumbers = true)
    {
        var chars = "";
        if(!includeUpper && !includeLower && !includeNumbers)
        {
            throw new ArgumentException("At least one character type must be included");
        }

        if (includeUpper)
        {
            chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }
        if (includeLower)
        {
            chars += "abcdefghijklmnopqrstuvwxyz";
        }
        if (includeNumbers)
        {
            chars += "0123456789";
        }
        
        var stringChars = new char[length];

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Shared.Next(chars.Length)];
        }

        var finalString = new string(stringChars);
        return finalString;
    }
    
    public static int GenerateRandomNumber(int min, int max)
    {
        return Random.Shared.Next(min, max);
    }
    
    public static string GenerateRandomPhoneNumber()
    {
        var areaCode = GenerateRandomNumber(100, 999);
        var firstThree = GenerateRandomNumber(100, 999);
        var lastFour = GenerateRandomNumber(1000, 9999);
        var phoneNumber = $"+44{areaCode}-{firstThree}-{lastFour}";
        return phoneNumber;
    }
}