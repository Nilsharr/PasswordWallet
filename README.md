# Password Wallet

The Password Wallet is an application designed to store and manage user passwords.
The backend is written in C# using ASP.NET Core and FastEndpoints library for building the API.
The frontend is developed with Angular. The application uses PostgreSQL as the database.

## Features

- Add, update, delete, and view saved passwords.
- All passwords are stored encrypted to ensure security.
- Passwords are organized into folders for better management.
- Rearrange item positions with drag and drop.
- Generate secure passwords.
- View login history.

## Run with docker

1. Make sure you have [Docker](https://www.docker.com/) installed.
1. Clone the repository.
1. Generate self signed certificate for ASP.NET Core image https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https.
1. Set the `ASPNETCORE_Kestrel__Certificates__Default__Password` variable in docker-compose.yml to the password used for the certificate.
1. (Optional) Trust the self signed certificate ca.crt in PasswordWallet.Client/cert directory.
1. Run `docker compose up` command in project root folder.
1. Navigate to https://localhost:8000.

## Integration tests

The app uses Testcontainers library to run the test database.
Docker is required to run integration tests.

## Screenshots

<img src="./resources/password_wallet_main.png" alt="Password Wallet" width="800" />
