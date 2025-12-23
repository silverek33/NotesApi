
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace NotesApi.Tests;

public static class HttpHelpers
{
    public record RegisterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);
    public record NoteDto(int Id, string Content);
    public record CreateNoteDto(string Content);
    public record UpdateNoteDto(string Content);

    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string email, string password)
    {
        var reg = await client.PostAsJsonAsync("/register/", new RegisterDto(email, password));
        if (!reg.IsSuccessStatusCode && reg.StatusCode != System.Net.HttpStatusCode.Conflict)
        {
            var body = await reg.Content.ReadAsStringAsync();
            throw new Xunit.Sdk.XunitException($"Register failed: {(int)reg.StatusCode} {body}");
        }
        var login = await client.PostAsJsonAsync("/login/", new LoginDto(email, password));
        login.EnsureSuccessStatusCode();
        login.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
        var token = await login.Content.ReadAsStringAsync();
        token.Should().NotBeNullOrWhiteSpace();
        return token;
    }

    public static void UseBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
