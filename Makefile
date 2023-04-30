all : clean restore build

clean:
	dotnet clean
	
restore:
	dotnet restore
	
build:
	dotnet build Loxnet.Tools
	dotnet run --project Loxnet.Tools Loxnet
	dotnet build
	
run:
	dotnet run