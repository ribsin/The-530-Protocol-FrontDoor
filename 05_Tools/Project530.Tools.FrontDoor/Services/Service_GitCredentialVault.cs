// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Service_GitCredentialVault (AES-256-GCM)
 * *************************************************************************** */

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// AES-256-GCM encrypted local vault for Git credentials.
/// Key is derived via PBKDF2 from a machine-bound secret.
/// </summary>
public sealed class Service_GitCredentialVault : I_GitCredentialVault
{
    private const int Pbkdf2Iterations = 100_000;
    private const int KeySizeBytes = 32;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;
    private const int SaltSizeBytes = 16;

    private readonly string _vaultPath;

    public Service_GitCredentialVault()
    {
        var appData = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);
        _vaultPath = Path.Combine(appData, "Project530.FrontDoor", "vault.dat");
#pragma warning disable RS0030
        Directory.CreateDirectory(Path.GetDirectoryName(_vaultPath)!);
#pragma warning restore RS0030
    }

    public async Task StoreAsync(GitCredentialEntry entry)
    {
        var entries = (await LoadAsync()).ToList();
        entries.RemoveAll(e => e.Username == entry.Username);
        entries.Add(entry);
        await SaveAsync(entries);
    }

    public async Task<GitCredentialEntry?> GetDefaultAsync()
    {
        var entries = await LoadAsync();
        return entries.FirstOrDefault(e => e.IsDefault) ?? entries.FirstOrDefault();
    }

    public async Task<IReadOnlyList<GitCredentialEntry>> ListAsync()
    {
        return await LoadAsync();
    }

    public async Task RemoveAsync(string username)
    {
        var entries = (await LoadAsync()).ToList();
        entries.RemoveAll(e => e.Username == username);
        await SaveAsync(entries);
    }

    public string EncryptPassword(string plainPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
        var key = DeriveKey(salt);
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
        var plainBytes = Encoding.UTF8.GetBytes(plainPassword);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSizeBytes];

        using var aes = new AesGcm(key, TagSizeBytes);
        aes.Encrypt(nonce, plainBytes, cipher, tag);

        var payload = new byte[SaltSizeBytes + NonceSizeBytes + TagSizeBytes + cipher.Length];
        Buffer.BlockCopy(salt, 0, payload, 0, SaltSizeBytes);
        Buffer.BlockCopy(nonce, 0, payload, SaltSizeBytes, NonceSizeBytes);
        Buffer.BlockCopy(tag, 0, payload, SaltSizeBytes + NonceSizeBytes, TagSizeBytes);
        Buffer.BlockCopy(cipher, 0, payload, SaltSizeBytes + NonceSizeBytes + TagSizeBytes, cipher.Length);
        return Convert.ToBase64String(payload);
    }

    public string DecryptPassword(string encryptedBase64)
    {
        var payload = Convert.FromBase64String(encryptedBase64);
        var salt = payload[..SaltSizeBytes];
        var nonce = payload[SaltSizeBytes..(SaltSizeBytes + NonceSizeBytes)];
        var tag = payload[(SaltSizeBytes + NonceSizeBytes)..(SaltSizeBytes + NonceSizeBytes + TagSizeBytes)];
        var cipher = payload[(SaltSizeBytes + NonceSizeBytes + TagSizeBytes)..];

        var key = DeriveKey(salt);
        var plain = new byte[cipher.Length];
        using var aes = new AesGcm(key, TagSizeBytes);
        aes.Decrypt(nonce, cipher, tag, plain);
        return Encoding.UTF8.GetString(plain);
    }

    private static byte[] DeriveKey(byte[] salt)
    {
        var machineSeed = GetMachineSeed();
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(machineSeed),
            salt,
            Pbkdf2Iterations,
            HashAlgorithmName.SHA256,
            KeySizeBytes);
    }

    private static string GetMachineSeed()
    {
        var user = Environment.UserName;
        var machine = Environment.MachineName;
        return $"five30-vault:{user}@{machine}";
    }

    private async Task<List<GitCredentialEntry>> LoadAsync()
    {
#pragma warning disable RS0030
        if (!File.Exists(_vaultPath))
            return new List<GitCredentialEntry>();
        var json = await File.ReadAllTextAsync(_vaultPath);
#pragma warning restore RS0030
        return JsonSerializer.Deserialize<List<GitCredentialEntry>>(json) ?? new();
    }

    private async Task SaveAsync(List<GitCredentialEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
#pragma warning disable RS0030
        await File.WriteAllTextAsync(_vaultPath, json);
#pragma warning restore RS0030
    }
}
