# Cosmos DB Studio

A tool to browse and query Azure Cosmos DB databases.

I created this app out of frustration at the completely broken experience in Azure Storage Explorer (which is basically the
same as Data Explorer in the Azure portal, with the same limitations and more bugs).

It's a bit rough around the edges, and some features are still missing, but it's mostly functional.

## Installation

The app should soon be available on the Windows Store.

In the meantime, you can build from source. You will need the .NET Core 3.1 SDK. Clone the repo, navigate to src/CosmosDbStudio
and run `dotnet build -c Release`.

## Getting started

- Start the app
- Right click in the left pane to add a Cosmos DB account. Enter a name, the account endpoint and key, and an optional folder
  to organize your account. (if you already have a Cosmos DB connection string in the clipboard, the endpoint and key will be
  filled automatically)
- Expand the account node in the treeview to select a database (or create a new one from the context menu)
- Expand the database node in the treeview to select a container (or create a new one from the context menu)
- Right-click the container node and select "New query sheet". This will create a new SQL query sheet for this container.
- Enter a valid Cosmos DB SQL query and hit Ctrl-Enter. It will select the whole query and execute it.
- The results will appear in the bottom pane

**Tip**: you can have
  multiple queries in the same query sheet, just separate them with an empty line.
