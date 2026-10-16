
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.API.Tests;

public class RaceDayApiTests
{
    private static async Task<string> RegisterAndLoginParticipant(
        HttpClient client, string email)
    {
        var register = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                Name = "Test Participant",
                Email = email,
                Password = "ParticipantPass123!",
                Contact_Info = "Test contact",
                User_Type = "Organizer"
            });

        register.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = email,
                Password = "ParticipantPass123!"
            });

        login.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(
            await login.Content.ReadAsStringAsync());

        return json.RootElement.GetProperty("token").GetString()!;
    }

    private static async Task SeedOrganizer(
        RaceDayApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        var organizer = new User
        {
            Name = "Test Organizer",
            Email = "organizer@test.com",
            Contact_Info = "Test contact",
            User_Type = "Organizer"
        };

        organizer.Password = hasher.HashPassword(
            organizer, "OrganizerPass123!");

        db.Users.Add(organizer);
        await db.SaveChangesAsync();
    }

    private static async Task<string> LoginOrganizer(
        HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = "organizer@test.com",
                Password = "OrganizerPass123!"
            });

        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        return json.RootElement.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task Register_ValidDetails_ReturnsSuccess()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                Name = "Jane",
                Email = "jane@test.com",
                Password = "Password123!",
                Contact_Info = "0123456789",
                User_Type = "Organizer"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Participant", body);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        await RegisterAndLoginParticipant(client, "duplicate@test.com");

        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                Name = "Another User",
                Email = "duplicate@test.com",
                Password = "Password123!"
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        await RegisterAndLoginParticipant(client, "login@test.com");

        var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                Email = "login@test.com",
                Password = "WrongPassword123!"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRace_Unauthenticated_ReturnsUnauthorized()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/Races",
            new
            {
                Name = "Test Race",
                Location = "Johannesburg",
                StartTime = "08:00",
                EndTime = "10:00",
                Distance_km = 5,
                Type = "Running"
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRace_ParticipantToken_ReturnsForbidden()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        var token = await RegisterAndLoginParticipant(
            client, "participant@test.com");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/Races",
            new
            {
                Name = "Test Race",
                Location = "Johannesburg",
                StartTime = "08:00",
                EndTime = "10:00",
                Distance_km = 5,
                Type = "Running"
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateRace_OrganizerToken_ReturnsCreated()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        await SeedOrganizer(factory);
        var token = await LoginOrganizer(client);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/Races",
            new
            {
                Name = "Organizer Race",
                Location = "Johannesburg",
                StartTime = "08:00",
                EndTime = "10:00",
                Distance_km = 10,
                Type = "Running"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CanEnrol_AndRegistrationIsRecorded()
    {
        using var factory = new RaceDayApiFactory();
        using var client = factory.CreateClient();

        var token = await RegisterAndLoginParticipant(
            client, "enrolment@test.com");

        int participantId;
        int raceId;

        // Prepare existing race and participant in the test database.
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var participant = new Participant
            {
                Name = "Test Participant",
                Gender = "Other",
                TeamName = "Test Team"
            };

            var race = new Race
            {
                Name = "Enrolment Test Race",
                Location = "Johannesburg",
                StartTime = "08:00",
                EndTime = "10:00",
                Distance_km = 5,
                Type = "Running"
            };

            db.Participants.Add(participant);
            db.Races.Add(race);
            await db.SaveChangesAsync();

            participantId = participant.ParticipantID;
            raceId = race.RaceID;
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var enrolment = new
        {
            Bib_Number = (int?)null,
            status = "Registered",
            RegistrationDate = DateTime.UtcNow,
            ParticipantID = (int?)participantId,
            RaceID = (int?)raceId
        };

        var response = await client.PostAsJsonAsync(
            "/api/Registrations", enrolment);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var responseJson = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var registrationId = responseJson.RootElement
            .GetProperty("registrationID").GetInt32();

        var getResponse = await client.GetAsync(
            $"/api/Registrations/{registrationId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using var savedJson = JsonDocument.Parse(
            await getResponse.Content.ReadAsStringAsync());

        Assert.Equal(
            participantId,
            savedJson.RootElement.GetProperty("participantID").GetInt32());

        Assert.Equal(
            raceId,
            savedJson.RootElement.GetProperty("raceID").GetInt32());
    }
}
