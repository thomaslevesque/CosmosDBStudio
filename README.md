# Cosmos DB Studio

![Application icon](assets/cosmosdb-small.png)

A tool to browse and query Azure Cosmos DB databases.

![Screenshot](assets/screenshots/CosmosDBStudio-screenshot.png)

I created this app out of frustration with the completely broken experience in Azure Storage Explorer (which is basically the
same as Data Explorer in the Azure portal, with the same limitations and many more bugs).

It's a bit rough around the edges, and some features are still missing, but it's mostly functional.

## Installation

The app is available [on the Windows Store](https://www.microsoft.com/en-us/p/cosmos-db-studio/9mxmw2k8j04h).

## Getting started

- Start the app
- Right click in the left pane to add a Cosmos DB account. Enter a name, the account endpoint and key, and an optional folder
  to organize your account. (If you have a Cosmos DB connection string in the clipboard, the endpoint and key will be
  filled automatically)
- Expand the account node in the treeview to select a database (or create a new one from the context menu)
- Expand the database node in the treeview to select a container (or create a new one from the context menu)
- Right-click the container node and select "New query sheet". This will create a new SQL query sheet for this container.
- Enter a valid Cosmos DB SQL query and hit Ctrl-Enter. It will select the whole query and execute it.
- The results will appear in the bottom pane

**Tip**: you can have multiple queries in the same query sheet, just separate them with an empty line.
