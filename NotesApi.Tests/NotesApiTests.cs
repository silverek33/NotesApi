
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using static NotesApi.Tests.HttpHelpers;

namespace NotesApi.Tests;

public class NotesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public NotesApiTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_and_Login_returns_plaintext_JWT()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "alice@example.com", "P@ssw0rd!");
        token.Should().Contain(".");
    }

    [Fact]
    public async Task Notes_CRUD_is_scoped_to_owner()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "bob@example.com", "P@ssw0rd!");
        client.UseBearer(token);

        var createResp = await client.PostAsJsonAsync("/notes/", new CreateNoteDto("Hello"));
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<NoteDto>();
        created!.Content.Should().Be("Hello");

        var list = await client.GetFromJsonAsync<NoteDto[]>("/notes/");
        list!.Should().ContainSingle().Which.Id.Should().Be(created.Id);

        var one = await client.GetFromJsonAsync<NoteDto>($"/notes/{created.Id}");
        one!.Content.Should().Be("Hello");

        var updResp = await client.PutAsJsonAsync($"/notes/{created.Id}", new UpdateNoteDto("Updated"));
        updResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updResp.Content.ReadFromJsonAsync<NoteDto>();
        updated!.Content.Should().Be("Updated");

        var del = await client.DeleteAsync($"/notes/{created.Id}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var after = await client.GetAsync($"/notes/{created.Id}");
        after.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task User_cannot_access_other_users_notes()
    {
        var client1 = _factory.CreateClient();
        var token1 = await RegisterAndLoginAsync(client1, "carol@example.com", "P@ssw0rd!");
        client1.UseBearer(token1);
        var created = await (await client1.PostAsJsonAsync("/notes/", new CreateNoteDto("Secret")))
                        .Content.ReadFromJsonAsync<NoteDto>();

        var client2 = _factory.CreateClient();
        var token2 = await RegisterAndLoginAsync(client2, "dave@example.com", "P@ssw0rd!");
        client2.UseBearer(token2);

        (await client2.GetAsync($"/notes/{created!.Id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await client2.PutAsJsonAsync($"/notes/{created!.Id}", new UpdateNoteDto("Hacked"))).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await client2.DeleteAsync($"/notes/{created!.Id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);

        var daveList = await client2.GetFromJsonAsync<NoteDto[]>("/notes/");
        daveList!.Should().BeEmpty();
    }

    [Fact]
    public async Task Unauthenticated_requests_get_401()
    {
        var client = _factory.CreateClient();
        (await client.GetAsync("/notes/")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.PostAsJsonAsync("/notes/", new CreateNoteDto("x"))).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.PutAsJsonAsync("/notes/1", new UpdateNoteDto("x"))).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.DeleteAsync("/notes/1")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Bad_requests_return_400()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client, "erin@example.com", "P@ssw0rd!");
        client.UseBearer(token);
        (await client.PostAsJsonAsync("/notes/", new CreateNoteDto(""))).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await client.PutAsJsonAsync("/notes/999", new UpdateNoteDto(""))).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
