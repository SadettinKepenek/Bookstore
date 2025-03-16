MIGRATION_NAME ?= $(shell date +%Y%m%d_%H%M%S)
PSQL_CONTAINER_NAME := bookstore-db
DB_USERNAME ?= postgres
DB_NAME ?= postgres

run-docker-compose:
	echo "Starting Docker containers..."
	docker-compose up -d

migrate-book-db:
	echo "Generating book migration..."
	dotnet ef migrations add $(MIGRATION_NAME) --project ./Book.Infrastructure/Book.Infrastructure.csproj --startup-project ./BookStore.Api/BookStore.Api.csproj --context Book.Infrastructure.Persistent.EntityFrameworkCore.BookDbContext --configuration Debug --output-dir Migrations

migrate-order-db:
	echo "Generating order migration..."
	dotnet ef migrations add $(MIGRATION_NAME) --project ./Order.Infrastructure/Order.Infrastructure.csproj --startup-project ./BookStore.Api/BookStore.Api.csproj --context Order.Infrastructure.Persistent.EntityFrameworkCore.OrderDbContext --configuration Debug --output-dir Migrations

update-book-db:
	echo "Updating book database..."
	dotnet ef database update --startup-project ./BookStore.Api --context BookDbContext

update-order-db:
	echo "Updating book database..."
	dotnet ef database update --startup-project ./BookStore.Api --context OrderDbContext

run-app:
	echo "Running application..."
	cd ./Bookstore.Api && dotnet run 

run-tests:
	echo "Running book unit tests..."
	dotnet test ./Book.UnitTests/Book.UnitTests.csproj
	echo "Running order unit tests..."
	dotnet test ./Order.UnitTests/Order.UnitTests.csproj