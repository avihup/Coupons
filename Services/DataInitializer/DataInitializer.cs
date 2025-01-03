﻿using MongoDB.Driver;
using TestCase.Interfaces.DataInitializer;
using TestCase.Models.Database;

namespace TestCase.Services.DataInitializer
{
    public class DataInitializer : IDataInitializer
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<DataInitializer> _logger;

        public DataInitializer(IMongoClient mongoClient, ILogger<DataInitializer> logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var database = _mongoClient.GetDatabase("CouponsDb");
            var clientsCollection = database.GetCollection<ClientDto>("Clients");
            var usersCollection = database.GetCollection<UserDto>("Users");
            var machinesCollection = database.GetCollection<MachineDto>("Machines");
            var kioskCollection = database.GetCollection<KioskDto>("Kiosks");

            //Add system admin
            if (await usersCollection.CountDocumentsAsync(x=> !x.ClientId.HasValue) == 0)
            {
                var admin = new UserDto
                {
                    UserName = "systemadmin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("systemadmin123"),
                };
                await usersCollection.InsertOneAsync(admin);
            }



            // Check if initialization is needed
            if (await clientsCollection.CountDocumentsAsync(FilterDefinition<ClientDto>.Empty) > 0)
            {
                _logger.LogInformation("Database already initialized");
                return;
            }

            // Create initial client
            var client = new ClientDto
            {
                Id = Guid.NewGuid(),
                Name = "Default Client",
            };
            await clientsCollection.InsertOneAsync(client);

            // Create initial user
            var user = new UserDto
            {
                ClientId = client.Id,
                UserName = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            };
            await usersCollection.InsertOneAsync(user);


            // add initial machine
            var machine = new MachineDto
            {
                ClientId = client.Id,
                Name = "test machine",
                AccessToken = Guid.NewGuid().ToString(),
            };
            await machinesCollection.InsertOneAsync(machine);
            // add initial kiosk
            var kiosk = new KioskDto
            {
                ClientId = client.Id,
                Name = "test kiosk",
                AccessToken = Guid.NewGuid().ToString(),
            };
            await kioskCollection.InsertOneAsync(kiosk);

            _logger.LogInformation("Database initialized with default client and admin user");
        }
    }
}
