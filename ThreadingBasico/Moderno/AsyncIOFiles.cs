using System.Text.Json;

namespace ThreadingBasico.Moderno
{
    public class Account
    {
        public string? Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class AsyncIOFiles
    {
        public static async Task WorkAsync()
        {
            // Combine a directory and file name, then create the directory if it doesn't exist
            string directoryPath = @"TempDir";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = "account.json";
            string filePath = Path.Combine(directoryPath, fileName);

            Account account = new Account { Name = "Elize Harmsen", Balance = 1000.00m };

            // Save account data to a file asynchronously
            await SaveAccountDataAsync(filePath, account);

            // Load account data from the file asynchronously
            Account loadedAccount = await LoadAccountDataAsync(filePath);
            Console.WriteLine($"Name: {loadedAccount.Name}, Balance: {loadedAccount.Balance}");
        }

        public static async Task SaveAccountDataAsync(string filePath, Account account)
        {
            string jsonString = JsonSerializer.Serialize(account);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public static async Task<Account> LoadAccountDataAsync(string filePath)
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Account>(jsonString);// ?? new Account { Name = "Deserialization Error", Balance = 0M };
        }
    }
}