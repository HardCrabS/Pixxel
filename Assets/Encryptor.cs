using System.Text;

public class Encryptor
{
    public static string EncryptDecrypt(string input, int key)
    {
        StringBuilder outSB = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            char ch = (char)(input[i] ^ key);
            outSB.Append(ch);
        }
        return outSB.ToString();
    }
}